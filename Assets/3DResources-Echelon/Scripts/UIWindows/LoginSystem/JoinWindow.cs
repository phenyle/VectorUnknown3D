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
    public Button btn_Join;


    public void Start()
    {
        joinWindow = this;
        panel.SetActive(false);
    }

    //--------------------------------
    // JOIN SESSION Functions
    //--------------------------------
    #region Join Session
    public void JS_ClickLoginBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        //       panel_awaitServer.gameObject.SetActive(true);
        //        awaitServePop.SetStatus("Checking Connection");

        btn_Join.enabled = false; //disable button so player can't spam click login and overload database with login requests
        query = queryType.joining;
        checkingGFSU = true;
        SendToGoogle.callback.AddListener(JoinSession);
        GameRootMaster.GSFU.VerifyDatabaseConnection();
    }

    private void JoinSession()
    {
        //database responded and found session name
        //stop waiting for database create new session player
        checkingGFSU = false;
        connectionTime = 0;
        Debug.Log("Session Found");

        //Check that the session being joined has not expired
        if (DateTime.Compare(DateTime.Now, DateTime.Parse(GameRootMaster.player.users.expires)) < 0)
        {
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
            string hashID = ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), Encoding.UTF8.GetBytes(salt));

            tmp_player = new PlayerData();

            //if the loaded sessions is from an Expo, trigger this machine to
            //run in the Expo state using the session name
            if (GameRootMaster.player.users.isExpo)
            {
                GameRootMaster.isExpo = true;
                GameRootMaster.expoName = sessionName.text;
                tmp_player.users.isExpo = true;
            }
            if (GameRootMaster.player.users.isClass)
            {
                GameRootMaster.isClass = true;
                GameRootMaster.expoName = sessionName.text;
                tmp_player.users.isClass = true;

            }

            tmp_player.users.stageNum = GameRootMaster.player.users.stageNum;
            tmp_player.users.difficultyNum = GameRootMaster.player.users.difficultyNum;
            tmp_player.users.forceStage = GameRootMaster.player.users.forceStage;

            if (GameRootMaster.isExpo)
            {
                tmp_player.SetUserName(GameRoot3D.expoName + hashID);
                tmp_player.users.isExpo = true;

            }
            else
                tmp_player.SetUserName(sessionName.text + hashID);

            tmp_player.users.last_login = System.DateTime.Now.ToString();
            tmp_player.users.sessionID = sessionName.text;
            GameRootMaster.player = tmp_player;
            GameRootMaster.GSFU.SaveNewPlayer();

            joinWindow.gameObject.SetActive(false);
            dualLoginWindow.gameObject.SetActive(true);


            if (GameRootMaster.player.users.isBunny)
                PlayBunny();
            else
                PlayEchelon();
        }
    }


    public void JS_ClickBackBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        sessionName.text = "";
        dualLoginWindow.panel.SetActive(true);
        joinWindow.panel.SetActive(false);
    }
    #endregion


}
