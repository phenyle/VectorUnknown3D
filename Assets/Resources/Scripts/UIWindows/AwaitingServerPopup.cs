using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AwaitingServerPopup : WindowRoot
{
    [Header("Objects")]
    public GameObject parrot;
    public GameObject computer;
    public GameObject ship;
    public Text statusText;
    public Text flavorText;
    private string currStatus;

    [Header("Text Waiting Periods")]
    [Range(1, 20)]
    public int statusSpeed;
    [Range(1, 20)]
    public int statusDotsLength;
    [Range(2, 20)]
    public int flavorSpeed;

    [Header("Parrot Animation")]
    public float flightSpeed;
    public float flightHeight;
    private Vector3 halfway;

    private bool computer2ship;
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
        computer2ship = true;
        flavor_iter = UnityEngine.Random.Range(0, flavorScripts.Length - 1);
        flavorText.text = flavorScripts[flavor_iter];
        timeSec_status = Time.time;
        timeSec_flavor = Time.time;
        halfway = (ship.transform.position - computer.transform.position) / 2;
    }

    private void OnEnable()
    {
        timeSec_status = Time.time;
        timeSec_flavor = Time.time;
        parrot.GetComponent<ParrotSpriteAnim>().ResetAnim();
        parrot.transform.position = computer.transform.position;
    }


    void Update()
    {
        if (timeSec_status + statusSpeed < Time.time)
        {
            timeSec_status = Time.time;
            StatusWaitingDots();
        }

        if (timeSec_flavor + flavorSpeed < Time.time)
        {
            timeSec_flavor = Time.time;
            ChangeFlavor();
        }

        //Flight back and forth from computer to ship
        if (computer2ship)
        {
            //parrot to ship
            parrot.transform.position = Vector3.MoveTowards(parrot.transform.position, ship.transform.position, flightSpeed);

            if ((parrot.transform.position - ship.transform.position).magnitude < 1)
            {
                computer2ship = false;
                parrot.transform.localScale = new Vector3(parrot.transform.localScale.x * -1, parrot.transform.localScale.y, 1);
            }
        }
        else
        {
            //parrot to computer
            parrot.transform.position = Vector3.MoveTowards(parrot.transform.position, computer.transform.position, flightSpeed);

            if ((parrot.transform.position - computer.transform.position).magnitude < 1)
            {
                computer2ship = true;
                parrot.transform.localScale = new Vector3(parrot.transform.localScale.x * -1, parrot.transform.localScale.y, 1);
            }
        }
    }

    public void SetStatus(string status)
    {
        numDots = 0;
        currStatus = status;
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
}
