using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompleteSplash : MonoBehaviour
{
    private EnterExit portal;
    public GameObject mainPanel;

    [SerializeField] private StarBar starBar;

    private TextMeshProUGUI text_Time;

    private GameObject record_Time;
    private GameObject record_Score;   


    // Start is called before the first frame update
    void Start()
    {
        GameRoot3D.instance.GCR.completeSplash = this;
        transform.parent.TryGetComponent(out portal);
        mainPanel = transform.Find("MainPanel").gameObject;
        
        //mainPanel.TryGetComponent<StarBar>(out starBar);
        
        text_Time = mainPanel.transform.Find("text_Time").GetComponent<TextMeshProUGUI>();

        record_Time = mainPanel.transform.Find("record_Time").gameObject;
        record_Score = mainPanel.transform.Find("record_Score").gameObject;

        mainPanel.SetActive(false); 
    }


    public void SetSplash(float prevTime, float newTime, float prevScore, float newScore)
    {
        record_Time.SetActive(newTime < prevTime);
        record_Score.SetActive(newScore > prevScore);

        int minutes = (int)newTime / 60;
        int seconds = (int)newTime - 60 * minutes;
        if (seconds < 10)
            text_Time.text = minutes.ToString() + " : 0" + seconds.ToString();
        else
            text_Time.text = minutes.ToString() + " : " + seconds.ToString();

        starBar.SetScore(newScore, newScore > prevScore);

    }

    public void SelectButton()
    {
        mainPanel.transform.Find("btn_Exit").GetComponent<Button>().Select();
    }

    public void btnExitClick()
    {
        if (portal.firstEntrance)
        {
            portal.PuzzleExitActions();
            portal.firstEntrance = false;
        }
        GameRoot3D.instance.ShowCursor(false);
        GameRoot3D.ShowTips("", false, false);
        GameRoot3D.instance.puzzleSystem.EnterPuzzle(portal.GetPlayerController().sceneName);
    }

}
