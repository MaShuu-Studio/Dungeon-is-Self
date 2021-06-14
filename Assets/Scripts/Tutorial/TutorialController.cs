using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControl;
using Data;
using Network;
using System.Linq;

public class TutorialController : MonoBehaviour
{
    #region Instance
    private static TutorialController instance;
    public static TutorialController Instance
    {
        get
        {
            var obj = FindObjectOfType<TutorialController>();
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
    #endregion

    public void StartTutorial()
    {
        SceneController.Instance.ChangeScene("Tutorial");
    }

    public void EndTutorial(bool allStop)
    {
        DefenderController.Instance.Reset();
        OffenderController.Instance.Reset();

        if (allStop)
        {
            string moveScene = "Main";
            if (NetworkManager.Instance.PlayerId == "")
                moveScene = "Title";

            GameController.Instance.SetTutorial(false);
            SceneController.Instance.ChangeScene(moveScene);
        }
    }
}
