using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VV_Grid : VisVectController
{
    public int GridSize = 100;
    [SerializeField] private LineRenderer gridLine;
    public Color clrSTD;
    public float thickSTD = 0.1f;
    public Color clr5incri;
    public float thick5incri = 0.2f;
    public Color clr10incri;
    public float thick10incri = 0.3f;


    [Header("----- XY Grid --------")]
    public GameObject xy_grid; //all the lines that make up the XY plane
    public GameObject xy_gridHori; //all the lines parallel to X
    public Transform xy_gridHori_Yneg; //parallel to X, negative Y
    public Transform xy_gridHori_Ypos; //parallel to X, positive Y
    public GameObject xy_gridVert; //all the lines parallel to Y
    public Transform xy_gridVert_Xneg; //parallel to Y, negative X
    public Transform xy_gridVert_Xpos; //parallel to Y, positive X

    private List<LineRenderer> xy_Horizontal;
    private List<LineRenderer> xy_Vertical;

    [Header("----- YZ Grid --------")]
    public GameObject yz_grid;
    public GameObject yz_gridHori;
    public Transform yz_gridHori_Yneg; //parallel to Z, negative Y
    public Transform yz_gridHori_Ypos; //parallel to Z, positive Y
    public GameObject yz_gridVert;
    public Transform yz_gridVert_Zneg; //parallel to Y, negative Z
    public Transform yz_gridVert_Zpos; //parallel to Y, positive Z
    private List<LineRenderer> yz_Horizontal;
    private List<LineRenderer> yz_Vertical;

    [Header("----- XZ Grid --------")]
    public GameObject xz_grid;
    public GameObject xz_gridHori;
    public Transform xz_gridHori_Zneg; //parallel to X, negative Z
    public Transform xz_gridHori_Zpos; //parallel to X, positive Z
    public GameObject xz_gridVert;
    public Transform xz_gridVert_Xneg; //parallel to Z, negative X
    public Transform xz_gridVert_Xpos; //parallel to Z, positive X
    private List<LineRenderer> xz_Horizontal;
    private List<LineRenderer> xz_Vertical;

    public void Start()
    {
        grid = this;
        Init();
    }

    public void Init()
    {
        gridLine.positionCount = 2;

        xy_Horizontal = new List<LineRenderer>();
        xy_Vertical = new List<LineRenderer>();
        yz_Horizontal = new List<LineRenderer>();
        yz_Vertical = new List<LineRenderer>();
        xz_Horizontal = new List<LineRenderer>();
        xz_Vertical = new List<LineRenderer>();

        for (int i = 0; i < GridSize * 2 + 1; i++)
        {
            xy_Horizontal.Add(Instantiate(gridLine));
            if(i < GridSize)
                xy_Horizontal[i].transform.parent = xy_gridHori_Yneg;
            else
                xy_Horizontal[i].transform.parent = xy_gridHori_Ypos;

            xy_Vertical.Add(Instantiate(gridLine));
            if (i < GridSize)
                xy_Vertical[i].transform.parent = xy_gridVert_Xneg;
            else
                xy_Vertical[i].transform.parent = xy_gridVert_Xpos;


            yz_Horizontal.Add(Instantiate(gridLine));
            if (i < GridSize)
                yz_Horizontal[i].transform.parent = yz_gridHori_Yneg;
            else
                yz_Horizontal[i].transform.parent = yz_gridHori_Ypos;

            yz_Vertical.Add(Instantiate(gridLine));
            if (i < GridSize)
                yz_Vertical[i].transform.parent = yz_gridVert_Zneg;
            else
                yz_Vertical[i].transform.parent = yz_gridVert_Zpos;


            xz_Horizontal.Add(Instantiate(gridLine));
            if (i < GridSize)
                xz_Horizontal[i].transform.parent = xz_gridHori_Zneg;
            else
                xz_Horizontal[i].transform.parent = xz_gridHori_Zpos;

            xz_Vertical.Add(Instantiate(gridLine));
            if (i < GridSize)
                xz_Vertical[i].transform.parent = xz_gridVert_Xneg;
            else
                xz_Vertical[i].transform.parent = xz_gridVert_Xpos;
        }
    }

    public void Draw2DGrid(Vector3 scale, Vector3 answer)
    {
        float x_incriment;
        float y_incriment;

        for (int i = 1; i < GridSize; i++)
        {
            if (answer.y == 0)
            {
                x_incriment = scale.x * i;
                y_incriment = scale.x * i;
            }
            else if (answer.x == 0)
            {
                x_incriment = scale.y * i;
                y_incriment = scale.y * i;
            }
            else
            {
                x_incriment = scale.x * i;
                y_incriment = scale.y * i;
            }

            //================DRAWING LINES ALONG X======================
            //set negative grid lines-----------------------
            //parallel to Y, incrimented along X
            xy_Vertical[GridSize - i].SetPosition(0, Y_negative + new Vector3(-x_incriment, 0, 0));
            xy_Vertical[GridSize - i].SetPosition(1, Y_positive + new Vector3(-x_incriment, 0, 0));

            //set positive grid lines------------------------
            //parallel to Y, incrimented along X
            xy_Vertical[GridSize + i].SetPosition(0, Y_negative + new Vector3(x_incriment, 0, 0));
            xy_Vertical[GridSize + i].SetPosition(1, Y_positive + new Vector3(x_incriment, 0, 0));

            //Colors and Thickness Incriments
            if (i % 10 == 0)
            {
                LineProperties(xy_Vertical[GridSize - i], clr10incri, thick10incri);
                LineProperties(xy_Vertical[GridSize + i], clr10incri, thick10incri);
            }
            else if (i % 5 == 0)
            {
                LineProperties(xy_Vertical[GridSize - i], clr5incri, thick5incri);
                LineProperties(xy_Vertical[GridSize + i], clr5incri, thick5incri);
            }
            else
            {
                LineProperties(xy_Vertical[GridSize - i], clrSTD, thickSTD);
                LineProperties(xy_Vertical[GridSize + i], clrSTD, thickSTD);
            }

            //================DRAWING LINES ALONG Y======================
            //set negative grid lines------------------------
            //parallel to X, incrimented along Y
            xy_Horizontal[GridSize - i].SetPosition(0, X_negative + new Vector3(0, -y_incriment, 0));
            xy_Horizontal[GridSize - i].SetPosition(1, X_positive + new Vector3(0, -y_incriment, 0));

            //set positive grid lines------------------------
            //parallel to X, incrimented along Y
            xy_Horizontal[GridSize + i].SetPosition(0, X_negative + new Vector3(0, y_incriment, 0));
            xy_Horizontal[GridSize + i].SetPosition(1, X_positive + new Vector3(0, y_incriment, 0));


            //Colors and Thickness Incriments
            if (i % 10 == 0)
            {
                LineProperties(xy_Horizontal[GridSize - i], clr10incri, thick10incri);
                LineProperties(xy_Horizontal[GridSize + i], clr10incri, thick10incri);
            }
            else if (i % 5 == 0)
            {
                LineProperties(xy_Horizontal[GridSize - i], clr5incri, thick5incri);
                LineProperties(xy_Horizontal[GridSize + i], clr5incri, thick5incri);
            }
            else
            {
                LineProperties(xy_Horizontal[GridSize - i], clrSTD, thickSTD);
                LineProperties(xy_Horizontal[GridSize + i], clrSTD, thickSTD);
            }
        }
    }

    public void Draw3DGrid(Vector3 scale, Vector3 answer)
    {
        for (int i = 1; i < GridSize; i++)
        {
            //================DRAWING LINES ALONG X======================
            if (answer.x != 0)
            {
                float x_incriment = scale.x * i;
                //set negative grid lines-----------------------
                //parallel to Y, incrimented along X
                xy_Vertical[GridSize - i].SetPosition(0, Y_negative + new Vector3(-x_incriment, 0, 0));
                xy_Vertical[GridSize - i].SetPosition(1, Y_positive + new Vector3(-x_incriment, 0, 0));
                //parallel to Z, incrimented along X
                xz_Vertical[GridSize - i].SetPosition(0, Z_negative + new Vector3(-x_incriment, 0, 0));
                xz_Vertical[GridSize - i].SetPosition(1, Z_positive + new Vector3(-x_incriment, 0, 0));

                //set positive grid lines------------------------
                //parallel to Y, incrimented along X
                xy_Vertical[GridSize + i].SetPosition(0, Y_negative + new Vector3(x_incriment, 0, 0));
                xy_Vertical[GridSize + i].SetPosition(1, Y_positive + new Vector3(x_incriment, 0, 0));
                //parallel to Z, incrimented along X
                xz_Vertical[GridSize + i].SetPosition(0, Z_negative + new Vector3(x_incriment, 0, 0));
                xz_Vertical[GridSize + i].SetPosition(1, Z_positive + new Vector3(x_incriment, 0, 0));

                //Colors and Thickness Incriments
                if (i % 10 == 0)
                {
                    LineProperties(xy_Vertical[GridSize - i], clr10incri, thick10incri);
                    LineProperties(xy_Vertical[GridSize + i], clr10incri, thick10incri);

                    LineProperties(xz_Vertical[GridSize - i], clr10incri, thick10incri);
                    LineProperties(xz_Vertical[GridSize + i], clr10incri, thick10incri);
                }
                else if (i % 5 == 0)
                {
                    LineProperties(xy_Vertical[GridSize - i], clr5incri, thick5incri);
                    LineProperties(xy_Vertical[GridSize + i], clr5incri, thick5incri);

                    LineProperties(xz_Vertical[GridSize - i], clr5incri, thick5incri);
                    LineProperties(xz_Vertical[GridSize + i], clr5incri, thick5incri);
                }
                else
                {
                    LineProperties(xy_Vertical[GridSize - i], clrSTD, thickSTD);
                    LineProperties(xy_Vertical[GridSize + i], clrSTD, thickSTD);

                    LineProperties(xz_Vertical[GridSize - i], clrSTD, thickSTD);
                    LineProperties(xz_Vertical[GridSize + i], clrSTD, thickSTD);
                }
            }

            //================DRAWING LINES ALONG Y======================
            if (answer.y != 0)
            {
                float y_incriment = scale.y * i;
                //set negative grid lines------------------------
                //parallel to X, incrimented along Y
                xy_Horizontal[GridSize - i].SetPosition(0, X_negative + new Vector3(0, -y_incriment, 0));
                xy_Horizontal[GridSize - i].SetPosition(1, X_positive + new Vector3(0, -y_incriment, 0));
                //parallel to Z, incrimented along Y
                yz_Horizontal[GridSize - i].SetPosition(0, Z_negative + new Vector3(0, -y_incriment, 0));
                yz_Horizontal[GridSize - i].SetPosition(1, Z_positive + new Vector3(0, -y_incriment, 0));

                //set positive grid lines------------------------
                //parallel to X, incrimented along Y
                xy_Horizontal[GridSize + i].SetPosition(0, X_negative + new Vector3(0, y_incriment, 0));
                xy_Horizontal[GridSize + i].SetPosition(1, X_positive + new Vector3(0, y_incriment, 0));
                //parallel to Z, incrimented along Y
                yz_Horizontal[GridSize + i].SetPosition(0, Z_negative + new Vector3(0, y_incriment, 0));
                yz_Horizontal[GridSize + i].SetPosition(1, Z_positive + new Vector3(0, y_incriment, 0));


                //Colors and Thickness Incriments
                if (i % 10 == 0)
                {
                    LineProperties(xy_Horizontal[GridSize - i], clr10incri, thick10incri);
                    LineProperties(xy_Horizontal[GridSize + i], clr10incri, thick10incri);

                    LineProperties(yz_Horizontal[GridSize - i], clr10incri, thick10incri);
                    LineProperties(yz_Horizontal[GridSize + i], clr10incri, thick10incri);
                }
                else if (i % 5 == 0)
                {
                    LineProperties(xy_Horizontal[GridSize - i], clr5incri, thick5incri);
                    LineProperties(xy_Horizontal[GridSize + i], clr5incri, thick5incri);

                    LineProperties(yz_Horizontal[GridSize - i], clr5incri, thick5incri);
                    LineProperties(yz_Horizontal[GridSize + i], clr5incri, thick5incri);
                }
                else
                {
                    LineProperties(xy_Horizontal[GridSize - i], clrSTD, thickSTD);
                    LineProperties(xy_Horizontal[GridSize + i], clrSTD, thickSTD);

                    LineProperties(yz_Horizontal[GridSize - i], clrSTD, thickSTD);
                    LineProperties(yz_Horizontal[GridSize + i], clrSTD, thickSTD);
                }
            }

            //================DRAWING LINES ALONG Z======================
            if (answer.z != 0)
            {
                float z_incriment = scale.z * i;
                //set negative grid lines------------------------
                //parallel to Y, incrimented along Z
                yz_Vertical[GridSize - i].SetPosition(0, Y_negative + new Vector3(0, 0, -z_incriment));
                yz_Vertical[GridSize - i].SetPosition(1, Y_positive + new Vector3(0, 0, -z_incriment));
                //parallel to X, incrimented along Z
                xz_Horizontal[GridSize - i].SetPosition(0, X_negative + new Vector3(0, 0, -z_incriment));
                xz_Horizontal[GridSize - i].SetPosition(1, X_positive + new Vector3(0, 0, -z_incriment));

                //set positive grid lines------------------------
                //parallel to Y, incrimented along Z
                yz_Vertical[GridSize + i].SetPosition(0, Y_negative + new Vector3(0, 0, z_incriment));
                yz_Vertical[GridSize + i].SetPosition(1, Y_positive + new Vector3(0, 0, z_incriment));
                //parallel to X, incrimented along Z
                xz_Horizontal[GridSize + i].SetPosition(0, X_negative + new Vector3(0, 0, z_incriment));
                xz_Horizontal[GridSize + i].SetPosition(1, X_positive + new Vector3(0, 0, z_incriment));

                //Colors and Thickness Incriments
                if (i % 10 == 0)
                {
                    LineProperties(yz_Vertical[GridSize - i], clr10incri, thick10incri);
                    LineProperties(yz_Vertical[GridSize + i], clr10incri, thick10incri);

                    LineProperties(xz_Horizontal[GridSize - i], clr10incri, thick10incri);
                    LineProperties(xz_Horizontal[GridSize + i], clr10incri, thick10incri);
                }
                else if (i % 5 == 0)
                {
                    LineProperties(yz_Vertical[GridSize - i], clr5incri, thick5incri);
                    LineProperties(yz_Vertical[GridSize + i], clr5incri, thick5incri);

                    LineProperties(xz_Horizontal[GridSize - i], clr5incri, thick5incri);
                    LineProperties(xz_Horizontal[GridSize + i], clr5incri, thick5incri);
                }
                else
                {
                    LineProperties(yz_Vertical[GridSize - i], clrSTD, thickSTD);
                    LineProperties(yz_Vertical[GridSize + i], clrSTD, thickSTD);

                    LineProperties(xz_Horizontal[GridSize - i], clrSTD, thickSTD);
                    LineProperties(xz_Horizontal[GridSize + i], clrSTD, thickSTD);
                }
            }
        }
    }

    public void ToggleXYplane(bool val)
    {
        xy_grid.SetActive(val);
    }
    public void ToggleYZplane(bool val)
    {
        yz_grid.SetActive(val);
    }
    public void ToggleXZplane(bool val)
    {
        xz_grid.SetActive(val);
    }

    public void ToggleGrid(bool val)
    {
        xy_grid.SetActive(val);
        yz_grid.SetActive(val);
        xz_grid.SetActive(val);

        switch(quadrant)
        {
            case -2: //1D "grid" Y direction
                xy_gridHori_Yneg.gameObject.SetActive(false);
                xy_gridHori_Ypos.gameObject.SetActive(false);

                xy_gridVert_Xneg.gameObject.SetActive(val);
                xy_gridVert_Xpos.gameObject.SetActive(val);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(false);
                yz_gridHori_Ypos.gameObject.SetActive(false);

                yz_gridVert_Zneg.gameObject.SetActive(false);
                yz_gridVert_Zpos.gameObject.SetActive(false);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(false);
                xz_gridHori_Zpos.gameObject.SetActive(false);

                xz_gridVert_Xneg.gameObject.SetActive(false);
                xz_gridVert_Xpos.gameObject.SetActive(false);
                break;

            case -1: //1D "grid" X direction
                xy_gridHori_Yneg.gameObject.SetActive(val);
                xy_gridHori_Ypos.gameObject.SetActive(val);

                xy_gridVert_Xneg.gameObject.SetActive(false);
                xy_gridVert_Xpos.gameObject.SetActive(false);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(false);
                yz_gridHori_Ypos.gameObject.SetActive(false);

                yz_gridVert_Zneg.gameObject.SetActive(false);
                yz_gridVert_Zpos.gameObject.SetActive(false);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(false);
                xz_gridHori_Zpos.gameObject.SetActive(false);

                xz_gridVert_Xneg.gameObject.SetActive(false);
                xz_gridVert_Xpos.gameObject.SetActive(false);

                break;

            case 0: //2D XY Grid
                xy_gridHori_Yneg.gameObject.SetActive(val);
                xy_gridHori_Ypos.gameObject.SetActive(val);

                xy_gridVert_Xneg.gameObject.SetActive(val);
                xy_gridVert_Xpos.gameObject.SetActive(val);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(false);
                yz_gridHori_Ypos.gameObject.SetActive(false);

                yz_gridVert_Zneg.gameObject.SetActive(false);
                yz_gridVert_Zpos.gameObject.SetActive(false);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(false);
                xz_gridHori_Zpos.gameObject.SetActive(false);

                xz_gridVert_Xneg.gameObject.SetActive(false);
                xz_gridVert_Xpos.gameObject.SetActive(false);
                break;

            case 1:// <1,1,1>
                xy_gridHori_Yneg.gameObject.SetActive(false);
                xy_gridHori_Ypos.gameObject.SetActive(val);

                xy_gridVert_Xneg.gameObject.SetActive(false);
                xy_gridVert_Xpos.gameObject.SetActive(val);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(false);
                yz_gridHori_Ypos.gameObject.SetActive(val);

                yz_gridVert_Zneg.gameObject.SetActive(false);
                yz_gridVert_Zpos.gameObject.SetActive(val);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(false);
                xz_gridHori_Zpos.gameObject.SetActive(val);

                xz_gridVert_Xneg.gameObject.SetActive(false);
                xz_gridVert_Xpos.gameObject.SetActive(val);
                break;

            case 2: // <1,1,-1>
                xy_gridHori_Yneg.gameObject.SetActive(false);
                xy_gridHori_Ypos.gameObject.SetActive(val);

                xy_gridVert_Xneg.gameObject.SetActive(false);
                xy_gridVert_Xpos.gameObject.SetActive(val);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(false);
                yz_gridHori_Ypos.gameObject.SetActive(val);

                yz_gridVert_Zneg.gameObject.SetActive(val);
                yz_gridVert_Zpos.gameObject.SetActive(false);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(val);
                xz_gridHori_Zpos.gameObject.SetActive(false);

                xz_gridVert_Xneg.gameObject.SetActive(false);
                xz_gridVert_Xpos.gameObject.SetActive(val);
                break;

            case 3: // <1,-1,1>
                xy_gridHori_Yneg.gameObject.SetActive(val);
                xy_gridHori_Ypos.gameObject.SetActive(false);

                xy_gridVert_Xneg.gameObject.SetActive(false);
                xy_gridVert_Xpos.gameObject.SetActive(val);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(val);
                yz_gridHori_Ypos.gameObject.SetActive(false);

                yz_gridVert_Zneg.gameObject.SetActive(false);
                yz_gridVert_Zpos.gameObject.SetActive(val);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(false);
                xz_gridHori_Zpos.gameObject.SetActive(val);

                xz_gridVert_Xneg.gameObject.SetActive(false);
                xz_gridVert_Xpos.gameObject.SetActive(val);

                break;

            case 4: // <1,-1,-1>
                xy_gridHori_Yneg.gameObject.SetActive(val);
                xy_gridHori_Ypos.gameObject.SetActive(false);

                xy_gridVert_Xneg.gameObject.SetActive(false);
                xy_gridVert_Xpos.gameObject.SetActive(val);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(val);
                yz_gridHori_Ypos.gameObject.SetActive(false);

                yz_gridVert_Zneg.gameObject.SetActive(val);
                yz_gridVert_Zpos.gameObject.SetActive(false);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(val);
                xz_gridHori_Zpos.gameObject.SetActive(false);

                xz_gridVert_Xneg.gameObject.SetActive(false);
                xz_gridVert_Xpos.gameObject.SetActive(val);

                break;

            case 5: // <-1,1,1>
                xy_gridHori_Yneg.gameObject.SetActive(false);
                xy_gridHori_Ypos.gameObject.SetActive(val);

                xy_gridVert_Xneg.gameObject.SetActive(val);
                xy_gridVert_Xpos.gameObject.SetActive(false);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(false);
                yz_gridHori_Ypos.gameObject.SetActive(val);

                yz_gridVert_Zneg.gameObject.SetActive(false);
                yz_gridVert_Zpos.gameObject.SetActive(val);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(false);
                xz_gridHori_Zpos.gameObject.SetActive(val);

                xz_gridVert_Xneg.gameObject.SetActive(val);
                xz_gridVert_Xpos.gameObject.SetActive(false);

                break;

            case 6: // <-1,1,-1>
                xy_gridHori_Yneg.gameObject.SetActive(false);
                xy_gridHori_Ypos.gameObject.SetActive(val);

                xy_gridVert_Xneg.gameObject.SetActive(val);
                xy_gridVert_Xpos.gameObject.SetActive(false);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(false);
                yz_gridHori_Ypos.gameObject.SetActive(val);

                yz_gridVert_Zneg.gameObject.SetActive(val);
                yz_gridVert_Zpos.gameObject.SetActive(false);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(val);
                xz_gridHori_Zpos.gameObject.SetActive(false);

                xz_gridVert_Xneg.gameObject.SetActive(val);
                xz_gridVert_Xpos.gameObject.SetActive(false);

                break;

            case 7: // <-1,-1,1>
                xy_gridHori_Yneg.gameObject.SetActive(val);
                xy_gridHori_Ypos.gameObject.SetActive(false);

                xy_gridVert_Xneg.gameObject.SetActive(val);
                xy_gridVert_Xpos.gameObject.SetActive(false);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(val);
                yz_gridHori_Ypos.gameObject.SetActive(false);

                yz_gridVert_Zneg.gameObject.SetActive(false);
                yz_gridVert_Zpos.gameObject.SetActive(val);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(false);
                xz_gridHori_Zpos.gameObject.SetActive(val);

                xz_gridVert_Xneg.gameObject.SetActive(val);
                xz_gridVert_Xpos.gameObject.SetActive(false);

                break;

            case 8: // <-1,-1,-1>
                xy_gridHori_Yneg.gameObject.SetActive(val);
                xy_gridHori_Ypos.gameObject.SetActive(false);

                xy_gridVert_Xneg.gameObject.SetActive(val);
                xy_gridVert_Xpos.gameObject.SetActive(false);
                //--------------
                yz_gridHori_Yneg.gameObject.SetActive(val);
                yz_gridHori_Ypos.gameObject.SetActive(false);

                yz_gridVert_Zneg.gameObject.SetActive(val);
                yz_gridVert_Zpos.gameObject.SetActive(false);
                //--------------
                xz_gridHori_Zneg.gameObject.SetActive(val);
                xz_gridHori_Zpos.gameObject.SetActive(false);

                xz_gridVert_Xneg.gameObject.SetActive(val);
                xz_gridVert_Xpos.gameObject.SetActive(false);

                break;
        }
    }

}
