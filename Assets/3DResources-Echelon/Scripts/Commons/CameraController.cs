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
    public bool enableMovement = true;

    //public bool isLock = false;


    public bool isLock = false;
    private Vector3 rotationSmoothVelocity, currentRotation;
    public float horizontal, vertical;

    public bool postPuzzleLock = false;

    private void Start()
    {
        currentRotation = transform.rotation.eulerAngles;

        horizontal = transform.rotation.eulerAngles.y;
        vertical = transform.rotation.eulerAngles.x;

        isLock = false;
        postPuzzleLock = false;
        enableMovement = true;
        rotatedSpeed = GameRoot.player.users.mouseSense;
    }

    private void LateUpdate()
    {
        if (!isLock)
        {
            MouseMovement();
            CameraMovement();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            if ((DialogueManager.isInDialogue || DialogueManager.isPuzzleLock || GameRoot.isPuzzleLock) && postPuzzleLock == false)
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
            horizontal -= Time.deltaTime * keyboardRotatedSpeed * rotatedSpeed;
        }
        if(Input.GetKey(KeyCode.L))
        {
            horizontal += Time.deltaTime * keyboardRotatedSpeed * rotatedSpeed;
        }
        if(Input.GetKey(KeyCode.I))
        {
            vertical -= Time.deltaTime * keyboardRotatedSpeed * rotatedSpeed;
        }
        if(Input.GetKey(KeyCode.K))
        {
            vertical += Time.deltaTime * keyboardRotatedSpeed * rotatedSpeed;
        }


        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(vertical, horizontal), ref rotationSmoothVelocity, rotatedSmoothTime);
        this.transform.eulerAngles = currentRotation;


        this.transform.position = target.position - this.transform.forward * dstFromTarget;
    }

    private void toggleMouse()
    {
        if ((!DialogueManager.isInDialogue || !DialogueManager.isPuzzleLock || !GameRoot.isPuzzleLock) && postPuzzleLock == true)
            isLock = !isLock;

        if (isLock)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetRotateSpeed(float val)
    {
        rotatedSpeed = val;
    }

}
