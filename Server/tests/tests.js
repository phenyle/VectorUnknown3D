// Tests for Code.gs, run in a browser with mocks.js. Results go to window.TEST_RESULTS.
const results = [];
function check(name, cond, detail) { results.push({ name, pass: !!cond, detail: cond ? '' : String(detail === undefined ? '' : JSON.stringify(detail)) }); }
function call(body) { return JSON.parse(doPost({ postData: { contents: JSON.stringify(body) } }).getContent()); }
const hex = b64 => Array.from(atob(b64), c => c.charCodeAt(0).toString(16).padStart(2, '0')).join('');

function runTests() {
  results.length = 0;

  // PBKDF2-HMAC-SHA256 published test vectors (password "password", salt "salt", 32-byte output)
  check('pbkdf2 c=1', hex(pbkdf2('password', 'salt', 1)) === '120fb6cffcf8b32c43e7225256c4f837a86548c92ccc35480805987cb70be17b', hex(pbkdf2('password', 'salt', 1)));
  check('pbkdf2 c=2', hex(pbkdf2('password', 'salt', 2)) === 'ae4d0c95af6b46d32d0adff928f06dd02a303f8ef3c251dfd6e2d85a95474c43', hex(pbkdf2('password', 'salt', 2)));
  check('pbkdf2 c=4096', hex(pbkdf2('password', 'salt', 4096)) === 'c5e478d59288c841aa530db6845c4c8d962893a001ce4e11a4963873aa98134a', hex(pbkdf2('password', 'salt', 4096)));

  // the algorithm is proven by the vectors above; the slow pure-JS HMAC makes 5000 rounds per login too slow here
  CONFIG.HASH_ITERATIONS = 20;
  resetMocks();
  setup();
  check('setup creates tabs', ['users', 'auth', 'sessions', 'attempts', 'records', 'Students', 'Overview', 'Data dictionary'].every(n => MOCK_SS.sheets[n]), Object.keys(MOCK_SS.sheets));
  check('retired tabs not created', !['statistics', 'bunny', 'bunRecords', 'p1_1', 'p4_3'].some(n => MOCK_SS.sheets[n]), Object.keys(MOCK_SS.sheets));
  check('auth and records hidden', MOCK_SS.sheets.auth.hidden === true && MOCK_SS.sheets.records.hidden === true);
  setup();
  check('report trigger installed once', MOCK_TRIGGERS.length === 1 && MOCK_TRIGGERS[0].fn === 'buildReports' && MOCK_TRIGGERS[0].minutes === 15, MOCK_TRIGGERS);

  // ---- account creation and IDs
  const a1 = call({ action: 'createAccount', first: 'Ashish', last: 'Amresh', password: 'correct horse', school: 'Flagstaff High', education: '9th grade' });
  check('create ok', a1.ok && a1.username === 'ashamr' && /^[0-9a-f]{64}$/.test(a1.token), a1);
  const a2 = call({ action: 'createAccount', first: 'Ashley', last: 'Amrein', password: 'another pass', school: 'X', education: 'Y' });
  check('collision gets suffix 2', a2.ok && a2.username === 'ashamr2', a2);
  const a3 = call({ action: 'createAccount', first: 'ash', last: 'AMR', password: 'third pass!', school: 'X', education: 'Y' });
  check('collision gets suffix 3', a3.username === 'ashamr3', a3);
  const a4 = call({ action: 'createAccount', first: 'José', last: 'Núñez-García', password: 'acentos123', school: 'X', education: 'Y' });
  check('accents stripped', a4.username === 'josnun', a4);
  const a5 = call({ action: 'createAccount', first: 'Al', last: 'Wu', password: 'shortname1', school: 'X', education: 'Y' });
  check('short names', a5.username === 'alwu', a5);
  check('short password rejected', call({ action: 'createAccount', first: 'Bo', last: 'Lee', password: 'short', school: 'X', education: 'Y' }).ok === false);
  check('missing name rejected', call({ action: 'createAccount', first: '123', last: 'Lee', password: 'longenough', school: 'X', education: 'Y' }).ok === false);

  const allCells = JSON.stringify(MOCK_SS.sheets.users.data) + JSON.stringify(MOCK_SS.sheets.auth.data);
  check('no real names stored', !/ashish|amresh|ashley|amrein|jos[eé]|n[uú][nñ]ez/i.test(allCells), allCells);
  check('users tab has no hash/salt', !MOCK_SS.sheets.users.data[0].some(h => /hash|salt|first/i.test(h)), MOCK_SS.sheets.users.data[0]);
  check('response has no hash', !JSON.stringify(a1).includes(MOCK_SS.sheets.auth.rows()[0].hash));
  check('school stored', MOCK_SS.sheets.users.rows()[0].school === 'Flagstaff High');

  // ---- login
  const l1 = call({ action: 'login', username: 'ASHAMR ', password: 'correct horse' });
  check('login ok (case/space-insensitive id)', l1.ok && l1.users.username === 'ashamr' && l1.token && l1.users.school === 'Flagstaff High', l1);
  check('login has no hash', !('hash' in l1.users) && !('salt' in l1.users));
  check('wrong password rejected', call({ action: 'login', username: 'ashamr', password: 'nope' }).error === 'Wrong player ID or password.');
  check('unknown id same message', call({ action: 'login', username: 'nobody', password: 'nope' }).error === 'Wrong player ID or password.');
  for (let i = 0; i < 4; i++) call({ action: 'login', username: 'ashamr2', password: 'bad' });
  const locked = call({ action: 'login', username: 'ashamr2', password: 'bad' });
  const lockedRight = call({ action: 'login', username: 'ashamr2', password: 'another pass' });
  check('lockout after 5 failures', /Too many wrong passwords/.test(lockedRight.error), [locked, lockedRight]);
  check('other ids unaffected by lockout', call({ action: 'login', username: 'ashamr3', password: 'third pass!' }).ok);

  const tok = l1.token;
  // ---- saving progress
  const s1 = call({ action: 'saveRow', token: tok, table: 'records', row: { username: 'ashamr2', sessionID: 'hacked', p1_totAttempts: 3, p1_answers: '=IMPORTDATA("http://evil")', p2_1_time: 12.5 } });
  check('save ok', s1.ok, s1);
  let rec = MOCK_SS.sheets.records.rows();
  check('username forced to token owner', rec.length === 1 && rec[0].username === 'ashamr' && rec[0].sessionID === '', rec);
  check('formula stored as text', MOCK_SS.sheets.records.formulas.length === 0 && rec[0].p1_answers === '=IMPORTDATA("http://evil")', MOCK_SS.sheets.records.formulas);
  check('number stays number', rec[0].p2_1_time === 12.5 && rec[0].p1_totAttempts === 3, rec[0]);
  call({ action: 'saveRow', token: tok, table: 'records', row: { p1_totAttempts: 4 } });
  rec = MOCK_SS.sheets.records.rows();
  check('upsert keeps one row per player and other columns', rec.length === 1 && rec[0].p1_totAttempts === 4 && rec[0].p2_1_time === 12.5, rec);

  const obs = { x: 1, y: [2, 3] };
  check('nested object saved', call({ action: 'saveRow', token: tok, table: 'records', row: { p4_1_questions: obs } }).ok);
  check('retired tabs rejected', ['p2_1', 'bunny', 'bunRecords', 'statistics'].every(t => call({ action: 'saveRow', token: tok, table: t, row: { usernameAttempt: 'ashamr.1' } }).ok === false));
  check('bad column name rejected', call({ action: 'saveRow', token: tok, table: 'records', row: { 'a-b': 1 } }).ok === false);
  check('unknown table rejected', call({ action: 'saveRow', token: tok, table: 'auth', row: { hash: 'x' } }).ok === false);
  check('bad token rejected', /expired/.test(call({ action: 'saveRow', token: 'abc', table: 'records', row: { a: 1 } }).error));

  const g1 = call({ action: 'getRow', token: tok, table: 'records' });
  check('getRow records', g1.row.p1_totAttempts === 4 && g1.row.username === 'ashamr', g1);
  const gObs = MOCK_SS.sheets.records.rows()[0].p4_1_questions;
  check('nested object stored as JSON text', gObs === JSON.stringify(obs), gObs);
  check('getRow parses JSON back', JSON.stringify(g1.row.p4_1_questions) === JSON.stringify(obs), g1.row.p4_1_questions);

  // ---- users tab: only safe fields writable
  call({ action: 'saveRow', token: tok, table: 'users', row: { admin: true, tot_time: 42.5, soundVol: 0.3, school: 'changed' } });
  const me = MOCK_SS.sheets.users.rows().find(r => r.username === 'ashamr');
  check('users: admin not writable, settings writable', me.admin === false && me.tot_time === 42.5 && me.soundVol === 0.3 && me.school === 'Flagstaff High', me);

  // ---- sessions
  const future = new Date(Date.now() + 86400000).toISOString();
  check('non-admin cannot create session', /admin/.test(call({ action: 'createSession', token: tok, sessionID: 'class1', isClass: true, expires: future }).error));
  const adminRow = MOCK_SS.sheets.users.data.findIndex(l => l[0] === 'ashamr');
  MOCK_SS.sheets.users.data[adminRow][MOCK_SS.sheets.users.data[0].indexOf('admin')] = 'TRUE';
  const cs = call({ action: 'createSession', token: tok, sessionID: 'Class1', isClass: true, isBunny: true, expires: future, forceStage: true, stageNum: 2, difficultyNum: 3 });
  check('admin creates session (TRUE typed as text)', cs.ok && cs.session.sessionID === 'class1', cs);
  check('bad session name rejected', call({ action: 'createSession', token: tok, sessionID: 'bad name!', expires: future }).ok === false);
  const j = call({ action: 'joinSession', sessionID: 'CLASS1' });
  check('join session', j.ok && /^class1-[0-9a-f]{6}$/.test(j.users.username) && j.users.isClass === true && j.users.isBunny === true && j.users.stageNum === 2 && j.users.forceStage === true, j);
  check('session player can save', call({ action: 'saveRow', token: j.token, table: 'records', row: { p1_totAttempts: 1 } }).ok);
  check('session player sessionID stamped', MOCK_SS.sheets.records.rows().find(r => r.username === j.users.username).sessionID === 'class1', MOCK_SS.sheets.records.rows());
  check('session player cannot password-login', call({ action: 'login', username: j.users.username, password: '' }).ok === false);
  const past = new Date(Date.now() - 1000).toISOString();
  check('cannot create session in the past', call({ action: 'createSession', token: tok, sessionID: 'old', expires: past }).ok === false);
  // expire class1 by editing the sheet
  const sRow = MOCK_SS.sheets.sessions.data.findIndex(l => l[0] === 'class1');
  MOCK_SS.sheets.sessions.data[sRow][MOCK_SS.sheets.sessions.data[0].indexOf('expires')] = new Date(Date.now() - 1000);
  check('expired session rejected', /expired/.test(call({ action: 'joinSession', sessionID: 'class1' }).error));
  check('unknown session rejected', call({ action: 'joinSession', sessionID: 'nope' }).ok === false);

  // ---- attempts (teacher data)
  const move = (t, m, mistake) => ({ t, move: m, mistake });
  const att = { game: '2D', activity: 'L1', stage: 0, difficulty: 1, ideal_moves: 1, outcome: 'completed', seconds: 95, score: 3,
    problem: 'Reach ⟨-10, 5⟩ with cards ⟨-1, 1⟩, ⟨0, -1⟩', final_answer: '⟨-10, 5⟩',
    moves: [move(10, '=1+1', 'sign'), move(11.5, '10·⟨-1, 1⟩ + 4·⟨0, -1⟩ = ⟨-10, 6⟩', 'scalar'), move(40, '10·⟨-1, 1⟩ + 5·⟨0, -1⟩ = ⟨-10, 5⟩', 'correct')] };
  const la = call({ action: 'logAttempt', token: j.token, attempt: att });
  check('logAttempt ok', la.ok, la);
  const ar = MOCK_SS.sheets.attempts.rows()[0];
  check('attempt row: player/section from login', ar.player === j.users.username && ar.section === 'class1', ar);
  check('attempt row: labels', ar.activity === '2D Level 1' && ar.difficulty === 'Easy' && ar.concept === 'Linear combination of two vectors', ar);
  check('attempt row: totals', ar.moves === 3 && ar.correct_moves === 1 && ar.sign_errors === 1 && ar.scalar_errors === 1 && ar.other_errors === 0 &&
    ar.first_move_correct === false && ar.rapid_guesses === 1 && ar.efficiency === 0.33 && ar.minutes === 1.6 && ar.score === 3, ar);
  check('attempt row: readable move log', /^1\. 10\.0 s  =1\+1  ✗ sign error\n2\. 11\.5 s .*✗ wrong scalar\n3\. 40\.0 s .*✓$/.test(ar.move_log), ar.move_log);
  check('attempt row: no formulas', MOCK_SS.sheets.attempts.formulas.length === 0, MOCK_SS.sheets.attempts.formulas);
  check('3D concept by stage', call({ action: 'logAttempt', token: tok, attempt: { game: '3D', activity: 'P4', stage: 3, outcome: 'completed', seconds: 60, ideal_moves: 5, efficiency: 0.5, moves: [move(5, 'Target ⟨1, 2, 3⟩', 'correct')] } }).ok &&
    MOCK_SS.sheets.attempts.rows()[1].concept === 'Linear combinations in 3D' && MOCK_SS.sheets.attempts.rows()[1].efficiency === 0.5 &&
    MOCK_SS.sheets.attempts.rows()[1].activity === '3D Puzzle 4, stage 3', MOCK_SS.sheets.attempts.rows()[1]);
  check('bad attempts rejected', [{ game: '2D', activity: 'P1', outcome: 'completed' }, { game: '2D', activity: 'L9', outcome: 'completed' },
    { game: '2D', activity: 'L1', outcome: 'won' }, null].every(a => call({ action: 'logAttempt', token: tok, attempt: a }).ok === false));
  check('logAttempt needs login', call({ action: 'logAttempt', token: 'x', attempt: att }).ok === false);
  check('unknown mistake ignored', call({ action: 'logAttempt', token: tok, attempt: { game: '2D', activity: 'L4', outcome: 'abandoned', seconds: 700, moves: [move(1, 'x', 'hack'), move(2, 'y', 'other')] } }).ok &&
    MOCK_SS.sheets.attempts.rows()[2].moves === 2 && MOCK_SS.sheets.attempts.rows()[2].other_errors === 1 && MOCK_SS.sheets.attempts.rows()[2].efficiency === '', MOCK_SS.sheets.attempts.rows()[2]);

  // ---- reports
  for (let i = 0; i < 3; i++)
    call({ action: 'logAttempt', token: tok, attempt: { game: '3D', activity: 'P2', stage: 1, outcome: 'abandoned', seconds: 30, moves: [move(1, 'shot', 'swap'), move(2, 'shot', 'swap')] } });
  for (let i = 0; i < 2; i++)
    call({ action: 'logAttempt', token: j.token, attempt: Object.assign({}, att, { moves: [move(3, 'right', 'correct')] }) });
  buildReports();
  const st = MOCK_SS.sheets.Students.rows();
  const me2 = st.find(r => r.Player === 'ashamr'), kid = st.find(r => r.Player === j.users.username);
  check('Students: one row per player', st.length === 2, st);
  check('Students: mastery', kid['Linear combination of two vectors'] === 'Mastered' && me2['Coordinates in a non-standard basis'] === 'Not yet' &&
    me2['Linear combinations in 3D'] === 'Developing' && me2['Planning several combinations'] === 'Not attempted', [kid, me2]);
  check('Students: needs attention', me2['Needs attention'] === true && /No completion in 3 plays: Coordinates in a non-standard basis/.test(me2.Why) &&
    /Gave up after 11\.7 min on 2D Level 4/.test(me2.Why) && kid['Needs attention'] === false, [me2.Why, kid.Why]);
  check('Students: most common mistake', me2['Most common mistake'] === 'swapped components or scalars (6)', me2['Most common mistake']);
  check('Students: colors', MOCK_SS.sheets.Students.backgrounds.length === 2 && MOCK_SS.sheets.Students.backgrounds.some(r => r.includes('#b7e1cd')));
  const ov = MOCK_SS.sheets.Overview.rows();
  const all1 = ov.find(r => r.Section === 'All sections' && r.Activity === '2D Level 1');
  check('Overview: all + per section', all1 && all1.Plays === 3 && all1.Completed === 1 && ov.some(r => r.Section === 'class1') && ov.some(r => r.Section === '(no section)'), ov);
  check('Data dictionary written', MOCK_SS.sheets['Data dictionary'].rows().length > 20);

  // ---- startFresh: legacy tabs removed, orphan password rows removed, players kept
  SpreadsheetApp.getActive().insertSheet('statistics');
  SpreadsheetApp.getActive().insertSheet('p4_3');
  MOCK_SS.sheets.auth.appendRow(['tesacc', 'salt', 'hash', 5000, new Date()]);
  const usersBefore = MOCK_SS.sheets.users.rows().length, authBefore = MOCK_SS.sheets.auth.rows().length;
  startFresh();
  check('startFresh removes legacy tabs', !MOCK_SS.sheets.statistics && !MOCK_SS.sheets.p4_3 && MOCK_SS.sheets.records && MOCK_SS.sheets.records.hidden, Object.keys(MOCK_SS.sheets));
  check('startFresh keeps players, drops orphan auth', MOCK_SS.sheets.users.rows().length === usersBefore && MOCK_SS.sheets.auth.rows().length === authBefore - 1 &&
    !MOCK_SS.sheets.auth.rows().some(r => r.username === 'tesacc'), MOCK_SS.sheets.auth.rows().map(r => r.username));
  check('login still works after startFresh', call({ action: 'login', username: 'ashamr3', password: 'third pass!' }).ok);

  // ---- misc
  check('unknown action', call({ action: 'dropTables' }).error === 'Unknown request.');
  const bad = JSON.parse(doPost({ postData: { contents: '{not json' } }).getContent());
  check('bad JSON handled', bad.ok === false && bad.error === 'Server error. Please try again.', bad);
  check('ping', call({ action: 'ping' }).ok);
  check('doGet', JSON.parse(doGet().getContent()).ok);

  window.TEST_RESULTS = { passed: results.filter(r => r.pass).length, failed: results.filter(r => !r.pass) , total: results.length };
  document.body.textContent = JSON.stringify(window.TEST_RESULTS, null, 2);
  return window.TEST_RESULTS;
}
