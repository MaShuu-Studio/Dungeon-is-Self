using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;

public class SettingUI : MonoBehaviour
{
    private static SettingUI instance;
    public static SettingUI Instance
    {
        get
        {
            var obj = FindObjectOfType<SettingUI>();
            instance = obj;
            return instance;
        }
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject settingObj;
    [SerializeField] private GameObject blindObject;
    [SerializeField] private GameObject settingControlObject;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Text bgmText;
    [SerializeField] private Text sfxText;
    [SerializeField] private Dropdown screenMode;
    [SerializeField] private Dropdown screenResolutions;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button surrenderButton;
    [SerializeField] private Text logOutButtonText;

    private void Start()
    {
        SetActive(false);
        settingControlObject.SetActive(false);
        int mode = 0;
        switch(Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                mode = 1;
                break;
            case FullScreenMode.FullScreenWindow:
                mode = 2;
                break;
        }
        screenMode.value = mode;
        
        #region for PC
        screenResolutions.options.Clear();

        for (int i = 0; i < ResolutionManager.Instance.SupportedResolutions.Length; i++) 
        {
            Resolution resolution = ResolutionManager.Instance.SupportedResolutions[i];
            screenResolutions.options.Add(new Dropdown.OptionData($"{resolution.width}X{resolution.height}"));
            if (Screen.currentResolution.width == resolution.width)
            {
                screenResolutions.value = i;
            }
        }
        #endregion
        
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            UIObjectOnOff();
        }
    }

    public void SetCanvasCamera()
    {
        canvas.worldCamera = Camera.main;
    }

    private void SetActive(bool b)
    {
        settingObj.SetActive(b);
        blindObject.SetActive(b);
    }

    public void UIObjectOnOff()
    {
        SetActive(!settingObj.activeSelf);

        settingControlObject.SetActive(false);
        if (SceneController.Instance.CurrentScene == "GAMEPLAY" && GameControl.GameController.Instance.isTutorial == false)
        {
            tutorialButton.gameObject.SetActive(false);
            surrenderButton.gameObject.SetActive(true);
        }
        else
        {
            if (NetworkManager.Instance.PlayerId != "")
                logOutButtonText.text = "LOGOUT";
            else logOutButtonText.text = "TITLE";

            tutorialButton.gameObject.SetActive(true);
            surrenderButton.gameObject.SetActive(false);
        }
    }

    public void BGMControl()
    {
        SoundController.Instance.SetBGMVolume(bgmSlider.value);
        bgmText.text = ((int)bgmSlider.value).ToString();
    }

    public void SFXControl()
    {
        SoundController.Instance.SetBGMVolume(sfxSlider.value);
        sfxText.text = ((int)sfxSlider.value).ToString();
    }

    public void SettingObjectOnOff()
    {
        settingControlObject.SetActive(!settingControlObject.activeSelf);
    }

    public void SetResolution()
    {
        // 1: Windowed, 2: FullScreen, 3: FullScreen Window
        ResolutionManager.Instance.SetScreenMode(screenMode.value);
        ResolutionManager.Instance.SetResolution(screenResolutions.value);

    }

    public void Tutorial()
    {
        TutorialController.Instance.StartTutorial();
        SetActive(false);
    }

    public void Surrender()
    {
        NetworkManager.Instance.Surrender(GameControl.GameController.Instance.roomId);
        SetActive(false);
    }

    public void LogOut()
    {
        NetworkManager.Instance.Disconnect();
        SetActive(false);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
