using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookTrigger : MonoBehaviour
{
    private bool hitObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "terrain")
        {
            hitObject = true;
            Debug.Log("hook hit object");
        }
    }

    public bool isHitObject()
    {
        return hitObject;
    }

    public void resetHitObject()
    {
        hitObject = false;
    }
}