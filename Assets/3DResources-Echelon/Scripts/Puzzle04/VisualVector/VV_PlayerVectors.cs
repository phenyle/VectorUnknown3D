using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VV_PlayerVectors : VisVectController
{
    [Header("----- Vector Lines --------")]
    public LineRenderer vector1;
    public LineRenderer vector2;
    public LineRenderer answer;
    public LineRenderer goal;

    [Header("----- Vector Ends --------")]
    public GameObject vector1end;
    public GameObject vector2end;

    public void Start()
    {
        playerVectors = this;
        Init();
    }


    public void Init()
    {
        vector1.positionCount = 2;
        vector2.positionCount = 2;
        answer.positionCount = 2;
        goal.positionCount = 2;

        vector1.enabled = false;
        vector2.enabled = false;
        answer.enabled = false;
        goal.enabled = false;

        vector1end.SetActive(false);
        vector2end.SetActive(false);
    }

    public Vector3 GetAnswerEnd()
    {
        return vector2.GetPosition(1);
    }

    public void UpdateVector1(Vector3 start, Vector3 end)
    {
        vector1.SetPosition(0, start);
        vector1.SetPosition(1, end);
        vector1end.transform.position = end;
        if (start != end)
        {
            vector1.enabled = true;
            vector1end.SetActive(true);
        }
        else
        {
            vector1.enabled = false;
            vector1end.SetActive(false);
        }
    }
    public void UpdateVector2(Vector3 start, Vector3 end)
    {
        vector2.SetPosition(0, start);
        vector2.SetPosition(1, end);
        vector2end.transform.position = end;
        if (start != end)
        {
            vector2.enabled = true;
            vector2end.SetActive(true);
        }
        else
        {
            vector2.enabled = false;
            vector2end.SetActive(false);
        }
    }
    public void UpdateAnswer(Vector3 start, Vector3 end)
    {
        answer.SetPosition(0, start);
        answer.SetPosition(1, end);
        if (start != end)
            answer.enabled = true;
        else
            answer.enabled = false;
    }

    public void UpdateGoal(Vector3 start, Vector3 end)
    {
        goal.SetPosition(0, start);
        goal.SetPosition(1, end);
        goal.enabled = true;
    }

    public void TogglePlayerVectors(bool val)
    {
        vector1.enabled = val;
        vector2.enabled = val;
        answer.enabled = val;
        goal.enabled = val;

        vector1end.SetActive(val);
        vector2end.SetActive(val);
    }

}
