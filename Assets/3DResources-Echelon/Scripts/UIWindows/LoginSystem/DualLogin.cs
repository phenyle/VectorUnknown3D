using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsForUnity;
using System.Text;


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
        messageEvents.AddListener(TimedOut);
        popUpsWindow.awaitServerPop.SetStatus("Checking Connection");
        popUpsWindow.awaitServerPop.AwaitingPopUp(true);

        checkingGFSU = true;
        connectionTime = 0.0f;
        ToggleInteractable(false);
        query = queryType.bunny;
        SendToGoogle.callback.AddListener(ConnectionsCheck);
        GameRootMaster.GSFU.VerifyDatabaseConnection();
    }

    public void Click_EchelonSeas()
    {
        messageEvents.AddListener(TimedOut);
        popUpsWindow.awaitServerPop.SetStatus("Checking Connection");
        popUpsWindow.awaitServerPop.AwaitingPopUp(false);

        checkingGFSU = true;
        connectionTime = 0.0f;
        ToggleInteractable(false);
        query = queryType.echelon;
        SendToGoogle.callback.AddListener(ConnectionsCheck);
        GameRootMaster.GSFU.VerifyDatabaseConnection();
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

        GameRootMaster.GSFU.retrieved = false; //reset retrieved flag
        loadingPlayer = false;
        checkingGFSU = false;
        connectionTime = 0.0f;
        dualLoginWindow.ToggleInteractable(true);


        string salt = GameRootMaster.player.users.salt;
        string passattempt = ComputeHash(Encoding.UTF8.GetBytes(password.text), Encoding.UTF8.GetBytes(salt));

        //Check for user/pass match
        if (passattempt.CompareTo(GameRoot3D.player.users.hash) == 0)
        {
            Debug.Log("LOGIN SUCCESFUL");

            //load in other player settings

            GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", username.text.ToLower(), false);
            GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.records, "username", username.text.ToLower(), false);

            tmp_player.users.last_login = System.DateTime.Now.ToString();
            GameRootMaster.GSFU.UpdatePlayerTable(false, PlayerData.sheets.users, GameRoot3D.player.users.username);
            ClearFields();

            if (query == queryType.bunny)
                PlayBunny();
            else if (query == queryType.echelon)
                PlayEchelon();

        }
        //player password wrong
        else
        {
            Debug.Log("LOGIN ERROR: wrong user/pass");
            ClearFields();
            //errorText.text = "Username or password Incorrect or Not Found";
            //panelError.gameObject.SetActive(true);
        }

    }

    public void TimedOut()
    {
        messageEvents.RemoveAllListeners();
        connectionTime = 0.0f;
        checkingGFSU = false;
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
