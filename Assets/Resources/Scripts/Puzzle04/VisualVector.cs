
/**


**/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class VisualVector : MonoBehaviour
{
    private Vector3 puzzleScale;
    private float overallScale;

    //Puzzle Vector Components
    [Header("---Puzzle Vector Components---")]
    public GameObject gapVectorVV;
    public GameObject start;
    public GameObject goal;

    //Player Vector Components
    [Header("---Player Vector Components---")]
    public GameObject vector1;
    public GameObject vector2;
    public GameObject vector1ball;
    public GameObject vector2ball;
    public GameObject finalVector;
    public GameObject vector1end;
    public GameObject vector2end;

    //Grid Vector Components
    [Header("---Grid Vector Components---")]
    public Transform GridMain;
    public Transform GridLines;
    public Transform GridMarks;
    public GameObject Xvector;
    public GameObject Yvector;
    public GameObject Zvector;
    public GameObject Xnegative;
    public GameObject Xpositive;
    public GameObject Ynegative;
    public GameObject Ypositive; 
    public GameObject Znegative;
    public GameObject Zpositive;
    public GameObject Xlabel;
    public GameObject Ylabel;
    public GameObject Zlabel;
    private List<GameObject> XSphereNodes;
    private List<GameObject> YSphereNodes;
    private List<GameObject> ZSphereNodes;
    private List<GameObject> XBarNodes;
    private List<GameObject> YBarNodes;
    private List<GameObject> YZHoriBarNodes;
    private List<GameObject> YZVertBarNodes;
    private List<GameObject> XZHoriBarNodes;
    private List<GameObject> XZVertBarNodes;

    [Header("---Grid Color Components---")]
    public float axisThickness = 0.18f;
    public float gridThickness = 0.09f;
    public float gridNodeScale = 0.6f;
    public int gridSize = 100;
    private Color gridOutline = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    private Color grid5incriOutline = new Color(0.2f, 0.2f, 0.0f, 0.4f);
    private Color grid10incriOutline = new Color(0.5f, 0.5f, 0.0f, 0.4f);
    public Material MainAxis;
    public Material NodeDefualt;
    public Material GridDefualt;
    public Material Grid5Incriment;
    public Material Grid10Incriment;
    private int quadrant;
    private int xMin, xMax, yMin, yMax, zMin, zMax;

    [Header("---3D Vector Projetion Components---")]
    public GameObject projections;
    public GameObject anchors;
    public GameObject XYplane;
    public GameObject XYanchors;
    public GameObject XZplane;
    public GameObject XZanchors;
    public GameObject YZplane;
    public GameObject YZanchors;
    private bool isProjecting;
    private bool isAnchored;

    //xy plane parts
    [Header("XY plane")]
    public GameObject XYvector1;
    public GameObject XYvector2;
    public GameObject XYfinal;
    public GameObject XYvector1end;
    public GameObject XYvector2end;
    public GameObject XYgoalPoint;
    //xz plane parts
    [Header("XZ plane")]
    public GameObject XZvector1;
    public GameObject XZvector2;
    public GameObject XZfinal;
    public GameObject XZvector1end;
    public GameObject XZvector2end;
    public GameObject XZgoalPoint;
    //yz plane parts
    [Header("YZ plane")]
    public GameObject YZvector1;
    public GameObject YZvector2;
    public GameObject YZfinal;
    public GameObject YZvector1end;
    public GameObject YZvector2end;
    public GameObject YZgoalPoint;
    //Grid Anchor Parts
    [Header("Anchors")]
    public GameObject XYanchor1;
    public GameObject XZanchor1;
    public GameObject YZanchor1;
    public GameObject XYanchor2;
    public GameObject XZanchor2;
    public GameObject YZanchor2;
    public GameObject XYanchorGoal;
    public GameObject XZanchorGoal;
    public GameObject YZanchorGoal;



    //Misc Controls
    private bool fudgeX;
    private bool fudgeY;
    private bool fudgeZ;


    // Start is called before the first frame update
    void Start()
    { 
        gapVectorVV.SetActive(false);

        vector1.SetActive(false);
        vector2.SetActive(false);
        vector1ball.SetActive(false);
        vector2ball.SetActive(false);
        finalVector.SetActive(false);


        Xvector.SetActive(false);
        Yvector.SetActive(false);
        Zvector.SetActive(false);
        Xlabel.SetActive(false);
        Ylabel.SetActive(false);
        Zlabel.SetActive(false);

        projections.SetActive(false);
        anchors.SetActive(false);

        XSphereNodes = new List<GameObject>();
        YSphereNodes = new List<GameObject>();
        ZSphereNodes = new List<GameObject>();

        XBarNodes = new List<GameObject>();
        YBarNodes = new List<GameObject>();
        YZHoriBarNodes = new List<GameObject>();
        YZVertBarNodes = new List<GameObject>();
        XZHoriBarNodes = new List<GameObject>();
        XZVertBarNodes = new List<GameObject>();

        xMin = -gridSize;
        xMax = gridSize;
        yMin = -gridSize;
        yMax = gridSize;
        zMin = -gridSize;
        zMax = gridSize;

        createAxis();
        createBarGrid();

        isProjecting = false;
        isAnchored = false;
    }
    public void InitVisVector()
    {
        setFudge();

        start.transform.position = GameControllerPuzzle04.instance.currPuzzle.startPoint.transform.position;
        goal.transform.position = GameControllerPuzzle04.instance.currPuzzle.goalPoint.transform.position;

        overallScale = setOverallScale();
        puzzleScale = setPuzzleScale();

        VectorBetweenPoints(gapVectorVV, goal.transform.position, start.transform.position, 0.25f);

        if (GameControllerPuzzle04.instance.Difficulty == 3)
        {
            quadrant = determineQuadrant();
            limitGraphSize();
        }

        GridMain.position = start.transform.position;
        placeAxisLabels();

        XYgoalPoint.transform.position = new Vector3(goal.transform.position.x, goal.transform.position.y, start.transform.position.z);
        XZgoalPoint.transform.position = new Vector3(goal.transform.position.x, start.transform.position.y, goal.transform.position.z);
        YZgoalPoint.transform.position = new Vector3(start.transform.position.x, goal.transform.position.y, goal.transform.position.z);

        VectorBetweenPoints(XYanchorGoal, XYgoalPoint.transform.position, goal.transform.position, gridThickness);
        VectorBetweenPoints(XZanchorGoal, XZgoalPoint.transform.position, goal.transform.position, gridThickness);
        VectorBetweenPoints(YZanchorGoal, YZgoalPoint.transform.position, goal.transform.position, gridThickness);

        VectorBetweenPoints(Xvector, Xnegative.transform.position, Xpositive.transform.position, axisThickness);
        VectorBetweenPoints(Yvector, Ynegative.transform.position, Ypositive.transform.position, axisThickness);
        VectorBetweenPoints(Zvector, Znegative.transform.position, Zpositive.transform.position, axisThickness);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControllerPuzzle04.instance.currPuzzle == null)
            return;

        if (GameControllerPuzzle04.instance.currPuzzle.isActive)
        { 
            //Updates for 3D vector projections
            if (GameControllerPuzzle04.instance.Difficulty == 3 && isProjecting)
            {
                //XY plane
                XYvector1end.transform.position = new Vector3(vector1end.transform.position.x, vector1end.transform.position.y, start.transform.position.z);
                XYvector2end.transform.position = new Vector3(vector2end.transform.position.x, vector2end.transform.position.y, start.transform.position.z);
                VectorBetweenPoints(XYvector1, start.transform.position, XYvector1end.transform.position, 0.25f);
                VectorBetweenPoints(XYvector2, XYvector1end.transform.position, XYvector2end.transform.position, 0.25f);

                //XZ plane
                XZvector1end.transform.position = new Vector3(vector1end.transform.position.x, start.transform.position.y, vector1end.transform.position.z);
                XZvector2end.transform.position = new Vector3(vector2end.transform.position.x, start.transform.position.y, vector2end.transform.position.z);
                VectorBetweenPoints(XZvector1, start.transform.position, XZvector1end.transform.position, 0.25f);
                VectorBetweenPoints(XZvector2, XZvector1end.transform.position, XZvector2end.transform.position, 0.25f);

                //YZ plane
                YZvector1end.transform.position = new Vector3(start.transform.position.x, vector1end.transform.position.y, vector1end.transform.position.z);
                YZvector2end.transform.position = new Vector3(start.transform.position.x, vector2end.transform.position.y, vector2end.transform.position.z);
                VectorBetweenPoints(YZvector1, start.transform.position, YZvector1end.transform.position, 0.25f);
                VectorBetweenPoints(YZvector2, YZvector1end.transform.position, YZvector2end.transform.position, 0.25f);

                if (GameControllerPuzzle04.instance.P04W.card1 != null && GameControllerPuzzle04.instance.P04W.card2 != null)
                {
                    if (GameControllerPuzzle04.instance.P04W.getXYtoggle())
                    {
                        XYfinal.SetActive(GameControllerPuzzle04.instance.P04W.getXYtoggle());
                        VectorBetweenPoints(XYfinal, start.transform.position, XYvector2end.transform.position, 0.25f);
                    }
                    else
                        XYfinal.SetActive(false);

                    if (GameControllerPuzzle04.instance.P04W.getXZtoggle())
                    {
                        XZfinal.SetActive(true);
                        VectorBetweenPoints(XZfinal, start.transform.position, XZvector2end.transform.position, 0.25f);
                    }
                    else
                        XZfinal.SetActive(false);

                    if (GameControllerPuzzle04.instance.P04W.getYZtoggle())
                    {
                        YZfinal.SetActive(true);
                        VectorBetweenPoints(YZfinal, start.transform.position, YZvector2end.transform.position, 0.25f);
                    }
                    else
                        YZfinal.SetActive(false);
                }
                else
                {
                    XYfinal.SetActive(false);
                    XZfinal.SetActive(false);
                    YZfinal.SetActive(false);
                }


                if (isAnchored)
                {
                    VectorBetweenPoints(XYanchor1, XYvector1end.transform.position, vector1end.transform.position, gridThickness);
                    VectorBetweenPoints(XYanchor2, XYvector2end.transform.position, vector2end.transform.position, gridThickness);

                    VectorBetweenPoints(XZanchor1, XZvector1end.transform.position, vector1end.transform.position, gridThickness);
                    VectorBetweenPoints(XZanchor2, XZvector2end.transform.position, vector2end.transform.position, gridThickness);

                    VectorBetweenPoints(YZanchor1, YZvector1end.transform.position, vector1end.transform.position, gridThickness);
                    VectorBetweenPoints(YZanchor2, YZvector2end.transform.position, vector2end.transform.position, gridThickness);
                }

            }

            if(GameControllerPuzzle04.instance.P04W.getAxisStatus())
            {
                Xlabel.transform.LookAt(GameControllerPuzzle04.instance.currPuzzle.visVectCamera.transform.position, -Vector3.up);
                Ylabel.transform.LookAt(GameControllerPuzzle04.instance.currPuzzle.visVectCamera.transform.position, -Vector3.up);
                Zlabel.transform.LookAt(GameControllerPuzzle04.instance.currPuzzle.visVectCamera.transform.position, -Vector3.up);
            }
        }

    }


    private float setOverallScale()
    {
        Vector3 worldVector, gapVector;
        float worldMag, gapMag;

        worldVector = start.transform.position - goal.transform.position;
        gapVector = GameControllerPuzzle04.instance.currPuzzle.getAnswerVector();

        worldMag = worldVector.magnitude;
        gapMag = gapVector.magnitude;

        return worldMag / gapMag;
    }


    private Vector3 setPuzzleScale()
    {
        Vector3 worldVector, gapVector, returnVector;  

        worldVector = start.transform.position - goal.transform.position;
        gapVector = GameControllerPuzzle04.instance.currPuzzle.getAnswerVector();

        //size you want, divided by the size you have
        //X-scale
        if (gapVector.x != 0)
            returnVector.x = Mathf.Abs(worldVector.x / gapVector.x);
        else
            returnVector.x = 0;

        //Z-scale
        if (gapVector.y != 0)
            returnVector.y = Mathf.Abs(worldVector.y / gapVector.y);
        else
            returnVector.y = 0;

        //Y-scale
        if (gapVector.z != 0)
            returnVector.z = Mathf.Abs(worldVector.z / gapVector.z);
        else
            returnVector.z = 0;

        
        return returnVector;
    }

    public Vector3 rescaledVector(Vector3 original)
    {
        Vector3 temp, worldVector, gapVector;

        worldVector = start.transform.position - goal.transform.position;
        gapVector = GameControllerPuzzle04.instance.currPuzzle.getAnswerVector();

        //rescale X
        if (gapVector.x != 0)
            temp.x = original.x * puzzleScale.x;
        else if (fudgeX)
            temp.x = original.x - worldVector.x / 2;
        else
            temp.x = original.x - worldVector.x;

        //rescale Y
        if (gapVector.y != 0)
            temp.y = original.y * puzzleScale.y;
        else if (fudgeY)
            temp.y = original.y - worldVector.y / 2;
        else
            temp.y = original.y - worldVector.y;

        //rescale Z
        if (gapVector.z != 0)
            temp.z = original.z * puzzleScale.z;
        else if (fudgeZ)
            temp.z = original.z - worldVector.z / 2;
        else
            temp.z = original.z - worldVector.z;


        return temp;
    }


    private void VectorBetweenPoints(GameObject visVector, Vector3 begin, Vector3 end, float width)
    {
        Vector3 offset = end - begin;
        Vector3 localScale = new Vector3(width, (offset.magnitude / 2.0f), width);
        Vector3 position = begin + (offset / 2.0f) ;

        visVector.transform.up = offset;
        visVector.transform.position = position;
        visVector.transform.localScale = localScale;
    }

    /// <summary>
    /// Turns on/off flags to "fudge" the final landing spot of the last vector.
    /// This is becuase sometimes the game Physical start/end vectors vs the PC04 gap entered
    /// is not exactly the same.  This is especially true if the entered gapVector
    /// contains a zero, in which case making a scale factor is impossible (divide by zero).
    /// 
    /// NOTE: This fudge offset only fixes VERY MINOR discrepancies, if the difference
    /// is too large it will look extremely wrong visually.
    /// </summary>
    private void setFudge()
    {
        switch(GameControllerPuzzle04.instance.currPuzzle.getDirection())
        {
            case Direction.X:
                fudgeX = false;
                fudgeY = true;
                fudgeZ = true;
                break;
            case Direction.Y:
                fudgeX = true;
                fudgeY = false;
                fudgeZ = true;
                break;
            case Direction.XY:
                fudgeX = false;
                fudgeY = false;
                fudgeZ = true;
                break;
            case Direction.XYZ:
                fudgeX = false;
                fudgeY = false;
                fudgeZ = false;
                break;
        }

    }


    public GameObject getGapVector()
    {
        return gapVectorVV;
    }

    public void Vector1EventList()
    {
        if(GameControllerPuzzle04.instance.currPuzzle != null)
        {
            if(GameControllerPuzzle04.instance.P04W.card1 != null)
            {
                ToggleVector1(true);
                vector1end.transform.position = start.transform.position + rescaledVector(GameControllerPuzzle04.instance.P04W.card1.cardVector * GameControllerPuzzle04.instance.P04W.getScalar1Value());
                vector1.GetComponent<LineRenderer>().SetPosition(0, start.transform.position);
                vector1.GetComponent<LineRenderer>().SetPosition(1, vector1end.transform.position);
            }
            else
            {
                vector1end.transform.position = start.transform.position;
                ToggleVector1(false);
            }
            Vector2EventList();
        }
    }
    public void Vector2EventList()
    {
        if (GameControllerPuzzle04.instance.currPuzzle != null)
        {
            if (GameControllerPuzzle04.instance.P04W.card2 != null)
            {
                ToggleVector2(true);

                vector2end.transform.position = vector1end.transform.position + rescaledVector(GameControllerPuzzle04.instance.P04W.card2.cardVector * GameControllerPuzzle04.instance.P04W.getScalar2Value());
                vector2.GetComponent<LineRenderer>().SetPosition(0, vector1end.transform.position);
                vector2.GetComponent<LineRenderer>().SetPosition(1, vector2end.transform.position);
            }
            else
            {
                vector2end.transform.position = vector1end.transform.position;
                ToggleVector2(false);
            }
        }
    }

    public void FinalVectEventList()
    {
        if (GameControllerPuzzle04.instance.P04W.card1 != null && GameControllerPuzzle04.instance.P04W.card2 != null)
        {
            finalVector.SetActive(true);
            finalVector.GetComponent<LineRenderer>().SetPosition(0, start.transform.position);
            finalVector.GetComponent<LineRenderer>().SetPosition(1, vector2end.transform.position);
        }
        else
        {
            finalVector.SetActive(false);
        }
    }

    public void ToggleVector1(bool val)
    {
        vector1.SetActive(val);
        vector1ball.SetActive(val);
    }

    public void ToggleVector2(bool val)
    {
        vector2.SetActive(val);
        vector2ball.SetActive(val);
    }

    public GameObject getFinalVector()
    {
        return finalVector;
    }

    public GameObject getXvector()
    {
        return Xvector;
    }
    public GameObject getYvector()
    {
        return Yvector;
    }
    public GameObject getZvector()
    {
        return Zvector;
    }


    public int determineQuadrant()
    {
        if(GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().x > 0)
        {
            if(GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().y > 0)
            {
                if(GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().z > 0)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }

            }
            else
            {
                if (GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().z > 0)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }

            }

        }
        else
        {
            if (GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().y > 0)
            {
                if (GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().z > 0)
                {
                    return 5;
                }
                else
                {
                    return 6;
                }

            }
            else
            {
                if (GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().z > 0)
                {
                    return 7;
                }
                else
                {
                    return 8;
                }
            }

        }

    }

    public void limitGraphSize()
    {

        switch(quadrant)
        {
            case 1:
                Xnegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ynegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Znegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMin = 0;
                yMin = 0;
                zMin = 0;
                break;
            case 2:
                Xnegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ynegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Zpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMin = 0;
                yMin = 0;
                zMax = 0;
                break;
            case 3:
                Xnegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ypositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Znegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMin = 0;
                yMax = 0;
                zMin = 0;
                break;
            case 4:
                Xnegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ypositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Zpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMin = 0;
                yMax = 0;
                zMax = 0;
                break;
            case 5:
                Xpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ynegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Znegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMax = 0;
                yMin = 0;
                zMin = 0;
                break;
            case 6:
                Xpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ynegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                Zpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMax = 0;
                yMin = 0;
                zMax = 0;
                break;
            case 7:
                Xpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ypositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Znegative.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMax = 0;
                yMax = 0;
                zMin = 0;
                break;
            case 8:
                Xpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Ypositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                Zpositive.transform.localPosition = new Vector3(0f, 0f, 0f);
                xMax = 0;
                yMax = 0;
                zMax = 0;
                break;

        }
    }


    public void createAxis()
    {
        for(int i = -gridSize; i < gridSize; i++)
        {
            GameObject xSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            xSphere.transform.localScale *= gridNodeScale;
            xSphere.GetComponent<Renderer>().material = NodeDefualt;
            if (i % 5 == 0)
                xSphere.GetComponent<Renderer>().material = Grid5Incriment;
            if(i % 10 == 0)
                xSphere.GetComponent<Renderer>().material = Grid10Incriment;
            xSphere.GetComponent<Collider>().enabled = false;
            xSphere.GetComponent<Renderer>().shadowCastingMode = 0;
            xSphere.GetComponent<Renderer>().receiveShadows = false;
            xSphere.layer = 10;
            xSphere.transform.parent = GridMarks;
            XSphereNodes.Add(xSphere);

            GameObject ySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ySphere.transform.localScale *= gridNodeScale;
            ySphere.GetComponent<Renderer>().material = NodeDefualt;
            if (i % 5 == 0)
                ySphere.GetComponent<Renderer>().material = Grid5Incriment;
            if (i % 10 == 0)
                ySphere.GetComponent<Renderer>().material = Grid10Incriment;
            ySphere.GetComponent<Collider>().enabled = false;
            ySphere.GetComponent<Renderer>().shadowCastingMode = 0;
            ySphere.GetComponent<Renderer>().receiveShadows = false;
            ySphere.layer = 10;
            ySphere.transform.parent = GridMarks;

            YSphereNodes.Add(ySphere);

            if (GameControllerPuzzle04.instance.Difficulty == 3)
            {
                GameObject zSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                zSphere.transform.localScale *= gridNodeScale;
                zSphere.GetComponent<Renderer>().material = NodeDefualt;
                if (i % 5 == 0)
                    zSphere.GetComponent<Renderer>().material = Grid5Incriment;
                if (i % 10 == 0)
                    zSphere.GetComponent<Renderer>().material = Grid10Incriment;
                zSphere.GetComponent<Collider>().enabled = false;
                zSphere.GetComponent<Renderer>().shadowCastingMode = 0;
                zSphere.GetComponent<Renderer>().receiveShadows = false;
                zSphere.layer = 10;
                zSphere.transform.parent = GridMarks;

                ZSphereNodes.Add(zSphere);
            }
        }


        deactivateAxis();

    }

    private void placeAxisLabels()
    {
        if(GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().x >= 0) //if x is positive
        {
            Xlabel.transform.position += new Vector3(2 * puzzleScale.x, 0, 0);
            Xlabel.GetComponent<TextMeshPro>().text = "+ X";
        }
        else
        {
            Xlabel.transform.position += new Vector3(-2 * puzzleScale.x, 0, 0);
            Xlabel.GetComponent<TextMeshPro>().text = "- X";

        }

        if (GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().y >= 0) //if y is positive
        {
            Ylabel.transform.position += new Vector3(0, 2 * puzzleScale.y, 0);
            Ylabel.GetComponent<TextMeshPro>().text = "+ Z";

        }
        else
        {
            Ylabel.transform.position += new Vector3(0, -2 * puzzleScale.y, 0);
            Ylabel.GetComponent<TextMeshPro>().text = "- Z";
        }

        if (GameControllerPuzzle04.instance.currPuzzle.getAnswerVector().z >= 0) //if y is positive
        {
            Zlabel.transform.position += new Vector3(0, 0, 2 * puzzleScale.z);
            Zlabel.GetComponent<TextMeshPro>().text = "+ Y";
        }
        else
        {
            Zlabel.transform.position += new Vector3(0, 0, -2 * puzzleScale.z);
            Zlabel.GetComponent<TextMeshPro>().text = "- Y";
        }

    }

    public void createBarGrid()
    {
        for (int i = -gridSize; i < gridSize; i++)
        {
            GameObject xCyln = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            xCyln.AddComponent<Outline>();
            xCyln.GetComponent<Outline>().OutlineMode = Outline.Mode.SilhouetteOnly;
            xCyln.GetComponent<Outline>().OutlineColor = gridOutline;
            xCyln.GetComponent<Renderer>().material = GridDefualt;
            if (i % 5 == 0)
            {
                xCyln.GetComponent<Renderer>().material = Grid5Incriment;
                xCyln.GetComponent<Outline>().OutlineColor = grid5incriOutline;
            }
            if (i % 10 == 0)
            {
                xCyln.GetComponent<Renderer>().material = Grid10Incriment;
                xCyln.GetComponent<Outline>().OutlineColor = grid10incriOutline;
            }
            xCyln.GetComponent<Collider>().enabled = false;
            xCyln.GetComponent<Renderer>().shadowCastingMode = 0;
            xCyln.GetComponent<Renderer>().receiveShadows = false;
            xCyln.layer = 10;
            xCyln.transform.parent = GridLines;

            XBarNodes.Add(xCyln);

            GameObject yCyln = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            yCyln.AddComponent<Outline>();
            yCyln.GetComponent<Outline>().OutlineMode = Outline.Mode.SilhouetteOnly;
            yCyln.GetComponent<Outline>().OutlineColor = gridOutline;
            yCyln.GetComponent<Renderer>().material = GridDefualt;
            if (i % 5 == 0)
            {
                yCyln.GetComponent<Renderer>().material = Grid5Incriment;
                yCyln.GetComponent<Outline>().OutlineColor = grid5incriOutline;
            }
            if (i % 10 == 0)
            {
                yCyln.GetComponent<Renderer>().material = Grid10Incriment;
                yCyln.GetComponent<Outline>().OutlineColor = grid10incriOutline;
            }
            yCyln.GetComponent<Collider>().enabled = false;
            yCyln.GetComponent<Renderer>().shadowCastingMode = 0;
            yCyln.GetComponent<Renderer>().receiveShadows = false;
            yCyln.layer = 10;
            yCyln.transform.parent = GridLines;


            YBarNodes.Add(yCyln);

            if (GameControllerPuzzle04.instance.Difficulty == 3)
            {
                GameObject yzHoriCyln = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                yzHoriCyln.AddComponent<Outline>();
                yzHoriCyln.GetComponent<Outline>().OutlineMode = Outline.Mode.SilhouetteOnly;
                yzHoriCyln.GetComponent<Outline>().OutlineColor = gridOutline;
                yzHoriCyln.GetComponent<Renderer>().material = GridDefualt;
                if (i % 5 == 0)
                {
                    yzHoriCyln.GetComponent<Renderer>().material = Grid5Incriment;
                    yzHoriCyln.GetComponent<Outline>().OutlineColor = grid5incriOutline;
                }
                if (i % 10 == 0)
                {
                    yzHoriCyln.GetComponent<Renderer>().material = Grid10Incriment;
                    yzHoriCyln.GetComponent<Outline>().OutlineColor = grid10incriOutline;
                }
                yzHoriCyln.GetComponent<Collider>().enabled = false;
                yzHoriCyln.GetComponent<Renderer>().shadowCastingMode = 0;
                yzHoriCyln.GetComponent<Renderer>().receiveShadows = false;
                yzHoriCyln.layer = 10;
                yzHoriCyln.transform.parent = GridLines;


                YZHoriBarNodes.Add(yzHoriCyln);

                GameObject yzVertCyln = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                yzVertCyln.AddComponent<Outline>();
                yzVertCyln.GetComponent<Outline>().OutlineMode = Outline.Mode.SilhouetteOnly;
                yzVertCyln.GetComponent<Outline>().OutlineColor = gridOutline;
                yzVertCyln.GetComponent<Renderer>().material = GridDefualt;
                if (i % 5 == 0)
                {
                    yzVertCyln.GetComponent<Renderer>().material = Grid5Incriment;
                    yzVertCyln.GetComponent<Outline>().OutlineColor = grid5incriOutline;
                }
                if (i % 10 == 0)
                {
                    yzVertCyln.GetComponent<Renderer>().material = Grid10Incriment;
                    yzVertCyln.GetComponent<Outline>().OutlineColor = grid10incriOutline;
                }
                yzVertCyln.GetComponent<Collider>().enabled = false;
                yzVertCyln.GetComponent<Renderer>().shadowCastingMode = 0;
                yzVertCyln.GetComponent<Renderer>().receiveShadows = false;
                yzVertCyln.layer = 10;
                yzVertCyln.transform.parent = GridLines;


                YZVertBarNodes.Add(yzVertCyln);

                GameObject xzHoriCyln = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                xzHoriCyln.AddComponent<Outline>();
                xzHoriCyln.GetComponent<Outline>().OutlineMode = Outline.Mode.SilhouetteOnly;
                xzHoriCyln.GetComponent<Outline>().OutlineColor = gridOutline;
                xzHoriCyln.GetComponent<Renderer>().material = GridDefualt;
                if (i % 5 == 0)
                {
                    xzHoriCyln.GetComponent<Renderer>().material = Grid5Incriment;
                    xzHoriCyln.GetComponent<Outline>().OutlineColor = grid5incriOutline;
                }
                if (i % 10 == 0)
                {
                    xzHoriCyln.GetComponent<Renderer>().material = Grid10Incriment;
                    xzHoriCyln.GetComponent<Outline>().OutlineColor = grid10incriOutline;
                }
                xzHoriCyln.GetComponent<Collider>().enabled = false;
                xzHoriCyln.GetComponent<Renderer>().shadowCastingMode = 0;
                xzHoriCyln.GetComponent<Renderer>().receiveShadows = false;
                xzHoriCyln.layer = 10;
                xzHoriCyln.transform.parent = GridLines;


                XZHoriBarNodes.Add(xzHoriCyln);

                GameObject xzVertCyln = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                xzVertCyln.AddComponent<Outline>();
                xzVertCyln.GetComponent<Outline>().OutlineMode = Outline.Mode.SilhouetteOnly;
                xzVertCyln.GetComponent<Outline>().OutlineColor = gridOutline;
                xzVertCyln.GetComponent<Renderer>().material = GridDefualt;
                if (i % 5 == 0)
                {
                    xzVertCyln.GetComponent<Renderer>().material = Grid5Incriment;
                    xzVertCyln.GetComponent<Outline>().OutlineColor = grid5incriOutline;
                }
                if (i % 10 == 0)
                {
                    xzVertCyln.GetComponent<Renderer>().material = Grid10Incriment;
                    xzVertCyln.GetComponent<Outline>().OutlineColor = grid10incriOutline;
                }
                xzVertCyln.GetComponent<Collider>().enabled = false;
                xzVertCyln.GetComponent<Renderer>().shadowCastingMode = 0;
                xzVertCyln.GetComponent<Renderer>().receiveShadows = false;
                xzVertCyln.layer = 10;
                xzVertCyln.transform.parent = GridLines;


                XZVertBarNodes.Add(xzVertCyln);
            }
        }

        deactivateBarGrid();

    }

    public void setAxisNodes(bool Xaxis, bool Yaxis, bool Zaxis)
    {
        if (Xaxis)
            for (int i = xMin; i < xMax; i++)
            {
                //Set X sphere nodes
                XSphereNodes[i + gridSize].transform.position = start.transform.position;
                XSphereNodes[i + gridSize].transform.position += new Vector3(i * puzzleScale.x, 0f, 0f);
            }

        if (Yaxis)
            for (int i = yMin; i < yMax; i++)
            {
                //Set Y sphere nodes
                YSphereNodes[i + gridSize].transform.position = start.transform.position;
                YSphereNodes[i + gridSize].transform.position += new Vector3(0f, i * puzzleScale.y, 0f);
            }

        if (Zaxis)
            for (int i = zMin; i < zMax; i++)
            {
                //Set Z sphere nodes
                ZSphereNodes[i + gridSize].transform.position = start.transform.position;
                ZSphereNodes[i + gridSize].transform.position += new Vector3(0f, 0f, i * puzzleScale.z);
            }        
    }

    public void setGridBarNodes(bool Xaxis, bool Yaxis, bool Zaxis)
    {

        if (Xaxis)
            for (int i = xMin; i < xMax; i++)
            {
                //X bars
                VectorBetweenPoints(XBarNodes[i + gridSize], Ypositive.transform.position + new Vector3(i * puzzleScale.x, 0f, 0f), Ynegative.transform.position + new Vector3(i * puzzleScale.x, 0f, 0f), gridThickness);
                if(Zaxis)
                    VectorBetweenPoints(XZHoriBarNodes[i + gridSize], Zpositive.transform.position + new Vector3(i * puzzleScale.x, 0f, 0f), Znegative.transform.position + new Vector3(i * puzzleScale.x, 0f, 0f), gridThickness);

            }

        if (Yaxis)
            for (int i = yMin; i < yMax; i++)
            {
                //Ybars
                VectorBetweenPoints(YBarNodes[i + gridSize], Xpositive.transform.position + new Vector3(0f, i * puzzleScale.y, 0f), Xnegative.transform.position + new Vector3(0f, i * puzzleScale.y, 0f), gridThickness);
                if(Zaxis)
                    VectorBetweenPoints(YZHoriBarNodes[i + gridSize], Zpositive.transform.position + new Vector3(0f, i * puzzleScale.y, 0f), Znegative.transform.position + new Vector3(0f, i * puzzleScale.y, 0f), gridThickness);


            }

        if (Zaxis)
            for (int i = zMin; i < zMax; i++)
            {
                //Set ZY plane nodes
                VectorBetweenPoints(YZVertBarNodes[i + gridSize], Ypositive.transform.position + new Vector3(0f, 0f, i * puzzleScale.z), Ynegative.transform.position + new Vector3(0f, 0f, i * puzzleScale.z), gridThickness);

                VectorBetweenPoints(XZVertBarNodes[i + gridSize], Xpositive.transform.position + new Vector3(0f, 0f, i * puzzleScale.z), Xnegative.transform.position + new Vector3(0f, 0f, i * puzzleScale.z), gridThickness);
            }       

    }

    public void deactivateAxis()
    {
        Xvector.SetActive(false);
        Yvector.SetActive(false);
        Zvector.SetActive(false);
        Xlabel.SetActive(false);
        Ylabel.SetActive(false);
        Zlabel.SetActive(false);

        foreach (GameObject sphere in XSphereNodes)
        {
            sphere.SetActive(false);
        }

        foreach (GameObject sphere in YSphereNodes)
        {
            sphere.SetActive(false);
        }

        foreach (GameObject sphere in ZSphereNodes)
        {
            sphere.SetActive(false);
        }
    }

    public void activateAxis(bool Xaxis, bool Yaxis, bool Zaxis)
    {
        Xvector.SetActive(Xaxis);
        Yvector.SetActive(Yaxis);
        Zvector.SetActive(Zaxis);
        Xlabel.SetActive(Xaxis);
        Ylabel.SetActive(Yaxis);
        Zlabel.SetActive(Zaxis);

        foreach (GameObject sphere in XSphereNodes)
        {
            sphere.SetActive(Xaxis);
        }

        foreach (GameObject sphere in YSphereNodes)
        {
            sphere.SetActive(Yaxis);
        }

        foreach (GameObject sphere in ZSphereNodes)
        {
            sphere.SetActive(Zaxis);
        }
        
    }

    public void deactivateBarGrid()
    {
        foreach (GameObject bar in XBarNodes)
        {
            bar.SetActive(false);
        }

        foreach (GameObject bar in YBarNodes)
        {
            bar.SetActive(false);
        }

        foreach (GameObject bar in YZHoriBarNodes)
        {
            bar.SetActive(false);
        }
        foreach (GameObject bar in YZVertBarNodes)
        {
            bar.SetActive(false);
        }

        foreach (GameObject bar in XZHoriBarNodes)
        {
            bar.SetActive(false);
        }
        foreach (GameObject bar in XZVertBarNodes)
        {
            bar.SetActive(false);
        }
    }

    public void activateBarGrid(bool Xaxis, bool Yaxis, bool Zaxis)
    {
        if (GameControllerPuzzle04.instance.Difficulty < 3)
        {
            foreach (GameObject bar in XBarNodes)
            {
                bar.SetActive(Xaxis);
            }

            foreach (GameObject bar in YBarNodes)
            {
                bar.SetActive(Yaxis);
            }

            foreach (GameObject bar in YZHoriBarNodes)
            {
                bar.SetActive(Zaxis);
            }
            foreach (GameObject bar in YZVertBarNodes)
            {
                bar.SetActive(Zaxis);
            }

            foreach (GameObject bar in XZHoriBarNodes)
            {
                bar.SetActive(Zaxis);
            }
            foreach (GameObject bar in XZVertBarNodes)
            {
                bar.SetActive(Zaxis);
            }
        }
        else
        {
            foreach (GameObject bar in XBarNodes)
            {
                bar.SetActive(Xaxis);
            }

            foreach (GameObject bar in YBarNodes)
            {
                bar.SetActive(Xaxis);
            }

            foreach (GameObject bar in XZHoriBarNodes)
            {
                bar.SetActive(Yaxis);
            }
            foreach (GameObject bar in XZVertBarNodes)
            {
                bar.SetActive(Yaxis);
            }

            foreach (GameObject bar in YZHoriBarNodes)
            {
                bar.SetActive(Zaxis);
            }
            foreach (GameObject bar in YZVertBarNodes)
            {
                bar.SetActive(Zaxis);
            }

        }

    }

    public void AxisDirector()
    {
        if (GameControllerPuzzle04.instance.P04W.axisOn)
        {
            switch (GameControllerPuzzle04.instance.Difficulty)
            {
                case 1:
                    if (GameControllerPuzzle04.instance.currPuzzle.getDirection().ToString().CompareTo("X") == 0)
                        GameControllerPuzzle04.instance.visVector.activateAxis(true, false, false);
                    else if (GameControllerPuzzle04.instance.currPuzzle.getDirection().ToString().CompareTo("Y") == 0)
                        GameControllerPuzzle04.instance.visVector.activateAxis(false, true, false);
                    break;
                case 2:
                    GameControllerPuzzle04.instance.visVector.activateAxis(true, true, false);
                    break;
                case 3:
                    GameControllerPuzzle04.instance.visVector.activateAxis(true, true, true);
                    break;
            }

            GridDirector();

        }
    }

    public void GridDirector()
    {

        if (GameControllerPuzzle04.instance.P04W.gridOn)
        {
            switch (GameControllerPuzzle04.instance.Difficulty)
            {
                case 1:
                    if (GameControllerPuzzle04.instance.currPuzzle.getDirection().ToString().CompareTo("X") == 0)
                        GameControllerPuzzle04.instance.visVector.activateBarGrid(true, false, false);
                    else if (GameControllerPuzzle04.instance.currPuzzle.getDirection().ToString().CompareTo("Y") == 0)
                        GameControllerPuzzle04.instance.visVector.activateBarGrid(false, true, false);
                    break;
                case 2:
                    GameControllerPuzzle04.instance.visVector.activateBarGrid(true, true, false);
                    break;
                case 3:
                    GameControllerPuzzle04.instance.visVector.activateBarGrid(GameControllerPuzzle04.instance.P04W.XYtoggle, GameControllerPuzzle04.instance.P04W.XZtoggle, GameControllerPuzzle04.instance.P04W.YZtoggle);
                    GameControllerPuzzle04.instance.P04W.advGridControls.gameObject.SetActive(true);
                    break;
            }

            toggleProjections(GameControllerPuzzle04.instance.P04W.projectionsOn);

            toggleAnchors(GameControllerPuzzle04.instance.P04W.anchorsOn);
        }
    }

    public void toggleProjections(bool val)
    {
        isProjecting = val;

        projections.SetActive(val);
    }

    public void toggleAnchors(bool val)
    {
        isAnchored = val;

        anchors.SetActive(val);
    }

    public void toggleXYplane(bool val, bool gridOn)
    {
        XYplane.SetActive(val);
        XYanchors.SetActive(val);

        if (gridOn)
        {
            foreach (GameObject bar in XBarNodes)
            {
                bar.SetActive(val);
            }

            foreach (GameObject bar in YBarNodes)
            {
                bar.SetActive(val);
            }
        }
        else
        {
            foreach (GameObject bar in XBarNodes)
            {
                bar.SetActive(false);
            }

            foreach (GameObject bar in YBarNodes)
            {
                bar.SetActive(false);
            }
        }
    }

    public void toggleXZplane(bool val, bool gridOn)
    {
        XZplane.SetActive(val);
        XZanchors.SetActive(val);

        if (gridOn)
        {
            foreach (GameObject bar in XZHoriBarNodes)
            {
                bar.SetActive(val);
            }

            foreach (GameObject bar in XZVertBarNodes)
            {
                bar.SetActive(val);
            }
        }
        else
        {
            foreach (GameObject bar in XZHoriBarNodes)
            {
                bar.SetActive(false);
            }

            foreach (GameObject bar in XZVertBarNodes)
            {
                bar.SetActive(false);
            }
        }
    }


    public void toggleYZplane(bool val, bool gridOn)
    {
        YZplane.SetActive(val);
        YZanchors.SetActive(val);


        if (gridOn)
        {
            foreach (GameObject bar in YZHoriBarNodes)
            {
                bar.SetActive(val);
            }

            foreach (GameObject bar in YZVertBarNodes)
            {
                bar.SetActive(val);
            }
        }
        else
        {
            foreach (GameObject bar in YZHoriBarNodes)
            {
                bar.SetActive(false);
            }

            foreach (GameObject bar in YZVertBarNodes)
            {
                bar.SetActive(false);
            }
        }
    }

    public void ToggleBasicVectors(bool value)
    {
        getGapVector().SetActive(value);
        ToggleVector1(value);
        ToggleVector2(value);
        getFinalVector().SetActive(value);
    }

}
