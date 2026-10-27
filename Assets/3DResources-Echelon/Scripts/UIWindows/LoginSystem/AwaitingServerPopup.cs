using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AwaitingServerPopup : PopUpWindow
{
    [Header("Objects")]
    public GameObject panel;
    public GameObject backNForthImage;
    public GameObject computer;
    public GameObject goal;
    public Text statusText;
    public Text flavorText;
    public Sprite pirateShip;
    public Sprite carrotBowl;
    public GameObject parrotObj;
    public GameObject bunnyObj;
    private string currStatus;

    [Header("Text Waiting Periods")]
    [Range(0.01f, 1f)]
    public float statusSpeed;
    [Range(1f, 10f)]
    public float flavorSpeed;
    [Range(1, 5)]
    public int statusDotsLength;


    [Header("Parrot Animation")]
    [Range(0.01f, 2f)]
    public float flightSpeed;
    public float flightHeight;
    private Vector3 halfway;

    private bool computer2goal;
    private int numDots;
    private int flavor_iter;
    private float timeSec_status;
    private float timeSec_flavor;

    private string[] flavorScripts = { "Did you know: You can check your username in the pause menu?  \nPress \"Esc\" in game and your logged in user name will be in the top left corner",
                                        "You can repeat a puzzle as much as you like, but only your best time is saved",
                                        "Feel like challenging yourself?  Try a puzzle at a higher difficulty.\nSome puzzles at advanced difficulties have larger/complex numbers and some add entire new dimensions"};

    
    // Start is called before the first frame update
    void Start()
    {
        numDots = 0;
        computer2goal = true;
        flavor_iter = UnityEngine.Random.Range(0, flavorScripts.Length - 1);
        flavorText.text = flavorScripts[flavor_iter];
        timeSec_status = 0.0f;
        timeSec_flavor = 0.0f;
        halfway = (goal.transform.position - computer.transform.position) / 2;
    }

    void Update()
    {
        timeSec_status += Time.deltaTime;
        timeSec_flavor += Time.deltaTime;

        if (timeSec_status > statusSpeed)
        {
            timeSec_status = 0.0f;
            StatusWaitingDots();
        }

        if (timeSec_flavor > flavorSpeed)
        {
            timeSec_flavor = 0.0f;
            ChangeFlavor();
        }

        //Flight back and forth from computer to goal
        if (computer2goal)
        {
            //parrot to ship
            backNForthImage.transform.position = Vector3.MoveTowards(backNForthImage.transform.position, goal.transform.position, flightSpeed);

            if ((backNForthImage.transform.position - goal.transform.position).magnitude < 1)
            {
                computer2goal = false;
                backNForthImage.transform.localScale = new Vector3(backNForthImage.transform.localScale.x * -1, backNForthImage.transform.localScale.y, 1);
            }
        }
        else
        {
            //parrot to computer
            backNForthImage.transform.position = Vector3.MoveTowards(backNForthImage.transform.position, computer.transform.position, flightSpeed);

            if ((backNForthImage.transform.position - computer.transform.position).magnitude < 1)
            {
                computer2goal = true;
                backNForthImage.transform.localScale = new Vector3(backNForthImage.transform.localScale.x * -1, backNForthImage.transform.localScale.y, 1);
            }
        }
    }

    public void SetStatus(string status)
    {
        numDots = 0;
        currStatus = status;
        StatusWaitingDots();
    }

    private void StatusWaitingDots()
    {
        string dots = new string('.', numDots);

        statusText.text = dots + currStatus + dots;

        numDots++;
        if (numDots > statusDotsLength)
            numDots = 0;
    }

    private void ChangeFlavor()
    {
        int temp = flavor_iter;
        do
        {
            flavor_iter = UnityEngine.Random.Range(0, flavorScripts.Length - 1);
        } while (flavor_iter == temp);

        flavorText.text = flavorScripts[flavor_iter];
    }

    public void AwaitingPopUp(bool bunny)
    {
        panel.SetActive(true);
        Start();
        backNForthImage.transform.position = computer.transform.position;
        backNForthImage.transform.localScale = new Vector3(Math.Abs(backNForthImage.transform.localScale.x), Math.Abs(backNForthImage.transform.localScale.y), 1);

        if (bunny)
        {
            backNForthImage = bunnyObj;
            backNForthImage.SetActive(true);
            parrotObj.SetActive(false);
            goal.GetComponent<Image>().sprite = carrotBowl;
        }
        else
        {
            backNForthImage.GetComponent<ParrotSpriteAnim>().ResetAnim();
            backNForthImage = parrotObj;
            backNForthImage.SetActive(true);
            bunnyObj.SetActive(false);
            goal.GetComponent<Image>().sprite = pirateShip;
        }
    }
}
