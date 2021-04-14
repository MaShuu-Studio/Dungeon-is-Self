using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private string currentScene = "Loading Scene";
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

        StartCoroutine(ChangeScene("Loading Scene", LoadSceneMode.Single));
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
        currentScene = name;
    }
}
