using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow : LoginSystem
{
    [Header("Awaiting PopUp")]
    public AwaitingServerPopup AwaitingPopUp;
    public Sprite bunnyBowl;
    public Sprite pirateShip;


    [Header("General Message PopUp")]
    public GameObject MessagePopUp;
    public Text messageText;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void AwaitPopUp(bool bunny)
    {
        AwaitingPopUp.gameObject.SetActive(true);
        AwaitingPopUp.AwaitingPopUp(bunny);
    }


    public void GeneralPopUp(string text)
    {
        messageText.text = text;
        MessagePopUp.SetActive(true);
    }

    public void General_BackBtn()
    {
        MessagePopUp.SetActive(false);
        messageEvents.Invoke();
    }
}
