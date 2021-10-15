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
    public float zoomSpeed = 2.0f;
    public float maxZoomOut = 150.0f;
    private CameraDragSurface dragController;


    void Start()
    {
        dragController = GameObject.FindGameObjectWithTag("cameraTools").GetComponent<CameraDragSurface>();
        rotCamera = GameObject.FindGameObjectWithTag("PuzzleCamera").transform;
 //       this.enabled = false;
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
        if (dragController.isCameraDragging())
        {
            RotateCamera();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && (rotCamera.position - cameraTarget).magnitude > 5)
        {
            rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && (rotCamera.position - cameraTarget).magnitude < maxZoomOut)
        {
            rotCamera.position -= Vector3.Normalize(rotCamera.forward) * zoomSpeed;
        }

        //Keyboard Camera Movement and Zoom Controls
        if (Input.GetKey(KeyCode.J))
        {
            rotCamera.LookAt(cameraTarget);
            rotCamera.RotateAround(cameraTarget, Vector3.up, rotateSpeed * 0.5f);
        }

        if (Input.GetKey(KeyCode.L))
        {
            rotCamera.LookAt(cameraTarget);
            rotCamera.RotateAround(cameraTarget, Vector3.up, -rotateSpeed * 0.5f);
        }

        if (Input.GetKey(KeyCode.I))
        {
            rotCamera.LookAt(cameraTarget);
            rotCamera.RotateAround(cameraTarget, Vector3.left, -rotateSpeed * 0.5f);
        }

        if (Input.GetKey(KeyCode.K))
        {
            rotCamera.LookAt(cameraTarget);
            rotCamera.RotateAround(cameraTarget, Vector3.left, rotateSpeed * 0.5f);
        }

        if (Input.GetKey(KeyCode.U) && (rotCamera.position - cameraTarget).magnitude > 5)
        {
            rotCamera.position += Vector3.Normalize(rotCamera.forward) * zoomSpeed * 0.25f;
        }

        if (Input.GetKey(KeyCode.O) && (rotCamera.position - cameraTarget).magnitude < 150)
        {
            rotCamera.position -= Vector3.Normalize(rotCamera.forward) * zoomSpeed * 0.25f;
        } 
    }

    private void RotateCamera()
    {
        Debug.Log("rotating camera");
        rotCamera.LookAt(cameraTarget);
        rotCamera.RotateAround(cameraTarget, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
        rotCamera.RotateAround(cameraTarget, Vector3.left, Input.GetAxis("Mouse Y") * rotateSpeed);

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
