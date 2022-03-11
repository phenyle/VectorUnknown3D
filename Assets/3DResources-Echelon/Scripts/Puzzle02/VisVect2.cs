using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisVect2 : MonoBehaviour
{
    //Axis
    public LineRenderer axisX;
    public LineRenderer axisZ;
    private bool XisZero;
    private bool ZisZero;

    //Lattice
    public GameObject lattice1Lines;
    public GameObject lattice2Lines;
    private List<LineRenderer> linesX;
    private List<LineRenderer> linesZ;

    //Vector
    public GameObject vectorPoint;
    public LineRenderer vectorLine;

    public int LimitZ = 300;
    public int LimitX = 300;
    private Vector3 vectX { get; set; }
    private Vector3 vectZ { get; set; }

    //Settings
    private int numOfLines = 400;

    // Start is called before the first frame update
    void Start()
    {
        linesX = new List<LineRenderer>();
        linesZ = new List<LineRenderer>();

        for(int i = 0; i< numOfLines; i++)
        {
            linesX.Add(Instantiate(axisX));
            linesX[linesX.Count - 1].transform.SetParent(lattice1Lines.transform);

            linesZ.Add(Instantiate(axisZ));
            linesZ[linesZ.Count - 1].transform.SetParent(lattice2Lines.transform);

            if(i%5 == 0 && i != 0)
            {
                linesX[i].startColor = Color.cyan;
                linesX[i].endColor = Color.cyan;
                linesZ[i].startColor = Color.cyan;
                linesZ[i].endColor = Color.cyan;

                linesX[i].startWidth = 0.3f;
                linesX[i].endWidth = 0.3f;
                linesZ[i].startWidth = 0.3f;
                linesZ[i].endWidth = 0.3f;
            }
        }

        axisX.startColor = Color.blue;
        axisX.endColor = Color.blue;
        axisZ.startColor = Color.red;
        axisZ.endColor = Color.red;
        axisX.startWidth = 0.5f;
        axisX.endWidth = 0.5f;
        axisZ.startWidth = 0.5f;
        axisZ.endWidth = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ToggleLattice(bool val)
    {
        axisX.gameObject.SetActive(val);
        axisZ.gameObject.SetActive(val);

        lattice1Lines.SetActive(val);
        lattice2Lines.SetActive(val);

        vectorLine.gameObject.SetActive(val);
        vectorPoint.SetActive(val);

    }

    public void CreateLattice(Vector3 mat1, Vector3 mat2)
    {
        vectX = mat1;
        vectZ = mat2;
        SetMasterAxis(mat1, mat2);
        IncrimentsX();
        IncrimentsZ();
    }


    private void SetMasterAxis(Vector3 mat1, Vector3 mat2)
    {
        if (mat1 != Vector3.zero)
        {
            XisZero = false;
            axisX.SetPosition(0, mat1 * 500);
            axisX.SetPosition(1, mat1 * (-1) * 500);
        }
        else
        {
            XisZero = true;
            lattice1Lines.SetActive(false);
        }

        if (mat2 != Vector3.zero)
        {
            ZisZero = false;
            axisZ.SetPosition(0, mat2 * 500);
            axisZ.SetPosition(1, mat2 * (-1) * 500);
        }
        else
        {
            ZisZero = true;
            lattice2Lines.SetActive(false);
        }
    }


    public void SetMasterVector(Vector3 vector)
    {
        vectorPoint.transform.position = vector;
        vectorLine.SetPosition(0, Vector3.zero);
        vectorLine.SetPosition(1, vector);
    }



    private void IncrimentsX()
    {
        if(!ZisZero)
        {
            //walk
            for(int i = 0; i < numOfLines / 2; i++)
            {
                //postive
                linesX[i].SetPosition(0, axisX.GetPosition(0) + (vectZ * (i + 1)));
                linesX[i].SetPosition(1, axisX.GetPosition(1) + (vectZ * (i + 1)));

                //negative
                linesX[numOfLines - (i + 1)].SetPosition(0, axisX.GetPosition(0) - (vectZ * (i + 1)));
                linesX[numOfLines - (i + 1)].SetPosition(1, axisX.GetPosition(1) - (vectZ * (i + 1)));
            }
        }
    }

    private void IncrimentsZ()
    {
        if (!XisZero)
        {
            //walk
            for (int i = 0; i < numOfLines / 2; i++)
            {
                //postive
                linesZ[i].SetPosition(0, axisZ.GetPosition(0) + (vectX * (i + 1)));
                linesZ[i].SetPosition(1, axisZ.GetPosition(1) + (vectX * (i + 1)));

                //negative
                linesZ[numOfLines - (i + 1)].SetPosition(0, axisZ.GetPosition(0) - (vectX * (i + 1)));
                linesZ[numOfLines - (i + 1)].SetPosition(1, axisZ.GetPosition(1) - (vectX * (i + 1)));
            }
        }
    }
}
