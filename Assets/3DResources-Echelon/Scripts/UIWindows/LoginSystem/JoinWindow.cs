using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsForUnity;

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


    public void JS_ClickLoginBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        //       panel_awaitServer.gameObject.SetActive(true);
        //        awaitServePop.SetStatus("Checking Connection");

        messageEvents.AddListener(ResetFields);
        //loginEvents.AddListener(TimedOut);
        popUpsWindow.awaitServerPop.SetStatus("Checking Connection");
        popUpsWindow.awaitServerPop.AwaitingPopUp(true);

        checkingGFSU = true;
        connectionTime = 0.0f;

        btn_Join.enabled = false; //disable button so player can't spam click login and overload database with login requests
        btn_Back.enabled = false;
        query = queryType.joining;
        SendToGoogle.callback.AddListener(ConnectionsCheck);
        GameRootMaster.GSFU.VerifyDatabaseConnection();
    }

    public void JoinSession()
    {
        //database responded and found session name
        //stop waiting for database create new session player
        checkingGFSU = false;
        connectionTime = 0;
        popUpsWindow.awaitServerPop.SetStatus("Joining Session");

        Debug.Log("Session Found");
        DateTime expireDate;
        bool parse = DateTime.TryParse(GameRootMaster.player.users.expires, out expireDate);

        //Check that the session being joined has not expired
        if (DateTime.Compare(DateTime.Now, expireDate) < 0)
        {
            messageEvents.RemoveAllListeners();
            Debug.Log("Join Session SUCCESFUL");

            //Since there is a delay in database response, it's possible
            //for multiple people to "join session" at the same time and 
            //the database tell all of them they're the first to join and give
            //them all the same incrimented username.
            //This will cause problems later when that username is referenced
            //to save the session data.
            //To combat this and ensure that only ONE username matches the
            //sessionID, we'll reuse our salt and hash functions.  But since there
            //is no password for joining a session, instead we'll hash the current
            //DateTime and concatenate the resulting string onto the joining name.
            //This will ensure every joining player has a unique session name no 
            //matter how many people join at the same time.
            string salt = GenerateSalt();
            System.Random rand = new System.Random();
            string randnum = rand.Next(1111, 9999).ToString();
            string hashID = ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString() + randnum), Encoding.UTF8.GetBytes(salt));


            //if the loaded sessions is from an Expo, trigger this machine to
            //run in the Expo state using the session name
            if (GameRootMaster.player.users.isExpo)
            {
                GameRootMaster.isExpo = true;
                GameRootMaster.expoName = sessionName.text;
            }
            if (GameRootMaster.player.users.isClass)
            {
                GameRootMaster.isClass = true;
                GameRootMaster.expoName = sessionName.text;
            }

            if (GameRootMaster.isExpo)
                GameRootMaster.player.SetUserName(GameRootMaster.expoName + hashID);
            else
                GameRootMaster.player.SetUserName(sessionName.text + hashID);

            GameRootMaster.player.users.last_login = System.DateTime.Now.ToString();
            GameRootMaster.player.SetSessionID(sessionName.text);
            GameRootMaster.GSFU.SaveNewPlayer();

            joinWindow.gameObject.SetActive(false);
            dualLoginWindow.gameObject.SetActive(true);

            GameRootMaster.isSession = true;

            if (GameRootMaster.player.users.isBunny)
                PlayBunny();
            else
                PlayEchelon();
        }
        else //session code is valid, but has expired
        {
            Debug.Log("LOGIN ERROR: session expired");

            popUpsWindow.GeneralPopUp("LOGIN ERROR:\nThat session does not exist or has expired");
            popUpsWindow.awaitServerPop.panel.SetActive(false);
            checkingGFSU = false;
            connectionTime = 0.0f;
        }

    }

    public void ResetFields()
    {
        loginEvents.RemoveAllListeners();
        messageEvents.RemoveAllListeners();

        checkingGFSU = false;
        connectionTime = 0.0f;

        popUpsWindow.awaitServerPop.panel.SetActive(false);

        btn_Join.enabled = true;
        btn_Back.enabled = true;
    }

    public void TimedOut()
    {
        loginEvents.RemoveAllListeners();
        messageEvents.RemoveAllListeners();
        checkingGFSU = false;
        connectionTime = 0.0f;

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
