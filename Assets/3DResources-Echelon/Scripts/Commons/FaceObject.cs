using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceObject : MonoBehaviour
{
    [SerializeField] private Transform target;

    [HideInInspector]
    public enum LockAxis { None, X, Y }
    [SerializeField] private LockAxis lockAxis;

    // Start is called before the first frame update
    void Start()
    {
        if(target == null)
            this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            this.enabled = false;
            return;
        }

        this.transform.LookAt(target);
        switch (lockAxis)
        {
            case LockAxis.None:
                break;

            case LockAxis.X: //lock x
                transform.localRotation = new Quaternion(0, transform.localRotation.y, 0, transform.localRotation.w);
                break;

            case LockAxis.Y: //lock y
                transform.localRotation = new Quaternion(0, transform.localRotation.y, transform.localRotation.z, 0);
                break;

            default:
                break;
        }
        transform.localRotation *= Quaternion.Euler(new Vector3(0, -180, 0));
    }

    public void SetTarget(Transform obj)
    {
        target = obj;
        this.enabled = true;
    }

    public void LockRotationAxis(LockAxis axis)
    {
        lockAxis = axis;
    }
}