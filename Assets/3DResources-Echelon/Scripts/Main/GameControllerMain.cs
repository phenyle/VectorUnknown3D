using UnityEngine;

public class GameControllerMain : GameControllerRoot
{
    [Header("Player & Camera")]
    public GameObject playerAndCam;
    public GameObject player;
    public GameObject mainCamera;

    [Header("Load in Locations")]
    public Transform fromPuzzle1;
    public Transform fromPuzzle2;
    public Transform fromPuzzle3;
    public Transform fromPuzzle4;
    public Transform fromGameStart;


    //public Transform PCFromPuzzle01;
    //public Transform PCFromPuzzle02;
    //public Transform PCFromPuzzle03;
    //public Transform Loadin;
    //public Transform PCFromPuzzle04; // puzzle 4 area to pop out of

    public Transform sunLight;
    public Transform moonLight;
    public LPWAsset.LowPolyWaterScript ocean;
    public Material night;
    public Transform portalPuzzle03; 
    public Sprite[] spriteChecks;

    public ShipRepair shipRepair;

    [HideInInspector]
    public MainWindow MW;


    private Vector3 startPosition;
    private Vector3 previousPosition;
    private int timer = 0;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameRoot3D.instance.FindPlayerAndCamera();
    }

    public override void InitGameController(MainWindow MW)
    {
        Debug.Log("Init GameController Main");
        base.InitGameController();

        Debug.Log("Connect Main Window");
        this.MW = MW;

        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameRoot3D.instance.FindPlayerAndCamera();
        startPosition = player.transform.position;

        CheckPlayerDataRecords();

        SetUserSettings();


        //Load player in at portals
        switch (GameRoot3D.instance.exitPuzzle)
        {
            case 0: // new player object is activated and others are turned off
                playerAndCam.transform.position = fromGameStart.position;
                playerAndCam.transform.forward = fromGameStart.forward;
                startPosition = player.transform.position;
                break;

            case 1:
                playerAndCam.transform.position = fromPuzzle1.position;
                playerAndCam.transform.forward = fromPuzzle1.forward;
                startPosition = player.transform.position;
                break;

            case 2:
                playerAndCam.transform.position = fromPuzzle2.position;
                playerAndCam.transform.forward = fromPuzzle2.forward;
                startPosition = player.transform.position;
                break;

            case 3:
                playerAndCam.transform.position = fromPuzzle3.position;
                playerAndCam.transform.forward = fromPuzzle3.forward;
                startPosition = player.transform.position;
                break;


            case 4: // new player object is activated and others are turned off
                playerAndCam.transform.position = fromPuzzle4.position;
                playerAndCam.transform.forward = fromPuzzle4.forward;
                startPosition = player.transform.position;
                break;


        }


//        //*****************Start of the game 
//        if (GameRoot.instance.puzzleCompleted[0] == false)
//        {
//            if (DialogueManager.showIntro)
//            {
//                FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Intro"));
//                DialogueManager.showIntro = false;
// //               MW.SetInstructionText("Explore the Land.");
//            }
//            else
//            {
//                GameRoot.instance.LockCamera(false);
// //               mainCamera.GetComponent<CameraController>().isLock = false;
//            }          
//        }

//        //*****************If player only finish puzzle 01 and not 2
//        if (GameRoot.instance.puzzleCompleted[0] == true && GameRoot.instance.puzzleCompleted[1] != true)
//        {
//            if(DialogueManager.showM_00)
//            {
//                FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Main_00"));
//                DialogueManager.showM_00 = false;
//            }
//            else
//            {
//                GameRoot.instance.LockCamera(false);
//                //               mainCamera.GetComponent<CameraController>().isLock = false;
//            }

//            //Set the instructional text and check image
////            MW.SetInstructionText("Find & enter the cave to beat the pirates.");
// //           MW.SetCheckImage(0, spriteChecks[0]);
//        }
//        //If player finish puzzle 01 & puzzle 02
//        else if(GameRoot.instance.puzzleCompleted[0] == true && GameRoot.instance.puzzleCompleted[1] == true && GameRoot.instance.puzzleCompleted[2] == false)
//        {
//            SetActive(sunLight, false);
//            SetActive(moonLight, true);
//            ocean.sun = moonLight.GetChild(0).GetComponent<Light>();      

//            RenderSettings.skybox = night;
//            SetActive(portalPuzzle03, true);

//            if(DialogueManager.showM_01)
//            {
//                FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Main_01"));
//                DialogueManager.showM_01 = false;
//            }
//            else
//            {
//                GameRoot.instance.LockCamera(false);
//                //               mainCamera.GetComponent<CameraController>().isLock = false;
//            }

////            MW.SetInstructionText("Find & enter the pirates' hideout to save villagers.");
////            MW.SetCheckImage(0, spriteChecks[0]);
////            MW.SetCheckImage(1, spriteChecks[1]);
//        }
//        //***************** if PLayer finishes puzzle 1, 2 ,and 3
//        else if (GameRoot.instance.puzzleCompleted[0] == true && GameRoot.instance.puzzleCompleted[1] == true && GameRoot.instance.puzzleCompleted[2])
//        {

//            if (DialogueManager.showM_00)
//            {
//                FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Main_OnePuzzleLeft"));
//                DialogueManager.showM_00 = false;
//            }
//            else
//            {
//                GameRoot.instance.LockCamera(false);
//                //               mainCamera.GetComponent<CameraController>().isLock = false;
//            }


// //           MW.SetCheckImage(0, spriteChecks[0]);
//  //          MW.SetCheckImage(1, spriteChecks[1]);
//   //         MW.SetCheckImage(2, spriteChecks[2]);

//            GameRoot.instance.LockCamera(false);
//            //               mainCamera.GetComponent<CameraController>().isLock = false;
//        }

//        //***************** if PLayer finishes puzzle 1, 2, 3 and 4
//        else if (GameRoot.instance.puzzleCompleted[0] == true && GameRoot.instance.puzzleCompleted[1] == true && GameRoot.instance.puzzleCompleted[2] && GameRoot.instance.puzzleCompleted[3])
//        {

// //           MW.SetCheckImage(0, spriteChecks[0]);
//  //          MW.SetCheckImage(1, spriteChecks[1]);
//   //         MW.SetCheckImage(2, spriteChecks[2]);
//    //        MW.SetCheckImage(3, spriteChecks[3]);

//            GameRoot.instance.LockCamera(false);
//            //               mainCamera.GetComponent<CameraController>().isLock = false;
//        }
//        //***************** if Player finishes puzzle 2
//        if (GameRoot.instance.puzzleCompleted[1] == true)
//        {
// //           MW.SetCheckImage(1, spriteChecks[1]);

//            GameRoot.instance.LockCamera(false);
//            //               mainCamera.GetComponent<CameraController>().isLock = false;
//        }
//        //***************** if PLayer finishes puzzle 3
//        if (GameRoot.instance.puzzleCompleted[2] == true)
//        {

// //           MW.SetCheckImage(2, spriteChecks[2]);

//            GameRoot.instance.LockCamera(false);
//            //               mainCamera.GetComponent<CameraController>().isLock = false;
//        }
//        //***************** if PLayer finishes puzzle 4
//        if (GameRoot.instance.puzzleCompleted[3] == true)
//        {
//            if (DialogueManager.showM_01)
//            {
//                FindObjectOfType<DialogueManager>().StartDialogue(resourceService.LoadConversation("Main_P4_completed"));
//                DialogueManager.showM_01 = false;
//            }
//            else
//            {
//                GameRoot.instance.LockCamera(false);
//                //               mainCamera.GetComponent<CameraController>().isLock = false;
//            }

//            MW.SetCheckImage(3, spriteChecks[3]);

//            GameRoot.instance.LockCamera(false);
//            //               mainCamera.GetComponent<CameraController>().isLock = false;
//        }
//        // DialogueManager.isInDialogue = true;
//        // DialogueManager.isPuzzleLock = false;


        if (GameRoot3D.instance.firstCompletion)
        {
            shipRepair.AnimatePartUnlock(player, mainCamera);
            GameRoot3D.instance.firstCompletion = false;
        }
        else
        {
            shipRepair.ShipUpdate();
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
        //         GameRoot.ShowTips("Press 'R' to reset position.", true, false);
        //     }
        // }
        // else
        // {
        //     timer = 0;
        // }

        if (shipRepair.fxComplete && !DialogueManager.isInDialogue)
        {
//            GameRoot.instance.IsLock(false);
            //    player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().isLock = false;
            //    player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>().enabled = true;
        }


        
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = startPosition;
            GameRoot3D.ShowTips("", true, false);
        }

		// if (!DialogueManager.isInDialogue)
		// {
		// 	DialogueManager.isInDialogue = false;
		// }

        previousPosition = player.transform.position;
    }

    /// <summary>
    /// Everytime the player is loaded into the main scene (like on game start or
    /// after completeing a level).  This method checks to see if they have a 
    /// recorded completion time for each stage.  If they do, it flags that level
    /// on GameRoot as complete.
    /// </summary>
    public void CheckPlayerDataRecords()
    {
        //Puzzle 1-----------
        //legacy puzzle checks:
        if (GameRoot3D.player.records.p1_time != 0.0f)
            GameRoot3D.instance.puzzleCompleted[0] = true;
        else
            GameRoot3D.instance.puzzleCompleted[0] = false;

        //new puzzle checks:
        if (GameRoot3D.player.records.p1_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[1][1] = true;
        else
            GameRoot3D.instance.puzzlesDone[1][1] = false;

        //Puzzle 2------------
        //legacy puzzle checks:
        if (GameRoot3D.player.records.p2_1_time != 0.0f ||
            GameRoot3D.player.records.p2_2_time != 0.0f ||
            GameRoot3D.player.records.p2_3_time != 0.0f)
            GameRoot3D.instance.puzzleCompleted[1] = true;
        else
            GameRoot3D.instance.puzzleCompleted[1] = false;

        //new puzzle checks:
        if (GameRoot3D.player.records.p2_1_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[2][1] = true;
        else
            GameRoot3D.instance.puzzlesDone[2][1] = false;

        if (GameRoot3D.player.records.p2_2_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[2][2] = true;
        else
            GameRoot3D.instance.puzzlesDone[2][2] = false;

        if (GameRoot3D.player.records.p2_3_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[2][3] = true;
        else
            GameRoot3D.instance.puzzlesDone[2][3] = false;


        //Puzzle 3-----------
        //legacy puzzle checks:
        if (GameRoot3D.player.records.p3_1_time != 0.0f ||
            GameRoot3D.player.records.p3_2_time != 0.0f ||
            GameRoot3D.player.records.p3_3_time != 0.0f)
            GameRoot3D.instance.puzzleCompleted[2] = true;
        else
            GameRoot3D.instance.puzzleCompleted[2] = false;


        //new puzzle checks:
        if (GameRoot3D.player.records.p3_1_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[3][1] = true;
        else
            GameRoot3D.instance.puzzlesDone[3][1] = false;

        if (GameRoot3D.player.records.p3_2_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[3][2] = true;
        else
            GameRoot3D.instance.puzzlesDone[3][2] = false;

        if (GameRoot3D.player.records.p3_3_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[3][3] = true;
        else
            GameRoot3D.instance.puzzlesDone[3][3] = false;

        //Puzzle 4-----------
        //legacy puzzle checks:
        if (GameRoot3D.player.records.p4_1_time != 0.0f ||
            GameRoot3D.player.records.p4_2_time != 0.0f ||
            GameRoot3D.player.records.p4_3_time != 0.0f)
            GameRoot3D.instance.puzzleCompleted[3] = true;
        else
            GameRoot3D.instance.puzzleCompleted[3] = false;

        //new puzzle checks
        if (GameRoot3D.player.records.p4_1_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[4][1] = true;
        else
            GameRoot3D.instance.puzzlesDone[4][1] = false;

        if (GameRoot3D.player.records.p4_2_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[4][2] = true;
        else
            GameRoot3D.instance.puzzlesDone[4][2] = false;

        if (GameRoot3D.player.records.p4_3_time != 0.0f)
            GameRoot3D.instance.puzzlesDone[4][3] = true;
        else
            GameRoot3D.instance.puzzlesDone[4][3] = false;
    }

    public void SetUserSettings()
    {
        GameRoot3D.instance.audioService.SetBgVolume(GameRoot3D.player.users.soundVol);
        GameRoot3D.instance.audioService.SetSoundFXVolume(GameRoot3D.player.users.soundFX);
    }
}
