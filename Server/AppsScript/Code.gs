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
 * Setup and deployment: see Server/README.md in the game repository.
 */

var VERSION = '2026.10.1';

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
  MAX_STAT_VALUES: 255,
  MAX_STAT_LENGTH: 1000
};

// Gameplay tabs the game may write to, and the column that identifies a row in each.
// "username" tabs keep one row per player; "usernameAttempt" tabs keep one row per attempt.
var PROGRESS_TABLES = {
  records: 'username',
  bunRecords: 'username',
  bunny: 'usernameAttempt',
  p1_1: 'usernameAttempt',
  p2_1: 'usernameAttempt',
  p2_2: 'usernameAttempt',
  p2_3: 'usernameAttempt',
  p3_1: 'usernameAttempt',
  p3_2: 'usernameAttempt',
  p3_3: 'usernameAttempt',
  p4_1: 'usernameAttempt',
  p4_2: 'usernameAttempt',
  p4_3: 'usernameAttempt'
};

var USER_COLUMNS = ['username', 'sessionID', 'school', 'education', 'created', 'last_login', 'tot_time',
  'admin', 'isClass', 'isExpo', 'isBunny', 'stageNum', 'difficultyNum', 'forceStage', 'expires',
  'mouseSense', 'soundVol', 'soundFX'];
var USER_WRITABLE = ['tot_time', 'mouseSense', 'soundVol', 'soundFX']; // fields the game may change
var AUTH_COLUMNS = ['username', 'salt', 'hash', 'iterations', 'created'];
var SESSION_COLUMNS = ['sessionID', 'createdBy', 'created', 'isClass', 'isExpo', 'isBunny', 'expires',
  'forceStage', 'stageNum', 'difficultyNum'];
var STATS_COLUMNS = ['time', 'username', 'sessionID', 'values'];

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
  logStats: logStats
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

// Run once from the Apps Script editor: creates the tabs with their headers and hides "auth".
function setup() {
  openTable('users', USER_COLUMNS);
  openTable('sessions', SESSION_COLUMNS);
  openTable('statistics', STATS_COLUMNS);
  Object.keys(PROGRESS_TABLES).forEach(function (name) { openTable(name, ['username', 'sessionID']); });
  var auth = openTable('auth', AUTH_COLUMNS);
  auth.sheet.hideSheet();
  console.log('Vector Unknown game server tabs are ready.');
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

// {token, values: [...]} -> {}: appends gameplay events to the "statistics" tab
function logStats(req) {
  var user = requireUser(req.token);
  var values = Array.isArray(req.values) ? req.values.slice(0, CONFIG.MAX_STAT_VALUES) : [];
  var cells = values.map(function (v) { return toCell(String(v).slice(0, CONFIG.MAX_STAT_LENGTH)); });
  return withLock(function () {
    var t = openTable('statistics', STATS_COLUMNS);
    t.sheet.appendRow([new Date(), toCell(user.username), toCell(user.row.sessionID || '')].concat(cells));
    return {};
  });
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
