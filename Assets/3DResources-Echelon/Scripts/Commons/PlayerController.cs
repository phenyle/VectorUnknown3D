using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Level2Stage02;
    [HideInInspector]
    public bool isEnterExit = false;
    [HideInInspector]
    public string sceneName;
    private EnterExit portal;

    private void Update()
    {
        if(isEnterExit)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                portal.PuzzleExitActions();
                GameRoot3D.ShowTips("", false, false);
                GameRoot3D.instance.puzzleSystem.EnterPuzzle(sceneName);
            }
        }
    }

    public void SetEnterExit(EnterExit exit)
    {
        portal = exit;
    }
}
