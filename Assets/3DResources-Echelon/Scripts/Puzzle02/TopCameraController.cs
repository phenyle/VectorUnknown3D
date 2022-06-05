using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCameraController : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;
    public Transform panCamera;
    public Transform subCamera;
    public float panSpeed = 1.25f;
    public float zoomSpeed = 1.5f;
    public float maxCamSize = 60.0f;
    public float minCamSize = 10.0f;

    private Vector2 previousPosition;


    // Start is called before the first frame update
    void Start()
    {
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();

        panCamera.position = new Vector3(0, 50, 55);
        subCamera.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GCP02.topViewOn)
            return;

        if (Input.GetMouseButtonDown(1))
            previousPosition = Input.mousePosition;
        else if (Input.GetMouseButton(1))
        {
            PanCamera();
        }


        if (Input.GetAxis("Mouse ScrollWheel") > 0f && panCamera.GetComponent<Camera>().orthographicSize > minCamSize)
        {
            //rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            panCamera.GetComponent<Camera>().orthographicSize -= zoomSpeed;
            if(panCamera.GetComponent<Camera>().orthographicSize < minCamSize)
                panCamera.GetComponent<Camera>().orthographicSize = minCamSize;            
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && panCamera.GetComponent<Camera>().orthographicSize < maxCamSize)
        {
            //rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            panCamera.GetComponent<Camera>().orthographicSize += zoomSpeed;
            if (panCamera.GetComponent<Camera>().orthographicSize > maxCamSize)
                panCamera.GetComponent<Camera>().orthographicSize = maxCamSize;
        }

        subCamera.GetComponent<Camera>().orthographicSize = panCamera.GetComponent<Camera>().orthographicSize;
    }

    private void PanCamera()
    {
        Vector2 currPosition = Input.mousePosition;
        Vector2 direction = (currPosition - previousPosition).normalized * panSpeed;
        panCamera.position += new Vector3(direction.x, 0, direction.y);
        //panCamera.Translate(delta.x * panSpeed, 0, delta.y * panSpeed);
        //previousPosition = currPosition;
    }
}
