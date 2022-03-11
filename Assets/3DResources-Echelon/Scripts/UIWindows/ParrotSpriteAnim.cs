using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// You might be asking yourself: why does this script exist? Isn't this just a
/// sprite animation?
/// valid.... valid
/// And yes, it basically is.  But Unity and its "Animation" wasn't doing
/// the one thing it says it supposed to do, being "animating" a thing.
/// So, here's the hack.  For a stupid one time use sprite only seen when logging
/// into the game.  Because why would Unity ever just work the way it's supposed to?
/// </summary>
public class ParrotSpriteAnim : MonoBehaviour
{
    public GameObject[] frames;

    public float anim_speed;
    private int anim_iter;
    private float frame_time_elap;

    // Start is called before the first frame update
    void Start()
    {
        anim_iter = 0;
        frame_time_elap = 0;

        for (int i = 0; i < frames.Length; i++)
        {
            frames[i].SetActive(false);
        }
    }

    void Update()
    {
        //Bird wing animation frames
        frame_time_elap += Time.deltaTime;

        if (frame_time_elap > anim_speed)
        {
            frame_time_elap = 0;
            frames[anim_iter].SetActive(false);
            anim_iter++;
            if (anim_iter > frames.Length - 1)
                anim_iter = 0;
            frames[anim_iter].SetActive(true);
        }        
    }

    public void ResetAnim()
    {
        anim_iter = 0;
        frame_time_elap = 0;

        for (int i = 0; i < frames.Length; i++)
        {
            frames[i].SetActive(false);
            frames[i].transform.localScale = new Vector3(1, 1, 1);
        }
    }
}