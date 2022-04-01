using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMLibrary : MonoBehaviour
{
    public List<AudioClip> bgClips;

    public static List<AudioClip> bgMusic;
    // Start is called before the first frame update
    void Start()
    {
        bgMusic = bgClips;
    }

}
