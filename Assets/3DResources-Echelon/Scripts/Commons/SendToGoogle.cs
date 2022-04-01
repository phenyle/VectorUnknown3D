using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GoogleSheetsForUnity
{
    public class SendToGoogle : MonoBehaviour
    {
        //A temporary holder when retrieving data
        public PlayerData tmp_player;

        public bool retrieved { get; set; }
        public static UnityEvent callback;

        public static string cellval { get; set; }

        public static List<List<string>> tableVals = new List<List<string>>();

        //SearchValues
        private static string searchTable;
        private static string searchColumn;
        private static string searchPlayer;

        private void Start()
        {
            tmp_player = new PlayerData();
            retrieved = false;
            callback = new UnityEvent();
        }

        private void OnEnable()
        {
            // Suscribe for catching cloud responses.
            Drive.responseCallback += HandleDriveResponse;
        }

        private void OnDisable()
        {
            // Remove listeners.
            Drive.responseCallback -= HandleDriveResponse;
        }

        public void SaveNewPlayer()
        {
            tmp_player = GameRootMaster.player;
            string jsonPlayer = JsonUtility.ToJson(tmp_player.users);

            Debug.Log("<color=yellow>Creating the following player table: " + PlayerData.sheets.users.ToString() + "\n</color>" + jsonPlayer);

            Drive.CreateObject(jsonPlayer, PlayerData.sheets.users.ToString());
        }

        public void UpdatePlayerTable(bool create, PlayerData.sheets sheetname, string searchName)
        {
            tmp_player = GameRootMaster.player;

            string jsonPlayer = "";
            string columnName = "usernameAttempt";

            switch (sheetname)
            {
                case PlayerData.sheets.users:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.users);
                    columnName = "username";
                    break;
                case PlayerData.sheets.records:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.records);
                    columnName = "username";
                    break;
                case PlayerData.sheets.bunRecords:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.bunRecords);
                    columnName = "username";
                    break;
                case PlayerData.sheets.bunny:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.bunny);                   
                    break;
                case PlayerData.sheets.p1_1:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p1_1);
                    break;
                case PlayerData.sheets.p2_1:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p2_1);
                    break;
                case PlayerData.sheets.p2_2:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p2_2);
                    break;
                case PlayerData.sheets.p2_3:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p2_3);
                    break;
                case PlayerData.sheets.p3_1:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p3_1);
                    break;
                case PlayerData.sheets.p3_2:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p3_2);
                    break;
                case PlayerData.sheets.p3_3:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p3_3);
                    break;
                case PlayerData.sheets.p4_1:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p4_1);
                    break;
                case PlayerData.sheets.p4_2:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p4_2);
                    break;
                case PlayerData.sheets.p4_3:
                    jsonPlayer = JsonUtility.ToJson(tmp_player.p4_3);
                    break;
            }
            Debug.Log("<color=yellow>Sending following player table: " + sheetname.ToString() + "\n</color>" + jsonPlayer);

            // Look in the 'PlayerInfo' table, for an object of name as specified, and overwrite with the current obj data.
            Drive.UpdateObjects(sheetname.ToString(), columnName, searchName, jsonPlayer, create, true);
        }

        public void UpdatePlayerAll(bool create)
        {
            tmp_player = GameRootMaster.player;
            PlayerData.sheets sheetName;
            int length = System.Enum.GetNames(typeof(PlayerData.sheets)).Length;

            string jsonPlayer = "";
            // Get the json string of the object.
            for (int i = 0; i < length; i++)
            {
                jsonPlayer = "";
                sheetName = (PlayerData.sheets)i;

                switch (i)
                {
                    case 0: //users
                        jsonPlayer = JsonUtility.ToJson(tmp_player.users);
                        break;
                    case 1: //records
                        jsonPlayer = JsonUtility.ToJson(tmp_player.records);
                        break;
                    case 2: //bunRecords
                        jsonPlayer = JsonUtility.ToJson(tmp_player.bunRecords);
                        break;
                    case 3: //bunny
                        jsonPlayer = JsonUtility.ToJson(tmp_player.bunny);
                        break;
                    case 4: //p1-1
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p1_1);
                        break;
                    case 5: //p2-1
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p2_1);
                        break;
                    case 6: //p2-2
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p2_2);
                        break;
                    case 7: //p2-3
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p2_3);
                        break;
                    case 8: //p3-1
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p3_1);
                        break;
                    case 9: //p3-2
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p3_2);
                        break;
                    case 10: //p3-3
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p3_3);
                        break;
                    case 11: //p4-1
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p4_1);
                        break;
                    case 12: //p4-2
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p4_2);
                        break;
                    case 13: //p4-3
                        jsonPlayer = JsonUtility.ToJson(tmp_player.p4_3);
                        break;
                }
                Debug.Log("<color=yellow>Sending following player table: " + sheetName.ToString() + "\n</color>" + jsonPlayer);

                // Look in the 'PlayerInfo' table, for an object of name as specified, and overwrite with the current obj data.
                Drive.UpdateObjects(sheetName.ToString(), "username", tmp_player.users.username, jsonPlayer, create, true);
            
            }
        }



        public void RetrieveCellData(string tableName, string column, string row) 
        {
            retrieved = false;
            Drive.GetCellValue(tableName, column, row, true);
        }

        public void GetTable(string tableName)
        {
            retrieved = false;
            searchTable = tableName;
            Drive.GetTable(tableName, true);
        }


        public void RetrievePlayerData(PlayerData.sheets tableName, string columnName, string playerName)
        {
            retrieved = false; //when first calling retrieve, reset trigger
            searchTable = tableName.ToString();
            searchColumn = columnName;
            searchPlayer = playerName;

            Debug.Log("<color=yellow>Retrieving from the worksheet table: </color>" + searchTable + "\n" +
                "<color=yellow>Under the column: </color>" + searchColumn + "\n" +
                "<color=yellow>Having the name: </color>" + searchPlayer);

            // Get any objects from table <searchTable> with value <searchPlayer> in the field called <searchColumn>.            

            Drive.GetObjectsByField(searchTable, searchColumn, searchPlayer, true);
        }

        public void RetrievePlayerData(PlayerData.sheets tableName, string columnName, string playerName, bool caseSense)
        {
                retrieved = false; //when first calling retrieve, reset trigger
                searchTable = tableName.ToString();
                searchColumn = columnName;

                if (caseSense)
                    searchPlayer = playerName;
                else
                    searchPlayer = playerName.ToLower();

                Debug.Log("<color=yellow>Retrieving from the worksheet table: </color>" + searchTable + "\n" +
                    "<color=yellow>Under the column: </color>" + searchColumn + "\n" +
                    "<color=yellow>Having the name: </color>" + searchPlayer);

                // Get any objects from table <searchTable> with value <searchPlayer> in the field called <searchColumn>.            

                Drive.GetObjectsByField(searchTable, searchColumn, searchPlayer, true);
        }


        public void SendBunnyArray(string[] array)
        {
            
        }


        /// <summary>
        /// This method simply calls the database to check for a connection to the
        /// database via the debug sheet.  If a connection is found, the method 
        /// HandleDriveResponse() will trigger retrieved to be true.
        /// 
        /// This is important because when creating a new user, if there is no match
        /// for the new users first.last when checking for duplicates (HIGHLY likely
        /// since it's a new users) the Drive will simply error, not respond and 
        /// not even call HandleDriveResponse().
        /// Be sure to unflag "retrieved" after a successful response.
        /// </summary>
        public void VerifyDatabaseConnection()
        {
            retrieved = false;
            Drive.GetObjectsByField("debug", "testConnection", "database");
        }


        public void HandleDriveResponse(Drive.DataContainer dataContainer)
        {
            Debug.Log(dataContainer.msg);
            retrieved = true;

            // First check the type of answer.
            if (dataContainer.QueryType == Drive.QueryType.getObjectsByField)
            {
                tmp_player = new PlayerData();
                string rawJSon = dataContainer.payload;
                Debug.Log(rawJSon);

                switch(dataContainer.objType)
                {
                    case "debug":
                        retrieved = true;
                        break;

                    case "users":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_Users[] tmp_users = JsonHelper.ArrayFromJson<PlayerData.tbl_Users>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_users.Length; i++)
                        {
                            tmp_player.users = tmp_users[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.users = tmp_player.users;
                        break;

                    case "records":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_Records[] tmp_records = JsonHelper.ArrayFromJson<PlayerData.tbl_Records>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_records.Length; i++)
                        {
                            tmp_player.records = tmp_records[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.records = tmp_player.records;
                        break;

                    case "bunRecords":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_bunRecords[] tmp_bunRecords = JsonHelper.ArrayFromJson<PlayerData.tbl_bunRecords>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_bunRecords.Length; i++)
                        {
                            tmp_player.bunRecords = tmp_bunRecords[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.bunRecords = tmp_player.bunRecords;
                        break;

                    case "bunny":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_Bunny[] tmp_bunny = JsonHelper.ArrayFromJson<PlayerData.tbl_Bunny>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_bunny.Length; i++)
                        {
                            tmp_player.bunny = tmp_bunny[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.bunny = tmp_player.bunny;
                        break;

                    case "p1_1":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P1_1[] tmp_p1_1 = JsonHelper.ArrayFromJson<PlayerData.tbl_P1_1>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p1_1.Length; i++)
                        {
                            tmp_player.p1_1 = tmp_p1_1[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p1_1 = tmp_player.p1_1;
                        break;

                    case "p2_1":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P2_1[] tmp_p2_1 = JsonHelper.ArrayFromJson<PlayerData.tbl_P2_1>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p2_1.Length; i++)
                        {
                            tmp_player.p2_1 = tmp_p2_1[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p2_1 = tmp_player.p2_1;
                        break;

                    case "p2_2":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P2_2[] tmp_p2_2 = JsonHelper.ArrayFromJson<PlayerData.tbl_P2_2>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p2_2.Length; i++)
                        {
                            tmp_player.p2_2 = tmp_p2_2[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p2_2 = tmp_player.p2_2;
                        break;

                    case "p2_3":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P2_3[] tmp_p2_3 = JsonHelper.ArrayFromJson<PlayerData.tbl_P2_3>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p2_3.Length; i++)
                        {
                            tmp_player.p2_3 = tmp_p2_3[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p2_3 = tmp_player.p2_3;
                        break;

                    case "p3_1":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P3_1[] tmp_p3_1 = JsonHelper.ArrayFromJson<PlayerData.tbl_P3_1>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p3_1.Length; i++)
                        {
                            tmp_player.p3_1 = tmp_p3_1[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p3_1 = tmp_player.p3_1;
                        break;

                    case "p3_2":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P3_2[] tmp_p3_2 = JsonHelper.ArrayFromJson<PlayerData.tbl_P3_2>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p3_2.Length; i++)
                        {
                            tmp_player.p3_2 = tmp_p3_2[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p3_2 = tmp_player.p3_2;
                        break;

                    case "p3_3":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P3_3[] tmp_p3_3 = JsonHelper.ArrayFromJson<PlayerData.tbl_P3_3>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p3_3.Length; i++)
                        {
                            tmp_player.p3_3 = tmp_p3_3[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p3_3 = tmp_player.p3_3;
                        break;

                    case "p4_1":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P4_1[] tmp_p4_1 = JsonHelper.ArrayFromJson<PlayerData.tbl_P4_1>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p4_1.Length; i++)
                        {
                            tmp_player.p4_1 = tmp_p4_1[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p4_1 = tmp_player.p4_1;
                        break;

                    case "p4_2":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P4_2[] tmp_p4_2 = JsonHelper.ArrayFromJson<PlayerData.tbl_P4_2>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p4_2.Length; i++)
                        {
                            tmp_player.p4_2 = tmp_p4_2[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p4_2 = tmp_player.p4_2;
                        break;

                    case "p4_3":
                        // Parse from json to the desired object type.
                        PlayerData.tbl_P4_3[] tmp_p4_3 = JsonHelper.ArrayFromJson<PlayerData.tbl_P4_3>(rawJSon);

                        //Retrieve returns the LAST match in the column
                        for (int i = 0; i < tmp_p4_3.Length; i++)
                        {
                            tmp_player.p4_3 = tmp_p4_3[i];
                            Debug.Log("<color=yellow>Object retrieved from the cloud and parsed: \n</color>");
                        }
                        GameRootMaster.player.p4_3 = tmp_player.p4_3;
                        break;
                }

            }

            if(dataContainer.QueryType == Drive.QueryType.getCellValue)
            {
                cellval = dataContainer.payload;
            }

            callback.Invoke();

        }
    }

}