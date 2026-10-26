using UnityEngine;

public class TriggerValue : MonoBehaviour
{
    public Vector3 spanValue = new Vector3();
    public int choiceID;
    
    private GameControllerPuzzle03 GCP03;

    private void Start()
    {
        GCP03 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle03>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //GCP03.isSelecting = true;
            //GCP03.spanValue = spanValue;
            //GCP03.choiceID = choiceID;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //GCP03.isSelecting = false;
        }
    }
}
