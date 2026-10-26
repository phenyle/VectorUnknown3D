using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GapTriggersController : MonoBehaviour
{
    Puzzle04Controller PC04;

    public bool reset = false;
    public bool finish = false;
    public bool fall = false;


    private List<ResetTrigger> resetTrig;
    private List<FinishTrigger> finishTrig;
    private List<FallTrigger> fallTrig;


    public void Start()
    {
        resetTrig = new List<ResetTrigger>();
        finishTrig = new List<FinishTrigger>();
        fallTrig = new List<FallTrigger>();


        foreach(Transform eachChild in this.transform)
        {
            if (eachChild.GetComponent<ResetTrigger>() != null)
            {
                eachChild.GetComponent<ResetTrigger>().SetPuzzleController(PC04);
                resetTrig.Add(eachChild.GetComponent<ResetTrigger>());
            }
            if (eachChild.GetComponent<FinishTrigger>() != null)
            {
                eachChild.GetComponent<FinishTrigger>().SetPuzzleController(PC04);
                finishTrig.Add(eachChild.GetComponent<FinishTrigger>());
            }
            if(eachChild.GetComponent<FallTrigger>() != null)
            {
                eachChild.GetComponent<FallTrigger>().SetPuzzleController(PC04);
                fallTrig.Add(eachChild.GetComponent<FallTrigger>());
            }
        }
    }

    public void ToggleResetTriggers(bool value)
    {
        foreach (ResetTrigger trig in resetTrig)
            trig.enabled = value;
    }

    public void ToggleFinishTriggers(bool value)
    {
        foreach (FinishTrigger trig in finishTrig)
            trig.enabled = value;
    }

    public void ToggleFallTriggers(bool value)
    {
        foreach (FallTrigger trig in fallTrig)
            trig.enabled = value;
    }

    public void SetPuzzleController(Puzzle04Controller controller)
    {
        PC04 = controller;
    }
}

