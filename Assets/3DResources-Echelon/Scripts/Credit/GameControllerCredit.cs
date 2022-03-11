using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerCredit : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.menuSceneName);
        }
    }
}
