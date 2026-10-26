using UnityEngine;

public class QuestionTrigger : MonoBehaviour
{
    private GameControllerPuzzle03 GCP03;
    public int convoNumber;
    public bool exiting;
    private float timeOutMax = 1.0f;
    private float currTime = 0.0f;

    private void Start()
    {
        GCP03 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle03>();
        exiting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (exiting)
                alarmCollider();
            else
            {
                GCP03.enterPuzzle();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            GCP03.exitPuzzle();
        }
    }

    public void Update()
    {
        if (exiting)
        {
            currTime += Time.deltaTime;
            if(currTime > timeOutMax)
            {
                this.GetComponent<Collider>().enabled = true;
                if (currTime > timeOutMax + 0.2f)
                    exiting = false;
            }
        }
    }

    public void alarmCollider()
    {
        currTime = 0.0f;
        exiting = true;
    }
}
