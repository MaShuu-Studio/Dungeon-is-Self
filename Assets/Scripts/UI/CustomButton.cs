using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;

public class CustomButton : MonoBehaviour
{
    public enum ButtonMethod { SceneMovement = 0, GameReady, GameStart, GameReadyEnd, RoundReadyEnd };

    [SerializeField] private string moveScene = "";
    [SerializeField] private UserType userType = UserType.Offender;
    [SerializeField] private ButtonMethod method;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
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
            case ButtonMethod.GameStart:
                button.onClick.AddListener(GameStart);
                break;
            case ButtonMethod.GameReadyEnd:
                button.onClick.AddListener(GameReadyEnd);
                break;
            case ButtonMethod.RoundReadyEnd:
                button.onClick.AddListener(RoundReadyEnd);
                break;
        }
    }

    void ChangeScene()
    {
        SceneController.Instance.ChangeScene(moveScene);
    }
    void SetUserType()
    {
        // 실제 게임에서는 서버에서 매칭 관련 정보를 넘겨주기 때문에 필요 X할듯
        GameController.Instance.SetUserType(userType);
    }
    void GameStart()
    {
        GameController.Instance.StartGame();
    }

    void GameReadyEnd()
    {        
        if (GameController.Instance.userType == UserType.Defender)
        {
            GamePlayUIController gamePlayUI = GameObject.FindWithTag("UI").GetComponent<GamePlayUIController>();
            if (DefenderController.Instance.CheckCadndidate())
            {

            }
            else
            {
                gamePlayUI.Alert();
                return;
            }

        }

        // 후보 세팅이 끝났다고 패킷 전송
        GameController.Instance.ReadyRound();
    }
    void RoundReadyEnd()
    {
        // 로스터 세팅이 끝났다고 패킷 전송
        GameController.Instance.StartRound();
    }
}
