using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoardUI : MonoBehaviour
{
    public GameObject mainPanelUI;
    public Collider trigger;
    private bool inTrigger;

    [Header("-------- MAIN HEADER --------")]
    public Text mn_loginName;
    public Button mn_OverallBtn;
    public Button mn_Stage1Btn;
    public Button mn_Stage2Btn;
    public Button mn_Stage3Btn;
    public Button mn_Stage4Btn;
    public Button mn_BackBtn;

    [Header("-------- OVERALL --------")]
    public GameObject ov_Panel;
    public Text ov_totalTime;
    public GameObject ov_Table;
    public Table ov_scoresTable;
    //public GameObject ov_totalTime;

    [Header("-------- STAGE 1 --------")]
    public GameObject s1_Panel;
    public Text s1_totalTime;
    public GameObject s1_Table;
    public Table s1_scoresTable;

    [Header("-------- STAGE 2 --------")]
    public GameObject s2_Panel;
    public Text s2_totalTime;
    public GameObject s2_Table;
    public Table s2_scoresTable;


    [Header("-------- STAGE 3 --------")]
    public GameObject s3_Panel;
    public Text s3_totalTime;
    public GameObject s3_Table;
    public Table s3_scoresTable;


    [Header("-------- STAGE 4 --------")]
    public GameObject s4_Panel;
    public Text s4_totalTime;
    public GameObject s4_Table;
    public Table s4_scoresTable;



    // Start is called before the first frame update
    void Start()
    {
        mainPanelUI.SetActive(false);
        inTrigger = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = true;
            GameRoot3D.instance.dynamicWindow.ShowTips("Press \"E\" to view records", true, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!inTrigger) return;

        if (inTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
                activateScoreboard();
        }
    }

    private void activateScoreboard()
    {
        mainPanelUI.SetActive(true);
        mn_OverallBtn.Select();
        GameRoot3D.instance.LockPlayerAndCam(true);
        GameRoot3D.instance.ShowCursor(true);

        SetOverallScores();
        ov_Panel.SetActive(true);

        SetStage1Scores();
        s1_Panel.SetActive(false);

        SetStage2Scores();
        s2_Panel.SetActive(false);

        SetStage3Scores();
        s3_Panel.SetActive(false);

        SetStage4Scores();
        s4_Panel.SetActive(false);
    }

    #region Score Entry Methods

    private void SetOverallScores()
    {
        GameObject currRow;


        ov_totalTime.text = ConvertTime(GameRoot3D.player.users.tot_time);

        //stage1
        //GetOrAddComponent<TextMeshProUGUI>(ov_scoresTable.GetCell(2, 2)).text = GameRoot3D.player.records.p1_totAttempts.ToString();
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(3, 2)).color = GameRoot3D.player.records.p1_time > 0 ? Color.white : Color.black;
        currRow = ov_Table.transform.Find("row1").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p1_totAttempts.ToString();
        currRow.transform.Find("col2").GetComponent<Image>().color = GameRoot3D.player.records.p1_time > 0 ? Color.white : Color.black;

        //stage2
        //GetOrAddComponent<TextMeshProUGUI>(ov_scoresTable.GetCell(2, 3)).text = GameRoot3D.player.records.p2_totAttempts.ToString();
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(3, 3)).color = GameRoot3D.player.records.p2_1_time > 0 ? Color.white : Color.black;
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(4, 3)).color = GameRoot3D.player.records.p2_2_time > 0 ? Color.white : Color.black;
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(5, 3)).color = GameRoot3D.player.records.p2_3_time > 0 ? Color.white : Color.black;

        currRow = ov_Table.transform.Find("row2").gameObject;
        currRow.transform.Find("col1").Find("text").GetComponent<Text>().text = GameRoot3D.player.records.p2_totAttempts.ToString();
        currRow.transform.Find("col2").GetComponent<Image>().color = GameRoot3D.player.records.p2_1_time > 0 ? Color.white : Color.black;
        currRow.transform.Find("col3").GetComponent<Image>().color = GameRoot3D.player.records.p2_2_time > 0 ? Color.white : Color.black;
        currRow.transform.Find("col4").GetComponent<Image>().color = GameRoot3D.player.records.p2_3_time > 0 ? Color.white : Color.black;

        //stage3
        //GetOrAddComponent<TextMeshProUGUI>(ov_scoresTable.GetCell(2, 4)).text = GameRoot3D.player.records.p3_totAttempts.ToString();
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(3, 4)).color = GameRoot3D.player.records.p3_1_time > 0 ? Color.white : Color.black;
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(4, 4)).color = GameRoot3D.player.records.p3_2_time > 0 ? Color.white : Color.black;
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(5, 4)).color = GameRoot3D.player.records.p3_3_time > 0 ? Color.white : Color.black;

        currRow = ov_Table.transform.Find("row3").gameObject;
        currRow.transform.Find("col1").Find("text").GetComponent<Text>().text = GameRoot3D.player.records.p3_totAttempts.ToString();
        currRow.transform.Find("col2").GetComponent<Image>().color = GameRoot3D.player.records.p3_1_time > 0 ? Color.white : Color.black;
        currRow.transform.Find("col3").GetComponent<Image>().color = GameRoot3D.player.records.p3_2_time > 0 ? Color.white : Color.black;
        currRow.transform.Find("col4").GetComponent<Image>().color = GameRoot3D.player.records.p3_3_time > 0 ? Color.white : Color.black;

        //stage4
        //GetOrAddComponent<TextMeshProUGUI>(ov_scoresTable.GetCell(2, 5)).text = GameRoot3D.player.records.p4_totAttempts.ToString();
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(3, 5)).color = GameRoot3D.player.records.p4_1_time > 0 ? Color.white : Color.black;
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(4, 5)).color = GameRoot3D.player.records.p4_2_time > 0 ? Color.white : Color.black;
        //GetOrAddComponent<Image>(ov_scoresTable.GetCell(5, 5)).color = GameRoot3D.player.records.p4_3_time > 0 ? Color.white : Color.black;

        currRow = ov_Table.transform.Find("row4").gameObject;
        currRow.transform.Find("col1").Find("text").GetComponent<Text>().text = GameRoot3D.player.records.p4_totAttempts.ToString();
        currRow.transform.Find("col2").GetComponent<Image>().color = GameRoot3D.player.records.p4_1_time > 0 ? Color.white : Color.black;
        currRow.transform.Find("col3").GetComponent<Image>().color = GameRoot3D.player.records.p4_2_time > 0 ? Color.white : Color.black;
        currRow.transform.Find("col4").GetComponent<Image>().color = GameRoot3D.player.records.p4_3_time > 0 ? Color.white : Color.black;
    }

    private void SetStage1Scores()
    {
        GameObject currRow;

        s1_totalTime.text = ConvertTime(GameRoot3D.player.records.p1_totTime);

        //level 1
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(1, 2)).text = GameRoot3D.player.records.p1_totAttempts.ToString();
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(2, 2)).text = GameRoot3D.player.records.p1_time > 0 ? ConvertTime(GameRoot3D.player.records.p1_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(3, 2)).text = GameRoot3D.player.records.p1_time > 0 ? GameRoot3D.player.records.p1_numMove.ToString() : "";
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(4, 2)).text = GameRoot3D.player.records.p1_time > 0 ? GameRoot3D.player.records.p1_answers : "";

        currRow = s1_Table.transform.Find("row1").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p1_totAttempts.ToString();
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p1_time > 0 ? ConvertTime(GameRoot3D.player.records.p1_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p1_time > 0 ? GameRoot3D.player.records.p1_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p1_time > 0 ? GameRoot3D.player.records.p1_answers : "";

    }

    private void SetStage2Scores()
    {
        GameObject currRow;

        s2_totalTime.text = ConvertTime(GameRoot3D.player.records.p2_totTime);

        //level 1
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(2, 2)).text = GameRoot3D.player.records.p2_1_attempts.ToString();
        //GetOrAddComponent<StarBar>(s1_scoresTable.GetCell(3, 2)).score = GameRoot3D.player.records.p2_1_score > 0 ? GameRoot3D.player.records.p2_1_score : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(4, 2)).text = GameRoot3D.player.records.p2_1_time > 0 ? ConvertTime(GameRoot3D.player.records.p2_1_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(5, 2)).text = GameRoot3D.player.records.p2_1_time > 0 ? GameRoot3D.player.records.p2_1_numMove.ToString() : "";

        currRow = s2_Table.transform.Find("row1").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_1_attempts.ToString();
 //       currRow.transform.Find("score").GetComponentInChildren<StarBar>().SetScore(GameRoot3D.player.records.p2_1_score);
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_1_time > 0 ? ConvertTime(GameRoot3D.player.records.p2_1_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_1_time > 0 ? GameRoot3D.player.records.p2_1_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS21").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p2_1_score > 0 ? GameRoot3D.player.records.p2_1_score : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(230, 40 * 9);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p2_1_questions;

        //level 2
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(2, 3)).text = GameRoot3D.player.records.p2_2_attempts.ToString();
        //GetOrAddComponent<StarBar>(s1_scoresTable.GetCell(3, 3)).score = GameRoot3D.player.records.p2_2_score > 0 ? GameRoot3D.player.records.p2_2_score : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(4, 3)).text = GameRoot3D.player.records.p2_2_time > 0 ? ConvertTime(GameRoot3D.player.records.p2_2_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(5, 3)).text = GameRoot3D.player.records.p2_2_time > 0 ? GameRoot3D.player.records.p2_2_numMove.ToString() : "";

        currRow = s2_Table.transform.Find("row2").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_2_attempts.ToString();
//        currRow.transform.Find("score").GetComponentInChildren<StarBar>().SetScore(GameRoot3D.player.records.p2_2_score);
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_2_time > 0 ? ConvertTime(GameRoot3D.player.records.p2_2_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_2_time > 0 ? GameRoot3D.player.records.p2_2_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS22").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p2_2_score > 0 ? GameRoot3D.player.records.p2_2_score : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(230, 40 * 8);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p2_2_questions;

        //level 3
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(2, 4)).text = GameRoot3D.player.records.p2_3_attempts.ToString();
        //GetOrAddComponent<StarBar>(s1_scoresTable.GetCell(3, 4)).score = GameRoot3D.player.records.p2_3_score > 0 ? GameRoot3D.player.records.p2_3_score : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(4, 4)).text = GameRoot3D.player.records.p2_3_time > 0 ? ConvertTime(GameRoot3D.player.records.p2_3_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s1_scoresTable.GetCell(5, 4)).text = GameRoot3D.player.records.p2_3_time > 0 ? GameRoot3D.player.records.p2_3_numMove.ToString() : "";

        currRow = s2_Table.transform.Find("row3").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_3_attempts.ToString();
//        currRow.transform.Find("score").GetComponentInChildren<StarBar>().SetScore(GameRoot3D.player.records.p2_3_score);
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_3_time > 0 ? ConvertTime(GameRoot3D.player.records.p2_3_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p2_3_time > 0 ? GameRoot3D.player.records.p2_3_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS23").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p2_3_score > 0 ? GameRoot3D.player.records.p2_3_score : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(230, 40 * 5);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p2_3_questions;

    }

    private void SetStage3Scores()
    {
        GameObject currRow;

        s3_totalTime.text = ConvertTime(GameRoot3D.player.records.p3_totTime);

        //level 1
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(2, 2)).text = GameRoot3D.player.records.p3_1_attempts.ToString();
        //GetOrAddComponent<StarBar>(s3_scoresTable.GetCell(3, 2)).score = GameRoot3D.player.records.p3_1_score > 0 ? GameRoot3D.player.records.p3_1_score : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(4, 2)).text = GameRoot3D.player.records.p3_1_time > 0 ? ConvertTime(GameRoot3D.player.records.p3_1_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(5, 2)).text = GameRoot3D.player.records.p3_1_time > 0 ? GameRoot3D.player.records.p3_1_numMove.ToString() : "";


        currRow = s3_Table.transform.Find("row1").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_1_attempts.ToString();
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_1_time > 0 ? ConvertTime(GameRoot3D.player.records.p3_1_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_1_time > 0 ? GameRoot3D.player.records.p3_1_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS31").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p3_1_score > 0 ? GameRoot3D.player.records.p3_1_score : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50 * GameRoot3D.player.records.p3_1_numMove);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p3_1_movesList;

        //level 2
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(2, 3)).text = GameRoot3D.player.records.p3_2_attempts.ToString();
        //GetOrAddComponent<StarBar>(s3_scoresTable.GetCell(3, 3)).score = GameRoot3D.player.records.p3_2_score > 0 ? GameRoot3D.player.records.p3_2_score : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(4, 3)).text = GameRoot3D.player.records.p3_2_time > 0 ? ConvertTime(GameRoot3D.player.records.p3_2_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(5, 3)).text = GameRoot3D.player.records.p3_2_time > 0 ? GameRoot3D.player.records.p3_2_numMove.ToString() : "";

        currRow = s3_Table.transform.Find("row2").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_2_attempts.ToString();
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_2_time > 0 ? ConvertTime(GameRoot3D.player.records.p3_2_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_2_time > 0 ? GameRoot3D.player.records.p3_2_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS32").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p3_2_score > 0 ? GameRoot3D.player.records.p3_2_score : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50 * GameRoot3D.player.records.p3_2_numMove);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p3_2_movesList;

        //level 3
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(2, 4)).text = GameRoot3D.player.records.p3_3_attempts.ToString();
        //GetOrAddComponent<StarBar>(s3_scoresTable.GetCell(3, 4)).score = GameRoot3D.player.records.p3_3_score > 0 ? GameRoot3D.player.records.p3_3_score : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(4, 4)).text = GameRoot3D.player.records.p3_3_time > 0 ? ConvertTime(GameRoot3D.player.records.p3_3_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s3_scoresTable.GetCell(5, 4)).text = GameRoot3D.player.records.p3_3_time > 0 ? GameRoot3D.player.records.p3_3_numMove.ToString() : "";

        currRow = s3_Table.transform.Find("row3").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_3_attempts.ToString();
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_3_time > 0 ? ConvertTime(GameRoot3D.player.records.p3_3_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p3_3_time > 0 ? GameRoot3D.player.records.p3_3_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS33").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p3_3_score > 0 ? GameRoot3D.player.records.p3_3_score : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50 * GameRoot3D.player.records.p3_3_numMove);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p3_3_movesList;
    }

    private void SetStage4Scores()
    {
        GameObject currRow;

        s4_totalTime.text = ConvertTime(GameRoot3D.player.records.p4_totTime);

        //level 1
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(2, 2)).text = GameRoot3D.player.records.p4_1_attempts.ToString();
        //GetOrAddComponent<StarBar>(s4_scoresTable.GetCell(3, 2)).score = GameRoot3D.player.records.p4_1_avgScore > 0 ? GameRoot3D.player.records.p4_1_avgScore : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(4, 2)).text = GameRoot3D.player.records.p4_1_time > 0 ? ConvertTime(GameRoot3D.player.records.p4_1_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(5, 2)).text = GameRoot3D.player.records.p4_1_time > 0 ? GameRoot3D.player.records.p4_1_numMove.ToString() : "";

        currRow = s4_Table.transform.Find("row1").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_1_attempts.ToString();
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_1_time > 0 ? ConvertTime(GameRoot3D.player.records.p4_1_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_1_time > 0 ? GameRoot3D.player.records.p4_1_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS41").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p4_1_avgScore > 0 ? GameRoot3D.player.records.p4_1_avgScore : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(230, 40 * 9);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p4_1_questions;

        //level 2
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(2, 3)).text = GameRoot3D.player.records.p4_2_attempts.ToString();
        //GetOrAddComponent<StarBar>(s4_scoresTable.GetCell(3, 3)).score = GameRoot3D.player.records.p4_2_avgScore > 0 ? GameRoot3D.player.records.p4_2_avgScore : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(4, 3)).text = GameRoot3D.player.records.p4_2_time > 0 ? ConvertTime(GameRoot3D.player.records.p4_2_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(5, 3)).text = GameRoot3D.player.records.p4_2_time > 0 ? GameRoot3D.player.records.p4_2_numMove.ToString() : "";

        currRow = s4_Table.transform.Find("row2").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_2_attempts.ToString();
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_2_time > 0 ? ConvertTime(GameRoot3D.player.records.p4_2_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_2_time > 0 ? GameRoot3D.player.records.p4_2_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS42").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p4_2_avgScore > 0 ? GameRoot3D.player.records.p4_2_avgScore : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(230, 40 * 8);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p4_2_questions;

        //level 3
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(2, 4)).text = GameRoot3D.player.records.p4_3_attempts.ToString();
        //GetOrAddComponent<StarBar>(s4_scoresTable.GetCell(3, 4)).score = GameRoot3D.player.records.p4_3_avgScore > 0 ? GameRoot3D.player.records.p4_3_avgScore : 0;
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(4, 4)).text = GameRoot3D.player.records.p4_3_time > 0 ? ConvertTime(GameRoot3D.player.records.p4_3_time) : "";
        //GetOrAddComponent<TextMeshProUGUI>(s4_scoresTable.GetCell(5, 4)).text = GameRoot3D.player.records.p4_3_time > 0 ? GameRoot3D.player.records.p4_3_numMove.ToString() : "";

        currRow = s4_Table.transform.Find("row3").gameObject;
        currRow.transform.Find("col1").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_3_attempts.ToString();
        currRow.transform.Find("col2").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_3_time > 0 ? ConvertTime(GameRoot3D.player.records.p4_3_time) : "";
        currRow.transform.Find("col3").Find("text").gameObject.GetComponent<Text>().text = GameRoot3D.player.records.p4_3_time > 0 ? GameRoot3D.player.records.p4_3_numMove.ToString() : "";
        currRow.transform.Find("col4").Find("StarBarS43").GetComponent<StarBar>().SetScore(GameRoot3D.player.records.p4_3_avgScore > 0 ? GameRoot3D.player.records.p4_3_avgScore : 0);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().sizeDelta = new Vector2(230, 40 * 5);
        //currRow.transform.Find("col4").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>().text = GameRoot3D.player.records.p4_3_questions;

    }
    #endregion

    protected string ConvertTime(float time)
    {
        int days, hours, mins, secs;
        string timeString = "";

        days = Mathf.FloorToInt(time / 86400);
        time = time - (days * 86400);
        hours = Mathf.FloorToInt(time / 3600);
        time = time - (hours * 3600);
        mins = Mathf.FloorToInt(time / 60);
        time = time - (mins * 60);
        secs = Mathf.FloorToInt(time);

        if (days >= 1)
            timeString += days.ToString() + "D : " + hours.ToString() + "H : ";
        else if (hours >= 1)
            timeString += hours.ToString() + "H : ";

        timeString += mins.ToString() + "M : " + secs.ToString() + "S";

        return timeString;
    }

    private T GetOrAddComponent<T>(GameObject element) where T : Component
    {
        T component = element.GetComponent<T>();

        if (component == null)
            component = element.AddComponent<T>() as T;

        return component;
    }
    private T GetOrAddComponent<T>(Transform element) where T : Component
    {
        T component = element.gameObject.GetComponent<T>();

        if (component == null)
            component = element.gameObject.AddComponent<T>() as T;

        return component;
    }


    #region BUTTONS
    public void ClickOverallButton()
    {
        GameRoot3D.instance.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        ov_Panel.SetActive(true);

        s1_Panel.SetActive(false);
        s2_Panel.SetActive(false);
        s3_Panel.SetActive(false);
        s4_Panel.SetActive(false);
    }

    public void ClickS1Button()
    {
        GameRoot3D.instance.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        s1_Panel.SetActive(true);

        ov_Panel.SetActive(false);
        s2_Panel.SetActive(false);
        s3_Panel.SetActive(false);
        s4_Panel.SetActive(false);
    }
    public void ClickS2Button()
    {
        GameRoot3D.instance.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        s2_Panel.SetActive(true);

        ov_Panel.SetActive(false);
        s1_Panel.SetActive(false);
        s3_Panel.SetActive(false);
        s4_Panel.SetActive(false);
    }

    public void ClickS3Button()
    {
        GameRoot3D.instance.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        s3_Panel.SetActive(true);

        ov_Panel.SetActive(false);
        s1_Panel.SetActive(false);
        s2_Panel.SetActive(false);
        s4_Panel.SetActive(false);
    }

    public void ClickS4Button()
    {
        GameRoot3D.instance.audioService.PlayUIAudio(Constants.audioUIClickBtn);

        s4_Panel.SetActive(true);

        ov_Panel.SetActive(false);
        s1_Panel.SetActive(false);
        s2_Panel.SetActive(false);
        s3_Panel.SetActive(false);
    }

    public void ClickBackButton()
    {
        GameRoot3D.instance.audioService.PlayUIAudio(Constants.audioUIClickBtn);
        mainPanelUI.SetActive(false);
        GameRoot3D.instance.LockPlayerAndCam(false);
        GameRoot3D.instance.ShowCursor(false);
    }

    #endregion
}
