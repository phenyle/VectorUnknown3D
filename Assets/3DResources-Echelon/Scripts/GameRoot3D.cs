using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using GoogleSheetsForUnity;

public class GameRoot3D : GameRootMaster
{
    public GameObject scenePlayer;
    public GameObject sceneCameraMain;

    public static GameRoot3D instance = null;
    public bool isPause { get; set; }
    public bool isPuzzleLock { get; set; }
    public bool isTutortial { get; set; }
    public bool showCursor { get; set; }
    
    [Header("Common API")]
 //   public LoadingWindow loadingWindow;
    public DynamicWindow dynamicWindow;
    public PauseWindow pauseWindow;
    public GameControllerRoot GCR;

    //Services
   // [HideInInspector]
   // public ResourceService resourceService { get; private set; }
    [HideInInspector]
    public AudioService audioService { get; private set; }


    //Systems
    [HideInInspector]
    public MenuSystem menuSystem;
    [HideInInspector]
    public PuzzleSystem puzzleSystem;
    [HideInInspector]
    public static UnityEvent camEvents;
    public static UnityEvent moveEvents;

    [HideInInspector]
    public int score = 0;
    //[HideInInspector]
    public bool firstCompletion = false;
    public int prevStage;
    public int prevLevel;
    public bool[][] puzzlesDone;
    public bool[] puzzleCompleted = { false, false, false, false };
    public int exitPuzzle = 0;
    private bool rootInitialized = false;

    //puzzle 2 
    private GameControllerPuzzle02 GCP02_01;
    private GameControllerPuzzle02 GCP02_02;

    public void GameMasterInit()
    {

        //First Array is Number of Stages
        //Second Array is the Levels of that Stage
        //Array is made for easier readability
        //exp: puzzlesDone[3][2] is Stage3, Level 2
        //yea that's right, we starting Arrays at 1 over here
        puzzlesDone = new bool[Constants.numOfStages + 1][];
        puzzlesDone[0] = new bool[0]; //empty
        puzzlesDone[1] = new bool[Constants.numLvlsStage1 + 1];
        puzzlesDone[2] = new bool[Constants.numLvlsStage2 + 1];
        puzzlesDone[3] = new bool[Constants.numLvlsStage3 + 1];
        puzzlesDone[4] = new bool[Constants.numLvlsStage4 + 1];

        //init all puzzleDones as assumed false
        //these will be reflipped after the player loads with their game data
        //upon entering the mainScene hubworld
        for (int i = 0; i < puzzlesDone.Length; i++)
            for (int j = 0; j < puzzlesDone[i].Length; j++)
                puzzlesDone[i][j] = false;

        camEvents = new UnityEvent();
        moveEvents = new UnityEvent();
        instance.isPause = false;
        instance.isPuzzleLock = false;
        instance.isTutortial = false;
        instance.showCursor = true;


        if (SceneManager.GetActiveScene().name == Constants.gameRootSceneName)
        {
            InitUI();
            loadingWindow.AltInit();
        }

        //3.Enter Menu Scene
        if (SceneManager.GetActiveScene().name == Constants.gameRootSceneName)
        {
            menuSystem.EnterMenu();
        }
    }

    public void Awake()
    {
    }

    public void Start()
    {
        if (!rootInitialized)
        {
            Debug.LogWarning("Game Start...");
            rootInitialized = true;
            instance = this;
            DontDestroyOnLoad(this);
            InitGameRoot();
            GameMasterInit();
            if (GameRootMaster.isSession)
                JoinEchelonLevel();
        }
        else
            Destroy(this);
    }

    //On game shut down, save player info
    public void OnDisable()
    {
        player.users.last_login = DateTime.Now.ToString();
        GSFU.UpdatePlayerTable(false, PlayerData.sheets.users, player.users.username);
    }

    //On game shut down, save player info
    public void OnApplicationQuit()
    {
        player.users.last_login = DateTime.Now.ToString();
        GSFU.UpdatePlayerTable(false, PlayerData.sheets.users, player.users.username);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!instance.isPause 
                && SceneManager.GetActiveScene().name != Constants.gameRootSceneName
                && SceneManager.GetActiveScene().name != Constants.menuSceneName)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
            ToggleCursor();                

        if (Input.GetKeyDown(KeyCode.R))
        {

        }

        if(!instance.isPause)
        {
            player.users.tot_time += Time.deltaTime;
        }

    }

    public void InitUI()
    {
        Transform canvas = transform.Find("Canvas");
        Transform uiWindows = canvas.Find("UIWindows");

        for (int i = 0; i < uiWindows.childCount; i++)
        {
            uiWindows.GetChild(i).gameObject.SetActive(false);
        }

        dynamicWindow.SetWindowState(true);
    }

    private void InitGameRoot()
    {
        //1. Init all services
        //a. Resource service
 //       resourceService = GetComponent<ResourceService>();
  //      resourceService.InitService();

        //b. Audio service
        audioService = GetComponent<AudioService>();
        audioService.InitService();

        //2. Init all systems
        //a. Menu System
        menuSystem = GetComponent<MenuSystem>();
        menuSystem.InitSystem();

        //b. Puzzle System
        puzzleSystem = GetComponent<PuzzleSystem>();
        puzzleSystem.InitSystem();

        GCR = GetComponent<GameControllerRoot>();
    }

    public static void AddTips(string tips)
    {
        instance.dynamicWindow.AddTips(tips);
    }

    public static void ShowTips(string tips, bool state, bool isReactive)
    {
        instance.dynamicWindow.ShowTips(tips, state, isReactive);
    }

    public static void ShowTips(string tips, Color color, bool state, bool isReactive)
    {
        instance.dynamicWindow.ShowTips(tips, color, state, isReactive);
    }

    public void Pause()
    {
        Debug.Log("Pause");

        pauseWindow.btn_Hub.SetActive(!isTutortial);

        ShowCursor(true);

        LockPlayerAndCam(true);
        Time.timeScale = 0;
        pauseWindow.SetWindowState(true);

        //Remove Hub Button if in ForcedStageMode
        if (player.users.forceStage)
            pauseWindow.ToggleHubButton(false);
        else
            pauseWindow.ToggleHubButton(true);

        //Show username in top corner of pause menu (if it's loaded)
        try
        {
            pauseWindow.setLoginInfo(player.users.username);
        }
        catch
        {
            pauseWindow.setLoginInfo("error");
        }
        instance.isPause = true;
        audioService.PauseAllAudios();
        Debug.Log("Lock");
    }

    public void Resume()
    {
        Resume(true);
    }

    // lockCursor: false when leaving gameplay (e.g. quitting to the main menu). On WebGL a cursor lock
    // requested outside a user input event is deferred to the next click, so locking here would
    // grab the pointer on the menu's first click even after the menu shows the cursor again.
    public void Resume(bool lockCursor)
    {
        //Debug.Log(DialogueManager.isInDialogue);
        //Debug.Log(isPuzzleLock);

        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;

        if (!DialogueManager.isInDialogue && !instance.isPuzzleLock)
        {
            LockPlayerAndCam(false);
            ShowCursor(!lockCursor);
            Debug.Log("Unlock");
        }
        else
            ShowCursor(true);

        if(!instance.isPuzzleLock)
            LockPlayerAndCam(false);

        Time.timeScale = 1;
        pauseWindow.panelOption.gameObject.SetActive(false);
        pauseWindow.panelPause.gameObject.SetActive(true);
        pauseWindow.SetWindowState(false);
        instance.isPause = false;
        pauseWindow.ResetPauseMenu();
        audioService.ResumeAllAudios();

    }

    public void LockPlayer(bool state)
    {
        try
        {
            scenePlayer.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().isLock = state;
        }
        catch { }
    }

    public void LockCamera(bool state)
    {
        try
        {
            sceneCameraMain.GetComponent<CameraController>().isLock = state;
        }
        catch
        {
            try
            {
                sceneCameraMain.GetComponent<Camera2DFollowMod>().isLock = state;
            }
            catch { }
        }
    }

    public void LockPlayerAndCam(bool state)
    {

        try
        {
            sceneCameraMain.GetComponent<CameraController>().isLock = state;
        }
        catch 
        {
            try
            {
                sceneCameraMain.GetComponent<Camera2DFollowMod>().isLock = state;
            }
            catch { }
        }
        try
        {
            scenePlayer.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().isLock = state;
        }
        catch { }


        //try
        //{
        //    Camera.main.GetComponent<CameraController>().isLock = state;
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().isLock = state;
        //}
        //catch { }
    }

    public void SetScenePlayer(GameObject playerObj)
    {
        scenePlayer = playerObj;
    }

    public void SetSceneCamera(GameObject cameraObj)
    {
        sceneCameraMain = cameraObj;
    }

    public void FindPlayerAndCamera()
    {
        instance.SetScenePlayer(GameObject.FindGameObjectWithTag("Player"));
        instance.SetSceneCamera(GameObject.FindGameObjectWithTag("MainCamera"));
    }

    public void ToggleCursor()
    {
        if (instance.isPuzzleLock || DialogueManager.isInDialogue)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            instance.showCursor = !instance.showCursor;

            if(instance.showCursor)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

    }

    public void ShowCursor(bool state)
    {
        instance.showCursor = state;

        if (instance.showCursor)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public static void AddScore(int score)
    {
        instance.score += score;
    }

    public void JoinEchelonLevel()
    {
        switch (GameRootMaster.player.users.stageNum)
        {
            case 0:
                GameRootMaster.resourceService.AsynLoadScene(Constants.mainSceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                break;
            case 1:

                break;
            case 2:
                switch (GameRootMaster.player.users.difficultyNum)
                {
                    case 1:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle02_1SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    case 2:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle02_2SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    case 3:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle02_3SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    default:
                        break;
                }

                break;
            case 3:
                switch (GameRootMaster.player.users.difficultyNum)
                {
                    case 1:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle03_1SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    case 2:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle03_2SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    case 3:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle03_3SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    default:
                        break;
                }

                break;
            case 4:
                switch (GameRootMaster.player.users.difficultyNum)
                {
                    case 1:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle04_1SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    case 2:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle04_2SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    case 3:
                        GameRootMaster.resourceService.AsynLoadScene(Constants.puzzle04_3SceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                        break;
                    default:
                        break;
                }

                break;
            default:
                GameRootMaster.resourceService.AsynLoadScene(Constants.mainSceneName, () => GameObject.Find("GameRoot").GetComponent<GameRoot3D>().Start());
                break;

        }
    }
}
