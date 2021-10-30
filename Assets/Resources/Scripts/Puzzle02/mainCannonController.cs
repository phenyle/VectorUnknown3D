using UnityEngine;
using UnityEngine.UI;

public class mainCannonController : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;
    private AudioSource audioSource;
    public Transform yawBase;
    public Transform pitchPivot;

    public GameObject text;
    public GameObject text2;

    [Header("----------- Animation ------------")]
    public bool animate = false;
    [Range(0.001f, 0.5f)]
    public float rotateSpeed = 0.01f;

    public bool inTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = GameRoot.instance.audioService.GetFXAudioClip(Constants.audioP03RotatedPuzzleEnvironment);


    }

    public void Update()
    {
        if(animate)
        {
            //cannon does funky things when you give it a [0,0,0]  best just not to do
            //anything if that happens. No ship is at [0,0,0]
            if(GCP02.currAnswer[0] != 0 && GCP02.currAnswer[2] != 0)
                yawBase.rotation = Quaternion.Slerp(yawBase.rotation, Quaternion.LookRotation(new Vector3(GCP02.currAnswer[0], yawBase.position.y, GCP02.currAnswer[2]).normalized), rotateSpeed);
            //            pitchPivot.rotation = Quaternion.Slerp(pitchPivot.rotation, Quaternion.LookRotation(new Vector3(pitchPivot.position.x, yawBase.position.y, pitchPivot.position.z).normalized), rotateSpeed);



            if (yawBase.rotation == Quaternion.LookRotation(new Vector3(GCP02.currAnswer[0], yawBase.position.y, GCP02.currAnswer[2]).normalized))
            {
                animate = false;
                //audioSource.Stop();
            }

        }
        else
        {
            animate = false;
        }


    }

    public void AnimateCannon()
    {
        animate = true;

        //if (yawBase.rotation == Quaternion.LookRotation(new Vector3(GCP02.currAnswer[0], yawBase.position.y, GCP02.currAnswer[2]).normalized))
        //{
        //    animate = false;
        //    audioSource.Stop();
        //}
        //else
        //{

        //    if (audioSource.clip == null)
        //    {
        //        audioSource.clip = GameRoot.instance.audioService.GetFXAudioClip(Constants.audioP03RotatedPuzzleEnvironment);
        //    }

        //    audioSource.loop = true;
        //    audioSource.Play();
        //}

    }


    //used to trigger UI
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GCP02.isMainCannonTrigger = true;
            inTrigger = true;
            GameRoot.ShowTips("Press \"E\" to fire the Cannon Ball", true, false);
            //text.GetComponent<Text>().color = Color.yellow;
            //text2.GetComponent<Text>().color = Color.yellow;
        }
    }

    //used to trigger UI
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GCP02.isMainCannonTrigger = false;
            inTrigger = false;
            GameRoot.ShowTips("Press \"E\" to fire the Cannon Ball", false, false);
            //text.GetComponent<Text>().color = Color.white;
            //text2.GetComponent<Text>().color = Color.white;
        }
    }
}
