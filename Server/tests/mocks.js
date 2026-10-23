// Minimal stand-ins for the Google Apps Script services Code.gs uses, so it can run in a browser.

// ---- SHA-256 / HMAC (synchronous, pure JS) ----
function sha256Bytes(bytes) {
  const K = [0x428a2f98,0x71374491,0xb5c0fbcf,0xe9b5dba5,0x3956c25b,0x59f111f1,0x923f82a4,0xab1c5ed5,0xd807aa98,0x12835b01,0x243185be,0x550c7dc3,0x72be5d74,0x80deb1fe,0x9bdc06a7,0xc19bf174,0xe49b69c1,0xefbe4786,0x0fc19dc6,0x240ca1cc,0x2de92c6f,0x4a7484aa,0x5cb0a9dc,0x76f988da,0x983e5152,0xa831c66d,0xb00327c8,0xbf597fc7,0xc6e00bf3,0xd5a79147,0x06ca6351,0x14292967,0x27b70a85,0x2e1b2138,0x4d2c6dfc,0x53380d13,0x650a7354,0x766a0abb,0x81c2c92e,0x92722c85,0xa2bfe8a1,0xa81a664b,0xc24b8b70,0xc76c51a3,0xd192e819,0xd6990624,0xf40e3585,0x106aa070,0x19a4c116,0x1e376c08,0x2748774c,0x34b0bcb5,0x391c0cb3,0x4ed8aa4a,0x5b9cca4f,0x682e6ff3,0x748f82ee,0x78a5636f,0x84c87814,0x8cc70208,0x90befffa,0xa4506ceb,0xbef9a3f7,0xc67178f2];
  let H = [0x6a09e667,0xbb67ae85,0x3c6ef372,0xa54ff53a,0x510e527f,0x9b05688c,0x1f83d9ab,0x5be0cd19];
  const b = bytes.map(x => x & 0xff);
  const len = b.length;
  b.push(0x80);
  while (b.length % 64 !== 56) b.push(0);
  const bits = len * 8;
  for (let i = 7; i >= 0; i--) b.push(Math.floor(bits / Math.pow(2, i * 8)) & 0xff);
  const w = new Array(64);
  for (let off = 0; off < b.length; off += 64) {
    for (let i = 0; i < 16; i++) w[i] = (b[off+4*i] << 24) | (b[off+4*i+1] << 16) | (b[off+4*i+2] << 8) | b[off+4*i+3];
    for (let i = 16; i < 64; i++) {
      const s0 = ((w[i-15] >>> 7) | (w[i-15] << 25)) ^ ((w[i-15] >>> 18) | (w[i-15] << 14)) ^ (w[i-15] >>> 3);
      const s1 = ((w[i-2] >>> 17) | (w[i-2] << 15)) ^ ((w[i-2] >>> 19) | (w[i-2] << 13)) ^ (w[i-2] >>> 10);
      w[i] = (w[i-16] + s0 + w[i-7] + s1) | 0;
    }
    let [a, bb, c, d, e, f, g, h] = H;
    for (let i = 0; i < 64; i++) {
      const S1 = ((e >>> 6) | (e << 26)) ^ ((e >>> 11) | (e << 21)) ^ ((e >>> 25) | (e << 7));
      const ch = (e & f) ^ (~e & g);
      const t1 = (h + S1 + ch + K[i] + w[i]) | 0;
      const S0 = ((a >>> 2) | (a << 30)) ^ ((a >>> 13) | (a << 19)) ^ ((a >>> 22) | (a << 10));
      const maj = (a & bb) ^ (a & c) ^ (bb & c);
      const t2 = (S0 + maj) | 0;
      h = g; g = f; f = e; e = (d + t1) | 0; d = c; c = bb; bb = a; a = (t1 + t2) | 0;
    }
    H = [H[0]+a, H[1]+bb, H[2]+c, H[3]+d, H[4]+e, H[5]+f, H[6]+g, H[7]+h].map(x => x | 0);
  }
  const out = [];
  H.forEach(x => out.push((x >>> 24) & 0xff, (x >>> 16) & 0xff, (x >>> 8) & 0xff, x & 0xff));
  return out;
}
function hmacSha256(value, key) {
  let k = key.map(x => x & 0xff);
  if (k.length > 64) k = sha256Bytes(k);
  while (k.length < 64) k.push(0);
  const inner = sha256Bytes(k.map(x => x ^ 0x36).concat(value.map(x => x & 0xff)));
  return sha256Bytes(k.map(x => x ^ 0x5c).concat(inner));
}
const toSigned = arr => arr.map(x => (x & 0xff) > 127 ? (x & 0xff) - 256 : (x & 0xff));

var Utilities = {
  getUuid: () => crypto.randomUUID(),
  newBlob: s => ({ getBytes: () => toSigned(Array.from(new TextEncoder().encode(String(s)))) }),
  computeHmacSha256Signature: (value, key) => toSigned(hmacSha256(value, key)),
  base64Encode: bytes => btoa(String.fromCharCode.apply(null, bytes.map(x => x & 0xff)))
};

// ---- Spreadsheet ----
class MockRange {
  constructor(sheet, row, col, rows, cols) { Object.assign(this, { sheet, row, col, rows, cols }); }
  getValues() {
    const out = [];
    for (let r = 0; r < this.rows; r++) {
      const line = [];
      for (let c = 0; c < this.cols; c++) {
        const v = (this.sheet.data[this.row - 1 + r] || [])[this.col - 1 + c];
        line.push(v === undefined ? '' : v);
      }
      out.push(line);
    }
    return out;
  }
  setValues(values) {
    values.forEach((line, r) => line.forEach((v, c) => {
      const R = this.row - 1 + r, C = this.col - 1 + c;
      while (this.sheet.data.length <= R) this.sheet.data.push([]);
      this.sheet.rawWrites.push(v);
      // Like Sheets: a leading apostrophe marks plain text and is not stored; "=..." becomes a formula
      if (typeof v === 'string' && v.startsWith("'")) v = v.slice(1);
      else if (typeof v === 'string' && v.startsWith('=')) { this.sheet.formulas.push(v); }
      this.sheet.data[R][C] = v;
    }));
    return this;
  }
  setFontWeight() { return this; }
  setNumberFormat() { return this; }
  setBackgrounds(colors) { this.sheet.backgrounds = colors; return this; }
  createTextFinder(text) {
    const range = this;
    const f = {
      matchCase() { return f; }, matchEntireCell() { return f; },
      findAll() {
        const hits = [];
        range.getValues().forEach((line, r) => line.forEach((v, c) => {
          const s = v instanceof Date ? v.toISOString() : String(v);
          if (s === text) hits.push({ getRow: () => range.row + r });
        }));
        return hits;
      }
    };
    return f;
  }
}
class MockSheet {
  constructor(name) { this.name = name; this.data = []; this.hidden = false; this.rawWrites = []; this.formulas = []; }
  getLastRow() { let n = 0; this.data.forEach((l, i) => { if (l.some(v => v !== '' && v !== undefined)) n = i + 1; }); return n; }
  getLastColumn() { return this.data.reduce((m, l) => Math.max(m, l.length), 0); }
  getRange(row, col, rows = 1, cols = 1) { return new MockRange(this, row, col, rows, cols); }
  appendRow(values) { this.getRange(this.getLastRow() + 1, 1, 1, values.length).setValues([values]); }
  hideSheet() { this.hidden = true; }
  setFrozenRows() {}
  setColumnWidth() {}
  clear() { this.data = []; }
  deleteRow(r) { this.data.splice(r - 1, 1); }
  // test helper: rows as objects
  rows() { const h = this.data[0] || []; return this.data.slice(1).map(l => Object.fromEntries(h.map((k, i) => [k, l[i]]))); }
}
var MOCK_SS = { sheets: {} };
var SpreadsheetApp = {
  getActive: () => ({
    getSheetByName: n => MOCK_SS.sheets[n] || null,
    insertSheet: n => (MOCK_SS.sheets[n] = new MockSheet(n)),
    deleteSheet: sh => { delete MOCK_SS.sheets[sh.name]; }
  }),
  flush: () => {}
};

// ---- Cache / Lock / Content ----
var MOCK_CACHE = {};
var CacheService = { getScriptCache: () => ({
  get: k => (k in MOCK_CACHE ? MOCK_CACHE[k] : null),
  put: (k, v) => { MOCK_CACHE[k] = String(v); },
  remove: k => { delete MOCK_CACHE[k]; }
}) };
var LockService = { getScriptLock: () => ({ tryLock: () => true, releaseLock: () => {} }) };
var ContentService = {
  MimeType: { JSON: 'json' },
  createTextOutput: s => ({ content: s, setMimeType() { return this; }, getContent() { return this.content; } })
};
var MOCK_TRIGGERS = [];
var ScriptApp = {
  getProjectTriggers: () => MOCK_TRIGGERS.slice(),
  deleteTrigger: t => { MOCK_TRIGGERS.splice(MOCK_TRIGGERS.indexOf(t), 1); },
  newTrigger: fn => ({ timeBased: () => ({ everyMinutes: m => ({ create: () => {
    const t = { fn, minutes: m, getHandlerFunction: () => fn };
    MOCK_TRIGGERS.push(t);
    return t;
  } }) }) })
};
function resetMocks() { MOCK_SS.sheets = {}; MOCK_TRIGGERS.length = 0; for (const k in MOCK_CACHE) delete MOCK_CACHE[k]; }
