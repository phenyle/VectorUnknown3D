using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSelected : MonoBehaviour
{
	//sText & sBrackets are the matrixes the player is currently colliding with
	//oT & oB are the current selected Matrix
	private GameObject getCanvas, sText, sBrackets, oT, oB;

	//current is the selected matrix
	//First only true when no matrix is selected
	//inCollision only true when in collider of matrix
	private bool current, first, inCollision;

	// Game Controllers
	public GameObject GCP02;

	// Start is called before the first frame update
	void Start()
	{
		current = false;
		first = true;
		inCollision = false;
	}

	// Update is called once per frame
	void Update()
	{
		//If E is pressed and is the first matrix selected and currently in matrix selection
		if (Input.GetKeyDown(KeyCode.E) && first && inCollision)
		{
			oT = sText;
			oB = sBrackets;
			oT.gameObject.GetComponent<Text>().color = Color.green;
			oB.gameObject.GetComponent<Text>().color = Color.green;
			current = true;
			first = false;
		}

		//If E is pressed and currently in matrix selection
		else if (Input.GetKeyDown(KeyCode.E) && !first && inCollision)
		{
			oT.gameObject.GetComponent<Text>().color = Color.black;
			oB.gameObject.GetComponent<Text>().color = Color.black;
			oT = sText;
			oB = sBrackets;
			oT.gameObject.GetComponent<Text>().color = Color.green;
			oB.gameObject.GetComponent<Text>().color = Color.green;
			current = true;
		}

		//Current matrixes are colored green, all other matixes are colored black
		if (current && !GCP02.GetComponent<GameControllerPuzzle02>().firedCannonYet)
		{
			oT.gameObject.GetComponent<Text>().color = Color.green;
			oB.gameObject.GetComponent<Text>().color = Color.green;
		}
	}

	//On enter, save the matrix selection to sText and sBrackets, set inCollision to true when entering
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Puzzle02Matrix")
		{
			inCollision = true;
			getCanvas = other.transform.Find("Canvas").gameObject;
			sText = getCanvas.transform.Find("Text").gameObject;
			sBrackets = getCanvas.transform.Find("Brackets").gameObject;
		}
	}

	//Set inCollision to false when exiting
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Puzzle02Matrix")
		{
			inCollision = false;
		}
	}
}
