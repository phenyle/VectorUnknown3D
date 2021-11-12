using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioService : MonoBehaviour
{
    public AudioSource audioBg;
    public AudioSource audioUI;
    public AudioSource audioFX;
    public AudioSource audioPlayerMove;
    public AudioSource gulls;
    public AudioSource ocean;
    public float bgVolume;
    public float UIFXVolume;

    public void InitService()
    {
        Debug.Log("Init Audio Service");
        bgVolume = 0.25f;
        UIFXVolume = 1f;
    }

    public void PlayBgMusic(string audioName, bool isLoop)
    {
        //Get the audio clip by the resource service
        AudioClip audioClip = GameRoot.instance.resourceService.LoadAudio("Audios/Musics/" + audioName, true);

        //Check if the background audio clip whether is null or next audio clip name matches the current audio clip
        if (audioBg.clip == null || audioBg.clip.name != audioClip.name)
        {
            audioBg.clip = audioClip;
            audioBg.loop = isLoop;
            audioBg.Play();
            if(GameRoot.isPause)
                audioBg.volume = bgVolume * 0.5f * 0.2f;
            else
                audioBg.volume = bgVolume * 0.5f;
        }

        if(SceneManager.GetActiveScene().name == Constants.mainSceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle01SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle04_1SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle04_2SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle04_3SceneName ||
            SceneManager.GetActiveScene().name == Constants.tutorialSceneName)
        {
            gulls.loop = isLoop;
            gulls.Play();
        }
        else
        {
            gulls.Stop();
        }
        if(GameRoot.isPause)
                gulls.volume = bgVolume * 0.3f * 0.2f;
            else
                gulls.volume = bgVolume * 0.3f;

        if(SceneManager.GetActiveScene().name == Constants.mainSceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle01SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle02_1SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle02_2SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle04_1SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle04_2SceneName ||
            SceneManager.GetActiveScene().name == Constants.puzzle04_3SceneName ||
            SceneManager.GetActiveScene().name == Constants.tutorialSceneName)
        {
            ocean.loop = isLoop;
            ocean.Play();
        }
        else
        {
            ocean.Stop();
        }
        if(GameRoot.isPause)
                ocean.volume = bgVolume * 0.2f;
            else
                ocean.volume = bgVolume;
    }

    public void PlayUIAudio(string audioName)
    {
        //Get the audio clip by the resource service
        AudioClip audioClip = GameRoot.instance.resourceService.LoadAudio("Audios/Sound FX/" + audioName, true);
        audioUI.clip = audioClip;
        audioUI.Play();
    }

    public void PlayFXAudio(string audioName)
    {
        //Get the audio clip by the resource service
        AudioClip audioClip = GameRoot.instance.resourceService.LoadAudio("Audios/Sound FX/" + audioName, true);
        audioFX.clip = audioClip;
        audioFX.Play();
    }

    public AudioClip GetFXAudioClip(string audioName)
    {
        return GameRoot.instance.resourceService.LoadAudio("Audios/Sound FX/" + audioName, false);
    }

    public void PauseAllAudios()
    {
        audioBg.volume *= 0.2f;
        gulls.volume *= 0.2f;
        ocean.volume *= 0.2f;
    }

    public void ResumeAllAudios()
    {
        audioBg.volume *= 5f;
        gulls.volume *= 5f;
        ocean.volume *= 5f;
    }

    public void SetBgVolume(float volume)
    {
        bgVolume = volume;
        if(GameRoot.isPause)
        {
            audioBg.volume = bgVolume * 0.5f * 0.2f;
            if(SceneManager.GetActiveScene().name == Constants.mainSceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle01SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_1SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_2SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_3SceneName ||
                SceneManager.GetActiveScene().name == Constants.tutorialSceneName)
            {
                gulls.volume = bgVolume * 0.3f * 0.2f;
            }
            if(SceneManager.GetActiveScene().name == Constants.mainSceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle01SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle02_1SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle02_2SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_1SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_2SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_3SceneName ||
                SceneManager.GetActiveScene().name == Constants.tutorialSceneName)
            {
                ocean.volume = bgVolume * 0.2f;
            }
        }
        else
        {
            audioBg.volume = bgVolume * 0.5f;
            if(SceneManager.GetActiveScene().name == Constants.mainSceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle01SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_1SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_2SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_3SceneName ||
                SceneManager.GetActiveScene().name == Constants.tutorialSceneName)
            {
                gulls.volume = bgVolume * 0.3f;
            }
            if(SceneManager.GetActiveScene().name == Constants.mainSceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle01SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle02_1SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle02_2SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_1SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_2SceneName ||
                SceneManager.GetActiveScene().name == Constants.puzzle04_3SceneName ||
                SceneManager.GetActiveScene().name == Constants.tutorialSceneName)
            {
                ocean.volume = bgVolume;
            }
        }
    }

    public void SetSoundFXVolume(float volume)
    {
        UIFXVolume = volume;
        audioUI.volume = UIFXVolume;
        audioFX.volume = UIFXVolume;
    }
}
