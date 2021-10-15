using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    public float rotatedSpeed = 1f;

    private void LateUpdate()
    {
        transform.Rotate(new Vector3(0, -1f, 0) * Time.deltaTime * rotatedSpeed);
    }
}
