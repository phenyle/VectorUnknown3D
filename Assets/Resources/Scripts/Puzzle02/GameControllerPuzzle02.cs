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

    [Header("--------Cannonball Parts---------")]
    public List<cannonballController> balls;
    public cannonballController currBall;

    [Header("--------Puzzle Parts---------")]
    public int[] currVector;
    public bool vectChosen = false;
    public int[][] currMatrix;
    public bool matrixChosen = false;
    public int[] currAnswer;


    public GameObject firingCannon;

    [HideInInspector]
    public Puzzle02Window P02W;
    [HideInInspector]
    public DatabasePuzzle02 DBP02;
    public DatabasePuzzle02_02 DBP02_02;

    private GameObject player;
    private GameObject mainCamera;

    private Vector3 startPosition;

    //camera stuff
    public GameObject topCamera;
    public GameObject MainCamera;
    public bool topCameraActive = false;

    //trigger bools
    public bool isCannonTrigger = false;
    public bool isCannonballTrigger = false;
    public bool isMainCannonTrigger = false;
    public bool ballIsFlying = false;
    public bool isCannonSelected = false;
    public bool isBallSelected = false;

    //cannonball 
    public Transform cannonball;
    private Transform tempCannonball;
    public Vector3 targetPosition;

    //main cannonbarrel
    public GameObject cannonBarrel;
    public GameObject maincannonText;
    public GameObject maincannonBrackets;

    //art stuff
    public Material trailMaterial;
    public Material trailMaterialCorrect;
    public Material trailMaterialWrong;

    //UI
    public GameObject[] TopViewText;
    public GameObject[] TopViewVectors;
    public GameObject[] normalText;
    public GameObject[] cannonBallsText;
    public bool topViewOn = false;
    public int ActiveBoat = 0;
    public Text Matrix;
    public Text Vector;
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
    public Transform endportal;

    // Vector cannons
    public Material currentCannonMaterial;

    // cannonCamera controls
    public GameObject CannonCamera;
    public bool fireCannon = false;
    public bool cameraFollow = false;
    private bool cameraSwitch = false;
    public float cameraCannonSpeed = 0.5f;
    private Vector3 cannonCameraOriginalPosition = new Vector3(0, 1, 0.0f);
    private Vector3 targetPositionOffset = new Vector3(0, 4, 0);

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


    }

    public override void InitGameController(Puzzle02Window P02W)
    {

        Debug.Log("Init GameController Puzzle02");
        base.InitGameController();

        Debug.Log("Connect Puzzle02 Window");
        this.P02W = P02W;

        Debug.Log("Connect Database of Puzzle02");
        if (stageNumber == 1)
        {
            DBP02 = GetComponent<DatabasePuzzle02>();
            Debug.Log("Call Database of Puzzle02 to connect");
            DBP02.InitDatabase();
        }

        else if (stageNumber == 2)
        {
            DBP02_02 = GetComponent<DatabasePuzzle02_02>();
            Debug.Log("Call Database of Puzzle02 to connect");
            DBP02_02.InitDatabase();
        }

        //GR = GameObject.FindGameObjectWithTag("").GetComponent<GameRoot>();

        player = GameObject.FindGameObjectWithTag("Player");
        GameRoot.instance.SetScenePlayer(player);
        GameRoot.instance.FindPlayerAndCamera();
        startPosition = player.transform.position;

        //Show Dialogue once
        if (showP02_00)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle02_00"));
            showP02_00 = false;
        }

        // cannonBarrel.gameObject.SetActive(false);

        trailMaterial = trailMaterialWrong;

        //foreach (GameObject text in TopViewText) { text.gameObject.SetActive(false); }
        //foreach (GameObject text in TopViewVectors) { text.gameObject.SetActive(false); }
        //foreach (GameObject text in normalText) { text.gameObject.SetActive(false); }
        //normalText[0].gameObject.SetActive(true);

        ////stop particle effects from playing at start
        //cannonBlast.GetComponent<ParticleSystem>().playOnAwake = false;

        //maincannonText.gameObject.GetComponent<Text>().text = "0\n0";

        //Matrix = P02W.Matrix;
        //Vector = P02W.Vector;
        //shipImages = P02W.shipImages;

        //if (stageNumber == 2 || stageNumber == 1)
        //{
        //    Vector.text = "0" + "\n" + "0";
        //    Matrix.text = "0" + " " + "0" + "\n" +
        //        "0" + " " + "0";
        //    foreach (Image ship in shipImages)
        //    {
        //        ship.sprite = activeBoat;
        //    }
        //}

        SetActive(endportal, false);

        GenerateShipLocations();

        PickTarget();

    }


    public void PickTarget()
    {
        int random = Random.Range(0, shipPuzzles.Count);

        currTarget = shipPuzzles[random];

        //check that there isn't some other target in the way, and if there is
        //choose that target instead;
        Ray check = new Ray(new Vector3(0, 0.5f, 3.5f), currTarget.transform.position);
        RaycastHit hitcheck;

        if(Physics.Raycast(check, out hitcheck))
        {
            currTarget = hitcheck.transform.GetComponentInParent<PuzzleController02>();
        }    

        P02W.SetTargetShip(currTarget.shipPos);
        currTarget.ToggleFrontText(true);
        currTarget.vectText.GetComponent<RubberizeAnim>().ToggleActivate(true);
        currTarget.vectText.GetComponent<FaceObject>().SetTarget(player);
        currTarget.vectText.GetComponent<FaceObject>().enabled = true;
        //set cannon ansVectors and balls ansMatricies
        for (int i = 0; i < 4; i++)
        {
            cannons[i].SetCannon(currTarget.vectorList[i]);
            balls[i].SetCannonBalls(currTarget.matrixList[i]);
        }
    }

    public void SelectCannon(cannonController cannon)
    {
        currCannon = cannon;
        currVector = currCannon.vector;
        vectChosen = true;
        P02W.SetVectorSelected(currVector);
        CalculateAnswer();
        if(matrixChosen)
            mainCannon.AnimateCannon();
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
        P02W.SetMatrixSelected(currMatrix);
        CalculateAnswer();
        if(vectChosen)
            mainCannon.AnimateCannon();
        //TODO
        //connect this to the cannonRotate class
        //connect this to the WindowUI
        //stretch connect this to the VisualVector
    }

    public void CalculateAnswer()
    {
        if(vectChosen && matrixChosen)
        {
            currAnswer = new int[3];
            currAnswer[0] = currVector[0] * currMatrix[0][0] + currVector[2] * currMatrix[1][0];
            currAnswer[2] = currVector[0] * currMatrix[0][1] + currVector[2] * currMatrix[1][1];
        }
    }



    void Update()
    {
        // reset position tip
        // if (player.transform.position.x - previousPosition.x < 0.01 && player.transform.position.y - previousPosition.y < 0.01 && player.transform.position.y - previousPosition.y < 0.01)
        // {
        //     timer += (int)Time.deltaTime + 1;
        //     if (timer == 1200)
        //     {
        //         GameRoot.ShowTips("Press 'R' to reset position, 'T' to dismiss", true, false);
        //     }
        // }
        // else
        // {
        //     timer = 0;
        // }

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

        if (ActiveBoat == 6 && !portalActive)
        {
            if (stageNumber == 1)
            {
                GameRoot.ShowTips("You Completed stage 1!", true, false);
            }
            else
            {
                GameRoot.ShowTips("You Completed the level!", true, false);
                GameRoot.instance.puzzleCompleted[1] = true;
            }
            SetActive(endportal, true);
            portalActive = true;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchCamera();
            topViewOn = !topViewOn;

            if (topViewOn)
            {
                TopViewText[ActiveBoat].gameObject.SetActive(true);
                TopViewText[6].gameObject.SetActive(true);
                if (ActiveBoat < 6)
                    normalText[ActiveBoat].gameObject.SetActive(false);
                for (int i = ActiveBoat - 1; i >= 0; i--)
                {
                    TopViewVectors[i].gameObject.SetActive(true);
                }
            }
            else
            {
                TopViewText[ActiveBoat].gameObject.SetActive(false);
                TopViewText[6].gameObject.SetActive(false);
                if (ActiveBoat < 6)
                    normalText[ActiveBoat].gameObject.SetActive(true);
                for (int i = ActiveBoat - 1; i >= 0; i--)
                {
                    TopViewVectors[i].gameObject.SetActive(false);
                }
            }

            Debug.Log("Z was pressed");
        }

        if (isCannonTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // cannonBarrel.SetActive(true);

            selectedVector = currentVector;
            cannonBarrel.GetComponent<Renderer>().material = currentCannonMaterial;
            isCannonSelected = true;
            Debug.Log("Selected Vector " + selectedVector[0] + ", " + selectedVector[1]);
            maincannonText.gameObject.GetComponent<Text>().text = selectedVector[0] + "\n" + selectedVector[1];
            Vector.text = selectedVector[0] + "\n" + selectedVector[1];

            /*
            //---------------------------------New Tips Function--------------------------------------
            if (isCannonSelected && isBallSelected)
                GameRoot.ShowTips("The main gun is ready to fire", true, true);
            else
                GameRoot.ShowTips("Go pick the Cannon Ball or pick another Cannon", true, true);
            //--------------------------------------------------------------------------------------------
            */

            audioService.PlayFXAudio(Constants.audioP02Selection);
            Debug.Log("Audio played");
        }

        if (isCannonballTrigger && Input.GetKeyDown(KeyCode.E))
        {
            firedCannonYet = false;
            selectedTransformMatrix = currentTransformMatrix;
            isBallSelected = true;
            Debug.Log("Selected Matrix " + selectedTransformMatrix[0] + ", " + selectedTransformMatrix[1] + ", " +
                selectedTransformMatrix[2] + ", " + selectedTransformMatrix[3]);
            Matrix.text = selectedTransformMatrix[0] + " " + selectedTransformMatrix[1] + "\n" +
                selectedTransformMatrix[2] + " " + selectedTransformMatrix[3];

            /*
            //---------------------------------New Tips Function--------------------------------------
            if (isCannonSelected && isBallSelected)
                GameRoot.ShowTips("The main gun is ready to fire", true, true);
            else
                GameRoot.ShowTips("Go pick the Cannon or pick another Cannon Ball", true, true);
            //--------------------------------------------------------------------------------------------
            */

            audioService.PlayFXAudio(Constants.audioP02Selection);
            Debug.Log("Audio played");
        }

        if (isMainCannonTrigger && Input.GetKeyDown(KeyCode.E) && !fireCannon)
        {
            if (selectedTransformMatrix != new int[] { 0, 0, 0, 0 } && selectedVector != new int[] { 0, 0 })
            {
                FireCannon();
                Debug.Log("cannon fired");
                isBallSelected = false;
                isCannonSelected = false;
            }
            audioService.PlayFXAudio(Constants.audioP02CannonFire);
        }

        // if (ballIsFlying)
        // {
        //     tempCannonball.transform.position += targetPosition * cameraCannonSpeed;

        //     if (tempCannonball.transform.position.y <= -10)
        //     {
        //         ballIsFlying = false;
        //     }
        // }

        //previousPosition = player.transform.position;

        if (fireCannon)
        {
            // have a camera follow cannon ball shot if answer is correct
            if (cameraFollow)
            {
                if (cameraSwitch)
                {
                    // switch cameras
                    MainCamera.SetActive(false);
                    CannonCamera.SetActive(true);
                    cameraSwitch = false;
                }

                //camera goes half way to ship
                Vector3 target = Vector3.Lerp(cannonCameraOriginalPosition, targetPosition + targetPositionOffset, 0.5f);
                Debug.Log("Camera Target: " + target);

                // move camera
                if (CannonCamera.transform.position != target)
                {
                    Vector3 pos = Vector3.MoveTowards(CannonCamera.transform.position, target, cameraCannonSpeed / 2);
                    CannonCamera.GetComponent<Rigidbody>().MovePosition(pos);
                }
            }
            else
            {
                ballIsFlying = false;
            }

            // move cannonball
            if (tempCannonball.transform.position != targetPosition)
            {
                Vector3 pos = Vector3.MoveTowards(tempCannonball.transform.position, targetPosition, cameraCannonSpeed);
                tempCannonball.GetComponent<Rigidbody>().MovePosition(pos);
            }

        }
    }

    private void SwitchCamera()
    {
        if (topCameraActive)
        {
            MainCamera.gameObject.SetActive(true);
            GameRoot.instance.LockCamera(false);
  //          MainCamera.GetComponent<CameraController>().isLock = false;

            topCamera.gameObject.SetActive(false);

            topCameraActive = false;
        }
        else if (!topCameraActive)
        {
            MainCamera.gameObject.SetActive(false);

            topCamera.gameObject.SetActive(true);

            topCameraActive = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //public void updateCannonBalls()
    //{
    //    cannonballs[0].GetComponent<cannonballController>().UpdateValues(ActiveBoat);
    //    cannonballs[1].GetComponent<cannonballController>().UpdateValues(ActiveBoat);
    //    cannonballs[2].GetComponent<cannonballController>().UpdateValues(ActiveBoat);
    //    cannonballs[3].GetComponent<cannonballController>().UpdateValues(ActiveBoat);
    //}

    public void UpdateUI()
    {
        shipImages[ActiveBoat].sprite = inactiveBoat;
    }

    /*
    public void playhit()
    {
        audioService.PlayFXAudio(Constants.audioP02BallHit);
        Debug.Log("Audio played");
    }
    
    public void playMiss()
    {
        audioService.PlayFXAudio(Constants.audioP02BallMiss);
        Debug.Log("Audio played");
    }
    */

    private void FireCannon()
    {
        targetPosition = new Vector3(0, -5, 0);
        int[] targetVector = { 1, 0 };

        if (stageNumber == 1)
            targetVector = DBP02.calculation(selectedVector, selectedTransformMatrix);
        else if (stageNumber == 2)
            targetVector = DBP02_02.calculation(selectedVector, selectedTransformMatrix);

        targetPosition.x = targetVector[0];
        targetPosition.z = targetVector[1];
        maincannonText.gameObject.GetComponent<Text>().text = targetPosition.x + "\n" + targetPosition.z;
        Debug.Log("Cannon is aiming at :(" + targetPosition.x + ", " + targetPosition.z + ")");
        // cannonBarrel.SetActive(false);
        tempCannonball = Instantiate(cannonball, new Vector3(0, 0, 0.7f), firingCannon.transform.rotation);

        // tempCannonball.GetComponent<TrailRenderer>().material = trailMaterialWrong;
        // maincannonBrackets.GetComponent<Text>().color = Color.red;
        // maincannonText.GetComponent<Text>().color = Color.red;

        bool activeBoatCheck = false;
        if (stageNumber == 1)
            activeBoatCheck = DBP02.tragetMatracies[ActiveBoat, 0] == targetPosition.x && DBP02.tragetMatracies[ActiveBoat, 1] == targetPosition.z;
        else if (stageNumber == 2)
            activeBoatCheck = DBP02_02.tragetMatracies[ActiveBoat, 0] == targetPosition.x && DBP02_02.tragetMatracies[ActiveBoat, 1] == targetPosition.z;

        if (ActiveBoat < 6 && activeBoatCheck)
        {
            tempCannonball.GetComponent<TrailRenderer>().material = trailMaterialCorrect;
            maincannonBrackets.GetComponent<Text>().color = Color.green;
            maincannonText.GetComponent<Text>().color = Color.green;
            Debug.Log("Will Hit!");
            firedCannonYet = true;
            foreach (GameObject cannonBall in cannonBallsText)
            {
                // cannonBall.GetComponent<Text>().color = Color.black;
                cannonBall.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
                cannonBall.gameObject.transform.GetChild(1).GetComponent<Text>().color = Color.black;
            }
            cameraFollow = true;
            cameraSwitch = true;
            CannonCamera.transform.position = cannonCameraOriginalPosition;
            CannonCamera.transform.LookAt(targetPosition, Vector3.up);
        }
        else
        {
            tempCannonball.GetComponent<TrailRenderer>().material = trailMaterialWrong;
            tempCannonball.GetComponent<BoxCollider>().enabled = false;
            maincannonBrackets.GetComponent<Text>().color = Color.red;
            maincannonText.GetComponent<Text>().color = Color.red;
        }

        // for (int i = 0; i < 6; i++)
        // {
        //    // Debug.Log("target is compared to :(" + DBP02.tragetMatracies[i, 0] + ", " + DBP02.tragetMatracies[i, 1] + ")");
        //     if (DBP02.tragetMatracies[i, 0] == targetPosition.x && DBP02.tragetMatracies[i, 1] == targetPosition.z)
        //     {
        //         tempCannonball.GetComponent<TrailRenderer>().material = trailMaterialCorrect;
        //         maincannonBrackets.GetComponent<Text>().color = Color.green;
        //         maincannonText.GetComponent<Text>().color = Color.green;
        //         Debug.Log("Will Hit!");
        //         cameraFollow = true;
        //         CannonCamera.transform.position = cannonCameraOriginalPosition;
        //         CannonCamera.transform.LookAt(targetPosition, Vector3.up);
        //     }
        // }

        ballIsFlying = true;
        fireCannon = true;
        cannonBlast.SetActive(true);
        cannonBlast.GetComponent<ParticleSystem>().Play(true);
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
