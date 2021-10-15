using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Switch : MonoBehaviour
{
    public SpanValue spanValue;
    //public Text txtSpanValue;

    private Transform player;
    private GameControllerPuzzle03 GCP03;

    private void Start()
    {
        //txtSpanValue.text = spanValue.point1 + "\n" +
           //                              spanValue.point2;

        GCP03 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle03>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameRoot.ShowTips("Press \"E\" to trigger the switch", true, false);

            //GCP03.isChoosing = true;
            //GCP03.rx = spanValue.x;
            //GCP03.ry = spanValue.y;
            //GCP03.rz = spanValue.z;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameRoot.ShowTips("Press \"E\" to trigger the switch", false, false);

            //GCP03.isChoosing = false;
        }
    }
}
