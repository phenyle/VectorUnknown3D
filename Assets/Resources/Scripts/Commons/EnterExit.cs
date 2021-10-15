using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    MainSceneName,
    Puzzle01SceneName,
    Puzzle02_1SceneName,
    Puzzle02_2SceneName,
    Puzzle02_3SceneName,
    Puzzle03_1SceneName,
    Puzzle03_2SceneName,
    Puzzle03_3SceneName,
    Puzzle04_1SceneName,
    Puzzle04_2SceneName,
    Puzzle04_3SceneName,
    CreditSceneName,
    MenuScene
}

public enum PuzzleComplete
{
    NullPuzzle,
    Puzzle01Complete,
    Puzzle02_1Complete,
    Puzzle02_2Complete,
    Puzzle02_3Complete,
    Puzzle03_1Complete,
    Puzzle03_2Complete,
    Puzzle03_3Complete,
    Puzzle04_1Complete,
    Puzzle04_2Complete,
    Puzzle04_3Complete
}

public class EnterExit : MonoBehaviour
{
    public SceneName sceneName;
    public PuzzleComplete puzzleComplete;

    private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if(playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if(other.tag == "Player")
        {
            playerController.isEnterExit = true;
            GameRoot.ShowTips("Press \"E\" to enter", true, false);

            switch(sceneName)
            {
                case SceneName.MenuScene:
                    playerController.sceneName = Constants.menuSceneName;

                    break;

                case SceneName.MainSceneName:
                    playerController.sceneName = Constants.mainSceneName;

                    if (SceneManager.GetActiveScene().name == Constants.menuSceneName)
                        GameRoot.instance.exitPuzzle = 0;                   
                    else if (SceneManager.GetActiveScene().name == Constants.puzzle01SceneName)
                        GameRoot.instance.exitPuzzle = 1;
                    else if (SceneManager.GetActiveScene().name == Constants.puzzle02_1SceneName ||
                                SceneManager.GetActiveScene().name == Constants.puzzle02_2SceneName)
                        GameRoot.instance.exitPuzzle = 2;
                    else if (SceneManager.GetActiveScene().name == Constants.puzzle03_1SceneName ||
                                SceneManager.GetActiveScene().name == Constants.puzzle03_2SceneName ||
                                SceneManager.GetActiveScene().name == Constants.puzzle03_3SceneName)
                        GameRoot.instance.exitPuzzle = 3;
                    else if (SceneManager.GetActiveScene().name == Constants.puzzle04_1SceneName ||
                                SceneManager.GetActiveScene().name == Constants.puzzle04_2SceneName ||
                                SceneManager.GetActiveScene().name == Constants.puzzle04_3SceneName)
                        GameRoot.instance.exitPuzzle = 4;                   

                    break;

                case SceneName.Puzzle01SceneName:
                    playerController.sceneName = Constants.puzzle01SceneName;
                    break;

                case SceneName.Puzzle02_1SceneName:
                    playerController.sceneName = Constants.puzzle02_1SceneName;
                    break;

                case SceneName.Puzzle02_2SceneName:
                    playerController.sceneName = Constants.puzzle02_2SceneName;
                    break;

                case SceneName.Puzzle03_1SceneName:
                    playerController.sceneName = Constants.puzzle03SceneName;
                    break;

                case SceneName.Puzzle04_1SceneName:
                    playerController.sceneName = Constants.puzzle04SceneName;
                    break;

                case SceneName.CreditSceneName:
                    playerController.sceneName = Constants.creditSceneName;
                    break;
            }

            switch(puzzleComplete)
            {
                case PuzzleComplete.Puzzle01Complete:

                    GameRoot.instance.prevStage = 1;
                    GameRoot.instance.prevLevel = 1;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[0] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle01>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p1_1, GameRoot.player.p1_1.usernameAttempt);

                    break;

                case PuzzleComplete.Puzzle02_1Complete:

                    GameRoot.instance.prevStage = 2;
                    GameRoot.instance.prevLevel = 1;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[1] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle02>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p2_1, GameRoot.player.p2_1.usernameAttempt);

                    break;

                case PuzzleComplete.Puzzle02_2Complete:


                    GameRoot.instance.prevStage = 2;
                    GameRoot.instance.prevLevel = 2;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[1] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle02>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p2_2, GameRoot.player.p2_2.usernameAttempt);

                    break;

                case PuzzleComplete.Puzzle02_3Complete:


                    GameRoot.instance.prevStage = 2;
                    GameRoot.instance.prevLevel = 3;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[1] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle02>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p2_3, GameRoot.player.p2_3.usernameAttempt);

                    break;

                case PuzzleComplete.Puzzle03_1Complete:

                    GameRoot.instance.prevStage = 3;
                    GameRoot.instance.prevLevel = 1;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[2] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle03>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p3_1, GameRoot.player.p3_1.usernameAttempt);
                    break;

                case PuzzleComplete.Puzzle03_2Complete:

                    GameRoot.instance.prevStage = 3;
                    GameRoot.instance.prevLevel = 2;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[2] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle03>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p3_2, GameRoot.player.p3_2.usernameAttempt);

                    break;

                case PuzzleComplete.Puzzle03_3Complete:

                    GameRoot.instance.prevStage = 3;
                    GameRoot.instance.prevLevel = 3;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[2] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle03>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p3_3, GameRoot.player.p3_3.usernameAttempt);

                    break;

                case PuzzleComplete.Puzzle04_1Complete:

                    GameRoot.instance.prevStage = 4;
                    GameRoot.instance.prevLevel = 1;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[3] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle04>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p4_1, GameRoot.player.p4_1.usernameAttempt);
                    break;

                case PuzzleComplete.Puzzle04_2Complete:

                    GameRoot.instance.prevStage = 4;
                    GameRoot.instance.prevLevel = 2;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[3] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle04>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p4_2, GameRoot.player.p4_2.usernameAttempt);
                    break;

                case PuzzleComplete.Puzzle04_3Complete:

                    GameRoot.instance.prevStage = 4;
                    GameRoot.instance.prevLevel = 3;

                    if (!GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel])
                        GameRoot.instance.firstCompletion = true;

                    GameRoot.instance.puzzlesDone[GameRoot.instance.prevStage][GameRoot.instance.prevLevel] = true;

                    GameRoot.instance.puzzleCompleted[3] = true;
                    GameObject.Find("GameController").GetComponent<GameControllerPuzzle04>().RecordData();
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.users, GameRoot.player.users.username);
                    GameRoot.GSFU.UpdatePlayerTable(true, PlayerData.sheets.p4_3, GameRoot.player.p4_3.usernameAttempt);
                    break;


                default:
                    break;
            }


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerController.isEnterExit = false;

            GameRoot.ShowTips("", false, false);
        }
    }
}
