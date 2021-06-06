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
    [SerializeField] private GameObject soundControlObject;
    [SerializeField] private GameObject graphicControlObject;
    [SerializeField] private Dropdown screenMode;

    private void Start()
    {
        settingObj.SetActive(false);
        graphicControlObject.SetActive(false);
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
        soundControlObject.SetActive(false);
        graphicControlObject.SetActive(false);
    }

    public void SoundObjectOnOff()
    {
        soundControlObject.SetActive(!soundControlObject.activeSelf);
    }

    public void GraphicObjectOnOff()
    {
        graphicControlObject.SetActive(!graphicControlObject.activeSelf);
    }

    public void SetResolution()
    {
        // 1: Windowed, 2: FullScreen, 3: FullScreen Window        
        ResolutionManager.Instance.SetScreenMode(screenMode.value);
    }

    public void LogOut()
    {
        NetworkManager.Instance.Disconnect();
        settingObj.SetActive(false);
        soundControlObject.SetActive(false);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
