using UnityEngine;
using UnityEngine.UI;

public class JoinWindow : LoginSystem
{
    //JOIN SESSION
    [Header("Join Session")]
    public GameObject panel;
    public InputField sessionName;
    public Button btn_Join, btn_Back;


    public void Start()
    {
        joinWindow = this;
        panel.SetActive(false);
    }


    // The server checks the session exists and hasn't expired, then creates a new player for it
    // (a generated ID with no password) carrying the session's rules.
    public void JS_ClickLoginBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        string id = sessionName.text.Trim().ToLower();
        btn_Join.enabled = false; //disable button so player can't spam click login and overload database with login requests
        btn_Back.enabled = false;
        if (id.Length == 0)
        {
            ShowMessage("Enter the session name you were given.", ResetFields);
            return;
        }

        query = queryType.joining;
        ShowWaiting("Joining Session", true);
        GameRootMaster.GSFU.JoinSession(id, JoinSession, error => ShowMessage("LOGIN ERROR:\n" + error, ResetFields));
    }

    public void JoinSession()
    {
        Debug.Log("Join Session SUCCESFUL");
        PlayerData.tbl_Users session = GameRootMaster.player.users;

        //if the loaded sessions is from an Expo, trigger this machine to
        //run in the Expo state using the session name
        if (session.isExpo)
        {
            GameRootMaster.isExpo = true;
            GameRootMaster.expoName = session.sessionID;
        }
        if (session.isClass)
        {
            GameRootMaster.isClass = true;
            GameRootMaster.expoName = session.sessionID;
        }

        btn_Join.enabled = true;
        btn_Back.enabled = true;
        joinWindow.gameObject.SetActive(false);
        dualLoginWindow.gameObject.SetActive(true);

        GameRootMaster.isSession = true;

        if (session.isBunny)
            PlayBunny();
        else
            PlayEchelon();
    }

    public void ResetFields()
    {
        loginEvents.RemoveAllListeners();
        messageEvents.RemoveAllListeners();

        popUpsWindow.awaitServerPop.panel.SetActive(false);

        btn_Join.enabled = true;
        btn_Back.enabled = true;
    }

    public void TimedOut()
    {
        loginEvents.RemoveAllListeners();
        messageEvents.RemoveAllListeners();

        popUpsWindow.generalPopPanel.SetActive(false);
        popUpsWindow.awaitServerPop.panel.SetActive(false);
        btn_Join.enabled = true;
        btn_Back.enabled = true;
    }


    public void JS_ClickBackBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        sessionName.text = "";
        dualLoginWindow.panel.SetActive(true);
        joinWindow.panel.SetActive(false);
    }

}
