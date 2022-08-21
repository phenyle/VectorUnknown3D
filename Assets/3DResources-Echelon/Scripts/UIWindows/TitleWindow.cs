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
    private bool DEBUG = false;

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

    public Dropdown admin_StageSelect;
    public Dropdown admin_DifficultySelect;
    public Toggle admin_forceStage;

    [Header("Other")]
    public Transform panelError;
    public Text errorText;

    public Transform panel_awaitServer;
    public AwaitingServerPopup awaitServePop;
    

    [Header("Buttons")]
    public Button lg_Echelonlogin;
    public Button lg_Bunnylogin;
    public Button na_login;
    public Button js_login;


    protected override void InitWindow()
    {
        base.InitWindow();

        // SetBgVolume();
        sliderVolume.value = audioService.bgVolume;
        audioService.PlayBgMusic(0, true);

        // SetSoundFXVolume();
        sliderSoundFX.value = audioService.UIFXVolume;

        // show mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        

        SetActive(panel_awaitServer, false);

        //doubling down on my initial values

        //admin stuff
        admin_dropMonth.value = DateTime.Now.Month - 1;
        admin_dropDay.value = DateTime.Now.Day - 1;
        ExpireYears();
        admin_dropYear.value = 0;


    }



    public void PlayGame()
    {
        //Reset puzzle complete status
        for (int i = 0; i < GameRoot3D.instance.puzzleCompleted.Length; i++)
        {
            GameRoot3D.instance.puzzleCompleted[i] = false;
        }

        DialogueManager.instance.ResetAll();

        audioService.PlayUIAudio(Constants.audioUIStartBtn);
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
            GameRoot3D.GSFU.RetrievePlayerData(PlayerData.sheets.users, "username", GameRoot3D.expoName);
        }
        else
        {
            SetActive(panelMain, false);
            SetActive(panelLogin, true);
        }
    }

    public void ClickTutorialBtn()
    {
        GameRoot3D.instance.isTutortial = true;
        audioService.PlayUIAudio(Constants.audioUIClickBtn);
        GameRoot3D.instance.puzzleSystem.EnterPuzzle(Constants.tutorialSceneName);
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
        GameRootMaster.resourceService.AsynLoadScene(Constants.mainLoginScreen, () => Destroy(GameRoot3D.instance.gameObject));
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

                session.users.stageNum = admin_StageSelect.value;
                session.users.difficultyNum = admin_DifficultySelect.value + 1;
                session.users.forceStage = admin_forceStage.isOn;

                GameRoot3D.player = session;
                GameRoot3D.isExpo = false;
                GameRoot3D.isClass = true;


 //               DateTime.Compare(DateTime.Now, DateTime.Parse(GameRoot.player.users.last_login));

                //TODO set Admin Specific play sessions rules
                //----
                //----

                GameRoot3D.GSFU.SaveNewPlayer();

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
                GameRoot3D.expoName = admin_sessionName.text;
                GameRoot3D.isExpo = true;

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

                session.users.stageNum = admin_StageSelect.value;
                session.users.difficultyNum = admin_DifficultySelect.value + 1;
                session.users.forceStage = admin_forceStage.isOn;

                GameRoot3D.player = session;


                //TODO set Admin Specific play sessions rules
                //----
                //----

                GameRoot3D.GSFU.SaveNewPlayer();

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
            admin_StageSelect.interactable = false;
            admin_DifficultySelect.interactable = false;
            admin_forceStage.interactable = false;
            admin_StageSelect.value = 0;
            admin_DifficultySelect.value = 0;
            admin_forceStage.isOn = false;
        }
        else
        {
            admin_sessionName.interactable = true;
            admin_toggleExpire.interactable = true;
            admin_StageSelect.interactable = true;
            StageSelectChange();
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

    public void StageSelectChange()
    {
        if(admin_StageSelect.value == 0)
        {
            admin_DifficultySelect.interactable = false;
            admin_forceStage.isOn = false;
            admin_forceStage.interactable = false;
        }
        else if(admin_StageSelect.value == 1)
        {
            admin_DifficultySelect.interactable = true;
            admin_forceStage.isOn = false;
            admin_forceStage.interactable = true;

            admin_DifficultySelect.ClearOptions();
            List<string> options = new List<string>();
            options.Add("Level 1");
            admin_DifficultySelect.AddOptions(options);
        }
        else
        {
            admin_DifficultySelect.interactable = true;
            admin_forceStage.interactable = true;

            admin_DifficultySelect.ClearOptions();
            List<string> options = new List<string>();
            options.Add("Level 1");
            options.Add("Level 2");
            options.Add("Level 3");
            admin_DifficultySelect.AddOptions(options);
        }
    }
    #endregion




}
