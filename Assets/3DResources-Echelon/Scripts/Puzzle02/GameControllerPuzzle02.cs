using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("--------Cannon Parts---------")]
    public List<cannonController> cannons;
    public cannonController currCannon;
    public mainCannonController mainCannon;
    public Material currCannonMat;

    [Header("--------Cannonball Parts---------")]
    public List<cannonballController> balls;
    public cannonballController currBall;

    [Header("--------Puzzle Parts---------")]
    public List<int[][]> matrixList; //a generate list of 4 possible answer matricies
    public int rValue = 10;

    public int[] currVector;
    public bool vectChosen = false;
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

    //Dialogue systems
    private static bool showP02_00 = true;

    //Boat selection feature

    private Vector3 previousPosition;

    // Vector cannons
    public Material currentCannonMaterial;

    //stage control
    public int stageNumber;
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
        endPortal.SetActive(false);

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

        //Show Dialogue once
        if (showP02_00)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation(2, 1, 0));
            showP02_00 = false;
        }

        //this.diffScalar.transform.localScale *= (float)Difficulty / 3.0f;

        //GenerateShipLocations();
        CreateMatrices();
        AltGenerateShipLocations();
        endPortal.SetActive(false);


        int random = Random.Range(0, shipPuzzles.Count);
        currTarget = shipPuzzles[random];
        PickTarget(currTarget);

    }

    private void CreateMatrices()
    {
        matrixList = new List<int[][]>();

        for (int i = 0; i < 4; i++)
        {
            float[] pos = new float[2];
            int[][] matrix = new int[2][] { new int[2], new int[2] };

            float thetaMax = Mathf.Deg2Rad * 120.0f;
            float thetaMin = Mathf.Deg2Rad * 60.0f;
            float theta;

            Vector2 x1 = new Vector2(1, 0);
            theta = Random.Range(0.0f, 2 * Mathf.PI);

            float s = Mathf.Cos(theta) * x1.x - Mathf.Sin(theta) * x1.y;
            float t = Mathf.Sin(theta) * x1.x + Mathf.Cos(theta) * x1.y;
            int randScal = Random.Range(Mathf.RoundToInt((float)rValue * 0.66f), rValue);
            Vector2 k = new Vector2(s, t) * randScal;
            k.x = Mathf.RoundToInt(k.x);
            k.y = Mathf.RoundToInt(k.y);

            x1 = k;

            //Rotating the second Vector
            theta = Random.Range(thetaMin, thetaMax);
            if (Random.Range(0.0f, 1.0f) < 0.5f)
                theta = (theta * -1);

            s = Mathf.Cos(theta) * x1.normalized.x - Mathf.Sin(theta) * x1.normalized.y;
            t = Mathf.Sin(theta) * x1.normalized.x + Mathf.Cos(theta) * x1.normalized.y;

            k = new Vector2(s, t);

            //scoring phase
            float bestScore = 99;
            float bestScal = 0;
            float magV1 = x1.magnitude;
            //We're only going to look at x2 that at least half the mag x1 but not larger than 1.5 the mag of x1 
            for (int j = Mathf.RoundToInt(magV1 * 0.5f); j < Mathf.RoundToInt(magV1 * 1.5f); j++)
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
        if (!DialogueManager.isInDialogue && !GameRoot3D.isPause)
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



        if (!DialogueManager.isInDialogue && !GameRoot3D.isPause)
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
                
                mainCannon.FireCannon(currAnswer, currAnswer == currTarget.shipParent.transform.position);
                audioService.PlayFXAudio(Constants.audioP02CannonFire);
                ResetSelections();
                //              FireCannon();
                Debug.Log("cannon fired");
            }

        }

    }

    public void ToggleOverlay(bool val)
    {
        //placeholder for Puzzle2UI class to access the visual vector class
        VV02.ToggleLattice(val);
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
        currTarget.ToggleFrontText(false);

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
        target.vectText.GetComponent<FaceObject>().SetTarget(player);
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
        VV02.CreateLattice(new Vector3(currMatrix[0][0], 0, currMatrix[0][1]), new Vector3(currMatrix[1][0], 0, currMatrix[1][1]));
        P02W.showOverlay = true;
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
            VV02.ToggleLattice(P02W.showOverlay);

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
            //           currTarget.vectTextTop.GetComponent<RubberizeAnim>().active = true;

            //ADD Feature for Turning on the Line Renderers

            topCameraActive = true;
        }
        else
        {
            VV02.ToggleLattice(false);
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


            //ADD Feature for Turning off the Line Renderers

            topCameraActive = false;
        }
    }

    private void ResetSelections()
    {
        vectChosen = false;
        currVector = new int[3] { 0, 0, 0 };
        P02W.SetVectorSelected(currVector);

        matrixChosen = false;
        currMatrix = new int[2][] { new int[2] { 0, 0 }, new int[2] { 0, 0 } };
        P02W.SetMatrixSelected(currMatrix);

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
                        GameRoot3D.player.p2_1.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p2_1.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p2_1.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p2_1.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p2_1.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p2_1.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p2_1.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p2_1.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot3D.player.p2_1.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_1.obs9 = puzzle.puzzleData;
                        GameRoot3D.player.p2_1.obs9_attempts = puzzle.attemptsList;
                        break;
                    case 10:
                        GameRoot3D.player.p2_1.obs10_time = puzzle.puzzleData.obsTime;
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
                        GameRoot3D.player.p2_2.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p2_2.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p2_2.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p2_2.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p2_2.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p2_2.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p2_2.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p2_2.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot3D.player.p2_2.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_2.obs9 = puzzle.puzzleData;
                        GameRoot3D.player.p2_2.obs9_attempts = puzzle.attemptsList;
                        break;
                    case 10:
                        GameRoot3D.player.p2_2.obs10_time = puzzle.puzzleData.obsTime;
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
                        GameRoot3D.player.p2_3.obs1_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs1 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs1_attempts = puzzle.attemptsList;
                        break;
                    case 2:
                        GameRoot3D.player.p2_3.obs2_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs2 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs2_attempts = puzzle.attemptsList;
                        break;
                    case 3:
                        GameRoot3D.player.p2_3.obs3_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs3 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs3_attempts = puzzle.attemptsList;
                        break;
                    case 4:
                        GameRoot3D.player.p2_3.obs4_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs4 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs4_attempts = puzzle.attemptsList;
                        break;
                    case 5:
                        GameRoot3D.player.p2_3.obs5_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs5 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs5_attempts = puzzle.attemptsList;
                        break;
                    case 6:
                        GameRoot3D.player.p2_3.obs6_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs6 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs6_attempts = puzzle.attemptsList;
                        break;
                    case 7:
                        GameRoot3D.player.p2_3.obs7_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs7 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs7_attempts = puzzle.attemptsList;
                        break;
                    case 8:
                        GameRoot3D.player.p2_3.obs8_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs8 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs8_attempts = puzzle.attemptsList;
                        break;
                    case 9:
                        GameRoot3D.player.p2_3.obs9_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs9 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs9_attempts = puzzle.attemptsList;
                        break;
                    case 10:
                        GameRoot3D.player.p2_3.obs10_time = puzzle.puzzleData.obsTime;
                        GameRoot3D.player.p2_3.obs10 = puzzle.puzzleData;
                        GameRoot3D.player.p2_3.obs10_attempts = puzzle.attemptsList;
                        break;
                }
                break;
        }
    }


    public void RecordData()
    {
        switch (stageNumber)
        {
            case 1:
                if (GameRoot3D.player.records.p2_1_time == 0.0f)
                {

                    GameRoot3D.player.records.p2_1_time = tot_puzzleTime;
                    //GameRoot.player.records.p2_1_numMove = totMoves;
                    //GameRoot.player.records.p2_1_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot3D.player.records.p2_1_time)
                {

                    GameRoot3D.player.records.p2_1_time = tot_puzzleTime;
                }

                GameRoot3D.player.p2_1.attemptDate = System.DateTime.Now.ToString();
                GameRoot3D.player.records.p2_1_attempts++;

                //Attempt Username Construction
                GameRoot3D.player.p2_1.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p2_1.attemptNum = GameRoot3D.player.records.p2_1_attempts;
                GameRoot3D.player.p2_1.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p2_1.attemptNum.ToString();
                GameRoot3D.player.p2_1.sessionID = GameRoot3D.player.users.sessionID;
                break;

            case 2:
                if (GameRoot3D.player.records.p2_2_time == 0.0f)
                {

                    GameRoot3D.player.records.p2_2_time = tot_puzzleTime;
                }
                else if (tot_puzzleTime < GameRoot3D.player.records.p2_2_time)
                {

                    GameRoot3D.player.records.p2_2_time = tot_puzzleTime;
                }

                GameRoot3D.player.p2_2.attemptDate = System.DateTime.Now.ToString();
                GameRoot3D.player.records.p2_2_attempts++;

                //Attempt Username Construction
                GameRoot3D.player.p2_2.username = GameRoot3D.player.users.username;
                GameRoot3D.player.p2_2.attemptNum = GameRoot3D.player.records.p2_2_attempts;
                GameRoot3D.player.p2_2.usernameAttempt = GameRoot3D.player.users.username + "." + GameRoot3D.player.p2_2.attemptNum.ToString();
                GameRoot3D.player.p2_2.sessionID = GameRoot3D.player.users.sessionID;
                break;

            case 3:
                if (GameRoot3D.player.records.p2_3_time == 0.0f)
                {

                    GameRoot3D.player.records.p2_3_time = tot_puzzleTime;
                }
                else if (tot_puzzleTime < GameRoot3D.player.records.p2_3_time)
                {

                    GameRoot3D.player.records.p2_3_time = tot_puzzleTime;
                }

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
    }
}
