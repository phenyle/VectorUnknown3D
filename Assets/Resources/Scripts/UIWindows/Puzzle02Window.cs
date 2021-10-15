using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Puzzle02Window : WindowRoot
{
    public bool isInit = false;
    public Image[] shipImages;
    public Text Matrix, Vector;

    private GameControllerPuzzle02 GCP02_01;
    private GameControllerPuzzle02 GCP02_02;

    private GameObject stage01;
    private GameObject stage02;

    private void Start()
    {
        if(isInit)
        {
            InitWindow();
        }
    }

    protected override void InitWindow()
    {
        Debug.Log("Init Puzzle02 window");
        base.InitWindow();

        if(SceneManager.GetActiveScene().name == Constants.puzzle02_1SceneName)
        {
            GCP02_01 = GameObject.Find("GameController_Puzzle02_01").GetComponent<GameControllerPuzzle02>();
            GCP02_01.InitGameController(this);
        }
        else if(SceneManager.GetActiveScene().name == Constants.puzzle02_2SceneName)
        {
            GCP02_02 = GameObject.Find("GameController_Puzzle02_02").GetComponent<GameControllerPuzzle02>();
            GCP02_02.InitGameController(this);
        }
        Debug.Log("Call GameController of Puzzle02 to connect");
    }

    public void switchStage()
    {
        stage01 = GameObject.Find("Stage1");
        stage01.SetActive(false);
        // stage02 = GameObject.Find("Stage2");
        // stage02.SetActive(true); 
        GCP02_02 = GameObject.Find("GameController_Puzzle02_02").GetComponent<GameControllerPuzzle02>();
        GCP02_02.InitGameController(this);
    }
}
