using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class VectorPoint : MonoBehaviour
{
    public int questionNum;
    public float defaultScalar = 0;
    public float defaultX = 0, defaultY = 0, defaultZ = 0;
    public Text info;
    public Text infoTop;
    public bool finishedQuestion = false;

    private GameControllerPuzzle01 GCP01;

    private void Start()
    {
        GCP01 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle01>();
    }

    private void LateUpdate()
    {
        if(!finishedQuestion)
        {
            info.text = "( " + this.transform.position.x + "," + this.transform.position.z + "," + this.transform.position.y + " )";
            infoTop.text = info.text;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !finishedQuestion)
        {
            if(questionNum != 0)
            {
                GameRoot.ShowTips("Press \"E\" to trigger the question", true, false);

                GCP01.isInQues = true;
                GCP01.SendConstrains(defaultScalar, defaultX, defaultY, defaultZ, questionNum);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !finishedQuestion)
        {
            GameRoot.ShowTips("", false, false);

            GCP01.isInQues = false;
            GCP01.questionNum = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !finishedQuestion && questionNum != 0)
        {
            if (!GCP01.isTriggerQuestion)
            {
                GameRoot.ShowTips("Press \"E\" to trigger the question", true, false);
            }
            else
            {
                GameRoot.ShowTips("", false, false);
            }
        }
    }
}
