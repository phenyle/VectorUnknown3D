using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class PopUpWindow : LoginSystem
{
    [Header("Awaiting PopUp")]
    [ReadOnly] public AwaitingServerPopup awaitServerPop;


    [Header("General Message PopUp")]
    public GameObject generalPopPanel;
    private Text generalText;
    private Button btn_okay;

    public void Init()
    {
        awaitServerPop = GetComponentInChildren<AwaitingServerPopup>();
        generalPopPanel = transform.Find("GeneralPopup").gameObject;
        generalText = generalPopPanel.transform.Find("message").GetComponent<Text>();
        btn_okay = generalPopPanel.transform.Find("Okay_button").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        popUpsWindow = this;

        awaitServerPop = GetComponentInChildren<AwaitingServerPopup>();
        generalPopPanel = transform.Find("GeneralPopup").gameObject;
        generalText = generalPopPanel.transform.Find("message").GetComponent<Text>();
        btn_okay = generalPopPanel.transform.Find("Okay_button").GetComponent<Button>();


        awaitServerPop.SetStatus("");
        generalPopPanel.SetActive(false);
        awaitServerPop.panel.SetActive(false);
    }

    public void AwaitPopUp(bool bunny)
    {
        awaitServerPop.panel.SetActive(true);
        awaitServerPop.AwaitingPopUp(bunny);
    }


    public void GeneralPopUp(string text)
    {
        generalText.text = text;
        generalPopPanel.SetActive(true);
    }


    public void Click_GenOkayBtn()
    {
        messageEvents.Invoke();
        messageEvents.RemoveAllListeners();
        generalPopPanel.SetActive(false);        
    }
}
