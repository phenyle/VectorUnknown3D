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
    private Vector3 startPos;
    private float destMagnitude;
    private float objDistMagnitude;
    private Vector3 halfwayPoint;
    private bool reachedDestination = false;
    private Vector3 offset;
    private float currDist;
    private bool glide = false;

    [Header("Glide Controls")]
    [Range(0.001f, 1.0f)]
    public float objectMoveSpeed = 0.1f;
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
            //offset = glideObject.transform.position - destinationPos.transform.position;
            //currDist = offset.sqrMagnitude;

            if (!isAtDestination(destinationPos.transform.position))
            {
                if (arcHeight != 0)
                    glideObject.transform.position = Vector3.MoveTowards(glideObject.transform.position, destinationPos.transform.position + CalcArcHeight(), objectMoveSpeed);
                else
                    glideObject.transform.position = Vector3.MoveTowards(glideObject.transform.position, destinationPos.transform.position, objectMoveSpeed);

                //      glideObject.transform.position += new Vector3(0, CalcArcHeight(), 0);
                if(focusAtObject)
                    glideObject.transform.rotation = Quaternion.Slerp(glideObject.transform.rotation, Quaternion.LookRotation((targetPos.transform.position - glideObject.transform.position).normalized), objectRotateSpeed);

            }
        }
    }


    #region Glide and Rotate Functions
    public void GlideAndLook(Vector3 newDestination, Vector3 target, Vector3 newStartPos)
    {
        SetDestinationPos(newDestination);
        SetTargetPos(target);
        SetStartPos(newStartPos);
        focusAtObject = true;
        arcHeight = 0.0f;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;

        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;

        reachedDestination = false;
        glide = true;
        this.enabled = true;
    }

    public void GlideAndLook(Vector3 newDestionation, Vector3 target, Vector3 newStartPos, float arc)
    {
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

        this.enabled = true;
    }

    public void GlideAndLook(GameObject gobject, Vector3 newDestination, Vector3 target, Vector3 newStartPos)
    {

        glideObject = gobject;
        SetDestinationPos(newDestination);
        SetTargetPos(target);
        SetStartPos(newStartPos);
        focusAtObject = true;
        arcHeight = 0.0f;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;

        this.enabled = true;
        
    }

    public void GlideAndLook(GameObject gobject, Vector3 newDestionation, Vector3 target, Vector3 newStartPos, float arc)
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

        this.enabled = true;
    }

    public void GlideAndLookMovingTarget(GameObject gobject, GameObject newDestination, GameObject target, Vector3 newStartPos)
    {
        glideObject = gobject;
        SetDestinationPos(newDestination);
        SetTargetPos(target);
        SetStartPos(newStartPos);
        focusAtObject = true;
        arcHeight = 0.0f;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;

        this.enabled = true;
    }

    public void GlideAndLookMovingTarget(GameObject gobject, GameObject newDestionation, GameObject target, Vector3 newStartPos, float arc)
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

        this.enabled = true;
    }
#endregion


    #region Glide Only Functions
    public void GlideOnly(Vector3 newDestination, Vector3 newStartPos)
    {
        SetDestinationPos(newDestination);
        SetStartPos(newStartPos);
        focusAtObject = false;
        arcHeight = 0.0f;
        startDistanceXY = (new Vector2(startPos.x, startPos.z) - new Vector2(destinationPos.transform.position.x, destinationPos.transform.position.z)).magnitude;
        halfwayPoint = (new Vector3(destinationPos.transform.position.x, 0, destinationPos.transform.position.z) - new Vector3(startPos.x, 0, startPos.z)) / 2;
        destMagnitude = new Vector3(Mathf.Abs(startPos.x - destinationPos.transform.position.x), 0, Mathf.Abs(startPos.z - destinationPos.transform.position.z)).magnitude;
        reachedDestination = false;
        glide = true;
        this.enabled = true;
    }

    public void GlideOnly(Vector3 newDestionation, Vector3 newStartPos, float arc)
    {
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

        this.enabled = true;
    }

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

        this.enabled = true;
    }

    public void GlideOnlyMovingTarget(GameObject gobject, GameObject newDestination, Vector3 newStartPos)
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

        this.enabled = true;
    }

    public void GlideOnlyMovingTarget(GameObject gobject, GameObject newDestionation, Vector3 newStartPos, float arc)
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

    public void SetObjectRotateSpeed(float val)
    {
        objectRotateSpeed = val;
    }

    public void SetArrivedAtDistaince(float val)
    {
        arrivedAtDistance = val;
    }

    public bool isAtDestination(Vector3 destination)
    {
        offset = glideObject.transform.position - destination;
        currDist = offset.sqrMagnitude;
        objDistMagnitude = new Vector3(Mathf.Abs(startPos.x - glideObject.transform.position.x), 0, Mathf.Abs(startPos.z - glideObject.transform.position.z)).magnitude;

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

}
