using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    public GameObject stage02;
    private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if(playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if(other.tag == "Player")
        {           
            playerController.level2ChangeStage = true;
            GameRoot.ShowTips("Press \"E\" to enter", true, false);
            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E pressed to change to level 2");
                GameRoot.ShowTips("", false, false);
                // stage02.SetActive(true);                         
                // GameObject.Find("Puzzle02Window").GetComponent<Puzzle02Window>().switchStage();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerController.level2ChangeStage = false;
            GameRoot.ShowTips("", false, false);
        }
    }
}
