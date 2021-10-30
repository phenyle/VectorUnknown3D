using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceObject : MonoBehaviour
{
    public GameObject target;
    [Range(0,3)]
    [Tooltip("0: off \n1: lock x \n2: lock y \n3: lock z")]
    public int lockAxis = 0;
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
        switch(lockAxis)
        {
            case 0:
                this.transform.LookAt(target.transform.position, axis);
                break;
            case 1: //lock x
                this.transform.LookAt(new Vector3(target.transform.position.x, this.gameObject.transform.position.y, target.transform.position.z), axis);
                break;
            case 2: //lock y
                this.transform.LookAt(new Vector3(this.gameObject.transform.position.x, target.transform.position.y, target.transform.position.z), axis);

                break;
            case 3:
                this.transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y, this.gameObject.transform.position.z), axis);

                break;
        }
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
