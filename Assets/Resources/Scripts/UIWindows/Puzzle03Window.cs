using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle03Window : WindowRoot
{
    public bool isInit;

    [Header("--------Instruction Panel--------")]
    public GameObject PanelInstruction;
    public Text txtInstruction;

    [Header("--------Feedback Panel--------")]
    public GameObject PanelFeedback;
    public Text txtFBChoice1;
    public Text txtFBChoice2;

    [Header("--------Answer Panel--------")]
    public GameObject PanelAnswer;
    public Text txtChoice1;
    public Text txtChoice2;

    [Header("--------Choice Panel--------")]
    public Transform panelChoice;
    public int choiceID1 = 0;
    public int choiceID2 = 0;
    public Vector3 choice1Pos;
    public Vector3 choice2Pos;

    [Header("--------Progress Panel--------")]
    public GameObject PanelProgress;
    public Text puzzleHeader;
    public Text movesNumber;
    public Text timeNumber;


    [Header("--------Misc--------")]
    public Transform quitPanel;    
    public bool bVal = true;
    private bool ansSelected = false;
 //   public List<ChoiceClickButton> BtnChoices1;
    public GameControllerPuzzle03 GCP03;  

    private void Start()
    {
        if (isInit)
        {
            InitWindow();
        }
    }

    protected override void InitWindow()
    {
        Debug.Log("Init Puzzle01 window");
        base.InitWindow();

        GCP03 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle03>();
        GCP03.InitGameController(this);

        puzzleHeader.text = "Stage 3 - Level " + GCP03.Difficulty.ToString();
        timeNumber.text = "";
        movesNumber.text = "0";

        ShowChoicePanel(false);
        quitPanel.gameObject.SetActive(false);
        ClearSpanValues("", "");
        SwapFeedbackPanel();
    }

    public void SetFirstSpanValue(Vector3 spanValue, int choiceID)
    {
        txtChoice1.text = spanValue.x + "\n" + spanValue.y + "\n" + spanValue.z;
        choiceID1 = choiceID;
        choice1Pos = spanValue;
    }

    public void SetSecondSpanValue(Vector3 spanValue, int choiceID)
    {
        //if (choiceID2 == 0)
        //{
            txtChoice2.text = spanValue.x + "\n" + spanValue.y + "\n<color=red>" + spanValue.z + "</color>";
            choiceID2 = choiceID;
            choice2Pos = spanValue;
        //}
        //else
        //{
        //    Debug.Log("Full");
        //}
    }

    public void ClickOneBtn()
    {
        int val = 1;
        if (bVal)
        {
            SetFirstSpanValue(new Vector3(1, 0, 0), 2);
            SetSecondSpanValue(new Vector3(0, 1, val), 6);
            GCP03.SetTipsPointsValue(new Vector3(1, 0, 0), new Vector3(0, 1, val));
        }
        else
        {
            SetFirstSpanValue(new Vector3(0, 1, 0), 5);
            SetSecondSpanValue(new Vector3(1, 0, val), 3);
            GCP03.SetTipsPointsValue(new Vector3(0, 1, 0), new Vector3(1, 0, val));
        }
        ansSelected = true;
    }

    public void ClickZeroBtn()
    {
        int val = 0;
        if (bVal)
        {
            SetFirstSpanValue(new Vector3(1, 0, 0), 2);
            SetSecondSpanValue(new Vector3(0, 1, val), 5);
            GCP03.SetTipsPointsValue(new Vector3(1, 0, 0), new Vector3(0, 1, val));
        }
        else
        {
            SetFirstSpanValue(new Vector3(0, 1, 0), 5);
            SetSecondSpanValue(new Vector3(1, 0, val), 2);
            GCP03.SetTipsPointsValue(new Vector3(0, 1, 0), new Vector3(1, 0, val));
        }
        ansSelected = true;
    }

    public void ClickNegOneBtn()
    {
        int val = -1;
        if (bVal)
        {
            SetFirstSpanValue(new Vector3(1, 0, 0), 2);
            SetSecondSpanValue(new Vector3(0, 1, val), 4);
            GCP03.SetTipsPointsValue(new Vector3(1, 0, 0), new Vector3(0, 1, val));
        }
        else
        {
            SetFirstSpanValue(new Vector3(0, 1, 0), 5);
            SetSecondSpanValue(new Vector3(1, 0, val), 1);
            GCP03.SetTipsPointsValue(new Vector3(0, 1, 0), new Vector3(1, 0, val));
        }
        ansSelected = true;
    }

    public void ClickSubmitBtn()
    {
        if (ansSelected)
        {
            GCP03.TriggerRotation(choiceID1, choiceID2, choice1Pos, choice2Pos);
            ClearSpanValues(txtFBChoice1.text, txtChoice2.text);
            ClickClearChoice2Btn();
            ansSelected = false;
        }

        //Show the span value in input panel
        /*if(choiceID1 != 0 && choiceID2 != 0)
        {
            txtFBChoice1.text = choice1Pos.x + "\n" + choice1Pos.y + "\n" + choice1Pos.z;
            txtFBChoice2.text = choice2Pos.x + "\n" + choice2Pos.y + "\n" + choice2Pos.z;
        }*/

        GameRoot.instance.audioService.PlayUIAudio(Constants.audioP03Click);
    }

    public void ClickSwitchBtn()
    {
        bVal = !bVal;
        if (bVal)
        {
            SetFirstSpanValue(new Vector3(1, 0, 0), 0);
            SetSecondSpanValue(new Vector3(0, 1, 0), 0);
        }
        else
        {
            SetFirstSpanValue(new Vector3(0, 1, 0), 0);
            SetSecondSpanValue(new Vector3(1, 0, 0), 0);
        }

        ClickClearChoice2Btn();
        ClickClearChoice1Btn();
        SwapFeedbackPanel();
        
//        GCP03.SetTipsPointsValue(new Vector3(1, 0, 0), new Vector3(0, 1, 0));
    }

    public void ClickExitQuestionBtn()
    {
        GCP03.exitPuzzle();
    }

    public void ClickClearChoice1Btn()
    {
  //      GCP03.ReactivateChoiceBtn(choiceID1);
        txtChoice1.text = "";
  //      choiceID1 = 0;
        choice1Pos = Vector3.zero;


        GameRoot.instance.audioService.PlayUIAudio(Constants.audioP03ClickClear);
    }

    public void ClickClearChoice2Btn()
    {
 //       GCP03.ReactivateChoiceBtn(choiceID2);
        txtChoice2.text = "";
 //       choiceID2 = 0;
        choice2Pos = Vector3.zero;

        GameRoot.instance.audioService.PlayUIAudio(Constants.audioP03ClickClear);
    }

    public void SwapFeedbackPanel()
    {
        if (bVal)
        {
            GCP03.tipsGlow1.SetActive(false);
            GCP03.tipsGlow2.SetActive(true);
            GCP03.vectorGlow1.SetActive(false);
            GCP03.vectorGlow2.SetActive(true);


            txtFBChoice1.text = "1\n0\n0";
            txtChoice1.text = "1\n0\n0";
            txtFBChoice2.text = "0\n1\n<color=red>b</color>";
            txtChoice2.text = "0\n1\n<color=red>b</color>";
        }
        else
        {

            GCP03.tipsGlow1.SetActive(true);
            GCP03.tipsGlow2.SetActive(false);
            GCP03.vectorGlow1.SetActive(true);
            GCP03.vectorGlow2.SetActive(false);

            txtFBChoice1.text = "0\n1\n0";
            txtChoice1.text = "0\n1\n0";
            txtFBChoice2.text = "1\n0\n<color=red>b</color>";
            txtChoice2.text = "1\n0\n<color=red>b</color>";
        }
    }


    public void ClearSpanValues(string choice1, string choice2)
    {
//        GCP03.ReactivateChoiceBtn(choiceID1);
//        GCP03.ReactivateChoiceBtn(choiceID2);
        txtChoice1.text = choice1;
        txtChoice2.text = choice2;
 //       choiceID1 = 0;
 //       choiceID2 = 0;
        choice1Pos = Vector3.zero;
        choice2Pos = Vector3.zero;

        Debug.Log("Test");
    }

    public void ShowChoicePanel(bool status)
    {
        Animation panelChoiceAni = panelChoice.GetComponent<Animation>();

        if (status)
        {
            SetActive(panelChoice, true);
            panelChoiceAni.Play("ChoiceShow");
            PanelAnswer.SetActive(true);
            //PanelInstruction.SetActive(true);
            PanelFeedback.SetActive(true);
            PanelProgress.SetActive(true);
            
        }
        else
        {
            //   panelChoiceAni.Play("ChoiceHide");
            panelChoice.gameObject.SetActive(false);
            PanelAnswer.SetActive(false);
            PanelInstruction.SetActive(false);
            PanelFeedback.SetActive(false);
            PanelProgress.SetActive(false);
            SetActive(panelChoice, false);
        }
    }

    public void ClickChoiceBtn(string value)
    {
        string[] valueArray = value.Split(',');

        SetSecondSpanValue(new Vector3(
                                        (float)Convert.ToDouble(valueArray[0]),
                                        (float)Convert.ToDouble(valueArray[1]),
                                        (float)Convert.ToDouble(valueArray[2])),
                                        Convert.ToInt32(valueArray[3]));
    }

    public void SetPanelChoice(int puzzleID)
    {
        panelChoice.gameObject.SetActive(false);

        panelChoice.gameObject.SetActive(true);
    }

    public void SetInstructionalText(string content)
    {
        txtInstruction.text = content;
    }

    public void SetTime(string val)
    {
        timeNumber.text = val;
    }

    public void SetMoves(int val)
    {
        movesNumber.text = val.ToString();
    }
}
