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
    public Button SubmitButton;
    public CardController card1 { get; private set; }
    public CardController card2 { get; private set; }

    //LEFT PIN WINDOWS------------------
    [Header("Answer Card Displays")]
    public Transform cardPanel;
    public Transform[] cardStarts;
    public GameObject Card1;
    public GameObject Card2;
    public GameObject Card3;
    public GameObject Card4;
    private List<GameObject> UIcards;

    //UPPER RIGHT PIN WINDOWS------------
    [Header("Feedback Displays")]
    public GameObject panelCommon;
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
    public bool axisOn { get; private set; }
    public bool gridOn { get; private set; }
    public bool projectionsOn { get; private set; }
    public bool anchorsOn { get; private set; }
    public bool XYtoggle { get; private set; }
    public bool YZtoggle { get; private set; }
    public bool XZtoggle { get; private set; }


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
        GCP04 = GameControllerPuzzle04.instance;
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

        //pauseWindow.setPuzzleControlsText(controlsText);
        //pauseWindow.resizePuzzleControls(600, 800);


        //LOWER RIGHT INIT--------------------------------------
        //scalar1 = GameObject.Find("Slider1").GetComponent<Slider>();
        //scalar2 = GameObject.Find("Slider2").GetComponent<Slider>();
        scalar1.value = 0;
        scalar2.value = 0;
        //Assign the Text objects to the UI Gameobjects
        //scalarText1 = GameObject.Find("ScalValue1").GetComponent<Text>();
        //scalarText2 = GameObject.Find("ScalValue2").GetComponent<Text>();
        //Assign the Text field to the current Slider position
        scalarText1.text = scalar1.value.ToString();
        scalarText2.text = scalar2.value.ToString();

        //        finalDisplay = GameObject.Find("FinalAnswer");



        //LEFT PIN INIT-----------------------------------------
        Card1.GetComponent<CardDrag>().cardIndexID = 0;
        Card2.GetComponent<CardDrag>().cardIndexID = 1;
        Card3.GetComponent<CardDrag>().cardIndexID = 2;
        Card4.GetComponent<CardDrag>().cardIndexID = 3;

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


    public void SetDisplays(List<Vector3> cards)
    {
        for (int i = 0; i < UIcards.Count; i++)
        {
            UIcards[i].GetComponent<CardController>().SetVector(cards[i]);
        }
        goalDisplay.GetComponent<CardController>().SetVector(GameControllerPuzzle04.instance.currPuzzle.getAnswerVector());
        finalDisplay.GetComponent<CardController>().SetVector(GameControllerPuzzle04.instance.currPuzzle.playerAnswer);

    }

    public void setFinalAnswerDisplay(Puzzle04Controller PC04)
    {
        finalDisplay.GetComponent<CardController>().SetVector(PC04.playerAnswer);
    }


    public void Update()
    {
        if (GameControllerPuzzle04.instance.currPuzzle == null)
            return;

        //Keyboard Scalar Controls
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (scalar1.value > -10)
            {
                scalar1.value--;
                OnSlider1Change();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (scalar1.value < 10)
            {
                scalar1.value++;
                OnSlider1Change();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (scalar2.value > -10)
            {
                scalar2.value--;
                OnSlider2Change();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (scalar2.value < 10)
            {
                scalar2.value++;
                OnSlider2Change();
            }
        }
    }

    public void OnGUI()
    {
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
            UIcards[i].GetComponent<CardController>().SetVector(cardVals[i]);
        }
    }

    public void setPuzzleController(Puzzle04Controller PC04)
    {
        this.PC04 = PC04;
        scalar1.value = 0;
        scalar2.value = 0;
        scalarText1.text = scalar1.value.ToString();
        scalarText2.text = scalar2.value.ToString();

    }

    public void setAnswer1(CardController card)
    {
        card1 = card;
        if (GameControllerPuzzle04.instance.currPuzzle != null)
        {
            GameControllerPuzzle04.instance.currPuzzle.CalculatePlayerAnswer();
            finalDisplay.GetComponent<CardController>().SetVector(GameControllerPuzzle04.instance.currPuzzle.playerAnswer);
            GameControllerPuzzle04.instance.currPuzzle.answer1events.Invoke();
            SubmitButtionInteractable();
        }

    }
    public void setAnswer2(CardController card)
    {
        card2 = card;

        if (GameControllerPuzzle04.instance.currPuzzle != null)
        {
            GameControllerPuzzle04.instance.currPuzzle.CalculatePlayerAnswer();
            finalDisplay.GetComponent<CardController>().SetVector(GameControllerPuzzle04.instance.currPuzzle.playerAnswer);
            GameControllerPuzzle04.instance.currPuzzle.answer2events.Invoke();
            SubmitButtionInteractable();
        }
    }

    public void OnSlider1Change()
    {
        scalarText1.text = getScalar1Value().ToString();

        if (GameControllerPuzzle04.instance.currPuzzle != null)
        {
            GameControllerPuzzle04.instance.currPuzzle.CalculatePlayerAnswer();
            finalDisplay.GetComponent<CardController>().SetVector(GameControllerPuzzle04.instance.currPuzzle.playerAnswer);

            SubmitButtionInteractable();

            GameControllerPuzzle04.instance.currPuzzle.answer1events.Invoke();
        }
    }
    public void OnSlider2Change()
    {
        scalarText2.text = getScalar2Value().ToString();

        if (GameControllerPuzzle04.instance.currPuzzle != null)
        {
            GameControllerPuzzle04.instance.currPuzzle.CalculatePlayerAnswer();
            finalDisplay.GetComponent<CardController>().SetVector(GameControllerPuzzle04.instance.currPuzzle.playerAnswer);

            SubmitButtionInteractable();
            GameControllerPuzzle04.instance.currPuzzle.answer2events.Invoke();
        }
    }

    private void SubmitButtionInteractable()
    {
        if (card1 != null && card2 != null &&
            GameControllerPuzzle04.instance.currPuzzle.playerAnswer != Vector3.zero)
            GameControllerPuzzle04.instance.P04W.SubmitButton.interactable = true;
        else
            GameControllerPuzzle04.instance.P04W.SubmitButton.interactable = false;
    }

    public CardController getAnswer1()
    {
        return card1;
    }

    public CardController getAnswer2()
    {
        return card2;
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

            ResetCardPositions();
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
            axisButton.GetComponent<Image>().color = Color.green;
        else
            axisButton.GetComponent<Image>().color = Color.white;

        VisVectController.axis.ToggleAxis(axisOn);
    }

    public void toggleGrid()
    {
        gridOn = !gridOn;

        if (gridOn)
            gridButton.GetComponent<Image>().color = Color.green;
        else
            gridButton.GetComponent<Image>().color = Color.white;


        VisVectController.grid.ToggleGrid(gridOn);
        VisVectController.projections.ToggleProjections(projectionsOn);
        VisVectController.anchors.ToggleAnchors(anchorsOn);
    }

    public void toggleProjectionsButton()
    {
        projectionsOn = !projectionsOn;

        VisVectController.projections.ToggleProjections(projectionsOn);
        VisVectController.projections.ToggleXYprojections(XYtoggle);
        VisVectController.projections.ToggleYZprojections(YZtoggle);
        VisVectController.projections.ToggleXZprojections(XZtoggle);

        if (projectionsOn)
        {
            projectionsButton.GetComponent<Image>().color = Color.green;            
            SetActive(advProjectionControls, projectionsOn || anchorsOn);

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

            VisVectController.anchors.ToggleAnchors(anchorsOn);
            VisVectController.anchors.ToggleXYanchors(XYtoggle);
            VisVectController.anchors.ToggleYZanchors(YZtoggle);
            VisVectController.anchors.ToggleXZanchors(XZtoggle);

            if (anchorsOn)
                anchorsButton.GetComponent<Image>().color = Color.green;
            else
                anchorsButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
    //        SetActive(advProjectionControls, false);
            anchorsButton.SetActive(false);
            VisVectController.anchors.ToggleAnchors(false);
            projectionsButton.GetComponent<Image>().color = Color.white;
        }
    }

    public void toggleAnchorsButton()
    {
        anchorsOn = !anchorsOn;

        if(anchorsOn)
            anchorsButton.GetComponent<Image>().color = Color.green;
        else
            anchorsButton.GetComponent<Image>().color = Color.white;

        VisVectController.anchors.ToggleAnchors(anchorsOn);
        VisVectController.anchors.ToggleXYanchors(XYtoggle);
        VisVectController.anchors.ToggleYZanchors(YZtoggle);
        VisVectController.anchors.ToggleXZanchors(XZtoggle);
    }

    public void toggleXY()
    {
        XYtoggle = !XYtoggle;

        if (XYtoggle)
            XYbutton.GetComponent<Image>().color = Color.green;
        else
            XYbutton.GetComponent<Image>().color = Color.white;

        VisVectController.grid.ToggleXYplane(XYtoggle);
        VisVectController.anchors.ToggleXYanchors(XYtoggle);
        VisVectController.projections.ToggleXYprojections(XYtoggle);

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

        VisVectController.grid.ToggleXZplane(XZtoggle);
        VisVectController.anchors.ToggleXZanchors(XZtoggle);
        VisVectController.projections.ToggleXZprojections(XZtoggle);

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

        VisVectController.grid.ToggleYZplane(YZtoggle);
        VisVectController.anchors.ToggleYZanchors(YZtoggle);
        VisVectController.projections.ToggleYZprojections(YZtoggle);
    }

    public bool getZYtoggle()
    {
        return YZtoggle;
    }

    public bool getAxisStatus()
    {
        return axisOn;
    }

    public Transform[] GetStartPositions()
    {
        return cardStarts;
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

    public void ResetCardPositions()
    {
        for (int i = 0; i < UIcards.Count; i++)
        {
            UIcards[i].transform.position = cardStarts[i].transform.position;
            UIcards[i].GetComponent<CardController>().SetBrackets();

        }
        goalDisplay.GetComponent<CardController>().SetBrackets();
        finalDisplay.GetComponent<CardController>().SetBrackets();
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
            GameRoot3D.instance.LockPlayerAndCam(false);
        }
    }

    public void Ans1DecreaseButton()
    {
        if (scalar1.value > -10)
            scalar1.value--;
    }

    public void Ans1IncreaseButton()
    {
        if (scalar1.value < 10)
            scalar1.value++;
    }


    public void Ans2DecreaseButton()
    {
        if (scalar2.value > -10)
            scalar2.value--;
    }

    public void Ans2IncreaseButton()
    {
        if (scalar2.value < 10)
            scalar2.value++;
    }


}
