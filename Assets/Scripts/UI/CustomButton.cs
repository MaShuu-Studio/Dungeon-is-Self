using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;

public class CustomButton : MonoBehaviour
{
    public enum ButtonMethod { SceneMovement = 0, GameReady, GamePlayReady };

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
                button.onClick.AddListener(SetUserType);
                button.onClick.AddListener(ChangeScene);
                break;
            case ButtonMethod.GamePlayReady:
                button.onClick.AddListener(GamePlayReady);
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

    void SetUserType()
    {
        // 실제 게임에서는 서버에서 매칭 관련 정보를 넘겨주기 때문에 필요 X할듯
        GameController.Instance.SetUserType(userType);
    }

    void GameReadyEnd()
    {
        GamePlayUIController gamePlayUI = GameObject.FindWithTag("UI").GetComponent<GamePlayUIController>();
        if (GameController.Instance.userType == UserType.Defender)
        {
            if (DefenderController.Instance.CheckCadndidate())
            {

            }
            else
            {
                gamePlayUI.Alert(10);
                return;
            }
        }
        else
        {
            if (OffenderController.Instance.CheckCadndidate())
            {

            }
            else
            {
                gamePlayUI.Alert(10);
                return;
            }
        }

        // 후보 세팅이 끝났다고 패킷 전송
        GameController.Instance.StartGame();
    }
    void RoundReadyEnd()
    {
            DefenderController.Instance.SetRoster();
            OffenderController.Instance.SetRoster();
            
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
}
