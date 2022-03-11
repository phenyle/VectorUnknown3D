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
            GameRoot.ShowTips("", false, false);
            titleWindow.SetWindowState(true);
            audioService.PlayBgMusic(Constants.audioBgMenu, true);
        });
    }
}
