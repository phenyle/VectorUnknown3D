using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Audio;
using UnityEngine;

public class GrappleCode : MonoBehaviour
{
    [Header("Rope Parts")]
    public GameObject player;
    public GameObject rope;
    public GameObject hook;
    public GameObject ropeStart;
    public GameObject ropeEnd;
    public GameObject goalPoint;
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


    private bool grappleAnimation;
    private Puzzle04Controller PC04;
    private bool rightAnswer;
    private bool playerglide = false;


    public Transform prevShoulderRotation;


    // Start is called before the first frame update
    void Start()
    {
        rope.SetActive(false);
        ropeStart.transform.position = charHand.transform.position;
        ropeEnd.transform.position = charHand.transform.position;
        hook.SetActive(false);
        gunSmoke.SetActive(false);
        hook.transform.position = ropeEnd.transform.position;
        grappleAnimation = false;
        prevShoulderRotation = charShoulder.transform;
        prevShoulderRotation.rotation = charShoulder.transform.rotation;
        hookSpeed = 1.5f;
        ropeWidth = 0.35f;
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
            rope.SetActive(true);
            hook.SetActive(true);
            player.transform.LookAt(new Vector3(goalPoint.transform.position.x, player.transform.position.y, goalPoint.transform.position.z));

            VectorBetweenPoints(rope, charHand.transform.position, ropeEnd.transform.position, ropeWidth);
            if (playerglide)
            {
                ropeEnd.transform.position = goalPoint.transform.position;
            }
        }        
    }

    private void VectorBetweenPoints(GameObject visVector, Vector3 begin, Vector3 end, float width)
    {
        Vector3 offset = end - begin;
        Vector3 localScale = new Vector3(width, (offset.magnitude / 2.0f), width);
        Vector3 position = begin + (offset / 2.0f);

        visVector.transform.up = offset;
        visVector.transform.position = position;
        visVector.transform.localScale = localScale;
    }

    public void InitGrapple(bool correct, Vector3 direction, GameObject goal, Vector3 playerStartPos, float arc)
    {
        goalPoint = goal;
        rightAnswer = correct;
        hookDirection = direction;
        startPos = playerStartPos;
        arcHeight = arc;


   //     ropeStart.transform.position = charHand.transform.localPosition;
    }


    public void grappleToGoal(Puzzle04Controller puzzleController)
    {
        PC04 = puzzleController;
        ropeStart.transform.position = charHand.transform.position;
        ropeEnd.transform.position = charHand.transform.position;

        PC04.getGameController().GetAudioService().PlayFXAudio("Puzzle04/grappleShot");

        GameRoot.instance.LockPlayer(true);

        grappleAnimation = true;
        this.enabled = true;

        if (PC04.getGameController().Difficulty == 3)
        {
            speedIncri = 1.3f;
        }
        else
            speedIncri = 1.0f;

        player.GetComponent<Rigidbody>().isKinematic = true;

        player.GetComponent<ObjectGlide>().SetEventSys(GameRoot.moveEvents);
        player.GetComponent<ObjectGlide>().glideObject = ropeEnd;
        player.GetComponent<ObjectGlide>().SetObjectMoveSpeed(0.5f * speedIncri);
        player.GetComponent<ObjectGlide>().SetArrivedAtDistaince(1.0f);
        GameRoot.moveEvents.AddListener(PlayerGlideMove);
        player.GetComponent<ObjectGlide>().GlideAndLookMovingTarget(ropeEnd, PC04.goalPoint, PC04.goalPoint, ropeEnd.transform.position, 0.0f);


    }

    private void PlayerGlideMove()
    {
        gunSmoke.SetActive(false);
        GameRoot.moveEvents.RemoveAllListeners();

        if (rightAnswer)
        {
            PC04.OnPortalExit();
            playerglide = true;

            Physics.gravity = Vector3.zero;

            player.GetComponent<ThirdPersonUserControl>().enableUnstuck = false;
            player.GetComponent<Rigidbody>().useGravity = false;
            player.GetComponent<Collider>().enabled = false;

            player.GetComponent<Animator>().Play("Airborne");

            player.GetComponent<ObjectGlide>().SetEventSys(GameRoot.moveEvents);
            player.GetComponent<ObjectGlide>().glideObject = player;
            player.GetComponent<ObjectGlide>().SetObjectMoveSpeed(0.4f * speedIncri);
            player.GetComponent<ObjectGlide>().SetArrivedAtDistaince(2.0f);
            GameRoot.moveEvents.AddListener(EndOfPlayerGlide);
            player.GetComponent<ObjectGlide>().GlideOnlyMovingTarget(player, PC04.goalPoint, player.transform.position, 0.0f);
        }
        else
        {
            rope.SetActive(false);
            hook.SetActive(false);
            grappleAnimation = false;
        }
    }

    private void EndOfPlayerGlide()
    {
        rope.SetActive(false);
        hook.SetActive(false);
        grappleAnimation = false;
        playerglide = false;
        GameRoot.moveEvents.RemoveAllListeners();
        GameRoot.moveEvents.AddListener(ReleaseGrapple);
        player.GetComponent<Rigidbody>().isKinematic = false;

        player.GetComponent<Collider>().enabled = true;
        GameRoot.instance.LockPlayer(false);

        player.GetComponent<ObjectGlide>().GlideOnlyMovingTarget(player, PC04.landPoint, player.transform.position, arcHeight);

    }


    public void ReleaseGrapple()
    {
        GameRoot.moveEvents.RemoveAllListeners();
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
        GameRoot.instance.LockPlayer(false);

        ropeStart.transform.localPosition = Vector3.zero;
        ropeEnd.transform.localPosition = Vector3.zero;

        grappleAnimation = false;
        this.enabled = false;
    }

    public bool isGrappling()
    {
        return grappleAnimation;
    }
}
