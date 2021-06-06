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

    private void Start()
    {
        settingObj.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            UIObjectOnOff();
        }
    }

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject settingObj;
    [SerializeField] private GameObject soundControlObject;

    public void SetCanvasCamera()
    {
        canvas.worldCamera = Camera.main;
    }

    public void UIObjectOnOff()
    {
        settingObj.SetActive(!settingObj.activeSelf);
        soundControlObject.SetActive(false);
    }

    public void SoundObjectOnOff()
    {
        soundControlObject.SetActive(!soundControlObject.activeSelf);
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
