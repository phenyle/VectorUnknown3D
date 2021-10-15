using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerPuzzle02 : GameControllerRoot
{
    public int Difficulty;

    //variables 
    public int[] currentTransformMatrix = new int[] { 0, 0, 0, 0 };
    public int[] currentVector = new int[] { 0, 0 };

    private int[] selectedTransformMatrix = new int[] { 0, 0, 0, 0 };
    public int[] selectedVector = new int[] { 0, 0 };


    [Header("--------Ship Puzzles---------")]
    public PuzzleController02 shipPuzzleMaster;
    public PuzzleController02 airshipPuzzleMaster;
    public int numberOfShips;
    public int numberOfAirships;
    private PuzzleController02[] shipPuzzles;
    private List<GameObject> shipList;
    private PuzzleController02[] airshipPuzzles;
    public int minDistApart = 15;

    public GameObject firingCannon;

    [HideInInspector]
    public Puzzle02Window P02W;
    [HideInInspector]
    public DatabasePuzzle02 DBP02;
    public DatabasePuzzle02_02 DBP02_02;
    [HideInInspector]
    public GameRoot GR;
    private GameObject player;

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

    public void Awake()
    {
        shipList = new List<GameObject>();
        IEnumerable<GameObject> targetShips = GameObject.FindGameObjectsWithTag("targetShips");
        shipList.AddRange(targetShips);

        shipPuzzles = new PuzzleController02[numberOfShips];
        airshipPuzzles = new PuzzleController02[numberOfAirships];
        GenerateShipLocations();
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
        startPosition = player.transform.position;

        //Show Dialogue once
        if (showP02_00)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Puzzle02_00"));
            showP02_00 = false;
        }

        // cannonBarrel.gameObject.SetActive(false);

        trailMaterial = trailMaterialWrong;

        foreach (GameObject text in TopViewText) { text.gameObject.SetActive(false); }
        foreach (GameObject text in TopViewVectors) { text.gameObject.SetActive(false); }
        foreach (GameObject text in normalText) { text.gameObject.SetActive(false); }
        normalText[0].gameObject.SetActive(true);

        //stop particle effects from playing at start
        cannonBlast.GetComponent<ParticleSystem>().playOnAwake = false;

        maincannonText.gameObject.GetComponent<Text>().text = "0\n0";

        Matrix = P02W.Matrix;
        Vector = P02W.Vector;
        shipImages = P02W.shipImages;

        if (stageNumber == 2 || stageNumber == 1)
        {
            Vector.text = "0" + "\n" + "0";
            Matrix.text = "0" + " " + "0" + "\n" +
                "0" + " " + "0";
            foreach (Image ship in shipImages)
            {
                ship.sprite = activeBoat;
            }
        }

        SetActive(endportal, false);

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
            MainCamera.GetComponent<CameraController>().isLock = true;

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

        switch (Difficulty)
        {
            case 1 :
                for(int i = 0; i < numberOfShips-1; i++)
                {
                    //Generate initial location
                    randX = Random.Range(-75, 76);
                    randY = 0;
                    randZ =  Random.Range(Mathf.RoundToInt(Mathf.Abs(randX)*0.2f) +10, 100);

                    shipPuzzles[i].shipParent.transform.position = new Vector3(randX, 0, randZ);

                    //Check that this location is not too close to previous ships
                    for(int j = 0; j < numberOfShips - 1; j++)
                    {
                        if (shipPuzzles[j].transform.position == Vector3.zero)
                            break;

                        if((shipPuzzles[i].transform.position - shipPuzzles[j].transform.position).magnitude < minDistApart)
                        {
                            i--;
                            break;
                        }
                    }
                }

                break;

            case 2:
                for (int i = 0; i < numberOfShips - 1; i++)
                {
                    //Generate initial location
                    randX = Random.Range(-75, 76);
                    randY = 0;
                    randZ = Random.Range(Mathf.RoundToInt(Mathf.Abs(randX) * 0.2f) + 10, 100);

                    shipPuzzles[i].shipParent.transform.position = new Vector3(randX, 0, randZ);

                    //Check that this location is not too close to previous ships
                    for (int j = 0; j < numberOfShips - 1; j++)
                    {
                        if (shipPuzzles[j].transform.position == Vector3.zero)
                            break;

                        if ((shipPuzzles[i].transform.position - shipPuzzles[j].transform.position).magnitude < minDistApart)
                        {
                            i--;
                            break;
                        }
                    }
                }


                break;
                
            case 3:
                //generate ships
                for (int i = 0; i < numberOfShips - 1; i++)
                {
                    //Generate initial location
                    randX = Random.Range(-75, 76);
                    randY = 0;
                    randZ = Random.Range(Mathf.RoundToInt(Mathf.Abs(randX) * 0.2f) + 10, 100);
                    shipPuzzles[i].shipParent.transform.position = new Vector3(randX, randY, randZ);

                    //Check that this location is not too close to previous ships
                    for (int j = 0; j < numberOfShips - 1; j++)
                    {
                        if (shipPuzzles[j].transform.position == Vector3.zero)
                            break;

                        if ((shipPuzzles[i].transform.position - shipPuzzles[j].transform.position).magnitude < minDistApart)
                        {
                            i--;
                            break;
                        }
                    }
                }

                //generate airships
                for(int i = 0; i < numberOfAirships - 1; i++)
                {
                    //Generate initial location
                    randX = Random.Range(-75, 76);
                    randY = Random.Range(20, 65);
                    randZ = Random.Range(Mathf.RoundToInt(Mathf.Abs(randX) * 0.2f) + 10, 100);
                    airshipPuzzles[i].shipParent.transform.position = new Vector3(randX, randY, randZ);

                    //Check that this location is not too close to previous ships
                    for (int j = 0; j < numberOfAirships - 1; j++)
                    {
                        if (airshipPuzzles[j].transform.position == Vector3.zero)
                            break;

                        if ((airshipPuzzles[i].transform.position - airshipPuzzles[j].transform.position).magnitude < minDistApart)
                        {
                            i--;
                            break;
                        }
                    }
                }
                break;
        }
    }



    public void GenerateAnswerMatrixPair(Vector3 shipLoc)
    {
        Vector3[] lvl1Vects = { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0) };
        Vector3[] lvl2Vects = { new Vector3(1, 2, 0), new Vector3(2, -1, 0), new Vector3(-2, 3, 0), new Vector3(1, -3, 0) };

        switch (Difficulty)
        {
            case 1:
                //pick a vector
                int iter = Random.Range(0, lvl1Vects.Length);
                Vector3 cannonVect = lvl1Vects[iter];

                Vector3[] matrix = { Vector3.zero, Vector3.zero };
                int v1x;
                int v2x;
                int v1y;
                int v2y;

                switch (iter)
                {
                    case 0: // 1,0
                        matrix[0] = shipLoc;
                        break;

                    case 1:  // 0,1
                        matrix[1] = shipLoc;
                        break;

                    case 2:  // 1,1
                        v1x = Mathf.RoundToInt(shipLoc.x / Random.Range(1, 7));
                        v2x = (int)shipLoc.x - v1x;
                        v1y = Mathf.RoundToInt(shipLoc.y / Random.Range(1, 7));
                        v2y = (int)shipLoc.y - v1y;

                        if(Random.Range(0.0f,1.0f) > 0.5f)
                        {
                            int temp = v1x;
                            v1x = v2x;
                            v2x = temp;
                        }
                        if (Random.Range(0.0f, 1.0f) > 0.5f)
                        {
                            int temp = v1y;
                            v1y = v2y;
                            v2y = temp;
                        }

                        matrix[0] = new Vector3(v1x, v1y, 0);
                        matrix[1] = new Vector3(v2x, v2y, 0);
                        break;

                    case 3:  // 1,-1
                        v2x = Mathf.RoundToInt(shipLoc.x / Random.Range(2, 5));
                        v1x = (int)shipLoc.x + v2x;
                        v2y = Mathf.RoundToInt(shipLoc.y / Random.Range(2, 5));
                        v1y = (int)shipLoc.y + v2y;

                        matrix[0] = new Vector3(v1x, v1y, 0);
                        matrix[1] = new Vector3(v2x, v2y, 0);
                        break;
                }

                break;

            case 2:
                break;

            case 3:
                break;
        }
    }


    // public void switchStage()
    // {
    //     stage01.SetActive(false);
    //     this.InitGameController(P02W);
    // }

    public void RecordData()
    {
        switch (stageNumber)
        {
            case 1:
                if (GameRoot.player.users.p2_1clear_time == 0.0f)
                    GameRoot.player.users.p2_1clear_time = tot_puzzleTime;
                else if (tot_puzzleTime < GameRoot.player.users.p2_1clear_time)
                    GameRoot.player.users.p2_1clear_time = tot_puzzleTime;

                GameRoot.player.users.p2_1attempts++;
                GameRoot.player.p2_1.attemptNum = GameRoot.player.users.p2_1attempts;
                GameRoot.player.p2_1.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.users.p2_1attempts.ToString();

                break;

            case 2:
                if (GameRoot.player.users.p2_2clear_time == 0.0f)
                    GameRoot.player.users.p2_2clear_time = tot_puzzleTime;
                else if (tot_puzzleTime < GameRoot.player.users.p2_2clear_time)
                    GameRoot.player.users.p2_2clear_time = tot_puzzleTime;

                GameRoot.player.users.p2_2attempts++;
                GameRoot.player.p2_2.attemptNum = GameRoot.player.users.p2_2attempts;
                GameRoot.player.p2_2.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.users.p2_2attempts.ToString();
                break;

            case 3:
                if (GameRoot.player.users.p2_3clear_time == 0.0f)
                    GameRoot.player.users.p2_3clear_time = tot_puzzleTime;
                else if (tot_puzzleTime < GameRoot.player.users.p2_3clear_time)
                    GameRoot.player.users.p2_3clear_time = tot_puzzleTime;

                GameRoot.player.users.p2_3attempts++;
                GameRoot.player.p2_3.attemptNum = GameRoot.player.users.p2_3attempts;
                GameRoot.player.p2_3.usernameAttempt = GameRoot.player.users.username + "." + GameRoot.player.users.p2_3attempts.ToString();
                break;
        }

    }
}
