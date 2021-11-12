using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FallTrigger : MonoBehaviour
{
    private GameControllerPuzzle04 GCP04;
    private Puzzle04Controller PC04;

    public void Start()
    {
        GCP04 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle04>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !PC04.grappleKit.isGrappling())
        {
            Debug.Log("fall trigger");

            GetComponentInParent<GapTriggersController>().fall = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !PC04.grappleKit.isGrappling())
        {

            GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle04>().getResetPos();

            GetComponentInParent<GapTriggersController>().fall = false;
        }

    }

    public void SetPuzzleController(Puzzle04Controller controller)
    {
        PC04 = controller;
    }

}
