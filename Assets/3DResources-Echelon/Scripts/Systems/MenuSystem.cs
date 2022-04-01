using UnityEngine;

public class MenuSystem : SystemRoot
{
    [Header("UI Windows")]
    public TitleWindow titleWindow;

    public override void InitSystem()
    {
        Debug.Log("Init Menu System");
        base.InitSystem();
    }

    public void EnterMenu()
    {
        resourceService.AsynLoadScene(Constants.menuSceneName, () =>
        {
            GameRoot3D.ShowTips("", false, false);
            titleWindow.SetWindowState(true);
            audioService.PlayBgMusic(0, true);
        });
    }
}
