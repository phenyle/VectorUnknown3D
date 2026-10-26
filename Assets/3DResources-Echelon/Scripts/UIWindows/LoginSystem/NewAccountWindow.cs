using UnityEngine;
using UnityEngine.UI;

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
    public Button backButton;

    private bool na_otherSel = false;
    private string newUsername = "";

    // Start is called before the first frame update
    void Start()
    {
        newAccountWindow = this;
        na_otherSchool.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    /// <summary>
    /// The server builds the player ID from the first 3 letters of the first and last names (plus a
    /// number if that ID is taken), hashes the password and stores neither name. The ID is shown to
    /// the player, who logs in with it.
    /// </summary>
    private void CreateAccount()
    {
        string school = na_otherSel ? na_otherSchool.text : na_schools.captionText.text;

        ToggleInteractable(false);
        ShowWaiting("Creating Account", false);
        GameRootMaster.GSFU.CreateAccount(na_first.text, na_last.text, na_pass1.text, school, na_education.captionText.text,
            AccountCreated, error => ShowMessage(error, ClickOkayGeneral));
    }

    private void AccountCreated(string username)
    {
        newUsername = username;
        ShowMessage("<b>Your Player ID is:</b>\n<size=28>" + username + "</size>\nWrite it down: you need it to log in.", ClickUserNameOkay);
    }


 //------------------------------------
 // CREATE ACCOUNT Functions
 //------------------------------------
        #region Create Account
    public void NA_ClickLoginBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        if (na_first.text.Trim().Length == 0 || na_last.text.Trim().Length == 0)
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
                        CreateAccount();
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
        backButton.interactable = val;

    }

    public void ClickOkayGeneral()
    {
        messageEvents.RemoveAllListeners();
        ToggleInteractable(true);
        popUpsWindow.generalPopPanel.SetActive(false);
    }

    // Back to the login panel with the new ID and password filled in
    public void ClickUserNameOkay()
    {
        messageEvents.RemoveAllListeners();
        ToggleInteractable(true);
        newAccountWindow.panel.SetActive(false);
        dualLoginWindow.panel.SetActive(true);
        dualLoginWindow.username.text = newUsername;
        dualLoginWindow.password.text = na_pass1.text;
        ClearNewAcctFields();
    }

    public void TimedOut()
    {
        messageEvents.RemoveAllListeners();
        popUpsWindow.generalPopPanel.SetActive(false);
        popUpsWindow.awaitServerPop.panel.SetActive(false);
        ToggleInteractable(true);
    }

}
