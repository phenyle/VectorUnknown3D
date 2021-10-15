using UnityEngine;
using UnityEngine.UI;

public class Puzzle01Window : WindowRoot
{
    public bool isInit;
    public Transform iptPanel;
    public Transform feedbackPanel;
    public Transform questionPanel;
    public InputField iptScalar;
    public InputField iptX, iptY, iptZ;
    public Text txtInstruction;
    public Text txtFBFormula;
    public Text txtFBTF;
    public Text txtFBTips;
    public Text vector1text;
    public Text answerText;
    public Text vectorSolution;
    public Image[] PlatformTips;
    public Image[] PlatformTipsChecked;

    private GameControllerPuzzle01 GCP01;
    [HideInInspector]
    public float defaultScalar;
    [HideInInspector]
    public float defaultX, defaultY, defaultZ;
    [HideInInspector]
    public int questionNum;

    private void Start()
    {
        if(isInit)
        {
            InitWindow();
        }
    }

    protected override void InitWindow()
    {
        Debug.Log("Init Puzzle01 window");
        base.InitWindow();

        GCP01 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle01>();

        Debug.Log("Call GameController of Puzzle01 to connect");
        GCP01.InitGameController(this);

        //Init Components
        //txtInstruction.text = "";

        txtFBFormula.text = "";
        txtFBTF.text = "";
        txtFBTips.text = "";

        SetActive(iptPanel, false);
        SetActive(feedbackPanel, false);
        SetActive(questionPanel, false);

        foreach (Image image in PlatformTips)
        {
            SetActive(image, true);
        }

        foreach (Image image in PlatformTipsChecked)
        {
            SetActive(image, false);
        }
    }

    public void ClickAnswerSubmitBtn()
    {
        float scalarValue;
        float xValue, yValue, zValue;

        if (iptScalar.text == "")
        {
            GameRoot.AddTips("Please input the scalar value");
        }
        else if (iptX.text == "" || iptY.text == "" || iptZ.text == "")
        {
            GameRoot.AddTips("Please input the vector value");
        }
        else if (!float.TryParse(iptScalar.text, out scalarValue) || !float.TryParse(iptX.text, out xValue) || !float.TryParse(iptY.text, out yValue) || !float.TryParse(iptZ.text, out zValue))
        {
            GameRoot.AddTips("Please input numbers");
        }
        else
        {
            if(GCP01.CheckAnswer(scalarValue, xValue, yValue, zValue))
            {
//                SetActive(PlatformTips[questionNum - 1], false);
//                SetActive(PlatformTipsChecked[questionNum - 1], true);
            }

            Debug.Log("Your answer is Scalar: " + iptScalar.text + ", X: " + iptX.text + ", Y: " + iptY.text + ", Z: " + iptZ.text);

            ClearInputField();
            SetActive(iptPanel, false);
            SetActive(questionPanel, false);

            GCP01.isTriggerQuestion = false;

            if(!DialogueManager.isInDialogue)
            {
                GameRoot.instance.IsLock(false);
                GameObject.Find("MainCamera").GetComponent<CameraController>().isLock = true;
            }
        }

        audioService.PlayUIAudio(Constants.audioUIClickBtn);
    }

    public void ShowInputPanel(bool status)
    {
        InitDefaultValue();
        SetActive(iptPanel, status);
    }

    public void ShowFeedbackPanel(bool status)
    {
        SetActive(feedbackPanel, status);
    }

    private void InitDefaultValue()
    {
        if (defaultScalar != 0)
        {
            iptScalar.text = defaultScalar.ToString();
            iptScalar.interactable = false;
        }

        if(defaultX != -999)
        {
            iptX.text = defaultX.ToString();
            iptX.interactable = false;
        }

        if (defaultY != -999)
        {
            iptY.text = defaultY.ToString();
            iptY.interactable = false;
        }

        if (defaultZ != -999)
        {
            iptZ.text = defaultZ.ToString();
            iptZ.interactable = false;
        }
    }

    public void ClearInputField()
    {
        iptScalar.text = "";
        iptX.text = "";
        iptY.text = "";
        iptZ.text = "";
        iptScalar.interactable = true;
        iptX.interactable = true;
        iptY.interactable = true;
        iptZ.interactable = true;
    }

    public void SetFeedback(string formula, string TF, Color color)
    {
        txtFBTips.text = "";

        txtFBFormula.text = formula;
        txtFBFormula.color = color;

        txtFBTF.text = TF;
        txtFBTF.color = color;
    }

    public void SetFeedbackQuestionTips(string tips)
    {
        txtFBFormula.text = "";
        txtFBTF.text = "";

        txtFBTips.text = tips;
    }

    public string GetCurrentInput()
    {
        return iptX.text + "|" + iptY.text + "|" + iptZ.text;
    }

    public void scalarUpdate()
    {
        float scalarValue;
        if (float.TryParse(iptScalar.text, out scalarValue))
        {
            GCP01.updateLine(0, scalarValue);
        }
    }

    public void xUpdate()
    {
        float x;
        if (float.TryParse(iptX.text, out x))
        {
            GCP01.updateLine(1, x);
        }
    }

    public void yUpdate()
    {
        float y;
        if (float.TryParse(iptY.text, out y))
        {
            GCP01.updateLine(2, y);
        }
    }

    public void zUpdate()
    {
        float z;
        if (float.TryParse(iptZ.text, out z))
        {
            GCP01.updateLine(3, z);
        }
    }
        /*
        public void SetGreenLineTips()
        {
            GCP01.DBP01.SetGreenLineTips();
        }*/
    }
