using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsForUnity;

public class GameRootMaster : MonoBehaviour
{
    public static GameRootMaster masterInstance;
    public static PlayerData player;
    public static SendToGoogle GSFU;


    public void Awake()
    {
        masterInstance = this;
        DontDestroyOnLoad(this);

        GSFU = this.GetComponent<SendToGoogle>();
        player = this.GetComponent<PlayerData>();
    }

    // Start is called before the first frame update
    void Start()
    {
    //    GameRoot3D.instance.GameMasterInit();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
