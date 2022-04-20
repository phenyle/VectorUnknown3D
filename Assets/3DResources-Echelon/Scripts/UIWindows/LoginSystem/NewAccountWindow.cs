using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsForUnity;

public class NewAccountWindow : LoginSystem
{
    //NEW ACCOUNT
    [Header("New Account")]
    public GameObject panel;
    public InputField na_first;
    public InputField na_last;
    public InputField na_pass1;
    public InputField na_pass2;
    public Dropdown na_education;
    public Dropdown na_schools;
    public InputField na_otherSchool;

    private bool na_otherSel = false;

    // Start is called before the first frame update
    void Start()
    {
        newAccountWindow = this;
        na_otherSchool.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    public void LookForUsername()
    {

    }

    /// <summary>
    /// This is the function called by the LoginSystem when the GFSU doesn't return and
    /// essentially times out.  LoginEvents calls this if the system times out, but the
    /// query was set for creating a new account
    /// </summary>
    public void CreatingNewAccount()
    {
        GameRootMaster.GSFU.retrieved = false;
        checkingGFSU = false;
        connectionTime = 0;
        loginEvents.RemoveAllListeners();
        SendToGoogle.callback.RemoveAllListeners();

        tmp_player.users.first_last = na_first.text.ToLower() + "." + na_last.text.ToLower();

        tmp_player.users.ID_num = 0;
        tmp_player.SetUserName(tmp_player.users.first_last);

        tmp_player.users.education = na_education.captionText.text;
        tmp_player.users.tot_time = 0.0f;

        string salt = GenerateSalt();
        string hashedpass = ComputeHash(Encoding.UTF8.GetBytes(na_pass1.text), Encoding.UTF8.GetBytes(salt));
        tmp_player.users.hash = "\'" + hashedpass;
        tmp_player.users.salt = "\'" + salt;

        if (na_otherSel)
            tmp_player.users.school = na_otherSchool.text;
        else
            tmp_player.users.school = na_schools.captionText.text;

        messageEvents.AddListener(ClickUserNameOkay);

        popUpsWindow.GeneralPopUp("<b>Your Username is:</b>\n" +
                            "<size=28>" + tmp_player.users.username + "</size>");

        tmp_player.users.last_login = System.DateTime.Now.ToString();

        GameRootMaster.player = tmp_player;
        GameRootMaster.GSFU.SaveNewPlayer();

    }

    /// <summary>
    /// This is the function called by the GFSU when it finds and returns a name already
    /// in the database.  This account creation creates an incremented account ID number
    /// </summary>
    public void CreatingIncrimentedAccount()
    {
        GameRootMaster.GSFU.retrieved = false;
        creatingAcct = false;
        checkingGFSU = false;
        connectionTime = 0;
        loginEvents.RemoveAllListeners();
        SendToGoogle.callback.RemoveAllListeners();

        tmp_player.users.first_last = na_first.text.ToLower() + "." + na_last.text.ToLower();

        //Take the last duplicate name ID num
        tmp_player.users.ID_num = GameRootMaster.player.users.ID_num;
        //Increase this new users ID by one
        tmp_player.users.ID_num++;
        tmp_player.SetUserName(tmp_player.users.first_last + tmp_player.users.ID_num.ToString());

        tmp_player.users.education = na_education.captionText.text;
        tmp_player.users.tot_time = 0.0f;

        string salt = GenerateSalt();
        string hashedpass = ComputeHash(Encoding.UTF8.GetBytes(na_pass1.text), Encoding.UTF8.GetBytes(salt));
        tmp_player.users.hash = "\'" + hashedpass;
        tmp_player.users.salt = "\'" + salt;

        if (na_otherSel)
            tmp_player.users.school = na_otherSchool.text;
        else
            tmp_player.users.school = na_schools.captionText.text;

        messageEvents.AddListener(ClickUserNameOkay);

        popUpsWindow.GeneralPopUp("<b>Your Username is:</b>\n" +
                            "<size=28>" + tmp_player.users.username + "</size>");


        tmp_player.users.last_login = System.DateTime.Now.ToString();

        GameRootMaster.player = tmp_player;
        GameRootMaster.GSFU.SaveNewPlayer();        
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
            popUpsWindow.GeneralPopUp("You must enter in both a First and Last name.");
            ToggleInteractable(false);
            messageEvents.AddListener(ClickOkayGeneral);
        }
        else
        {
            if (na_pass1.text.CompareTo(na_pass2.text) != 0)
            {
                popUpsWindow.GeneralPopUp("The passwords do not match");
                ToggleInteractable(false);
                messageEvents.AddListener(ClickOkayGeneral);

            }
            else
            {
                if (na_pass1.text.Length < 8 || na_pass2.text.Length < 8)
                {
                    popUpsWindow.GeneralPopUp("Password must be at least 8 characters long");
                    ToggleInteractable(false);
                    messageEvents.AddListener(ClickOkayGeneral);
                }
                else
                {
                    if (na_schools.value == 0)
                    {
                        popUpsWindow.GeneralPopUp("You must select a school");
                        ToggleInteractable(false);
                        messageEvents.AddListener(ClickOkayGeneral);
                    }
                    else if (na_otherSel && na_otherSchool.text.CompareTo("") == 0)
                    {
                        popUpsWindow.GeneralPopUp("You must enter in your other school's name");
                        ToggleInteractable(false);
                        messageEvents.AddListener(ClickOkayGeneral);
                    }
                    else
                    {
                        messageEvents.AddListener(TimedOut);
                        popUpsWindow.AwaitPopUp(false);
                        popUpsWindow.awaitServerPop.SetStatus("Checking Connection");

                        checkingGFSU = true;
                        connectionTime = 0.0f;
                        ToggleInteractable(false);

                        query = queryType.creating;
                        SendToGoogle.callback.AddListener(ConnectionsCheck);
                        GameRootMaster.GSFU.VerifyDatabaseConnection();
                    }
                }
            }
        }
    }

    public void NA_ClickBackBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);       

        ClearNewAcctFields();

        //newAcctCheck = true;
        newAccountWindow.panel.SetActive(false);
        dualLoginWindow.panel.SetActive(true);
    }

    public void NA_ClickUserNameOK()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        ClearNewAcctFields();

        panel.SetActive(false);

        newAccountWindow.gameObject.SetActive(false);
    }

    private void ClearNewAcctFields()
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

    public void ToggleInteractable(bool val)
    {
        na_first.interactable = val;
        na_last.interactable = val;
        na_pass1.interactable = val;
        na_pass2.interactable = val;
        na_education.interactable = val;
        na_schools.interactable = val;
        na_otherSchool.interactable = val;
    }

    public void ClickOkayGeneral()
    {
        messageEvents.RemoveAllListeners();
        ToggleInteractable(true);
        popUpsWindow.generalPopPanel.SetActive(false);
    }

    public void ClickUserNameOkay()
    {
        messageEvents.RemoveAllListeners();
        ToggleInteractable(true);
        newAccountWindow.panel.SetActive(false);
        dualLoginWindow.panel.SetActive(true);
        dualLoginWindow.username.text = tmp_player.users.username;
        dualLoginWindow.password.text = na_pass1.text;
        ClearNewAcctFields();
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

}
