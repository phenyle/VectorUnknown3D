using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;


/// <summary>
/// This whole class originally started as a replacment for Puzzle01
/// That plan was abandoned when we decided to keep both the old Puzzle01
/// and make this into a whole new separate Puzzle.
/// There is still some legacy code that does not apply to the new puzzle
/// I did not write it but touching it makes bad things happen.
/// </summary>

public class GameControllerPuzzle04 : GameControllerRoot
{
    public GameObject topCamPlayer;

    [Header("STAGE LEVEL")]
    [Range(1, 3)]
    public int Difficulty;

    [Header("Database Records")]
    public float tot_puzzleTime;
    private bool pausePuzzTime;
    public List<Puzzle04Controller> puzzleDatas;
    public Puzzle04Controller currPuzzle;


    [Header("Environment Components")]
    public Transform sunLight;
    public Transform moonLight;
    public LPWAsset.LowPolyWaterScript ocean;
    public Material night;


    [Header("Misc Components")]
    public GameObject puzzleWindow;
    public bool inPuzzle = false;

    //MVC Components
    [HideInInspector]
    public Puzzle04Window P04W;
    [HideInInspector]
    public UnityEvent events;

    private AudioService audio04;

    private GameObject player;
    private GameObject mainCamera;
    private Vector3 startPosition;
    private Vector3 previousPosition;
    public int timer = 0;
    public int numCorrect = 0;

    public string questionsList;
    public int totMoves;

    public void Start()
    {
        P04W = puzzleWindow.GetComponent<Puzzle04Window>();

        player = GameObject.FindGameObjectWithTag("Player");

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameRoot.instance.FindPlayerAndCamera();

        pausePuzzTime = false;
        tot_puzzleTime = 0.0f;
        questionsList = "";
        totMoves = 0;
    }

    public override void InitGameController(Puzzle04Window P04W)
    {
        Debug.Log("Init GameController for Puzzle04");
        base.InitGameController();

        Debug.Log("Connect Puzzle04 Window");
        this.P04W = P04W;

        audio04 = audioService;
  //      Debug.Log("Connect Database of Puzzle01");
  //      DBP01 = GetComponent<DatabasePuzzle01>();

  //     Debug.Log("Call Database of Puzzle01 to connect");
  //      DBP01.InitDatabase();

        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameRoot.instance.FindPlayerAndCamera();
        startPosition = player.transform.position;

        //Starter Conversation Managment
        conversation(0);
  //      conversationStart();

        if (GameRoot.instance.puzzleCompleted[0] == true && GameRoot.instance.puzzleCompleted[1] == true)
        {
            SetActive(sunLight, false);
            SetActive(moonLight, true);
            ocean.sun = moonLight.GetChild(0).GetComponent<Light>();
            RenderSettings.skybox = night;
        }

        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("p4puzzles"))
            puzzleDatas.Add(obstacle.GetComponent<Puzzle04Controller>());

        //Init Components
        // SetActive(endportal, false);
    }

    private void Update()
    {

        if (player.transform.position.x - previousPosition.x < 0.01 && player.transform.position.y - previousPosition.y < 0.01 && player.transform.position.z - previousPosition.y < 0.01)
        {
            timer += (int)Time.deltaTime + 1;
            if (timer == 1200)
            {
                GameRoot.ShowTips("Press 'R' to reset position.", true, false);
            }
        }
        else
        {
            timer = 0;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = startPosition;
            GameRoot.ShowTips("", true, false);
        }

        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    player.GetComponent<ThirdPersonUserControl>().enabled = true;
        //    player.GetComponent<ThirdPersonCharacter>().enabled = true;
        //    if(currPuzzle != null && inPuzzle)
        //    {
        //        currPuzzle.OnPortalExit();
        //    }
        //}
/**
        //Update the instruction text based on the player status
        if (isInQues == true)
        {
            if (isTriggerQuestion)
            {
                SetText("- Press 'E': Exit the question.\n" +
                             "- Press 'Z': Top-down camera.");
            }
            else
            {
                SetText("- Press 'E': Trigger the question.");
            }
        }
        else
        {
            SetText("Please stand on the question platform.");
        }
        //************************************************************
**/

        if (!DialogueManager.isInDialogue)
        {
            if (!GameRoot.isPause && !pausePuzzTime)
            {
                tot_puzzleTime += Time.deltaTime;
                int minutes = (int)tot_puzzleTime / 60;
                int seconds = (int)tot_puzzleTime - 60 * minutes;
                if (seconds < 10)
                    P04W.SetTime(minutes.ToString() + " : 0" + seconds.ToString());
                else
                    P04W.SetTime(minutes.ToString() + " : " + seconds.ToString());
            }
        }

        previousPosition = player.transform.position;
    }

    private void conversationStart()
    {
        switch(Difficulty)
        {
            case 1:
                if (DialogueManager.showP04_1[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle04-1_00"));
                    DialogueManager.showP04_1[0] = false;
                }

                break;
            case 2:
                if (DialogueManager.showP04_2[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle04-2_00"));
                    DialogueManager.showP04_2[0] = false;
                }

                break;
            case 3:
                if (DialogueManager.showP04_3[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle04-3_00"));
                    DialogueManager.showP04_3[0] = false;
                }

                break;
        }
    }

    /// <summary>
    /// Selects the conversation dialog to pop up
    /// Requires that the associated bool[] and conversation file be established
    /// with the DialogManager.cs
    /// </summary>
    /// <param name="val"></param>
    public void conversation(int val)
    {
        switch (Difficulty)
        {
            case 1: //Level 1 Dialogs
                if (DialogueManager.showP04_1[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle04-1_0" + val.ToString()));
                    DialogueManager.showP04_1[val] = false;
                }

                break;
            case 2: //Level 2 Dialogs
                if (DialogueManager.showP04_2[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle04-2_0" + val.ToString()));
                    DialogueManager.showP04_2[val] = false;
                }

                break;
            case 3: //Level 3 Dialogs
                if (DialogueManager.showP04_3[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle04-3_0" + val.ToString()));
                    DialogueManager.showP04_3[val] = false;
                }

                break;
        }
    }

    private void SwitchCamera()
    {
        switch (Camera.main.depth)
        {
            case 1:
                Camera.main.depth = -1;
                topCamPlayer.GetComponent<TopCameraPlayer>().isTopCamera = true;
                GameRoot.instance.LockPlayerAndCam(true);
                break;

            case -1:
                Camera.main.depth = 1;
                topCamPlayer.GetComponent<TopCameraPlayer>().isTopCamera = false;

                break;
        }
    }

    public void setResetPos(Vector3 pos)
    {
        startPosition = pos;
    }

    public Vector3 getResetPos()
    {
        return startPosition;
    }

    public AudioService GetAudioService()
    {
        return audio04;
    }


    /// <summary>
    /// Everytime a player correctly answers a puzzle, that puzzle data struct
    /// will be sent to their local playerData.  This is not sent to GSFU unless
    /// the player completes the level, exits via pause->menu, or closes the game
    /// 
    /// This is good for making one consistent object but structs cannot be walked
    /// through.  Unfortunately, GSFU can only parse Serializalble structs, so arrays[],
    /// Lists<>, etc aren't it
    /// </summary>
    /// <param name="puzzle"></param>
    public void SaveLocalPuzzleData(Puzzle04Controller puzzle)
    {
        int index;

        index = puzzleDatas.IndexOf(puzzle);

        switch (Difficulty)
        {
            case 1:
                switch (puzzle.puzzleID)
                {
                    case 1:
                        GameRoot.player.p4_1.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs1 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot.player.p4_1.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs2 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot.player.p4_1.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs3 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot.player.p4_1.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs4 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot.player.p4_1.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs5 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot.player.p4_1.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs6 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot.player.p4_1.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs7 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot.player.p4_1.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs8 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot.player.p4_1.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_1.obs9 = puzzle.puzzleData;
                        GameRoot.player.p4_1.obs9_attempts = puzzle.attemptsList;
                        break;

                }
                break;


            case 2:
                switch (puzzle.puzzleID)
                {
                    case 1:
                        GameRoot.player.p4_2.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs1 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot.player.p4_2.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs2 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot.player.p4_2.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs3 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot.player.p4_2.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs4 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot.player.p4_2.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs5 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot.player.p4_2.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs6 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot.player.p4_2.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs7 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot.player.p4_2.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_2.obs8 = puzzle.puzzleData;
                        GameRoot.player.p4_2.obs8_attempts = puzzle.attemptsList;
                        break;
                }
                break;


            case 3:
                switch (puzzle.puzzleID)
                {
                    case 1:
                        GameRoot.player.p4_3.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_3.obs1 = puzzle.puzzleData;
                        GameRoot.player.p4_3.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot.player.p4_3.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_3.obs2 = puzzle.puzzleData;
                        GameRoot.player.p4_3.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot.player.p4_3.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_3.obs3 = puzzle.puzzleData;
                        GameRoot.player.p4_3.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot.player.p4_3.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_3.obs4 = puzzle.puzzleData;
                        GameRoot.player.p4_3.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot.player.p4_3.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot.player.p4_3.obs5 = puzzle.puzzleData;
                        GameRoot.player.p4_3.obs5_attempts = puzzle.attemptsList;
                        break;
                }
                break;
        }
    }

    public void RecordData()
    {

        switch(Difficulty)
        {
            case 1:
                if (GameRoot.player.records.p4_1_time == 0.0f)
                {

//                    GameRoot.player.users.p4_1clear_time = tot_puzzleTime;
                    GameRoot.player.records.p4_1_time = tot_puzzleTime;
                    GameRoot.player.records.p4_1_numMove = totMoves;
                    GameRoot.player.records.p4_1_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot.player.records.p4_1_time)
                {

//                    GameRoot.player.users.p4_1clear_time = tot_puzzleTime;
                    GameRoot.player.records.p4_1_time = tot_puzzleTime;
                    GameRoot.player.records.p4_1_numMove = totMoves;
                    GameRoot.player.records.p4_1_questions = questionsList;
                }

                GameRoot.player.p4_1.attemptDate = System.DateTime.Now.ToString();

                GameRoot.player.records.p4_1_attempts++;

                //Attempt Username Construction
                GameRoot.player.p4_1.username = GameRoot.player.users.username;
                GameRoot.player.p4_1.attemptNum = GameRoot.player.records.p4_1_attempts;
                GameRoot.player.p4_1.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.p4_1.attemptNum.ToString();

                break;

            case 2:
                if (GameRoot.player.records.p4_2_time == 0.0f)
                {

  //                  GameRoot.player.users.p4_2clear_time = tot_puzzleTime;
                    GameRoot.player.records.p4_2_time = tot_puzzleTime;
                    GameRoot.player.records.p4_2_numMove = totMoves;
                    GameRoot.player.records.p4_2_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot.player.records.p4_2_time)
                {

 //                   GameRoot.player.users.p4_2clear_time = tot_puzzleTime;
                    GameRoot.player.records.p4_2_time = tot_puzzleTime;
                    GameRoot.player.records.p4_2_numMove = totMoves;
                    GameRoot.player.records.p4_2_questions = questionsList;
                }

                GameRoot.player.p4_2.attemptDate = System.DateTime.Now.ToString();

                GameRoot.player.records.p4_2_attempts++;

                //Attempt Username Construction
                GameRoot.player.p4_2.username = GameRoot.player.users.username;
                GameRoot.player.p4_2.attemptNum = GameRoot.player.records.p4_2_attempts;
                GameRoot.player.p4_2.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.p4_2.attemptNum.ToString();

                break;

            case 3:
                if (GameRoot.player.records.p4_3_time == 0.0f)
                {

 //                   GameRoot.player.users.p4_3clear_time = tot_puzzleTime;
                    GameRoot.player.records.p4_3_time = tot_puzzleTime;
                    GameRoot.player.records.p4_3_numMove = totMoves;
                    GameRoot.player.records.p4_3_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot.player.records.p4_3_time)
                {

 //                   GameRoot.player.users.p4_3clear_time = tot_puzzleTime;
                    GameRoot.player.records.p4_3_time = tot_puzzleTime;
                    GameRoot.player.records.p4_3_numMove = totMoves;
                    GameRoot.player.records.p4_3_questions = questionsList;
                }

                GameRoot.player.p4_3.attemptDate = System.DateTime.Now.ToString();

                GameRoot.player.records.p4_3_attempts++;


                //Attempt Username Construction
                GameRoot.player.p4_3.username = GameRoot.player.users.username;
                GameRoot.player.p4_3.attemptNum = GameRoot.player.records.p4_3_attempts;
                GameRoot.player.p4_3.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.p4_3.attemptNum.ToString();

                break;
        }

        GameRoot.player.records.username = GameRoot.player.users.username;
        GameRoot.player.records.p4_totAttempts = GameRoot.player.records.p4_1_attempts + GameRoot.player.records.p4_2_attempts + GameRoot.player.records.p4_3_attempts;
        GameRoot.player.records.p4_totTime += tot_puzzleTime;
    }

    public void SetPuzzlePause(bool val)
    {
        pausePuzzTime = val;
    }


}
