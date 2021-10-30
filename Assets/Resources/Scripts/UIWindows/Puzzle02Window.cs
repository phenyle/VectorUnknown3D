using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Puzzle02Window : WindowRoot
{
    public bool isInit = false;
    public Image[] shipImages;
    public Text Matrix1, Matrix2, Vector, target;

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
            GCP02_01 = GameObject.Find("GameController_Puzzle02").GetComponent<GameControllerPuzzle02>();
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

    public void SetTargetShip(int[] shipPos)
    {
        target.text = shipPos[0].ToString() + "\n" + shipPos[2].ToString();
    }

    public void SetMatrixSelected(int[][] matrix)
    {
        Matrix1.text = matrix[0][0].ToString() + "\n" + matrix[0][1].ToString();
        Matrix2.text = matrix[1][0].ToString() + "\n" + matrix[1][1].ToString();
    }

    public void SetVectorSelected(int[] vect)
    {
        Vector.text = vect[0].ToString() + "\n" + vect[2].ToString();
    }

    public void ClickChangeCamera()
    {

    }
}
