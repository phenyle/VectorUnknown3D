using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButtons : MonoBehaviour
{
    [Header("Title Cards")]
    public GameObject stageTitle;
    public GameObject stageDescrip;
    public GameObject lvl1comp;
    public TextMeshProUGUI lvl1time;
    public GameObject lvl2comp;
    public TextMeshProUGUI lvl2time;
    public GameObject lvl3comp;
    public TextMeshProUGUI lvl3time;

    public SceneNameMod sceneName;
    public PuzzleCompleteMod puzzleComplete;

    private int level = 0;
    private bool submit = false;
    private string stageName;


    public int getLevel()
    {
        return level;
    }

    public void setLevel(int val)
    {
        level = val;
    }

    public void setSceneName(SceneNameMod val)
    {
        sceneName = val;
    }

    public void setPuzzleCompleteName(PuzzleCompleteMod val)
    {
        puzzleComplete = val;
    }

    public void submitButton()
    { 
        //while the player is in the portal, if they select the "GO" button
        //Find which Puzzle/Scene this portal applies to
        switch (sceneName)
        {
            case SceneNameMod.Puzzle01SceneName:
                //Find which level the player selected and goto that puzzle
                //at that level
                switch (level)
                {
                    case 0:
                        GameRoot.ShowTips("Please select a level", true, false);
                        break;
                    case 1:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle01SceneName);
                        break;
                    case 2:
                        GameRoot.ShowTips("That level doesn't exist right now\nCheck back later", true, false);
                        break;
                    case 3:
                        GameRoot.ShowTips("That level doesn't exist right now\nCheck back later", true, false);
                        break;
                    default:
                        GameRoot.ShowTips("Error: what?? how?", true, false);
                        break;

                }
                break;

            case SceneNameMod.Puzzle02SceneName:
                //Find which level the player selected and goto that puzzle
                //at that level
                switch (level)
                {
                    case 0:
                        GameRoot.ShowTips("Please select a level", true, false);
                        break;
                    case 1:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle02_1SceneName);
                        break;
                    case 2:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle02_2SceneName);
                        break;
                    case 3:
                        GameRoot.ShowTips("That level doesn't exist right now\nCheck back later", true, false);
                        break;
                    default:
                        GameRoot.ShowTips("Error: what?? how?", true, false);
                        break;

                }
                break;

            case SceneNameMod.Puzzle03SceneName:
                //Find which level the player selected and goto that puzzle
                //at that level
                switch (level)
                {
                    case 0:
                        GameRoot.ShowTips("Please select a level", true, false);
                        break;
                    case 1:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle03_1SceneName);
                        break;
                    case 2:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle03_2SceneName);
                        break;
                    case 3:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle03_3SceneName);
                        break;
                    default:
                        GameRoot.ShowTips("Error: what?? how?", true, false);
                        break;

                }
                break;

            case SceneNameMod.Puzzle04SceneName:
                //Find which level the player selected and goto that puzzle
                //at that level
                switch (level)
                {
                    case 0:
                        GameRoot.ShowTips("Please select a level", true, false);
                        break;
                    case 1:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle04_1SceneName);
                        break;
                    case 2:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle04_2SceneName);
                        break;
                    case 3:
                        GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.puzzle04_3SceneName);
                        break;
                    default:
                        GameRoot.ShowTips("Error: what?? how?", true, false);
                        break;

                }
                break;

            default:
                break;
        }

        this.gameObject.SetActive(false);

    }

    public void setSceneName(string sceneName)
    {
        stageName = sceneName;
    }

    public void setHeaderInfo(string title, string descript)
    {
        stageTitle.GetComponent<Text>().text = title;
        stageDescrip.GetComponent<Text>().text = descript;
    }

    public void setSubmit(bool val)
    {
        submit = val;
    }

    public bool getSubmit()
    {
        return submit;
    }

    public void ActivateBestTimes()
    {
        switch (sceneName)
        {
            case SceneNameMod.Puzzle02SceneName:
                if (GameRoot.player.users.p2_1clear_time != 0)
                {
                    lvl1comp.SetActive(true);
                    lvl1time.text = ConvertTime(GameRoot.player.users.p2_1clear_time);
                }
                else
                    lvl1comp.SetActive(false);

                if (GameRoot.player.users.p2_2clear_time != 0)
                {
                    lvl2comp.SetActive(true);
                    lvl2time.text = ConvertTime(GameRoot.player.users.p2_2clear_time);
                }
                else
                    lvl2comp.SetActive(false);


                break;

            case SceneNameMod.Puzzle03SceneName:
                if (GameRoot.player.users.p3_1clear_time != 0)
                {
                    lvl1comp.SetActive(true);
                    lvl1time.text = ConvertTime(GameRoot.player.users.p3_1clear_time);
                }
                else
                    lvl1comp.SetActive(false);

                if (GameRoot.player.users.p3_2clear_time != 0)
                {
                    lvl2comp.SetActive(true);
                    lvl2time.text = ConvertTime(GameRoot.player.users.p3_2clear_time);
                }
                else
                    lvl2comp.SetActive(false);

                if (GameRoot.player.users.p3_3clear_time != 0)
                {
                    lvl3comp.SetActive(true);
                    lvl3time.text = ConvertTime(GameRoot.player.users.p3_3clear_time);
                }
                else
                    lvl3comp.SetActive(false);

                break;

            case SceneNameMod.Puzzle04SceneName:
                if (GameRoot.player.users.p4_1clear_time != 0)
                {
                    lvl1comp.SetActive(true);
                    lvl1time.text = ConvertTime(GameRoot.player.users.p4_1clear_time);
                }
                else
                    lvl1comp.SetActive(false);

                if (GameRoot.player.users.p4_2clear_time != 0)
                {
                    lvl2comp.SetActive(true);
                    lvl2time.text = ConvertTime(GameRoot.player.users.p4_2clear_time);
                }
                else
                    lvl2comp.SetActive(false);

                if (GameRoot.player.users.p4_3clear_time != 0)
                {
                    lvl3comp.SetActive(true);
                    lvl3time.text = ConvertTime(GameRoot.player.users.p4_3clear_time);
                }
                else
                    lvl3comp.SetActive(false);
                break;

            default:
                lvl1comp.SetActive(false);
                lvl2comp.SetActive(false);
                lvl3comp.SetActive(false);
                break;

        }
    }

    public  void DeactivateTimes()
    {
        lvl1comp.SetActive(false);
        lvl2comp.SetActive(false);
        lvl3comp.SetActive(false);
    }

    public string ConvertTime(float val)
    {
        string temp = "";
        int minutes = (int)val / 60;
        int seconds = (int)val - 60 * minutes;
        if (seconds < 10)
            temp = minutes.ToString() + ":0" + seconds.ToString();
        else
            temp = minutes.ToString() + ":" + seconds.ToString();

        return temp;
    }
}
