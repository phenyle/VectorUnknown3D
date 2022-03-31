using UnityEngine;

public class Puzzle01Complete : MonoBehaviour
{
    private GameControllerPuzzle01 GCP01;

    private void Start()
    {
        GCP01 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle01>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GCP01.isInMast = true;

            GameRoot3D.ShowTips("Press \"E\" to grab the mast", true, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GCP01.isInMast = false;

            GameRoot3D.ShowTips("", false, false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!GCP01.isTriggerMast)
            {
                GameRoot3D.ShowTips("Press \"E\" to grab the mast", true, false);
            }
            else
            {
                GameRoot3D.ShowTips("", false, false);
            }
        }
    }
}