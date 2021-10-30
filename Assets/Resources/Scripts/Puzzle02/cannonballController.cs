using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class cannonballController : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;

    public int[][] matrix;

    public TextMeshProUGUI matrixCol1text;
    public TextMeshProUGUI matrixCol2text;
    public TextMeshProUGUI bracket;

    private bool inTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        // vectorText.gameObject.SetActive(false);
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
    }

    public void SetCannonBalls(int[][] mat)
    {
        matrix = mat;
        matrixCol1text.text = matrix[0][0].ToString() + "\n" + matrix[0][1].ToString();
        matrixCol2text.text = matrix[1][0].ToString() + "\n" + matrix[1][1].ToString();
    }

    //used to trigger UI
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Matrix:\n" + matrix[0][0].ToString() + "  " + matrix[1][0].ToString() + "\n" + matrix[0][1].ToString() + "  " + matrix[1][1].ToString());
            // vectorText.gameObject.SetActive(true);
            //          GCP02.currentVector = vector;
            //           GCP02.isCannonTrigger = true;
            inTrigger = true;
            matrixCol1text.color = Color.yellow;
            matrixCol2text.color = Color.yellow;
            bracket.color = Color.yellow;


            //---------------------------------New Tips Function--------------------------------------
            GameRoot.ShowTips("Press \"E\" to pick the Matrix", Color.green, true, false);
            //--------------------------------------------------------------------------------------------
        }
    }

    public void Update()
    {
        if(inTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GCP02.SelectCannonBalls(this);
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
            //            GCP02.isCannonTrigger = false;
            inTrigger = false;
            matrixCol1text.color = Color.white;
            matrixCol2text.color = Color.white;
            bracket.color = Color.white;

            //---------------------------------New Tips Function--------------------------------------
            GameRoot.ShowTips("", false, false);
            //--------------------------------------------------------------------------------------------
        }
    }
}
