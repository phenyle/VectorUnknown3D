using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceObject : MonoBehaviour
{
    public GameObject target;
    private Vector3 axis;


    // Start is called before the first frame update
    void Start()
    {
        if(target == null)
            this.enabled = false;

        axis = -Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(target.transform.position, axis);
    }

    public void SetTarget(GameObject obj)
    {
        target = obj;
        this.enabled = true;
    }

    public void SetTarget(Vector3 vect)
    {
        target.transform.position = vect;
        this.enabled = true;
    }

    public void SetRotationAxis(Vector3 vect)
    {
        axis = vect;
    }
}
