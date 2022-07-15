using UnityEngine;

public class PuzzleSystem : SystemRoot
{
    [Header("Title Window")]
    public TitleWindow titleWindow;

    [Header("Main Window")]
    public MainWindow mainWindow;

    [Header("Puzzle Windows")]
    public Puzzle01Window puzzle01Window;
    public Puzzle02Window puzzle02Window;
    public Puzzle03Window puzzle03Window;
    public Puzzle04Window puzzle04Window;

    [Header("Credits Windows")]
    public CreditsWindow creditsWindow;

    [Header("Tutorial Windows")]
    public TutorialWindow tutorialWindow;

    [Header("Pause Windows")]
    public PauseWindow pauseWindow;

    public override void InitSystem()
    {
        Debug.Log("Init Puzzle System");
        base.InitSystem();
    }

    public void EnterPuzzle(string puzzleName)
    {
        resourceService.AsynLoadScene(puzzleName, () =>
        {
            CloseAllWindow();
            GameRoot3D.ShowTips("", false, false);
            switch (puzzleName)
            {
                case Constants.menuSceneName:
                    try
                    {
                        DialogueManager.instance.ResetAll();
                    }
                    catch { }

                    titleWindow.SetWindowState(true);
                    audioService.PlayBgMusic(0, true);
                    break;

                case Constants.mainSceneName:
                    mainWindow.SetWindowState(true);
                    pauseWindow.showPuzzleControls(false);
                    audioService.PlayBgMusic(1, true);
                    break;

                case Constants.puzzle01SceneName:
                    puzzle01Window.SetWindowState(true);
                    audioService.PlayBgMusic(2, true);
                    break;

                case Constants.puzzle02_1SceneName:
                    puzzle02Window.SetWindowState(true);
                    pauseWindow.showPuzzleControls(2);
                    audioService.PlayBgMusic(3, true);
                    break;

                case Constants.puzzle02_2SceneName:
                    puzzle02Window.SetWindowState(true);
                    pauseWindow.showPuzzleControls(2);
                    audioService.PlayBgMusic(3, true);
                    break;

                case Constants.puzzle02_3SceneName:
                    puzzle02Window.SetWindowState(true);
                    pauseWindow.showPuzzleControls(2);
                    audioService.PlayBgMusic(3, true);
                    break;

                case Constants.puzzle03_1SceneName:
                    puzzle03Window.SetWindowState(true);
                    puzzle03Window.SetPanelChoice(0);
                    pauseWindow.showPuzzleControls(3);
                    puzzle03Window.bVal = true;
                    audioService.PlayBgMusic(4, true);
                    break;

                case Constants.puzzle03_2SceneName:
                    puzzle03Window.SetWindowState(true);
                    puzzle03Window.SetPanelChoice(0);
                    pauseWindow.showPuzzleControls(3);
                    puzzle03Window.bVal = true;
                    audioService.PlayBgMusic(4, true);
                    break;

                case Constants.puzzle03_3SceneName:
                    puzzle03Window.SetWindowState(true);
                    puzzle03Window.SetPanelChoice(0);
                    pauseWindow.showPuzzleControls(3);
                    puzzle03Window.bVal = true;
                    audioService.PlayBgMusic(4, true);
                    break;

                case Constants.puzzle04_1SceneName:
                    puzzle04Window.SetWindowState(true);
                    pauseWindow.showPuzzleControls(true);
                    pauseWindow.showPuzzleControls(4);
                    audioService.PlayBgMusic(3, true);
                    break;

                case Constants.puzzle04_2SceneName:
                    puzzle04Window.SetWindowState(true);
                    pauseWindow.showPuzzleControls(true);
                    pauseWindow.showPuzzleControls(4);
                    audioService.PlayBgMusic(3, true);
                    break;

                case Constants.puzzle04_3SceneName:
                    puzzle04Window.SetWindowState(true);
                    pauseWindow.showPuzzleControls(true);
                    pauseWindow.showPuzzleControls(4);
                    audioService.PlayBgMusic(3, true);
                    break;

                case Constants.creditSceneName:
                    creditsWindow.SetWindowState(true);
                    audioService.PlayBgMusic(5, true);
                    break;

                case Constants.tutorialSceneName:
                    tutorialWindow.SetWindowState(true);
                    audioService.PlayBgMusic(6, true);
                    break;
            }            
        });
    }
}
