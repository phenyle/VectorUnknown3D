using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PortalTrigger : MonoBehaviour
{
    public GameObject player;
    public bool inPortal = false;
    public Puzzle04Controller PC04;

    private float triggerDelay;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PC04 = this.GetComponentInParent<Puzzle04Controller>();
    }

    public void Update()
    {
        if (triggerDelay > 0)
        {
            this.GetComponent<Collider>().enabled = false;
            triggerDelay--;
        }
        else
            this.GetComponent<Collider>().enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PC04.OnPortalEnter();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PC04.OnPortalExit();
            triggerDelay = 10f;
        }
    }
  
    public void SetPuzzleController(Puzzle04Controller controller)
    {
        PC04 = controller;
    }
}
