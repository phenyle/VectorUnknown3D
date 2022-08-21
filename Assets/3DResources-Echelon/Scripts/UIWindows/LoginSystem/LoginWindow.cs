using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsForUnity;


public class LoginWindow : LoginSystem
{
    private bool DEBUG = false;

    //LOGIN
    [Header("Login")]
    [SerializeField] private Transform panelLogin;
    [SerializeField] private InputField lg_username;
    [SerializeField] private InputField lg_pass;
    [SerializeField] private bool passHide = true;

    ////NEW ACCOUNT
    //[Header("New Account")]
    //public Transform panelNewAcct;
    //public InputField na_first;
    //public InputField na_last;
    //public InputField na_pass1;
    //public InputField na_pass2;
    //public Dropdown na_education;
    //public Dropdown na_schools;
    //public InputField na_otherSchool;
    //private bool na_otherSel = false;
    //public Transform panelNewUserName;
    //public Text userNameText;

    ////JOIN SESSION
    //[Header("Join Session")]
    //public Transform panelJoin;
    //public InputField js_Name;

    ////ADMIN
    //[Header("Admin")]
    //public Transform panelAdmin;
    //public Toggle admin_toggleClassroom;
    //public Toggle admin_toggleExpo;
    //public Toggle admin_toggleNone;
    //public InputField admin_sessionName;
    //public Toggle admin_toggleExpire;
    //public Dropdown admin_dropMonth;
    //public Dropdown admin_dropDay;
    //public Dropdown admin_dropYear;

    //public Dropdown admin_StageSelect;
    //public Dropdown admin_DifficultySelect;
    //public Toggle admin_forceStage;

    //[Header("Other")]
    //public Transform panelError;
    //public Text errorText;

    //   public Transform panel_awaitServer;
    //   public AwaitingServerPopup awaitServePop;


    [Header("Buttons")]
    public Button lg_Echelonlogin;
    public Button lg_Bunnylogin;
    public Button na_login;


    private List<string> schools;


    //private Resolution[] resolutions;

    public void Start()
    {
        // show mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        //       panel_awaitServer.gameObject.SetActive(false);

        //doubling down on my initial values
        //na_otherSchool.gameObject.SetActive(false);
        schools = new List<string>();

    }

    //Title Screen Late Updates is only for awaiting response from
    //database when logging in, joining sessions or creating accounts
    public void LateUpdate()
    {
        if (DEBUG)
        {
            GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", "asdf.qwer", false);
            GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.records, "username", "asdf.qwer", false);

            PlayGame();
        }

        #region Checking connection to database
        //Checking for database/internet connection
        //if (checkingConnection)
        //{
        //    //GSFU successfully respond, proceed with login actions
        //    if (GameRootMaster.GSFU.retrieved)
        //    {
        //        GameRootMaster.GSFU.retrieved = false; //reset retrieved flag
        //        checkingConnection = false;
        //        attemptNum = 0;

        //        switch (query)
        //        {
        //            case 1: //query type: Logging into Echelon Game
        //                    //call to database for "username" to find login name
        //                    //                      panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
        //                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text, false);
        //                loadingPlayer = true;
        //                break;

        //            case 2: //query type: Creating a new Account
        //                    //call to database for "first.last" to check for duplicates
        //                    //                     panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Creating Account");
        //                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "first_last", newAccountWindow.na_first.text.ToLower() + "." + newAccountWindow.na_last.text.ToLower(), false);
        //                creatingAcct = true;
        //                break;

        //            case 3: //query type: Joining Session
        //                    //                    panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Session");

        //                //GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", js_Name.text);
        //                joiningSession = true;
        //                break;
        //            case 4:
        //                //                    panel_awaitServer.gameObject.SetActive(false);

        //                newAccountWindow.NA_setSchools();
        //                LG_ClearLoginFields();

        //                panelLogin.gameObject.SetActive(false);
        //                newAccountWindow.panelNewAcct.gameObject.SetActive(true);
        //                break;
        //            case 5: //query type: Logging into Bunny Game
        //                    //call to database for "username" to find login name
        //                    //                    panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
        //                GameRoot3D.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text, false);
        //                loadingPlayer = true;
        //                break;

        //        }
        //    }
        //    else //awaiting GSFU response
        //    {
        //        attemptNum++;

        //        //timeout
        //        if (attemptNum > connectionAttmpTimeout)
        //        {
        //            //login timeout
        //            //                  panel_awaitServer.gameObject.SetActive(false);
        //            checkingConnection = false;
        //            Debug.Log("LOGIN ERROR: timeout");
        //            attemptNum = 0;
        //            //errorText.text = "Error connecting to database.\nPlease try again later.";
        //            //panelError.gameObject.SetActive(true);
        //        }
        //    }
        //}
        #endregion

        #region Logging into personal player account
        //Login Check awaiting response from database
        if (loadingPlayer)
        {
            ////database responded
            ////stop waiting for database and check credentials
            //if (GameRoot3D.GSFU.retrieved)
            //{

            //    GameRoot3D.GSFU.retrieved = false; //reset retrieved flag
            //    loadingPlayer = false;
            //    attemptNum = 0;
            //    lg_Echelonlogin.enabled = true; //reenable the login button, in case login fails
            //    lg_Bunnylogin.enabled = true; //reenable the login button, in case login fails


            //    string salt = GameRoot3D.player.users.salt;
            //    string passattempt = ComputeHash(Encoding.UTF8.GetBytes(lg_pass.text), Encoding.UTF8.GetBytes(salt));

            //    //Check for user/pass match
            //    if (passattempt.CompareTo(GameRoot3D.player.users.hash) == 0)
            //    {
            //        Debug.Log("LOGIN SUCCESFUL");

            //        //load in other player settings

            //        GameRoot3D.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text.ToLower(), false);
            //        GameRoot3D.GSFU.RetrievePlayerData(PlayerData.sheets.records, "username", lg_username.text.ToLower(), false);

            //        //for (int i = 1; i < GameRoot.player.sheetNames.Length; i++)
            //        //{
            //        //    GameRoot.GSFU.RetrievePlayerData(GameRoot.player.sheetNames[i], "username", lg_username.text);
            //        //}

            //        tmp_player.users.last_login = System.DateTime.Now.ToString();
            //        GameRoot3D.GSFU.UpdatePlayerTable(false, PlayerData.sheets.users, GameRoot3D.player.users.username);
            //        LG_ClearLoginFields();

            //        //                 awaitServePop.SetStatus("");
            //        //                 panel_awaitServer.gameObject.SetActive(false);


            //        panelLogin.gameObject.SetActive(false);

            //        PlayGame();

            //    }
            //    //player password wrong
            //    else
            //    {
            //        Debug.Log("LOGIN ERROR: wrong user/pass");
            //        LG_ClearLoginFields();
            //        //errorText.text = "Username or password Incorrect or Not Found";
            //        //panelError.gameObject.SetActive(true);
            //    }

            //}
            //else  //awaiting database response
            //{
            //    attemptNum++;

            //    //username not found
            //    //connection to database already confirmed, if drive has
            //    //no response at this stage, it is because that name is not
            //    //in the database
            //    if (attemptNum > connectionAttmpTimeout)
            //    {
            //        lg_Echelonlogin.enabled = true; //reenable login button
            //        loadingPlayer = false;
            //        Debug.Log("LOGIN ERROR: wrong user/pass");
            //        LG_ClearLoginFields();
            //        attemptNum = 0;
            //        //errorText.text = "Username or password Incorrect or Not Found";
            //        //panelError.gameObject.SetActive(true);
            //    }
            //}
        }
        #endregion

        #region Joining an Admin created game session
        //JOINING SESSION
        ////used for both isClass and isExpo sessions
        //if (joiningSession)
        //{
        //    //database responded and found session name
        //    //stop waiting for database create new session player
        //    if (GameRoot3D.GSFU.retrieved)
        //    {
        //        GameRoot3D.GSFU.retrieved = false;
        //        joiningSession = false;
        //        attemptNum = 0;
        //        Debug.Log("Session Found");

        //        //Check that the session being joined has not expired
        //        if (DateTime.Compare(DateTime.Now, DateTime.Parse(GameRoot3D.player.users.expires)) < 0)
        //        {
        //            Debug.Log("Join Session SUCCESFUL");

        //            //Since there is a delay in database response, it's possible
        //            //for multiple people to "join session" at the same time and 
        //            //the database tell all of them they're the first to join and give
        //            //them all the same incrimented username.
        //            //This will cause problems later when that username is referenced
        //            //to save the session data.
        //            //To combat this and ensure that only ONE username matches the
        //            //sessionID, we'll reuse our salt and hash functions.  But since there
        //            //is no password for joining a session, instead we'll hash the current
        //            //DateTime and concatenate the resulting string onto the joining name.
        //            //This will ensure every joining player has a unique session name no 
        //            //matter how many people join at the same time.
        //            string salt = GenerateSalt();
        //            string hashID = ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), Encoding.UTF8.GetBytes(salt));

        //            tmp_player = new PlayerData();

        //            //if the loaded sessions is from an Expo, trigger this machine to
        //            //run in the Expo state using the session name
        //            if (GameRoot3D.player.users.isExpo)
        //            {
        //                GameRoot3D.isExpo = true;
        //                GameRoot3D.expoName = js_Name.text;

        //            }
        //            if (GameRoot3D.player.users.isClass)
        //            {
        //                GameRoot3D.isClass = true;
        //                GameRoot3D.expoName = js_Name.text;
        //                tmp_player.users.isClass = true;

        //            }

        //            tmp_player.users.stageNum = GameRoot3D.player.users.stageNum;
        //            tmp_player.users.difficultyNum = GameRoot3D.player.users.difficultyNum;
        //            tmp_player.users.forceStage = GameRoot3D.player.users.forceStage;

        //            if (GameRoot3D.isExpo)
        //            {
        //                tmp_player.SetUserName(GameRoot3D.expoName + hashID);
        //                tmp_player.users.isExpo = true;

        //            }
        //            else
        //                tmp_player.SetUserName(js_Name.text + hashID);

        //            tmp_player.users.last_login = System.DateTime.Now.ToString();
        //            tmp_player.users.sessionID = js_Name.text;
        //            GameRoot3D.player = tmp_player;
        //            GameRoot3D.GSFU.SaveNewPlayer();

        //            panelJoin.gameObject.SetActive(false);
        //            panelMain.gameObject.SetActive(true);

        //            PlayGame();
        //        }
        //        else
        //        {
        //            joiningSession = false;
        //            Debug.Log("LOGIN ERROR:\nSession has expired");
        //            attemptNum = 0;
        //            errorText.text = "Sorry, that session has expired.";
        //            panelError.gameObject.SetActive(true);
        //        }


        //    }
        //    else //awaiting database response
        //    {
        //        attemptNum++;

        //        //no session with that name
        //        if (attemptNum > connectionAttmpTimeout)
        //        {
        //            //login timeout
        //            joiningSession = false;
        //            Debug.Log("LOGIN ERROR: wrong Session ID");
        //            attemptNum = 0;
        //            errorText.text = "No session with that name/ID found\nPlease verify the Session name/ID and try again.";
        //            panelError.gameObject.SetActive(true);
        //        }
        //    }

        //}
        #endregion
    }

    public void PlayGame()
    {
        //Reset puzzle complete status
        for (int i = 0; i < GameRoot3D.instance.puzzleCompleted.Length; i++)
        {
            GameRoot3D.instance.puzzleCompleted[i] = false;
        }

        DialogueManager.instance.ResetAll();

        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIStartBtn);
        //GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle01SceneName);// original 

        if (GameRootMaster.player.users.isClass || GameRootMaster.player.users.isExpo)
        {
            switch (GameRootMaster.player.users.stageNum)
            {
                case 0: //no level restrictions, join at main hub
                    GameRoot3D.instance.exitPuzzle = 0;
                    GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.mainSceneName);
                    break;
                case 1: //stage 1
                    GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle01SceneName);
                    break;

                case 2: //stage 2
                    switch (GameRootMaster.player.users.difficultyNum)
                    {
                        case 1:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle02_1SceneName);
                            break;
                        case 2:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle02_2SceneName);
                            break;
                        case 3:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle02_3SceneName);
                            break;
                        default:
                            break;
                    }
                    break;

                case 3: //stage 3
                    switch (GameRootMaster.player.users.difficultyNum)
                    {
                        case 1:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle03_1SceneName);
                            break;
                        case 2:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle03_2SceneName);
                            break;
                        case 3:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle03_3SceneName);
                            break;
                        default:
                            break;
                    }
                    break;

                case 4: //stage 4
                    switch (GameRootMaster.player.users.difficultyNum)
                    {
                        case 1:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle04_1SceneName);
                            break;
                        case 2:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle04_2SceneName);
                            break;
                        case 3:
                            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.puzzle04_3SceneName);
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    GameRoot3D.instance.exitPuzzle = 0;
                    GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.mainSceneName);
                    break;
            }

        }
        else
        {
            GameRoot3D.instance.exitPuzzle = 0;
            GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.mainSceneName);//added by LaQuez Brown 1-26-21
        }
        

        // hides mouse when loading game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ClickPlayBtn()
    {
        //When the game is in the "Expo" state, clicking play automatically
        //enters the player in and just creates a new database entry according
        //to the expo name.
        GameRoot3D.instance.isTutortial = false;

        if (GameRoot3D.isExpo)
        {
            GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", GameRoot3D.expoName);
            joiningSession = true;
        }
        else
        {
            panelLogin.gameObject.SetActive(true);
        }
    }


    //------------------------------------
    // LOGIN Functions
    //------------------------------------
    #region Login
    public void LG_ClickSessionBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        LG_ClearLoginFields();
        joinWindow.gameObject.SetActive(true);
    }

    public void LG_ClickNewAccount()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        //    NA_setSchools();
        LG_ClearLoginFields();

        newAccountWindow.gameObject.SetActive(true);


        //events.AddListener()

        //SetActive(panel_awaitServer, true);
        //awaitServePop.SetStatus("Checking Connection");

        //query = 4;
        //checkingConnection = true;
        //GameRoot.GSFU.VerifyDatabaseConnection();  
    }

    public void LG_ClickEchelonBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        //      panel_awaitServer.gameObject.SetActive(true);
        //       panel_awaitServer.GetComponent<AwaitingServerPopup>().enabled = true;
        //      awaitServePop.SetStatus("Checking Connection");

        lg_Echelonlogin.enabled = false; //disable button so player can't spam click login and overload database with login requests
        lg_Bunnylogin.enabled = false; //disable button so player can't spam click login and overload database with login requests
        //query = 1;
        //checkingConnection = true;
        bunnyGame = false;
        SendToGoogle.callback.AddListener(ConnectionCheck);
        GameRootMaster.GSFU.VerifyDatabaseConnection();
    }
    public void LG_ClickBunnyBtn()
    {
        //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        //      panel_awaitServer.gameObject.SetActive(true);
        //      panel_awaitServer.GetComponent<AwaitingServerPopup>().enabled = true;
        //      awaitServePop.SetStatus("Checking Connection");

        lg_Echelonlogin.enabled = false; //disable button so player can't spam click login and overload database with login requests
        lg_Bunnylogin.enabled = false; //disable button so player can't spam click login and overload database with login requests
        //query = 4;
        //checkingConnection = true;
        bunnyGame = true;
        GameRootMaster.GSFU.VerifyDatabaseConnection();
    }

    public void LG_ClickPassPeekBtn()
    {
        passHide = !passHide;

        if (passHide)
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
    //public void Admin_ClickPlay()
    //{
    //    //GameRootMaster.audioService.PlayUIAudio(Constants.audioUIClickBtn);

    //    //Classroom creates a session name so that any additional players
    //    //can join on any machine with that session name instead of creating 
    //    //an account or logging in with a user/pass
    //    if (admin_toggleClassroom.isOn)
    //    {
    //        //Error: No session name entered
    //        if (admin_sessionName.text.Length <= 0)
    //        {
    //            errorText.text = "No session name entered";
    //            panelError.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            PlayerData session = new PlayerData();
    //            session.SetUserName(admin_sessionName.text);
    //            session.users.isClass = true;

    //            if (admin_toggleExpire.isOn)
    //            {
    //                DateTime expireDay = new DateTime(DateTime.Now.Year + admin_dropYear.value, admin_dropMonth.value + 1, admin_dropDay.value + 1, 23, 59, 59);

    //                session.users.expires = expireDay.ToString();
    //            }
    //            else
    //                session.users.expires = new DateTime(DateTime.Now.Year + 100, 1, 1).ToString();

    //            session.users.stageNum = admin_StageSelect.value;
    //            session.users.difficultyNum = admin_DifficultySelect.value + 1;
    //            session.users.forceStage = admin_forceStage.isOn;

    //            GameRoot3D.player = session;
    //            GameRoot3D.isExpo = false;
    //            GameRoot3D.isClass = true;


    //            //               DateTime.Compare(DateTime.Now, DateTime.Parse(GameRoot.player.users.last_login));

    //            //TODO set Admin Specific play sessions rules
    //            //----
    //            //----

    //            GameRoot3D.GSFU.SaveNewPlayer();

    //            panelAdmin.gameObject.SetActive(false);
    //            panelMain.gameObject.SetActive(true);
    //            PlayGame();
    //        }
    //    }

    //    //Expo is similar to Classroom except that turning on Expo
    //    //alters this local running version of the game to create a new
    //    //database player everytime the "Play" button is pressed, no
    //    //login info needed.
    //    if (admin_toggleExpo.isOn)
    //    {
    //        //Error: No session name entered
    //        if (admin_sessionName.text.Length <= 0)
    //        {
    //            errorText.text = "No session name entered";
    //            panelError.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            GameRoot3D.expoName = admin_sessionName.text;
    //            GameRoot3D.isExpo = true;

    //            PlayerData session = new PlayerData();
    //            session.SetUserName(admin_sessionName.text);
    //            session.users.isExpo = true;

    //            if (admin_toggleExpire.isOn)
    //            {
    //                DateTime expireDay = new DateTime(DateTime.Now.Year + admin_dropYear.value, admin_dropMonth.value + 1, admin_dropDay.value + 1, 23, 59, 59);

    //                session.users.expires = expireDay.ToString();
    //            }
    //            else
    //                session.users.expires = new DateTime(DateTime.Now.Year + 100, 1, 1).ToString();

    //            session.users.stageNum = admin_StageSelect.value;
    //            session.users.difficultyNum = admin_DifficultySelect.value + 1;
    //            session.users.forceStage = admin_forceStage.isOn;

    //            GameRootMaster.player = session;


    //            //TODO set Admin Specific play sessions rules
    //            //----
    //            //----

    //            GameRootMaster.GSFU.SaveNewPlayer();

    //            panelAdmin.gameObject.SetActive(false);
    //            panelMain.gameObject.SetActive(true);
    //            PlayGame();
    //        }

    //    }

    //    //Play the game on the Admin's Account, save data
    //    //to Admin name
    //    if (admin_toggleNone.isOn)
    //    {
    //        panelAdmin.gameObject.SetActive(false);
    //        panelMain.gameObject.SetActive(true);
    //        PlayGame();
    //    }
    //}

    //public void SessionNoneToggle()
    //{
    //    if (admin_toggleNone.isOn)
    //    {
    //        admin_sessionName.text = "";
    //        admin_sessionName.interactable = false;
    //        admin_toggleExpire.isOn = false;
    //        admin_toggleExpire.interactable = false;
    //        admin_StageSelect.interactable = false;
    //        admin_DifficultySelect.interactable = false;
    //        admin_forceStage.interactable = false;
    //        admin_StageSelect.value = 0;
    //        admin_DifficultySelect.value = 0;
    //        admin_forceStage.isOn = false;
    //    }
    //    else
    //    {
    //        admin_sessionName.interactable = true;
    //        admin_toggleExpire.interactable = true;
    //        admin_StageSelect.interactable = true;
    //        StageSelectChange();
    //    }
    //}

    //public void SessionExpireToggle()
    //{
    //    if (admin_toggleExpire.isOn)
    //    {
    //        admin_dropMonth.interactable = true;
    //        admin_dropDay.interactable = true;
    //        admin_dropYear.interactable = true;
    //    }
    //    else
    //    {
    //        admin_dropMonth.interactable = false;
    //        admin_dropDay.interactable = false;
    //        admin_dropYear.interactable = false;
    //    }

    //}

    //public void ExpireMonthChange()
    //{
    //    admin_dropDay.options.Clear();

    //    List<Dropdown.OptionData> daysList = new List<Dropdown.OptionData>();
    //    for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, admin_dropMonth.value + 1); i++)
    //    {
    //        daysList.Add(new Dropdown.OptionData() { text = i.ToString() });
    //    }

    //    admin_dropDay.options = daysList;
    //}

    //private void ExpireYears()
    //{
    //    admin_dropYear.options.Clear();

    //    List<Dropdown.OptionData> yearsList = new List<Dropdown.OptionData>();
    //    for (int i = 0; i <= 5; i++)
    //    {
    //        yearsList.Add(new Dropdown.OptionData() { text = (DateTime.Now.Year + i).ToString() });
    //    }

    //    admin_dropYear.options = yearsList;

    //}

    //public void StageSelectChange()
    //{
    //    if (admin_StageSelect.value == 0)
    //    {
    //        admin_DifficultySelect.interactable = false;
    //        admin_forceStage.isOn = false;
    //        admin_forceStage.interactable = false;
    //    }
    //    else if (admin_StageSelect.value == 1)
    //    {
    //        admin_DifficultySelect.interactable = true;
    //        admin_forceStage.isOn = false;
    //        admin_forceStage.interactable = true;

    //        admin_DifficultySelect.ClearOptions();
    //        List<string> options = new List<string>();
    //        options.Add("Level 1");
    //        admin_DifficultySelect.AddOptions(options);
    //    }
    //    else
    //    {
    //        admin_DifficultySelect.interactable = true;
    //        admin_forceStage.interactable = true;

    //        admin_DifficultySelect.ClearOptions();
    //        List<string> options = new List<string>();
    //        options.Add("Level 1");
    //        options.Add("Level 2");
    //        options.Add("Level 3");
    //        admin_DifficultySelect.AddOptions(options);
    //    }
    //}
    #endregion


    //--------------------------------
    // Connection Actions
    //--------------------------------
    private void ConnectionCheck()
    {
        GameRootMaster.GSFU.retrieved = false; //reset retrieved flag
        //checkingConnection = false;
        //attemptNum = 0;
        SendToGoogle.callback.RemoveAllListeners();
        int temp = 1;
        switch (temp)
        {
            case 1: //query type: Logging into Echelon Game
                    //call to database for "username" to find login name
                    //                      panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
                SendToGoogle.callback.AddListener(EchelonLogin);
                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text, false);
                loadingPlayer = true;
                break;

            case 2: //query type: Creating a new Account
                    //call to database for "first.last" to check for duplicates
                    //                     panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Creating Account");
                //SendToGoogle.callback.AddListener(CreateAccount);

                GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "first_last", newAccountWindow.na_first.text.ToLower() + "." + newAccountWindow.na_last.text.ToLower(), false);
                creatingAcct = true;
                break;

            case 3: //query type: Joining Session
                    //                    panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Session");

                //GameRootMaster.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", js_Name.text);
                joiningSession = true;
                break;
            case 4:
                //                    panel_awaitServer.gameObject.SetActive(false);

                newAccountWindow.NA_setSchools();
                LG_ClearLoginFields();

                panelLogin.gameObject.SetActive(false);
                break;
            case 5: //query type: Logging into Bunny Game
                    //call to database for "username" to find login name
                    //                    panel_awaitServer.gameObject.GetComponent<AwaitingServerPopup>().SetStatus("Finding Player");
                GameRoot3D.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", lg_username.text, false);
                loadingPlayer = true;
                break;


        }
    }

    private void EchelonLogin()
    {

    }
}
