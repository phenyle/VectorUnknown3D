using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class cannonController : MonoBehaviour
{

    public GameControllerPuzzle02 GCP02;
    public TextMeshProUGUI text;
    public TextMeshProUGUI bracket;
    public GameObject cannonObject;

    public int[] vector;

    private bool inTrigger = false;


    // Start is called before the first frame update
    void Start()
    {
        // vectorText.gameObject.SetActive(false);
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
    }

    public void SetCannon(int[] vect)
    {
        vector = vect;
        text.text = vector[0].ToString() + "\n" + vector[1].ToString();
    }

    //used to trigger UI
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("vector" + vector[0] + ", " + vector[1]);
            inTrigger = true;
            // vectorText.gameObject.SetActive(true);
            text.color = Color.yellow;
            bracket.color = Color.yellow;


            //---------------------------------New Tips Function--------------------------------------
                GameRoot.ShowTips("Press \"E\" to pick the Cannon", Color.green, true, false);
            //--------------------------------------------------------------------------------------------
        }
    }

    public void Update()
    {
        if(this.inTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GCP02.SelectCannon(this);
            }
        }
    }

    //used to trigger UI
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // vectorText.gameObject.SetActive(false);
            //GCP02.currentVector = null;
            inTrigger = false;
            text.color = Color.white;
            bracket.color = Color.white;

            //---------------------------------New Tips Function--------------------------------------
            GameRoot.ShowTips("", false, false);
            //--------------------------------------------------------------------------------------------
        }
    }
}
