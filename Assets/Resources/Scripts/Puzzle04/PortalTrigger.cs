using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PortalTrigger : MonoBehaviour
{
    public GameObject player;
    public bool inPortal = false;
    public Puzzle04Controller PC04;

    public bool exiting;
    private float timeOutMax = 1.0f;
    private float currTime = 0.0f;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PC04 = this.GetComponentInParent<Puzzle04Controller>();
    }

    public void Update()
    {
        if (exiting)
        {
            currTime += Time.deltaTime;
            if (currTime > timeOutMax)
            {
                this.GetComponent<Collider>().enabled = true;
                if (currTime > timeOutMax + 0.2f)
                    exiting = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (exiting)
                alarmCollider();
            else
                PC04.OnPortalEnter();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && GameControllerPuzzle04.instance.grappleKit.isGrappling())
        {
            alarmCollider();
            PC04.OnPortalExit();
        }
    }

    public void alarmCollider()
    {
        this.GetComponent<Collider>().enabled = false;
        currTime = 0.0f;
        exiting = true;
    }


    public void SetPuzzleController(Puzzle04Controller controller)
    {
        PC04 = controller;
    }
}
