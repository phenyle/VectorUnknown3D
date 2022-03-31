using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class JoinWindow : LoginSystem
{
    //JOIN SESSION
    [Header("Join Session")]
    public Transform panelJoin;
    public InputField js_Name;
    public Button js_login;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        js_login.enabled = false; //disable button so player can't spam click login and overload database with login requests
        query = 3;
        checkingConnection = true;
        GameRootMaster.GSFU.VerifyDatabaseConnection();
    }

    public void JS_ClickBackBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        js_Name.text = "";
        loginWindow.gameObject.SetActive(true);
        joinWindow.gameObject.SetActive(false);
    }
    #endregion


}
