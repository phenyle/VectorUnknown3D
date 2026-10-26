using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

namespace GoogleSheetsForUnity
{
    /// <summary>
    /// Saves and loads player data through the game server (see GameServer). The class keeps its
    /// old name and its UpdatePlayerTable / RetrievePlayerData methods so the scenes and gameplay
    /// scripts that use it are unchanged; logging in, creating accounts and sessions now happen on
    /// the server, which only ever returns the logged-in player's own data.
    /// </summary>
    public class SendToGoogle : MonoBehaviour
    {
        public bool retrieved { get; set; }   // true once the last RetrievePlayerData finished
        public static UnityEvent callback;    // invoked when a RetrievePlayerData finishes

        private void Start()
        {
            retrieved = false;
            callback = new UnityEvent();
        }

        // ---- Accounts and sessions -------------------------------------------------------------

        /// <summary>Creates an account; onSuccess gets the new player ID (e.g. "ashamr2").</summary>
        public void CreateAccount(string first, string last, string password, string school, string education,
                                  Action<string> onSuccess, Action<string> onError)
        {
            JSONObject req = new JSONObject();
            req["first"] = first;
            req["last"] = last;
            req["password"] = password;
            req["school"] = school;
            req["education"] = education;
            GameServer.Post("createAccount", req, res =>
            {
                StartPlayer(res["users"]);
                onSuccess(res["username"].Value);
            }, onError);
        }

        public void Login(string username, string password, Action onSuccess, Action<string> onError)
        {
            JSONObject req = new JSONObject();
            req["username"] = username;
            req["password"] = password;
            GameServer.Post("login", req, res =>
            {
                StartPlayer(res["users"]);
                onSuccess();
            }, onError);
        }

        /// <summary>Joins a classroom/expo session as a new player with a generated ID and no password.</summary>
        public void JoinSession(string sessionID, Action onSuccess, Action<string> onError)
        {
            JSONObject req = new JSONObject();
            req["sessionID"] = sessionID;
            GameServer.Post("joinSession", req, res =>
            {
                StartPlayer(res["users"]);
                onSuccess();
            }, onError);
        }

        /// <summary>Creates a classroom/expo session (admin accounts only) with the rules in "rules".</summary>
        public void CreateSession(PlayerData.tbl_Users rules, DateTime expires, Action onSuccess, Action<string> onError)
        {
            JSONObject req = new JSONObject();
            req["sessionID"] = rules.sessionID;
            req["isClass"] = rules.isClass;
            req["isExpo"] = rules.isExpo;
            req["isBunny"] = rules.isBunny;
            req["expires"] = expires.ToUniversalTime().ToString("o");
            req["forceStage"] = rules.forceStage;
            req["stageNum"] = rules.stageNum;
            req["difficultyNum"] = rules.difficultyNum;
            GameServer.Post("createSession", req, res => onSuccess(), onError);
        }

        public void Logout()
        {
            GameServer.Logout();
            GameRootMaster.player = new PlayerData();
        }

        private static void StartPlayer(JSONNode users)
        {
            PlayerData player = new PlayerData();
            player.users = JsonUtility.FromJson<PlayerData.tbl_Users>(users.ToString());
            player.SetUserName(player.users.username);
            player.SetSessionID(player.users.sessionID);
            GameRootMaster.player = player;
        }

        // ---- Progress ------------------------------------------------------------------------------

        /// <summary>
        /// Loads the logged-in player's row of a table into GameRootMaster.player, then sets
        /// "retrieved" and invokes "callback". (The column/name arguments are from the old connector;
        /// the server always answers for the logged-in player.)
        /// </summary>
        public void RetrievePlayerData(PlayerData.sheets tableName, string columnName, string playerName)
        {
            LoadPlayerTable(tableName, null);
        }

        public void RetrievePlayerData(PlayerData.sheets tableName, string columnName, string playerName, bool caseSense)
        {
            LoadPlayerTable(tableName, null);
        }

        public void LoadPlayerTable(PlayerData.sheets table, Action onDone)
        {
            retrieved = false;
            if (!GameServer.IsLoggedIn)
            {
                Debug.LogWarning("[SendToGoogle] not logged in; " + table + " not loaded");
                Finish(onDone);
                return;
            }

            JSONObject req = new JSONObject();
            req["table"] = table.ToString();
            GameServer.Post("getRow", req, res =>
            {
                if (res["row"] != null && res["row"].IsObject)
                    ApplyRow(table, res["row"].ToString());
                Finish(onDone);
            }, error => Finish(onDone));
        }

        private void Finish(Action onDone)
        {
            retrieved = true;
            if (onDone != null)
                onDone();
            if (callback != null)
                callback.Invoke();
        }

        /// <summary>Saves the player's current row of a table (creates it if needed).</summary>
        public void UpdatePlayerTable(bool create, PlayerData.sheets sheetname, string searchName)
        {
            if (!GameServer.IsLoggedIn)
            {
                Debug.LogWarning("[SendToGoogle] not logged in; " + sheetname + " not saved");
                return;
            }
            if (!GameServer.SaveData)
                return;   // the player chose not to save data

            string json = RowJson(sheetname);
            Debug.Log("<color=yellow>Saving " + sheetname + "</color>\n" + json);
            JSONObject req = new JSONObject();
            req["table"] = sheetname.ToString();
            req["row"] = JSON.Parse(json);
            GameServer.Post("saveRow", req, null, error => Debug.LogWarning("[SendToGoogle] " + sheetname + " not saved: " + error));
        }

        public void UpdatePlayerAll(bool create)
        {
            foreach (PlayerData.sheets sheet in Enum.GetValues(typeof(PlayerData.sheets)))
                UpdatePlayerTable(create, sheet, GameRootMaster.player.users.username);
        }

        /// <summary>Adds one play of a level or puzzle to the "attempts" tab (see AttemptLog).</summary>
        public void LogAttempt(JSONObject attempt)
        {
            if (!GameServer.IsLoggedIn)
            {
                Debug.LogWarning("[SendToGoogle] not logged in; attempt not saved");
                return;
            }
            if (!GameServer.SaveData)
                return;   // the player chose not to save data
            JSONObject req = new JSONObject();
            req["attempt"] = attempt;
            GameServer.Post("logAttempt", req, null, error => Debug.LogWarning("[SendToGoogle] attempt not saved: " + error));
        }

        private static string RowJson(PlayerData.sheets sheet)
        {
            PlayerData p = GameRootMaster.player;
            switch (sheet)
            {
                case PlayerData.sheets.users: return JsonUtility.ToJson(p.users);
                case PlayerData.sheets.records: return JsonUtility.ToJson(p.records);
                case PlayerData.sheets.bunRecords: return JsonUtility.ToJson(p.bunRecords);
                case PlayerData.sheets.bunny: return JsonUtility.ToJson(p.bunny);
                case PlayerData.sheets.p1_1: return JsonUtility.ToJson(p.p1_1);
                case PlayerData.sheets.p2_1: return JsonUtility.ToJson(p.p2_1);
                case PlayerData.sheets.p2_2: return JsonUtility.ToJson(p.p2_2);
                case PlayerData.sheets.p2_3: return JsonUtility.ToJson(p.p2_3);
                case PlayerData.sheets.p3_1: return JsonUtility.ToJson(p.p3_1);
                case PlayerData.sheets.p3_2: return JsonUtility.ToJson(p.p3_2);
                case PlayerData.sheets.p3_3: return JsonUtility.ToJson(p.p3_3);
                case PlayerData.sheets.p4_1: return JsonUtility.ToJson(p.p4_1);
                case PlayerData.sheets.p4_2: return JsonUtility.ToJson(p.p4_2);
                case PlayerData.sheets.p4_3: return JsonUtility.ToJson(p.p4_3);
                default: return "{}";
            }
        }

        private static void ApplyRow(PlayerData.sheets sheet, string json)
        {
            PlayerData p = GameRootMaster.player;
            switch (sheet)
            {
                case PlayerData.sheets.users: p.users = JsonUtility.FromJson<PlayerData.tbl_Users>(json); break;
                case PlayerData.sheets.records: p.records = JsonUtility.FromJson<PlayerData.tbl_Records>(json); break;
                case PlayerData.sheets.bunRecords: p.bunRecords = JsonUtility.FromJson<PlayerData.tbl_bunRecords>(json); break;
                case PlayerData.sheets.bunny: p.bunny = JsonUtility.FromJson<PlayerData.tbl_Bunny>(json); break;
                case PlayerData.sheets.p1_1: p.p1_1 = JsonUtility.FromJson<PlayerData.tbl_P1_1>(json); break;
                case PlayerData.sheets.p2_1: p.p2_1 = JsonUtility.FromJson<PlayerData.tbl_P2_1>(json); break;
                case PlayerData.sheets.p2_2: p.p2_2 = JsonUtility.FromJson<PlayerData.tbl_P2_2>(json); break;
                case PlayerData.sheets.p2_3: p.p2_3 = JsonUtility.FromJson<PlayerData.tbl_P2_3>(json); break;
                case PlayerData.sheets.p3_1: p.p3_1 = JsonUtility.FromJson<PlayerData.tbl_P3_1>(json); break;
                case PlayerData.sheets.p3_2: p.p3_2 = JsonUtility.FromJson<PlayerData.tbl_P3_2>(json); break;
                case PlayerData.sheets.p3_3: p.p3_3 = JsonUtility.FromJson<PlayerData.tbl_P3_3>(json); break;
                case PlayerData.sheets.p4_1: p.p4_1 = JsonUtility.FromJson<PlayerData.tbl_P4_1>(json); break;
                case PlayerData.sheets.p4_2: p.p4_2 = JsonUtility.FromJson<PlayerData.tbl_P4_2>(json); break;
                case PlayerData.sheets.p4_3: p.p4_3 = JsonUtility.FromJson<PlayerData.tbl_P4_3>(json); break;
            }
        }
    }
}
