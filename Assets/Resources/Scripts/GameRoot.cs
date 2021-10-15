using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using GoogleSheetsForUnity;

public class GameRoot : MonoBehaviour
{

    public static PlayerData player;
    public static SendToGoogle GSFU;
    public static bool isExpo = false;
    public static string expoName;
    public static bool isClass = false;

    public static GameRoot instance = null;
    public static bool isPause = false;
    public static bool isPuzzleLock = false;
    public static bool isTutortial = false;
    
    [Header("Common API")]
    public LoadingWindow loadingWindow;
    public DynamicWindow dynamicWindow;
    public PauseWindow pauseWindow;

    //Services
    [HideInInspector]
    public ResourceService resourceService;
    [HideInInspector]
    public AudioService audioService;

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

    //puzzle 2 
    private GameControllerPuzzle02 GCP02_01;
    private GameControllerPuzzle02 GCP02_02;


    private void Awake()
    {
        DontDestroyOnLoad(this);

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

        instance = this;
        GSFU = this.GetComponent<SendToGoogle>();
        player = this.GetComponent<PlayerData>();
        camEvents = new UnityEvent();
        moveEvents = new UnityEvent();

        DontDestroyOnLoad(GSFU);
        DontDestroyOnLoad(player);

        if (SceneManager.GetActiveScene().name == Constants.gameRootSceneName)
        {
            InitUI();
        }

        InitGameRoot();
    }

    private void Start()
    {
        Debug.Log("Game Start...");
        //InitUI();
        //InitGameRoot();

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
            if(!isPause 
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

        if (Input.GetKeyDown(KeyCode.R))
        {

        }

        if(!isPause)
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
        resourceService = GetComponent<ResourceService>();
        resourceService.InitService();

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

        //3.Enter Menu Scene
        if (SceneManager.GetActiveScene().name == Constants.gameRootSceneName)
        {
            menuSystem.EnterMenu();
        }
    }

    public static void AddTips(string tips)
    {
        instance.dynamicWindow.AddTips(tips);
    }

    public static void ShowTips(string tips, bool state, bool isReactive)
    {
        instance.dynamicWindow.ShowTips(tips, state, isReactive);
    }

    public void Pause()
    {
        Debug.Log("Pause");

        pauseWindow.btn_Hub.SetActive(!isTutortial);

        // check to change cameras in puzzle 2
        if(SceneManager.GetActiveScene().name == Constants.puzzle02_1SceneName)
        {
            Debug.Log("Puzzle 2 Level 1");
            GCP02_01 = GameObject.Find("GameController_Puzzle02_01").GetComponent<GameControllerPuzzle02>();
            if(GCP02_01.cameraFollow)
            {
                Debug.Log("Puzzle 2 Level 1 : camera follow");
                GCP02_01.MainCamera.SetActive(true);
                GCP02_01.CannonCamera.SetActive(false);
            }
            else
                Debug.Log("Puzzle 2 Level 1: no camera follow");
        }
        if(SceneManager.GetActiveScene().name == Constants.puzzle02_2SceneName)
        {
            Debug.Log("Puzzle 2 Level 2");
            GCP02_02 = GameObject.Find("GameController_Puzzle02_02").GetComponent<GameControllerPuzzle02>();
            if(GCP02_02.cameraFollow)
            {
                GCP02_02.CannonCamera.SetActive(false);
                GCP02_02.MainCamera.SetActive(true);
            }
        }
        IsLock(true);
        Time.timeScale = 0;
        pauseWindow.SetWindowState(true);
        try
        {
            pauseWindow.setLoginInfo(player.users.username);
        }
        catch
        {
            pauseWindow.setLoginInfo("error");
        }
        isPause = true;
        audioService.PauseAllAudios();
        Debug.Log("Lock");
    }

    public void Resume()
    {
        //Debug.Log(DialogueManager.isInDialogue);
        //Debug.Log(isPuzzleLock);
        if (!DialogueManager.isInDialogue && !isPuzzleLock)
        {
            IsLock(false);

            Debug.Log("Unlock");
        }
        IsLock(false);
        Time.timeScale = 1;
        pauseWindow.panelOption.gameObject.SetActive(false);
        pauseWindow.panelPause.gameObject.SetActive(true);
        pauseWindow.SetWindowState(false);
        isPause = false;
        pauseWindow.ResetPauseMenu();
        audioService.ResumeAllAudios();
        // check to change cameras in puzzle 2
        if(SceneManager.GetActiveScene().name == Constants.puzzle02_1SceneName)
        {
            GCP02_01 = GameObject.Find("GameController_Puzzle02_01").GetComponent<GameControllerPuzzle02>();
            if(GCP02_01.cameraFollow)
            {
                GCP02_01.MainCamera.SetActive(false);
                GCP02_01.CannonCamera.SetActive(true);
            }
        }
        if(SceneManager.GetActiveScene().name == Constants.puzzle02_2SceneName)
        {
            GCP02_02 = GameObject.Find("GameController_Puzzle02_02").GetComponent<GameControllerPuzzle02>();
            if(GCP02_02.cameraFollow)
            {
                GCP02_02.MainCamera.SetActive(false);
                GCP02_02.CannonCamera.SetActive(true);
            }
        }
    }

    public void IsLock(bool state)
    {
        try
        {
            Camera.main.GetComponent<CameraController>().isLock = !state;
            GameObject.FindGameObjectWithTag("Player").GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().isLock = state;
        }
        catch { }
    }

    public static void AddScore(int score)
    {
        instance.score += score;
    }
}
