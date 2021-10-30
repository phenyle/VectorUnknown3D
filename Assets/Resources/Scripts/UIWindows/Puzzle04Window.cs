using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Puzzle04Window : WindowRoot
{
    public PauseWindow pauseWindow;

    //LOWER RIGHT PIN WINDOWS---------
    [Header("Input Displays")]
    public Transform iptPanel;
    public Slider scalar1;
    public Slider scalar2;
    public Text scalarText1;
    public Text scalarText2;
    public CameraDragSurface cameraDragInput;
    //    private GameObject finalDisplay;
    public GameObject ansField1; //background field object
    public GameObject ansField2; //background field object
    private GameObject ansCard1;
    private GameObject ansCard2;

    //LEFT PIN WINDOWS------------------
    [Header("Answer Card Displays")]
    public Transform cardPanel;
    public GameObject Card1;
    public GameObject Card2;
    public GameObject Card3;
    public GameObject Card4;
    private List<GameObject> UIcards;

    //UPPER RIGHT PIN WINDOWS------------
    [Header("Feedback Displays")]
    public Transform feedbackPanel;
    public GameObject goalDisplay;
    public GameObject finalDisplay;

    //TOP CENTER PIN WINDOW-------------
    [Header("Axis/Grid Buttons")]
    public Transform buttonPanel;
    public GameObject axisButton;
    public GameObject gridButton;
    public Transform advGridControls;
    public Transform advProjectionControls;
    public GameObject projectionsButton;
    public GameObject anchorsButton;
    public GameObject XYbutton;
    public GameObject XZbutton;
    public GameObject YZbutton;
    private bool axisOn;
    private bool gridOn;
    private bool projectionsOn;
    private bool anchorsOn;
    private bool XYtoggle;
    private bool XZtoggle;
    private bool YZtoggle;


    //UPPER LEFT PIN WINDOW------------
    [Header("Puzzle Complete Info")]
    public Text puzzleName;
    public Text puzzleComplete;
    public Text timeDisplay;
    public Slider puzzleSlider;
    private int numComplete;


    //UPPER LEFT PIN WINDOW------------
    [Header("Lower Left Panel")]
    public Transform exitPanel;
    public GameObject exitButton;

    //Legacy
    [Header("Legacy")]
    public bool isInit;
    private bool doOnce = true;




    public string defaultInstructions = "Enter the Portals to Solve Vector Puzzles\nHold Shift to Run";

    private GameControllerPuzzle04 GCP04;
    private Puzzle04Controller PC04;


    private void Start()
    {
        GCP04 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle04>();
        numComplete = 0;
        puzzleSlider.value = 0;
        if (isInit)
        {
            InitWindow();
        }
    }

    protected override void InitWindow()
    {
//        pauseWindow = GameRoot.instance.pauseWindow;
        GCP04 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle04>();

        string controlsText = "MOUSE CONTROLS:\n" +
                               "Hold L-Click - Drag Answer Cards\n" +
                               "Hold R-Click - Drag to Rotate Camera\n" +
                               "MouseWheel - Zoom Camera In/Out\n" +
                               "\n" +
                               "KEYBOARD CONTROLS:\n" +
                                "I - Rotate Camera Up\n" +
                                "J - Rotate Camera Left\n" +
                                "K - Rotate Camera Down\n" +
                                "L - Rotate Camera Right\n" +
                                "U - Zoom Camera In\n" +
                                "O - Zoom Camera Out\n" +
                                "\n" +
                                "1 - Decrease Scalar 1\n" +
                                "2 - Increase Scalar 1\n" + 
                                "3 - Decrease Scalar 2\n" + 
                                "4 - Increase Scalar 2";

        pauseWindow.setPuzzleControlsText(controlsText);
        //pauseWindow.resizePuzzleControls(600, 800);


        //LOWER RIGHT INIT--------------------------------------
        //scalar1 = GameObject.Find("Slider1").GetComponent<Slider>();
        //scalar2 = GameObject.Find("Slider2").GetComponent<Slider>();
        scalar1.value = 1;
        scalar2.value = 1;
        //Assign the Text objects to the UI Gameobjects
        //scalarText1 = GameObject.Find("ScalValue1").GetComponent<Text>();
        //scalarText2 = GameObject.Find("ScalValue2").GetComponent<Text>();
        //Assign the Text field to the current Slider position
        scalarText1.text = scalar1.value.ToString();
        scalarText2.text = scalar2.value.ToString();

        //        finalDisplay = GameObject.Find("FinalAnswer");



        //LEFT PIN INIT-----------------------------------------
        if (doOnce)
        {
            UIcards = new List<GameObject>();
            UIcards.Add(Card1);
            UIcards.Add(Card2);
            UIcards.Add(Card3);
            UIcards.Add(Card4);
            doOnce = false;
        }



        //UPPER RIGHT PIN INIT-----------------------------------
        /**
        gapDisplay = feedbackPanel.gameObject.transform.Find("Gap").gameObject;
        windDisplay = feedbackPanel.Find("Wind").gameObject;
        goalDisplay = feedbackPanel.Find("Goal").gameObject;
        finalDisplay = iptPanel.Find("FinalAnswer").gameObject;
        **/

        //GENERAL INIT---------------------------------------
        setBrackets();
        setAnswer1(null);
        setAnswer2(null);

        axisOn = false;
        gridOn = false;
        projectionsOn = false;
        anchorsOn = false;
        XYtoggle = true;
        XZtoggle = true;
        YZtoggle = true;

        axisButton.transform.localPosition = new Vector3(0, 19, 0);
        XYbutton.GetComponent<Image>().color = Color.green;
        XZbutton.GetComponent<Image>().color = Color.green;
        YZbutton.GetComponent<Image>().color = Color.green;


        //Legacy---------------------------------------------
        Debug.Log("Init Puzzle04 window");
        base.InitWindow();

        Debug.Log("Call GameController of Puzzle04 to connect");
        GCP04.InitGameController(this);

        //Init Components
        numComplete = 0;
        puzzleSlider.value = 0;
        puzzleName.text = "Stage 4 - Level " + GCP04.Difficulty.ToString();
        puzzleComplete.text = numComplete.ToString() + "/" + GCP04.puzzleDatas.Count.ToString();
        puzzleSlider.maxValue = GCP04.puzzleDatas.Count;
        timeDisplay.text = "00 : 00";

        ShowInputPanel(false);
        ShowFeedbackPanel(false);
        ShowCardPanel(false);
        ShowButtonPanel(false);
        SetActive(exitPanel, false);
        gridButton.SetActive(true);
        anchorsButton.SetActive(false);
        if (GCP04.Difficulty < 3)
        {
            SetActive(advGridControls, false);
            SetActive(advProjectionControls, false);
        }
        else
        {
            SetActive(advGridControls, true);
            SetActive(advProjectionControls, true);
        }

    }

    public void setBrackets()
    {
        //Why did I do it this way?  why not just search for all the 1D brackets or 2D brackets
        //Well I tried that, but Unity kept screwing things up and doing weird shit.  SO,
        //You get this mess, it's not pretty, but it works.
        switch (GCP04.Difficulty)
        {
            case 1:
                finalDisplay.transform.Find("1DBracketFin").gameObject.SetActive(true);
                finalDisplay.transform.Find("2DBracketFin").gameObject.SetActive(false);
                finalDisplay.transform.Find("3DBracketFin").gameObject.SetActive(false);
                goalDisplay.transform.Find("1DBracket").gameObject.SetActive(true);
                goalDisplay.transform.Find("2DBracket").gameObject.SetActive(false);
                goalDisplay.transform.Find("3DBracket").gameObject.SetActive(false);

                break;
            case 2:
                finalDisplay.transform.Find("1DBracketFin").gameObject.SetActive(false);
                finalDisplay.transform.Find("2DBracketFin").gameObject.SetActive(true);
                finalDisplay.transform.Find("3DBracketFin").gameObject.SetActive(false);
                goalDisplay.transform.Find("1DBracket").gameObject.SetActive(false);
                goalDisplay.transform.Find("2DBracket").gameObject.SetActive(true);
                goalDisplay.transform.Find("3DBracket").gameObject.SetActive(false);
                break;
            case 3:
                finalDisplay.transform.Find("1DBracketFin").gameObject.SetActive(false);
                finalDisplay.transform.Find("2DBracketFin").gameObject.SetActive(false);
                finalDisplay.transform.Find("3DBracketFin").gameObject.SetActive(true);
                goalDisplay.transform.Find("1DBracket").gameObject.SetActive(false);
                goalDisplay.transform.Find("2DBracket").gameObject.SetActive(false);
                goalDisplay.transform.Find("3DBracket").gameObject.SetActive(true);
                break;


        }

        foreach (GameObject card in UIcards)
        {
            switch(GCP04.Difficulty)
            {
                case 1:
                    card.transform.Find("1DBracket").gameObject.SetActive(true);
                    card.transform.Find("2DBracket").gameObject.SetActive(false);
                    card.transform.Find("3DBracket").gameObject.SetActive(false);
                    break;
                case 2:
                    card.transform.Find("1DBracket").gameObject.SetActive(false);
                    card.transform.Find("2DBracket").gameObject.SetActive(true);
                    card.transform.Find("3DBracket").gameObject.SetActive(false);
                    break;
                case 3:
                    card.transform.Find("1DBracket").gameObject.SetActive(false);
                    card.transform.Find("2DBracket").gameObject.SetActive(false);
                    card.transform.Find("3DBracket").gameObject.SetActive(true);
                    break;
            }
        }
    }



    /// <summary>
    /// Gets the cards display info from "CardVectors" and sets the vector values info
    /// to the UIcards.
    /// 
    /// NOTE: Which vectors display is dependent on the InitWindow() method where
    /// certain vector types are displayed/not displayed dependent on the 
    /// GameControllerPuzzle01.Difficutly
    /// NOTE2: If GameControllerPuzzle01.Diffculty is set to 1 then the resulting
    /// displayed values will be dependent on if the PARTICULAR Puzzle01Controller
    /// is set for an X or Y direction constraint.  This is done for flexibliy reasons.
    /// </summary>
    /// <param name="PC04"> Only used for 1D vectors in the X or Y direction</param>
    public void setCardDisplay(Puzzle04Controller PC04)
    {
        //1D Vectors----------------------------------
        if (PC04.getDirection().ToString().CompareTo("X") == 0)
        {
            for (int i = 0; i < UIcards.Count; i++)
            {
                UIcards[i].transform.Find("1DBracket").transform.Find("1Dval").GetComponentInChildren<Text>().text = UIcards[i].GetComponent<CardVectors>().getCardVector().x.ToString();
            }
        }
        if (PC04.getDirection().ToString().CompareTo("Y") == 0)
        {
            for (int i = 0; i < UIcards.Count; i++)
            {
                UIcards[i].transform.Find("1DBracket").transform.Find("1Dval").GetComponentInChildren<Text>().text = UIcards[i].GetComponent<CardVectors>().getCardVector().y.ToString();
            }
        }

        //2D Vectors----------------------------------------
        for (int i = 0; i < UIcards.Count; i++)
        {
            UIcards[i].transform.Find("2DBracket").transform.Find("2DvalX").GetComponentInChildren<Text>().text = UIcards[i].GetComponent<CardVectors>().getCardVector().x.ToString();
            UIcards[i].transform.Find("2DBracket").transform.Find("2DvalY").GetComponentInChildren<Text>().text = UIcards[i].GetComponent<CardVectors>().getCardVector().y.ToString();
        }

        //3D Vectors------------------------------------------
        for (int i = 0; i < UIcards.Count; i++)
        {
            UIcards[i].transform.Find("3DBracket").transform.Find("3DvalX").GetComponentInChildren<Text>().text = UIcards[i].GetComponent<CardVectors>().getCardVector().x.ToString();
            UIcards[i].transform.Find("3DBracket").transform.Find("3DvalY").GetComponentInChildren<Text>().text = UIcards[i].GetComponent<CardVectors>().getCardVector().z.ToString();
            UIcards[i].transform.Find("3DBracket").transform.Find("3DvalZ").GetComponentInChildren<Text>().text = UIcards[i].GetComponent<CardVectors>().getCardVector().y.ToString();
        }

    }

    public void setFeedbackDisplay(Puzzle04Controller PC04)
    {
        //1D Vectors------------------------------------------
        if (PC04.getDirection().ToString().CompareTo("X") == 0)
        {
            goalDisplay.transform.Find("1DBracket").transform.Find("1Dval").GetComponentInChildren<Text>().text = PC04.getAnswerVector().x.ToString();
        }
        if (PC04.getDirection().ToString().CompareTo("Y") == 0)
        {

            goalDisplay.transform.Find("1DBracket").transform.Find("1Dval").GetComponentInChildren<Text>().text = PC04.getAnswerVector().y.ToString();
        }

        //2D Vectors----------------------------------------
        goalDisplay.transform.Find("2DBracket").transform.Find("2DvalX").GetComponentInChildren<Text>().text = PC04.getAnswerVector().x.ToString();
        goalDisplay.transform.Find("2DBracket").transform.Find("2DvalY").GetComponentInChildren<Text>().text = PC04.getAnswerVector().y.ToString();


        //3D Vectors------------------------------------------
        goalDisplay.transform.Find("3DBracket").transform.Find("3DvalX").GetComponentInChildren<Text>().text = PC04.getAnswerVector().x.ToString();
        goalDisplay.transform.Find("3DBracket").transform.Find("3DvalY").GetComponentInChildren<Text>().text = PC04.getAnswerVector().z.ToString();
        goalDisplay.transform.Find("3DBracket").transform.Find("3DvalZ").GetComponentInChildren<Text>().text = PC04.getAnswerVector().y.ToString();
    }


    public void setFinalAnswerDisplay(Puzzle04Controller PC04)
    {
        //1D Vectors------------------------------------------
        if (PC04.getDirection().ToString().CompareTo("X") == 0)
        {
            finalDisplay.transform.Find("1DBracketFin").transform.Find("1Dval").GetComponentInChildren<Text>().text = PC04.getPlayerAnswer().x.ToString();
        }
        if (PC04.getDirection().ToString().CompareTo("Y") == 0)
        {

            finalDisplay.transform.Find("1DBracketFin").transform.Find("1Dval").GetComponentInChildren<Text>().text = PC04.getPlayerAnswer().y.ToString();
        }

        //2D Vectors----------------------------------------
        finalDisplay.transform.Find("2DBracketFin").transform.Find("2DvalX").GetComponentInChildren<Text>().text = PC04.getPlayerAnswer().x.ToString();
        finalDisplay.transform.Find("2DBracketFin").transform.Find("2DvalY").GetComponentInChildren<Text>().text = PC04.getPlayerAnswer().y.ToString();

        //3D Vectors------------------------------------------
        finalDisplay.transform.Find("3DBracketFin").transform.Find("3DvalX").GetComponentInChildren<Text>().text = PC04.getPlayerAnswer().x.ToString();
        finalDisplay.transform.Find("3DBracketFin").transform.Find("3DvalY").GetComponentInChildren<Text>().text = PC04.getPlayerAnswer().z.ToString();
        finalDisplay.transform.Find("3DBracketFin").transform.Find("3DvalZ").GetComponentInChildren<Text>().text = PC04.getPlayerAnswer().y.ToString();
    }


    public void Update()
    {
        //if (scalar1.value == 0)
        //    scalar1.value = 1;
        //if (scalar2.value == 0)
        //    scalar2.value = 1;

        scalarText1.text = scalar1.value.ToString();
        scalarText2.text = scalar2.value.ToString();
    }

    public List<GameObject> getCards()
    {
        return UIcards;
    }

    public void setCardValues(List<Vector3> cardVals)
    {
        for (int i = 0; i < UIcards.Count; i++)
        {
            UIcards[i].GetComponent<CardVectors>().setCardVector(cardVals[i]);
        }
    }

    public void resetCardPos()
    {
        for (int i = 0; i < UIcards.Count; i++)
        {
            UIcards[i].GetComponent<CardVectors>().resetCard();
        }

        ansCard1 = null;
        ansCard2 = null;
    }

    public void setPuzzleController(Puzzle04Controller PC04)
    {
        this.PC04 = PC04;
    }

    public void setAnswer1(GameObject card)
    {
        ansCard1 = card;
    }
    public void setAnswer2(GameObject card)
    {
        ansCard2 = card;
    }

    public GameObject getAnswer1()
    {
        return ansCard1;
    }

    public GameObject getAnswer2()
    {
        return ansCard2;
    }

    public int getScalar1Value()
    {
        return (int)scalar1.value;
    }
    public int getScalar2Value()
    {
        return (int)scalar2.value;
    }

    public Transform getAnsField1trans()
    {
        return ansField1.transform;
    }
    public Transform getAnsField2trans()
    {
        return ansField2.transform;
    }


    public void ClickAnswerSubmitBtn()
    {
        if (PC04.checkFinalAnswer())
        {
            foreach (GameObject card in UIcards)
            {
                card.transform.position = card.GetComponent<CardVectors>().getStartPos();
            }
            setAnswer1(null);
            setAnswer2(null);
        }

        audioService.PlayUIAudio(Constants.audioUIClickBtn);
    }

    public void ShowInputPanel(bool status)
    {
        SetActive(iptPanel, status);
    }

    public void ShowFeedbackPanel(bool status)
    {
        SetActive(feedbackPanel, status);
    }

    public void ShowCardPanel(bool status)
    {
        SetActive(cardPanel, status);
    }

    public void ShowButtonPanel(bool status)
    {
        SetActive(buttonPanel, status);

        axisButton.SetActive(status);
        gridButton.SetActive(status);

        if(GCP04.Difficulty == 3 )
        {
            SetActive(advGridControls, status);
            SetActive(advProjectionControls, status);
        }
    }

    public void toggleAxis()
    {
        axisOn = !axisOn;


        if(axisOn)
        {
            switch (GCP04.Difficulty)
            {
                case 1:
                    if (PC04.getDirection().ToString().CompareTo("X") == 0)
                        PC04.getVisualVector().activateAxis(true, false, false);
                    else if (PC04.getDirection().ToString().CompareTo("Y") == 0)
                        PC04.getVisualVector().activateAxis(false, true, false);
                    break;
                case 2:
                    PC04.getVisualVector().activateAxis(true, true, false);
                    break;
                case 3:
                    PC04.getVisualVector().activateAxis(true, true, true);
                    break;
            }

 //           gridButton.SetActive(true);

            if (gridOn)
            {
                switch (GCP04.Difficulty)
                {
                    case 1:
                        if (PC04.getDirection().ToString().CompareTo("X") == 0)
                            PC04.getVisualVector().activateBarGrid(true, false, false);
                        else if (PC04.getDirection().ToString().CompareTo("Y") == 0)
                            PC04.getVisualVector().activateBarGrid(false, true, false);
                        break;
                    case 2:
                        PC04.getVisualVector().activateBarGrid(true, true, false);
                        break;
                    case 3:
                        PC04.getVisualVector().activateBarGrid(XYtoggle, XZtoggle, YZtoggle);
                        SetActive(advGridControls, true);
                        break;
                }

                if (projectionsOn)
                {
                    PC04.getVisualVector().toggleProjections(true);

                    projectionsButton.GetComponent<Image>().color = Color.green;

                    if (anchorsOn)
                    {
                        anchorsButton.GetComponent<Image>().color = Color.green;
                        PC04.getVisualVector().toggleAnchors(true);
                    }
                }
            }

            axisButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            PC04.getVisualVector().deactivateAxis();
            axisButton.GetComponent<Image>().color = Color.white;

 //           gridButton.SetActive(false);

 //           PC04.getVisualVector().deactivateBarGrid();

 //           SetActive(advGridControls, false);

 //           PC04.getVisualVector().toggleProjections(false);
 //           PC04.getVisualVector().toggleAnchors(false);
        }
    }

    public void toggleGrid()
    {
        gridOn = !gridOn;

        if (gridOn)
        {
            switch(GCP04.Difficulty)
            {
                case 1:
                    if(PC04.getDirection().ToString().CompareTo("X") == 0)
                        PC04.getVisualVector().activateBarGrid(true, false, false);
                    else if(PC04.getDirection().ToString().CompareTo("Y") == 0)
                        PC04.getVisualVector().activateBarGrid(false, true, false);
                    break;
                case 2:
                    PC04.getVisualVector().activateBarGrid(true, true, false);
                    break;
                case 3:
                    PC04.getVisualVector().activateBarGrid(XYtoggle, XZtoggle, YZtoggle);
                    SetActive(advGridControls, true);
                    break;
            }
            gridButton.GetComponent<Image>().color = Color.green;



            if(projectionsOn)
            {
                PC04.getVisualVector().toggleProjections(true);

                projectionsButton.GetComponent<Image>().color = Color.green;

                if (anchorsOn)
                {
                    anchorsButton.GetComponent<Image>().color = Color.green;
                    PC04.getVisualVector().toggleAnchors(true);
                }
            }

        }
        else
        {
            PC04.getVisualVector().deactivateBarGrid();
            gridButton.GetComponent<Image>().color = Color.white;

 //           SetActive(advGridControls, false);

 //           PC04.getVisualVector().toggleProjections(false);
 //           PC04.getVisualVector().toggleAnchors(false);
        }
    }

    public void toggleProjectionsButton()
    {
        projectionsOn = !projectionsOn;

        if (projectionsOn)
        {

            projectionsButton.GetComponent<Image>().color = Color.green;
            
            SetActive(advProjectionControls, true);
            if (XYtoggle)
                XYbutton.GetComponent<Image>().color = Color.green;
            else
                XYbutton.GetComponent<Image>().color = Color.white;

            if (XZtoggle)
                XZbutton.GetComponent<Image>().color = Color.green;
            else
                XZbutton.GetComponent<Image>().color = Color.white;

            if (YZtoggle)
                YZbutton.GetComponent<Image>().color = Color.green;
            else
                YZbutton.GetComponent<Image>().color = Color.white;


            anchorsButton.SetActive(true);

            if (anchorsOn)
            {
                PC04.getVisualVector().toggleAnchors(true);
                anchorsButton.GetComponent<Image>().color = Color.green;
            }
            else
                anchorsButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
 //           SetActive(advProjectionControls, false);
            anchorsButton.SetActive(false);
            PC04.getVisualVector().toggleAnchors(false);
            projectionsButton.GetComponent<Image>().color = Color.white;
        }

        PC04.getVisualVector().toggleProjections(projectionsOn);
    }

    public void toggleAnchorsButton()
    {
        anchorsOn = !anchorsOn;

        if(anchorsOn)
            anchorsButton.GetComponent<Image>().color = Color.green;
        else
            anchorsButton.GetComponent<Image>().color = Color.white;

        PC04.getVisualVector().toggleAnchors(anchorsOn);
    }

    public void toggleXY()
    {
        XYtoggle = !XYtoggle;

        if (XYtoggle)
            XYbutton.GetComponent<Image>().color = Color.green;
        else
            XYbutton.GetComponent<Image>().color = Color.white;

        PC04.getVisualVector().toggleXYplane(XYtoggle, gridOn);
    }

    public bool getXYtoggle()
    {
        return XYtoggle;
    }

    public void toggleXZ()
    {
        XZtoggle = !XZtoggle;

        if (XZtoggle)
            XZbutton.GetComponent<Image>().color = Color.green;
        else
            XZbutton.GetComponent<Image>().color = Color.white;

        PC04.getVisualVector().toggleXZplane(XZtoggle, gridOn);
    }

    public bool getXZtoggle()
    {
        return XZtoggle;
    }

    public void toggleYZ()
    {
        YZtoggle = !YZtoggle;

        if (YZtoggle)
            YZbutton.GetComponent<Image>().color = Color.green;
        else
            YZbutton.GetComponent<Image>().color = Color.white;

        PC04.getVisualVector().toggleYZplane(YZtoggle, gridOn);
    }

    public bool getYZtoggle()
    {
        return YZtoggle;
    }

    public bool getAxisStatus()
    {
        return axisOn;
    }


    public void resetCamera()
    {
        PC04.mainCamera.transform.position = PC04.CameraStartPosition.transform.position;
        PC04.mainCamera.transform.LookAt(PC04.cameraTarget.transform.position);
    }

    public void resetButtons()
    {
        gridOn = false;
        axisOn = false;
        projectionsOn = false;
        anchorsOn = false;

 //       gridButton.SetActive(false);
 //       SetActive(advGridControls, false);

        axisButton.GetComponent<Image>().color = Color.white;
        gridButton.GetComponent<Image>().color = Color.white;
        projectionsButton.GetComponent<Image>().color = Color.white;
        anchorsButton.GetComponent<Image>().color = Color.white;
    }

    public CameraDragSurface getCameraDragController()
    {
        return cameraDragInput.GetComponent<CameraDragSurface>();
    }

    public void SetTime(string val)
    {
        timeDisplay.text = val;
    }

    public void SetComplete()
    {
        numComplete++;
        puzzleSlider.value = numComplete;

        puzzleComplete.text = numComplete.ToString() + "/" + GCP04.puzzleDatas.Count.ToString();
    }

    public void ClickExitButton()
    {
        if(PC04 != null)
        {
            PC04.OnPortalExit();
            GameRoot.instance.LockPlayerAndCam(false);
        }
    }


}
