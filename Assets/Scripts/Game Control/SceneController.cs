using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameControl;

public class SceneController : MonoBehaviour
{
    public string CurrentScene { get; private set; } = "LOADING SCENE";
    private static SceneController instance;
    public static SceneController Instance
    {
        get
        {
            var obj = FindObjectOfType<SceneController>();
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

        Resources.LoadAll("");
        StartCoroutine(ChangeScene("Title", LoadSceneMode.Single));
    }

    public void ChangeScene(string name, string mode = "SINGLE")
    {
        LoadSceneMode loadMode = LoadSceneMode.Single;
        if (mode != "SINGLE") loadMode = LoadSceneMode.Additive;

        StartCoroutine(ChangeScene(name, loadMode));
    }

    IEnumerator ChangeScene(string name, LoadSceneMode mode)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name, mode);

        while (async != null && async.isDone == false)
        {
            yield return null;
        }
        CurrentScene = name.ToUpper();
        SettingUI.Instance.SetCanvasCamera();
        if (CurrentScene == "GAMEPLAY")
        {
            GameController.Instance.ReadyGame();
        }else if (CurrentScene == "TITLE" || CurrentScene == "MAIN")
        {
            if (CurrentScene == "MAIN") MainUIController.Instance.ResetScene();
            SoundController.Instance.PlayBGM("TITLE");
        }
    }
}
