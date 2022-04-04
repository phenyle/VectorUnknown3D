using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Events;

public class GrappleCode : MonoBehaviour
{
    [Header("Rope Parts")]
    public GameObject player;
    public GameObject rope;
    public GameObject hook;
    public GameObject ropeStart;
    public GameObject ropeEnd;
    public GameObject goalPoint;
    public GameObject muzzleFlash;
    public GameObject gunSmoke;
    private float ropeWidth;
    public float hookSpeed;
    private Vector3 hookDirection;
    private Vector3 startPos;


    private float speedIncri;

    [Header("Player Parts")]
    public GameObject charHand;
    public GameObject charShoulder;
    private float arcHeight;
    public Transform prevShoulderRotation;



    private bool grappleAnimation;
    private bool rightAnswer;
    private bool playerglide = false;
    private bool failGrapple = false;
    private bool failFall = false;
    private float failTimer = 0.0f;
    private float maxFailTime = 3.5f;
    public GameObject demoTarget;
    public bool DemoAnim = false;
    private Vector3 prevTargetPos;
    public TrailRenderer trail;
    public TrailRenderer failTrail;
    public LineRenderer ropeLine;
    public Vector3 dir2Target;

    private Puzzle04Controller PC04;




    // Start is called before the first frame update
    void Start()
    {
        player = GameControllerPuzzle04.instance.player;
        charShoulder = player.transform.Find("Root").Find("Hips").Find("Spine_01").Find("Spine_02").Find("Spine_03").Find("Clavicle_R").Find("Shoulder_R").gameObject;
        charHand = charShoulder.transform.Find("Elbow_R").Find("Hand_R").gameObject;


        ropeEnd.SetActive(false);
        ropeStart.transform.position = charHand.transform.position;
        ropeEnd.transform.position = charHand.transform.position;
        gunSmoke.SetActive(false);
        hook.transform.position = ropeEnd.transform.position;
        grappleAnimation = false;
        prevShoulderRotation = charShoulder.transform;
        prevShoulderRotation.rotation = charShoulder.transform.rotation;
        hookSpeed = 1.5f;
        ropeWidth = 0.35f;
        prevTargetPos = demoTarget.transform.position;
        trail.sortingOrder = 500;
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        /**Theory of the Grapple Animation in steps
         * 1: player hits submit with the correct answer
         * 2: Set the pistol, rope, and hook active at hand (do later)
         * 3: Rotate shoulder towards the goal
         * 4: Move ropeEnd towards goal
         * 5: Update rope between hand and ropeEnd
         * 6: ropeEnd at the same position as goal
         * 7: Move player towards goal
         * 8: Update rope between goal and new player position
         * 9: When player is near goal, rotate player pos around goal
         * 10: rotate should be opposite of the terrain (raycasts?)
         * 11: when player is around and above goal disconnect rope
         * 12: continue moving player a set distance from goal
         */

        //player hits submit with the correct answer
        if (grappleAnimation)
        {
            this.gameObject.SetActive(true);
            ropeEnd.SetActive(true);
            trail.gameObject.SetActive(false);
            ropeLine.enabled = true;
            player.transform.LookAt(new Vector3(goalPoint.transform.position.x, player.transform.position.y, goalPoint.transform.position.z));

            ropeLine.SetPosition(0, charHand.transform.position);
            ropeLine.SetPosition(1, ropeEnd.transform.position);
            if (playerglide)
            {
                ropeEnd.transform.position = goalPoint.transform.position;
            }

        }
        else if (DemoAnim)
        {
            float currMag = (demoTarget.transform.position - ropeEnd.transform.position).magnitude;
            if (prevTargetPos != demoTarget.transform.position || currMag < 0.05f)
            {
                ropeEnd.transform.position = GameControllerPuzzle04.instance.currPuzzle.startPoint.transform.position;
                trail.Clear();
            }
            ropeEnd.transform.LookAt(demoTarget.transform.position, Vector3.up);
            trail.gameObject.SetActive(true);

            ropeEnd.transform.position = Vector3.MoveTowards(ropeEnd.transform.position, demoTarget.transform.position, 0.01f * dir2Target.magnitude);
 //           ropeEnd.GetComponent<Rigidbody>().MovePosition(dir2Target.normalized * 0.1f * Time.deltaTime);
 //           VectorBetweenPoints(rope, start, end, ropeWidth);
            prevTargetPos = demoTarget.transform.position;
        }
        else if (failGrapple)
        {
            float currMag = (demoTarget.transform.position - ropeEnd.transform.position).magnitude;
            if (currMag < 0.05f)
            {
                failFall = true;
                failGrapple = false;
                ropeEnd.GetComponent<SphereCollider>().enabled = true;
                ropeEnd.GetComponent<Rigidbody>().isKinematic = false;
                ropeEnd.GetComponent<Rigidbody>().AddForce(dir2Target * dir2Target.magnitude, ForceMode.Force);

                trail.Clear();

            }
            failTrail.gameObject.SetActive(true);

            ropeEnd.transform.position = Vector3.MoveTowards(ropeEnd.transform.position, demoTarget.transform.position, 0.05f * dir2Target.magnitude);
        }
        else if (failFall)
        {
            failTimer += Time.deltaTime;
            if (failTimer > maxFailTime)
            {
                failTimer = 0.0f;
                failGrapple = false;
                failTrail.Clear();
                trail.Clear();
                failTrail.gameObject.SetActive(false);
                ropeEnd.GetComponent<SphereCollider>().enabled = false;
                ropeEnd.GetComponent<Rigidbody>().isKinematic = true;
                ropeEnd.transform.position = PC04.startPoint.transform.position;

                failGrapple = false;
                GameControllerPuzzle04.instance.visVector.ToggleVisualVectors(true);
                VisVectController.axis.ToggleAxis(GameControllerPuzzle04.instance.P04W.axisOn);
                VisVectController.grid.ToggleGrid(GameControllerPuzzle04.instance.P04W.gridOn);
                VisVectController.grid.ToggleXYplane(GameControllerPuzzle04.instance.P04W.XYtoggle);
                VisVectController.grid.ToggleYZplane(GameControllerPuzzle04.instance.P04W.YZtoggle);
                VisVectController.grid.ToggleXZplane(GameControllerPuzzle04.instance.P04W.XZtoggle);
                VisVectController.projections.ToggleProjections(GameControllerPuzzle04.instance.P04W.projectionsOn);
                VisVectController.projections.ToggleXYprojections(GameControllerPuzzle04.instance.P04W.XYtoggle);
                VisVectController.projections.ToggleYZprojections(GameControllerPuzzle04.instance.P04W.YZtoggle);
                VisVectController.projections.ToggleXZprojections(GameControllerPuzzle04.instance.P04W.XZtoggle);
                VisVectController.anchors.ToggleAnchors(GameControllerPuzzle04.instance.P04W.anchorsOn);
                VisVectController.anchors.ToggleXYanchors(GameControllerPuzzle04.instance.P04W.XYtoggle);
                VisVectController.anchors.ToggleYZanchors(GameControllerPuzzle04.instance.P04W.YZtoggle);
                VisVectController.anchors.ToggleXZanchors(GameControllerPuzzle04.instance.P04W.XZtoggle);


                GameControllerPuzzle04.instance.P04W.SubmitButton.interactable = true;
                DemoAnim = true;

            }
        }
    }

    public void GrappleEventListener()
    {
        demoTarget.transform.position = VisVectController.playerVectors.GetAnswerEnd();

        if (GameControllerPuzzle04.instance.P04W.card1 != null &&
            GameControllerPuzzle04.instance.P04W.card1 != null &&
            GameControllerPuzzle04.instance.currPuzzle.playerAnswer != Vector3.zero)
        {
            gameObject.SetActive(true);
            enabled = true;
            DemoAnim = true;
            SetDemoTargetPos(VisVectController.playerVectors.GetAnswerEnd());
        }
        else
        {
            DemoAnim = false;
        }
    }

    public void InitGrapple(bool correct, Vector3 direction, GameObject goal, Vector3 playerStartPos, float arc)
    {
        goalPoint = goal;
        rightAnswer = correct;
        hookDirection = direction;
        startPos = playerStartPos;
        arcHeight = arc;
    }


    public void grappleToGoal(Puzzle04Controller puzzleController)
    {
        PC04 = puzzleController;
        this.gameObject.SetActive(true);
        trail.Clear();
        ropeStart.transform.position = charHand.transform.position;
        ropeEnd.transform.position = charHand.transform.position;

        GameControllerPuzzle04.instance.GetAudioService().PlayFXAudio("Puzzle04/grappleShot");
        muzzleFlash.SetActive(true);
        muzzleFlash.transform.position = charHand.transform.position;
        muzzleFlash.transform.LookAt(demoTarget.transform.position, Vector3.up);
        muzzleFlash.GetComponent<ParticleSystem>().Play();

        GameRoot3D.instance.LockPlayer(true);

        failGrapple = false;
        failFall = false;
        DemoAnim = false;
        playerglide = false;
        grappleAnimation = true;
        trail.gameObject.SetActive(false);
        this.enabled = true;

        if (GameControllerPuzzle04.instance.Difficulty == 3)
        {
            speedIncri = 1.3f;
        }
        else
            speedIncri = 1.0f;

        player.GetComponent<Rigidbody>().isKinematic = true;

        player.GetComponent<ObjectGlide>().SetEventSys(GameRoot3D.moveEvents);
        player.GetComponent<ObjectGlide>().glideObject = ropeEnd;
        player.GetComponent<ObjectGlide>().SetArrivedAtDistance(1.0f);
        GameRoot3D.moveEvents.AddListener(PlayerGlideMove);
        player.GetComponent<ObjectGlide>().GlideAndLookMovingTarget(ropeEnd, goalPoint, goalPoint, 0.0f, 0.5f * speedIncri, 0.0f);


    }

    private void PlayerGlideMove()
    {
        gunSmoke.SetActive(false);
        muzzleFlash.SetActive(false);
        GameRoot3D.moveEvents.RemoveAllListeners();

        if (rightAnswer)
        {
            playerglide = true;
            failGrapple = false;

            Physics.gravity = Vector3.zero;

            player.GetComponent<ThirdPersonUserControl>().enableUnstuck = false;
            player.GetComponent<Rigidbody>().useGravity = false;
            player.GetComponent<Collider>().enabled = false;

            player.GetComponent<Animator>().Play("Airborne");

            player.GetComponent<ObjectGlide>().SetEventSys(GameRoot3D.moveEvents);
            player.GetComponent<ObjectGlide>().glideObject = player;
            player.GetComponent<ObjectGlide>().SetObjectMoveSpeed(0.4f * speedIncri);
            player.GetComponent<ObjectGlide>().SetArrivedAtDistance(2.0f);
            GameRoot3D.moveEvents.AddListener(EndOfPlayerGlide);
            player.GetComponent<ObjectGlide>().GlideAndLookMovingTarget(player, PC04.goalPoint, player, 0.0f, 0.4f * speedIncri, 0.0f);
            PC04.OnPortalExit();

        }
        else
        {
            grappleAnimation = false;
            failGrapple = true;
            failTrail.gameObject.SetActive(true);
            ropeEnd.GetComponent<SphereCollider>().enabled = true;
            ropeEnd.GetComponent<Rigidbody>().isKinematic = false;
            ropeEnd.GetComponent<Rigidbody>().AddForce(dir2Target.normalized * 10);            

            ropeLine.enabled = false;
        }
    }

    private void EndOfPlayerGlide()
    {
        trail.Clear();

        ropeEnd.SetActive(false);
        grappleAnimation = false;
        playerglide = false;
        GameRoot3D.moveEvents.RemoveAllListeners();
        GameRoot3D.moveEvents.AddListener(ReleaseGrapple);
        player.GetComponent<Rigidbody>().isKinematic = false;

        player.GetComponent<Collider>().enabled = true;
        GameRoot3D.instance.LockPlayer(false);

        player.GetComponent<ObjectGlide>().GlideAndLookMovingTarget(player, PC04.landPoint, player, arcHeight, 0.4f * speedIncri, 0.0f);
        this.gameObject.SetActive(false);
    }

    public void SetDemoTargetPos(Vector3 pos)
    {
        ropeEnd.GetComponent<SphereCollider>().enabled = false;
        failGrapple = false;
        failFall = false;
        DemoAnim = true;
        grappleAnimation = false;

        this.enabled = true;
        ropeEnd.transform.position = GameControllerPuzzle04.instance.currPuzzle.startPoint.transform.position;
        hook.transform.position = ropeEnd.transform.position;

        demoTarget.transform.position = pos;
        dir2Target = demoTarget.transform.position - ropeEnd.transform.position;
        ropeEnd.transform.LookAt(demoTarget.transform.position, Vector3.up);
        ropeEnd.SetActive(true);
        ropeLine.enabled = false;
        trail.gameObject.SetActive(true);
        trail.Clear();
    }

    public void SetFailShot(Vector3 pos, Puzzle04Controller controller)
    {
        demoTarget.transform.position = pos;
        PC04 = controller;
        GameControllerPuzzle04.instance.currPuzzle.turnOffVisualVectors();

        ropeEnd.GetComponent<SphereCollider>().enabled = false;
        failGrapple = true;
        failFall = false;
        DemoAnim = false;
        grappleAnimation = false;

        GameControllerPuzzle04.instance.GetAudioService().PlayFXAudio("Puzzle04/grappleShot");
        muzzleFlash.SetActive(true);
        muzzleFlash.transform.position = charHand.transform.position;
        muzzleFlash.transform.LookAt(demoTarget.transform.position, Vector3.up);
        muzzleFlash.GetComponent<ParticleSystem>().Play();


        this.enabled = true;
        ropeEnd.transform.position = GameControllerPuzzle04.instance.currPuzzle.startPoint.transform.position;
        hook.transform.position = ropeEnd.transform.position;

        dir2Target = demoTarget.transform.position - ropeEnd.transform.position;
        ropeEnd.transform.LookAt(demoTarget.transform.position, Vector3.up);
        ropeEnd.SetActive(true);
        ropeLine.enabled = false;
        failTrail.gameObject.SetActive(true);
        trail.gameObject.SetActive(false);
        failTrail.Clear();
        trail.Clear();

    }

    public void ReleaseGrapple()
    {
        GameRoot3D.moveEvents.RemoveAllListeners();
        player.GetComponent<ObjectGlide>().enabled = false;
        PC04.ToggleAllTriggers(true);
        player.GetComponent<ThirdPersonUserControl>().enableUnstuck = true;
        //reset the player velocity if they had any
   //     player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //this force pushes the player up and beind the goal anchor
   //     player.GetComponent<Rigidbody>().AddForce(goalPoint.transform.TransformDirection(Vector3.right) * 25f + goalPoint.transform.TransformDirection(Vector3.up) * 10f, ForceMode.VelocityChange);

        Physics.gravity = new Vector3(0, -9.81f, 0);
        player.GetComponent<Rigidbody>().useGravity = true;

        //    player.GetComponent<Collider>().enabled = true;
        GameRoot3D.instance.LockPlayer(false);

        ropeStart.transform.localPosition = Vector3.zero;
        ropeEnd.transform.localPosition = Vector3.zero;
        failGrapple = false;

        grappleAnimation = false;
        this.enabled = false;
    }

    public bool isGrappling()
    {
        return grappleAnimation;
    }
}
