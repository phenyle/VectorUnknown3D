using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDragSurface : EventTrigger
{
    private bool cameraDrag;

    public void Start()
    {
        cameraDrag = false;
    }

    public void Update()
    {
        //if Right-Click is down
        if (Input.GetMouseButton(1))
        {
            Debug.Log("background click down");
            cameraDrag = true;
        }
        else
            cameraDrag = false;
    }

    public bool isCameraDragging()
    {
        return cameraDrag;
    }

}
