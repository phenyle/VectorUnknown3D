using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreaureChestScript : GameControllerRoot
{

    public GameObject Chest;
    public GameObject ClosedLid;
    public GameObject OpenLid;
    bool isTrue = false;

    public Sprite[] spriteChecks;
    public MainWindow MW;


    public SceneName sceneName;
    public PuzzleComplete puzzleComplete;

    private PlayerController playerController;


  


        void start()
    {

    }
    void Update()
    {
        if (isTrue == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Chest.SetActive(false);
                GameRoot.ShowTips("", false, false);
            }
        }
    }
    

    //used to trigger UI
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameRoot.ShowTips("Press \"E\" to collect Ship Parts", true, false);
            ClosedLid.SetActive(false);
            OpenLid.SetActive(true);
            isTrue = true;
        }
    }

    //used to trigger UI
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameRoot.ShowTips("", false, false);
            isTrue = false;
            ClosedLid.SetActive(true);
            OpenLid.SetActive(false);
        }
    }
}
