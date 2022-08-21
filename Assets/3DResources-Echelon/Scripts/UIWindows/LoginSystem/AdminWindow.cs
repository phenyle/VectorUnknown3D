using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AdminWindow : LoginSystem
{
    public GameObject panel;
    public Button btn_PlayCreate;

    [Header("------- Panel Toggles ---------")]
    public Toggle tgl_create;
    public Toggle tgl_rules;
    public Transform pnl_create, pnl_rules;

    [Header("------- Create Controls ---------")]
    public ToggleGroup tglgroup_Session;
    public Toggle tgl_class, tgl_Expo;

    public ToggleGroup tglgroup_GameType;
    public Toggle tgl_2D, tgl_3D;

    public TMP_InputField input_SessionID;

    public Toggle tgl_expire;
    public TMP_Dropdown drop_expireDay, drop_expireMonth, drop_expireYear;

    [Header("------- Rules Controls ---------")]
    public TMP_Dropdown drop_restrictStage;
    public TMP_Dropdown drop_restrictDiff;


    private bool tf_create, tf_sess, tf_game, tf_name, tf_expire;
    private bool tf_rules, tf_stage;


    // Start is called before the first frame update
    void Start()
    {
        adminWindow = this;
        panel.SetActive(false);


        InitExpire();
        OnToggleCreateSession();
        OnToggleSessionType();
        OnToggleGameType();
        OnToggleExpire();
        OnInputSessionName();
        OnToggleRules();
        OnStageSelect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Click_PlayCreate()
    {
        if (tgl_create.isOn)
        {
            btn_PlayCreate.interactable = false;
            PlayerData temp = new PlayerData();
            temp.SetUserName(input_SessionID.text.ToLower());
            temp.users.sessionID = input_SessionID.text.ToLower();

            temp.users.isClass = tgl_class.isOn;
            temp.users.isExpo = tgl_Expo.isOn;

            if (tgl_expire.isOn)
            {
                DateTime expire = new DateTime(int.Parse(drop_expireYear.options[drop_expireYear.value].text), drop_expireMonth.value + 1, drop_expireDay.value + 1);
                temp.users.expires = expire.ToString();
            }
            else
            {
                DateTime expire = new DateTime(int.Parse(drop_expireYear.options[drop_expireYear.value].text) + 100, drop_expireMonth.value + 1, drop_expireDay.value + 1);
                temp.users.expires = expire.ToString();
            }

            if (drop_restrictStage.value != 0)
            {
                temp.users.forceStage = true;
                temp.users.stageNum = drop_restrictStage.value;
                temp.users.difficultyNum = drop_restrictDiff.value + 1;
            }
            else
            {
                temp.users.forceStage = false;
                temp.users.stageNum = 0;
                temp.users.difficultyNum = 0;
            }

            if (tgl_3D.isOn)
            {
                temp.users.isBunny = false;
                GameRootMaster.player = temp;
                GameRootMaster.GSFU.SaveNewPlayer();
                PlayEchelon();
            }
            if (tgl_2D.isOn)
            {
                temp.users.isBunny = true;
                GameRootMaster.player = temp;
                GameRootMaster.GSFU.SaveNewPlayer();
                PlayBunny();
            }
        }
        else
        {
            btn_PlayCreate.interactable = false;
            if (query == queryType.echelon)
                PlayEchelon();
            else if (query == queryType.bunny)
                PlayBunny();
        }
    }

    private void ErrorLogin()
    {
        btn_PlayCreate.interactable = true;
    }

    public void OnToggleCreateSession()
    {
        if(tgl_create.isOn)
        {
            pnl_create.gameObject.SetActive(true);
            tgl_rules.transform.parent.gameObject.SetActive(true);
            tgl_rules.gameObject.SetActive(true);
            tgl_class.isOn = false;
            tgl_Expo.isOn = false;
            tf_create = true;
        }
        else
        {
            pnl_create.gameObject.SetActive(false);
            tgl_rules.transform.parent.gameObject.SetActive(false);
            tgl_rules.gameObject.SetActive(false);
            btn_PlayCreate.interactable = true;
            btn_PlayCreate.GetComponentInChildren<TMP_Text>().text = "Play";
            tf_create = false;
        }
        CheckValidSession();
    }

    public void OnToggleSessionType()
    {
        if(tgl_class.isOn || tgl_Expo.isOn)
        {
            tgl_3D.interactable = true;
            tgl_2D.interactable = true;
            tgl_3D.isOn = false;
            tgl_2D.isOn = false;
            tf_sess = true;
        }
        else
        {
            tgl_3D.interactable = false;
            tgl_2D.interactable = false;
            tf_sess = false;
        }
        CheckValidSession();
    }

    public void OnToggleGameType()
    {
        if (tgl_2D.isOn || tgl_3D.isOn)
        {
            input_SessionID.interactable = true;
            tgl_expire.interactable = true;
            tf_game = true;
            ChangeRules();
        }
        else
        {
            input_SessionID.interactable = false;
            tgl_expire.interactable = false;
            tf_game = false;
        }
        CheckValidSession();
    }

    public void OnToggleExpire()
    {
        if(tgl_expire.isOn)
        {
            drop_expireDay.interactable = true;
            drop_expireMonth.interactable = true;
            drop_expireYear.interactable = true;
            tf_expire = ValidateExpire();
        }
        else
        {
            drop_expireDay.interactable = false;
            drop_expireMonth.interactable = false;
            drop_expireYear.interactable = false;
            tf_expire = true;
        }
        CheckValidSession();
    }

    public void OnInputSessionName()
    {
        if (input_SessionID.text.Length >= 1)
            tf_name = true;
        else
            tf_name = false;
        CheckValidSession();
    }

    public void OnToggleRules()
    {
        if (tgl_rules.isOn)
        {
            pnl_rules.gameObject.SetActive(true);
            tf_rules = true;

            ChangeRules();
        }
        else
        {
            pnl_rules.gameObject.SetActive(false);
            tf_rules = false;
        }
        CheckValidSession();
    }

    private void ChangeRules()
    {
        if (tgl_2D.isOn)
        {
            drop_restrictStage.ClearOptions();
            drop_restrictStage.AddOptions(new List<TMP_Dropdown.OptionData> {
                    new TMP_Dropdown.OptionData("none"),
                    new TMP_Dropdown.OptionData("Level 1"),
                    new TMP_Dropdown.OptionData("Level 2"),
                    new TMP_Dropdown.OptionData("Level 3"),
                    new TMP_Dropdown.OptionData("Level 4"),
                    new TMP_Dropdown.OptionData("Level 5"),
                    new TMP_Dropdown.OptionData("Level 6"),
                    new TMP_Dropdown.OptionData("Level 7")
            });

        }
        else
        {
            drop_restrictStage.ClearOptions();
            drop_restrictStage.AddOptions(new List<TMP_Dropdown.OptionData> {
                    new TMP_Dropdown.OptionData("none"),
                    new TMP_Dropdown.OptionData("Stage 1 : Easy Displace"),
                    new TMP_Dropdown.OptionData("Stage 2 : Matrix Mult"),
                    new TMP_Dropdown.OptionData("Stage 3 : Vector Spans"),
                    new TMP_Dropdown.OptionData("Stage 4 : Displacement") 
            });
        }
    }


    public void OnStageSelect()
    {
        if (drop_restrictStage.value != 0)
        { 
            drop_restrictDiff.interactable = true;
            tf_stage = true;
        }
        else
        {
            drop_restrictDiff.interactable = false;
            tf_stage = false;
        }
        CheckValidSession();
    }
 
    
    /// <summary>
    /// This is ugly as sin.  This is some YandereSim levels of nested Ifs and I feel bad.
    /// but this is just a hot fix to get this running by tomorrow.  Needs fixes later
    /// /// </summary>
    private void CheckValidSession()
    {
        btn_PlayCreate.interactable = false;
        if(tf_create)
        {
            btn_PlayCreate.GetComponentInChildren<TMP_Text>().text = "Create";
           if (tf_sess && tf_game && tf_name && tf_expire)
            {
                if (tf_rules)
                {
                    if(tf_stage)
                        btn_PlayCreate.interactable = true;
                }
                else
                    btn_PlayCreate.interactable = true;
            }

        }
        else
        {
            btn_PlayCreate.interactable = true;
            btn_PlayCreate.GetComponentInChildren<TMP_Text>().text = "Play";
        }
    }

    public void OnMonthChange()
    {
        int selDate = drop_expireDay.value;
        int numOfDays = DaysOfMonth(drop_expireMonth.value + 1);

        drop_expireDay.ClearOptions();
        for(int i = 1; i <= numOfDays; i++)
            drop_expireDay.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        Debug.Log("Days in Month: " + numOfDays + "\tDowndown Length: " + drop_expireDay.options.Count);

        if (selDate + 1 <= numOfDays)
            drop_expireDay.value = selDate;
        else
            drop_expireDay.value = numOfDays - 1;
        drop_expireDay.RefreshShownValue();
    }

    private bool ValidateExpire()
    {
        DateTime expire = new DateTime(int.Parse(drop_expireYear.options[drop_expireYear.value].text), drop_expireMonth.value + 1, drop_expireDay.value + 1);
        Debug.Log(expire);
        return DateTime.Compare(DateTime.Now, expire) < 0;
    }

    private void InitExpire()
    {
        drop_expireMonth.value = DateTime.Now.Month;
        OnMonthChange();
        int currYear = DateTime.Now.Year;
        drop_expireYear.ClearOptions();
        for(int i = currYear; i < currYear + 5; i++)
            drop_expireYear.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        drop_expireYear.value = 0;
    }

    private int DaysOfMonth(int month)
    { 
        switch(month)
        {
            case 1: return 31; 
            case 2: return 28; 
            case 3: return 31;
            case 4: return 30;
            case 5: return 31;
            case 6: return 30;
            case 7: return 31;
            case 8: return 31;
            case 9: return 30;
            case 10: return 31;
            case 11: return 30;
            case 12: return 31;
            default: return 31;
        }
    }
}
