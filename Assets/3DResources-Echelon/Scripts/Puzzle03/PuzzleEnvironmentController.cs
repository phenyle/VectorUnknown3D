using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEnvironmentController : MonoBehaviour
{

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = GameRoot.instance.audioService.GetFXAudioClip(Constants.audioP03RotatedPuzzleEnvironment);

    }

    public void PlayRotatedSoundFX(bool isPlay)
    {
        if(isPlay)
        {
            if(audioSource.clip == null)
            {
                audioSource.clip = GameRoot.instance.audioService.GetFXAudioClip(Constants.audioP03RotatedPuzzleEnvironment);
            }

            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
