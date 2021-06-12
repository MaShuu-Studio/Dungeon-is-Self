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
        GameController.Instance.SetTutorial(true);
        GameController.Instance.StartGame("tutorial", UserType.Offender);
    }

    public void StartGame(UserType type)
    {
    }
    
    public void ReadyGame()
    {
    }

    public void ReadyGameEnd(List<int> enemyCandidats)
    {

    }

    public void ReadyRound(int round)
    {
    }
    public void StartRound(int round, List<S_RoundReadyEnd.EnemyRoster> enemys)
    {
    }
    public void ProgressTurn(int round, int turn, S_ProgressTurn packet)
    {
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
