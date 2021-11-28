using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerPuzzle02 : GameControllerRoot
{
    [Range(1, 3)]
    public int Difficulty;

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
    [Range(0,10,order = 1)]
    public int numberOfShips;
 //   [Range(0, 10, order = 1)]
 //   public int numberOfAirships;
    private List<PuzzleController02> shipPuzzles;
 //   private List<PuzzleController02> airshipPuzzles;
    public float minDistApart = 5;
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
        GameRoot.instance.FindPlayerAndCamera();

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
        GameRoot.instance.SetScenePlayer(player);
        GameRoot.instance.FindPlayerAndCamera();
        startPosition = player.transform.position;

        //DATABASE INIT STUFF
        attemptData.difficulty = Difficulty;
        attemptData.tot_Time = 0.0f;
        attemptData.numOfShips = numberOfShips;
        attemptData.numOfShots = 0;

        //Show Dialogue once
        if (showP02_00)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle02_00"));
            showP02_00 = false;
        }

        GenerateShipLocations();
        endPortal.SetActive(false);


        int random = Random.Range(0, shipPuzzles.Count);
        currTarget = shipPuzzles[random];
        PickTarget(currTarget);

    }

    void Update()
    {
        if (!DialogueManager.isInDialogue && !GameRoot.isPause)
        {
            attemptData.tot_Time += Time.deltaTime;
            if(currTarget != null)
            {

            }

            int minutes = (int)attemptData.tot_Time / 60;
            int seconds = (int)attemptData.tot_Time - 60 * minutes;
            if (seconds < 10)
                P02W.SetTime(minutes.ToString() + " : 0" + seconds.ToString());
            else
                P02W.SetTime(minutes.ToString() + " : " + seconds.ToString());
        }



        if (!DialogueManager.isInDialogue && !GameRoot.isPause)
            tot_puzzleTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = startPosition;
            GameRoot.ShowTips("", true, false);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GameRoot.ShowTips("", true, false);
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
                mainCannon.FireCannon(currAnswer, currAnswer == currTarget.shipParent.transform.position);
                audioService.PlayFXAudio(Constants.audioP02CannonFire);
                ResetSelections();
                //              FireCannon();
                Debug.Log("cannon fired");
            }

        }

    }


    public void SinkShip()
    {
        currTarget.ToggleFrontText(false);

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

        if(Physics.SphereCast(new Vector3(0, 0.5f, 5.0f), 2.0f, target.transform.position, out hitcheck))
        {
            PuzzleController02 temp = new PuzzleController02();
            if(hitcheck.transform.TryGetComponent(out temp))
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
        currTarget.vectTextTop.GetComponent<RubberizeAnim>().ToggleActivate(false);
        currTarget.vectText.GetComponent<RubberizeAnim>().ToggleActivate(false);
        currTarget.vectText.transform.localScale = new Vector3(1, -1, 1);
        currTarget.vectTextTop.transform.localScale = new Vector3(2, 2, 1);
        currTarget.ToggleFrontText(false);

        ResetSelections();

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
            mainCannon.AnimateCannon();
            mainCannon.cannonFuzeFX.SetActive(true);
        //    mainCannon.ToggleTriggerBox(true);
        }
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
        CalculateAnswer();
        
        if (vectChosen)
        {
            mainCannon.AnimateCannon();
            mainCannon.cannonFuzeFX.SetActive(true);
            //    mainCannon.ToggleTriggerBox(true);

        }
        //TODO
        //connect this to the cannonRotate class
        //connect this to the WindowUI
        //stretch connect this to the VisualVector
    }

    public void CalculateAnswer()
    {
        if(vectChosen && matrixChosen)
        {
            currAnswer = Vector3.zero;
            currAnswer.x = currVector[0] * currMatrix[0][0] + currVector[2] * currMatrix[1][0];
            currAnswer.z = currVector[0] * currMatrix[0][1] + currVector[2] * currMatrix[1][1];
        }
    }




    public void SwitchCamera()
    {
        topViewOn = !topViewOn;

        if (topViewOn)
        {
            topCamera.gameObject.SetActive(true);
            mainCamera.GetComponent<Camera>().enabled = false;
            topCamera.GetComponent<Camera>().enabled = true;
            GameRoot.instance.LockCamera(true);
            AxisMain.SetActive(true);

            foreach(PuzzleController02 ship in shipPuzzles)
            {
                ship.ToggleTopText(true);
                ship.ToggleFrontText(false);
                ship.shipLine.gameObject.SetActive(true);
            }
            currTarget.shipLine.material = currTarget.lineMatActive;
 //           currTarget.vectTextTop.GetComponent<RubberizeAnim>().active = true;

            //ADD Feature for Turning on the Line Renderers

            topCameraActive = true;
        }
        else
        {
            topCamera.gameObject.SetActive(false);
            mainCamera.GetComponent<Camera>().enabled = true;
            topCamera.GetComponent<Camera>().enabled = false;
            GameRoot.instance.LockCamera(false);
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
        currMatrix = new int[2][] { new int[2]{ 0, 0 }, new int[2]{ 0, 0 } };
        P02W.SetMatrixSelected(currMatrix);

        lastAnswer = new int[3] { (int)currAnswer.x, 0, (int)currAnswer.z };

        mainCannon.mainBarrel.SetActive(false);
        mainCannon.ToggleTriggerBox(false);

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
            ship.CreateVectorsList();
            ship.CreateMatrixList();
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

    public PuzzleController02 GetCurrTarget()
    {
        return currTarget;
    }


    public void RecordData()
    {
        switch (stageNumber)
        {
            case 1:
                if (GameRoot.player.records.p2_1_time == 0.0f)
                {

                    GameRoot.player.records.p2_1_time = tot_puzzleTime;
                    //GameRoot.player.records.p2_1_numMove = totMoves;
                    //GameRoot.player.records.p2_1_questions = questionsList;
                }
                else if (tot_puzzleTime < GameRoot.player.records.p2_1_time)
                {

                    GameRoot.player.records.p2_1_time = tot_puzzleTime;
                }

                GameRoot.player.p2_1.attemptDate = System.DateTime.Now.ToString();
                GameRoot.player.records.p2_1_attempts++;

                //Attempt Username Construction
                GameRoot.player.p2_1.username = GameRoot.player.users.username;
                GameRoot.player.p2_1.attemptNum = GameRoot.player.records.p2_1_attempts;
                GameRoot.player.p2_1.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.p2_1.attemptNum.ToString();

                break;

            case 2:
                if (GameRoot.player.records.p2_2_time == 0.0f)
                {

                    GameRoot.player.records.p2_2_time = tot_puzzleTime;
                }
                else if (tot_puzzleTime < GameRoot.player.records.p2_2_time)
                {

                    GameRoot.player.records.p2_2_time = tot_puzzleTime;
                }

                GameRoot.player.p2_2.attemptDate = System.DateTime.Now.ToString();
                GameRoot.player.records.p2_2_attempts++;

                //Attempt Username Construction
                GameRoot.player.p2_2.username = GameRoot.player.users.username;
                GameRoot.player.p2_2.attemptNum = GameRoot.player.records.p2_2_attempts;
                GameRoot.player.p2_2.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.p2_2.attemptNum.ToString();
                break;

            case 3:
                if (GameRoot.player.records.p2_3_time == 0.0f)
                {

                    GameRoot.player.records.p2_3_time = tot_puzzleTime;
                }
                else if (tot_puzzleTime < GameRoot.player.records.p2_3_time)
                {

                    GameRoot.player.records.p2_3_time = tot_puzzleTime;
                }

                GameRoot.player.p2_3.attemptDate = System.DateTime.Now.ToString();
                GameRoot.player.records.p2_3_attempts++;

                //Attempt Username Construction
                GameRoot.player.p2_3.username = GameRoot.player.users.username;
                GameRoot.player.p2_3.attemptNum = GameRoot.player.records.p2_3_attempts;
                GameRoot.player.p2_3.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.p2_3.attemptNum.ToString();
                break;
        }

        GameRoot.player.records.username = GameRoot.player.users.username;
        GameRoot.player.records.p2_totAttempts = GameRoot.player.records.p2_1_attempts + GameRoot.player.records.p2_2_attempts + GameRoot.player.records.p2_3_attempts;
        GameRoot.player.records.p2_totTime += tot_puzzleTime;
    }
}
