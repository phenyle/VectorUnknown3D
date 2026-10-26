using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class enables the users to drag the camera around a focal point
/// Enable the class when needed, it will attempt to automatically disable any other
/// camera controllers attached, they will be reenabled with this class is disabled.
/// </summary>
public class BunnyMouseCamera : MonoBehaviour
{
    public Transform rotCamera;
    public Transform[] cameraTarget;
    public int targetIndex = 0;
    public float rotateSpeed = 1.25f;
    public float zoomSpeed = 1f;
    public float maxZoomOut = 175.0f;

    private float distanceToTarget = 10;
    private Vector3 previousPosition;


    void Start()
    {
        distanceToTarget = (this.transform.position - cameraTarget[targetIndex].position).magnitude;
        //       this.enabled = false;
    }


    public void OnEnable()
    {
        distanceToTarget = (rotCamera.position - cameraTarget[targetIndex].position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        //Mouse Camera Movement and Zoom Controls



        if (Input.GetMouseButtonDown(1))
            previousPosition = rotCamera.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
        else if (Input.GetMouseButton(1))
        {
            distanceToTarget = (rotCamera.position - cameraTarget[targetIndex].position).magnitude;
            MouseRotate();
        }


        if (Input.GetAxis("Mouse ScrollWheel") > 0f && (rotCamera.position - cameraTarget[targetIndex].position).magnitude > 5)
        {
            //rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            rotCamera.Translate(new Vector3(0, 0, zoomSpeed * 5));
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && (rotCamera.position - cameraTarget[targetIndex].position).magnitude < maxZoomOut)
        {
            //rotCamera.position -= Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            rotCamera.Translate(new Vector3(0, 0, -zoomSpeed * 5));
        }

        //Toggle Target
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            targetIndex++;
            do { 
                if (cameraTarget[targetIndex] == null) 
                    targetIndex++;
                if (targetIndex > cameraTarget.Length - 1)
                    targetIndex = 0;
            } while (cameraTarget[targetIndex] == null);

            rotCamera.LookAt(cameraTarget[targetIndex]);
        }

        //Keyboard Camera Movement and Zoom Controls
        if (Input.GetKey(KeyCode.J))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.up, rotateSpeed * 0.5f);
            distanceToTarget = (rotCamera.position - cameraTarget[targetIndex].position).magnitude;
            KeyboardRotate(Vector3.left * rotateSpeed * 0.005f);
        }

        if (Input.GetKey(KeyCode.L))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.up, -rotateSpeed * 0.5f);
            distanceToTarget = (rotCamera.position - cameraTarget[targetIndex].position).magnitude;
            KeyboardRotate(Vector3.right * rotateSpeed * 0.005f);
        }

        if (Input.GetKey(KeyCode.I))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.left, -rotateSpeed * 0.5f);
            distanceToTarget = (rotCamera.position - cameraTarget[targetIndex].position).magnitude;
            KeyboardRotate(Vector3.up * rotateSpeed * 0.005f);
        }

        if (Input.GetKey(KeyCode.K))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.left, rotateSpeed * 0.5f);
            distanceToTarget = (rotCamera.position - cameraTarget[targetIndex].position).magnitude;
            KeyboardRotate(Vector3.down * rotateSpeed * 0.005f);
        }

        if (Input.GetKey(KeyCode.U) && (rotCamera.position - cameraTarget[targetIndex].position).magnitude > 5)
        {
            rotCamera.Translate(new Vector3(0, 0, zoomSpeed));
        }

        if (Input.GetKey(KeyCode.O) && (rotCamera.position - cameraTarget[targetIndex].position).magnitude < 150)
        {
            rotCamera.Translate(new Vector3(0, 0, -zoomSpeed));
        }
    }

    private void RotateCamera()
    {
        Debug.Log("rotating camera");
        rotCamera.LookAt(cameraTarget[targetIndex]);
        rotCamera.RotateAround(cameraTarget[targetIndex].position, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
        rotCamera.RotateAround(cameraTarget[targetIndex].position, Vector3.left, Input.GetAxis("Mouse Y") * rotateSpeed);

    }

    private void KeyboardRotate(Vector3 position)
    {
        Vector3 newPosition = rotCamera.position + position;

        float rotationAroundYAxis = -position.x * 180; // camera moves horizontally
        float rotationAroundXAxis = position.y * 180; // camera moves vertically

        rotCamera.position = cameraTarget[targetIndex].position;
        rotCamera.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis * rotateSpeed);
        rotCamera.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis * rotateSpeed, Space.World); // <— This is what makes it work!

        rotCamera.Translate(new Vector3(0, 0, -distanceToTarget));

        previousPosition = newPosition;
    }


    private void MouseRotate()
    {
        Vector3 newPosition = rotCamera.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
        Vector3 direction = previousPosition - newPosition;

        float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
        float rotationAroundXAxis = direction.y * 180; // camera moves vertically

        rotCamera.position = cameraTarget[targetIndex].position;
        rotCamera.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis * rotateSpeed);
        rotCamera.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis * rotateSpeed, Space.World); // <— This is what makes it work!

        rotCamera.Translate(new Vector3(0, 0, -distanceToTarget));

        previousPosition = newPosition;
    }

    public void SetRotCamera(Transform camera)
    {
        rotCamera = camera;

    }
    public void SetMaxZoomOut(float val)
    {
        maxZoomOut = val;
    }
}
