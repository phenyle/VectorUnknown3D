using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : WindowRoot
{
    [Header("---- Pause Main -----")]
    public Transform panelPause;
    public GameObject btn_Hub;
    public Transform[] pauseButtons;

    [Header("---- Basic Controls ----")]
    public Transform basicControls;
    public Text controlsText;


    [Header("---- Puzzle Controls ----")]
    public Transform puzzleControls;
    public Text puzzleText;

    [Header("---- Options ----")]
    public Transform panelOption;
    public Slider sliderVolume;
    public Slider sliderSoundFX;
    public Slider sliderMouseSense;

    [Header("---- PopUp ----")]
    public Transform popupConfirm;
    public Text popupHeader;
    private int popupContext = 0;
    public bool popupActive;

    [Header("---- LoginInfo ----")]
    public Transform panelLoginInfo;
    public Text loginInfoText;


    protected override void InitWindow()
    {
        base.InitWindow();
        Debug.Log("Init Pause Window");
        sliderVolume.value = audioService.bgVolume;
        sliderSoundFX.value = audioService.UIFXVolume;
        popupActive = false;


        defaultBasicControlsText();
    }

    public void ClickResumeBtn()
    {
        if (!popupActive)
        {
            audioService.PlayUIAudio(Constants.audioUIClickBtn);
            GameRoot.instance.Resume();
        }
    }

    public void ClickSettingBtn()
    {
        if (!popupActive)
        {
            audioService.PlayUIAudio(Constants.audioUIClickBtn);

            SetActive(panelPause, false);
            SetActive(panelOption, true);
            sliderMouseSense.value = GameRoot.player.users.mouseSense;
            sliderSoundFX.value = GameRoot.player.users.soundFX;
            sliderVolume.value = GameRoot.player.users.soundVol;
        }
    }

    public void ClickSettingsBackBtn()
    {
        if (!popupActive)
        {
            audioService.PlayUIAudio(Constants.audioUIClickBtn);

            GameRoot.player.users.mouseSense = sliderMouseSense.value;
            GameRoot.player.users.soundFX = sliderSoundFX.value;
            GameRoot.player.users.soundVol = sliderVolume.value;

            if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>() != null)
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().SetRotateSpeed(sliderMouseSense.value);

            SetActive(panelOption, false);
            SetActive(panelPause, true);
        }
    }

    public void ClickHubBtn()
    {
        if (!popupActive)
        {
            popupActive = true;
            popupContext = 0;
            popupHeader.text = "Return to Hub";
            SetActive(popupConfirm, true);
            audioService.PlayUIAudio(Constants.audioUIClickBtn);
        }
    }
    public void ClickQuitBtn()
    {
        if (!popupActive)
        {
            popupActive = true;
            popupContext = 1;
            popupHeader.text = "Quit to Menu";
            SetActive(popupConfirm, true);
            audioService.PlayUIAudio(Constants.audioUIClickBtn);
        }
    }


    public void ClickPopUpConfirmBtn()
    {
        switch (popupContext)
        {
            case 0: //Return to Hub
                audioService.PlayUIAudio(Constants.audioUIClickBtn);
                GameRoot.instance.Resume();
                ResetPauseMenu();

                GameRoot.instance.exitPuzzle = 0;
                GameRoot.instance.puzzleSystem.EnterPuzzle(Constants.mainSceneName);
                // hides mouse when loading game
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case 1: //Quit to Main Menu
                popupActive = false;
                audioService.PlayUIAudio(Constants.audioUIClickBtn);
                GameObject.Find("DialogueManager").GetComponent<DialogueManager>().EndDialogue();
                //Resume can unlock the lock
                GameRoot.isPuzzleLock = false;

                GameRoot.player.users.last_login = System.DateTime.Now.ToString();
                GameRoot.GSFU.UpdatePlayerTable(false, PlayerData.sheets.users, GameRoot.player.users.username);
                GameRoot.player = new PlayerData();

                ResetPauseMenu();

                //Dialogue manager can unlock the lock;
                DialogueManager.isPuzzleLock = false;
                GameRoot.instance.InitUI();
                GameRoot.instance.menuSystem.EnterMenu();
                GameRoot.instance.Resume();
                break;
        }
    }

    public void ClickPopUpBackBtn()
    {
        audioService.PlayUIAudio(Constants.audioUIClickBtn);
        ResetPauseMenu();
    }

    public void ResetPauseMenu()
    {
        popupActive = false;
        SetActive(popupConfirm, false);
    }


    public void SetBgVolume()
    {
        audioService.SetBgVolume(sliderVolume.value);
    }

    public void SetSoundFXVolume()
    {
        audioService.SetSoundFXVolume(sliderSoundFX.value);
    }

    public void defaultBasicControlsText()
    {
        controlsText.text = "MOVE:\n" +
                            "W - Move Forward\n" +
                            "A - Move Left\n" +
                            "S - Move Backward\n" +
                            "D - Move Right\n" +
                            "Left Shift - Run\n" +
                            "\n" +
                            "I - Rotate Camera Up\n" +
                            "J - Rotate Camera Left\n" +
                            "K - Rotate Camera Down\n" +
                            "L - Rotate Camera Right\n" +
                            "Mouse - Move Camera\n" +
                            "\n" +
                            "R - Reset Character\n" +
                            "Z - Swich To Top-Down View\n" +
                            "Tab - Show / Hide Mouse\n" +
                            "Space - Advance Dialogue\n" +
                            "Escape - Pause Game";
    }

    public void setBasicControlsText(string newControls)
    {
        controlsText.text = newControls;
    }

    public void showPuzzleControls(bool val)
    {
        SetActive(puzzleControls, val);
    }

    public void setPuzzleControlsText(string newControls)
    {
        puzzleText.text = newControls;
    }

    public void setLoginInfo(string userName)
    {
        loginInfoText.text = "<b>Logged in As:</b>\n" +
                     "<size=18>" + userName + "</size>";

    }


    /// <summary>
    /// Resizes the Puzzle Controls UI window Element
    /// For Reference the default size of the Puzzle Controls UI Window is
    /// width: 500
    /// height: 600
    /// the UI element is also automatically moved to offset the size changes
    /// </summary>
    /// <param name="width">new input width size</param>
    /// <param name="height">new input height size</param>
    public void resizePuzzleControls(float width, float height)
    {
        float xDelta = width / basicControls.GetComponent<RectTransform>().rect.width;
        float yDelta = height / basicControls.GetComponent<RectTransform>().rect.height;
        float xDifference = width - basicControls.GetComponent<RectTransform>().rect.width;

        float yOffset = basicControls.Find("Title").GetComponent<RectTransform>().rect.y * (yDelta - 1);

        basicControls.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        basicControls.localPosition += new Vector3(xDifference / 2, 0, 0);
        basicControls.Find("Text").GetComponent<RectTransform>().sizeDelta *= new Vector2(xDelta, yDelta);
        basicControls.Find("Title").transform.localPosition += new Vector3(0, yOffset, 0);
    }
}
