using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using GoogleSheetsForUnity;

public class LoginSystem : MonoBehaviour
{
    [SerializeField] protected static LoginSystem loginRoot { get; private set; }
    [SerializeField] protected static DualLogin dualLoginWindow { get; private set; }
    [SerializeField] protected static LoginWindow loginWindow { get; private set; }
    [SerializeField] protected static NewAccountWindow newAccountWindow { get; private set; }
    [SerializeField] protected static PopUpWindow popUpsWindow { get; private set; }
    [SerializeField] protected static JoinWindow joinWindow { get; private set; }

    protected static UnityEvent messageEvents;
    protected static UnityEvent loginEvents;

    protected static PlayerData tmp_player;

    protected static int attemptNum = 0;
    protected static bool checkingGFSU = false;
    protected static float connectionTime = 0.0f;
    private static float connectionAttmpTimeout = 8.0f;
    protected static bool checkingConnection = false;

    protected static bool loadingPlayer = false;
    protected static bool newAcctCheck = false;
    protected static bool creatingAcct = false;
    protected static bool joiningSession = false;
    protected static bool bunnyGame = false;



    // Start is called before the first frame update
    void Start()
    {
        loginRoot = this;
        dualLoginWindow = GetComponentInChildren<DualLogin>();
        loginWindow = GetComponentInChildren<LoginWindow>();
        newAccountWindow = GetComponentInChildren<NewAccountWindow>();
        popUpsWindow = GetComponentInChildren<PopUpWindow>();
        joinWindow = GetComponentInChildren<JoinWindow>();

        messageEvents = new UnityEvent();
        loginEvents = new UnityEvent();

        tmp_player = new PlayerData();


        newAccountWindow.panelNewAcct.gameObject.SetActive(false);
        joinWindow.panelJoin.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(checkingGFSU)
        {
            connectionTime += Time.deltaTime;
            if (connectionTime > connectionAttmpTimeout)
            {
                //Connection time out

            }
        }
    }

    protected enum queryType
    {
        echelon,
        bunny,
        creating,
        joining,
    }

    protected static queryType query { private get; set; }
    protected static void ConnectionsCheck()
    {
        checkingGFSU = false;
        connectionTime = 0.0f;
        SendToGoogle.callback.RemoveAllListeners();

        switch (query)
        {
            case queryType.echelon: //query type: Logging into Echelon Game
                    //call to database for "username" to find login name
                    //                      panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
     //           SendToGoogle.callback.AddListener(EchelonLogin);
                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", dualLoginWindow.username.text, false);
                loadingPlayer = true;
                break;

            case queryType.bunny: //query type: Creating a new Account
                    //call to database for "first.last" to check for duplicates
                    //                     panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Creating Account");
                    //SendToGoogle.callback.AddListener(CreateAccount);

                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "first_last", newAccountWindow.na_first.text.ToLower() + "." + newAccountWindow.na_last.text.ToLower(), false);
                creatingAcct = true;
                break;

            case queryType.creating: //query type: Joining Session
                    //                    panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Session");

                //GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", js_Name.text);
                joiningSession = true;
                break;
            case queryType.joining:
                //                    panel_awaitServer.gameObject.SetActive(false);

                newAccountWindow.NA_setSchools();
       //         LG_ClearLoginFields();

        //        panelLogin.gameObject.SetActive(false);
                newAccountWindow.panelNewAcct.gameObject.SetActive(true);
                break;
            case 5: //query type: Logging into Bunny Game
                    //call to database for "username" to find login name
                    //                    panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
                GameRoot3D.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text, false);
                loadingPlayer = true;
                break;



        }


        protected static string ComputeHash(byte[] bytesToHash, byte[] salt)
    {
        var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
        return Convert.ToBase64String(byteResult.GetBytes(24));
    }

    protected static string GenerateSalt()
    {
        var bytes = new byte[128 / 8];
        var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

}
