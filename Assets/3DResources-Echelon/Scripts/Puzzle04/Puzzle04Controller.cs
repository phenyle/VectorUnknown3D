using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

public enum Direction
{
    X,
    Y,
    Z,
    XY,
    XYZ
};

public class Puzzle04Controller : MonoBehaviour
{
    [Header("---Object Assignment---")]
    public GameObject player;
    //    public GameObject portal;
    public GameObject startPoint;
    public GameObject goal;
    public GameObject goalPoint;
    public GameObject landPoint;
    public GameObject reset;
    public GameObject finish;
    public int puzzleID;
    public bool isActive;

    [Header("---View Controls---")]
    public GameObject mainCamera;
    public GameObject puzzleCamera;
    public GameObject visVectCamera;
    public GameObject cameraTarget;
    public GameObject CameraStartPosition;
    private Vector3 prevCameraPosition;

    [Header("---Puzzle Vectors---")]
    public Vector3 InitFinalVector; //this is the vector input from the Unity Inspector, it's the basis for the FinalAnswer
    public bool isDynamic; //this bool tells the system to slightly randomize the InitFinalVector before setting it as the startingAnswer
    [Range(1, 10)]
    public int FinalDynamicRange;
    public Direction LimitAxis = new Direction();
    private Vector3 DynamicVector;

    [Header("---Answer Card Vectors---")]
    [Range(1, 10)]
    public int MaxCardScalar;
    //   [Range(1, 10)]
    //   public int MaxCardMagnitudes;

    //Object Tools
    [Header("Puzzle Tools")]
    public AnswerGenerator generator;
    public PortalTrigger portal;
    public GapTriggersController gapTriggers;
    private bool inPortal = false;
    public ReleaseGrappleTrigger releaseTrigger;
    private ObjectGlide cameraGlide;
    public float grappleArcHeight = 0.0f;
    private CameraRotate cameraRotate;
    public UnityEvent answer1events { get; private set; }
    public UnityEvent answer2events { get; private set; }

    [Header("---Conversation Select---")]
    [Range(0, 9)]
    [Tooltip("Sets the dialog number used.\n0 val means no dialog\n1-9 selects that specific dialog")]
    public int convoTriggerNumber;


    //Answer Cards
    private Vector3 startingAnswer; //this is the starting vector after being slightly randomized (when applicable) used for answer card calculation
    private Vector3 finalAnswerVector; //this is the Final answer vector that is tailored to match what the pair of answer cards can reproduce
    private List<Vector3> cardVectors;

    //Player Answers
    public Vector3 playerAnswer { get; private set; }
    private Vector3 prevPlayerAnswer = Vector3.zero;
    private bool ans1Entered; //condition flag that a card is slotted in position1 in the UI
    private bool ans2Entered; //condition flag that a card is slotted in position2 in the UI
    private bool correct;

    //MISC
    private int attempts = 0;
    private float attmpWaitTime = 0.0f;
    private bool newAttempt = false;
    

    //Database Records
    private float obstacleTime;

    [HideInInspector]
    [System.Serializable]
    public struct obsData
    {
        public int obsID;
        public float obsTime;
        public Vector3 answer;
        public bool userUniqueSolution;

        public int e_Scal1;
        public int e_Scal2;
        public Vector3 e_Vect1;
        public Vector3 e_Vect2;

        public int i_Scal1;
        public int i_Scal2;
        public Vector3 i_Vect1;
        public Vector3 i_Vect2;

        

        ////Puzzle generated and Expected values (e)
        //public struct expectedData {
        //    public int e_Scal1;
        //    public int e_Scal2;
        //    public Vector3 e_Vect1;
        //    public Vector3 e_Vect2;
        //}
        //public expectedData expected;

        ////Player generated and Input values (i)
        //public struct inputData {
        //    public int i_Scal1;
        //    public int i_Scal2;
        //    public Vector3 i_Vect1;
        //    public Vector3 i_Vect2;
        //}
        //public inputData input;

        public struct attemptsData
        {
            public int attemptNum;
            public float time;
            public int a_Scal1;
            public Vector3 a_Vect1;
            public int a_Scal2;
            public Vector3 a_Vect2;
            public Vector3 a_Ans;
        }
    }

    [HideInInspector]
    public obsData puzzleData;
    [HideInInspector]
    public string attemptsList;

    // Awake is called when the Instance is Initialized
    private void Awake()
    {
        cameraRotate = this.GetComponent<CameraRotate>();
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        puzzleCamera = GameObject.FindGameObjectWithTag("PuzzleCamera");
        cameraGlide = GameObject.FindGameObjectWithTag("cameraTools").GetComponent<ObjectGlide>();
        cameraRotate = GameObject.FindGameObjectWithTag("cameraTools").GetComponent<CameraRotate>();
        visVectCamera = GameObject.Find("VisVectorCamera");
        //have to flicker visvect clear flags on start, because of a weird bug.
        //it should be set to depth, but on Level 3 it only works if it gets reset to depth after
        //starting the level.  So, that's what this does.
        visVectCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        visVectCamera.GetComponent<Camera>().enabled = false;

        gapTriggers.SetPuzzleController(this);
        portal.SetPuzzleController(this);
        inPortal = false;
        releaseTrigger.SetPuzzleController(this);

    }
    void Start()
    { 
        cardVectors = new List<Vector3>();

        answer1events = new UnityEvent();
        answer2events = new UnityEvent();

        //If vector is dynamic, generate an additional vector to add
        //to starting answer
        //if (isDynamic)
        //{
        //    DynamicVector = DynamicGenerator();
        //}
        //else
        //{
        //    DynamicVector = new Vector3(0f, 0f, 0f);
        //}

        generator = new AnswerGenerator(GameControllerPuzzle04.instance.Difficulty, InitFinalVector, FinalDynamicRange, LimitAxis);

        ////Final Answer is created after generating the cards       
        //startingAnswer = InitFinalVector + DynamicVector;


        //Move camera target inbetween the starting position and the goal
        cameraTarget.transform.position += (goalPoint.transform.position - startPoint.transform.position) / 2;

        correct = false;

        obstacleTime = 0.0f;
        puzzleData = new obsData();
        attemptsList = "";
        //puzzleData.expected = new obsData.expectedData();
        //puzzleData.input = new obsData.inputData();
        puzzleData.obsID = puzzleID;

        //       intAnswerGenerator();
        //AnswerGenerators();
 //       AnswerCards();

        cardVectors = generator.AnswerCards(MaxCardScalar);
        puzzleData = generator.PuzzleData(puzzleData);
        finalAnswerVector = generator.FinalAnswer();


    }

    // Update is called once per frame
    void Update()
    {
        if (inPortal)
        {
            if (!DialogueManager.isInDialogue && !GameRoot3D.isPause)
                obstacleTime += Time.deltaTime;

            isActive = true;

            if(newAttempt)
            {
                attmpWaitTime += Time.deltaTime;
                if (attmpWaitTime > 0.5f)
                {
                    attmpWaitTime = 0.0f;
                    newAttempt = false;
                    attemptsList += CreateAttemptCSV();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OnPortalExit();
                GameRoot3D.instance.LockPlayerAndCam(false);
            }
        }
        else
        {
            isActive = false;
        }
    }

    #region Get/Sets

    public Vector3 getAnswerVector()
    {
        return finalAnswerVector;
    }

    public Direction getDirection()
    {
        return LimitAxis;
    }

    public bool getCorrect()
    {
        return correct;
    }
    #endregion

    /// <summary>
    /// This function creates an attemptData CSV string with the current attempt values and returns it.
    /// This is done so that when checking for possible attempt entries to add to the attemptsList
    /// we aren't needlessly creating heap memory items.  The heap memory is only reserved WHEN
    /// EnterAttempt() needs to add a new node
    /// </summary>
    /// <returns></returns>
    private string CreateAttemptCSV()
    {
        string temp = "";
        if (GameControllerPuzzle04.instance.currPuzzle != null)
        {
            temp += attempts.ToString() + ", "; //attempt number
            temp += obstacleTime.ToString("0.00") + ", "; //time attempted at
            temp += GameControllerPuzzle04.instance.P04W.getScalar1Value().ToString() + ", "; //scalar value for vector 1
            if (GameControllerPuzzle04.instance.P04W.card1 != null)
                temp += GameControllerPuzzle04.instance.P04W.card1.cardVector.x.ToString() + ", " + GameControllerPuzzle04.instance.P04W.card1.cardVector.y.ToString() + ", " + GameControllerPuzzle04.instance.P04W.card1.cardVector.z.ToString() + ", "; // vector1 information
            else
                temp += "0,0,0";
            temp += GameControllerPuzzle04.instance.P04W.getScalar2Value().ToString() + ", "; //scalar value for vector 2
            if (GameControllerPuzzle04.instance.P04W.card2 != null)
                temp += GameControllerPuzzle04.instance.P04W.card2.cardVector.x.ToString() + ", " + GameControllerPuzzle04.instance.P04W.card2.cardVector.y.ToString() + ", " + GameControllerPuzzle04.instance.P04W.card2.cardVector.z.ToString(); // vector 2 information
            else
                temp += "0,0,0";
            temp += "\n";
        }
        return temp;
    }

    private void EnterAttempts()
    {
        attmpWaitTime = 0.0f;
        newAttempt = true;
    }

    public Vector3 CalculatePlayerAnswer()
    {
        Vector3 vect1, vect2;
        if (GameControllerPuzzle04.instance.P04W.card1 != null)
            vect1 = (int)GameControllerPuzzle04.instance.P04W.scalar1.value *
                        GameControllerPuzzle04.instance.P04W.card1.cardVector;
        else
            vect1 = Vector3.zero;

        if (GameControllerPuzzle04.instance.P04W.card2 != null)
            vect2 = (int)GameControllerPuzzle04.instance.P04W.scalar2.value *
                        GameControllerPuzzle04.instance.P04W.card2.cardVector;
        else
            vect2 = Vector3.zero;

        playerAnswer = vect1 + vect2;

        return playerAnswer;
    }

    public bool checkFinalAnswer()
    {
        if (GameControllerPuzzle04.instance.P04W.card1 != null && GameControllerPuzzle04.instance.P04W.card2 != null)
        {
            if (playerAnswer == finalAnswerVector)
            {
                //on first time answering correctly
                if (!correct)
                    GameControllerPuzzle04.instance.P04W.SetComplete();
                correct = true;

                GameRoot3D.instance.LockCamera(false);
                answer1events.RemoveAllListeners();
                answer2events.RemoveAllListeners();
                turnOffVisualVectors();
                GameControllerPuzzle04.instance.grappleKit.InitGrapple(correct, finalAnswerVector, goalPoint, player.transform.position, grappleArcHeight);
                GameControllerPuzzle04.instance.grappleKit.grappleToGoal(this);

                puzzleData.obsTime = this.obstacleTime;
                puzzleData.i_Scal1 = GameControllerPuzzle04.instance.P04W.getScalar1Value();
                puzzleData.i_Vect1 = GameControllerPuzzle04.instance.P04W.card1.cardVector;
                puzzleData.i_Scal2 = GameControllerPuzzle04.instance.P04W.getScalar2Value();
                puzzleData.i_Vect2 = GameControllerPuzzle04.instance.P04W.card2.cardVector;

                //Did the player arrive at the answer with the expected Scal/Vectors?
                if (puzzleData.e_Scal1 != puzzleData.i_Scal1 ||
                    puzzleData.e_Scal2 != puzzleData.i_Scal2 ||
                    puzzleData.e_Vect1 != puzzleData.i_Vect1 ||
                    puzzleData.e_Vect2 != puzzleData.i_Vect2)
                    puzzleData.userUniqueSolution = true;
                else
                    puzzleData.userUniqueSolution = false;

                GameControllerPuzzle04.instance.totMoves += attempts;
                GameControllerPuzzle04.instance.questionsList += "[" +
                                        finalAnswerVector.x.ToString() + ", " +
                                        finalAnswerVector.y.ToString() + ", " +
                                        finalAnswerVector.z.ToString() + "]\n";

                GameControllerPuzzle04.instance.SaveLocalPuzzleData(this);

                return true;
            }
            else
            {
                correct = false;
                GameControllerPuzzle04.instance.grappleKit.SetFailShot(GameControllerPuzzle04.instance.visVector.vector2end.transform.position, this);
                GameControllerPuzzle04.instance.P04W.SubmitButton.interactable = false;
                turnOffVisualVectors();
                //grappleKit.InitGrapple(correct, finalAnswerVector, VV01.vector2end, player.transform.position, grappleArcHeight);
                //grappleKit.grappleToGoal(this);


                return false;
            }
        }
        else
        {
            GameRoot3D.instance.dynamicWindow.ShowTips("You must select two answer cards", true, true);

            return false;
        }

    }



    //------------------------------------
    // TOOL ACTIONS
    //------------------------------------
    #region Tool Actions
    // PORTAL---------------
    #region Portal
    public void OnPortalEnter()
    {
        prevCameraPosition = mainCamera.transform.position;
        inPortal = true;
        GameControllerPuzzle04.instance.currPuzzle = this;
        GameControllerPuzzle04.instance.SetPuzzlePause(true);

        answer1events.RemoveAllListeners();
        answer2events.RemoveAllListeners();

        answer1events.AddListener(EnterAttempts);
        answer2events.AddListener(EnterAttempts);

        //visvect listeners
        answer1events.AddListener(GameControllerPuzzle04.instance.visVector.Vector1EventList);
        answer2events.AddListener(GameControllerPuzzle04.instance.visVector.Vector2EventList);
        answer1events.AddListener(GameControllerPuzzle04.instance.visVector.FinalVectEventList);
        answer2events.AddListener(GameControllerPuzzle04.instance.visVector.FinalVectEventList);

        answer1events.AddListener(GameControllerPuzzle04.instance.grappleKit.GrappleEventListener);
        answer2events.AddListener(GameControllerPuzzle04.instance.grappleKit.GrappleEventListener);



        GameRoot3D.isPuzzleLock = true;
        GameRoot3D.instance.LockPlayer(true);

        visVectCamera.GetComponent<Camera>().enabled = true;
        visVectCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;

        cameraRotate.SetCameraTarget(cameraTarget.transform.position);

        puzzleCamera.transform.position = mainCamera.transform.position;
        puzzleCamera.transform.rotation = mainCamera.transform.rotation;      
        
        puzzleCamera.GetComponent<Camera>().enabled = true;
        mainCamera.GetComponent<Camera>().enabled = false;
       

        cameraGlide.SetObjectRotateSpeed(0.07f);
        GameRoot3D.camEvents.RemoveAllListeners();
        cameraGlide.SetArrivedAtDistance(0.01f);
        cameraGlide.SetEventSys(GameRoot3D.camEvents);
        cameraGlide.GlideAndLookMovingTarget(puzzleCamera, CameraStartPosition, cameraTarget, 0, 0.1f, 0.02f);

        GameRoot3D.camEvents.AddListener(activateCamRotate);

        GameControllerPuzzle04.instance.P04W.setPuzzleController(this);
        GameControllerPuzzle04.instance.visVector.InitVisVector();


        //Dialog Trigger
        if (convoTriggerNumber != 0)
        {
            GameControllerPuzzle04.instance.conversation(convoTriggerNumber);
            DialogueManager.isPuzzleLock = true;
        }

        GameControllerPuzzle04.instance.P04W.ShowInputPanel(true);
        GameControllerPuzzle04.instance.P04W.ShowFeedbackPanel(true);
        GameControllerPuzzle04.instance.P04W.ShowCardPanel(true);
        GameControllerPuzzle04.instance.P04W.ResetCardPositions();
        GameControllerPuzzle04.instance.P04W.ShowButtonPanel(true);
        GameControllerPuzzle04.instance.P04W.exitPanel.gameObject.SetActive(true);
        GameControllerPuzzle04.instance.inPuzzle = true;

        GameControllerPuzzle04.instance.P04W.resetButtons();

        GameControllerPuzzle04.instance.grappleKit.ropeStart.transform.position = this.startPoint.transform.position;
        GameControllerPuzzle04.instance.grappleKit.ropeEnd.transform.position = this.startPoint.transform.position;


        GameControllerPuzzle04.instance.P04W.SetDisplays(cardVectors);
        GameControllerPuzzle04.instance.P04W.setAnswer1(null);
        GameControllerPuzzle04.instance.P04W.setAnswer2(null);

        turnOnVisualVectors(false);

        GameRoot3D.instance.ShowCursor(true);
    }

    public void OnPortalExit()
    {
        inPortal = false;
        isActive = false;
        GameControllerPuzzle04.instance.SetPuzzlePause(true);
        GameControllerPuzzle04.instance.currPuzzle = null;

        GameRoot3D.isPuzzleLock = true;

        answer1events.RemoveAllListeners();
        answer2events.RemoveAllListeners();


        visVectCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        visVectCamera.GetComponent<Camera>().enabled = false;        

        if (getCorrect())
        {
            ToggleAllTriggers(false);
            float speed = 0.25f + (puzzleCamera.transform.position - mainCamera.transform.position).magnitude / 10f;
            cameraGlide.SetObjectMoveSpeed(speed);
            cameraGlide.SetObjectRotateSpeed(0.3f);
        }
        portal.alarmCollider();

        cameraRotate.enabled = false;
        GameRoot3D.camEvents.RemoveAllListeners();
        cameraGlide.SetArrivedAtDistance(0.01f);
        cameraGlide.SetEventSys(GameRoot3D.camEvents);
        cameraGlide.GlideAndLookMovingTarget(puzzleCamera, mainCamera, player, 0, 0.08f, 0.02f);

        GameRoot3D.camEvents.AddListener(activateCamPlayer);

        GameControllerPuzzle04.instance.P04W.ShowInputPanel(false);
        GameControllerPuzzle04.instance.P04W.ShowFeedbackPanel(false);
        GameControllerPuzzle04.instance.P04W.ShowCardPanel(false);
        GameControllerPuzzle04.instance.P04W.ShowButtonPanel(false);
        GameControllerPuzzle04.instance.P04W.exitPanel.gameObject.SetActive(false);
        GameControllerPuzzle04.instance.inPuzzle = false;

        GameControllerPuzzle04.instance.P04W.resetButtons();

        GameControllerPuzzle04.instance.P04W.ResetCardPositions();

        GameControllerPuzzle04.instance.P04W.setAnswer1(null);
        GameControllerPuzzle04.instance.P04W.setAnswer2(null);

        GameControllerPuzzle04.instance.P04W.scalar1.value = 1;
        GameControllerPuzzle04.instance.P04W.scalar2.value = 1;

        turnOffVisualVectors();

        DialogueManager.isPuzzleLock = false;

        GameRoot3D.instance.ShowCursor(false);
    }


    #endregion

    //GAP TRIGGERS------------------
    #region Gap Triggers
    public void ToggleAllTriggers(bool value)
    {
        gapTriggers.ToggleFallTriggers(value);
        gapTriggers.ToggleFinishTriggers(value);
        gapTriggers.ToggleResetTriggers(value);
    }

    #endregion

    // VISUAL VECTOR----------------
    #region Visual Vector
    public void turnOnVisualVectors(bool all)
    {
        if (all)
        {
            GameControllerPuzzle04.instance.visVector.ToggleBasicVectors(true);
        }
        else
        {
            GameControllerPuzzle04.instance.visVector.getGapVector().SetActive(true);
        }
        if (GameControllerPuzzle04.instance.P04W.axisOn)
            GameControllerPuzzle04.instance.visVector.AxisDirector();
        if (GameControllerPuzzle04.instance.P04W.gridOn)
            GameControllerPuzzle04.instance.visVector.GridDirector();


        switch (GameControllerPuzzle04.instance.Difficulty)
        {
            case 1:
                switch (getDirection())
                {
                    case Direction.X:
                        GameControllerPuzzle04.instance.visVector.setAxisNodes(true, false, false);
                        GameControllerPuzzle04.instance.visVector.setGridBarNodes(true, false, false);
                        break;
                    case Direction.Y:
                        GameControllerPuzzle04.instance.visVector.setAxisNodes(false, true, false);
                        GameControllerPuzzle04.instance.visVector.setGridBarNodes(false, true, false);
                        break;
                }
                break;
            case 2:
                GameControllerPuzzle04.instance.visVector.setAxisNodes(true, true, false);
                GameControllerPuzzle04.instance.visVector.setGridBarNodes(true, true, false);
                break;
            case 3:
                GameControllerPuzzle04.instance.visVector.setAxisNodes(true, true, true);
                GameControllerPuzzle04.instance.visVector.setGridBarNodes(true, true, true);
                break;
        }

    }

    public void turnOffVisualVectors()
    {
        //Basic is player vectors, goal vector, orbs
        GameControllerPuzzle04.instance.visVector.ToggleBasicVectors(false);

        GameControllerPuzzle04.instance.visVector.getXvector().SetActive(false);
        GameControllerPuzzle04.instance.visVector.getYvector().SetActive(false);
        GameControllerPuzzle04.instance.visVector.getZvector().SetActive(false);

        GameControllerPuzzle04.instance.visVector.deactivateAxis();
        GameControllerPuzzle04.instance.visVector.deactivateBarGrid();
        GameControllerPuzzle04.instance.visVector.toggleProjections(false);
        GameControllerPuzzle04.instance.visVector.toggleAnchors(false);
    }

    #endregion

    //CAMERA GLIDE/ROTATE------------------
    #region Camera Glide/Rotate
    private void activateCamRotate()
    {
        GameControllerPuzzle04.instance.SetPuzzlePause(false);  
        GameRoot3D.camEvents.RemoveListener(activateCamRotate);
 //       player.GetComponent<ThirdPersonCharacter>().enabled = true;
        cameraRotate.enabled = true;
    }

    private void activateCamPlayer()
    {
        GameControllerPuzzle04.instance.SetPuzzlePause(false);
        GameRoot3D.camEvents.RemoveListener(activateCamPlayer);

        puzzleCamera.transform.position = mainCamera.transform.position;
        puzzleCamera.transform.LookAt(player.transform, Vector3.up);
        cameraRotate.enabled = false;

        GameRoot3D.instance.LockPlayer(false);

        visVectCamera.GetComponent<Camera>().enabled = false;
        puzzleCamera.GetComponent<Camera>().enabled = false;
        mainCamera.GetComponent<Camera>().enabled = true;
    }
    #endregion
    #endregion
}

