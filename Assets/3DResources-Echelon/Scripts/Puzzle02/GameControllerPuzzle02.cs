using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using MyBox;

public class GameControllerPuzzle02 : GameControllerRoot
{
    [Range(1, 3)]
    public int Difficulty;
    public GameObject diffScalar;

    //variables 
    public int[] currentTransformMatrix = new int[] { 0, 0, 0, 0 };
    public int[] currentVector = new int[] { 0, 0 };

    private int[] selectedTransformMatrix = new int[] { 0, 0, 0, 0 };
    public int[] selectedVector = new int[] { 0, 0 };

    public GameObject endPortal;

    [Header("--------Player/Camera Parts---------")]
    public GameObject player;
    public GameObject mainCamera;
    public GameObject chaseCamera;
    public GameObject topCamera;
    public GameObject visVectCamera;
    public bool topCameraActive = false;
    private bool showVector = false;


    [Header("--------Matrix Controls---------")]
    [MinMaxRange(1, 50)]
    public MinMaxInt magRange = new MinMaxInt(6, 10);
    [MinMaxRange(0, 180)]
    public MinMaxFloat thetaRange = new MinMaxFloat(60, 120);
    [MinMaxRange(0.5f, 2.0f)]
    public MinMaxFloat aspectRange = new MinMaxFloat(0.75f, 1.25f);
    [Range(1,20)]
    [Tooltip("The ratio value of the matrix vector magnitudes vs the vector magnitudes\n" +
        "A higher value here favors larger vectors, a lower value favors larger matricies")]
    public int ratioValue = 10;


    [Header("--------Ship Parts---------")]
    public GameObject shipMaster;
    //    public GameObject airShipMaster;
    [Range(0, 10, order = 1)]
    public int numberOfShips;
    //   [Range(0, 10, order = 1)]
    //   public int numberOfAirships;
    private List<PuzzleController02> shipPuzzles;
    //   private List<PuzzleController02> airshipPuzzles;
    public float minDistApart = 10;
    private PuzzleController02 currTarget;
    public GameObject fanFair;


    [Header("--------Cannon Parts---------")]
    public List<cannonController> cannons;
    public cannonController currCannon;
    public mainCannonController mainCannon;
    public Material currCannonMat;

    [Header("--------Cannonball Parts---------")]
    public List<cannonballController> balls;
    public cannonballController currBall;


    [Header("--------Puzzle Parts---------")]
    public int[] currVector;
    public bool vectChosen = false;
    public List<int[][]> matrixList; //a generate list of 4 possible answer matricies
    public int[][] currMatrix;
    public bool matrixChosen = false;
    public int[] lastAnswer;
    public Vector3 currAnswer;
    [HideInInspector]
    [System.Serializable]
    public struct stageData
    {
        public int difficulty;
        public int attemptNum;
        public float tot_Time;
        public int numOfShips;
        public int numOfShots;
    }
    [HideInInspector]
    public stageData attemptData;

    [Header("--------Axis Parts---------")]
    public GameObject AxisMain;
    public VisVect2 VV02;

    public GameObject firingCannon;

    [HideInInspector]
    public Puzzle02Window P02W;


    private Vector3 startPosition;

    [Header("---Scoring---")]
    public int maxTime = 90;
    private List<PuzzleController02.obsData> data;
    private List<FaceObject> starBars;
    private float averageScore;

    //camera stuff


    //trigger bools
    public bool isMainCannonTrigger = false;

    //UI
    public bool topViewOn = false;
    public int ActiveBoat = 0;
    public Image[] shipImages;
    public Sprite activeBoat;
    public Sprite inactiveBoat;
    public int timer = 0;
    public bool firedCannonYet = false;

    //particle systems
    public GameObject cannonBlast;

    //Boat selection feature

    private Vector3 previousPosition;

    // Vector cannons
    public Material currentCannonMaterial;

    //stage control
    public int levelNumber;
    private bool portalActive = false;

    //Database Records
    public float tot_puzzleTime = 0;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameRoot3D.instance.FindPlayerAndCamera();

        cannons = new List<cannonController>();
        foreach (GameObject cannon in GameObject.FindGameObjectsWithTag("Cannon"))
            cannons.Add(cannon.GetComponent<cannonController>());

        balls = new List<cannonballController>();
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("CannonBall"))
            balls.Add(ball.GetComponent<cannonballController>());

        shipPuzzles = new List<PuzzleController02>();
        //       airshipPuzzles = new List<PuzzleController02>();

        data = new List<PuzzleController02.obsData>();
        starBars = new List<FaceObject>();

        switch (SceneManager.GetActiveScene().name)
        {
            case "04Puzzle02-1": levelNumber = 1; break;
            case "04Puzzle02-2": levelNumber = 2; break;
            case "04Puzzle02-3": levelNumber = 3; break;
        }
        GameRoot3D.instance.GCR.GCP02 = this;

        endPortal.SetActive(false);
        fanFair.SetActive(false);
    }

    public override void InitGameController(Puzzle02Window P02W)
    {

        Debug.Log("Init GameController Puzzle02");
        base.InitGameController();

        Debug.Log("Connect Puzzle02 Window");
        this.P02W = P02W;

        //GR = GameObject.FindGameObjectWithTag("").GetComponent<GameRoot>();

        player = GameObject.FindGameObjectWithTag("Player");
        GameRoot3D.instance.SetScenePlayer(player);
        GameRoot3D.instance.FindPlayerAndCamera();
        startPosition = player.transform.position;

        //DATABASE INIT STUFF
        attemptData.difficulty = Difficulty;
        attemptData.tot_Time = 0.0f;
        attemptData.numOfShips = numberOfShips;
        attemptData.numOfShots = 0;

        conversation(0);

        //this.diffScalar.transform.localScale *= (float)Difficulty / 3.0f;

        //GenerateShipLocations();
        shipMaster.SetActive(true);
        CreateMatrices();
        AltGenerateShipLocations();
        endPortal.SetActive(false);


        int random = Random.Range(0, shipPuzzles.Count);
        currTarget = shipPuzzles[random];
        PickTarget(currTarget);

        // teachers' attempt record: one shot per ship
        string ships = "";
        foreach (PuzzleController02 ship in shipPuzzles)
            ships += (ships.Length > 0 ? ", " : "") + AttemptLog.Vec(ShipTarget(ship));
        attempt = new AttemptLog("3D", "P2", levelNumber > 0 ? levelNumber : Difficulty, 0,
                                 "Hit " + shipPuzzles.Count + " ships at " + ships +
                                 " by choosing a basis {b1, b2} (cannonball) and coordinates (a, b) (cannon) with a·b1 + b·b2 = ship",
                                 shipPuzzles.Count, () => tot_puzzleTime);
    }

    private static Vector2 ShipTarget(PuzzleController02 ship)
    {
        Vector3 p = ship.shipParent.transform.position;
        return new Vector2(Mathf.Round(p.x), Mathf.Round(p.z));
    }

    // a shot a·b1 + b·b2 at the current target, for the attempt record
    private void RecordShot()
    {
        Vector2 target = ShipTarget(currTarget);
        Vector2 b1 = new Vector2(currMatrix[0][0], currMatrix[0][1]);
        Vector2 b2 = new Vector2(currMatrix[1][0], currMatrix[1][1]);
        Vector3[] intended = null;
        if (currTarget.ansMatrix != null)
            intended = new Vector3[] { new Vector2(currTarget.ansMatrix[0][0], currTarget.ansMatrix[0][1]),
                                       new Vector2(currTarget.ansMatrix[1][0], currTarget.ansMatrix[1][1]) };
        attempt.AddMove("Ship " + AttemptLog.Vec(target) + ": " + AttemptLog.Combination(currVector[0], b1, currVector[1], b2, true),
                        MathMistakes.Classify(target, b1, currVector[0], b2, currVector[1], intended));
    }

    private void CreateMatrices()
    {
        matrixList = new List<int[][]>();

        for (int i = 0; i < 4; i++)
        {
            float[] pos = new float[2];
            int[][] matrix = new int[2][] { new int[2], new int[2] };

            float theta;

            Vector2 x1 = new Vector2(1, 0);
            theta = Random.Range(0.0f, 2 * Mathf.PI);

            float s = Mathf.Cos(theta) * x1.x - Mathf.Sin(theta) * x1.y;
            float t = Mathf.Sin(theta) * x1.x + Mathf.Cos(theta) * x1.y;
            int randScal = Random.Range(magRange.Min, magRange.Max);
            Vector2 k = new Vector2(s, t) * randScal;
            k.x = Mathf.RoundToInt(k.x);
            k.y = Mathf.RoundToInt(k.y);

            x1 = k;

            //Rotating the second Vector
            theta = Random.Range(Mathf.Deg2Rad * thetaRange.Min, Mathf.Deg2Rad * thetaRange.Max);
            if (Random.Range(0.0f, 1.0f) < 0.5f)
                theta = (theta * -1);

            s = Mathf.Cos(theta) * x1.normalized.x - Mathf.Sin(theta) * x1.normalized.y;
            t = Mathf.Sin(theta) * x1.normalized.x + Mathf.Cos(theta) * x1.normalized.y;

            k = new Vector2(s, t);

            //scoring phase
            float bestScore = 99;
            float bestScal = 0;
            float magV1 = x1.magnitude;
            //Looking through possible vector candidates that are within our aspect ratio and scoring them by distance
            for (float j = Mathf.RoundToInt(magV1 * aspectRange.Min); j < Mathf.RoundToInt(magV1 * aspectRange.Max); j+=0.01f)
            {
                Vector2 tempRound = new Vector2(Mathf.Round(k.x * j), Mathf.Round(k.y * j));
                float score = Mathf.Abs((tempRound - (k * j)).magnitude);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestScal = j;
                }
            }

            Vector2 x2 = new Vector2(Mathf.Round(k.x * bestScal), Mathf.Round(k.y * bestScal));

            matrix[0][0] = (int)x1.x;
            matrix[0][1] = (int)x1.y;
            matrix[1][0] = (int)x2.x;
            matrix[1][1] = (int)x2.y;

            matrixList.Add(matrix);
        }

    }

    void Update()
    {
        if (!DialogueManager.isInDialogue && !GameRoot3D.instance.isPause)
        {
            attemptData.tot_Time += Time.deltaTime;
            if (currTarget != null)
            {
                currTarget.puzzleData.obsTime += Time.deltaTime;
            }

            int minutes = (int)attemptData.tot_Time / 60;
            int seconds = (int)attemptData.tot_Time - 60 * minutes;
            if (seconds < 10)
                P02W.SetTime(minutes.ToString() + " : 0" + seconds.ToString());
            else
                P02W.SetTime(minutes.ToString() + " : " + seconds.ToString());
        }



        if (!DialogueManager.isInDialogue && !GameRoot3D.instance.isPause)
            tot_puzzleTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = startPosition;
            GameRoot3D.ShowTips("", true, false);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GameRoot3D.ShowTips("", true, false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PickNextTarget();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchCamera();

            Debug.Log("Z was pressed");
        }
        if (isMainCannonTrigger && Input.GetKeyDown(KeyCode.E) && mainCannon.canFire)
        {
            if (vectChosen && matrixChosen && !mainCannon.animate)
            {
                currTarget.CreateAttemptCSV(currMatrix, currVector);
                RecordShot();
                //VV02.ToggleVectorLine(true);
                mainCannon.FireCannon(currAnswer, currAnswer == currTarget.shipParent.transform.position);
                audioService.PlayFXAudio(Constants.audioP02CannonFire);
                showVector = true;
                ResetSelections();
                //              FireCannon();
                Debug.Log("cannon fired");
            }

        }

    }


    private void conversationStart()
    {
        switch (Difficulty)
        {
            case 1:
                if (DialogueManager.instance.showP02_1.Length != 0 && DialogueManager.instance.showP02_1[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(2, 1, 0));
                    DialogueManager.instance.showP02_1[0] = false;
                }

                break;
            case 2:
                if (DialogueManager.instance.showP02_2.Length != 0 && DialogueManager.instance.showP02_2[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(2, 2, 0));
                    DialogueManager.instance.showP02_2[0] = false;
                }

                break;
            case 3:
                if (DialogueManager.instance.showP02_3.Length != 0 && DialogueManager.instance.showP02_3[0])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(2, 3, 0));
                    DialogueManager.instance.showP02_3[0] = false;
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
                if (DialogueManager.instance.showP02_1.Length >= val + 1 && DialogueManager.instance.showP02_1[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(2, 1, val));
                    DialogueManager.instance.showP02_1[val] = false;
                }

                break;
            case 2: //Level 2 Dialogs
                if (DialogueManager.instance.showP02_2.Length >= val + 1 && DialogueManager.instance.showP02_2[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(2, 2, val));
                    DialogueManager.instance.showP02_2[val] = false;
                }

                break;
            case 3: //Level 3 Dialogs
                if (DialogueManager.instance.showP02_3.Length >= val + 1 && DialogueManager.instance.showP02_3[val])
                {
                    FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(2, 3, val));
                    DialogueManager.instance.showP02_3[val] = false;
                }

                break;
        }
    }


    public void ToggleOverlay(bool val)
    {
        //placeholder for Puzzle2UI class to access the visual vector class
        switch (Difficulty)
        {
            case 1:
                VV02.ToggleLattice(val);
                VV02.ToggleVectorLine(val);
                break;
            case 2:
                VV02.ToggleLattice(val);
                VV02.ToggleVectorLine(val && showVector);
                break;
            case 3:
                VV02.ToggleLattice(false);
                VV02.ToggleVectorLine(val && showVector);
                break;
        }
    }

    public void ToggleTargetLines(bool val)
    {
        foreach (PuzzleController02 ship in shipPuzzles)
        {
            ship.shipLine.gameObject.SetActive(val);
        }
    }


    public void SinkShip()
    {
  //      currTarget.animator.Play("ShipSink")
  //      currTarget.anime.Play("ShipSink");
        currTarget.anim.SetTrigger("Sink");

        currTarget.ToggleFrontText(false);
        currTarget.CalculateScore(maxTime);
        data.Add(currTarget.puzzleData);
        starBars.Add(currTarget.scoreFace);


        currTarget.vectText.gameObject.SetActive(false);
        currTarget.vectTextTop.gameObject.SetActive(false);
        currTarget.scoreFace.SetTarget(player.transform);
        currTarget.scoreFace.gameObject.SetActive(true);
        currTarget.starBar.SetSize(6f, 1.0f);
        currTarget.starBar.SetScore(currTarget.puzzleData.score);

        SaveLocalPuzzleData(currTarget);
        
        shipPuzzles.Remove(currTarget);
        P02W.SetCompleteNumber(numberOfShips - shipPuzzles.Count, numberOfShips);

        if (shipPuzzles.Count > 0)
        {
            int random = Random.Range(0, shipPuzzles.Count);
            currTarget = shipPuzzles[random];
            PickTarget(currTarget);
        }
        else
        {
            mainCannon.ToggleTriggerBox(false);
            endPortal.SetActive(true);
            fanFair.SetActive(true);
            foreach (cannonController cannon in cannons)
                cannon.gameObject.SetActive(false);
            foreach (cannonballController ball in balls)
                ball.gameObject.SetActive(false);
            // you win
        }
    }

    public void PickTarget(PuzzleController02 target)
    {
        //check that there isn't some other target in the way, and if there is
        //choose that target instead;

        RaycastHit hitcheck;

        if (Physics.SphereCast(new Vector3(0, 0.5f, 5.0f), 2.0f, target.transform.position, out hitcheck))
        {
            PuzzleController02 temp = new PuzzleController02();
            if (hitcheck.transform.TryGetComponent(out temp))
            {
                target = temp;
            }
        }

        if (topViewOn)
            foreach (PuzzleController02 ship in shipPuzzles)
            {
                ship.ToggleTopText(true);
                ship.ToggleFrontText(false);
                ship.shipLine.gameObject.SetActive(true);
                ship.shipLine.material = ship.lineMatDef;
            }
        else
            foreach (PuzzleController02 ship in shipPuzzles)
            {
                ship.ToggleTopText(false);
                ship.shipLine.material = ship.lineMatDef;
                ship.shipLine.gameObject.SetActive(false);
            }

        target.shipLine.material = target.lineMatActive;


        P02W.SetTargetShip(target.transform.position);
        target.ToggleFrontText(true);
        target.vectText.GetComponent<RubberizeAnim>().ToggleActivate(true);
        target.vectTextTop.GetComponent<RubberizeAnim>().ToggleActivate(true);
        target.vectText.GetComponent<FaceObject>().SetTarget(player.transform);
        target.vectText.GetComponent<FaceObject>().enabled = true;
        //set cannon ansVectors and balls ansMatricies
        for (int i = 0; i < 4; i++)
        {
            cannons[i].SetCannon(target.vectorList[i]);
            balls[i].SetCannonBalls(target.matrixList[i]);
        }
    }

    public void PickNextTarget()
    {
        if (shipPuzzles.Count <= 0)
            return;

        currTarget.vectTextTop.GetComponent<RubberizeAnim>().ToggleActivate(false);
        currTarget.vectText.GetComponent<RubberizeAnim>().ToggleActivate(false);
        currTarget.vectText.transform.localScale = new Vector3(1, -1, 1);
        currTarget.vectTextTop.transform.localScale = new Vector3(2, 2, 1);
        currTarget.ToggleFrontText(false);


        vectChosen = false;
        currVector = new int[3] { 0, 0, 0 };
        P02W.SetVectorSelected(currVector);

        lastAnswer = new int[3] { (int)currAnswer.x, 0, (int)currAnswer.z };

        mainCannon.mainBarrel.SetActive(false);
        mainCannon.ToggleTriggerBox(false);


        int index = shipPuzzles.IndexOf(currTarget);
        index++;
        if (index > shipPuzzles.Count - 1)
        {
            currTarget = shipPuzzles[0];
            PickTarget(currTarget);
        }
        else
        {
            currTarget = shipPuzzles[index];
            PickTarget(currTarget);
        }
    }

    public void SelectCannon(cannonController cannon)
    {
        currCannon = cannon;
        currVector = currCannon.vector;
        mainCannon.mainBarrel.SetActive(true);
        mainCannon.SetCannonText(new int[3] { 0, 0, 0 });
        vectChosen = true;
        showVector = false;
        P02W.SetVectorSelected(currVector);
        CalculateAnswer();

        if (matrixChosen)
        {
            currTarget.CreateAttemptCSV(currMatrix, currVector);
            mainCannon.AnimateCannon();
            mainCannon.cannonFuzeFX.SetActive(true);
            //    mainCannon.ToggleTriggerBox(true);

            VV02.SetMasterVector(currAnswer);
        }
        else
            currTarget.CreateAttemptCSV(new int[2][] { new int[2] { 0, 0 }, new int[2] { 0, 0 } }, currVector);

        currTarget.puzzleData.steps++;
        //TODO
        //connect this to the cannonRotate class
        //connect this to the WindowUI
        //stretch connect this to the VisualVector
    }

    public void SelectCannonBalls(cannonballController ball)
    {
        currBall = ball;
        currMatrix = currBall.matrix;
        matrixChosen = true;
        mainCannon.SetCannonText(new int[3] { 0, 0, 0 });
        P02W.SetMatrixSelected(currMatrix);

        Vector3 vector1 = new Vector3(currMatrix[0][0], 0, currMatrix[0][1]);
        Vector3 vector2 = new Vector3(currMatrix[1][0], 0, currMatrix[1][1]);
        VV02.CreateLattice(vector1, vector2);

        showVector = false;
        P02W.showOverlay = true;
        if (Difficulty != 3)
            SwitchCamera();

        CalculateAnswer();

        if (vectChosen)
        {
            currTarget.CreateAttemptCSV(currMatrix, currVector);
            mainCannon.AnimateCannon();
            mainCannon.cannonFuzeFX.SetActive(true);
            //    mainCannon.ToggleTriggerBox(true);
            VV02.SetMasterVector(currAnswer);

        }
        else
            currTarget.CreateAttemptCSV(currMatrix, new int[2] { 0, 0 });

        currTarget.puzzleData.steps++;
        //TODO
        //connect this to the cannonRotate class
        //connect this to the WindowUI
        //stretch connect this to the VisualVector
    }

    public void CalculateAnswer()
    {
        if (vectChosen && matrixChosen)
        {
            currAnswer = Vector3.zero;
            currAnswer.x = currVector[0] * currMatrix[0][0] + currVector[1] * currMatrix[1][0];
            currAnswer.z = currVector[0] * currMatrix[0][1] + currVector[1] * currMatrix[1][1];
        }
    }

    public void SwitchCamera()
    {
        topViewOn = !topViewOn;
        P02W.OnCameraSwitch(topViewOn);
        if (topViewOn)
        {
            ToggleOverlay(P02W.showOverlay);

            topCamera.gameObject.SetActive(true);
            mainCamera.GetComponent<Camera>().enabled = false;
            topCamera.transform.position = new Vector3(0, 50, 55);
            topCamera.GetComponent<Camera>().orthographicSize = 60;
            visVectCamera.GetComponent<Camera>().orthographicSize = 60;
            topCamera.GetComponent<Camera>().enabled = true;
            //GameRoot3D.instance.LockCamera(true);
            GameRoot3D.instance.LockPlayerAndCam(true);
            AxisMain.SetActive(true);

            foreach (PuzzleController02 ship in shipPuzzles)
            {
                ship.ToggleTopText(true);
                ship.ToggleFrontText(false);
                ship.shipLine.gameObject.SetActive(P02W.showTargetLines);
            }
            currTarget.shipLine.material = currTarget.lineMatActive;

            foreach (FaceObject bar in starBars)
            {
                bar.GetComponentInChildren<StarBar>().SetSize(11f,3f);
                bar.LockRotationAxis(FaceObject.LockAxis.Y);
                bar.SetTarget(topCamera.transform);
            }
            //           currTarget.vectTextTop.GetComponent<RubberizeAnim>().active = true;

            //ADD Feature for Turning on the Line Renderers

            topCameraActive = true;
        }
        else
        {

            ToggleOverlay(false);
            topCamera.gameObject.SetActive(false);
            mainCamera.GetComponent<Camera>().enabled = true;
            topCamera.GetComponent<Camera>().enabled = false;
           // GameRoot3D.instance.LockCamera(false);
            GameRoot3D.instance.LockPlayerAndCam(false);

            AxisMain.SetActive(false);

            foreach (PuzzleController02 ship in shipPuzzles)
            {
                ship.ToggleFrontText(false);
                ship.ToggleTopText(false);
                ship.shipLine.material = ship.lineMatDef;
                ship.shipLine.gameObject.SetActive(false);
            }
            currTarget.vectText.gameObject.SetActive(true);

            foreach (FaceObject bar in starBars)
            {
                bar.GetComponentInChildren<StarBar>().SetSize(6f, 1.0f);
                bar.LockRotationAxis(FaceObject.LockAxis.None);
                bar.SetTarget(player.transform);
            }

            //ADD Feature for Turning off the Line Renderers

            topCameraActive = false;
        }
    }

    private void ResetSelections()
    {
        vectChosen = false;
        currVector = new int[3] { 0, 0, 0 };
        P02W.SetVectorSelected(currVector);

        //matrixChosen = false;
        //currMatrix = new int[2][] { new int[2] { 0, 0 }, new int[2] { 0, 0 } };
        //P02W.SetMatrixSelected(currMatrix);

        lastAnswer = new int[3] { (int)currAnswer.x, 0, (int)currAnswer.z };

        mainCannon.mainBarrel.SetActive(false);
        mainCannon.ToggleTriggerBox(false);

    }

    private void AltGenerateShipLocations()
    {
        shipPuzzles = new List<PuzzleController02>();

        for (int i = 0; i < numberOfShips; i++)
        {
            GameObject temp = Instantiate(shipMaster);
            temp.SetActive(true);
            temp.GetComponent<PuzzleController02>().SetGameController(this);
            temp.GetComponent<PuzzleController02>().SetID(i + 1);
            temp.GetComponent<PuzzleController02>().ToggleFrontText(false);
            temp.GetComponent<PuzzleController02>().ToggleTopText(false);
            temp.GetComponent<PuzzleController02>().matrixList = matrixList;
            temp.GetComponent<PuzzleController02>().ansMatrix = matrixList[i%matrixList.Count];
            //temp.GetComponent<PuzzleController02>().AltCreateMatrixList();
            temp.GetComponent<PuzzleController02>().AltCreateVectorsList();
            //temp.GetComponent<PuzzleController02>().answerShuffle();
            temp.GetComponent<PuzzleController02>().SetShipDirection();
            
            shipPuzzles.Add(temp.GetComponent<PuzzleController02>());
        }

        //shipMaster.SetActive(false);
        P02W.SetCompleteNumber(numberOfShips - shipPuzzles.Count, numberOfShips);
        shipMaster.SetActive(false);
    }

    private void GenerateShipLocations() 
    {
        int randX, randY, randZ;

        //generate ships and locations
        for (int i = 0; i < numberOfShips; i++)
        {
            GameObject temp = Instantiate(shipMaster);

            //Generate initial location
            if (Difficulty != 3)
            {
                //for the lower diff, I want all the placments to be in multiples of 5
                randX = 5 * Random.Range(-15, 16);
                randY = 0;
                randZ = 5 * Random.Range(3, 21);

            }
            else
            {                
                randX = Random.Range(-75, 76);
                randY = 0;
                randZ = Random.Range(Mathf.RoundToInt(Mathf.Abs(randX) * 0.2f) + 15, 100);
            }

            temp.GetComponent<PuzzleController02>().SetPosition(new Vector3(randX, randY, randZ));
            temp.GetComponent<PuzzleController02>().puzzleData.obsID = i+1;

            shipPuzzles.Add(temp.GetComponent<PuzzleController02>());

            float distApart;
            //Check that this location is not too close to previous ships
            for (int j = 0; j < shipPuzzles.Count; j++)
            {
                distApart = (shipPuzzles[i].transform.position - shipPuzzles[j].transform.position).magnitude;
                if (shipPuzzles[i].gameObject != shipPuzzles[j].gameObject && distApart < minDistApart)
                {                    
                    shipPuzzles.RemoveAt(i);
                    Destroy(temp);
                    i--;
                    break;
                }
            }
        }

        foreach(PuzzleController02 ship in shipPuzzles)
        {
            ship.ToggleFrontText(false);
            ship.ToggleTopText(false);
//            ship.CreateVectorsList();
//            ship.CreateMatrixList();
            ship.AltCreateMatrixList();
            ship.AltCreateVectorsList();
            ship.answerShuffle();
            ship.SetShipDirection();
        }

        P02W.SetCompleteNumber(numberOfShips - shipPuzzles.Count, numberOfShips);
        //generate airships
        //for (int i = 0; i < numberOfAirships - 1; i++)
        //{
        //    //Generate initial location
        //    randX = Random.Range(-75, 76);
        //    randY = Random.Range(20, 65);
        //    randZ = Random.Range(Mathf.RoundToInt(Mathf.Abs(randX) * 0.2f) + 10, 100);
        //    airshipPuzzles[i].shipParent.transform.position = new Vector3(randX, randY, randZ);

        //    //Check that this location is not too close to previous ships
        //    for (int j = 0; j < numberOfAirships - 1; j++)
        //    {
        //        if (airshipPuzzles[j].transform.position == Vector3.zero)
        //            break;

        //        if ((airshipPuzzles[i].transform.position - airshipPuzzles[j].transform.position).magnitude < minDistApart)
        //        {
        //            i--;
        //            break;
        //        }
        //    }
        //}
    }

    public List<PuzzleController02> GetShipList()
    {
        return shipPuzzles;
    }

    public PuzzleController02 GetCurrTarget()
    {
        return currTarget;
    }

    public override void SetSplash()
    {
        foreach(PuzzleController02.obsData obs in data)
        {
            averageScore += obs.score;
        }
        averageScore /= data.Count;

        base.SetSplash();

            P02W.panelProgress.gameObject.SetActive(false);

        switch (Difficulty)
        {
            case 1:
                GameRoot3D.instance.GCR?.completeSplash?.SetSplash(
                    GameRoot3D.player.records.p2_1_time,
                    tot_puzzleTime,
                    GameRoot3D.player.records.p2_1_score,
                    averageScore);
                break;

            case 2:
                GameRoot3D.instance.GCR?.completeSplash?.SetSplash(
                    GameRoot3D.player.records.p2_2_time,
                    tot_puzzleTime,
                    GameRoot3D.player.records.p2_2_score,
                    averageScore);
                break;

            case 3:
                GameRoot3D.instance.GCR?.completeSplash?.SetSplash(
                    GameRoot3D.player.records.p2_3_time,
                    tot_puzzleTime,
                    GameRoot3D.player.records.p2_3_score,
                    averageScore);
                break;

        }
    }


    public void SaveLocalPuzzleData(PuzzleController02 puzzle)
    {
        puzzle.puzzleData.i_matrix = currMatrix;
        puzzle.puzzleData.i_vect = currVector;
        if (puzzle.puzzleData.i_vect != puzzle.puzzleData.e_vect || puzzle.puzzleData.i_matrix != puzzle.puzzleData.e_matrix)
            puzzle.puzzleData.userUniqueSolution = true;
        else
            puzzle.puzzleData.userUniqueSolution = false;

        switch (Difficulty)
        {
            case 1:
                GameRoot3D.player.records.p2_1_numMove += puzzle.attemptNum;

                switch (puzzle.puzzleData.obsID)
                {
                    case 1:
                        GameRoot3D.player.p2_1.obs1_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs1_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs1_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p2_1.obs2_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs2_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs2_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p2_1.obs3_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs3_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs3_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p2_1.obs4_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs4_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs4_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p2_1.obs5_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs5_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs5_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p2_1.obs6_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs6_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs6_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p2_1.obs7_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs7_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs7_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p2_1.obs8_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs8_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs8_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot3D.player.p2_1.obs9_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs9_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs9_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs9 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs9_attempts = puzzle.attemptsList;
                        break;
                    case 10:
                        GameRoot3D.player.p2_1.obs10_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_1.obs10_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_1.obs10_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs10_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_1.obs10 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs10_attempts = puzzle.attemptsList;
                        break;                       
                }
                break;


            case 2:
                GameRoot3D.player.records.p2_2_numMove += puzzle.attemptNum;

                switch (puzzle.puzzleData.obsID)
                {
                    case 1:
                        GameRoot3D.player.p2_2.obs1_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs1_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs1_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p2_2.obs2_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs2_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs2_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p2_2.obs3_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs3_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs3_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p2_2.obs4_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs4_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs4_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p2_2.obs5_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs5_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs5_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p2_2.obs6_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs6_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs6_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p2_2.obs7_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs7_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs7_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p2_2.obs8_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs8_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs8_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot3D.player.p2_2.obs9_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs9_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs9_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs9 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs9_attempts = puzzle.attemptsList;
                        break;
                    case 10:
                        GameRoot3D.player.p2_2.obs10_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_2.obs10_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_2.obs10_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs10_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_2.obs10 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs10_attempts = puzzle.attemptsList;
                        break;
                }
                break;


            case 3:
                GameRoot3D.player.records.p2_3_numMove += puzzle.attemptNum;

                switch (puzzle.puzzleData.obsID)
                {
                    case 1:
                        GameRoot3D.player.p2_3.obs1_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs1_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs1_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p2_3.obs2_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs2_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs2_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p2_3.obs3_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs3_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs3_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p2_3.obs4_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs4_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs4_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p2_3.obs5_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs5_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs5_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p2_3.obs6_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs6_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs6_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p2_3.obs7_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs7_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs7_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p2_3.obs8_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs8_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs8_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot3D.player.p2_3.obs9_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs9_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs9_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs9 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs9_attempts = puzzle.attemptsList;
                        break;
                    case 10:
                        GameRoot3D.player.p2_3.obs10_Ideal = puzzle.puzzleData.Ideal;
                        GameRoot3D.player.p2_3.obs10_steps = puzzle.puzzleData.steps;
                        GameRoot3D.player.p2_3.obs10_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs10_score = puzzle.puzzleData.score;
                        GameRoot3D.player.p2_3.obs10 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs10_attempts = puzzle.attemptsList;
                        break;
                }
                break;
        }
    }


    public void RecordData()
    {
        switch (levelNumber)
        {
            case 1:
                foreach (PuzzleController02.obsData temp in data)
                {
                    GameRoot3D.player.p2_1.tot_Ideal += temp.Ideal;
                    GameRoot3D.player.p2_1.tot_steps += temp.steps;
                    GameRoot3D.player.p2_1.avg_score += temp.score;
                }
                GameRoot3D.player.p2_1.tot_score = GameRoot3D.player.p2_1.tot_Ideal / GameRoot3D.player.p2_1.tot_steps;
                GameRoot3D.player.p2_1.avg_score /= data.Count;

                if (GameRoot3D.player.records.p2_1_time == 0.0f)
                    GameRoot3D.player.records.p2_1_time = tot_puzzleTime;
                else if (tot_puzzleTime < GameRoot3D.player.records.p2_1_time)
                    GameRoot3D.player.records.p2_1_time = tot_puzzleTime;

                if (GameRoot3D.player.records.p2_1_score == 0.0f)
                    GameRoot3D.player.records.p2_1_score = GameRoot3D.player.p2_1.avg_score;
                else if (GameRoot3D.player.p2_1.avg_score < GameRoot3D.player.records.p2_1_score)
                    GameRoot3D.player.records.p2_1_score = GameRoot3D.player.p2_1.avg_score;

                if (GameRoot3D.player.records.p2_1_numMove == 0.0f)
                    GameRoot3D.player.records.p2_1_numMove = GameRoot3D.player.p2_1.tot_steps;
                else if (GameRoot3D.player.p2_1.tot_steps < GameRoot3D.player.records.p2_1_numMove)
                    GameRoot3D.player.records.p2_1_numMove = GameRoot3D.player.p2_1.tot_steps;

                GameRoot3D.player.p2_1.attemptDate = System.DateTime.Now.ToString();
                GameRoot3D.player.records.p2_1_attempts++;

                //Attempt Username Construction
                GameRoot3D.player.p2_1.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p2_1.attemptNum = GameRoot3D.player.records.p2_1_attempts;
                GameRoot3D.player.p2_1.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p2_1.attemptNum.ToString();
                GameRoot3D.player.p2_1.sessionID = GameRoot3D.player.users.sessionID;
                break;

            case 2:
                foreach (PuzzleController02.obsData temp in data)
                {
                    GameRoot3D.player.p2_2.tot_Ideal += temp.Ideal;
                    GameRoot3D.player.p2_2.tot_steps += temp.steps;
                    GameRoot3D.player.p2_2.avg_score += temp.score;
                }
                GameRoot3D.player.p2_2.tot_score = GameRoot3D.player.p2_2.tot_Ideal / GameRoot3D.player.p2_2.tot_steps;
                GameRoot3D.player.p2_2.avg_score /= data.Count;

                if (GameRoot3D.player.records.p2_2_time == 0.0f)
                    GameRoot3D.player.records.p2_2_time = tot_puzzleTime;
                else if (tot_puzzleTime < GameRoot3D.player.records.p2_2_time)
                    GameRoot3D.player.records.p2_2_time = tot_puzzleTime;

                if (GameRoot3D.player.records.p2_2_score == 0.0f)
                    GameRoot3D.player.records.p2_2_score = GameRoot3D.player.p2_2.avg_score;
                else if (GameRoot3D.player.p2_2.avg_score < GameRoot3D.player.records.p2_2_score)
                    GameRoot3D.player.records.p2_2_score = GameRoot3D.player.p2_2.avg_score;

                if (GameRoot3D.player.records.p2_2_numMove == 0.0f)
                    GameRoot3D.player.records.p2_2_numMove = GameRoot3D.player.p2_2.tot_steps;
                else if (GameRoot3D.player.p2_2.tot_steps < GameRoot3D.player.records.p2_2_numMove)
                    GameRoot3D.player.records.p2_2_numMove = GameRoot3D.player.p2_2.tot_steps;

                GameRoot3D.player.p2_2.attemptDate = System.DateTime.Now.ToString();
                GameRoot3D.player.records.p2_2_attempts++;

                //Attempt Username Construction
                GameRoot3D.player.p2_2.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p2_2.attemptNum = GameRoot3D.player.records.p2_2_attempts;
                GameRoot3D.player.p2_2.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p2_2.attemptNum.ToString();
                GameRoot3D.player.p2_2.sessionID = GameRoot3D.player.users.sessionID;
                break;

            case 3:
                foreach (PuzzleController02.obsData temp in data)
                {
                    GameRoot3D.player.p2_3.tot_Ideal += temp.Ideal;
                    GameRoot3D.player.p2_3.tot_steps += temp.steps;
                    GameRoot3D.player.p2_3.avg_score += temp.score;
                }
                GameRoot3D.player.p2_3.tot_score = GameRoot3D.player.p2_3.tot_Ideal / GameRoot3D.player.p2_3.tot_steps;
                GameRoot3D.player.p2_3.avg_score /= data.Count;

                if (GameRoot3D.player.records.p2_3_time == 0.0f)
                    GameRoot3D.player.records.p2_3_time = tot_puzzleTime;
                else if (tot_puzzleTime < GameRoot3D.player.records.p2_3_time)
                    GameRoot3D.player.records.p2_3_time = tot_puzzleTime;

                if (GameRoot3D.player.records.p2_3_score == 0.0f)
                    GameRoot3D.player.records.p2_3_score = GameRoot3D.player.p2_3.avg_score;
                else if (GameRoot3D.player.p2_3.avg_score < GameRoot3D.player.records.p2_3_score)
                    GameRoot3D.player.records.p2_3_score = GameRoot3D.player.p2_3.avg_score;

                if (GameRoot3D.player.records.p2_3_numMove == 0.0f)
                    GameRoot3D.player.records.p2_3_numMove = GameRoot3D.player.p2_3.tot_steps;
                else if (GameRoot3D.player.p2_3.tot_steps < GameRoot3D.player.records.p2_3_numMove)
                    GameRoot3D.player.records.p2_3_numMove = GameRoot3D.player.p2_3.tot_steps;

                GameRoot3D.player.p2_3.attemptDate = System.DateTime.Now.ToString();
                GameRoot3D.player.records.p2_3_attempts++;

                //Attempt Username Construction
                GameRoot3D.player.p2_3.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p2_3.attemptNum = GameRoot3D.player.records.p2_3_attempts;
                GameRoot3D.player.p2_3.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p2_3.attemptNum.ToString();
                GameRoot3D.player.p2_3.sessionID = GameRoot3D.player.users.sessionID;
                break;
        }

        GameRoot3D.player.records.username = GameRoot3D.player.users.username;
        GameRoot3D.player.records.sessionID = GameRoot3D.player.users.sessionID;

        GameRoot3D.player.records.p2_totAttempts = GameRoot3D.player.records.p2_1_attempts + GameRoot3D.player.records.p2_2_attempts + GameRoot3D.player.records.p2_3_attempts;
        GameRoot3D.player.records.p2_totTime += tot_puzzleTime;

        attempt.Finish(AttemptLog.Completed);
    }
}
