using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cannonController : MonoBehaviour
{

    public GameControllerPuzzle02 GCP02;
    public int[] vector;
    public GameObject vectorText;
    public int index;
    public int cannonNumber;
	public GameObject prevText;
	public GameObject prevBracket;
	public GameObject text;
    public GameObject bracket;

    // Start is called before the first frame update
    void Start()
    {
        // vectorText.gameObject.SetActive(false);
        // GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
    }

    //used to trigger UI
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("vector" + vector[0] + ", " + vector[1]);
            // vectorText.gameObject.SetActive(true);
            GCP02.currentVector = vector;
            GCP02.currentCannonMaterial = this.GetComponent<Renderer>().material;
            GCP02.isCannonTrigger = true;
            text.GetComponent<Text>().color = Color.yellow;
            bracket.GetComponent<Text>().color = Color.yellow;
			prevText = text;
			prevBracket = bracket;

            //---------------------------------New Tips Function--------------------------------------
                GameRoot.ShowTips("Press \"E\" to pick the Cannon", true, false);
            //--------------------------------------------------------------------------------------------
        }
    }

    //used to trigger UI
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // vectorText.gameObject.SetActive(false);
            //GCP02.currentVector = null;
            GCP02.isCannonTrigger = false;
            text.GetComponent<Text>().color = Color.white;
            bracket.GetComponent<Text>().color = Color.white;

            //---------------------------------New Tips Function--------------------------------------
            GameRoot.ShowTips("", false, false);
            //--------------------------------------------------------------------------------------------
        }
    }
}
