using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairShip : GameControllerRoot
{

    public GameObject Shipwreck;
    public GameObject Shipwreck2;
    public GameObject CompleteShip;

    public GameObject Parts_1;
    public GameObject Parts_2;
    public GameObject Parts_3;
    public GameObject Parts_4;


    bool isTrue = false;



    void start()
    {

    }
    void Update()
    {
        //***************** if Player finshes puzzzle 1
        if (GameRoot.instance.puzzleCompleted[0] == true)
        {


            if (isTrue == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Parts_1.SetActive(true);

                    GameRoot.ShowTips("", false, false);
                }


            }
        }
        //Puzzle 2
        if (GameRoot.instance.puzzleCompleted[1] == true)
        {


            if (isTrue == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Parts_2.SetActive(true);

                    GameRoot.ShowTips("", false, false);
                }


            }
        }
        //Puzzle 3
        if (GameRoot.instance.puzzleCompleted[2] == true)
        {


            if (isTrue == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Parts_3.SetActive(true);
                    GameRoot.ShowTips("", false, false);
                }


            }
        }
        //puzzle 4
        if (GameRoot.instance.puzzleCompleted[3] == true)
        {


            if (isTrue == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Parts_4.SetActive(true);

                    GameRoot.ShowTips("", false, false);
                }


            }
        }
        if (GameRoot.instance.puzzleCompleted[0] == true && GameRoot.instance.puzzleCompleted[1] == true && GameRoot.instance.puzzleCompleted[2] == true && GameRoot.instance.puzzleCompleted[3] == true)
        {


            if (isTrue == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CompleteShip.SetActive(true);
                    Shipwreck.SetActive(false);
                    Shipwreck.SetActive(false);
                    Parts_1.SetActive(false);
                    Parts_2.SetActive(false);
                    Parts_3.SetActive(false);
                    Parts_4.SetActive(false);

                    GameRoot.ShowTips("", false, false);
                }


            }
        }
    }


   

    //used to trigger UI
    private void OnTriggerEnter(Collider other)
    {
        if (GameRoot.instance.puzzleCompleted[0] == true && GameRoot.instance.puzzleCompleted[1] == true && GameRoot.instance.puzzleCompleted[2] == true && GameRoot.instance.puzzleCompleted[3] == true)
        {
            if (other.gameObject.tag == "Player")
        {
            GameRoot.ShowTips("Press \"E\" to repair ship", true, false);
            isTrue = true;

        }


        }
        else {
            if (other.gameObject.tag == "Player")
            {
                GameRoot.ShowTips("Press \"E\" to place ship parts", true, false);
                isTrue = true;
            }
        }
    }

//used to trigger UI
private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameRoot.ShowTips("", false, false);
            isTrue = false;
        }
    }
}
