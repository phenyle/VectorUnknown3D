
public class Constants
{
    //Game Settings
    public const int numOfStages = 4;
    public const int numLvlsStage1 = 1;
    public const int numLvlsStage2 = 3;
    public const int numLvlsStage3 = 3;
    public const int numLvlsStage4 = 3;

    //Scene Names
    public const string gameRootSceneName = "00GameRoot";
    public const string menuSceneName = "01Menu";
    public const string mainSceneName = "02Main";
    public const string puzzle01SceneName = "03Puzzle01";
    public const string puzzle02_1SceneName = "04Puzzle02-1";
    public const string puzzle02_2SceneName = "04Puzzle02-2";
    public const string puzzle02_3SceneName = "04Puzzle02-3";
    public const string puzzle03SceneName = "05Puzzle03";
    public const string puzzle03_1SceneName = "05Puzzle03-1";
    public const string puzzle03_2SceneName = "05Puzzle03-2";
    public const string puzzle03_3SceneName = "05Puzzle03-3";
    public const string puzzle04SceneName = "06Puzzle04-1"; // for scene 4
    public const string puzzle04_1SceneName = "06Puzzle04-1"; // for scene 4
    public const string puzzle04_2SceneName = "06Puzzle04-2"; // for scene 4
    public const string puzzle04_3SceneName = "06Puzzle04-3"; // for scene 4
    public const string creditSceneName = "06Credit";
    public const string tutorialSceneName = "07Tutorial";

    //Music Names
    public const string audioBgMenu = "00MenuBg";
    public const string audioBgMain = "01MainBg";
    public const string audioBgPuzzle1 = "02Puzzle01Bg";
    public const string audioBgPuzzle2 = "03Puzzle02Bg";
    public const string audioBgPuzzle3 = "04Puzzle03Bg";
    //public const string audioBgPuzzle4 = "04Puzzle04Bg"; 
    public const string audioBgCredit = "05CreditsBg";
    public const string audioBgTutorial = "06TutorialBg";

    //UI Sound FX Names
    public const string audioUIStartBtn = "UI_button_click";
    public const string audioUIClickBtn = "UI_button_click";
    public const string audioUIBackBtn = "UI_button_click";
    public const string audioUINextBtn = "UI_button_click";

    //FX Puzzle01
    public const string audioP01CorrectAnswer = "MUSIC_EFFECT_Solo_Harp_Positive_01_stereo";
    public const string audioP01WrongAnswer = "MUSIC_EFFECT_Solo_Harp_Negative_01_stereo";
    public const string audioP01Congratulation = "MUSIC_EFFECT_Platform_Positive_03a_Fast_stereo";

    //FX Puzzle02
    public const string audioP02Selection = "Puzzle02/IMPACT_Metal_Barrel_Subtle_mono";
    public const string audioP02CannonFire = "Puzzle02/FIREWORKS_Rocket_Explode_Large_RR4_mono";
    public const string audioP02BallHit = "Puzzle02/DEMOLISH_Wood_Fall_stereo";
    public const string audioP02BallMiss = "Puzzle02/SPLASH_Big_Noise_mono";

    //FX Puzzle03
    public const string audioP03Click = "UI_button_click";
    public const string audioP03ClickClear = "UI_Click_Cut_mono";
    public const string audioP03TriggerQuestion = "UI_Toggle_Voice_Tripe_Note_Enable_stereo";
    public const string audioP03ExitQuestion = "UI_Toggle_Voice_Triple_Note_Disable_stereo";
    public const string audioP03FinishSubPuzzle = "MUSIC_EFFECT_Solo_Harp_Positive_01_stereo";
    public const string audioP03RotatedPuzzleEnvironment = "CONSTRUCTION_Digger_Loading_01_loop_stereo";
}
