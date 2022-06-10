using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCameraController : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;
    public Transform panCamera;
    public Transform subCamera;
    public float panSpeed = 1.25f;
    public float zoomSpeed = 2.75f;
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


        if (Input.GetMouseButton(1))
        {
            MousePan();
        }
        //Keyboard Pans
        if (Input.GetKey(KeyCode.I) && panCamera.position.z < 100)
            panCamera.Translate(new Vector3(0, panSpeed, 0));
        if (Input.GetKey(KeyCode.K) && panCamera.position.z > 10)
            panCamera.Translate(new Vector3(0, -panSpeed, 0));

        if (Input.GetKey(KeyCode.L) && panCamera.position.x < 75)
            panCamera.Translate(new Vector3(panSpeed, 0, 0));
        if (Input.GetKey(KeyCode.J) && panCamera.position.x > -75)
            panCamera.Translate(new Vector3(-panSpeed, 0, 0));


        if ((Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKey(KeyCode.U)) && panCamera.GetComponent<Camera>().orthographicSize > minCamSize)
        {
            //rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            panCamera.GetComponent<Camera>().orthographicSize -= zoomSpeed;
            if(panCamera.GetComponent<Camera>().orthographicSize < minCamSize)
                panCamera.GetComponent<Camera>().orthographicSize = minCamSize;            
        }

        if ((Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKey(KeyCode.O)) && panCamera.GetComponent<Camera>().orthographicSize < maxCamSize)
        {
            //rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            panCamera.GetComponent<Camera>().orthographicSize += zoomSpeed;
            if (panCamera.GetComponent<Camera>().orthographicSize > maxCamSize)
                panCamera.GetComponent<Camera>().orthographicSize = maxCamSize;
        }

        subCamera.GetComponent<Camera>().orthographicSize = panCamera.GetComponent<Camera>().orthographicSize;
    }

    private void MousePan()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        panCamera.Translate(new Vector3(-x, -y, 0) * panSpeed);

        if (panCamera.position.x < -75)
            panCamera.position = new Vector3(-75, panCamera.position.y, panCamera.position.z);
        else if (panCamera.position.x > 75)
            panCamera.position = new Vector3(75, panCamera.position.y, panCamera.position.z);

        if (panCamera.position.z < 10)
            panCamera.position = new Vector3(panCamera.position.x, panCamera.position.y, 10);
        else if (panCamera.position.z > 100)
            panCamera.position = new Vector3(panCamera.position.x, panCamera.position.y, 100);

        subCamera = panCamera;

        //Vector2 currPosition = Input.mousePosition;
        //Vector2 direction = (currPosition - previousPosition).normalized * panSpeed;
        //panCamera.position += new Vector3(direction.x, 0, direction.y);
        //panCamera.Translate(delta.x * panSpeed, 0, delta.y * panSpeed);
        //previousPosition = currPosition;
    }
}
