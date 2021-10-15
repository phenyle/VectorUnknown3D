using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : WindowRoot
{
    public Slider loadingBar;

    protected override void InitWindow()
    {
        base.InitWindow();
        Debug.Log("Init loading window");
    }

    public void SetProgress(float percentage)
    {
        loadingBar.value = percentage;
    }
}
