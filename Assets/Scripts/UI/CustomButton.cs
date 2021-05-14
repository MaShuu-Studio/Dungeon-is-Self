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
    public enum ButtonMethod { SceneMovement = 0, GameReady, GamePlayReady, GameExit, ConnectServer, MatchRequest};

    [SerializeField] private string moveScene = "";
    [SerializeField] private UserType userType = UserType.Offender;
    [SerializeField] private ButtonMethod method;
    private Button button;
    private Image image;
    private bool isOn = false;
    private IEnumerator coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
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
                button.onClick.AddListener(ChangeScene);
                break;
            case ButtonMethod.GamePlayReady:
                button.onClick.AddListener(GamePlayReady);
                break;
            case ButtonMethod.GameExit:
                button.onClick.AddListener(Application.Quit);
                break;
            case ButtonMethod.ConnectServer:
                button.onClick.AddListener(ConnectServer);
                button.onClick.AddListener(ChangeScene);
                break;
            case ButtonMethod.MatchRequest:
                button.onClick.AddListener(MatchRequest);
                break;
        }
    }

    public void SetButtonInteract(bool b)
    {
        button.interactable = b;
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
        GameProgress progress = GameController.Instance.currentProgress;
        switch (progress)
        {
            case GameProgress.ReadyGame:
                GameReadyEnd();
                break;

            case GameProgress.ReadyRound:
                RoundReadyEnd();
                break;

            case GameProgress.PlayRound:
                TurnReadyEnd();
                break;
        }
    }

    void GameReadyEnd()
    {
        if (GameController.Instance.userType == UserType.Defender)
        {
            if (DefenderController.Instance.CheckCadndidate())
            {
                C_ReadyGame packet = new C_ReadyGame();
                packet.roomId = GameController.Instance.roomId;
                packet.playerType = (ushort)GameController.Instance.userType;
                packet.candidates = DefenderController.Instance.selectedMonsterCandidates.ToList();

                NetworkManager.Instance.Send(packet.Write());
            }
            else
            {
                GamePlayUIController.Instance.Alert(10);
                return;
            }
        }
        else
        {
            if (OffenderController.Instance.CheckCadndidate())
            {
                C_ReadyGame packet = new C_ReadyGame();
                packet.roomId = GameController.Instance.roomId;
                packet.playerType = (ushort)GameController.Instance.userType;
                packet.candidates = OffenderController.Instance.selectedCharacterCandidates.ToList();

                NetworkManager.Instance.Send(packet.Write());
            }
            else
            {
                GamePlayUIController.Instance.Alert(10);
                return;
            }
        }
    }

    void RoundReadyEnd()
    {
        if (GameController.Instance.userType == UserType.Defender) DefenderController.Instance.SetRoster();
        else OffenderController.Instance.SetRoster();
            
        // 로스터 세팅이 끝났다고 패킷 전송
        GameController.Instance.StartRound();
    }

    void TurnReadyEnd()
    {
        if (coroutine != null) StopCoroutine(coroutine);

        if (GameController.Instance.progressRound == false)
        {
            isOn = !isOn;
            GameController.Instance.ReadyTurn(GameController.Instance.userType, isOn);
            coroutine = TurnProgress();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator TurnProgress()
    {
        while (GameController.Instance.progressRound == false) yield return null;
        while (GameController.Instance.progressRound) yield return null;

        isOn = false;
    }
    #endregion

    #region Server
    void ConnectServer()
    {
        NetworkManager.Instance.ConnectToServer();
    }

    void MatchRequest()
    {
        NetworkManager.Instance.MatchRequest(userType);
    }
    void SingleGameRequest()
    {
        NetworkManager.Instance.SingleGameRequest(userType);
    }
    #endregion
}
