using UnityEngine;
using UnityEngine.UI;


public class DualLogin : LoginSystem
{
    public GameObject panel;
    public InputField username;
    public InputField password;
    private bool passhide = true;


    [Header("Buttons-------------")]
    public Button btn_bunny;
    public Button btn_echelon;
    public Button btn_join;
    public Button btn_create;
    public Button btn_passpeek;


    public void Start()
    {
        dualLoginWindow = this;
    }


    public void Click_BunnyGame()
    {
        LogIn(queryType.bunny);
    }

    public void Click_EchelonSeas()
    {
        LogIn(queryType.echelon);
    }

    // The server checks the password; on success it returns the player's data and a session token.
    private void LogIn(queryType game)
    {
        string id = username.text.Trim().ToLower();
        if (id.Length == 0 || password.text.Length == 0)
        {
            ToggleInteractable(false);
            ShowMessage("Enter your player ID and password.", TimedOut);
            return;
        }

        query = game;
        ToggleInteractable(false);
        ShowWaiting("Logging In", game == queryType.bunny);
        GameRootMaster.GSFU.Login(id, password.text, LoginToGame, error =>
        {
            ClearFields();
            ShowMessage("LOGIN ERROR:\n" + error, TimedOut);
        });
    }

    public void Click_JoinSession()
    {
        ClearFields();
        //We're not adding the query type yet.
        //We'll add it from the JoinSession script
        query = queryType.joining;
        joinWindow.panel.SetActive(true);
        dualLoginWindow.panel.SetActive(false);
    }

    public void Click_CreateAccount()
    {
        ClearFields();
        //We're not adding the query type yet.
        //We'll add it from the NewAccount script
        newAccountWindow.panel.SetActive(true);
        dualLoginWindow.panel.SetActive(false);
    }

    public void Click_PeekPassword()
    {
        passhide = !passhide;

        if (passhide)
            password.contentType = InputField.ContentType.Password;
        else
            password.contentType = InputField.ContentType.Standard;

        password.ForceLabelUpdate();
    }

    public void LoginToGame()
    {
        Debug.Log("LOGIN SUCCESFUL");
        dualLoginWindow.ToggleInteractable(true);
        ClearFields();

        if(GameRootMaster.player.users.admin)
        {
            HideWaiting();
            adminWindow.panel.SetActive(true);
            dualLoginWindow.panel.SetActive(false);
        }
        else
        {
            if (query == queryType.bunny)
                PlayBunny();
            else if (query == queryType.echelon)
                PlayEchelon();
        }
    }

    public void TimedOut()
    {
        messageEvents.RemoveAllListeners();
        popUpsWindow.generalPopPanel.SetActive(false);
        popUpsWindow.awaitServerPop.panel.SetActive(false);
        ToggleInteractable(true);
    }

    public void ToggleInteractable(bool val)
    {
        btn_bunny.interactable = val;
        btn_echelon.interactable = val;
        btn_join.interactable = val;
        btn_create.interactable = val;
        btn_passpeek.interactable = val;
    }

    private void ClearFields()
    {
        username.text = "";
        password.text = "";
        passhide = true;
        password.contentType = InputField.ContentType.Password;

    }
}
