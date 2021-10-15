using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private Puzzle04Controller PC04;
    private Vector3 resetPos;

    public void Awake()
    {
        this.tag = "finTrig";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !PC04.grappleKit.isGrappling())
        {
            GetComponentInParent<GapTriggersController>().finish = true;
            resetPos = GetComponentInParent<GapTriggersController>().GetComponentInParent<Puzzle04Controller>().finish.transform.position;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle04>().setResetPos(resetPos);

            Debug.Log("finish trigger");
        }
    }

    public void SetPuzzleController(Puzzle04Controller controller)
    {
        PC04 = controller;
    }
}
