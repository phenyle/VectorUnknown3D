using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float rotatedSpeed = 5.0f;
    public float rotatedSmoothTime = 0.12f;
    public float dstFromTarget = 3f;
    public Vector2 verticalMinMax = new Vector2(-30, 60);
    public float keyboardRotatedSpeed = 50.0f;

    [HideInInspector]
    //public bool isLock = false;
    public bool isLock = true;
    private Vector3 rotationSmoothVelocity, currentRotation;
    public float horizontal, vertical;

    public bool postPuzzleLock = false;
    private bool puzzle04hack = false;


    private void Start()
    {
        currentRotation = transform.rotation.eulerAngles;

        horizontal = transform.rotation.eulerAngles.y;
        vertical = transform.rotation.eulerAngles.x;

        isLock = true;
        postPuzzleLock = false;
        rotatedSpeed = GameRoot.player.users.mouseSense;
    }

    private void LateUpdate()
    {
        if (isLock)
        {
            MouseMovement();
        }
        CameraMovement();
        if(!puzzle04hack)
            toggleMouse();
    }

    public void MouseMovement()
    {
        horizontal += Input.GetAxis("Mouse X") * rotatedSpeed;
        vertical -= Input.GetAxis("Mouse Y") * rotatedSpeed;
        vertical = Mathf.Clamp(vertical, verticalMinMax.x, verticalMinMax.y);
    }

    private void CameraMovement()
    {
        if(Input.GetKey(KeyCode.J))
        {
            horizontal -= Time.deltaTime * keyboardRotatedSpeed;
        }
        if(Input.GetKey(KeyCode.L))
        {
            horizontal += Time.deltaTime * keyboardRotatedSpeed;
        }
        if(Input.GetKey(KeyCode.I))
        {
            vertical -= Time.deltaTime * keyboardRotatedSpeed;
        }
        if(Input.GetKey(KeyCode.K))
        {
            vertical += Time.deltaTime * keyboardRotatedSpeed;
        }


        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(vertical, horizontal), ref rotationSmoothVelocity, rotatedSmoothTime);
        this.transform.eulerAngles = currentRotation;


        this.transform.position = target.position - this.transform.forward * dstFromTarget;
    }

    private void toggleMouse()
    {
        // press tab to toggle the mouse to interact with games
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("mouse hidden:" + isLock);
            isLock = !isLock;
            if(SceneManager.GetActiveScene().name == Constants.puzzle03SceneName)
                GameRoot.isPuzzleLock = !GameRoot.isPuzzleLock;
        }

        //if (DialogueManager.isPuzzleLock && newScene == true)
        //{
        //    isLock = false;
        //    newScene = false;
        //}

        if ((DialogueManager.isInDialogue || DialogueManager.isPuzzleLock || GameRoot.isPuzzleLock) && postPuzzleLock == false)
        {
            //Debug.Log("free mouse");
            GameRoot.instance.IsLock(true);
            isLock = false;          
        }

        if (isLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetRotateSpeed(float val)
    {
        rotatedSpeed = val;
    }

    public void SetPuzzle04Hack(bool val)
    {
        puzzle04hack = val;
    }
}
