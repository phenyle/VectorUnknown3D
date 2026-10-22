using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Shared state and helpers for the login screen windows (log in, new account, join session,
/// admin). All account checks happen on the game server; see GameServer and SendToGoogle.
/// </summary>
public class LoginSystem : MonoBehaviour
{

    [SerializeField] protected static LoginSystem loginRoot;
    public static DualLogin dualLoginWindow;
    public static NewAccountWindow newAccountWindow;
    public static JoinWindow joinWindow;
    public static PopUpWindow popUpsWindow;
    public static AdminWindow adminWindow;

    protected static UnityEvent messageEvents;   // run when the player clicks OK on a message
    protected static UnityEvent loginEvents;

    protected static PlayerData tmp_player;

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

        // reaching the login screen (first load, or Quit from a game menu) ends any earlier login
        GameServer.Logout();

        if(dualLoginWindow == null)
            dualLoginWindow = GetComponentInChildren<DualLogin>();
        if (newAccountWindow == null)
            newAccountWindow = GetComponentInChildren<NewAccountWindow>();
        if (popUpsWindow == null)
            popUpsWindow = GetComponentInChildren<PopUpWindow>();
        if (joinWindow == null)
            joinWindow = GetComponentInChildren<JoinWindow>();
        if (adminWindow == null)
            adminWindow = GetComponentInChildren<AdminWindow>();
    }

    protected enum queryType
    {
        echelon,
        bunny,
        creating,
        joining,
    }

    protected static queryType query { get; set; }

    protected static void ShowWaiting(string status, bool bunny)
    {
        popUpsWindow.awaitServerPop.SetStatus(status);
        popUpsWindow.awaitServerPop.AwaitingPopUp(bunny);
    }

    protected static void HideWaiting()
    {
        popUpsWindow.awaitServerPop.panel.SetActive(false);
    }

    /// <summary>Shows a message box; onOkay runs when the player clicks OK.</summary>
    protected static void ShowMessage(string text, UnityAction onOkay)
    {
        HideWaiting();
        messageEvents.RemoveAllListeners();
        if (onOkay != null)
            messageEvents.AddListener(onOkay);
        popUpsWindow.GeneralPopUp(text);
    }

    protected void PlayEchelon()
    {
        loginEvents.RemoveAllListeners();
        messageEvents.RemoveAllListeners();
        // load the player's saved progress before the game starts using it
        GameRootMaster.GSFU.LoadPlayerTable(PlayerData.sheets.records, () =>
            GameRootMaster.resourceService.AsynLoadScene(Constants.gameRootSceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start()));
    }

    protected void PlayBunny()
    {
        Debug.LogWarning("Loading Bunny Menu");
        loginEvents.RemoveAllListeners();
        messageEvents.RemoveAllListeners();
        GameRootMaster.GSFU.LoadPlayerTable(PlayerData.sheets.bunRecords, () => SceneManager.LoadScene("menu_scene"));
    }
}
