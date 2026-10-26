using UnityEngine;

public class CreditsWindow : WindowRoot
{
    public bool isInit;

    private void Start()
    {
        if(isInit)
        {
            InitWindow();
        }
    }

    protected override void InitWindow()
    {
        Debug.Log("Init Credits window");
        base.InitWindow();
    }
}
