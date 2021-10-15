using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shipContoller : MonoBehaviour
{
    public bool isHit = false;
    public bool isplayed = false;
    public GameObject text1;
    public GameObject text2;
    public GameObject effect;

    private GameControllerPuzzle02 GCP02;
    private GameObject tempEffect;
    private AudioSource audioSource;

    void Start()
    {
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
        audioSource = GetComponent<AudioSource>();

        isHit = false;
    }
    
    void Update()
    {
        if (isHit)
        {
            if (isplayed == false) {
                tempEffect = Instantiate(effect, this.gameObject.transform.position, this.gameObject.transform.rotation);
                tempEffect.GetComponent<ParticleSystem>().Play(true);
                isplayed = !isplayed;
                GCP02.normalText[GCP02.ActiveBoat].SetActive(false);
                GCP02.UpdateUI();
                GCP02.ActiveBoat += 1;
                print("Active boat: " + GCP02.ActiveBoat); 
            }
            

            if (transform.position.y > -30)
            {
                transform.position += Vector3.down * 1.0f * Time.deltaTime;
            }

            // if (transform.position.y < -30)
            // {
            //     Destroy(gameObject);
            // }

            int tempActiveBoat = GCP02.ActiveBoat;

            if(tempActiveBoat > 5)
            {
                tempActiveBoat = 5;
            }

            if (GCP02.topViewOn)
            {
                for(int i = tempActiveBoat; i >=0; i--)
                {
                    GCP02.TopViewText[i].gameObject.SetActive(true);
                }
                GCP02.normalText[tempActiveBoat].gameObject.SetActive(false);
            }
            else if (tempActiveBoat < 6)
            {
                for(int i = tempActiveBoat; i >=0; i--)
                {
                    GCP02.TopViewText[i].gameObject.SetActive(false);
                }
                GCP02.normalText[tempActiveBoat].gameObject.SetActive(true);
            }
        }
        
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("collision tiggered");
       
        isHit = true;
        Destroy(other.gameObject);
        GCP02.ballIsFlying = false;
        Debug.Log("Boat was hit");

        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);

        //GCP02.playhit();
        audioSource.clip = GameRoot.instance.audioService.GetFXAudioClip(Constants.audioP02BallHit);
        audioSource.Play();

        GCP02.fireCannon = false;
        if(GCP02.cameraFollow)
        {
            GCP02.CannonCamera.SetActive(false);
            GCP02.MainCamera.SetActive(true); 
            GCP02.cameraFollow = false;
        } 

        StartCoroutine(resetCannonText(5));

    }

    // sets cannon text back to selected vector after sinking ship
    public IEnumerator resetCannonText(float sec) 
    {
        yield return new WaitForSeconds(sec);
        GCP02.maincannonText.gameObject.GetComponent<Text>().text = GCP02.selectedVector[0] + "\n" + GCP02.selectedVector[1];
    }
}

   




