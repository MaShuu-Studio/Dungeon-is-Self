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
    [SerializeField] private GameObject settingControlObject;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Text bgmText;
    [SerializeField] private Text sfxText;
    [SerializeField] private Dropdown screenMode;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button surrenderButton;

    private void Start()
    {
        settingObj.SetActive(false);
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

    public void UIObjectOnOff()
    {
        settingObj.SetActive(!settingObj.activeSelf);
        settingControlObject.SetActive(false);
        if (SceneController.Instance.CurrentScene == "GAMEPLAY")
        {
            tutorialButton.gameObject.SetActive(false);
            surrenderButton.gameObject.SetActive(true);
        }
        else
        {
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
    }

    public void Tutorial()
    {
        settingObj.SetActive(false);
    }

    public void Surrender()
    {
        NetworkManager.Instance.Surrender(GameControl.GameController.Instance.roomId);
        settingObj.SetActive(false);
    }

    public void LogOut()
    {
        NetworkManager.Instance.Disconnect();
        settingObj.SetActive(false);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
