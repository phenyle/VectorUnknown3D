using UnityEngine;
using UnityEngine.UI;

public class ChoiceClickButton : MonoBehaviour
{
    public Vector3 spanValue = new Vector3();
    public int choiceID;

    private Button btnChoice;
    private GameControllerPuzzle03 GCP03;

    [HideInInspector]
    public Puzzle03Window P03W;

    private void Start()
    {
        GCP03 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle03>();

        P03W = GameObject.Find("Puzzle03Window").GetComponent<Puzzle03Window>();

        btnChoice = GetComponent<Button>();

        btnChoice.onClick.AddListener(TaskOnClick);
    }

    private void TaskOnClick()
    {      
        btnChoice.interactable = false;
        P03W.ClickClearChoice2Btn();
        GCP03.SetSpanValue(spanValue, choiceID);
        GCP03.activateOtherChoiceBtn(choiceID);
        GameRoot.instance.audioService.PlayUIAudio(Constants.audioP03Click);
    }
}
