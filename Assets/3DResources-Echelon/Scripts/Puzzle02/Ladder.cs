using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;
    private bool inTrigger = false;
    [SerializeField] private GameObject effect;


    // Start is called before the first frame update
    void Start()
    {
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            effect.SetActive(true);
            inTrigger = true;
            GameRoot3D.ShowTips("Press \"E\" to Look from Above", Color.green, true, false);

        }
    }

    public void Update()
    {
        if (inTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GCP02.SwitchCamera();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = false;
            effect.SetActive(false);
        }
    }
}
