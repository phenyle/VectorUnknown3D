using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NewAccountWindow : LoginSystem
{
    //NEW ACCOUNT
    [Header("New Account")]
    public Transform panelNewAcct;
    public InputField na_first;
    public InputField na_last;
    public InputField na_pass1;
    public InputField na_pass2;
    public Dropdown na_education;
    public Dropdown na_schools;
    public InputField na_otherSchool;
    public Transform panelNewUserName;
    public Text userNameText;

    private bool na_otherSel = false;

    // Start is called before the first frame update
    void Start()
    {
        na_otherSchool.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (creatingAcct)
        {
            //database responded
            //stop waiting for database and create new player
            //if the database responded, we're almost guaranteed to have a
            //duplicate user with first.last
            if (GameRootMaster.GSFU.retrieved)
            {
                GameRootMaster.GSFU.retrieved = false;
                creatingAcct = false;
                attemptNum = 0;
                loginWindow.na_login.enabled = true;

                tmp_player.users.first_last = na_first.text.ToLower() + "." + na_last.text.ToLower();

                tmp_player.users.ID_num = GameRoot3D.player.users.ID_num;
                tmp_player.users.ID_num++;
                tmp_player.SetUserName(tmp_player.users.first_last + tmp_player.users.ID_num.ToString());

                tmp_player.users.education = na_education.captionText.text;
                tmp_player.users.tot_time = 0.0f;

                string salt = GenerateSalt();
                string hashedpass = ComputeHash(Encoding.UTF8.GetBytes(na_pass1.text), Encoding.UTF8.GetBytes(salt));
                tmp_player.users.hash = "\'" + hashedpass;
                tmp_player.users.salt = "\'" + salt;

                userNameText.text = "<b>Your Username is:</b>\n" +
                                    "<size=28>" + tmp_player.users.username + "</size>";

                if (na_otherSel)
                    tmp_player.users.school = na_otherSchool.text;
                else
                    tmp_player.users.school = na_schools.captionText.text;

                panelNewUserName.gameObject.SetActive(true);

                tmp_player.users.last_login = System.DateTime.Now.ToString();

                GameRootMaster.player = tmp_player;
                GameRootMaster.GSFU.SaveNewPlayer();


            }
            else //wait timer for database to respond to search for name first.last
            {
                attemptNum++;

                //Since we know the connection is good from VerifyDatabaseConnection()
                //no database response HERE means that the user entered "first.last" 
                //doesn't exist in the database hence the DriveResponse did not trigger.
                //Meaning, this new account is the first of its name
                if (attemptNum > connectionAttmpTimeout)
                {
                    GameRootMaster.GSFU.retrieved = false;
                    creatingAcct = false;
                    attemptNum = 0;
                    loginWindow.na_login.enabled = true;


                    tmp_player.users.first_last = na_first.text.ToLower() + "." + na_last.text.ToLower();


                    tmp_player.users.ID_num = 0;
                    tmp_player.SetUserName(tmp_player.users.first_last);

                    tmp_player.users.education = na_education.captionText.text;
                    tmp_player.users.tot_time = 0.0f;

                    string salt = GenerateSalt();
                    string hashedpass = ComputeHash(Encoding.UTF8.GetBytes(na_pass1.text), Encoding.UTF8.GetBytes(salt));
                    tmp_player.users.hash = "\'" + hashedpass;
                    tmp_player.users.salt = "\'" + salt;

                    userNameText.text = "<b>Your Username is:</b>\n" +
                                        "<size=28>" + tmp_player.users.username + "</size>";

                    if (na_otherSel)
                        tmp_player.users.school = na_otherSchool.text;
                    else
                        tmp_player.users.school = na_schools.captionText.text;

                    panelNewUserName.gameObject.SetActive(true);

                    tmp_player.users.last_login = System.DateTime.Now.ToString();

                    GameRootMaster.player = tmp_player;
                    GameRootMaster.GSFU.SaveNewPlayer();
                }
            }
        }

    }


    //------------------------------------
    // CREATE ACCOUNT Functions
    //------------------------------------
    #region Create Account
    public void NA_ClickLoginBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        if (na_first.text.Length == 0 && na_last.text.Length == 0)
        {
            //loginSystem.login.errorText.text = "You must enter in both a First and Last name.";
            //loginSystem.login.panelError.gameObject.SetActive(true);
        }
        else
        {
            if (na_pass1.text.CompareTo(na_pass2.text) != 0)
            {
                //loginSystem.login.errorText.text = "The passwords do not match";
                //loginSystem.login.panelError.gameObject.SetActive(true);
            }
            else
            {
                if (na_pass1.text.Length < 8 || na_pass2.text.Length < 8)
                {
                    //loginSystem.login.errorText.text = "Password must be at least 8 characters long";
                    //loginSystem.login.panelError.gameObject.SetActive(true);
                }
                else
                {
                    //if (na_schools.value == 0)
                    //{
                    //    errorText.text = "You must select a school";
                    //    SetActive(panelError, true);
                    //}
                    if (na_otherSel && na_otherSchool.text.CompareTo("") == 0)
                    {
                        //loginSystem.login.errorText.text = "You must enter in your other school's name";
                        //loginSystem.login.panelError.gameObject.SetActive(true);
                    }
                    else
                    {
 //                       loginSystem.loginWindow.panel_awaitServer.gameObject.SetActive(true);
 //                       loginSystem.loginWindow.awaitServePop.SetStatus("Checking Connection");

                        loginWindow.na_login.enabled = false; //disable button so player can't spam click login and overload database with login requests
                        query = 2;
                        checkingConnection = true;
                        GameRootMaster.GSFU.VerifyDatabaseConnection();
                    }
                }
            }
        }
    }

    public void NA_ClickBackBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        

        NA_ClearNewAcctFields();

        //newAcctCheck = true;
        newAccountWindow.gameObject.SetActive(false);
        loginWindow.gameObject.SetActive(true);
    }

    public void NA_ClickUserNameOK()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        panelNewUserName.gameObject.SetActive(false);
        NA_ClearNewAcctFields();

        panelNewAcct.gameObject.SetActive(false);

        loginWindow.PlayGame();
    }

    private void NA_ClearNewAcctFields()
    {
        na_first.text = "";
        na_last.text = "";
        na_pass1.text = "";
        na_pass2.text = "";
        na_education.value = 0;
    }

    public void NA_SchoolChanged()
    {
        //Option selected is "Other"
        if (na_schools.captionText.text.CompareTo("Other") == 0)
            na_otherSel = true;
        else
            na_otherSel = false;

        na_otherSchool.gameObject.SetActive(na_otherSel);
    }

    public void NA_setSchools()
    {

    }
    #endregion
}
