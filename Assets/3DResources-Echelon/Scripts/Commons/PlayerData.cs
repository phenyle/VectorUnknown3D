using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //This string array of sheet names MUST MATCH EXACTLY
    //to the sheet names on the Google Sheets spreadsheet
    public string[] sheetNames = { 
                    "users",
                    "records",
                    "bunRecords",
                    "bunny",
                    "p1-1", 
                    "p2-1", 
                    "p2-2",
                    "p2-3",
                    "p3-1",
                    "p3-2",
                    "p3-3",
                    "p4-1", 
                    "p4-2", 
                    "p4-3" };

    public enum sheets
    {
        users,
        records,
        bunRecords,
        bunny,
        p1_1,
        p2_1,
        p2_2,
        p2_3,
        p3_1,
        p3_2,
        p3_3,
        p4_1,
        p4_2,
        p4_3
    }


    public List<string> schools = new List<string>();

    // Each struct corresponds to a Worksheet on Google Sheets.
    // If you wish to add additional worksheets, the struct name
    // does not have to match the Worksheet name, but be sure to
    // add the EXACT name to the string array above.
    // IMPORTANT:
    // properties inside the struct must be named EXACTLY how they
    // appear as headers in Worksheet.  This means that  
    // properties/headers must follow C# naming syntax.
    // NO dashes, periods, special characters, etc.
    [System.Serializable]
    public struct tbl_Users
    {
        // Passwords and real names are never stored here or in the sheet: the game server checks
        // passwords itself and builds the player ID from the name (see Server/AppsScript/Code.gs).
        public string username;
        public string sessionID;
        public string education;
        public string school;
        public string last_login;
        public float tot_time;
        public bool admin;
        public bool isClass;
        public bool isExpo;
        public bool isBunny;
        public int stageNum;
        public int difficultyNum;
        public bool forceStage;
        public string expires;
        public float mouseSense;
        public float soundVol;
        public float soundFX;
    }

    [System.Serializable]
    public struct tbl_Records
    {
        public string username;
        public string sessionID;

        //stage 1
        public int p1_totAttempts;
        public float p1_totTime;
        public float p1_time;
        public int p1_numMove;
        public string p1_answers;

        //stage 2
        public int p2_totAttempts;
        public float p2_totTime;

        public int p2_1_attempts;
        public float p2_1_time;
        public float p2_1_score;
        public int p2_1_numMove;
        public string p2_1_questions;

        public int p2_2_attempts;
        public float p2_2_time;
        public float p2_2_score;
        public int p2_2_numMove;
        public string p2_2_questions;

        public int p2_3_attempts;
        public float p2_3_time;
        public float p2_3_score;
        public int p2_3_numMove;
        public string p2_3_questions;

        //stage 3
        public int p3_totAttempts;
        public float p3_totTime;

        public int p3_1_attempts;
        public float p3_1_time;
        public float p3_1_score;
        public int p3_1_numMove;
        public string p3_1_movesList;

        public int p3_2_attempts;
        public float p3_2_time;
        public float p3_2_score;
        public int p3_2_numMove;
        public string p3_2_movesList;

        public int p3_3_attempts;
        public float p3_3_time;
        public float p3_3_score;
        public int p3_3_numMove;
        public string p3_3_movesList;


        //stage 4
        public int p4_totAttempts;
        public float p4_totTime;

        public int p4_1_attempts;
        public float p4_1_time;
        public float p4_1_totScore;
        public float p4_1_avgScore;
        public int p4_1_numMove;
        public string p4_1_questions;

        public int p4_2_attempts;
        public float p4_2_time;
        public float p4_2_totScore;
        public float p4_2_avgScore;
        public int p4_2_numMove;
        public string p4_2_questions;

        public int p4_3_attempts;
        public float p4_3_time;
        public float p4_3_totScore;
        public float p4_3_avgScore;
        public int p4_3_numMove;
        public string p4_3_questions;
    }

    [System.Serializable]
    public struct tbl_bunRecords
    {
        public string username;
        public string sessionID;

        public int bun_00_attempts;

        public int bun_11_attempts;
        public int bun_21_attempts;
        public int bun_31_attempts;
        public int bun_41_attempts;
        public int bun_51_attempts;
        public int bun_61_attempts;
        public int bun_71_attempts;

        public int bun_12_attempts;
        public int bun_22_attempts;
        public int bun_32_attempts;
        public int bun_42_attempts;
        public int bun_52_attempts;
        public int bun_62_attempts;
        public int bun_72_attempts;

        public int bun_13_attempts;
        public int bun_23_attempts;
        public int bun_33_attempts;
        public int bun_43_attempts;
        public int bun_53_attempts;
        public int bun_63_attempts;
        public int bun_73_attempts;
    }

    [System.Serializable]
    public struct tbl_Bunny 
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;

        public int stage;
        public int level;

        public int idealSteps;
        public string answer;
        public string a;
        public string b;
        public string c;
        public string d;
        public string x;
        public string y;
        public string k;
        public string j;
        public string time;
        public string steps;
        public string results;

    }


    [System.Serializable]
    public struct tbl_P1_1
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;
        public string obs_answers1;
        public string obs_answers2;
        public string obs_answers3;
        public string obs_answers4;
    }
    [System.Serializable]
    public struct tbl_P2_1
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;

        public int tot_Ideal;
        public int tot_steps;
        public float tot_score;
        public float avg_score;

        public PuzzleController02.obsData obs1;
        public int obs1_Ideal;
        public int obs1_steps;
        public float obs1_time;
        public float obs1_score;
        public string obs1_attempts;
        public PuzzleController02.obsData obs2;
        public int obs2_Ideal;
        public int obs2_steps;
        public float obs2_time;
        public float obs2_score;
        public string obs2_attempts;
        public PuzzleController02.obsData obs3;
        public int obs3_Ideal;
        public int obs3_steps;
        public float obs3_time;
        public float obs3_score;
        public string obs3_attempts;
        public PuzzleController02.obsData obs4;
        public int obs4_Ideal;
        public int obs4_steps;
        public float obs4_time;
        public float obs4_score;
        public string obs4_attempts;
        public PuzzleController02.obsData obs5;
        public int obs5_Ideal;
        public int obs5_steps;
        public float obs5_time;
        public float obs5_score;
        public string obs5_attempts;
        public PuzzleController02.obsData obs6;
        public int obs6_Ideal;
        public int obs6_steps;
        public float obs6_time;
        public float obs6_score;
        public string obs6_attempts;
        public PuzzleController02.obsData obs7;
        public int obs7_Ideal;
        public int obs7_steps;
        public float obs7_time;
        public float obs7_score;
        public string obs7_attempts;
        public PuzzleController02.obsData obs8;
        public int obs8_Ideal;
        public int obs8_steps;
        public float obs8_time;
        public float obs8_score;
        public string obs8_attempts;
        public PuzzleController02.obsData obs9;
        public int obs9_Ideal;
        public int obs9_steps;
        public float obs9_time;
        public float obs9_score;
        public string obs9_attempts;
        public PuzzleController02.obsData obs10;
        public int obs10_Ideal;
        public int obs10_steps;
        public float obs10_time;
        public float obs10_score;
        public string obs10_attempts;

    }
    [System.Serializable]
    public struct tbl_P2_2
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;

        public int tot_Ideal;
        public int tot_steps;
        public float tot_score;
        public float avg_score;

        public PuzzleController02.obsData obs1;
        public int obs1_Ideal;
        public int obs1_steps;
        public float obs1_time;
        public float obs1_score;
        public string obs1_attempts;
        public PuzzleController02.obsData obs2;
        public int obs2_Ideal;
        public int obs2_steps;
        public float obs2_time;
        public float obs2_score;
        public string obs2_attempts;
        public PuzzleController02.obsData obs3;
        public int obs3_Ideal;
        public int obs3_steps;
        public float obs3_time;
        public float obs3_score;
        public string obs3_attempts;
        public PuzzleController02.obsData obs4;
        public int obs4_Ideal;
        public int obs4_steps;
        public float obs4_time;
        public float obs4_score;
        public string obs4_attempts;
        public PuzzleController02.obsData obs5;
        public int obs5_Ideal;
        public int obs5_steps;
        public float obs5_time;
        public float obs5_score;
        public string obs5_attempts;
        public PuzzleController02.obsData obs6;
        public int obs6_Ideal;
        public int obs6_steps;
        public float obs6_time;
        public float obs6_score;
        public string obs6_attempts;
        public PuzzleController02.obsData obs7;
        public int obs7_Ideal;
        public int obs7_steps;
        public float obs7_time;
        public float obs7_score;
        public string obs7_attempts;
        public PuzzleController02.obsData obs8;
        public int obs8_Ideal;
        public int obs8_steps;
        public float obs8_time;
        public float obs8_score;
        public string obs8_attempts;
        public PuzzleController02.obsData obs9;
        public int obs9_Ideal;
        public int obs9_steps;
        public float obs9_time;
        public float obs9_score;
        public string obs9_attempts;
        public PuzzleController02.obsData obs10;
        public int obs10_Ideal;
        public int obs10_steps;
        public float obs10_time;
        public float obs10_score;
        public string obs10_attempts;
    }
    [System.Serializable]
    public struct tbl_P2_3
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;

        public int tot_Ideal;
        public int tot_steps;
        public float tot_score;
        public float avg_score;

        public PuzzleController02.obsData obs1;
        public int obs1_Ideal;
        public int obs1_steps;
        public float obs1_time;
        public float obs1_score;
        public string obs1_attempts;
        public PuzzleController02.obsData obs2;
        public int obs2_Ideal;
        public int obs2_steps;
        public float obs2_time;
        public float obs2_score;
        public string obs2_attempts;
        public PuzzleController02.obsData obs3;
        public int obs3_Ideal;
        public int obs3_steps;
        public float obs3_time;
        public float obs3_score;
        public string obs3_attempts;
        public PuzzleController02.obsData obs4;
        public int obs4_Ideal;
        public int obs4_steps;
        public float obs4_time;
        public float obs4_score;
        public string obs4_attempts;
        public PuzzleController02.obsData obs5;
        public int obs5_Ideal;
        public int obs5_steps;
        public float obs5_time;
        public float obs5_score;
        public string obs5_attempts;
        public PuzzleController02.obsData obs6;
        public int obs6_Ideal;
        public int obs6_steps;
        public float obs6_time;
        public float obs6_score;
        public string obs6_attempts;
        public PuzzleController02.obsData obs7;
        public int obs7_Ideal;
        public int obs7_steps;
        public float obs7_time;
        public float obs7_score;
        public string obs7_attempts;
        public PuzzleController02.obsData obs8;
        public int obs8_Ideal;
        public int obs8_steps;
        public float obs8_time;
        public float obs8_score;
        public string obs8_attempts;
        public PuzzleController02.obsData obs9;
        public int obs9_Ideal;
        public int obs9_steps;
        public float obs9_time;
        public float obs9_score;
        public string obs9_attempts;
        public PuzzleController02.obsData obs10;
        public int obs10_Ideal;
        public int obs10_steps;
        public float obs10_time;
        public float obs10_score;
        public string obs10_attempts;
    }
    [System.Serializable]
    public struct tbl_P3_1
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;
        public GameControllerPuzzle03.obsData obs;
        public string obs_movesList;
    }
    [System.Serializable]
    public struct tbl_P3_2
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;
        public GameControllerPuzzle03.obsData obs;
        public string obs_movesList;
    }
    [System.Serializable]
    public struct tbl_P3_3
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;
        public GameControllerPuzzle03.obsData obs;
        public string obs_movesList;
    }
    [System.Serializable]
    public struct tbl_P4_1
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;

        public int tot_Ideal;
        public int tot_steps;
        public float tot_score;
        public float avg_score;

        public Puzzle04Controller.obsData obs1;
        public string obs1_ans;
        public string obs1_a;
        public string obs1_b;
        public string obs1_c;
        public string obs1_d;
        public int obs1_x;
        public int obs1_y;
        public int obs1_Ideal;
        public int obs1_steps;
        public float obs1_time;
        public string obs1_attempts;
        public Puzzle04Controller.obsData obs2;
        public string obs2_ans;
        public string obs2_a;
        public string obs2_b;
        public string obs2_c;
        public string obs2_d;
        public int obs2_x;
        public int obs2_y;
        public int obs2_Ideal;
        public int obs2_steps;
        public float obs2_time;
        public string obs2_attempts;
        public Puzzle04Controller.obsData obs3;
        public string obs3_ans;
        public string obs3_a;
        public string obs3_b;
        public string obs3_c;
        public string obs3_d;
        public int obs3_x;
        public int obs3_y;
        public int obs3_Ideal;
        public int obs3_steps;
        public float obs3_time;
        public string obs3_attempts;
        public Puzzle04Controller.obsData obs4;
        public string obs4_ans;
        public string obs4_a;
        public string obs4_b;
        public string obs4_c;
        public string obs4_d;
        public int obs4_x;
        public int obs4_y;
        public int obs4_Ideal;
        public int obs4_steps;
        public float obs4_time;
        public string obs4_attempts;
        public Puzzle04Controller.obsData obs5;
        public string obs5_ans;
        public string obs5_a;
        public string obs5_b;
        public string obs5_c;
        public string obs5_d;
        public int obs5_x;
        public int obs5_y;
        public int obs5_Ideal;
        public int obs5_steps;
        public float obs5_time;
        public string obs5_attempts;
        public Puzzle04Controller.obsData obs6;
        public string obs6_ans;
        public string obs6_a;
        public string obs6_b;
        public string obs6_c;
        public string obs6_d;
        public int obs6_x;
        public int obs6_y;
        public int obs6_Ideal;
        public int obs6_steps;
        public float obs6_time;
        public string obs6_attempts;
        public Puzzle04Controller.obsData obs7;
        public string obs7_ans;
        public string obs7_a;
        public string obs7_b;
        public string obs7_c;
        public string obs7_d;
        public int obs7_x;
        public int obs7_y;
        public int obs7_Ideal;
        public int obs7_steps;
        public float obs7_time;
        public string obs7_attempts;
        public Puzzle04Controller.obsData obs8;
        public string obs8_ans;
        public string obs8_a;
        public string obs8_b;
        public string obs8_c;
        public string obs8_d;
        public int obs8_x;
        public int obs8_y;
        public int obs8_Ideal;
        public int obs8_steps;
        public float obs8_time;
        public string obs8_attempts;
        public Puzzle04Controller.obsData obs9;
        public string obs9_ans;
        public string obs9_a;
        public string obs9_b;
        public string obs9_c;
        public string obs9_d;
        public int obs9_x;
        public int obs9_y;
        public int obs9_Ideal;
        public int obs9_steps;
        public float obs9_time;
        public string obs9_attempts;
    }
    [System.Serializable]
    public struct tbl_P4_2
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;

        public int tot_Ideal;
        public int tot_steps;
        public float tot_score;
        public float avg_score;

        public Puzzle04Controller.obsData obs1;
        public string obs1_ans;
        public string obs1_a;
        public string obs1_b;
        public string obs1_c;
        public string obs1_d;
        public int obs1_x;
        public int obs1_y;
        public int obs1_Ideal;
        public int obs1_steps;
        public float obs1_time;
        public string obs1_attempts;
        public Puzzle04Controller.obsData obs2;
        public string obs2_ans;
        public string obs2_a;
        public string obs2_b;
        public string obs2_c;
        public string obs2_d;
        public int obs2_x;
        public int obs2_y;
        public int obs2_Ideal;
        public int obs2_steps;
        public float obs2_time;
        public string obs2_attempts;
        public Puzzle04Controller.obsData obs3;
        public string obs3_ans;
        public string obs3_a;
        public string obs3_b;
        public string obs3_c;
        public string obs3_d;
        public int obs3_x;
        public int obs3_y;
        public int obs3_Ideal;
        public int obs3_steps;
        public float obs3_time;
        public string obs3_attempts;
        public Puzzle04Controller.obsData obs4;
        public string obs4_ans;
        public string obs4_a;
        public string obs4_b;
        public string obs4_c;
        public string obs4_d;
        public int obs4_x;
        public int obs4_y;
        public int obs4_Ideal;
        public int obs4_steps;
        public float obs4_time;
        public string obs4_attempts;
        public Puzzle04Controller.obsData obs5;
        public string obs5_ans;
        public string obs5_a;
        public string obs5_b;
        public string obs5_c;
        public string obs5_d;
        public int obs5_x;
        public int obs5_y;
        public int obs5_Ideal;
        public int obs5_steps;
        public float obs5_time;
        public string obs5_attempts;
        public Puzzle04Controller.obsData obs6;
        public string obs6_ans;
        public string obs6_a;
        public string obs6_b;
        public string obs6_c;
        public string obs6_d;
        public int obs6_x;
        public int obs6_y;
        public int obs6_Ideal;
        public int obs6_steps;
        public float obs6_time;
        public string obs6_attempts;
        public Puzzle04Controller.obsData obs7;
        public string obs7_ans;
        public string obs7_a;
        public string obs7_b;
        public string obs7_c;
        public string obs7_d;
        public int obs7_x;
        public int obs7_y;
        public int obs7_Ideal;
        public int obs7_steps;
        public float obs7_time;
        public string obs7_attempts;
        public Puzzle04Controller.obsData obs8;
        public string obs8_ans;
        public string obs8_a;
        public string obs8_b;
        public string obs8_c;
        public string obs8_d;
        public int obs8_x;
        public int obs8_y;
        public int obs8_Ideal;
        public int obs8_steps;
        public float obs8_time;
        public string obs8_attempts;
    }
    [System.Serializable]
    public struct tbl_P4_3
    {
        public string username;
        public string sessionID;

        public int attemptNum;
        public string attemptDate;
        public string usernameAttempt;

        public int tot_Ideal;
        public int tot_steps;
        public float tot_score;
        public float avg_score;

        public Puzzle04Controller.obsData obs1;
        public string obs1_ans;
        public string obs1_a;
        public string obs1_b;
        public string obs1_c;
        public string obs1_d;
        public int obs1_x;
        public int obs1_y;
        public int obs1_Ideal;
        public int obs1_steps;
        public float obs1_time;
        public string obs1_attempts;
        public Puzzle04Controller.obsData obs2;
        public string obs2_ans;
        public string obs2_a;
        public string obs2_b;
        public string obs2_c;
        public string obs2_d;
        public int obs2_x;
        public int obs2_y;
        public int obs2_Ideal;
        public int obs2_steps;
        public float obs2_time;
        public string obs2_attempts;
        public Puzzle04Controller.obsData obs3;
        public string obs3_ans;
        public string obs3_a;
        public string obs3_b;
        public string obs3_c;
        public string obs3_d;
        public int obs3_x;
        public int obs3_y;
        public int obs3_Ideal;
        public int obs3_steps;
        public float obs3_time;
        public string obs3_attempts;
        public Puzzle04Controller.obsData obs4;
        public string obs4_ans;
        public string obs4_a;
        public string obs4_b;
        public string obs4_c;
        public string obs4_d;
        public int obs4_x;
        public int obs4_y;
        public int obs4_Ideal;
        public int obs4_steps;
        public float obs4_time;
        public string obs4_attempts;
        public Puzzle04Controller.obsData obs5;
        public string obs5_ans;
        public string obs5_a;
        public string obs5_b;
        public string obs5_c;
        public string obs5_d;
        public int obs5_x;
        public int obs5_y;
        public int obs5_Ideal;
        public int obs5_steps;
        public float obs5_time;
        public string obs5_attempts;

    }


    //Player Data Items
    public tbl_Users users;
    public tbl_Records records;
    public tbl_bunRecords bunRecords;
    public tbl_Bunny bunny;
    public tbl_P1_1 p1_1;
    public tbl_P2_1 p2_1;
    public tbl_P2_2 p2_2;
    public tbl_P2_3 p2_3;
    public tbl_P3_1 p3_1;
    public tbl_P3_2 p3_2;
    public tbl_P3_3 p3_3;
    public tbl_P4_1 p4_1;
    public tbl_P4_2 p4_2;
    public tbl_P4_3 p4_3;

    //PlayerData Constructor
    public PlayerData()
    {
        users = new tbl_Users();
        records = new tbl_Records();
        bunRecords = new tbl_bunRecords();
        bunny = new tbl_Bunny();
        p1_1 = new tbl_P1_1();
        p2_1 = new tbl_P2_1();
        p2_2 = new tbl_P2_2();
        p3_1 = new tbl_P3_1();
        p3_2 = new tbl_P3_2();
        p3_3 = new tbl_P3_3();
        p4_1 = new tbl_P4_1();
        p4_2 = new tbl_P4_2();
        p4_3 = new tbl_P4_3();

        //TODO: Table Value Initilaztions

        //player.users INIT
        users.username = "default";
        users.isExpo = false;
        users.isBunny = false;
        users.last_login = System.DateTime.Now.ToString();
        users.admin = false;
        users.mouseSense = 2.0f;
        users.soundVol = 0.25f;
        users.soundFX = 1.0f;

        //users.p1_1clear_time = 0.0f;
        //users.p2_1clear_time = 0.0f;
        //users.p2_2clear_time = 0.0f;
        //users.p2_3clear_time = 0.0f;
        //users.p3_1clear_time = 0.0f;
        //users.p3_2clear_time = 0.0f;
        //users.p3_3clear_time = 0.0f;
        //users.p4_1clear_time = 0.0f;
        //users.p4_2clear_time = 0.0f;
        //users.p4_3clear_time = 0.0f;
        //users.p1_1attempts = 0;
        //users.p2_1attempts = 0;
        //users.p2_2attempts = 0;
        //users.p2_3attempts = 0;
        //users.p3_1attempts = 0;
        //users.p3_2attempts = 0;
        //users.p3_3attempts = 0;
        //users.p4_1attempts = 0;
        //users.p4_2attempts = 0;
        //users.p4_3attempts = 0;


        //player.p1_1 INIT

        //player.p2_1 INIT

        //player.p2_2 INIT

        //player.p3_1 INIT

        //player.p4_1 INIT
        p4_1.obs1_time = 0.0f;
        p4_1.obs2_time = 0.0f;
        p4_1.obs3_time = 0.0f;
        p4_1.obs4_time = 0.0f;
        p4_1.obs5_time = 0.0f;
        p4_1.obs6_time = 0.0f;
        p4_1.obs7_time = 0.0f;
        p4_1.obs8_time = 0.0f;
        p4_1.obs9_time = 0.0f;

        //player.p4_2 INIT
        p4_2.obs1_time = 0.0f;
        p4_2.obs2_time = 0.0f;
        p4_2.obs3_time = 0.0f;
        p4_2.obs4_time = 0.0f;
        p4_2.obs5_time = 0.0f;
        p4_2.obs6_time = 0.0f;
        p4_2.obs7_time = 0.0f;
        p4_2.obs8_time = 0.0f;

        //player.p4_3 INIT
        p4_3.obs1_time = 0.0f;
        p4_3.obs2_time = 0.0f;
        p4_3.obs3_time = 0.0f;
        p4_3.obs4_time = 0.0f;
        p4_3.obs5_time = 0.0f;
    }

    public void SetUserName(string new_name)
    {
        users.username = new_name;
        records.username = new_name;
        bunRecords.username = new_name;
        bunny.username = new_name;
        p1_1.username = new_name;
        p2_1.username = new_name;
        p2_2.username = new_name;
        p2_3.username = new_name;
        p3_1.username = new_name;
        p3_2.username = new_name;
        p3_3.username = new_name;
        p4_1.username = new_name;
        p4_2.username = new_name;
        p4_3.username = new_name;
    }

    public void SetSessionID(string session)
    {
        users.sessionID = session;
        records.sessionID = session;
        bunRecords.sessionID = session;
        bunny.sessionID = session;
        p1_1.sessionID = session;
        p2_1.sessionID = session;
        p2_2.sessionID = session;
        p2_3.sessionID = session;
        p3_1.sessionID = session;
        p3_2.sessionID = session;
        p3_3.sessionID = session;
        p4_1.sessionID = session;
        p4_2.sessionID = session;
        p4_3.sessionID = session;
    }
}
