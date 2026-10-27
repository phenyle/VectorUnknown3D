using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/* Author : Nate Cortes
 * Load Level Methods. Made for CPI441 capstone. 
 */

public class gui_select : MonoBehaviour {

	public int attempt_count = 0;
	public int display_upcoming_path = 0;
	public int display_past_paths = 0;
	public int load_game_mode = 0;
    public bool tutorial = false;
    [Range(0, 7)]
    public int level_number;

	void Awake(){
		DontDestroyOnLoad (transform.gameObject);
	}

    void Start()
    {
        if (GameRootMaster.isSession)
        {
            GameConstants.difficulty = GameRootMaster.player.users.difficultyNum;
            GameRootMaster.player.bunny.level = GameRootMaster.player.users.difficultyNum;

            switch (GameRootMaster.player.users.stageNum)
            {
                case 1: level_one(); break;
                case 2: level_two(); break;
                case 3: level_five(); break;
                case 4: level_six(); break;
                case 5: level_seven(); break;
                case 6: level_eight(); break;
                case 7: level_nine(); break;
                default: GameRootMaster.isSession = false; break;
            }
        }
    }

	public void load_game(){
		SceneManager.LoadScene ("VectorGame");
	}

	public void level_one(){
        Psychometrics.logEvent("L1");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 1;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings(-1, 1, 1, 0);
        PlayerPrefs.SetInt("CurrentLevel", 1);
		SceneManager.LoadScene ("VectorGame");
	}

	public void level_two(){
        Psychometrics.logEvent("L2");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 2;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings(-1, 0, 1, 0);
        PlayerPrefs.SetInt("CurrentLevel", 2);
        SceneManager.LoadScene ("VectorGame");
	}

	//public void level_three(){
 //       Psychometrics.logEvent("L3");
 //       puzzle_settings ( 5, 1, 1, 0);
 //       PlayerPrefs.SetInt("CurrentLevel", 3);
 //       SceneManager.LoadScene ("VectorGame");
	//}

	//public void level_four(){
 //       Psychometrics.logEvent("L4");
 //       puzzle_settings ( 5, 0, 0, 0);
 //       PlayerPrefs.SetInt("CurrentLevel", 4);
 //       SceneManager.LoadScene ("VectorGame");
	//}

	public void level_five(){
        Psychometrics.logEvent("L3");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 3;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings( -1, 1, 1, 1);
        PlayerPrefs.SetInt("CurrentLevel", 3);
        SceneManager.LoadScene ("VectorGame");

        Debug.Log("Button 5 Pressed");
	}
    public void level_six()
    {
        Psychometrics.logEvent("L4");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 4;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings(-1, 1, 1, 2);
        PlayerPrefs.SetInt("CurrentLevel", 4);
        SceneManager.LoadScene("VectorGame");

        Debug.Log("Button 6 Pressed");
    }
    public void level_seven()
    {
        Psychometrics.logEvent("L5");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 5;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings(-1, 1, 1, 3);
        PlayerPrefs.SetInt("CurrentLevel", 5);
        SceneManager.LoadScene("VectorGame");

        Debug.Log("Button 7 Pressed");
    }

    public void level_eight()
    {
        Psychometrics.logEvent("L6");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 6;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings(-1, 1, 1, 4);
        PlayerPrefs.SetInt("CurrentLevel", 6);
        SceneManager.LoadScene("VectorGame");

        Debug.Log("Button 8 Pressed");
    }

    public void level_nine()
    {
        Psychometrics.logEvent("L7");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 7;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings(-1, 1, 1, 5);
        PlayerPrefs.SetInt("CurrentLevel", 7);
        SceneManager.LoadScene("VectorGame");

        Debug.Log("Button 9 Pressed");
    }

    public void level_tutorial()
    {
        Psychometrics.logEvent("LT");
        Psychometrics.steps = new List<string>();
        GameRootMaster.player.bunny.stage = 0;
        SaveAttemptNum(GameRootMaster.player.bunny.stage);
        puzzle_settings(-1, 1, 1, 3);
        set_tutorial();
        PlayerPrefs.SetInt("CurrentLevel", 0);
        SceneManager.LoadScene("tutorial");
    }

	/***************************************/
	/* Helper Method, sets values of Puzzle*/
	/***************************************/
    private void SaveAttemptNum(int stage)
    {
        int attemptNum = 0;
        switch (stage)
        {
            case 0:
                switch(GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_00_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_00_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_00_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_00_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_00_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_00_attempts;
                        break;
                }
                break;

            case 1:
                switch (GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_11_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_11_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_12_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_12_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_13_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_13_attempts;
                        break;
                }
                break;

            case 2:
                switch (GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_21_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_21_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_22_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_22_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_23_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_23_attempts;
                        break;
                }
                break;

            case 3:
                switch (GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_31_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_31_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_32_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_32_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_33_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_33_attempts;
                        break;
                }
                break;

            case 4:
                switch (GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_41_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_41_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_42_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_42_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_43_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_43_attempts;
                        break;
                }
                break;

            case 5:
                switch (GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_51_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_51_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_52_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_52_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_53_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_53_attempts;
                        break;
                }
                break;

            case 6:
                switch (GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_61_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_61_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_62_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_62_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_63_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_63_attempts;
                        break;
                }
                break;

            case 7:
                switch (GameConstants.difficulty)
                {
                    case 1:
                        ++GameRootMaster.player.bunRecords.bun_71_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_71_attempts;
                        break;
                    case 2:
                        ++GameRootMaster.player.bunRecords.bun_72_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_72_attempts;
                        break;
                    case 3:
                        ++GameRootMaster.player.bunRecords.bun_73_attempts;
                        attemptNum = GameRootMaster.player.bunRecords.bun_73_attempts;
                        break;
                }
                break;
        }

        GameRootMaster.player.bunny.username = GameRootMaster.player.users.username;
        GameRootMaster.player.bunny.attemptNum = attemptNum;
        string usernameAttempt = GameRootMaster.player.bunny.username + "_" +
                                 GameRootMaster.player.bunny.stage +
                                 GameRootMaster.player.bunny.level + "_" +
                                 attemptNum;
        GameRootMaster.player.bunny.usernameAttempt = usernameAttempt;
        GameRootMaster.player.bunRecords.username = GameRootMaster.player.users.username;
    }

	private void puzzle_settings( int attempts, int future_paths, int previous_paths, int game_mode){
		attempt_count = attempts;
		display_upcoming_path = future_paths;
		display_past_paths = previous_paths;
		load_game_mode = game_mode;
	}

    private void set_tutorial()
    {
        this.tutorial = true;
    }

}
