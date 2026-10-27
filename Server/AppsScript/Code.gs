/**
 * Vector Unknown game server: a Google Apps Script web app bound to the team's Google Sheet.
 *
 * The game never reads or writes the sheet directly. Every request is an HTTP POST whose body is
 * JSON ({"action": "...", ...}). This script checks the request, does the work and answers with
 * JSON: {"ok": true, ...} or {"ok": false, "error": "message to show the player"}.
 *
 * Security model
 * - Passwords are hashed and checked here (PBKDF2-HMAC-SHA256 with a per-player salt). Hashes are
 *   kept in the hidden "auth" tab and are never sent to the game.
 * - Login returns a random session token, kept in the script cache for 6 hours and renewed on use.
 *   Progress writes must carry a token; the script stamps each row with that token's username, so
 *   a player can only write and read their own rows.
 * - Real names are only used to build the player ID (first 3 letters of the first name + first 3
 *   letters of the last name) and are never stored.
 * - Only known tabs and well-formed column names are accepted, and text is written as plain text,
 *   so a value such as "=IMPORTDATA(...)" is never run as a formula.
 * - Teachers/researchers get the admin panel when the team sets "admin" to TRUE in the users tab.
 *
 * Teacher reports
 * - Every play of a 2D level or 3D puzzle is one row in "attempts" (logAttempt): the problem, the
 *   outcome, time, moves, and each move's mistake type. "Students" and "Overview" are rebuilt from
 *   it every 15 minutes (buildReports) and "Data dictionary" explains every column.
 *
 * Setup and deployment: see Server/README.md in the game repository.
 */

var VERSION = '2026.10.2';

var CONFIG = {
  HASH_ITERATIONS: 5000,       // PBKDF2 rounds; each login/sign-up costs about this many HMACs
  TOKEN_TTL_SECONDS: 21600,    // 6 hours, the CacheService maximum (renewed on every request)
  MAX_FAILED_LOGINS: 5,        // wrong passwords before the ID is locked out...
  LOCKOUT_SECONDS: 600,        // ...for 10 minutes
  MAX_SIGNUPS_PER_MINUTE: 60,  // across all players, to blunt scripted account creation
  MIN_PASSWORD_LENGTH: 8,
  MAX_PASSWORD_LENGTH: 128,
  MAX_TEXT_LENGTH: 40000,      // Google Sheets cells hold at most 50,000 characters
  MAX_COLUMNS: 200,
  MAX_MOVES: 300,              // moves kept per attempt
  RAPID_GUESS_SECONDS: 3,      // a wrong move this soon after the previous one counts as a guess
  REPORT_MINUTES: 15           // how often Students/Overview are rebuilt
};

// Game-state tabs the game may write to, and the column that identifies a row in each.
// "records" (hidden) keeps each player's 3D best times and run counts for the in-game scoreboard.
// Learning data goes to "attempts" instead (logAttempt).
var PROGRESS_TABLES = {
  records: 'username'
};

// Tabs written by earlier versions of the game, removed by startFresh().
var LEGACY_TABS = ['statistics', 'bunny', 'bunRecords', 'records', 'p1_1', 'p2_1', 'p2_2', 'p2_3',
  'p3_1', 'p3_2', 'p3_3', 'p4_1', 'p4_2', 'p4_3'];

var USER_COLUMNS = ['username', 'sessionID', 'school', 'education', 'created', 'last_login', 'tot_time',
  'admin', 'isClass', 'isExpo', 'isBunny', 'stageNum', 'difficultyNum', 'forceStage', 'expires',
  'mouseSense', 'soundVol', 'soundFX'];
var USER_WRITABLE = ['tot_time', 'mouseSense', 'soundVol', 'soundFX']; // fields the game may change
var AUTH_COLUMNS = ['username', 'salt', 'hash', 'iterations', 'created'];
var SESSION_COLUMNS = ['sessionID', 'createdBy', 'created', 'isClass', 'isExpo', 'isBunny', 'expires',
  'forceStage', 'stageNum', 'difficultyNum'];
var ATTEMPT_COLUMNS = ['time', 'player', 'section', 'school', 'education', 'game', 'activity', 'stage',
  'difficulty', 'concept', 'problem', 'outcome', 'minutes', 'moves', 'ideal_moves', 'efficiency',
  'first_move_correct', 'correct_moves', 'sign_errors', 'swap_errors', 'scalar_errors',
  'parallel_pair_errors', 'partial_errors', 'other_errors', 'rapid_guesses', 'score', 'final_answer', 'move_log'];
var OUTCOMES = ['completed', 'failed', 'restarted', 'abandoned'];
// mistake types sent by the game (MathMistakes in AttemptLog.cs) -> attempts column and plain words
var MISTAKES = {
  sign: { column: 'sign_errors', label: 'sign error' },
  swap: { column: 'swap_errors', label: 'swapped components or scalars' },
  scalar: { column: 'scalar_errors', label: 'wrong scalar' },
  parallel_pair: { column: 'parallel_pair_errors', label: 'chose parallel vectors' },
  partial: { column: 'partial_errors', label: 'one term right' },
  other: { column: 'other_errors', label: 'other mistake' }
};
// [game, activity code, stage (0 = any)] -> concept, in report order
var CONCEPTS = [
  ['Scalar multiples (3D displacement)', [['3D', 'P1', 0]]],
  ['Linear combination of two vectors', [['2D', 'L1', 0], ['2D', 'L2', 0], ['3D', 'P4', 2]]],
  ['Planning several combinations', [['2D', 'L3', 0]]],
  ['Span of one vector (line through origin)', [['2D', 'L4', 0], ['2D', 'L6', 0], ['3D', 'P4', 1]]],
  ['Lines not through the origin (p + t·v)', [['2D', 'L5', 0], ['2D', 'L7', 0]]],
  ['Coordinates in a non-standard basis', [['3D', 'P2', 0]]],
  ['Span of two vectors (plane in 3D)', [['3D', 'P3', 0]]],
  ['Linear combinations in 3D', [['3D', 'P4', 3]]]
];
var COLUMN_NAME = /^[A-Za-z_][A-Za-z0-9_]{0,63}$/;
var SESSION_NAME = /^[a-z0-9_]{1,40}$/;


// ---------------------------------------------------------------------------------------------
// Entry points
// ---------------------------------------------------------------------------------------------

class ClientError extends Error {}

var ACTIONS = {
  ping: ping,
  createAccount: createAccount,
  login: login,
  joinSession: joinSession,
  createSession: createSession,
  getRow: getRow,
  saveRow: saveRow,
  logAttempt: logAttempt
};

function doPost(e) {
  var response;
  try {
    var request = JSON.parse((e && e.postData && e.postData.contents) || '{}');
    var handler = ACTIONS.hasOwnProperty(request.action) ? ACTIONS[request.action] : null;
    if (!handler)
      throw new ClientError('Unknown request.');
    response = handler(request);
    response.ok = true;
  } catch (err) {
    if (err instanceof ClientError) {
      response = { ok: false, error: err.message };
    } else {
      console.error(err && err.stack ? err.stack : err);
      response = { ok: false, error: 'Server error. Please try again.' };
    }
  }
  return ContentService.createTextOutput(JSON.stringify(response)).setMimeType(ContentService.MimeType.JSON);
}

// Opening the web app URL in a browser shows this, which is a quick deployment check.
function doGet() {
  return ContentService.createTextOutput(JSON.stringify({ ok: true, service: 'Vector Unknown game server', version: VERSION }))
    .setMimeType(ContentService.MimeType.JSON);
}

// Run from the Apps Script editor after installing or updating the script: creates the tabs,
// hides the internal ones, writes the data dictionary and schedules the report rebuild.
function setup() {
  openTable('users', USER_COLUMNS);
  openTable('sessions', SESSION_COLUMNS);
  openTable('attempts', ATTEMPT_COLUMNS);
  Object.keys(PROGRESS_TABLES).forEach(function (name) { openTable(name, ['username', 'sessionID']).sheet.hideSheet(); });
  openTable('auth', AUTH_COLUMNS).sheet.hideSheet();
  writeDataDictionary();
  buildReports();
  installReportTrigger();
  console.log('Vector Unknown game server tabs are ready.');
}

// Run once from the editor to switch an existing sheet to the attempts layout: deletes the tabs
// earlier versions wrote (and their data), removes password rows of deleted players, then setup().
// Players, passwords and sessions are kept.
function startFresh() {
  var ss = SpreadsheetApp.getActive();
  LEGACY_TABS.forEach(function (name) {
    var sheet = ss.getSheetByName(name);
    if (sheet)
      ss.deleteSheet(sheet);
  });
  var users = openTable('users', USER_COLUMNS);
  var players = {};
  if (users.sheet.getLastRow() > 1)
    users.sheet.getRange(2, users.headers.indexOf('username') + 1, users.sheet.getLastRow() - 1, 1).getValues()
      .forEach(function (r) { players[String(r[0]).toLowerCase()] = true; });
  var auth = openTable('auth', AUTH_COLUMNS);
  for (var r = auth.sheet.getLastRow(); r >= 2; r--) {
    var name = String(auth.sheet.getRange(r, auth.headers.indexOf('username') + 1).getValues()[0][0]).toLowerCase();
    if (!players[name])
      auth.sheet.deleteRow(r);
  }
  setup();
}

// Rebuild Students/Overview every CONFIG.REPORT_MINUTES minutes (replaces an earlier schedule).
function installReportTrigger() {
  ScriptApp.getProjectTriggers().forEach(function (t) {
    if (t.getHandlerFunction() === 'buildReports')
      ScriptApp.deleteTrigger(t);
  });
  ScriptApp.newTrigger('buildReports').timeBased().everyMinutes(CONFIG.REPORT_MINUTES).create();
}

// Adds a "Vector Unknown" menu to the sheet so the team can refresh the reports by hand.
function onOpen() {
  SpreadsheetApp.getUi().createMenu('Vector Unknown')
    .addItem('Rebuild Students and Overview now', 'buildReports')
    .addToUi();
}


// ---------------------------------------------------------------------------------------------
// Actions
// ---------------------------------------------------------------------------------------------

function ping() {
  return { version: VERSION };
}

// {first, last, password, school, education} -> {username, token, users}
function createAccount(req) {
  var first = nameLetters(req.first);
  var last = nameLetters(req.last);
  if (!first || !last)
    throw new ClientError('Enter both a first and a last name (letters only).');
  var password = checkNewPassword(req.password);
  var school = cleanText(req.school, 100);
  var education = cleanText(req.education, 50);
  rateLimit('signups', CONFIG.MAX_SIGNUPS_PER_MINUTE, 60, 'Too many new accounts right now. Please try again in a minute.');

  // hash outside the lock: it is the slow part and doesn't depend on the final ID
  var salt = newSalt();
  var hash = pbkdf2(password, salt, CONFIG.HASH_ITERATIONS);
  var base = first.slice(0, 3) + last.slice(0, 3);

  return withLock(function () {
    var users = openTable('users', USER_COLUMNS);
    var username = nextFreeUsername(users, base);
    var now = new Date();
    appendRow(openTable('auth', AUTH_COLUMNS),
      { username: username, salt: salt, hash: hash, iterations: CONFIG.HASH_ITERATIONS, created: now });
    var user = newUser(username, now);
    user.school = school;
    user.education = education;
    appendRow(users, user);
    return { username: username, token: newToken(username), users: publicRow(user) };
  });
}

// {username, password} -> {token, users}
function login(req) {
  var username = String(req.username || '').trim().toLowerCase();
  var password = String(req.password || '');
  var cache = CacheService.getScriptCache();
  var failKey = 'fail:' + username;
  var fails = Number(cache.get(failKey) || 0);
  if (fails >= CONFIG.MAX_FAILED_LOGINS)
    throw new ClientError('Too many wrong passwords for this ID. Please wait 10 minutes and try again.');

  var auth = username && username.length <= 80 ? findRow(openTable('auth', AUTH_COLUMNS), 'username', username) : null;
  // hash even when the ID doesn't exist, so response time doesn't reveal which IDs are real
  var iterations = auth ? Number(auth.iterations) || CONFIG.HASH_ITERATIONS : CONFIG.HASH_ITERATIONS;
  var attempt = pbkdf2(password, auth ? String(auth.salt) : 'no-such-player', iterations);
  if (!auth || !safeEqual(attempt, String(auth.hash))) {
    cache.put(failKey, String(fails + 1), CONFIG.LOCKOUT_SECONDS);
    throw new ClientError('Wrong player ID or password.');
  }
  cache.remove(failKey);

  var user = withLock(function () {
    var users = openTable('users', USER_COLUMNS);
    var row = findRow(users, 'username', username);
    if (!row)
      throw new ClientError('Wrong player ID or password.');
    row.last_login = new Date();
    writeRow(users, row);
    return row;
  });
  return { token: newToken(username), users: publicRow(user) };
}

// {sessionID} -> {token, users}: a classroom/expo player with a generated ID and no password
function joinSession(req) {
  var sessionID = String(req.sessionID || '').trim().toLowerCase();
  var session = SESSION_NAME.test(sessionID) ? findRow(openTable('sessions', SESSION_COLUMNS), 'sessionID', sessionID) : null;
  if (!session || isExpired(session.expires))
    throw new ClientError('That session does not exist or has expired.');
  rateLimit('joins', CONFIG.MAX_SIGNUPS_PER_MINUTE * 5, 60, 'Too many players are joining right now. Please try again in a minute.');

  return withLock(function () {
    var users = openTable('users', USER_COLUMNS);
    var username;
    do {
      username = sessionID + '-' + randomCode(6);
    } while (findRowNumber(users, 'username', username) > 0);
    var user = newUser(username, new Date());
    ['isClass', 'isExpo', 'isBunny', 'expires', 'forceStage', 'stageNum', 'difficultyNum'].forEach(function (k) {
      user[k] = session[k];
    });
    user.sessionID = sessionID;
    appendRow(users, user);
    return { token: newToken(username), users: publicRow(user) };
  });
}

// {token, sessionID, isClass, isExpo, isBunny, expires, forceStage, stageNum, difficultyNum} -> {session}
function createSession(req) {
  var admin = requireUser(req.token);
  if (!isTrue(admin.row.admin))
    throw new ClientError('Only admin accounts can create sessions.');
  var sessionID = String(req.sessionID || '').trim().toLowerCase();
  if (!SESSION_NAME.test(sessionID))
    throw new ClientError('Session names can only use letters, numbers and _ (up to 40 characters).');
  var expires = new Date(req.expires);
  if (isNaN(expires.getTime()) || expires <= new Date())
    throw new ClientError('Pick an expiry date in the future.');

  return withLock(function () {
    var sessions = openTable('sessions', SESSION_COLUMNS);
    var existing = findRow(sessions, 'sessionID', sessionID);
    if (existing && existing.createdBy !== admin.username && !isExpired(existing.expires))
      throw new ClientError('That session name is already in use.');
    var session = {
      sessionID: sessionID,
      createdBy: admin.username,
      created: new Date(),
      isClass: req.isClass === true,
      isExpo: req.isExpo === true,
      isBunny: req.isBunny === true,
      expires: expires,
      forceStage: req.forceStage === true,
      stageNum: clampInt(req.stageNum, 0, 7),
      difficultyNum: clampInt(req.difficultyNum, 0, 3)
    };
    if (existing)
      session.__row = existing.__row;
    writeRow(sessions, session);
    return { session: publicRow(session) };
  });
}

// {token, table} -> {row}: the player's own row ("users" or a gameplay tab; latest attempt for attempt tabs)
function getRow(req) {
  var user = requireUser(req.token);
  if (req.table === 'users')
    return { row: publicRow(user.row) };
  if (!PROGRESS_TABLES.hasOwnProperty(req.table))
    throw new ClientError('Unknown table.');
  var row = findRow(openTable(req.table, ['username', 'sessionID']), 'username', user.username);
  return { row: row ? publicRow(row) : null };
}

// {token, table, row} -> {}: creates or updates the player's row
function saveRow(req) {
  var user = requireUser(req.token);
  var row = req.row;
  if (!row || typeof row !== 'object' || Array.isArray(row))
    throw new ClientError('Nothing to save.');
  var keys = Object.keys(row);
  if (keys.length > CONFIG.MAX_COLUMNS || !keys.every(function (k) { return COLUMN_NAME.test(k); }))
    throw new ClientError('Invalid data.');

  if (req.table === 'users') {
    return withLock(function () {
      var users = openTable('users', USER_COLUMNS);
      var current = findRow(users, 'username', user.username);
      USER_WRITABLE.forEach(function (k) {
        if (row.hasOwnProperty(k) && typeof row[k] === 'number' && isFinite(row[k]))
          current[k] = row[k];
      });
      writeRow(users, current);
      return {};
    });
  }

  var keyColumn = PROGRESS_TABLES.hasOwnProperty(req.table) ? PROGRESS_TABLES[req.table] : null;
  if (!keyColumn)
    throw new ClientError('Unknown table.');
  var clean = {};
  keys.forEach(function (k) { clean[k] = row[k]; });
  clean.username = user.username;
  clean.sessionID = user.row.sessionID || '';
  if (keyColumn === 'usernameAttempt') {
    var attempt = String(clean.usernameAttempt || '');
    if (attempt.indexOf(user.username) !== 0 || attempt.length > 120)
      throw new ClientError('This attempt does not belong to the logged-in player.');
  }

  return withLock(function () {
    var t = openTable(req.table, ['username', 'sessionID']);
    ensureColumns(t, Object.keys(clean));
    var existing = findRowNumber(t, keyColumn, clean[keyColumn]);
    if (existing > 0)
      clean.__row = existing;
    writeRow(t, clean);
    return {};
  });
}

// {token, attempt: {...}} -> {}: one play of a level or puzzle (AttemptLog.cs) -> a row in "attempts".
// The player, section and time come from the login, not the game; totals are worked out here.
function logAttempt(req) {
  var user = requireUser(req.token);
  var a = req.attempt;
  if (!a || typeof a !== 'object' || Array.isArray(a))
    throw new ClientError('Invalid data.');
  var game = a.game === '2D' || a.game === '3D' ? a.game : null;
  var code = /^(L[0-7]|P[1-4])$/.test(String(a.activity)) ? String(a.activity) : null;
  var outcome = OUTCOMES.indexOf(a.outcome) >= 0 ? a.outcome : null;
  if (!game || !code || !outcome || (game === '2D') !== (code.charAt(0) === 'L'))
    throw new ClientError('Invalid data.');
  var stage = clampInt(a.stage, 0, 3);
  var difficulty = clampInt(a.difficulty, 0, 3);
  var ideal = clampInt(a.ideal_moves, 0, 1000);
  var seconds = Math.min(Math.max(Number(a.seconds) || 0, 0), 86400);

  var moves = (Array.isArray(a.moves) ? a.moves : []).slice(0, CONFIG.MAX_MOVES).map(function (m) {
    m = m && typeof m === 'object' ? m : {};
    var mistake = String(m.mistake || '');
    return {
      t: Math.min(Math.max(Number(m.t) || 0, 0), 86400),
      move: cleanText(m.move, 200),
      mistake: mistake === 'correct' || MISTAKES.hasOwnProperty(mistake) ? mistake : ''
    };
  });

  var row = {
    time: new Date(), player: user.username, section: user.row.sessionID || '',
    school: user.row.school || '', education: user.row.education || '',
    game: game, activity: activityLabel(game, code, stage), stage: stage || '',
    difficulty: game === '2D' ? (['', 'Easy', 'Medium', 'Hard'][difficulty] || '') : '',
    concept: conceptFor(game, code, stage), problem: cleanText(a.problem, 500), outcome: outcome,
    minutes: Math.round(seconds / 6) / 10, moves: moves.length, ideal_moves: ideal || '',
    score: isFinite(Number(a.score)) && a.score !== null && a.score !== undefined ? Number(a.score) : '',
    final_answer: cleanText(a.final_answer, 200)
  };

  var efficiency = Number(a.efficiency);
  if (a.efficiency !== undefined && isFinite(efficiency))
    row.efficiency = Math.min(Math.max(efficiency, 0), 1);
  else
    row.efficiency = outcome === 'completed' && ideal > 0 && moves.length > 0 ? Math.min(1, Math.round(100 * ideal / moves.length) / 100) : '';

  var judged = moves.filter(function (m) { return m.mistake !== ''; });
  row.first_move_correct = judged.length ? judged[0].mistake === 'correct' : '';
  row.correct_moves = judged.length ? judged.filter(function (m) { return m.mistake === 'correct'; }).length : '';
  Object.keys(MISTAKES).forEach(function (k) {
    row[MISTAKES[k].column] = judged.length ? judged.filter(function (m) { return m.mistake === k; }).length : '';
  });
  row.rapid_guesses = moves.filter(function (m, i) {
    return i > 0 && m.mistake !== '' && m.mistake !== 'correct' && m.t - moves[i - 1].t < CONFIG.RAPID_GUESS_SECONDS;
  }).length;
  row.move_log = moves.map(function (m, i) {
    var label = m.mistake === '' ? '' : m.mistake === 'correct' ? '  ✓' : '  ✗ ' + MISTAKES[m.mistake].label;
    return (i + 1) + '. ' + m.t.toFixed(1) + ' s  ' + m.move + label;
  }).join('\n');

  return withLock(function () {
    appendRow(openTable('attempts', ATTEMPT_COLUMNS), row);
    return {};
  });
}

function activityLabel(game, code, stage) {
  if (game === '2D')
    return code === 'L0' ? '2D Tutorial' : '2D Level ' + code.slice(1);
  return '3D Puzzle ' + code.slice(1) + (stage ? ', stage ' + stage : '');
}

function conceptFor(game, code, stage) {
  for (var i = 0; i < CONCEPTS.length; i++)
    for (var j = 0; j < CONCEPTS[i][1].length; j++) {
      var c = CONCEPTS[i][1][j];
      if (c[0] === game && c[1] === code && (c[2] === 0 || c[2] === stage))
        return CONCEPTS[i][0];
    }
  return '';
}


// ---------------------------------------------------------------------------------------------
// Players, tokens and passwords
// ---------------------------------------------------------------------------------------------

function newUser(username, now) {
  return {
    username: username, sessionID: '', school: '', education: '', created: now, last_login: now, tot_time: 0,
    admin: false, isClass: false, isExpo: false, isBunny: false, stageNum: 0, difficultyNum: 0,
    forceStage: false, expires: '', mouseSense: 2, soundVol: 0.25, soundFX: 1   // the game's defaults (PlayerData)
  };
}

// The ID is the name letters plus the lowest free number: "ashamr", then "ashamr2", "ashamr3"...
function nextFreeUsername(users, base) {
  var col = users.headers.indexOf('username');
  var last = users.sheet.getLastRow();
  var taken = {};
  if (last > 1) {
    users.sheet.getRange(2, col + 1, last - 1, 1).getValues().forEach(function (r) { taken[String(r[0]).toLowerCase()] = true; });
  }
  if (!taken[base])
    return base;
  for (var n = 2; ; n++) {
    if (!taken[base + n])
      return base + n;
  }
}

function newToken(username) {
  var token = Utilities.getUuid().replace(/-/g, '') + Utilities.getUuid().replace(/-/g, '');
  CacheService.getScriptCache().put('tok:' + token, username, CONFIG.TOKEN_TTL_SECONDS);
  return token;
}

// -> {username, row}; renews the token's lifetime
function requireUser(token) {
  token = String(token || '');
  var cache = CacheService.getScriptCache();
  var username = /^[0-9a-f]{64}$/.test(token) ? cache.get('tok:' + token) : null;
  if (!username)
    throw new ClientError('Your login has expired. Please log in again.');
  cache.put('tok:' + token, username, CONFIG.TOKEN_TTL_SECONDS);
  var row = findRow(openTable('users', USER_COLUMNS), 'username', username);
  if (!row)
    throw new ClientError('Your login has expired. Please log in again.');
  return { username: username, row: row };
}

function checkNewPassword(password) {
  password = String(password || '');
  if (password.length < CONFIG.MIN_PASSWORD_LENGTH)
    throw new ClientError('Password must be at least ' + CONFIG.MIN_PASSWORD_LENGTH + ' characters long.');
  if (password.length > CONFIG.MAX_PASSWORD_LENGTH)
    throw new ClientError('Password is too long.');
  return password;
}

function newSalt() {
  return Utilities.getUuid().replace(/-/g, '') + Utilities.getUuid().replace(/-/g, '');
}

// PBKDF2-HMAC-SHA256, 32-byte output, base64
function pbkdf2(password, salt, iterations) {
  var key = Utilities.newBlob(String(password)).getBytes();
  var u = Utilities.computeHmacSha256Signature(Utilities.newBlob(String(salt)).getBytes().concat([0, 0, 0, 1]), key);
  var out = u.slice();
  for (var i = 1; i < iterations; i++) {
    u = Utilities.computeHmacSha256Signature(u, key);
    for (var j = 0; j < out.length; j++)
      out[j] ^= u[j];
  }
  return Utilities.base64Encode(out);
}

function safeEqual(a, b) {
  if (a.length !== b.length)
    return false;
  var diff = 0;
  for (var i = 0; i < a.length; i++)
    diff |= a.charCodeAt(i) ^ b.charCodeAt(i);
  return diff === 0;
}

function randomCode(length) {
  return Utilities.getUuid().replace(/-/g, '').slice(0, length);
}

function rateLimit(name, max, seconds, message) {
  var cache = CacheService.getScriptCache();
  var key = 'rate:' + name + ':' + Math.floor(Date.now() / (seconds * 1000));
  var count = Number(cache.get(key) || 0) + 1;
  cache.put(key, String(count), seconds * 2);
  if (count > max)
    throw new ClientError(message);
}


// ---------------------------------------------------------------------------------------------
// Sheet helpers
// ---------------------------------------------------------------------------------------------

// -> {sheet, headers}; creates the tab and any missing columns
function openTable(name, requiredColumns) {
  var ss = SpreadsheetApp.getActive();
  var sheet = ss.getSheetByName(name) || ss.insertSheet(name);
  var lastCol = sheet.getLastColumn();
  var headers = lastCol > 0 ? sheet.getRange(1, 1, 1, lastCol).getValues()[0].map(String) : [];
  var t = { sheet: sheet, headers: headers };
  ensureColumns(t, requiredColumns || []);
  return t;
}

function ensureColumns(t, names) {
  var missing = names.filter(function (n) { return n !== '__row' && t.headers.indexOf(n) < 0; });
  if (!missing.length)
    return;
  if (t.headers.length + missing.length > CONFIG.MAX_COLUMNS)
    throw new ClientError('Invalid data.');
  t.sheet.getRange(1, t.headers.length + 1, 1, missing.length).setValues([missing]).setFontWeight('bold');
  t.headers = t.headers.concat(missing);
  t.sheet.setFrozenRows(1);
}

// Row number (1-based, >= 2) of the LAST row whose column equals value, or -1
function findRowNumber(t, column, value) {
  var col = t.headers.indexOf(column);
  var last = t.sheet.getLastRow();
  if (col < 0 || last < 2 || value === undefined || value === null || value === '')
    return -1;
  var matches = t.sheet.getRange(2, col + 1, last - 1, 1)
    .createTextFinder(String(value)).matchCase(true).matchEntireCell(true).findAll();
  return matches.length ? matches[matches.length - 1].getRow() : -1;
}

// -> row object (with __row) or null
function findRow(t, column, value) {
  var r = findRowNumber(t, column, value);
  if (r < 0)
    return null;
  var values = t.sheet.getRange(r, 1, 1, t.headers.length).getValues()[0];
  var row = { __row: r };
  t.headers.forEach(function (h, i) { row[h] = values[i]; });
  return row;
}

function appendRow(t, obj) {
  delete obj.__row;
  writeRow(t, obj);
}

// Writes obj into its row (obj.__row) or a new row at the bottom
function writeRow(t, obj) {
  ensureColumns(t, Object.keys(obj));
  var r = obj.__row || t.sheet.getLastRow() + 1;
  var existing = obj.__row ? t.sheet.getRange(r, 1, 1, t.headers.length).getValues()[0] : null;
  var cells = t.headers.map(function (h, i) {
    if (obj.hasOwnProperty(h))
      return toCell(obj[h]);
    return existing ? existing[i] : '';
  });
  t.sheet.getRange(r, 1, 1, cells.length).setValues([cells]);
}

// Numbers, booleans and dates are stored as-is; everything else as plain text. The leading
// apostrophe makes Sheets keep the text exactly (no formulas, no auto-converted dates); it is
// not part of the stored value.
function toCell(v) {
  if (v === null || v === undefined)
    return '';
  if (typeof v === 'number')
    return isFinite(v) ? v : '';
  if (typeof v === 'boolean' || v instanceof Date)
    return v;
  if (typeof v === 'object')
    v = JSON.stringify(v);
  v = String(v).slice(0, CONFIG.MAX_TEXT_LENGTH);
  return v === '' ? '' : "'" + v;
}

// Row as sent to the game: dates as ISO strings, JSON text parsed back, empty cells left out
// (so the game's defaults apply), internal fields dropped.
function publicRow(row) {
  var out = {};
  Object.keys(row).forEach(function (k) {
    var v = row[k];
    if (k === '__row' || v === '' || v === null || v === undefined)
      return;
    if (v instanceof Date) {
      v = v.toISOString();
    } else if (typeof v === 'string' && /^[\[{]/.test(v)) {
      try { v = JSON.parse(v); } catch (e) { /* plain text that starts with a bracket */ }
    }
    out[k] = v;
  });
  return out;
}

function withLock(fn) {
  var lock = LockService.getScriptLock();
  if (!lock.tryLock(30000))
    throw new ClientError('The server is busy. Please try again.');
  try {
    var result = fn();
    SpreadsheetApp.flush();
    return result;
  } finally {
    lock.releaseLock();
  }
}


// ---------------------------------------------------------------------------------------------
// Small validators
// ---------------------------------------------------------------------------------------------

// Letters only, lowercase, accents removed: "José" -> "jose"
function nameLetters(name) {
  return String(name || '').normalize('NFD').replace(/[̀-ͯ]/g, '').replace(/[^A-Za-z]/g, '').toLowerCase().slice(0, 40);
}

function cleanText(s, max) {
  return String(s || '').replace(/[\u0000-\u001f]/g, ' ').trim().slice(0, max);
}

// TRUE whether the team ticked a checkbox or typed TRUE/true into the cell
function isTrue(v) {
  return v === true || String(v).trim().toUpperCase() === 'TRUE';
}

function clampInt(v, min, max) {
  var n = Math.round(Number(v) || 0);
  return Math.min(max, Math.max(min, n));
}

function isExpired(expires) {
  var d = expires instanceof Date ? expires : new Date(expires);
  return isNaN(d.getTime()) || d <= new Date();
}


// ---------------------------------------------------------------------------------------------
// Teacher reports: "Students" and "Overview", rebuilt from "attempts"
// ---------------------------------------------------------------------------------------------

var MASTERY_COLORS = { 'Mastered': '#b7e1cd', 'Developing': '#fce8b2', 'Not yet': '#f4c7c3', 'Not attempted': '#ffffff' };

function buildReports() {
  var attempts = readRows(openTable('attempts', ATTEMPT_COLUMNS)).filter(function (a) { return a.player !== ''; });
  attempts.sort(function (a, b) { return timeOf(a) - timeOf(b); });
  var users = {};
  readRows(openTable('users', USER_COLUMNS)).forEach(function (u) { users[String(u.username)] = u; });
  writeStudents(attempts, users);
  writeOverview(attempts);
}

function writeStudents(attempts, users) {
  var byPlayer = groupBy(attempts, function (a) { return String(a.player); });
  var concepts = CONCEPTS.map(function (c) { return c[0]; });
  var headers = ['Player', 'Section', 'School', 'Education', 'First played', 'Last played', 'Active minutes',
    'Plays', 'Completed'].concat(concepts).concat(['Most common mistake', 'Trend', 'Needs attention', 'Why']);
  var rows = [], colors = [];
  Object.keys(byPlayer).sort().forEach(function (player) {
    var list = byPlayer[player];
    var user = users[player] || {};
    var last = list[list.length - 1];
    var mastery = concepts.map(function (c) {
      return masteryOf(list.filter(function (a) { return a.concept === c; }));
    });
    var reasons = attentionReasons(list);
    var row = [player, String(last.section || user.sessionID || ''), user.school || last.school || '',
      user.education || last.education || '', list[0].time, last.time, round1(sum(list, 'minutes')), list.length,
      countOutcome(list, 'completed')]
      .concat(mastery)
      .concat([commonMistake(list), trendOf(list), reasons.length > 0, reasons.join('; ')]);
    rows.push(row);
    colors.push(row.map(function (v, i) {
      if (MASTERY_COLORS.hasOwnProperty(v))
        return MASTERY_COLORS[v];
      return headers[i] === 'Needs attention' && v === true ? '#f4c7c3' : '#ffffff';
    }));
  });
  var sheet = writeTable('Students', headers, rows);
  if (rows.length)
    sheet.getRange(2, 1, rows.length, headers.length).setBackgrounds(colors);
}

function writeOverview(attempts) {
  var headers = ['Section', 'Game', 'Activity', 'Concept', 'Players', 'Plays', 'Completed', 'Median minutes',
    'Median efficiency', 'First move correct', 'Most common mistake'];
  var sections = groupBy(attempts, function (a) { return String(a.section || '') || '(no section)'; });
  var groups = [['All sections', attempts]];
  Object.keys(sections).sort().forEach(function (s) { groups.push([s, sections[s]]); });
  var rows = [];
  groups.forEach(function (g) {
    var byActivity = groupBy(g[1], function (a) { return a.game + '|' + a.activity; });
    Object.keys(byActivity).sort().forEach(function (k) {
      var list = byActivity[k];
      var players = {};
      list.forEach(function (a) { players[a.player] = true; });
      var effs = list.filter(function (a) { return a.outcome === 'completed' && a.efficiency !== ''; })
        .map(function (a) { return Number(a.efficiency); });
      var judged = list.filter(function (a) { return a.first_move_correct === true || a.first_move_correct === false; });
      rows.push([g[0], list[0].game, list[0].activity, list[0].concept, Object.keys(players).length, list.length,
        countOutcome(list, 'completed') / list.length,
        round1(median(list.map(function (a) { return Number(a.minutes) || 0; }))),
        effs.length ? median(effs) : '',
        judged.length ? judged.filter(function (a) { return a.first_move_correct === true; }).length / judged.length : '',
        commonMistake(list)]);
    });
  });
  var sheet = writeTable('Overview', headers, rows);
  if (rows.length) {
    sheet.getRange(2, 7, rows.length, 1).setNumberFormat('0%');
    sheet.getRange(2, 9, rows.length, 2).setNumberFormat('0%');
  }
}

// Mastered: 2 of the last 3 plays completed efficiently (efficiency >= 0.6, where measured) and a
// first move right at least once; Developing: completed at least once; Not yet: never completed.
function masteryOf(list) {
  if (!list.length)
    return 'Not attempted';
  var recent = list.slice(-3);
  var good = recent.filter(function (a) {
    return a.outcome === 'completed' && (a.efficiency === '' || Number(a.efficiency) >= 0.6);
  });
  var firstRight = recent.some(function (a) { return a.first_move_correct === true || a.first_move_correct === ''; });
  if (good.length >= 2 && firstRight)
    return 'Mastered';
  return countOutcome(list, 'completed') > 0 ? 'Developing' : 'Not yet';
}

function attentionReasons(list) {
  var reasons = [];
  var byConcept = groupBy(list, function (a) { return a.concept; });
  Object.keys(byConcept).forEach(function (c) {
    var l = byConcept[c];
    if (c && l.length >= 3 && countOutcome(l, 'completed') === 0)
      reasons.push('No completion in ' + l.length + ' plays: ' + c);
  });
  var recent = list.slice(-5);
  if (list.length >= 3 && countOutcome(list.slice(-3), 'completed') === 0)
    reasons.push('Last 3 plays not completed');
  var guesses = sum(recent, 'rapid_guesses');
  if (guesses >= 3)
    reasons.push('Rapid guessing (' + guesses + ' quick wrong moves in recent plays)');
  recent.forEach(function (a) {
    if (a.outcome === 'abandoned' && Number(a.minutes) >= 10)
      reasons.push('Gave up after ' + round1(a.minutes) + ' min on ' + a.activity);
  });
  return reasons;
}

function trendOf(list) {
  var effs = list.filter(function (a) { return a.outcome === 'completed' && a.efficiency !== ''; })
    .map(function (a) { return Number(a.efficiency); });
  if (effs.length < 4)
    return 'Not enough data';
  var half = Math.floor(effs.length / 2);
  var diff = mean(effs.slice(effs.length - half)) - mean(effs.slice(0, half));
  return diff > 0.1 ? 'Improving' : diff < -0.1 ? 'Declining' : 'Steady';
}

function commonMistake(list) {
  var best = '', bestCount = 0;
  Object.keys(MISTAKES).forEach(function (k) {
    var n = sum(list, MISTAKES[k].column);
    if (n > bestCount) {
      best = MISTAKES[k].label;
      bestCount = n;
    }
  });
  return best ? best + ' (' + bestCount + ')' : '';
}

// Replaces a report tab's contents with headers + rows.
function writeTable(name, headers, rows) {
  var ss = SpreadsheetApp.getActive();
  var sheet = ss.getSheetByName(name) || ss.insertSheet(name);
  sheet.clear();
  sheet.getRange(1, 1, 1, headers.length).setValues([headers]).setFontWeight('bold');
  if (rows.length)
    sheet.getRange(2, 1, rows.length, headers.length).setValues(rows.map(function (r) { return r.map(toCell); }));
  sheet.setFrozenRows(1);
  return sheet;
}

function writeDataDictionary() {
  var mistakes = 'sign error (right size, a sign flipped), swapped (x and y, or the two scalars, swapped), ' +
    'wrong scalar (right direction or right vectors, wrong multiple), chose parallel vectors (the two vectors ' +
    'cannot reach the target), one term right, other';
  var rows = [
    ['Students', 'Player', 'Player ID: first 3 letters of the first name + first 3 of the last name. No names are stored.'],
    ['Students', 'Section', 'Class or expo session the player joined (the teacher names it in the admin panel).'],
    ['Students', 'Active minutes', 'Time spent working on levels and puzzles (pauses and menus excluded).'],
    ['Students', 'Plays', 'Levels and puzzles finished, failed, restarted, or left after at least one move.'],
    ['Students', '(concept columns)', 'Mastered: 2 of the last 3 plays of that concept completed with efficiency of at least 60% and a correct first move at least once. Developing: completed at least once. Not yet: played but never completed. Not attempted: never played.'],
    ['Students', 'Most common mistake', 'The mistake type the player made most often, with the count.'],
    ['Students', 'Trend', 'Efficiency of recent completed plays compared with earlier ones (needs 4 or more).'],
    ['Students', 'Needs attention / Why', 'Flagged when a concept has 3+ plays and no completion, the last 3 plays were not completed, there were 3+ rapid wrong moves recently, or the player gave up after 10+ minutes.'],
    ['Overview', '(all)', 'One row per activity, for all sections together and for each section. Completed = share of plays completed; First move correct = share of plays whose first judged move was right.'],
    ['attempts', 'time', 'When the play ended (server time).'],
    ['attempts', 'player, section, school, education', 'Who played, from their account.'],
    ['attempts', 'game, activity, stage, difficulty', '2D Bunny levels 1-7 (difficulty Easy, Medium or Hard) or 3D Echelon Seas puzzles 1-4 (stage 1-3).'],
    ['attempts', 'concept', 'The linear algebra idea the activity practises.'],
    ['attempts', 'problem', 'The task as the player saw it: target, available vector cards, and one correct solution where the game knows it.'],
    ['attempts', 'outcome', 'completed, failed (2D: out of attempts), restarted (pressed Restart), abandoned (left without finishing).'],
    ['attempts', 'minutes', 'Active time in the play.'],
    ['attempts', 'moves, ideal_moves', 'Moves submitted (2D: GO; 3D: answer, shot, tilt or gap crossed) and the fewest possible.'],
    ['attempts', 'efficiency', 'ideal_moves / moves for completed plays (3D Puzzle 4: fewest card or slider changes / changes made). 1 means no wasted moves.'],
    ['attempts', 'first_move_correct', 'Whether the first judged move was right (blank when moves cannot be judged, e.g. 3D Puzzle 3).'],
    ['attempts', 'correct_moves and *_errors', 'How many moves were right, and how many of each mistake type: ' + mistakes + '.'],
    ['attempts', 'rapid_guesses', 'Wrong moves made less than ' + CONFIG.RAPID_GUESS_SECONDS + ' seconds after the previous move.'],
    ['attempts', 'score', '2D: stars (1-3) for completed levels. 3D Puzzle 4: the game\'s own score.'],
    ['attempts', 'final_answer', 'Where the player ended (2D: the bunny\'s position).'],
    ['attempts', 'move_log', 'Every move with its time in seconds, the vectors and scalars used, and ✓ or ✗ with the mistake type.']
  ];
  var sheet = writeTable('Data dictionary', ['Tab', 'Column', 'Meaning'], rows);
  sheet.setColumnWidth(3, 700);
}

// ---- small helpers for the reports ----

function readRows(t) {
  var last = t.sheet.getLastRow();
  if (last < 2)
    return [];
  return t.sheet.getRange(2, 1, last - 1, t.headers.length).getValues().map(function (values) {
    var row = {};
    t.headers.forEach(function (h, i) { row[h] = values[i]; });
    return row;
  });
}

function groupBy(list, keyFn) {
  var out = {};
  list.forEach(function (x) {
    var k = keyFn(x);
    (out[k] = out[k] || []).push(x);
  });
  return out;
}

function timeOf(a) {
  var t = a.time instanceof Date ? a.time.getTime() : new Date(a.time).getTime();
  return isNaN(t) ? 0 : t;
}

function countOutcome(list, outcome) {
  return list.filter(function (a) { return a.outcome === outcome; }).length;
}

function sum(list, column) {
  return list.reduce(function (s, a) { return s + (Number(a[column]) || 0); }, 0);
}

function mean(values) {
  return values.reduce(function (s, v) { return s + v; }, 0) / values.length;
}

function median(values) {
  var v = values.slice().sort(function (a, b) { return a - b; });
  if (!v.length)
    return '';
  var m = Math.floor(v.length / 2);
  return v.length % 2 ? v[m] : (v[m - 1] + v[m]) / 2;
}

function round1(v) {
  return Math.round((Number(v) || 0) * 10) / 10;
}
