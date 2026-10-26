# Vector Unknown: player data review and plan

Reviewed 23 Oct 2026 against the live sheet `VectorUnknownData` and the `OCT-2026-Update` code.
The sheet currently holds one real player (`ashamr`, the team's own testing) plus test leftovers.

**Decisions (23 Oct 2026):** no `Moves` tab (each `attempts` row carries mistake counts and a readable
move log instead); start fresh (old tabs deleted by `startFresh`); default mastery and needs-attention
thresholds; the class/expo session is the class **section** in the reports.

**Implemented:** `attempts` logging from both games (`AttemptLog.cs`), `Students`, `Overview` and
`Data dictionary` tabs built by the server (`buildReports`, every 15 minutes), the old per-puzzle tabs,
`bunny`, `bunRecords` and `statistics` retired. Bugs in section 3 that only affected the retired tabs no
longer matter; the in-game 3D scoreboard still uses `records` as before.

---

## 1. What the sheet holds today

| Tab | Written when | One row per | Status |
|-----|--------------|-------------|--------|
| `users` | login, puzzle exit, quit | player | OK. `tot_time` = lifetime seconds in the 3D app incl. menus/idle, not learning time. |
| `auth` (hidden) | sign-up | player | OK, never shown. **Cleanup:** one orphan `tesacc` row left from testing. |
| `sessions` | teacher creates a class/expo session | session | Empty only because no session has been created yet. **Keep.** |
| `records` | every 3D puzzle completion | player (overwritten) | Mixed "best run" snapshots and running totals; several columns are always 0 (see 3). **Cleanup:** `p1_1_time` and `note` columns are junk from my server test. |
| `p1_1` | Puzzle 1 completed | 3D attempt | Raw answers only (`"s, x, y, z"` per line), no right/wrong, no target. |
| `p2_1..p2_3` | Puzzle 2 completed | 3D attempt | Empty so far (not played). **When played, every per-ship column will still be empty: bug.** |
| `p3_1..p3_3` | Puzzle 3 completed | 3D attempt | Only stage 1 played. Moves list only, no outcome per move. |
| `p4_1..p4_3` | Puzzle 4 completed | 3D attempt | Richest 3D data, but stage-1 "expected answer" columns are placeholders and `avg_score` is always 0. |
| `bunny` | 2D level won or failed | 2D attempt | Problem + steps, but **no outcome column**, no stars/score, abandoned attempts never recorded. |
| `bunRecords` | 2D level started | player | Just counters of how many times each level/difficulty was started. |
| `statistics` | 2D level won or failed | 2D attempt | Same events as `bunny`, as cryptic codes spread over ~100 columns. Redundant. |

**Empty tabs, keep or remove?** None of the empty tabs is dead: `sessions` fills when a teacher uses
classroom/expo mode, and `p2_*`, `p3_2/3`, `p4_2/3` fill when those puzzles are played. But in the plan
below, the 12 per-puzzle tabs, `records`, `bunRecords` and `statistics` are replaced by three uniform
tabs, so they would be **archived** (moved to a separate "raw" spreadsheet), not kept in the
educator-facing sheet.

---

## 2. The cryptic columns, decoded

### 2D Bunny game (`bunny` tab)
The puzzle: reach the goal with `x·v1 + y·v2`, choosing two of four vector cards and the two scalars.

| Column | Meaning |
|--------|---------|
| `stage` | Level 0-7 (0 = tutorial). |
| `level` | Difficulty 1 = easy, 2 = medium, 3 = hard (named "level" in the code). |
| `answer` | The goal point, e.g. `(2.00, 8.00)`. |
| `a`, `b` | The two vectors that generate the intended solution. |
| `c`, `d` | Scalar multiples of `a` and `b` (distractor cards that are parallel to the real ones, `c = k·a`, `d = j·b`). |
| `x`, `y` | Intended scalars: `answer = x·a + y·b`. |
| `k`, `j` | The multiples used to make `c` and `d`. |
| `idealSteps` | `|x| + |y| + 2` style estimate of the fewest clicks. |
| `steps` | **Not moves.** Every formula *edit* (each card drag and each +/- click), as `( c1 * < x1, y1>) + ( c2 * < x2, y2>)`. |
| `results` | The vector each of those edits would produce. |
| `time` | Seconds when the last edit happened. |

### `statistics` tab (same 2D events, as codes)
`10-23-2026-10-18-37` = local session start, `L1..L7/LT` = level chosen, `A:2*(0,1)+2*(1,3)` = intended
answer, `Cs(..)(..)(..)(..)` = the four cards, `K1(x,y)` = key positions (level 3), then every formula
edit, then `H <formula>` (hit) or `M <formula>` (miss) with the final submitted formula.

### 3D game
| Tab / column | Math task | Meaning |
|--------------|-----------|---------|
| `p1_1.obs_answers1..4` | Puzzle 1: displacement between 3D points as `s·(x,y,z)` | Every submission for question 1-4, one per line `s, x, y, z`. No target, no correctness. |
| `records.p1_answers` | | Correct answers of the *fastest* run only. |
| `p2_*` | Puzzle 2: hit a ship = express a point in a non-standard basis (choose basis "cannonball" + coefficient "cannon") | Per-ship columns never filled (bug). Totals use integer division. |
| `p3_*.obs_movesList` | Puzzle 3: span of two 3D vectors `(1,0,0)` and `(0,1,b)`, tilt the plane to roll the ball out | `move#, time, span1 xyz, span2 xyz` per submit. No success flag per move. |
| `p4_*.obsN_*` | Puzzle 4: `s1·v1 + s2·v2 = T` with 2 of 4 cards (1D, 2D, 3D stages) | `ans` = target T, `a/b` = expected cards, `c/d` = distractors, `x/y` = expected scalars, `Ideal` = fewest moves, `steps` = UI edits, `attempts` = every edit with time. |

---

## 3. Data-quality problems found

1. **2D outcome not recorded as a column**; win/miss only exists as `H`/`M` codes in `statistics`. Stars/score stay in the browser.
2. **2D "steps" are UI edits, not submitted moves.** The moves the player actually made (pressing GO) are not separated.
3. **2D abandoned or restarted attempts are lost** (rows are only written on win/fail; Restart clears the steps).
4. **2D level 3 false successes** before today's fix (lock reached without keys logged as `H`). Filter those older rows.
5. **3D Puzzle 2 per-ship data is never saved** (`obsID` is only set in an unused method), totals use integer division, `records.p2_N_score` keeps the *worst* score, `p2_*_questions` never written.
6. **3D Puzzle 4 stage 1** expected-answer columns are placeholders; `avg_score` never written; steps count UI edits so the score can exceed 1.
7. **3D Puzzle 1** answers have no correctness flag; a replay in the same login appends to the previous run; the row key (`ashamr21`) can collide with another player (`ashamr2` attempt 1) and overwrite it.
8. **3D Puzzle 3** has no score; `records.p3_N_score` always 0.
9. **Abandoned 3D puzzles write nothing** (only completed puzzles produce a row), so struggle that ends in quitting is invisible.
10. **`records` mixes "best run" snapshots with totals**, so its columns can't be compared with each other.
11. **Times are inconsistent** (some count walking/menus, some only console time), dates are the device's local format with no time zone, and decimals use the device's locale.
12. **Hard to read**: vectors as `(2.00, 8.00)` text, formulas as code strings, everything one-row-per-player-wide (up to 117 columns).

---

## 4. Who the data is for

A math educator teaching linear algebra who wants to know, per student and per class:
- which concepts the student **has mastered**, is **developing**, or is **stuck on**;
- **what kind of mistake** they make (so remediation can target it);
- whether they **improve** with practice, and how much effort/time it took;
- who needs attention **now**.

### Concept map (what each level exercises)

| Concept | 2D Bunny | 3D Echelon Seas |
|---------|----------|-----------------|
| Vectors as displacement, scalar multiplication | tutorial | Puzzle 1 |
| Linear combination of two vectors | L1 (path preview on), L2 (preview off) | Puzzle 4 stage 2 |
| Planning several combinations / reachability | L3 (keys) | |
| Span of one vector (line through origin) | L4, L6 (hidden targets) | Puzzle 4 stage 1 (1D) |
| Affine line `p + t·v` | L5, L7 (hidden targets) | |
| Coordinates in a non-standard basis | | Puzzle 2 |
| Span of two vectors in 3D (plane) | | Puzzle 3 |
| Linear combinations in 3D | | Puzzle 4 stage 3 |

### Mistake categories the game can detect automatically (per submitted move)
For a move `c1·v1 + c2·v2 = r` against target `T`:
- **Correct**.
- **Sign error**: `r = -T`, or one term has the wrong sign.
- **Component swap**: `r = (Ty, Tx)`.
- **Right vectors, wrong scalar(s)**: used the intended pair but scalars off (scalar multiplication).
- **Wrong vector pair**: chose parallel/dependent cards (`a` with its multiple `c`), i.e. cannot span; a span / linear-independence issue.
- **Partial**: one term right, the other wrong.
- **Overshoot/undershoot**: `r` on the line through `T` but wrong length.
- **Rapid guessing**: several submits within a few seconds each with no consistent improvement.

---

## 5. Proposed sheet layout (educator-facing)

Every row gets a **server timestamp** (one time zone), the player ID, school, education level, and the
class/expo session. Vectors are written readably as `⟨2, 8⟩`. Only these tabs are visible to teachers:

### `Overview` (dashboard, rebuilt automatically)
Per level/puzzle: players, attempts, completion rate, median time, median moves vs ideal, the most
common mistake type. Filterable by session (class section).

### `Students` (one row per player)
Player ID, school, education, session, first/last played, active minutes, levels attempted/completed,
**mastery per concept** (Mastered / Developing / Not yet / Not attempted, colour-coded), most frequent
mistake type, trend (improving / flat / declining), and a **"Needs attention"** flag with the reason
(e.g. "3 failed attempts on span", "guessing on L5", "stalled 10 min on Puzzle 2").

### `Attempts` (one row per level/puzzle play, 2D and 3D in the same columns)
`time, player, session, game (2D/3D), level/puzzle, stage, difficulty, concept, problem (target and
given vectors, readable), outcome (completed / failed / abandoned / restarted), duration (active
seconds), moves submitted, ideal moves, efficiency (ideal / submitted), first-move correct, final answer,
mistakes by type (counts), stars/score`.

### `Moves` (one row per submitted move, i.e. GO / Fire / Submit, not every click)
`time, player, attempt ID, move #, seconds since start, target, vectors chosen, scalars, result,
correct?, distance to target before/after, mistake type`.

### `Data dictionary`
Plain-language meaning of every column and each mistake type, so the sheet is self-explanatory.

### Behind the scenes (hidden or separate file)
`users`, `auth`, `sessions` (needed for logins and classes), and an archive of the current raw tabs.

Mastery rule (proposal, easy to tune in the script): per concept, over the student's last 3 attempts
at that concept, **Mastered** = ≥ 2 completed with efficiency ≥ 0.6 and first move correct at least once;
**Developing** = at least one completion; **Not yet** = attempts but no completion.

---

## 6. Plan

**Phase 0: tidy the sheet now (no game change, ~1 hour).**
Delete the orphan `tesacc` auth row and the `p1_1_time`/`note` junk columns; add the `Data dictionary`
tab; add a readable decoded view of the existing `bunny` and `p4_1` data; mark pre-23-Oct level-3 rows.

**Phase 1: fix the data bugs in the game (~1 day + build).**
Items 1-11 in section 3: record 2D outcome/stars/abandon/restart, separate submitted moves from edits,
fix Puzzle 2 `obsID`/integer division/worst-score, Puzzle 4 stage-1 expected answer and `avg_score`,
Puzzle 1 correctness flag, reset, and row key, write a row when a 3D puzzle is abandoned, use server time
and culture-independent numbers.

**Phase 2: new logging (~2-3 days + build).**
Two new server actions, `logAttempt` and `logMove`, with validation like the existing ones; the game
sends one attempt row per play and one move row per submission, with the mistake type computed in the
game (it knows the target and the intended vectors). The old per-puzzle tabs stop being written; their
existing rows move to the archive.

**Phase 3: educator views (~2 days).**
An Apps Script time trigger (e.g. every 15 minutes) rebuilds `Overview` and `Students` from `Attempts`
and `Moves`, with conditional formatting, per-session filters, and the "Needs attention" rules. Optional
later: a Looker Studio dashboard on the same tabs.

**Verification for each phase:** offline demo + the server test page, then one real class-like run with
2-3 test accounts against the live sheet, then remove the test rows.

---

## 7. Privacy

Richer per-student learning data makes these education records more sensitive even with pseudonymous
IDs. Before collecting from real students: keep the sheet shared only with the research team, confirm the
IRB protocol covers move-level gameplay logging and the "needs attention" flags, and decide a retention
period. Raw archives can be dropped once summarized if the protocol requires it.

## 8. Decisions needed

1. Approve the target layout (section 5), or trim it (e.g. skip `Moves` and keep only `Attempts`).
2. Keep the existing rows (archive) or start fresh when Phase 2 ships?
3. Mastery and "needs attention" thresholds (section 5 defaults are a starting point).
4. Should "session" double as the class section the teacher filters by?
