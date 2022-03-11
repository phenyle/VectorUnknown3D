using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseGrappleTrigger : MonoBehaviour
{
    Puzzle04Controller PC04;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && GameControllerPuzzle04.instance.grappleKit.isGrappling())
        {
            GameControllerPuzzle04.instance.grappleKit.ReleaseGrapple();
        }
    }

    public void SetPuzzleController(Puzzle04Controller controller)
    {
        PC04 = controller;
    }
}
