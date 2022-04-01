using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using GoogleSheetsForUnity;
using System.Text;
using UnityEngine.SceneManagement;
using MyBox;

public class LoginSystem : MonoBehaviour
{

    [SerializeField] protected static LoginSystem loginRoot;
    public static DualLogin dualLoginWindow;
    public static NewAccountWindow newAccountWindow;
    public static JoinWindow joinWindow;
    public static PopUpWindow popUpsWindow;


    protected static UnityEvent messageEvents;
    protected static UnityEvent loginEvents;

    protected static PlayerData tmp_player;

    protected static bool checkingGFSU = false;
    [ReadOnly]
    public float connectionTime = 0.0f;
    private static float connectionAttmpTimeout = 10.0f;

    protected static bool loadingPlayer = false;
    protected static bool newAcctCheck = false;
    protected static bool creatingAcct = false;
    protected static bool joiningSession = false;
    protected static bool bunnyGame = false;



    public void Awake()
    {
        loginRoot = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        messageEvents = new UnityEvent();
        loginEvents = new UnityEvent();

        tmp_player = new PlayerData();

        if(dualLoginWindow == null)
            dualLoginWindow = GetComponentInChildren<DualLogin>();
        if (newAccountWindow == null)
            newAccountWindow = GetComponentInChildren<NewAccountWindow>();
        if (popUpsWindow == null)
            popUpsWindow = GetComponentInChildren<PopUpWindow>();
        if (joinWindow == null)
            joinWindow = GetComponentInChildren<JoinWindow>();
    }

    // Update is called once per frame
    void Update()
    {
        if(checkingGFSU)
        {
            connectionTime += Time.deltaTime;
            if (connectionTime > connectionAttmpTimeout)
            {
                //When creating an account it looks for duplicate names
                //but when it does this, if it cant find name in the database it just
                //doesn't return, which means it won't call GFSU.callback.
                //Essentially, if after our first connection check (to debug) is good
                //but our second connection check (to the username) is bad, then what
                //we need to do is invoke account creation.
                if (query == queryType.creating)
                    loginEvents.Invoke();
                else
                {
                    //Connection time out
                    popUpsWindow.GeneralPopUp("ERROR:\nConnection Timed Out");
                    popUpsWindow.awaitServerPop.panel.SetActive(false);
                    checkingGFSU = false;
                    connectionTime = 0.0f;
                }
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

    protected static queryType query { get; set; }
    protected void ConnectionsCheck()
    {
        connectionTime = 0.0f;
        SendToGoogle.callback.RemoveAllListeners();

        switch (query)
        {
            case queryType.echelon: //query type: Logging into Echelon Game
                                    //call to database for "username" to find login name
                                    //                      panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
                popUpsWindow.awaitServerPop.SetStatus("Finding Player");
                popUpsWindow.awaitServerPop.AwaitingPopUp(false);

                SendToGoogle.callback.AddListener(dualLoginWindow.LoginToGame);
                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", dualLoginWindow.username.text, false);
                break;

            case queryType.bunny: //query type: Creating a new Account
                                  //call to database for "first.last" to check for duplicates
                                  //                     panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Creating Account");
                popUpsWindow.awaitServerPop.SetStatus("Finding Player");
                popUpsWindow.awaitServerPop.AwaitingPopUp(true);

                SendToGoogle.callback.AddListener(dualLoginWindow.LoginToGame);
                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", dualLoginWindow.username.text, false);
                break;

            case queryType.creating: //query type: Joining Session
                loginEvents.AddListener(newAccountWindow.CreatingNewAccount);
                SendToGoogle.callback.AddListener(newAccountWindow.CreatingIncrimentedAccount);
                break;

            case queryType.joining:
                checkingGFSU = false;
                checkingGFSU = false;
                newAccountWindow.panel.SetActive(true);
                break;
        }
    }

    protected void PlayEchelon()
    {
        if (query == queryType.joining)
        {

        }
        else
        {
            loginEvents.RemoveAllListeners();
            messageEvents.RemoveAllListeners();
            SendToGoogle.callback.RemoveAllListeners();
            GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.records, "username", GameRootMaster.player.users.username);
            GameRootMaster.resourceService.AsynLoadScene(Constants.gameRootSceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start() );
        }

    }

    protected void PlayBunny()
    {
        if (query == queryType.joining)
        {

        }
        else
        {
            Debug.LogWarning("Loading Bunny Menu");
            loginEvents.RemoveAllListeners();
            messageEvents.RemoveAllListeners();
            SendToGoogle.callback.RemoveAllListeners();
            GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.bunRecords, "username", GameRootMaster.player.users.username);
            SceneManager.LoadScene("menu_scene");
        }

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

    protected void GotoJoinSession()
    {
        
    }
}
