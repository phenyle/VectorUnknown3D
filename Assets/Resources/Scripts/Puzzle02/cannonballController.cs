using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cannonballController : MonoBehaviour
{

    private GameControllerPuzzle02 GCP02;
    private DatabasePuzzle02 DBP02;
    private DatabasePuzzle02_02 DBP02_02;
	public GameObject text;
	public GameObject newB;
	public GameObject newT;
    public GameObject brackets;
	public GameObject currBrackets;
	public GameObject currText;
	public bool selected;
    public transformMatrix transform;
    public int index; 
    private int gameIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
		selected = false;
        text.gameObject.GetComponent<Text>().color = Color.black;
        brackets.gameObject.GetComponent<Text>().color = Color.black;

        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();

        if(GCP02.stageNumber == 1)
        {
            DBP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<DatabasePuzzle02>();
            DBP02.InitDatabase();
            transform.values = new int[] { 0, 0, 0, 0 };
            transform.values = DBP02.transforms0[index].values;
        }
        else if(GCP02.stageNumber == 2)
        {
            DBP02_02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<DatabasePuzzle02_02>();
            DBP02_02.InitDatabase();
            transform.values = new int[] { 0, 0, 0, 0 };
            transform.values = DBP02_02.transforms0[index].values;
        }


        Debug.Log("Matrix" + index + ": " + transform.values[0] + ", " + transform.values[1]
                + ", " + transform.values[2] + ", " + transform.values[3]);

        text.GetComponent<Text>().text = "" + transform.values[0] + "  " + transform.values[1] + "\n" 
            + transform.values[2] + "  " + transform.values[3];
    }

    void Update()
    {
        if (GCP02.ActiveBoat != gameIndex) {
            UpdateValues(GCP02.ActiveBoat);
            gameIndex = GCP02.ActiveBoat;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(GCP02.stageNumber == 1)
            {
                if (GCP02.ActiveBoat == 0) { transform.values = DBP02.transforms0[index].values; }
                else if (GCP02.ActiveBoat == 1) { transform.values = DBP02.transforms1[index].values; }
                else if (GCP02.ActiveBoat == 2) { transform.values = DBP02.transforms2[index].values; }
                else if (GCP02.ActiveBoat == 3) { transform.values = DBP02.transforms3[index].values; }
                else if (GCP02.ActiveBoat == 4) { transform.values = DBP02.transforms4[index].values; }
                else { transform.values = DBP02.transforms5[index].values; }
            }
            else if(GCP02.stageNumber == 2)
            {
                if (GCP02.ActiveBoat == 0) { transform.values = DBP02_02.transforms0[index].values; }
                else if (GCP02.ActiveBoat == 1) { transform.values = DBP02_02.transforms1[index].values; }
                else if (GCP02.ActiveBoat == 2) { transform.values = DBP02_02.transforms2[index].values; }
                else if (GCP02.ActiveBoat == 3) { transform.values = DBP02_02.transforms3[index].values; }
                else if (GCP02.ActiveBoat == 4) { transform.values = DBP02_02.transforms4[index].values; }
                else { transform.values = DBP02_02.transforms5[index].values; }
            }
            Debug.Log("Matrix" + index + ": " + transform.values[0] + ", " + transform.values[1]
                + ", " + transform.values[2] + ", " + transform.values[3]);


			text.gameObject.GetComponent<Text>().color = Color.yellow;
			brackets.gameObject.GetComponent<Text>().color = Color.yellow;

			GCP02.currentTransformMatrix = transform.values;
            GCP02.isCannonballTrigger = true;



			//---------------------------------New Tips Function--------------------------------------
			GameRoot.ShowTips("Press \"E\" to pick the Cannon Ball", true, false);
            //--------------------------------------------------------------------------------------------
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            text.gameObject.GetComponent<Text>().color = Color.black;
            brackets.gameObject.GetComponent<Text>().color = Color.black;
            GCP02.isCannonballTrigger = false;
			selected = false;
			//---------------------------------New Tips Function--------------------------------------
			GameRoot.ShowTips("", false, false);
            //--------------------------------------------------------------------------------------------
        }
    }

    public void UpdateValues(int ActiveBoat)
    {
        if(GCP02.stageNumber == 1)
        {
            if (GCP02.ActiveBoat == 0) { transform.values = DBP02.transforms0[index].values; }
            else if (GCP02.ActiveBoat == 1) { transform.values = DBP02.transforms1[index].values; }
            else if (GCP02.ActiveBoat == 2) { transform.values = DBP02.transforms2[index].values; }
            else if (GCP02.ActiveBoat == 3) { transform.values = DBP02.transforms3[index].values; }
            else if (GCP02.ActiveBoat == 4) { transform.values = DBP02.transforms4[index].values; }
            else { transform.values = DBP02.transforms5[index].values; }
        }
        else if(GCP02.stageNumber == 2)
        {
            if (GCP02.ActiveBoat == 0) { transform.values = DBP02_02.transforms0[index].values; }
            else if (GCP02.ActiveBoat == 1) { transform.values = DBP02_02.transforms1[index].values; }
            else if (GCP02.ActiveBoat == 2) { transform.values = DBP02_02.transforms2[index].values; }
            else if (GCP02.ActiveBoat == 3) { transform.values = DBP02_02.transforms3[index].values; }
            else if (GCP02.ActiveBoat == 4) { transform.values = DBP02_02.transforms4[index].values; }
            else { transform.values = DBP02_02.transforms5[index].values; }
        }

        Debug.Log("Updated Matrix" + index + ": " + transform.values[0] + ", " + transform.values[1]
                + ", " + transform.values[2] + ", " + transform.values[3]);

        text.GetComponent<Text>().text = "" + transform.values[0] + "  " + transform.values[1] + "\n"
           + transform.values[2] + "  " + transform.values[3];
    }
}
