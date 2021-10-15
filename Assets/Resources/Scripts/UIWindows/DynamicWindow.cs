using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWindow : WindowRoot
{
    public Animation aniTips;
    public Text txtTips;
    public Text txtTipsShow;

    private bool isTipsShow = false;
    private Queue<String> QueueTips = new Queue<String>();

    protected override void InitWindow()
    {
        base.InitWindow();
        SetActive(txtTips, false);
        SetActive(txtTipsShow, false);
    }

    private void Update()
    {
        if(QueueTips.Count > 0 && !isTipsShow)
        {
            //Lock for multithreading
            lock(QueueTips)
            {
                isTipsShow = true;
                SetTips(QueueTips.Dequeue());
            }
        }
    }

    public void AddTips(string tips)
    {
        lock(QueueTips)
        {
            QueueTips.Enqueue(tips);
        }
    }

    private void SetTips(string tips)
    {
        //Active the tip gameobject and set the text
        SetActive(txtTips, true);
        SetText(txtTips, tips);

        //Get the animation for later use and play the animation
        AnimationClip clip = aniTips.GetClip("TipsShow");
        aniTips.Play();

        //Track the animation time, when it finishs, close the tip gameobject
        StartCoroutine(AniPlayDone(clip.length, () =>
        {
            SetActive(txtTips, false);
            isTipsShow = false;
        }));
    }

    private IEnumerator AniPlayDone(float sec, Action cb)
    {
        yield return new WaitForSeconds(sec);

        if (cb != null)
        {
            cb();
        }
    }

    public void ShowTips(string tips, bool state, bool isReactive)
    {
        if(isReactive)
        {
            SetActive(txtTipsShow, false);
            SetActive(txtTipsShow, true);
        }

        SetActive(txtTipsShow, state);
        SetText(txtTipsShow, tips);
    }
}
