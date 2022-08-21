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
    public static GameControllerPuzzle04 instance;

    [Header("STAGE LEVEL")]
    [Range(1, 3)]
    public int Difficulty;

    [Header("Database Records")]
    public float tot_puzzleTime;
    private bool pausePuzzTime;
    public List<Puzzle04Controller> puzzleDatas;
    public Puzzle04Controller currPuzzle;
    public VisVectController visVector;
    public GrappleCode grappleKit;


    [Header("Environment Components")]
    public Transform sunLight;
    public Transform moonLight;
    public LPWAsset.LowPolyWaterScript ocean;
    public Material night;


    [Header("Misc Components")]
    public EnterExit exitPortal;
    public GameObject puzzleWindow;
    public bool inPuzzle = false;

    //MVC Components
    [HideInInspector]
    public Puzzle04Window P04W;
    [HideInInspector]
    public UnityEvent events;

    private AudioService audio04;

    public GameObject player;
    public GameObject mainCamera;
    private Vector3 startPosition;
    private Vector3 previousPosition;
    public int timer = 0;
    public int numCorrect = 0;

    public string questionsList;
    public int totMoves;
    public int overallIdeal = 0;
    public int overallSteps = 0;
    public float overallScore = 0.0f;
    public float averageScore = 0.0f;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public void Start()
    {
        GameRoot3D.instance.GCR.GCP04 = this;

        P04W = puzzleWindow.GetComponent<Puzzle04Window>();

        GameRoot3D.instance.FindPlayerAndCamera();

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
        GameRoot3D.instance.FindPlayerAndCamera();
        startPosition = player.transform.position;

        //Starter Conversation Managment
        conversation(0);
  //      conversationStart();

        if (GameRoot3D.instance.puzzleCompleted[0] == true && GameRoot3D.instance.puzzleCompleted[1] == true)
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
                GameRoot3D.ShowTips("Press 'R' to reset position.", true, false);
            }
        }
        else
        {
            timer = 0;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = startPosition;
            GameRoot3D.ShowTips("", true, false);
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
            if (!GameRoot3D.instance.isPause && !pausePuzzTime)
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
                if (DialogueManager.instance.showP04_1.Length != 0 && DialogueManager.instance.showP04_1[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(4, 1, 0));
                    DialogueManager.instance.showP04_1[0] = false;
                }

                break;
            case 2:
                if (DialogueManager.instance.showP04_2.Length != 0 && DialogueManager.instance.showP04_2[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(4, 2, 0));
                    DialogueManager.instance.showP04_2[0] = false;
                }

                break;
            case 3:
                if (DialogueManager.instance.showP04_3.Length != 0 && DialogueManager.instance.showP04_3[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(4, 3, 0));
                    DialogueManager.instance.showP04_3[0] = false;
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
                if (DialogueManager.instance.showP04_1.Length >= val + 1 && DialogueManager.instance.showP04_1[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(4, 1, val));
                    DialogueManager.instance.showP04_1[val] = false;
                }

                break;
            case 2: //Level 2 Dialogs
                if (DialogueManager.instance.showP04_2.Length >= val + 1 && DialogueManager.instance.showP04_2[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(4, 2, val));
                    DialogueManager.instance.showP04_2[val] = false;
                }

                break;
            case 3: //Level 3 Dialogs
                if (DialogueManager.instance.showP04_3.Length >= val + 1 && DialogueManager.instance.showP04_3[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(4, 3, val));
                    DialogueManager.instance.showP04_3[val] = false;
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
                GameRoot3D.instance.LockPlayerAndCam(true);
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

    public override void SetSplash()
    {
        base.SetSplash();
        if(instance != null)
        {
            instance.SetPuzzlePause(true);
            instance.P04W.panelCommon.SetActive(false);

            switch (Difficulty)
            {
                case 1:
                    GameRoot3D.instance.GCR?.completeSplash?.SetSplash(
                        GameRoot3D.player.records.p4_1_time,
                        tot_puzzleTime,
                        GameRoot3D.player.records.p4_1_totScore,
                        averageScore);
                    break;

                case 2:
                    GameRoot3D.instance.GCR?.completeSplash?.SetSplash(
                        GameRoot3D.player.records.p4_2_time,
                        tot_puzzleTime,
                        GameRoot3D.player.records.p4_2_totScore,
                        averageScore);
                    break;

                case 3:
                    GameRoot3D.instance.GCR?.completeSplash?.SetSplash(
                        GameRoot3D.player.records.p4_3_time,
                        tot_puzzleTime,
                        GameRoot3D.player.records.p4_3_totScore,
                        averageScore);
                    break;
            }
        }
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
        overallIdeal += puzzle.puzzleData.minMoves;
        overallSteps += puzzle.attempts;
        overallScore = (float)overallIdeal / (float)overallSteps;
        averageScore += puzzle.score;

        switch (Difficulty)
        {
            case 1:
                GameRoot3D.player.p4_1.tot_Ideal = overallIdeal;
                GameRoot3D.player.p4_1.tot_steps = overallSteps;
                GameRoot3D.player.p4_1.tot_score = overallScore;

                switch (puzzle.puzzleID)
                {
                    case 1:
                        GameRoot3D.player.p4_1.obs1_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs1_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs1_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs1_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs1_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs1_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs1_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs1_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs1_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p4_1.obs2_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs2_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs2_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs2_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs2_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs2_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs2_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs2_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs2_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p4_1.obs3_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs3_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs3_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs3_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs3_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs3_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs3_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs3_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs3_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p4_1.obs4_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs4_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs4_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs4_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs4_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs4_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs4_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs4_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs4_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p4_1.obs5_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs5_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs5_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs5_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs5_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs5_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs5_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs5_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs5_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p4_1.obs6_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs6_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs6_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs6_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs6_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs6_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs6_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs6_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs6_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p4_1.obs7_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs7_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs7_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs7_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs7_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs7_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs7_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs7_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs7_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p4_1.obs8_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs8_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs8_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs8_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs8_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs8_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs8_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs8_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs8_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot3D.player.p4_1.obs9_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_1.obs9_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_1.obs9_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_1.obs9_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_1.obs9_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_1.obs9_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_1.obs9_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_1.obs9_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_1.obs9_steps = puzzle.attempts;

                        GameRoot3D.player.p4_1.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_1.obs9 = puzzle.puzzleData;
                        GameRoot3D.player.p4_1.obs9_attempts = puzzle.attemptsList;
                        averageScore /= 9.0f;
                        break;

                }
                break;


            case 2:
                GameRoot3D.player.p4_2.tot_Ideal = overallIdeal;
                GameRoot3D.player.p4_2.tot_steps = overallSteps;
                GameRoot3D.player.p4_2.tot_score = overallScore;

                switch (puzzle.puzzleID)
                {
                    case 1:
                        GameRoot3D.player.p4_2.obs1_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs1_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs1_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs1_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs1_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs1_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs1_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs1_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs1_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p4_2.obs2_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs2_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs2_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs2_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs2_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs2_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs2_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs2_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs2_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p4_2.obs3_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs3_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs3_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs3_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs3_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs3_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs3_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs3_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs3_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p4_2.obs4_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs4_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs4_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs4_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs4_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs4_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs4_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs4_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs4_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p4_2.obs5_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs5_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs5_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs5_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs5_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs5_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs5_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs5_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs5_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p4_2.obs6_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs6_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs6_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs6_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs6_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs6_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs6_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs6_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs6_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p4_2.obs7_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs7_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs7_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs7_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs7_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs7_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs7_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs7_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs7_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p4_2.obs8_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_2.obs8_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_2.obs8_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_2.obs8_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_2.obs8_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_2.obs8_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_2.obs8_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_2.obs8_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_2.obs8_steps = puzzle.attempts;

                        GameRoot3D.player.p4_2.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_2.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p4_2.obs8_attempts = puzzle.attemptsList;
                        averageScore /= 8.0f;

                        break;
                }
                break;


            case 3:
                GameRoot3D.player.p4_3.tot_Ideal = overallIdeal;
                GameRoot3D.player.p4_3.tot_steps = overallSteps;
                GameRoot3D.player.p4_3.tot_score = overallScore;

                switch (puzzle.puzzleID)
                {
                    case 1:
                        GameRoot3D.player.p4_3.obs1_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_3.obs1_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_3.obs1_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_3.obs1_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_3.obs1_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_3.obs1_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_3.obs1_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_3.obs1_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_3.obs1_steps = puzzle.attempts;

                        GameRoot3D.player.p4_3.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_3.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p4_3.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p4_3.obs2_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_3.obs2_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_3.obs2_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_3.obs2_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_3.obs2_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_3.obs2_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_3.obs2_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_3.obs2_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_3.obs2_steps = puzzle.attempts;

                        GameRoot3D.player.p4_3.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_3.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p4_3.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p4_3.obs3_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_3.obs3_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_3.obs3_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_3.obs3_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_3.obs3_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_3.obs3_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_3.obs3_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_3.obs3_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_3.obs3_steps = puzzle.attempts;

                        GameRoot3D.player.p4_3.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_3.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p4_3.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p4_3.obs4_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_3.obs4_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_3.obs4_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_3.obs4_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_3.obs4_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_3.obs4_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_3.obs4_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_3.obs4_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_3.obs4_steps = puzzle.attempts;

                        GameRoot3D.player.p4_3.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_3.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p4_3.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p4_3.obs5_ans = puzzle.puzzleData.answer.ToString();
                        GameRoot3D.player.p4_3.obs5_a = puzzle.puzzleData.e_Vect1.ToString();
                        GameRoot3D.player.p4_3.obs5_b = puzzle.puzzleData.e_Vect2.ToString();
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect1);
                        puzzle.cardVectors.Remove(puzzle.puzzleData.e_Vect2);
                        GameRoot3D.player.p4_3.obs5_c = puzzle.cardVectors[0].ToString();
                        puzzle.cardVectors.RemoveAt(0);
                        GameRoot3D.player.p4_3.obs5_d = puzzle.cardVectors[0].ToString();
                        GameRoot3D.player.p4_3.obs5_x = puzzle.puzzleData.e_Scal1;
                        GameRoot3D.player.p4_3.obs5_y = puzzle.puzzleData.e_Scal2;
                        GameRoot3D.player.p4_3.obs5_Ideal = puzzle.puzzleData.minMoves;
                        GameRoot3D.player.p4_3.obs5_steps = puzzle.attempts;

                        GameRoot3D.player.p4_3.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p4_3.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p4_3.obs5_attempts = puzzle.attemptsList;
                        averageScore /= 5.0f;

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
                if (GameRoot3D.player.records.p4_1_time == 0.0f)
                {
                    GameRoot3D.player.records.p4_1_time = tot_puzzleTime;
                    GameRoot3D.player.records.p4_1_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot3D.player.records.p4_1_time)
                {
                    GameRoot3D.player.records.p4_1_time = tot_puzzleTime;
                    GameRoot3D.player.records.p4_1_questions = questionsList;
                }

                if(GameRoot3D.player.records.p4_1_numMove == 0.0f)
                    GameRoot3D.player.records.p4_1_numMove = totMoves;
                else if(totMoves < GameRoot3D.player.records.p4_1_numMove)
                    GameRoot3D.player.records.p4_1_numMove = totMoves;

                if (GameRoot3D.player.records.p4_1_totScore == 0.0f)
                    GameRoot3D.player.records.p4_1_totScore = overallScore;
                else if (overallScore > GameRoot3D.player.records.p4_1_totScore)
                    GameRoot3D.player.records.p4_1_totScore = overallScore;

                if (GameRoot3D.player.records.p4_1_avgScore == 0.0f)
                    GameRoot3D.player.records.p4_1_avgScore = averageScore;
                else if (averageScore > GameRoot3D.player.records.p4_1_avgScore)
                    GameRoot3D.player.records.p4_1_avgScore = averageScore;

                GameRoot3D.player.p4_1.attemptDate = System.DateTime.Now.ToString();

                GameRoot3D.player.records.p4_1_attempts++;

                //Attempt Username Construction
                GameRoot3D.player.p4_1.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p4_1.attemptNum = GameRoot3D.player.records.p4_1_attempts;
                GameRoot3D.player.p4_1.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p4_1.attemptNum.ToString();
                GameRoot3D.player.p4_1.sessionID = GameRoot3D.player.users.sessionID;

                break;

            case 2:
                if (GameRoot3D.player.records.p4_2_time == 0.0f)
                {
                    GameRoot3D.player.records.p4_2_time = tot_puzzleTime;
                    GameRoot3D.player.records.p4_2_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot3D.player.records.p4_2_time)
                {
                    GameRoot3D.player.records.p4_2_time = tot_puzzleTime;
                    GameRoot3D.player.records.p4_2_questions = questionsList;
                }

                if (GameRoot3D.player.records.p4_2_numMove == 0.0f)
                    GameRoot3D.player.records.p4_2_numMove = totMoves;
                else if (totMoves < GameRoot3D.player.records.p4_2_numMove)
                    GameRoot3D.player.records.p4_2_numMove = totMoves;

                if (GameRoot3D.player.records.p4_2_totScore == 0.0f)
                    GameRoot3D.player.records.p4_2_totScore = overallScore;
                else if (overallScore > GameRoot3D.player.records.p4_2_totScore)
                    GameRoot3D.player.records.p4_2_totScore = overallScore;

                if (GameRoot3D.player.records.p4_2_avgScore == 0.0f)
                    GameRoot3D.player.records.p4_2_avgScore = averageScore;
                else if (averageScore > GameRoot3D.player.records.p4_2_avgScore)
                    GameRoot3D.player.records.p4_2_avgScore = averageScore;

                GameRoot3D.player.p4_2.attemptDate = System.DateTime.Now.ToString();

                GameRoot3D.player.records.p4_2_attempts++;

                //Attempt Username Construction
                GameRoot3D.player.p4_2.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p4_2.attemptNum = GameRoot3D.player.records.p4_2_attempts;
                GameRoot3D.player.p4_2.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p4_2.attemptNum.ToString();
                GameRoot3D.player.p4_2.sessionID = GameRoot3D.player.users.sessionID;

                break;

            case 3:
                if (GameRoot3D.player.records.p4_3_time == 0.0f)
                {
                    GameRoot3D.player.records.p4_3_time = tot_puzzleTime;
                    GameRoot3D.player.records.p4_3_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot3D.player.records.p4_3_time)
                {
                    GameRoot3D.player.records.p4_3_time = tot_puzzleTime;
                    GameRoot3D.player.records.p4_3_questions = questionsList;
                }

                if (GameRoot3D.player.records.p4_3_numMove == 0.0f)
                    GameRoot3D.player.records.p4_3_numMove = totMoves;
                else if (totMoves < GameRoot3D.player.records.p4_3_numMove)
                    GameRoot3D.player.records.p4_3_numMove = totMoves;

                if (GameRoot3D.player.records.p4_3_totScore == 0.0f)
                    GameRoot3D.player.records.p4_3_totScore = overallScore;
                else if (overallScore > GameRoot3D.player.records.p4_3_totScore)
                    GameRoot3D.player.records.p4_3_totScore = overallScore;

                if (GameRoot3D.player.records.p4_3_avgScore == 0.0f)
                    GameRoot3D.player.records.p4_3_avgScore = averageScore;
                else if (averageScore > GameRoot3D.player.records.p4_3_avgScore)
                    GameRoot3D.player.records.p4_3_avgScore = averageScore;

                GameRoot3D.player.p4_3.attemptDate = System.DateTime.Now.ToString();

                GameRoot3D.player.records.p4_3_attempts++;


                //Attempt Username Construction
                GameRoot3D.player.p4_3.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p4_3.attemptNum = GameRoot3D.player.records.p4_3_attempts;
                GameRoot3D.player.p4_3.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p4_3.attemptNum.ToString();
                GameRoot3D.player.p4_3.sessionID = GameRoot3D.player.users.sessionID;

                break;
        }

        GameRoot3D.player.records.username = GameRoot3D.player.users.username;
        GameRoot3D.player.records.sessionID = GameRoot3D.player.users.sessionID;
        GameRoot3D.player.p4_3.sessionID = GameRoot3D.player.users.sessionID;

        GameRoot3D.player.records.p4_totAttempts = GameRoot3D.player.records.p4_1_attempts + GameRoot3D.player.records.p4_2_attempts + GameRoot3D.player.records.p4_3_attempts;
        GameRoot3D.player.records.p4_totTime += tot_puzzleTime;
        GameRoot3D.player.records.sessionID = GameRoot3D.player.users.sessionID;
    }


    public void SetPuzzlePause(bool val)
    {
        pausePuzzTime = val;
    }


}
