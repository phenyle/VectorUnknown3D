using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class ObjBetweenPoints : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public GameObject objBetween;
    public float width;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VectorBetweenPoints(objBetween, point1.transform.position, point2.transform.position, width);
    }

    private void VectorBetweenPoints(GameObject visVector, Vector3 begin, Vector3 end, float width)
    {
        Vector3 offset = end - begin;
        Vector3 localScale = new Vector3(width, (offset.magnitude / 2.0f), width);
        Vector3 position = begin + (offset / 2.0f);

        visVector.transform.up = offset;
        visVector.transform.position = position;
        visVector.transform.localScale = localScale;
    }
}
