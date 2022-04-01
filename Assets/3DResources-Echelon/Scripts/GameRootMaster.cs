using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsForUnity;

public class GameRootMaster : MonoBehaviour
{
    public static GameRootMaster masterInstance;
    public static PlayerData player;
    public static SendToGoogle GSFU;

    public static ResourceService resourceService { get; private set; }
    public LoadingWindow loadingWindow;


    public static bool isExpo = false;
    public static bool isClass = false;
    public static string expoName = "";

    public void Awake()
    {
        //GameObject[] masterRoots = GameObject.FindGameObjectsWithTag("GameRootMaster");
        //if (masterRoots.Length > 1)
        //    for (int i = 1; i < masterRoots.Length; i++)
        //        Destroy(masterRoots[i]);
        masterInstance = this;
        DontDestroyOnLoad(this);

        GSFU = this.GetComponent<SendToGoogle>();
        player = this.GetComponent<PlayerData>();
    }

    // Start is called before the first frame update
    void Start()
    {


        resourceService = GetComponent<ResourceService>();
        resourceService.InitService();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
