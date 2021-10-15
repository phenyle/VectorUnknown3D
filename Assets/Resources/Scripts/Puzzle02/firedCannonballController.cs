using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firedCannonballController : MonoBehaviour
{
    public GameObject waterEffect;
    private GameObject tempEffect;
    private float timer = 0;

    private GameControllerPuzzle02 GCP02;
    private bool isEffectPlayed = false;
    private AudioSource audioSource;

    void Start()
    {
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        if (this.gameObject.transform.position.y <= -5.0f)
        {
            if (isEffectPlayed == false)
            {
                tempEffect = Instantiate(waterEffect, this.gameObject.transform.position, Quaternion.Euler(-90, 0, 0));
                tempEffect.GetComponent<ParticleSystem>().Play(true);
                isEffectPlayed = true;
            }

            timer = timer + 1;
            Debug.Log(timer);

            if(timer == 100)
                GCP02.fireCannon = false;

            if (timer > 450)
            {
                //GCP02.playMiss();
                // audioSource.clip = GameRoot.instance.audioService.GetFXAudioClip(Constants.audioP02BallMiss);
                // audioSource.Play();

                Destroy(tempEffect);
                Destroy(this.gameObject);
            }

        }
    }
}
