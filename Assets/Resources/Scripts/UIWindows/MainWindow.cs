using UnityEngine;
using UnityEngine.UI;

public class MainWindow : WindowRoot
{
    public bool isInit;
    public Text txtInstruction;
    public Sprite[] imgTips;
    public Image[] imgTipsChecks;

    private GameControllerMain GCM;

    private void Start()
    {
        if(isInit)
        {
            InitWindow();
        }
    }

    protected override void InitWindow()
    {
        Debug.Log("Init Main Window");
        base.InitWindow();

        for (int i = 0; i < imgTipsChecks.Length; i++)
        {
            imgTipsChecks[i].sprite = imgTips[i];
        }

        GCM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerMain>();
        GCM.InitGameController(this);
    }

    public void SetInstructionText(string content)
    {
        txtInstruction.text = content;
    }

    public void SetCheckImage(int index, Sprite sprite)
    {
        imgTipsChecks[index].sprite = sprite;
    }
}
