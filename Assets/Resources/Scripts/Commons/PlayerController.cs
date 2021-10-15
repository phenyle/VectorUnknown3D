using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Level2Stage02;
    [HideInInspector]
    public bool isEnterExit = false;
    public bool level2ChangeStage = false;
    [HideInInspector]
    public string sceneName;

    private void Update()
    {
        if(isEnterExit)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                GameRoot.ShowTips("", false, false);
                GameRoot.instance.puzzleSystem.EnterPuzzle(sceneName);
            }
        }

        if(level2ChangeStage)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                GameRoot.ShowTips("", false, false);
                Level2Stage02.SetActive(true);                         
                GameObject.Find("Puzzle02Window").GetComponent<Puzzle02Window>().switchStage();
            }
        }
    }
}
