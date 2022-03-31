using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsForUnity;


public class DualLogin : LoginSystem
{
    public InputField username;
    public InputField password;
    private bool passhide = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click_BunnyGame()
    {
        checkingGFSU = true;
        connectionTime = 0.0f;
        SendToGoogle.callback.AddListener(ConnectionsCheck);

    }

    public void Click_EchelonSeas()
    {
        checkingGFSU = true;
        connectionTime = 0.0f;
        SendToGoogle.callback.AddListener(ConnectionsCheck);

    }

    public void Click_PeekPassword()
    {
        passhide = !passhide;

        if (passhide)
            password.contentType = InputField.ContentType.Password;
        else
            password.contentType = InputField.ContentType.Standard;

        password.ForceLabelUpdate();
    }

    public void Click_JoinSession()
    {
    }

    public void Click_CreateAccount()
    {

    }
}
