using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class ShipRepair : MonoBehaviour
{
    [Header("Hull Parts")]
    public GameObject hullBroken;
    public GameObject hullFixed;

    [Header("Mast/Sail Parts")]
    public GameObject mast1;
    public GameObject mast2;
    public GameObject mast3;
    public GameObject sail1;
    public GameObject sail2;
    public GameObject sail3;
    public GameObject rigging;
    public GameObject sailMisc1;
    public GameObject sailMisc2;

    [Header("Cannon/CannonBall Parts")]
    public GameObject portCannons;
    public GameObject stbdCannons;
    public GameObject portCannonBalls;
    public GameObject stbdCannonBalls;

    [Header("Treasure Parts")]
    public GameObject treasureForecastle;
    public GameObject treasureStern;
    public GameObject treasureChests;
    public GameObject treasureSparkles;

    [Header("Camera Positions")]
    public Transform fromPuzzle1;
    public Transform fromPuzzle2;
    public Transform fromPuzzle3;
    public Transform fromPuzzle4;

    [Header("FX Settings")]
    public GameObject pulseFX;
    public GameObject explosionFX;
    [Range(1,1000)]
    public int fxFixedInterval = 24;
    [Range(1,10)]
    [Tooltip("FX animation hang time in seconds")]
    public float fxMaxTime = 3.0f;
    private float fxTime = 0.0f;
    private bool playFX = true;
    [Range(0.0f, 1.0f)]
    [Tooltip("Increasing this Can HUGELY effect performance if there a lot of objects\nSet to 0 to turn off all random effects.")]
    public float fxRand = 0.06f;
    private int fxIter = 0;

    [Range(1, 10)]
    [Tooltip("Wait time after animation hang time in seconds")]
    public float waitMaxTime = 3.0f;
    private float waitTime = 0.0f;


    [Header("Sounds")]
    public AudioClip fixedPulseSound;
    public AudioClip[] randomPulseSounds;
    public AudioClip fixedExploSound;
    public AudioClip[] randomExploSounds;

    private GameObject player;
    private GameObject mainCamera;
    private GameObject shipCamera;
    private Vector3 prevCameraPos;
    private GameObject cameraPos;
    private GameObject targetPos;

    public GameControllerMain GCM;

    private bool animate = false;
    public bool fxComplete = true;

    // Start is called before the first frame update
    void Start()
    {
  //      GCM = GameObject.Find("GameController").GetComponent<GameControllerMain>();

        fxIter = 0;
        waitTime = 0;

        shipCamera = GameObject.FindGameObjectWithTag("PuzzleCamera");

        pulseFX.SetActive(false);
        explosionFX.SetActive(false);

        //ShipUpdate();
    }


    public void Update()
    {
        if(animate)
        {  
            if (playFX)
            {
                FXSelect(GameRoot3D.instance.prevStage, GameRoot3D.instance.prevLevel, fxIter);
                fxIter++;

                fxTime += Time.deltaTime;
                if (fxTime > fxMaxTime)
                {
                    GameRoot3D.instance.puzzlesDone[GameRoot3D.instance.prevStage][GameRoot3D.instance.prevLevel] = true;
                    ShipUpdate();
                    playFX = false;
                }
            }
            else
            {
                waitTime += Time.deltaTime;
                if (waitTime > waitMaxTime)
                {
                    GCM.GetComponent<ObjectGlide>().SetEventSys(GameRoot3D.camEvents);
                    GameRoot3D.camEvents.AddListener(camAtPlayer);
                    float speed = ((shipCamera.transform.position - mainCamera.transform.position).magnitude / 1400.0f) * 6.0f;
                    GCM.GetComponent<ObjectGlide>().SetObjectMoveSpeed(speed);
                    GCM.GetComponent<ObjectGlide>().GlideAndLookMovingTarget(shipCamera, mainCamera, mainCamera.GetComponent<CameraController>().target.gameObject, -5.0f, speed, 0.1f);
                    waitTime = 0;
                    playFX = true;
                    animate = false;
                }
            }
        }

    }

    /// <summary>
    /// This function begins the Animated ship repair steps
    /// It first temporarily turns off player control and camera control
    /// Then, based on the previous stage/level completed from GameRoot it
    /// determines where to move the camera to and what it should look at
    /// When the parameters are set it tells the camera to glide over and enables
    /// the animate tag for the update to take over.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="camera"></param>
    public void AnimatePartUnlock(GameObject player, GameObject camera)
    {
        //since this only runs when a new part is unlocked
        //we must turn that part OFF first before running ShipUpdate()
        //then we'll turn that puzzleDones[][] back on after the sparkles and fanfair
        GameRoot3D.instance.puzzlesDone[GameRoot3D.instance.prevStage][GameRoot3D.instance.prevLevel] = false;
        ShipUpdate();
        this.player = player;
        this.mainCamera = camera;
        fxComplete = false;
        prevCameraPos = mainCamera.transform.position;

        player.GetComponent<ThirdPersonUserControl>().isLock = true;

        mainCamera.GetComponent<Camera>().enabled = false;
        shipCamera.transform.position = mainCamera.transform.position;
        shipCamera.transform.rotation = mainCamera.transform.rotation;
        shipCamera.GetComponent<Camera>().enabled = true;


        switch (GameRoot3D.instance.prevStage)
        {
            case 1:
                cameraPos = fromPuzzle1.gameObject;
                targetPos = this.gameObject;
                break;

            case 2:
                cameraPos = fromPuzzle2.gameObject;
                switch(GameRoot3D.instance.prevLevel)
                {
                    case 1:
                        targetPos = stbdCannons;
                        break;
                    case 2:
                        targetPos = portCannons;
                        break;
                }    

                break;

            case 3:
                cameraPos = fromPuzzle3.gameObject;
                switch(GameRoot3D.instance.prevLevel)
                {
                    case 1:
                        targetPos = treasureStern;
                        break;
                    case 2:
                        targetPos = treasureForecastle;
                        break;
                    case 3:
                        targetPos = treasureChests;
                        break;
                }
                break;

            case 4:
                cameraPos = fromPuzzle4.gameObject;
                switch(GameRoot3D.instance.prevLevel)
                {
                    case 1:
                        targetPos = sail3;
                        break;
                    case 2:
                        targetPos = sail1;
                        break;
                    case 3:
                        targetPos = sail2;
                        break;
                }

                break;
        }

        GCM.GetComponent<ObjectGlide>().SetEventSys(GameRoot3D.camEvents);
        GameRoot3D.camEvents.AddListener(camAtShip);
        float speed = ((this.player.transform.position - cameraPos.transform.position).magnitude / 1400.0f) * 6.0f;
        GCM.GetComponent<ObjectGlide>().SetObjectMoveSpeed(speed);
        GCM.GetComponent<ObjectGlide>().GlideAndLookMovingTarget(shipCamera, cameraPos, targetPos, 30.0f, speed, 0.1f);
    }

    public void camAtShip()
    {
        animate = true;
        playFX = true;
        fxIter = 0;
        fxTime = 0.0f;
        waitTime = 0.0f;
        GameRoot3D.camEvents.RemoveListener(camAtShip);
    }

    public void camAtPlayer()
    {
        player.GetComponent<ThirdPersonUserControl>().isLock = false;

        shipCamera.GetComponent<Camera>().enabled = false;
        mainCamera.GetComponent<Camera>().enabled = true;

        fxComplete = true;
        animate = false;
        playFX = true;
        fxIter = 0;
        fxTime = 0.0f;
        waitTime = 0.0f;
        GameRoot3D.camEvents.RemoveListener(camAtPlayer);
    }



    /// <summary>
    /// This Method determines what parts of the ship to play effects at depending
    /// on which level was competeled
    /// </summary>
    /// <param name="stage">The stage that was just completed</param>
    /// <param name="level">The level at which the stage was completed</param>
    /// <param name="iter">the duration thusfar for the effects</param>
    public void FXSelect(int stage, int level, int iter)
    {
        switch(stage)
        {
            case 1:
                switch(level)
                {
                    case 1:
                        FXGlowRings(hullFixed.GetComponent<Renderer>(), iter, true);

                        ////special case turn on/off for stage1
                        ////stage1 has to turn an object on AND turn an object off since it's the main hull
                        ////every other stage only has to turn an object on
                        //if(iter == fxMaxTime)
                        //{
                        //    hullFixed.SetActive(true);
                        //    hullBroken.SetActive(false);
                        //}
                        ////hullBroken.GetComponent<Renderer>().material.color = new Color(brokenMat.r, brokenMat.g, brokenMat.b, (fxSpeed - iter) / fxSpeed);
                        ////hullFixed.GetComponent<Renderer>().material.color = new Color(fixedMat.r, fixedMat.g, fixedMat.b,  iter / fxSpeed);

                        break;                        
                }

                break;

            case 2:
                switch (level)
                {
                    case 1:
                        foreach (Renderer cannonRends in portCannons.GetComponentsInChildren<Renderer>())
                            FXGlowRings(cannonRends, iter, true);

                        //foreach (Renderer cannonBallRends in portCannonBalls.GetComponentsInChildren<Renderer>())
                        //    FXGlowRings(cannonBallRends, iter, false);
                        break;

                    case 2:
                        foreach (Renderer cannonRends in stbdCannons.GetComponentsInChildren<Renderer>())
                            FXGlowRings(cannonRends, iter, true);

                        //foreach (Renderer cannonBallRends in stbdCannonBalls.GetComponentsInChildren<Renderer>())
                        //    FXGlowRings(cannonBallRends, iter, false);

                        break;
                }

                break;

            case 3:
                switch (level)
                {
                    case 1:
                        //Might be too many objects here to really work
                        //might just make a custom fx method for this one
                        foreach (Renderer treasurRend in treasureStern.GetComponentsInChildren<Renderer>())
                            FXGlowRings(treasurRend, iter, true);
                        break;

                    case 2:
                        foreach (Renderer treasurRend in treasureForecastle.GetComponentsInChildren<Renderer>())
                            FXGlowRings(treasurRend, iter, true);
                        break;

                    case 3:
                        foreach (Renderer treasurRend in treasureChests.GetComponentsInChildren<Renderer>())
                            FXGlowRings(treasurRend, iter, true);
                        break;
                }

                break;

            case 4:
                switch (level)
                {
                    case 1:
                        FXGlowRings(mast3.GetComponent<Renderer>(), iter, true);
                        break;

                    case 2:
                        FXGlowRings(mast1.GetComponent<Renderer>(), iter, true);
                        break;

                    case 3:
                        FXGlowRings(mast2.GetComponent<Renderer>(), iter, true);
                        break;
                }

                break;
        }

        fxComplete = true;
    }

    private void FXGlowRings(Renderer objUnlocked, int iter, bool randomEffects)
    {
        GameObject explosions = GameObject.Instantiate(explosionFX);
        GameObject pulses = GameObject.Instantiate(pulseFX);

        pulses.transform.position = objUnlocked.bounds.center;
        pulses.transform.localScale = objUnlocked.bounds.extents;
        //fixed interval overall FX scaled to object size
        if (iter % fxFixedInterval == 0)
        {
            pulses.SetActive(true);
            pulses.GetComponent<ParticleSystem>().Play(true);
            GameRoot3D.instance.audioService.PlayFXAudio("ShipRepair/" + fixedPulseSound.name.ToString());            
        }

        if (randomEffects)
        {
            float rand = Random.Range(0.0001f, 1.0f);
            if (rand < fxRand)
            {
                GameObject randPulse = GameObject.Instantiate(pulseFX);
                Vector3 randPos = new Vector3(Random.Range(-0.5f * objUnlocked.bounds.extents.x, 0.5f * objUnlocked.bounds.extents.x), Random.Range(-0.5f * objUnlocked.bounds.extents.y, 0.5f * objUnlocked.bounds.extents.y), Random.Range(-0.5f * objUnlocked.bounds.extents.z, 0.5f * objUnlocked.bounds.extents.z)) + objUnlocked.bounds.center;
                randPulse.transform.position = randPos;
                float randScal = Random.Range(0.05f, 0.5f);
                randPulse.transform.localScale = randScal * objUnlocked.bounds.extents;
                randPulse.SetActive(true);
                randPulse.GetComponent<ParticleSystem>().Play(true);
                int soundRand = Random.Range(0, randomPulseSounds.Length - 1);
                GameRoot3D.instance.audioService.PlayFXAudio("ShipRepair/" + randomPulseSounds[soundRand].name.ToString());
            }
        }
    }


    public void ShipUpdate()
    {
        //Stage1 Completed
        hullBroken.SetActive(!GameRoot3D.instance.puzzlesDone[1][1]); //toggle off the broken ship
        hullFixed.SetActive(GameRoot3D.instance.puzzlesDone[1][1]); //toggle on the fixed ship


        //Stage2 Completed
        portCannons.SetActive(GameRoot3D.instance.puzzlesDone[2][1]);
        portCannonBalls.SetActive(GameRoot3D.instance.puzzlesDone[2][1]);

        stbdCannons.SetActive(GameRoot3D.instance.puzzlesDone[2][2]);
        stbdCannonBalls.SetActive(GameRoot3D.instance.puzzlesDone[2][2]);
        //this can be later broken up with more levels into something like:
        //level1 done: both port/stbd cannonballs
        //level2 done: portside Cannons
        //level3 done: stbd Cannons
        //all done: hull cannons (note: need to break hull model to rip hull cannons out)


        //Stage3 Completed        
        treasureStern.SetActive(GameRoot3D.instance.puzzlesDone[3][1]);
        treasureForecastle.SetActive(GameRoot3D.instance.puzzlesDone[3][2]);
        treasureChests.SetActive(GameRoot3D.instance.puzzlesDone[3][3]);

        if (GameRoot3D.instance.puzzlesDone[3][1] &&
                GameRoot3D.instance.puzzlesDone[3][2] &&
                GameRoot3D.instance.puzzlesDone[3][3])
        {
            treasureSparkles.SetActive(true);
        }
        else
        {
            treasureSparkles.SetActive(false);

        }


        //Stage4 Completed
        mast1.SetActive(GameRoot3D.instance.puzzlesDone[4][2]);
        mast2.SetActive(GameRoot3D.instance.puzzlesDone[4][3]); //mast2 is the main, so making it for level 3
        mast3.SetActive(GameRoot3D.instance.puzzlesDone[4][1]);

        sail1.SetActive(GameRoot3D.instance.puzzlesDone[4][2]);
        sail2.SetActive(GameRoot3D.instance.puzzlesDone[4][3]); //sail2 is the main, so making it for level 3
        sail3.SetActive(GameRoot3D.instance.puzzlesDone[4][1]);

        if(GameRoot3D.instance.puzzlesDone[4][1] &&
            GameRoot3D.instance.puzzlesDone[4][2] &&
            GameRoot3D.instance.puzzlesDone[4][3])
        {
            rigging.SetActive(true);
            sailMisc1.SetActive(true);
            sailMisc2.SetActive(true);
        }
        else
        {
            rigging.SetActive(false);
            sailMisc1.SetActive(false);
            sailMisc2.SetActive(false);

        }

    }


}
