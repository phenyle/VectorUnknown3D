using UnityEngine;

public class SelectionTutorial : MonoBehaviour
{
    public int value;
    public GameObject Text;
	public GameObject bkObj;
    public GameObject pressedE;
    //public GameObject gameContoller;
    //private TutorialContorller tutorialContorller;
    private TutorialWindow tutorialWindow;

    void Start()
    {
        //pressedE.SetActive(false);
        Text.SetActive(false);
		bkObj.SetActive(false);
		//tutorialContorller = gameContoller.GetComponent<TutorialContorller>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (tutorialWindow == null)
            {
                tutorialWindow = GameObject.FindGameObjectWithTag("TutorialWindow").GetComponent<TutorialWindow>();
            }

            if (pressedE == null)
            {
                pressedE = tutorialWindow.pressE;
            }

            GameRoot.ShowTips("Press \"E\" to select", true, false);
			bkObj.SetActive(true);
            Text.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (pressedE == null)
            {
                pressedE = tutorialWindow.pressE;
            }

            GameRoot.ShowTips("", false, false);
            Text.SetActive(false);
			bkObj.SetActive(false);
		}
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (value == 3)
            {
                tutorialWindow.selectionComplete = true;
            }
        }
    }
}
