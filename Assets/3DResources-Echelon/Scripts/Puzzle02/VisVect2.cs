using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisVect2 : MonoBehaviour
{
    //Axis
    public LineRenderer axisXpos;
    public LineRenderer axisZpos;
    private LineRenderer axisXneg;
    private LineRenderer axisZneg;
    private bool XisZero;
    private bool ZisZero;

    //Lattice
    public GameObject lattice1Lines;
    public GameObject lattice2Lines;
    private List<LineRenderer> linesXpos;
    private List<LineRenderer> linesXneg;
    private List<LineRenderer> linesZpos;
    private List<LineRenderer> linesZneg;
    public Color incri5_CLR;
    public Color incri10_CLR;
    public float incri5_width = 0.15f;
    public float incri10_width = 0.2f;

    //Vector
    public GameObject vectorPoint;
    public LineRenderer vectorLine;

    public int LimitZ = 300;
    public int LimitX = 300;
    private Vector3 vectX { get; set; }
    private Vector3 vectZ { get; set; }

    //Settings
    private int numOfLines = 200;
    private bool updateLattice = false;

    // Start is called before the first frame update
    void Start()
    {
        linesXpos = new List<LineRenderer>();
        linesXneg = new List<LineRenderer>();
        linesZpos = new List<LineRenderer>();
        linesZneg = new List<LineRenderer>();
        //incri5_width = 0.15f;
        //incri10_width = 0.2f;

        for(int i = 0; i< numOfLines; i++)
        {
            linesXpos.Add(Instantiate(axisXpos));
            linesXpos[linesXpos.Count - 1].transform.SetParent(lattice1Lines.transform);
            linesXneg.Add(Instantiate(axisXpos));
            linesXneg[linesXneg.Count - 1].transform.SetParent(lattice1Lines.transform);

            linesZpos.Add(Instantiate(axisZpos));
            linesZpos[linesZpos.Count - 1].transform.SetParent(lattice2Lines.transform);
            linesZneg.Add(Instantiate(axisZpos));
            linesZneg[linesZneg.Count - 1].transform.SetParent(lattice2Lines.transform);
        }

        axisXpos.startWidth = 0.6f;
        axisXpos.endWidth = 0.6f;
        axisZpos.startWidth = 0.6f;
        axisZpos.endWidth = 0.6f;

        axisXpos.startColor = Color.blue;
        axisXpos.endColor = Color.blue;
        axisZpos.startColor = Color.red;
        axisZpos.endColor = Color.red;

        axisXneg = Instantiate(axisXpos).GetComponent<LineRenderer>();
        axisXneg.transform.SetParent(transform);
        axisZneg = Instantiate(axisZpos).GetComponent<LineRenderer>();
        axisZneg.transform.SetParent(transform);
        axisXneg.startColor = Color.cyan;
        axisXneg.endColor = Color.cyan;

        axisZneg.startColor = Color.magenta;
        axisZneg.endColor = Color.magenta;

        ToggleLattice(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(updateLattice)
        {
            //Vector3 temp1 = Vector3.RotateTowards(axisXpos.GetPosition(1).normalized, vectX.normalized, 0.01f, 0.1f);
            //Vector3 temp2 = Vector3.RotateTowards(axisZpos.GetPosition(1).normalized, vectZ.normalized, 0.01f, 0.1f);

            Vector3 temp1 = Vector3.RotateTowards(axisXpos.GetPosition(1).normalized, vectX.normalized, 0.005f, 0.01f);
            Vector3 temp2 = Vector3.RotateTowards(axisZpos.GetPosition(1).normalized, vectZ.normalized, 0.005f, 0.01f);


            SetMasterAxis(temp1, temp2);
        }
    }
    
    public void ToggleLattice(bool val)
    {
        axisXpos.gameObject.SetActive(val);
        axisZpos.gameObject.SetActive(val);
        axisXneg.gameObject.SetActive(val);
        axisZneg.gameObject.SetActive(val);

        lattice1Lines.SetActive(val);
        lattice2Lines.SetActive(val);

        vectorLine.gameObject.SetActive(val);
        vectorPoint.SetActive(val);
    }

    public void CreateLattice(Vector3 mat1, Vector3 mat2)
    {
        vectX = mat1;
        vectZ = mat2;
        //SetMasterAxis(Vector3.right, Vector3.forward);
        updateLattice = true;
        //IncrimentsX();
        //IncrimentsZ();
    }


    private void SetMasterAxis(Vector3 mat1, Vector3 mat2)
    {
        if (mat1 != Vector3.zero)
        {
            XisZero = false;
            axisXpos.SetPosition(0, Vector3.zero);
            axisXpos.SetPosition(1, mat1.normalized * 500);
            axisXneg.SetPosition(0, Vector3.zero);
            axisXneg.SetPosition(1, mat1.normalized * -500);
        }
        else
        {
            XisZero = true;
            lattice1Lines.SetActive(false);
        }

        if (mat2 != Vector3.zero)
        {
            ZisZero = false;
            axisZpos.SetPosition(0, Vector3.zero);
            axisZpos.SetPosition(1, mat2.normalized * 500);
            axisZneg.SetPosition(0, Vector3.zero);
            axisZneg.SetPosition(1, mat2.normalized * -500);
        }
        else
        {
            ZisZero = true;
            lattice2Lines.SetActive(false);
        }

        IncrimentsX();
        IncrimentsZ();


        if (axisXpos.GetPosition(1).normalized == vectX.normalized &&
            axisZpos.GetPosition(1).normalized == vectZ.normalized)
            updateLattice = false;
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
            linesXpos[0].gameObject.SetActive(false);
            linesXneg[0].gameObject.SetActive(false);

            Vector3 cross = Vector3.Cross(axisXpos.GetPosition(1).normalized, Vector3.down).normalized;
            cross *= vectX.magnitude;

            //walk
            for (int i = 1; i < numOfLines; i++)
            {
                //postive

                linesXpos[i].SetPosition(0, axisXpos.GetPosition(1) + (cross * i));
                linesXpos[i].SetPosition(1, axisXneg.GetPosition(1) + (cross * i));

                //negative
                linesXneg[i].SetPosition(0, axisXpos.GetPosition(1) - (cross * i));
                linesXneg[i].SetPosition(1, axisXneg.GetPosition(1) - (cross * i));


                if (i % 10 == 0)
                {
                    linesXpos[i].startColor = incri10_CLR;
                    linesXpos[i].endColor = incri10_CLR;
                    linesXpos[i].startWidth = incri10_width;
                    linesXpos[i].endWidth = incri10_width;

                    linesXneg[i].startColor = incri10_CLR;
                    linesXneg[i].endColor = incri10_CLR;
                    linesXneg[i].startWidth = incri10_width;
                    linesXneg[i].endWidth = incri10_width;

                }
                else if (i%5 == 0)
                {
                    linesXpos[i].startColor = incri5_CLR;
                    linesXpos[i].endColor = incri5_CLR;
                    linesXpos[i].startWidth = incri5_width;
                    linesXpos[i].endWidth = incri5_width;

                    linesXneg[i].startColor = incri5_CLR;
                    linesXneg[i].endColor = incri5_CLR;
                    linesXneg[i].startWidth = incri5_width;
                    linesXneg[i].endWidth = incri5_width;
                }
            }
        }
    }

    private void IncrimentsZ()
    {
        if (!XisZero)
        {
            linesZpos[0].gameObject.SetActive(false);
            linesZneg[0].gameObject.SetActive(false);

            Vector3 cross = Vector3.Cross(axisZpos.GetPosition(1).normalized, Vector3.down).normalized;
            cross *= vectZ.magnitude;


            for (int i = 1; i < numOfLines; i++)
            {
                //postive
                linesZpos[i].SetPosition(0, axisZpos.GetPosition(1) + (cross * i));
                linesZpos[i].SetPosition(1, axisZneg.GetPosition(1) + (cross * i));

                //negative
                linesZneg[i].SetPosition(0, axisZpos.GetPosition(1) - (cross * i));
                linesZneg[i].SetPosition(1, axisZneg.GetPosition(1) - (cross * i));

                if (i % 10 == 0)
                {
                    linesZpos[i].startColor = incri10_CLR;
                    linesZpos[i].endColor = incri10_CLR;
                    linesZpos[i].startWidth = incri10_width;
                    linesZpos[i].endWidth = incri10_width;

                    linesZneg[i].startColor = incri10_CLR;
                    linesZneg[i].endColor = incri10_CLR;
                    linesZneg[i].startWidth = incri10_width;
                    linesZneg[i].endWidth = incri10_width;

                }
                else if (i % 5 == 0)
                {
                    linesZpos[i].startColor = incri5_CLR;
                    linesZpos[i].endColor = incri5_CLR;
                    linesZpos[i].startWidth = incri5_width;
                    linesZpos[i].endWidth = incri5_width;

                    linesZneg[i].startColor = incri5_CLR;
                    linesZneg[i].endColor = incri5_CLR;
                    linesZneg[i].startWidth = incri5_width;
                    linesZneg[i].endWidth = incri5_width;
                }
            }

            ////walk
            //for (int i = 0; i < numOfLines / 2; i++)
            //{
            //    //postive
            //    linesZpos[i].SetPosition(0, axisZ.GetPosition(0) + (vectX * (i + 1)));
            //    linesZpos[i].SetPosition(1, axisZ.GetPosition(1) + (vectX * (i + 1)));

            //    //negative
            //    linesZneg[numOfLines - (i + 1)].SetPosition(0, axisZ.GetPosition(0) - (vectX * (i + 1)));
            //    linesZneg[numOfLines - (i + 1)].SetPosition(1, axisZ.GetPosition(1) - (vectX * (i + 1)));
            //}
        }
    }
}
