using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class may have gotten away from me.
/// All of these panels are related to loging into the game from the
/// TitleMenu (except for options)
/// However, it would probably serve better to break each of these panels
/// into their own classes and functions for clarity.
/// Functionally it works as is, but it may benefit for readability and
/// from a module standpoint to break it up later
/// put it to the TODO
/// </summary>

public class TitleWindow : WindowRoot
{
    //MAIN SCREEN
    [Header ("Main Screen")]
    public Transform panelMain;

    //OPTIONS
    [Header ("Options")]
    public Transform panelOptions;
    public Slider sliderVolume;
    public Slider sliderSoundFX;
    public Dropdown ddResolution;

    //LOGIN
    [Header ("Login")]
    public Transform panelLogin;
    public InputField lg_username;
    public InputField lg_pass;
    private bool passHide = true;

    //NEW ACCOUNT
    [Header ("New Account")]
    public Transform panelNewAcct;
    public InputField na_first;
    public InputField na_last;
    public InputField na_pass1;
    public InputField na_pass2;
    public Dropdown na_education;
    public Dropdown na_schools;
    public InputField na_otherSchool;
    private bool na_otherSel = false;
    public Transform panelNewUserName;
    public Text userNameText;

    //JOIN SESSION
    [Header("Join Session")]
    public Transform panelJoin;
    public InputField js_Name;

    //ADMIN
    [Header ("Admin")]
    public Transform panelAdmin;
    public Toggle admin_toggleClassroom;
    public Toggle admin_toggleExpo;
    public Toggle admin_toggleNone;
    public InputField admin_sessionName;
    public Toggle admin_toggleExpire;
    public Dropdown admin_dropMonth;
    public Dropdown admin_dropDay;
    public Dropdown admin_dropYear;

    [Header("Other")]
    public Transform panelError;
    public Text errorText;

    public Transform panel_awaitServer;
    public AwaitingServerPopup awaitServePop;
    

    [Header("Buttons")]
    public Button lg_login;
    public Button na_login;
    public Button js_login;

    private PlayerData tmp_player;

    private bool checkingConnection = false;
    private int query; //0: error, 1:loadingPlayer, 2:creatingAcct, 3:joiningSession
    private bool loadingPlayer = false;
    private bool newAcctCheck = false;
    private bool creatingAcct = false;
    private bool joiningSession = false;
    private List<string> schools;
    private int attemptNum = 0;
    private int connectionAttmpTimeout = 600;


    //private Resolution[] resolutions;

    protected override void InitWindow()
    {
        base.InitWindow();

        // SetBgVolume();
        sliderVolume.value = audioService.bgVolume;
        audioService.PlayBgMusic(Constants.audioBgMenu, true);

        // SetSoundFXVolume();
        sliderSoundFX.value = audioService.UIFXVolume;

        // show mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        

        SetActive(panel_awaitServer, false);

        //doubling down on my initial values
        loadingPlayer = false;
        creatingAcct = false;
        joiningSession = false;
        attemptNum = 0;
        SetActive(na_otherSchool.gameObject, false);
        schools = new List<string>();
        tmp_player = new PlayerData();

        //admin stuff
        admin_dropMonth.value = DateTime.Now.Month - 1;
        admin_dropDay.value = DateTime.Now.Day - 1;
        ExpireYears();
        admin_dropYear.value = 0;

        //Initialize Resolution Dropdown
        /*
        resolutions = Screen.resolutions;
        ddResolution.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            if((double)resolutions[i].width/resolutions[i].height == 16.0/9.0)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
        }

        ddResolution.AddOptions(options);
        ddResolution.value = currentResolutionIndex;
        ddResolution.RefreshShownValue();*/
        //*************************************
    }

    //Title Screen Late Updates is only for awaiting response from
    //database when logging in, joining sessions or creating accounts
    public void LateUpdate()
    {
        #region Checking connection to database
        //Checking for database/internet connection
        if (checkingConnection)
        {
            //GSFU successfully respond, proceed with login actions
            if(GameRoot.GSFU.retrieved)
            {
                GameRoot.GSFU.retrieved = false; //reset retrieved flag
                checkingConnection = false;
                attemptNum = 0;

                switch (query)
                {
                    case 1: //query type: Logging into Account
                        //call to database for "username" to find login name
                        panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
                        GameRoot.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text, false);
                        loadingPlayer = true;
                        break;

                    case 2: //query type: Creating a new Account
                            //call to database for "first.last" to check for duplicates
                        panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Creating Account");
                        GameRoot.GSFU.RetrievePlayerData(PlayerData.sheets.users, "first_last", na_first.text + "." + na_last.text, false);
                        creatingAcct = true;
                        break;

                    case 3: //query type: Joining Session
                        panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Session");

                        GameRoot.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", js_Name.text);
                        joiningSession = true;
                        break;
                    case 4:
                        SetActive(panel_awaitServer, false);

                        NA_setSchools();
                        LG_ClearLoginFields();

                        SetActive(panelLogin, false);
                        SetActive(panelNewAcct, true);
                        break;
                }
            }
            else //awaiting GSFU response
            {
                attemptNum++;

                //timeout
                if (attemptNum > connectionAttmpTimeout)
                {
                    //login timeout
                    SetActive(panel_awaitServer, false);
                    checkingConnection = false;
                    Debug.Log("LOGIN ERROR: timeout");
                    attemptNum = 0;
                    errorText.text = "Error connecting to database.\nPlease try again later.";
                    SetActive(panelError, true);
                }
            }
        }
        #endregion

        #region Logging into personal player account
        //Login Check awaiting response from database
        if (loadingPlayer)
        {
            //database responded
            //stop waiting for database and check credentials
            if (GameRoot.GSFU.retrieved)
            {

                GameRoot.GSFU.retrieved = false; //reset retrieved flag
                loadingPlayer = false;
                attemptNum = 0;
                lg_login.enabled = true; //reenable the login button, in case login fails


                string salt = GameRoot.player.users.salt;
                string passattempt = ComputeHash(Encoding.UTF8.GetBytes(lg_pass.text), Encoding.UTF8.GetBytes(salt));

                //Check for user/pass match
                if(passattempt.CompareTo(GameRoot.player.users.hash) == 0)
                {
                    Debug.Log("LOGIN SUCCESFUL");

                    //load in other player settings

                    GameRoot.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text, false);
    
                    //for (int i = 1; i < GameRoot.player.sheetNames.Length; i++)
                    //{
                    //    GameRoot.GSFU.RetrievePlayerData(GameRoot.player.sheetNames[i], "username", lg_username.text);
                    //}

                    tmp_player.users.last_login = System.DateTime.Now.ToString();
                    GameRoot.GSFU.UpdatePlayerTable(false, PlayerData.sheets.users, GameRoot.player.users.username);
                    LG_ClearLoginFields();

                    awaitServePop.SetStatus("");
                    SetActive(panel_awaitServer, false);

                    //admin logins
                    if(GameRoot.player.users.admin)
                    {
                        SetActive(panelAdmin, true);
                        SetActive(panelLogin, false);
                    }
                    else
                    {
                        SetActive(panelLogin, false);
                        SetActive(panelMain, true);

                        PlayGame();
                    }
                }
                //player password wrong
                else
                {
                    Debug.Log("LOGIN ERROR: wrong user/pass");
                    LG_ClearLoginFields();
                    errorText.text = "Username or password Incorrect or Not Found";
                    SetActive(panelError, true);
                }

            }
            else  //awaiting database response
            {
                attemptNum++;

                //username not found
                //connection to database already confirmed, if drive has
                //no response at this stage, it is because that name is not
                //in the database
                if(attemptNum > connectionAttmpTimeout)
                {
                    lg_login.enabled = true; //reenable login button
                    loadingPlayer = false;
                    Debug.Log("LOGIN ERROR: wrong user/pass");
                    LG_ClearLoginFields();
                    attemptNum = 0;
                    errorText.text = "Username or password Incorrect or Not Found";
                    SetActive(panelError, true);
                }
            }
        }
        #endregion

        #region Creating a new player account
        //CREATING NEW PLAYER
        //create a player in the database using the entered username/password
        //if a first.last already exists, create a username with an incriment
        if (creatingAcct)
        {
            //database responded
            //stop waiting for database and create new player
            //if the database responded, we're almost guaranteed to have a
            //duplicate user with first.last
            if(GameRoot.GSFU.retrieved)
            {
                GameRoot.GSFU.retrieved = false;  
                creatingAcct = false;
                attemptNum = 0;
                na_login.enabled = true;

                tmp_player.users.first_last = na_first.text.ToLower() + "." + na_last.text.ToLower();

                tmp_player.users.ID_num = GameRoot.player.users.ID_num;
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
                
                SetActive(panelNewUserName, true);

                tmp_player.users.last_login = System.DateTime.Now.ToString();

                GameRoot.player = tmp_player;
                GameRoot.GSFU.SaveNewPlayer();


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
                    GameRoot.GSFU.retrieved = false;
                    creatingAcct = false;
                    attemptNum = 0;
                    na_login.enabled = true;


                    tmp_player.users.first_last = na_first.text + "." + na_last.text;


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

                    SetActive(panelNewUserName, true);

                    tmp_player.users.last_login = System.DateTime.Now.ToString();

                    GameRoot.player = tmp_player;
                    GameRoot.GSFU.SaveNewPlayer();
                }
            }
        }
        #endregion

        #region Joining an Admin created game session
        //JOINING SESSION
        //used for both isClass and isExpo sessions
        if (joiningSession)
        {
            //database responded and found session name
            //stop waiting for database create new session player
            if (GameRoot.GSFU.retrieved)
            {
                GameRoot.GSFU.retrieved = false;
                joiningSession = false;
                attemptNum = 0;
                Debug.Log("Session Found");

                //Check that the session being joined has not expired
                if(DateTime.Compare(DateTime.Now, DateTime.Parse(GameRoot.player.users.expires)) < 0)
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
                    if (GameRoot.player.users.isExpo)
                    {
                        GameRoot.isExpo = true;
                        GameRoot.expoName = js_Name.text;
                    }

                    if (GameRoot.isExpo)
                    {
                        tmp_player.SetUserName(GameRoot.expoName + hashID);
                        tmp_player.users.isExpo = true;

                    }
                    else
                        tmp_player.SetUserName(js_Name.text + hashID);

                    tmp_player.users.last_login = System.DateTime.Now.ToString();
                    GameRoot.player = tmp_player;
                    GameRoot.GSFU.SaveNewPlayer();

                    SetActive(panelJoin, false);
                    SetActive(panelMain, true);

                    PlayGame();
                }
                else
                {
                    joiningSession = false;
                    Debug.Log("LOGIN ERROR:\nSession has expired");
                    attemptNum = 0;
                    errorText.text = "Sorry, that session has expired.";
                    SetActive(panelError, true);
                }

               
            }
            else //awaiting database response
            {
                attemptNum++;

                //no session with that name
                if (attemptNum > connectionAttmpTimeout)
                {
                    //login timeout
                    joiningSession = false;
                    Debug.Log("LOGIN ERROR: wrong Session ID");
                    attemptNum = 0;
                    errorText.text = "No session with that name/ID found\nPlease verify the Session name/ID and try again.";
                    SetActive(panelError, true);
                }
            }

        }
        #endregion
    }

    public void PlayGame()
    {
        //Reset puzzle complete status
        for (int i = 0; i < GameRoot.instance.puzzleCompleted.Length; i++)
        {
            GameRoot.instance.puzzleCompleted[i] = false;
        }

        DialogueManager.instance.ResetAll();

        audioService.PlayUIAudio(Constants.audioUIStartBtn);
        //GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle01SceneName);// original 

        GameRoot.instance.exitPuzzle = 0;
        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.mainSceneName);//added by LaQuez Brown 1-26-21

        // hides mouse when loading game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ClickPlayBtn()
    {
        //When the game is in the "Expo" state, clicking play automatically
        //enters the player in and just creates a new database entry according
        //to the expo name.
        GameRoot.isTutortial = false;

        if (GameRoot.isExpo)
        {
            GameRoot.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", GameRoot.expoName);
            joiningSession = true;
        }
        else
        {
            SetActive(panelMain, false);
            SetActive(panelLogin, true);
        }
    }

    public void ClickTutorialBtn()
    {
        GameRoot.isTutortial = true;
        audioService.PlayUIAudio(Constants.audioUIClickBtn);
        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.tutorialSceneName);
    }

    public void ClickOptionsBtn()
    {
        SetActive(panelMain, false);
        SetActive(panelOptions, true);
        audioService.PlayUIAudio(Constants.audioUIClickBtn);
    }

    public void ClickQuitBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);
        Application.Quit();
    }

    //------------------------------------
    // OPTIONS Functions
    //------------------------------------
    #region Options
    public void SetBgVolume()
    {
        audioService.SetBgVolume(sliderVolume.value);
    }

    public void SetSoundFXVolume()
    {
        audioService.SetSoundFXVolume(sliderSoundFX.value);
    }

    public void ClickBackBtn()
    {
        SetActive(panelOptions, false);
        SetActive(panelMain, true);
        audioService.PlayUIAudio(Constants.audioUIClickBtn);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex + 1);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

    }

    public void SetResolution(int index)
    {
        //Resolution resolution = resolutions[index];
        //Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        //Screen.SetResolution(1920, 1080, false);
    }
    #endregion


    //------------------------------------
    // LOGIN Functions
    //------------------------------------
    #region Login
    public void LG_ClickBackBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);
        LG_ClearLoginFields();

        SetActive(panelLogin, false);
        SetActive(panelMain, true);
    }

    //TODO================
    public void LG_ClickSessionBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        LG_ClearLoginFields();
        SetActive(panelLogin, false);
        SetActive(panelJoin, true);
    }

    public void LG_ClickNewAccount()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

    //    NA_setSchools();
        LG_ClearLoginFields();

        SetActive(panelLogin, false);
        SetActive(panelNewAcct, true);

        //SetActive(panel_awaitServer, true);
        //awaitServePop.SetStatus("Checking Connection");

        //query = 4;
        //checkingConnection = true;
        //GameRoot.GSFU.VerifyDatabaseConnection();  
    }

    public void LG_ClickLoginBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        SetActive(panel_awaitServer, true);
        panel_awaitServer.GetComponent<AwaitingServerPopup>().enabled = true;
        awaitServePop.SetStatus("Checking Connection");

        lg_login.enabled = false; //disable button so player can't spam click login and overload database with login requests
        query = 1;
        checkingConnection = true;
        GameRoot.GSFU.VerifyDatabaseConnection();
    }

    public void LG_ClickPassPeekBtn()
    {
        passHide = !passHide;

        if(passHide)
            lg_pass.contentType = InputField.ContentType.Password;
        else
            lg_pass.contentType = InputField.ContentType.Standard;

        lg_pass.ForceLabelUpdate();

    }

    private void LG_ClearLoginFields()
    {
        lg_username.text = "";
        lg_pass.text = "";
    }
    #endregion


    //------------------------------------
    // CREATE ACCOUNT Functions
    //------------------------------------
    #region Create Account
    public void NA_ClickLoginBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        if (na_first.text.Length == 0 && na_last.text.Length == 0)
        {
            errorText.text = "You must enter in both a First and Last name.";
            SetActive(panelError, true);
        }
        else
        {
            if(na_pass1.text.CompareTo(na_pass2.text) != 0)
            {
                errorText.text = "The passwords do not match";
                SetActive(panelError, true);
            }
            else
            {
                if (na_pass1.text.Length < 8 || na_pass2.text.Length < 8)
                {
                    errorText.text = "Password must be at least 8 characters long";
                    SetActive(panelError, true);
                }
                else
                {
                    if (na_schools.value == 0)
                    {
                        errorText.text = "You must select a school";
                        SetActive(panelError, true);
                    }
                    else if (na_otherSel && na_otherSchool.text.CompareTo("") == 0)
                    {
                        errorText.text = "You must enter in your other school's name";
                        SetActive(panelError, true);
                    }
                    else {
                        SetActive(panel_awaitServer, true);
                        awaitServePop.SetStatus("Checking Connection");

                        na_login.enabled = false; //disable button so player can't spam click login and overload database with login requests
                        query = 2;
                        checkingConnection = true;
                        GameRoot.GSFU.VerifyDatabaseConnection();
                    }
                }
            }
        }
    }

    public void NA_ClickBackBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        NA_ClearNewAcctFields();

        newAcctCheck = true;

        SetActive(panelNewAcct, false);
        SetActive(panelLogin, true);
    }

    public void NA_ClickUserNameOK()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        SetActive(panelNewUserName, false);
        NA_ClearNewAcctFields();

        SetActive(panelNewAcct, false);
        SetActive(panelMain, true);

        PlayGame();
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
        if(na_schools.captionText.text.CompareTo("Other") == 0)
            na_otherSel = true;
        else
            na_otherSel = false;

        SetActive(na_otherSchool.gameObject, na_otherSel);
    }

    private void NA_setSchools()
    {

    }

    private string ComputeHash(byte[] bytesToHash, byte[] salt)
    {
        var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
        return Convert.ToBase64String(byteResult.GetBytes(24));
    }

    private string GenerateSalt()
    {
        var bytes = new byte[128 / 8];
        var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
    #endregion


    //--------------------------------
    // JOIN SESSION Functions
    //--------------------------------
    #region Join Session
    public void JS_ClickLoginBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        SetActive(panel_awaitServer, true);
        awaitServePop.SetStatus("Checking Connection");

        js_login.enabled = false; //disable button so player can't spam click login and overload database with login requests
        query = 3;
        checkingConnection = true;
        GameRoot.GSFU.VerifyDatabaseConnection();  
    }

    public void JS_ClickBackBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        js_Name.text = "";
        SetActive(panelLogin, true);
        SetActive(panelJoin, false);
    }
    #endregion

    
    //--------------------------------
    // ADMIN Functions
    //--------------------------------
    #region Admin

    ///Admin accounts can ONLY be created by modifing the database (googlesheets)
    ///By default all users accounts created in game are automatically set to NOT
    ///admin
    ///To make a user account an admin, you must goto the database(googlesheets)
    ///under the person's username (NOT first_last) and scroll to the admin column
    ///change the value to "TRUE" for admin accounts


    /// <summary>
    /// Admin Play button does 3 things:
    /// --Classroom option. They will be required to enter a session ID.
    ///   After this, anyone from any system can remote into the game
    ///   using the "Join Session" button at login
    /// --Expo/Demo option.  They will be required to enter a session ID.
    ///   After this, ONLY this system will login using that session ID name.
    ///   No username/password required by others playing.  Exiting the game
    ///   returns the player to the menu screen.  Clicking play again after
    ///   the Admin has selected "Expo" will NOT take the player to the login
    ///   screen.  Instead they will be automatically placed in game and an
    ///   incrimented username will be entered into the database
    /// --None Option. The admin logs into their own personal game.
    /// </summary>
    public void Admin_ClickPlay()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        //Classroom creates a session name so that any additional players
        //can join on any machine with that session name instead of creating 
        //an account or logging in with a user/pass
        if (admin_toggleClassroom.isOn)
        {
            //Error: No session name entered
            if(admin_sessionName.text.Length <= 0)
            {
                errorText.text = "No session name entered";
                SetActive(panelError, true);
            }
            else
            {
                PlayerData session = new PlayerData();
                session.SetUserName(admin_sessionName.text);
                session.users.isClass = true;

                if (admin_toggleExpire.isOn)
                {
                    DateTime expireDay = new DateTime(DateTime.Now.Year + admin_dropYear.value, admin_dropMonth.value + 1, admin_dropDay.value + 1, 23, 59, 59);

                    session.users.expires = expireDay.ToString();
                }
                else
                    session.users.expires = new DateTime(DateTime.Now.Year + 100, 1, 1).ToString();

                GameRoot.player = session;
                GameRoot.isExpo = false;
                GameRoot.isClass = true;

 //               DateTime.Compare(DateTime.Now, DateTime.Parse(GameRoot.player.users.last_login));

                //TODO set Admin Specific play sessions rules
                //----
                //----

                GameRoot.GSFU.SaveNewPlayer();

                SetActive(panelAdmin, false);
                SetActive(panelMain, true);
                PlayGame();
            }
        }

        //Expo is similar to Classroom except that turning on Expo
        //alters this local running version of the game to create a new
        //database player everytime the "Play" button is pressed, no
        //login info needed.
        if(admin_toggleExpo.isOn)
        {
            //Error: No session name entered
            if (admin_sessionName.text.Length <= 0)
            {
                errorText.text = "No session name entered";
                SetActive(panelError, true);
            }
            else
            {
                GameRoot.expoName = admin_sessionName.text;
                GameRoot.isExpo = true;

                PlayerData session = new PlayerData();
                session.SetUserName(admin_sessionName.text);
                session.users.isExpo = true;

                if (admin_toggleExpire.isOn)
                {
                    DateTime expireDay = new DateTime(DateTime.Now.Year + admin_dropYear.value, admin_dropMonth.value + 1, admin_dropDay.value + 1, 23, 59, 59);

                    session.users.expires = expireDay.ToString();
                }
                else
                    session.users.expires = new DateTime(DateTime.Now.Year + 100, 1, 1).ToString();

                GameRoot.player = session;


                //TODO set Admin Specific play sessions rules
                //----
                //----

                GameRoot.GSFU.SaveNewPlayer();

                SetActive(panelAdmin, false);
                SetActive(panelMain, true);
                PlayGame();
            }

        }

        //Play the game on the Admin's Account, save data
        //to Admin name
        if (admin_toggleNone.isOn)
        {
            SetActive(panelAdmin, false);
            SetActive(panelMain, true);
            PlayGame();
        }
    }

    public void SessionNoneToggle()
    {
        if (admin_toggleNone.isOn)
        {
            admin_sessionName.text = "";
            admin_sessionName.interactable = false;
            admin_toggleExpire.isOn = false;
            admin_toggleExpire.interactable = false;
        }
        else
        {
            admin_sessionName.interactable = true;
            admin_toggleExpire.interactable = true;
        }
    }

    public void SessionExpireToggle()
    {
        if(admin_toggleExpire.isOn)
        {
            admin_dropMonth.interactable = true;
            admin_dropDay.interactable = true;
            admin_dropYear.interactable = true;
        }
        else
        {
            admin_dropMonth.interactable = false;
            admin_dropDay.interactable = false;
            admin_dropYear.interactable = false;
        }

    }    

    public void ExpireMonthChange()
    {
        admin_dropDay.options.Clear();

        List<Dropdown.OptionData> daysList = new List<Dropdown.OptionData>();
        for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, admin_dropMonth.value + 1); i++)
        {
            daysList.Add(new Dropdown.OptionData() { text = i.ToString() });
        }

        admin_dropDay.options = daysList;
    }

    private void ExpireYears()
    {
        admin_dropYear.options.Clear();

        List<Dropdown.OptionData> yearsList = new List<Dropdown.OptionData>();
        for (int i = 0; i <= 5; i++)
        {
            yearsList.Add(new Dropdown.OptionData() { text = (DateTime.Now.Year + i).ToString() });
        }

        admin_dropYear.options = yearsList;

    }
    #endregion


    //--------------------------------
    // MISC
    //--------------------------------
    #region Misc
    public void ErrorOKButton()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);

        lg_login.enabled = true;
        na_login.enabled = true;
        js_login.enabled = true;

        SetActive(panel_awaitServer, false);

        SetActive(panelError, false);
    }
    #endregion


}
