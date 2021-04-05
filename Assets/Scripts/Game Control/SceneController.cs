using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
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
    }

    public static void ChangeScene(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    public static void AddScene(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }
}
