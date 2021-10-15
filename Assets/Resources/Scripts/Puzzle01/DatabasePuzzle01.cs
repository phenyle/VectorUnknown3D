using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabasePuzzle01 : MonoBehaviour
{
    public List<Transform> points;
    public List<Transform> Bridges;
    public List<Transform> glowZones;
    public List<Vector3> pointVectors;
    public LineRenderer[] lineTips = new LineRenderer[2];
    public Transform wrongAnswerPointRed;
    public Transform wrongAnswerPointGreen;
    public Vector3 targetPositionDisplay;

    private float iptScalar, iptX, iptY, iptZ;
    private GameControllerPuzzle01 GCP01;
    private int questionNum;
    private float scalar;
    private bool isFirstDisplayGreenTips = true;

    public void InitDatabase()
    {
        Debug.Log("Connect GameController of Puzzle01");
        GCP01 = GetComponent<GameControllerPuzzle01>();

        //Convert all data in Unity xyz standard into math xyz standard
        foreach (Transform T in points)
        {
            pointVectors.Add(new Vector3(T.position.x, T.position.z, T.position.y));
        }

        //Init first line tip from point 1 to point 2
        InitLineTips(lineTips[0], points[0].position, points[1].position, Color.yellow, Color.yellow);
        iptX = 1;
        iptY = 1;
        iptZ = 1;
        iptScalar = 1;
        InitLineTips(lineTips[1], points[0].position, points[0].position + new Vector3(iptX, iptY, iptZ), Color.red, Color.red);
    }

    public bool Calculation(int questionNum, float scalar, float x, float y, float z)
    {
        this.questionNum = questionNum;
        this.scalar = scalar;

        Vector3 answer = pointVectors[questionNum] - pointVectors[questionNum - 1];
        Vector3 playerAnswer = new Vector3(x, y, z) * scalar;

        targetPositionDisplay = pointVectors[questionNum - 1] + new Vector3(x, y, z) * scalar;
        Vector3 targetPosition = new Vector3(targetPositionDisplay.x, targetPositionDisplay.z, targetPositionDisplay.y);

        if (answer == playerAnswer)
        {
            //GCP01.SetText("Correct");

            //Open the bridge for current question

            Bridges[questionNum - 1].gameObject.SetActive(true);

            //Active the zone of next question
            foreach(Transform t in glowZones)
            {
                t.gameObject.SetActive(false);
            }
            try
            {
                glowZones[questionNum].gameObject.SetActive(true);
            }
            catch { }


            //If there are more question, move line tip to next question
            lineTips[0].gameObject.SetActive(false);
            if (questionNum + 1 < points.Count)
            {
                InitLineTips(lineTips[0], points[questionNum].position, points[questionNum + 1].position, Color.yellow, Color.yellow);
                InitLineTips(lineTips[1], points[questionNum].position, points[questionNum].position + new Vector3(1, 1, 1), Color.red, Color.red);

                //Make sure wrong answer tips line is inactive
                //wrongAnswerPoint.gameObject.SetActive(false);
                lineTips[1].gameObject.SetActive(false);
                wrongAnswerPointRed.gameObject.SetActive(false);
            }

            return true;
        }
        else
        {
            InitLineTips(lineTips[1], points[questionNum - 1].position, targetPosition, Color.red, Color.red);

            wrongAnswerPointRed.position = targetPosition;
            wrongAnswerPointRed.gameObject.SetActive(true);

            return false;
        }
    }

    private void InitLineTips(LineRenderer lineTips, Vector3 startPoint, Vector3 endPoint, Color startColor, Color endColor)
    {
        lineTips.gameObject.SetActive(true);
        lineTips.SetPosition(0, startPoint);
        lineTips.SetPosition(1, endPoint);
        lineTips.startColor = startColor;
        lineTips.endColor = endColor;
    }

    public void updateLineScalar(float scalar, int questionNum)
    {
        iptScalar = scalar;
        Vector3 targetPositionDisplay = pointVectors[questionNum - 1] + new Vector3(iptX, iptY, iptZ) * iptScalar;
        Vector3 targetPosition = new Vector3(targetPositionDisplay.x, targetPositionDisplay.z, targetPositionDisplay.y);
        lineTips[1].SetPosition(1, targetPosition);
        Debug.Log("scalar");
    }

    public void updateLineX(float x, int questionNum)
    {
        iptX = x;
        Vector3 targetPositionDisplay = pointVectors[questionNum - 1] + new Vector3(iptX, iptY, iptZ) * iptScalar;
        Vector3 targetPosition = new Vector3(targetPositionDisplay.x, targetPositionDisplay.z, targetPositionDisplay.y);
        lineTips[1].SetPosition(1, targetPosition);
        Debug.Log("x" + iptX);
    }

    public void updateLineY(float y, int questionNum)
    {
        iptY = y;
        Vector3 targetPositionDisplay = pointVectors[questionNum - 1] + new Vector3(iptX, iptY, iptZ) * iptScalar;
        Vector3 targetPosition = new Vector3(targetPositionDisplay.x, targetPositionDisplay.z, targetPositionDisplay.y);
        lineTips[1].SetPosition(1, targetPosition);
        Debug.Log("y" + iptY);
    }

    public void updateLineZ(float z, int questionNum)
    {
        iptZ = z;
        Vector3 targetPositionDisplay = pointVectors[questionNum - 1] + new Vector3(iptX, iptY, iptZ) * iptScalar;
        Vector3 targetPosition = new Vector3(targetPositionDisplay.x, targetPositionDisplay.z, targetPositionDisplay.y);
        lineTips[1].SetPosition(1, targetPosition);
        Debug.Log("z" + iptZ);
    }

    public string GetCurrentVector(int questionNum)
    {
        return "( " + pointVectors[questionNum - 1].x + ", "
            + pointVectors[questionNum - 1].y + ", "
            + pointVectors[questionNum - 1].z + " )";
    }

    public string GetResultVector()
    {
        return "( " + targetPositionDisplay.x + ", "
            + targetPositionDisplay.y + ", "
            + targetPositionDisplay.z + " )";
    }

    /*
    public void SetGreenLineTips()
    {
        if (GCP01.isAnswerCorrect)
        {
            return;   
        }

        Debug.Log("Test2");
        
        try
        {
            Debug.Log("Test3");
            //Get the current input in UI
            string[] currentInput = GCP01.P01W.GetCurrentInput().Split('|');

            Debug.Log(GCP01.P01W.GetCurrentInput());
            Vector3 currentInputValue = new Vector3((float)Convert.ToDouble(currentInput[0]),
                                                                          (float)Convert.ToDouble(currentInput[1]),
                                                                          (float)Convert.ToDouble(currentInput[2]));

            Vector3 targetPosition = pointVectors[questionNum - 1] + currentInputValue * scalar;

            targetPosition = new Vector3(targetPosition.x, targetPosition.z, targetPosition.y);

            InitLineTips(lineTips[2], points[questionNum - 1].position, targetPosition, Color.green, Color.green);

            wrongAnswerPointGreen.position = targetPosition;
            wrongAnswerPointGreen.gameObject.SetActive(true);
        }
        catch { }
    }

    public void ClearGreenLineTips()
    {
        lineTips[2].gameObject.SetActive(false);
        wrongAnswerPointGreen.gameObject.SetActive(false);
    }*/
}
