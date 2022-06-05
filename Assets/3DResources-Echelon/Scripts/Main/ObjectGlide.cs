using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.ThirdPerson;

public class ObjectGlide : MonoBehaviour
{
    [Header("Glide Object")]
    public GameObject glideObject;
    private UnityEvent eventCalls;
    private bool hasEvents = false;
    private GameObject targetPos;
    private GameObject destinationPos;
    private Vector3 destOffset = Vector3.zero;
    private Vector3 startPos;
    private float destMagnitude;
    private float objDistMagnitude;
    private float distToTarget;
    private Vector3 halfwayPoint;
    private bool reachedDestination = false;
    private Vector3 offset;
    private float currDist;
    private bool glide = false;

    [Header("Glide Controls")]
    [Range(0.001f, 1.0f)]
    public float objectMoveSpeed = 0.1f;
    private float baseMoveSpeed = 0.1f;
    private float modMoveSpeed = 0.0f;
    private float catchUpMoveRate = 0.0f;
    private float catchUpspeed = 0.0f;
    [Range(0.001f, 0.5f)]
    public float objectRotateSpeed = 0.07f;
    [Range(0.0f, 50.0f)]
    public float arcHeight = 0.0f;
    [Range(0.0f, 5.0f)]
    public float arrivedAtDistance = 0.2f;
    public bool focusAtObject = false;
    private Vector3 arcTargetPos = Vector3.zero;
    private float startDistanceXY = 0.0f;


    void Awake()
    {
        this.enabled = false;
        targetPos = new GameObject();
        destinationPos = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (glide)
        {
            catchUpspeed += catchUpMoveRate;

            if (!isAtDestination(destinationPos.transform.position))
            {
                //if (arcHeight != 0)
                //    glideObject.transform.position = Vector3.MoveTowards(glideObject.transform.position, destinationPos.transform.position + CalcArcHeight(), objectMoveSpeed);
                //else
                //    glideObject.transform.position = Vector3.MoveTowards(glideObject.transform.position, destinationPos.transform.position, objectMoveSpeed);
                
                distToTarget = (destinationPos.transform.position - glideObject.transform.position).magnitude;

                float speed = baseMoveSpeed + (modMoveSpeed + catchUpspeed) * distToTarget;

                if (arcHeight != 0)
                    glideObject.transform.position = Vector3.MoveTowards(glideObject.transform.position, destinationPos.transform.position + CalcArcHeight(), speed);
                else
                    glideObject.transform.position = Vector3.MoveTowards(glideObject.transform.position, destinationPos.transform.position, speed);


                if (focusAtObject)
                    glideObject.transform.rotation = Quaternion.Slerp(glideObject.transform.rotation, Quaternion.LookRotation((targetPos.transform.position - glideObject.transform.position).normalized), objectRotateSpeed);
            }
        }
    }


    #region Glide and Rotate Functions

    public void GlideAndLookMovingTarget(GameObject gobject, GameObject newDestionation, GameObject target, float arc, float baseSpeed, float speedMod)
    {
        glideObject = gobject;
        SetDestinationPos(newDestionation);
        SetTargetPos(target);
        focusAtObject = true;
        baseMoveSpeed = baseSpeed;
        modMoveSpeed = speedMod;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        arcHeight = arc;
        arcTargetPos = new Vector3(0, arcHeight, 0);
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;
        catchUpMoveRate = 0;

        this.enabled = true;
    }

    public void SetCatchUpSpeedMod(float val)
    {
        catchUpMoveRate = val;
    }

    public void GlideAndLookMovingTarget(GameObject gobject, GameObject newDestionation, Vector3 Offset, GameObject target, float arc, float baseSpeed, float speedMod)
    {
        glideObject = gobject;
        SetDestinationPos(newDestionation);
        SetTargetPos(target);
        destOffset = Offset;
        focusAtObject = true;
        baseMoveSpeed = baseSpeed;
        modMoveSpeed = speedMod;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        arcHeight = arc;
        arcTargetPos = new Vector3(0, arcHeight, 0);
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;
        catchUpMoveRate = 0;
        this.enabled = true;
    }



    public void GlideAndLookMovingTarget(GameObject gobject, Vector3 newDestionation, GameObject target, Vector3 newStartPos, float arc)
    {
        glideObject = gobject;
        SetDestinationPos(newDestionation);
        SetTargetPos(target);
        SetStartPos(newStartPos);
        focusAtObject = true;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        arcHeight = arc;
        arcTargetPos = new Vector3(0, arcHeight, 0);
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;
        catchUpMoveRate = 0;
        this.enabled = true;
    }
    #endregion


    #region Glide Only Functions

    public void GlideOnly(GameObject gobject, Vector3 newDestination, Vector3 newStartPos)
    {

        glideObject = gobject;
        SetDestinationPos(newDestination);
        SetStartPos(newStartPos);
        focusAtObject = false;
        arcHeight = 0.0f;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;
        catchUpMoveRate = 0;
        this.enabled = true;
    }

    public void GlideOnly(GameObject gobject, Vector3 newDestionation, Vector3 newStartPos, float arc)
    {
        glideObject = gobject;
        SetDestinationPos(newDestionation);
        SetStartPos(newStartPos);
        focusAtObject = false;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        arcHeight = arc;
        arcTargetPos = new Vector3(0, arcHeight, 0);
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;
        catchUpMoveRate = 0;
        this.enabled = true;
    }

    #endregion

    public void SetEventSys(UnityEvent eventSys)
    {
        eventCalls = eventSys;
        hasEvents = true;
    }

    public void DisableEvents()
    {
        if (eventCalls != null)
            eventCalls.RemoveAllListeners();

        hasEvents = false;
    }

    public void SetStartPos(Vector3 newStartPos)
    {
        startPos = newStartPos;
    }

    public Vector3 GetStartPos()
    {
        return startPos;
    }

    public void SetDestinationPos(Vector3 newPos)
    {
        destinationPos.transform.position = newPos;
    }

    public void SetTargetPos(Vector3 newPos)
    {
        targetPos.transform.position = newPos;
    }

    public void SetDestinationPos(GameObject newPos)
    {
        destinationPos = newPos;
    }

    public void SetTargetPos(GameObject newPos)
    {
        targetPos = newPos;
    }

    public void SetObjectMoveSpeed(float val)
    {
        objectMoveSpeed = val;
    }

    public void SetMoveSpeeds(float baseSpeed, float modSpeed, float catchRate )
    {
        baseMoveSpeed = baseSpeed;
        modMoveSpeed = modSpeed;
        catchUpMoveRate = catchRate;
    }

    public void SetObjectRotateSpeed(float val)
    {
        objectRotateSpeed = val;
    }

    public void SetArrivedAtDistance(float val)
    {
        arrivedAtDistance = val;
    }

    public bool isAtDestination(Vector3 destination)
    {
        offset = glideObject.transform.position - destination;
        currDist = offset.sqrMagnitude;
        objDistMagnitude = new Vector3(Mathf.Abs(destinationPos.transform.position.x - glideObject.transform.position.x), 0, Mathf.Abs(destinationPos.transform.position.z - glideObject.transform.position.z)).magnitude;

        if (currDist < arrivedAtDistance  || objDistMagnitude > destMagnitude)
        {
            reachedDestination = true;
            glide = false;

            if (hasEvents)
                eventCalls.Invoke();
            else
                this.enabled = false;

            return reachedDestination;
        }
        else
        {
            reachedDestination = false;
            return reachedDestination;
        }
    }

    public bool isAtDestination()
    {       
        return reachedDestination;
    }

    private Vector3 CalcArcHeight()
    {
        float currDistanceXY = (new Vector2(glideObject.transform.position.x, glideObject.transform.position.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        float distTravelRemainPercent = (currDistanceXY / startDistanceXY);
        arcTargetPos = new Vector3(0, arcHeight, 0) * distTravelRemainPercent;

        return arcTargetPos;
    }

    public void CancelGlide()
    {
        glide = false;
    }
}
