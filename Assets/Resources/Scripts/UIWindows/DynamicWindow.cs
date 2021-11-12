using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicWindow : WindowRoot
{
    public Animation aniTips;
    public TextMeshProUGUI txtTip;
    public TextMeshProUGUI txtTips;
    public TextMeshProUGUI txtTipsShow;

    private bool isTipsShow = false;
    private Queue<String> QueueTips = new Queue<String>();

    protected override void InitWindow()
    {
        base.InitWindow();
        SetActive(txtTips.gameObject, false);
        SetActive(txtTip.gameObject, false);
        SetActive(txtTipsShow.gameObject, false);
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
        SetActive(txtTips.gameObject, true);
        SetText(txtTips, tips);

        //Get the animation for later use and play the animation
        AnimationClip clip = aniTips.GetClip("TipsShow");
        aniTips.Play();

        //Track the animation time, when it finishs, close the tip gameobject
        StartCoroutine(AniPlayDone(clip.length, () =>
        {
            SetActive(txtTips.gameObject, false);
            SetActive(txtTip.gameObject, false);
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
        txtTips.color = new Vector4(246, 197, 97, 255) / 255;
        txtTip.color = new Vector4(246, 197, 97, 255) / 255;


        if (isReactive)
        {
            SetActive(txtTipsShow.gameObject, false);
            SetActive(txtTipsShow.gameObject, true);
        }

        SetActive(txtTipsShow.gameObject, state);
        SetText(txtTipsShow, tips);
    }

    public void ShowTips(string tips, Color color, bool state, bool isReactive)
    {
        txtTips.color = color;
        txtTip.color = color;

        if (isReactive)
        {
            SetActive(txtTipsShow.gameObject, false);
            SetActive(txtTipsShow.gameObject, true);
        }

        SetActive(txtTipsShow.gameObject, state);
        SetText(txtTipsShow, tips);
    }

}
