using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleCam : MonoBehaviour
{
    public Transform mainCam, target;
    public float distance = 10.0f;
    public static bool activeTrack = false;

    // Start is called before the first frame update
    void Start()
    {
        activeTrack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(activeTrack)
        {
            gameObject.transform.position = mainCam.position.normalized *  distance;
            gameObject.transform.Translate(target.position, Space.World);
            transform.LookAt(target);
        }
    }
}
