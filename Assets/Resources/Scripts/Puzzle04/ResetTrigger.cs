using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrigger : MonoBehaviour
{
    private Puzzle04Controller PC04;

    private Vector3 resetPos;

    public void Awake()
    {
        this.tag = "resTrig";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !PC04.grappleKit.isGrappling())
        {
            resetPos = GetComponentInParent<GapTriggersController>().GetComponentInParent<Puzzle04Controller>().reset.transform.position;
            GetComponentInParent<GapTriggersController>().reset = true;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle04>().setResetPos(resetPos);
            Debug.Log("reset trigger");
        }
    }

    public void SetPuzzleController(Puzzle04Controller controller)
    {
        PC04 = controller;
    }

}
