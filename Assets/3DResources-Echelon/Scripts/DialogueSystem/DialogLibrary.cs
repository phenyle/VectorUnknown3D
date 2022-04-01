using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogLibrary : MonoBehaviour
{
    public List<Conversation> intro;
    public List<Conversation> main;
    public List<Conversation> p01;
    public List<Conversation> p02_1;
    public List<Conversation> p02_2;
    public List<Conversation> p02_3;
    public List<Conversation> p03_1;
    public List<Conversation> p03_2;
    public List<Conversation> p03_3;
    public List<Conversation> p04_1;
    public List<Conversation> p04_2;
    public List<Conversation> p04_3;

    private List<List<Conversation>> mainDialogs;
    private List<List<Conversation>> p01Dialogs;
    private List<List<Conversation>> p02Dialogs;
    private List<List<Conversation>> p03Dialogs;
    private List<List<Conversation>> p04Dialogs;
    public static List<List<List<Conversation>>> conversations;

    public void Start()
    {
        mainDialogs = new List<List<Conversation>>();
        mainDialogs.Add(intro);
        mainDialogs.Add(main);
        p01Dialogs = new List<List<Conversation>>();
        p01Dialogs.Add(p01);
        p01Dialogs.Add(p01);
        p02Dialogs = new List<List<Conversation>>();
        p02Dialogs.Add(p02_1);
        p02Dialogs.Add(p02_1);
        p02Dialogs.Add(p02_2);
        p02Dialogs.Add(p02_3);
        p03Dialogs = new List<List<Conversation>>();
        p03Dialogs.Add(p03_1);
        p03Dialogs.Add(p03_1);
        p03Dialogs.Add(p03_2);
        p03Dialogs.Add(p03_3);
        p04Dialogs = new List<List<Conversation>>();
        p04Dialogs.Add(p04_1);
        p04Dialogs.Add(p04_1);
        p04Dialogs.Add(p04_2);
        p04Dialogs.Add(p04_3);

        conversations = new List<List<List<Conversation>>>();
        conversations.Add(mainDialogs);
        conversations.Add(p01Dialogs);
        conversations.Add(p02Dialogs);
        conversations.Add(p03Dialogs);
        conversations.Add(p04Dialogs);
    }
}
