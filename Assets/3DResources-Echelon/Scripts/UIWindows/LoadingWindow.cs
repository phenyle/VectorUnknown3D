using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : WindowRoot
{
    public Slider loadingBar;

    protected override void InitWindow()
    {
        if (GameRoot3D.instance != null)
        {
            base.InitWindow();
            Debug.Log("Init loading window");
        }
    }

    public void AltInit()
    {
        InitWindow();
    }

    public void SetProgress(float percentage)
    {
        loadingBar.value = percentage;
    }
}
