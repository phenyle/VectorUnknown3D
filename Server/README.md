# Vector Unknown game server

The game stores accounts, progress and gameplay data in a Google Sheet through a small Google Apps
Script web app (`AppsScript/Code.gs`). The game never opens the sheet itself: it sends JSON requests
to the web app, which checks them and reads or writes the sheet.

## What's in the sheet

For teachers (the `Data dictionary` tab explains every column):

| Tab | One row per | Contents |
|-----|-------------|----------|
| `Students` | player | Mastery per linear algebra concept (Mastered / Developing / Not yet / Not attempted), most common mistake, trend, and a **Needs attention** flag with the reason. Rebuilt every 15 minutes. |
| `Overview` | activity, per class section | Plays, completion rate, median time and efficiency, first-move accuracy, most common mistake. Rebuilt every 15 minutes. |
| `attempts` | play of a level or puzzle | The problem, outcome (completed / failed / restarted / abandoned), time, moves vs. fewest possible, mistake counts by type, and a readable log of every move. |
| `Data dictionary` | column | Plain-language meaning of every column and mistake type. |

Behind the scenes:

| Tab | One row per | Contents |
|-----|-------------|----------|
| `users` | player | Player ID, school, education level, settings, session rules, `admin` flag. **No names, no passwords.** |
| `sessions` | classroom / expo session | Session name (also the class **section** in the reports), who created it, expiry and rules. |
| `auth` (hidden) | player | Salted password hash (PBKDF2-SHA256). Never sent to the game. |
| `records` (hidden) | player | 3D best times and run counts for the in-game scoreboard. |

**Mistake types** are worked out by the game for every move it can judge: sign error, swapped
components or scalars, wrong scalar (right direction or right vectors, wrong multiple), chose parallel
vectors (they can't reach the target), one term right, and other. Wrong moves made within 3 seconds
of the previous move are counted as rapid guesses.

**Player IDs** are the first 3 letters of the first name plus the first 3 letters of the last name,
lowercased, with accents and non-letters removed (Ashish Amresh → `ashamr`). If that ID is taken the
next number is added (`ashamr2`, `ashamr3`, …). The names themselves are never stored. Classroom and
expo players get `<session>-<6 random characters>` and have no password.

These IDs are pseudonymous, not anonymous: within a class an ID like `ashamr` is easy to link to a
student, especially next to their school. Treat the sheet as student records: keep sharing
restricted to the research team, and check NAU's FERPA and IRB requirements before collecting data.

## Security

- Passwords are hashed and checked on the server; the old design sent hashes to the browser.
- Login returns a random session token (6 hours, renewed while playing). Every save carries it, and
  the server stamps each row with the token's player ID, so players can only write and read their own rows.
- 5 wrong passwords lock that ID for 10 minutes. Sign-ups are rate-limited.
- Only known tabs and column names are accepted, and all text is stored as plain text (a value such as
  `=IMPORTDATA(...)` is never run as a formula).
- There are no passwords or keys inside the game build; the web app URL is public by design.

## Setting it up (one time)

1. **Create the sheet.** In your NAU Google account, create a new Google Sheet (e.g. "Vector Unknown data").
   Keep its sharing restricted to the people who need the data.
2. **Add the script.** In the sheet: **Extensions → Apps Script**. Delete the sample code, paste the whole
   of `AppsScript/Code.gs`, and save.
3. **Use the manifest (recommended).** In the Apps Script editor: **Project Settings → Show "appsscript.json"
   manifest file in editor**, then replace the contents of `appsscript.json` with `AppsScript/appsscript.json`.
4. **Create the tabs.** Choose the `setup` function in the toolbar and click **Run**. Google asks you to
   authorize the script (access to this spreadsheet, a timer to rebuild the reports, and a menu in the
   sheet). When it finishes, the sheet has its tabs, `auth` and `records` are hidden, and `Students`
   and `Overview` are rebuilt every 15 minutes. **Vector Unknown → Rebuild Students and Overview now**
   in the sheet's menu refreshes them on demand.
5. **Deploy.** **Deploy → New deployment → Select type: Web app.** Set **Execute as: Me** and
   **Who has access: Anyone**, then **Deploy** and copy the **Web app URL**
   (`https://script.google.com/macros/s/…/exec`).
   *"Anyone" is required because players aren't signed in to Google; the script itself checks every request.*
6. **Check it.** Open the URL in a browser. You should see
   `{"ok":true,"service":"Vector Unknown game server",...}`. If you're signed in to Google the page may
   also show a Google banner (such as "End visitor session"); it doesn't affect the game.
7. **Point the game at it.** Put the URL in `Assets/Resources/GameServerUrl.txt` and rebuild.

If your NAU Google Workspace doesn't allow "Anyone" access for web apps, ask NAU ITS; the game can't
use a deployment that requires a Google sign-in.

## Making a teacher or researcher an admin

They create an account in the game as usual. Then in the `users` tab, set their `admin` cell to `TRUE`.
Admins see the admin panel after logging in and can create classroom and expo sessions.

## Speed and limits

Measured on the first deployment (October 2026): logging in takes about 5 seconds, creating an
account about 10 seconds, and loading or saving progress about 3 seconds. Most of that is Apps
Script's own start-up time; password hashing adds about 2 seconds to logins and sign-ups
(`CONFIG.HASH_ITERATIONS`).

Google runs only a limited number of requests for one script at the same time (about 30), so if a
whole class logs in at the same moment a few players may get an error and need to try again.

## Updating the script

Edit the code in the Apps Script editor, then **Deploy → Manage deployments → (edit) → Version: New version → Deploy**.
Updating the existing deployment keeps the same URL, so the game doesn't need rebuilding.

### Moving a sheet from the October 2026 layout to the teacher reports (one time)

The first version wrote `statistics`, `bunny`, `bunRecords` and one tab per 3D puzzle (`p1_1` … `p4_3`).
To switch:

1. Replace `Code.gs` and `appsscript.json` in the Apps Script editor with the new versions and save.
2. Choose **`startFresh`** in the toolbar and click **Run**, then authorize the new permissions. It
   **deletes the old tabs and their data** (and password rows of players no longer in `users`), keeps
   players, passwords and sessions, and runs `setup`.
3. **Deploy → Manage deployments → (edit) → Version: New version → Deploy.**

## Tests

`tests/` runs `Code.gs` in a browser against stand-ins for Google's services (including published
PBKDF2 test vectors). From the repository root:

```bash
python -m http.server 8790 --directory Server
```

Then open `http://localhost:8790/tests/index.html`; it shows the pass/fail summary.
