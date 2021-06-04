using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Network;
using System.Linq;

public class CustomButton : MonoBehaviour
{
    public enum ButtonMethod { SceneMovement = 0, GameReady, GamePlayReady, GameExit, ConnectServer, MatchRequest, MatchRequestCancel };

    [SerializeField] private string moveScene = "";
    [SerializeField] private UserType userType = UserType.Offender;
    [SerializeField] private ButtonMethod method;
    private Button button;
    private Image image;
    //private IEnumerator coroutine = null;
    private bool isReady = false;

    // Start is called before the first frame update
    void Start()
    {
        isReady = false;
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        if (button == null) return;

        switch (method)
        {
            case ButtonMethod.SceneMovement:
                button.onClick.AddListener(ChangeScene);
                break;
            case ButtonMethod.GameReady:
                button.onClick.AddListener(SingleGameRequest);
                break;
            case ButtonMethod.GamePlayReady:
                button.onClick.AddListener(GamePlayReady);
                break;
            case ButtonMethod.GameExit:
                button.onClick.AddListener(Application.Quit);
                break;
            case ButtonMethod.ConnectServer:
                button.onClick.AddListener(ConnectServer);
                break;
            case ButtonMethod.MatchRequest:
                button.onClick.AddListener(MatchRequest);
                break;
            case ButtonMethod.MatchRequestCancel:
                button.onClick.AddListener(MatchRequestCancel);
                break;
        }
    }

    private void Update()
    {
        if (image != null)
            if (isReady) image.color = Color.gray;
            else image.color = Color.white;
    }

    public void SetButtonInteract(bool b)
    {
        button.interactable = b;
        if (image != null)
            if (b) image.color = Color.white;
            else image.color = Color.gray;
    }

    void ChangeScene()
    {
        SceneController.Instance.ChangeScene(moveScene);
    }

    #region Play Single
    void GamePlayReady()
    {
        if (isReady == false)
        {
            GameProgress progress = GameController.Instance.currentProgress;
            switch (progress)
            {
                case GameProgress.ReadyGame:
                    NetworkManager.Instance.GameReadyEnd(ref isReady);
                    break;

                case GameProgress.ReadyRound:
                    NetworkManager.Instance.RoundReadyEnd();
                    break;

                case GameProgress.PlayRound:
                    NetworkManager.Instance.TurnReadyEnd();
                    break;
            }
        }
        else
        {
            NetworkManager.Instance.ReadyCancel();
        }
        isReady = !isReady;
    }


    public void ResetCancel()
    {
        isReady = false;
    }

    #endregion

    #region Network
    void ConnectServer()
    {
        Join join = FindObjectOfType<Join>();
        if (join == null) return;
        if(join.ClickStart())
        {
            string pid = UnityEngine.Random.Range(0, 999999).ToString();
            NetworkManager.Instance.ConnectToServer("", pid);
            SetButtonInteract(false);
            StartCoroutine(Connecting());
        }
        else
        {

            
        }
    }

    IEnumerator Connecting()
    {
        float time = 2.0f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        SetButtonInteract(true);
    }

    void MatchRequest()
    {
        NetworkManager.Instance.MatchRequest(userType);
        GameController.Instance.SetUserType(userType);
    }
    void SingleGameRequest()
    {
        NetworkManager.Instance.SingleGameRequest(userType);
        GameController.Instance.SetUserType(userType);
    }
    void MatchRequestCancel()
    {
        NetworkManager.Instance.MatchRequestCancel(GameController.Instance.userType);
    }
    #endregion
}
