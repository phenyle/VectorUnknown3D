using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VV_Axis : VisVectController
{
    public Color axisColor;
    public float axisWidth = 0.5f;
    [Header("----- Axis Lines --------")]
    public GameObject AxisLines;
    public LineRenderer Xaxis;
    public LineRenderer Yaxis;
    public LineRenderer Zaxis;

    [Header("----- Axis Labels --------")]
    public GameObject AxisLabels;
    public GameObject Xlabel;
    public GameObject Ylabel;
    public GameObject Zlabel;

    private TextMeshPro XlabelText;
    private TextMeshPro YlabelText;
    private TextMeshPro ZlabelText;

    public void Start()
    {
        axis = this;
        Init();
    }


    public void Init()
    {
        LineProperties(Xaxis, axisColor, axisWidth);
        LineProperties(Yaxis, axisColor, axisWidth);
        LineProperties(Zaxis, axisColor, axisWidth);

        Xaxis.enabled = false;
        Yaxis.enabled = false;
        Zaxis.enabled = false;

        XlabelText = Xlabel.GetComponent<TextMeshPro>();
        YlabelText = Ylabel.GetComponent<TextMeshPro>();
        ZlabelText = Zlabel.GetComponent<TextMeshPro>();

        ToggleAxis(false);
    }

    public void Set_Xaxis(Vector3 answer, Vector3 start, Vector3 end)
    {
        if(answer.x == 0)
        {
            Xaxis.enabled = false;
            return;
        }
        Xaxis.SetPosition(0, start);
        Xaxis.SetPosition(1, end);
        Xaxis.enabled = true;
    }
    public void Set_XLabel(Vector3 start, Vector3 answer, Vector3 scale)
    {
        Xlabel.transform.position = start;
        Xlabel.SetActive(true);
        Xlabel.GetComponent<FaceObject>().SetTarget(GameControllerPuzzle04.instance.currPuzzle.puzzleCamera);
        Xlabel.GetComponent<FaceObject>().enabled = true;

        if (answer.x > 0)
        {
            Xlabel.transform.position += 2 * new Vector3(scale.x, 0, 0);
            XlabelText.text = "+X";
        }
        else if (answer.x < 0)
        {
            Xlabel.transform.position -= 2 * new Vector3(scale.x, 0, 0);
            XlabelText.text = "-X";
        }
        else
            Xlabel.SetActive(false);
    }
    public void Set_Yaxis(Vector3 answer, Vector3 start, Vector3 end)
    {
        if (answer.y == 0)
        {
            Yaxis.enabled = false;
            return;
        }

        Yaxis.SetPosition(0, start);
        Yaxis.SetPosition(1, end);
        Yaxis.enabled = true;
    }
    public void Set_YLabel(Vector3 start, Vector3 answer, Vector3 scale)
    {
        Ylabel.transform.position = start;
        Ylabel.SetActive(true);
        Ylabel.GetComponent<FaceObject>().SetTarget(GameControllerPuzzle04.instance.currPuzzle.puzzleCamera);
        Ylabel.GetComponent<FaceObject>().enabled = true;

        if (answer.y > 0)
        {
            Ylabel.transform.position += 2 * new Vector3(0, scale.y, 0);
            YlabelText.text = "+Z";
        }
        else if (answer.y < 0)
        {
            Ylabel.transform.position -= 2 * new Vector3(0, scale.y, 0);
            YlabelText.text = "-Z";
        }
        else
            Ylabel.SetActive(false);
    }

    public void Set_Zaxis(Vector3 answer, Vector3 start, Vector3 end)
    {
        if (answer.z == 0)
        {
            Zaxis.enabled = false;
            return;
        }
        Zaxis.SetPosition(0, start);
        Zaxis.SetPosition(1, end);
        Zaxis.enabled = true;
    }
    public void Set_ZLabel(Vector3 start, Vector3 answer, Vector3 scale)
    {
        Zlabel.transform.position = start;
        Zlabel.SetActive(true);
        Zlabel.GetComponent<FaceObject>().SetTarget(GameControllerPuzzle04.instance.currPuzzle.puzzleCamera);
        Zlabel.GetComponent<FaceObject>().enabled = true;

        if (answer.z > 0)
        {
            Zlabel.transform.position += 2 * new Vector3(0, 0, scale.z);
            ZlabelText.text = "+Y";
        }
        else if(answer.z < 0)
        {
            Zlabel.transform.position -= 2 * new Vector3(0, 0, scale.z);
            ZlabelText.text = "-Y";
        }
        else
            Zlabel.SetActive(false);
    }

    public void ToggleAxis(bool val)
    {
        AxisLines.SetActive(val);
        AxisLabels.SetActive(val);
    }
}
