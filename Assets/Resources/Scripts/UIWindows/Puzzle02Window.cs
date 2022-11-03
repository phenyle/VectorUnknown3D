using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Puzzle02Window : WindowRoot
{
    public bool isInit = false;
    public Text Matrix1, Matrix2, Vector, target;

    private GameControllerPuzzle02 GCP02;

    private GameObject stage01;
    private bool showOverlay = false;
    [HideInInspector]
    public bool showTargetLines = false;


    [Header("------ Progress Panel ---------")]
    public Transform panelProgress;
    public Text txt_Header;
    public Text txt_Complete;
    public Slider slider_Complete;
    public Text txt_Time;

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

        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
        GCP02.InitGameController(this);

        txt_Header.text = "Stage 2 - Level " + GCP02.Difficulty.ToString();

        Debug.Log("Call GameController of Puzzle02 to connect");
    }


    public void SetTargetShip(Vector3 shipPos)
    {
        target.text = shipPos.x.ToString() + "\n" + shipPos.z.ToString();       
    }

    public void SetMatrixSelected(int[][] matrix)
    {
        Matrix1.text = matrix[0][0].ToString() + "\n" + matrix[0][1].ToString();
        Matrix2.text = matrix[1][0].ToString() + "\n" + matrix[1][1].ToString();
    }

    public void SetVectorSelected(int[] vect)
    {
        Vector.text = vect[0].ToString() + "\n" + vect[1].ToString();
    }

    public void SetCompleteNumber(int sunk, int total)
    {
        txt_Complete.text = sunk.ToString() + "/" + total.ToString();
        slider_Complete.maxValue = total;
        slider_Complete.value = sunk;
    }

    public void SetTime(string time)
    {
        txt_Time.text = time;
    }

    public void ClickChangeCamera()
    {
        GCP02.SwitchCamera();
    }

    public void ClickShowLattice()
    {
        showOverlay = !showOverlay;
        GCP02.ToggleOverlay(showOverlay);
    }

    public void ClickShowTargetLines()
    {
        showTargetLines = !showTargetLines;
        GCP02.ToggleTargetLines(showTargetLines);

    }
}
