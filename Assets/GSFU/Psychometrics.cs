using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class Psychometrics : MonoBehaviour
{
    private static ArrayList metrics = new ArrayList();
    public static List<string> steps = new List<string>();
    private static string sessionID = "";
    private static string temp = "";
    public static string id = "";

    // Use this for initialization
    void Start()
    {
        if (sessionID.Equals(""))
        {
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            sessionID = (month + "-" + day + "-" + year + "-" + hour + "-" + minute + "-" + second);
            logEvent(sessionID);
            GameRootMaster.player.bunny.attemptDate = year + "/" + month + "/" + day + "(" + hour + ":" + minute + ":" + second + ")";
        }
    }

    public static void attempt(string str)
    {
        temp = str;
    }

    public static void report(string str)
    {
        logEvent(str + " " + temp);
        temp = "";
    }

    public static void logEvent(string str)
    {
        metrics.Add(str);
    }

    public static void logDatabaseEvent(string step, string result)
    {
        steps.Add(step);
        GameRootMaster.player.bunny.steps += step + "\n";
        GameRootMaster.player.bunny.results += result + "\n";
        GameRootMaster.player.bunny.time = scorekeeper.GetGameTime().ToString("0.00");
    }


    public static void sendData()
    {
        GameRootMaster.player.bunny.username = GameRootMaster.player.users.username;
        GameRootMaster.player.bunny.attemptDate = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + 
                                                "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ")";

        GameRootMaster.GSFU.UpdatePlayerTable(true, PlayerData.sheets.bunny, GameRootMaster.player.bunny.usernameAttempt);
        while (metrics.Count > 255)
        {
            metrics.RemoveAt(metrics.Count - 1);
        }
        UnityDataConnector.instance.SendDataToSheet((string[])metrics.ToArray(typeof(string)));
        metrics = new ArrayList();
        logEvent(sessionID);
        logEvent(id);
    }
    
}
