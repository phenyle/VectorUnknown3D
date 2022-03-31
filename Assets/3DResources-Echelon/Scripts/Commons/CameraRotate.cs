using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class enables the users to drag the camera around a focal point
/// Enable the class when needed, it will attempt to automatically disable any other
/// camera controllers attached, they will be reenabled with this class is disabled.
/// </summary>
public class CameraRotate : MonoBehaviour
{
    public Transform rotCamera;
    private Vector3 cameraTarget;
    public float rotateSpeed = 1.0f;
    public float zoomSpeed = 0.1f;
    public float maxZoomOut = 175.0f;
    private CameraDragSurface dragController;

    private float distanceToTarget = 10;
    private Vector3 previousPosition;


    void Start()
    {
        dragController = GameObject.FindGameObjectWithTag("cameraTools").GetComponent<CameraDragSurface>();
        rotCamera = GameObject.FindGameObjectWithTag("PuzzleCamera").transform;
        distanceToTarget = (this.transform.position - cameraTarget).magnitude;
 //       this.enabled = false;
    }


    public void OnEnable()
    {
        distanceToTarget = (rotCamera.position - cameraTarget).magnitude;
    }

    public void OnDisable()
    {
        if (rotCamera != null)
        {
            if (rotCamera.TryGetComponent(out Camera2DFollowMod cam2d))
            {
                cam2d.enabled = true;
            }
            if (rotCamera.TryGetComponent(out CameraController cam3d))
            {
                cam3d.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Mouse Camera Movement and Zoom Controls

        distanceToTarget = (rotCamera.position - cameraTarget).magnitude;


        if (Input.GetMouseButtonDown(1))
            previousPosition = rotCamera.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
        else if (Input.GetMouseButton(1))
            MouseRotate();


        if (Input.GetAxis("Mouse ScrollWheel") > 0f && (rotCamera.position - cameraTarget).magnitude > 5)
        {
            //rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            rotCamera.Translate(new Vector3(0, 0, zoomSpeed));
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && (rotCamera.position - cameraTarget).magnitude < maxZoomOut)
        {
            //rotCamera.position -= Vector3.Normalize(rotCamera.forward) * zoomSpeed;
            rotCamera.Translate(new Vector3(0, 0, -zoomSpeed));
        }

        //Keyboard Camera Movement and Zoom Controls
        if (Input.GetKey(KeyCode.J))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.up, rotateSpeed * 0.5f);
            KeyboardRotate(Vector3.left * GameRoot3D.player.users.mouseSense * 0.006f);
        }

        if (Input.GetKey(KeyCode.L))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.up, -rotateSpeed * 0.5f);
            KeyboardRotate(Vector3.right * GameRoot3D.player.users.mouseSense * 0.006f);
        }

        if (Input.GetKey(KeyCode.I))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.left, -rotateSpeed * 0.5f);
            KeyboardRotate(Vector3.up * GameRoot3D.player.users.mouseSense * 0.006f);
        }

        if (Input.GetKey(KeyCode.K))
        {
            //rotCamera.LookAt(cameraTarget);
            //rotCamera.RotateAround(cameraTarget, Vector3.left, rotateSpeed * 0.5f);
            KeyboardRotate(Vector3.down * GameRoot3D.player.users.mouseSense * 0.006f);
        }

        if (Input.GetKey(KeyCode.U) && (rotCamera.position - cameraTarget).magnitude > 5)
        {
            rotCamera.Translate(new Vector3(0, 0, GameRoot3D.player.users.mouseSense * 0.45f));
        }

        if (Input.GetKey(KeyCode.O) && (rotCamera.position - cameraTarget).magnitude < 150)
        {
            rotCamera.Translate(new Vector3(0, 0, -GameRoot3D.player.users.mouseSense * 0.45f));
        }
    }

    private void RotateCamera()
    {
        Debug.Log("rotating camera");
        rotCamera.LookAt(cameraTarget);
        rotCamera.RotateAround(cameraTarget, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
        rotCamera.RotateAround(cameraTarget, Vector3.left, Input.GetAxis("Mouse Y") * rotateSpeed);

    }

    private void KeyboardRotate(Vector3 position)
    {
        Vector3 newPosition = rotCamera.position + position;

        float rotationAroundYAxis = -position.x * 180; // camera moves horizontally
        float rotationAroundXAxis = position.y * 180; // camera moves vertically

        rotCamera.position = cameraTarget;

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

        rotCamera.position = cameraTarget;

        rotCamera.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis * rotateSpeed);
        rotCamera.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis * rotateSpeed, Space.World); // <— This is what makes it work!

        rotCamera.Translate(new Vector3(0, 0, -distanceToTarget));

        previousPosition = newPosition;
    }

    public void SetRotCamera(Transform camera)
    {
        rotCamera = camera;

    }

    public void SetCameraTarget(Vector3 target)
    {
        cameraTarget = target;
    }

    public void SetMaxZoomOut(float val)
    {
        maxZoomOut = val;
    }

    public void EnableCameraControls(GameObject camera, Vector3 target, bool val)
    {
        if (camera.TryGetComponent(out Camera2DFollowMod cam2d))
        {
            cam2d.enabled = false;
        }
        if (camera.TryGetComponent(out CameraController cam3d))
        {
            cam3d.enabled = false;
        }


        SetRotCamera(camera.transform);
        cameraTarget = target;
        this.enabled = true;
    }
}
