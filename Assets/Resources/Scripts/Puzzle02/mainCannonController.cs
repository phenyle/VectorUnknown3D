using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class mainCannonController : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;
    private AudioSource audioSource;
    public Transform yawBase;
    public Transform pitchPivot;

    public TextMeshProUGUI text;

    public bool inTrigger = false;

    [Header("----------- Cannon Animation ------------")]
    public bool animate = false;
    [Range(0.001f, 0.5f)]
    public float rotateSpeed = 0.02f;
    public bool canFire = false;
    public float angleToFire = 3.0f;

    [Header("----------- CannonBall Animation ------------")]
    private Vector3 balltarget;

    [Header("----------- Parts ------------")]
    public Collider triggerBox;
    public GameObject mainBarrel;
    public GameObject mainBall;
    public ObjectGlide objGlide;
    private GameObject tempBall;

    [Header("----------- FX ------------")]
    public GameObject splashFX;
    public GameObject cannonGlowFX;
    public GameObject cannonFuzeFX;

    // Start is called before the first frame update
    void Start()
    {
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = GameRoot.instance.audioService.GetFXAudioClip(Constants.audioP03RotatedPuzzleEnvironment);
        triggerBox = this.GetComponent<Collider>();
        text.text = "??\n??";

    }

    public void Update()
    {
        if(animate)
        {
            //cannon does funky things when you give it a [0,0,0]  best just not to do
            //anything if that happens. No ship is at [0,0,0]
            //            pitchPivot.rotation = Quaternion.Slerp(pitchPivot.rotation, Quaternion.LookRotation(new Vector3(pitchPivot.position.x, yawBase.position.y, pitchPivot.position.z).normalized), rotateSpeed);

            Vector3 cannonCurrForward = yawBase.forward;
            Vector3 angleToTarget = ((GCP02.currAnswer + new Vector3(0, yawBase.transform.position.y, 0)) - yawBase.transform.position).normalized;
            if (GCP02.currAnswer.x != 0 && GCP02.currAnswer.z != 0)
                if(Vector3.Angle(cannonCurrForward, angleToTarget) > 0)
                    yawBase.transform.Rotate(Vector3.up, Vector3.Angle(cannonCurrForward, angleToTarget) * rotateSpeed);
                else
                    yawBase.transform.Rotate(Vector3.up, -Vector3.Angle(cannonCurrForward, angleToTarget) * rotateSpeed);


            if (Vector3.Angle(cannonCurrForward, angleToTarget) < angleToFire)
            {
                animate = false;
                canFire = true;
                ToggleTriggerBox(true);
                //audioSource.Stop();
            }

            if (yawBase.rotation == Quaternion.LookRotation(new Vector3(GCP02.currAnswer[0], yawBase.position.y, GCP02.currAnswer[2]).normalized))
            {

            }

        }
        else
        {
            animate = false;
        }

        if(!animate && inTrigger)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {

            }
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

    public void SetCannonText(int[] vector)
    {
        if (vector[0] == 0 && vector[2] == 0)
            text.text = "??\n??";
        else
            text.text = vector[0].ToString() + "\n" + vector[2].ToString();
    }

    public void FireCannon(Vector3 target, bool willHit)
    {
        //Vector3 direction = ConvertToVector(target);
        //Vector3 unitDirection = direction.normalized;
        //float initMag = direction.magnitude;
        balltarget = target;
        inTrigger = false;
        GCP02.isMainCannonTrigger = false;
        canFire = false;
        ToggleTriggerBox(false);

        GameObject temp = new GameObject();
        temp.transform.position = mainBall.transform.position;

        tempBall = Instantiate(mainBall);
        tempBall.transform.position = mainBall.transform.position;
        tempBall.SetActive(true);

        tempBall.transform.parent = temp.transform;

        SetCannonText(new int[3] { (int)target.x, 0, (int)target.z });

        if (willHit)
            GameRoot.moveEvents.AddListener(CannonBallHit);
        else
            GameRoot.moveEvents.AddListener(CannonBallMiss);
        tempBall.GetComponent<ObjectGlide>().SetEventSys(GameRoot.moveEvents);
        tempBall.GetComponent<ObjectGlide>().GlideOnly(temp, target, tempBall.transform.position);

        GCP02.chaseCamera.transform.position = GCP02.mainCamera.transform.position;
        GCP02.chaseCamera.transform.rotation = GCP02.mainCamera.transform.rotation;

        GCP02.mainCamera.GetComponent<Camera>().enabled = false;
        GCP02.chaseCamera.SetActive(true);
        GCP02.chaseCamera.GetComponent<Camera>().enabled = true;

        GameRoot.camEvents.AddListener(EndCamChase);
        GCP02.chaseCamera.GetComponent<ObjectGlide>().SetEventSys(GameRoot.camEvents);
        GCP02.chaseCamera.GetComponent<ObjectGlide>().SetObjectMoveSpeed(0.2f);
        GCP02.chaseCamera.GetComponent<ObjectGlide>().GlideAndLookMovingTarget(GCP02.chaseCamera, (target - target.normalized *20) + Vector3.up*15, temp, GCP02.mainCamera.transform.position, -5f);


    
    }

    public void EndCamChase()
    {
        GCP02.mainCamera.GetComponent<Camera>().enabled = true;
        GCP02.chaseCamera.GetComponent<Camera>().enabled = false;
        GCP02.chaseCamera.SetActive(false);

        cannonFuzeFX.SetActive(false);
        ToggleTriggerBox(false);
        mainBarrel.SetActive(false);
    }

    public void CannonBallHit()
    {
        GameRoot.moveEvents.RemoveListener(CannonBallHit);
        GCP02.GetCurrTarget().explosion.transform.position = tempBall.transform.position;
        GCP02.GetCurrTarget().explosion.SetActive(true);
        GCP02.GetCurrTarget().explosion.GetComponent<ParticleSystem>().Play();
        GCP02.GetCurrTarget().fires.SetActive(true);

        GCP02.SinkShip();

        canFire = true;
        Destroy(tempBall);
        //play sound;
        //Sink ship
    }

    public void CannonBallMiss()
    {
        GameRoot.moveEvents.RemoveListener(CannonBallMiss);

        Vector3 temp = balltarget.normalized;
        temp *= 6.0f;
        temp = new Vector3(temp.x, -2.0f, temp.z);
        GameRoot.moveEvents.AddListener(CannonBallSplash);
        tempBall.GetComponent<ObjectGlide>().GlideOnly(tempBall.transform.parent.gameObject, balltarget + temp, tempBall.transform.position, 0.5f);
    }

    public void CannonBallSplash()
    {
        GameRoot.moveEvents.RemoveListener(CannonBallSplash);
        splashFX.transform.position = tempBall.transform.position;
        splashFX.SetActive(true);
        splashFX.GetComponent<ParticleSystem>().Play();

        canFire = true;
        Destroy(tempBall);
        //play sound;
    }

    public void ToggleTriggerBox(bool val)
    {
        cannonGlowFX.SetActive(val);
        triggerBox.enabled = val;
    }

    private Vector3 ConvertToVector(int[] target)
    {
        Vector3 temp = new Vector3();

        temp.x = target[0];
        temp.y = target[1];
        temp.z = target[2];

        return temp;
    }
}
