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
    public enum ButtonMethod { SceneMovement = 0, GameReady, GamePlayReady, GameExit, ConnectServer, MatchRequest, MatchRequestCancel};

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
        List<int> roster = new List<int>();
        List<List<int>> skillRoster = new List<List<int>>();
        int attackSkill = 0;
        // 로스터 추가

        if (GameController.Instance.userType == UserType.Defender)
        {
            roster.Add(DefenderController.Instance.monsterRoster);
            for (int i = 0; i < roster.Count; i++)
            {
                skillRoster.Add(DefenderController.Instance.GetSkillRosterWithUnit(roster[i]));
                attackSkill = DefenderController.Instance.GetAttackSkillWithUnit(roster[i]).id;
            }
        }
        else
        {
            for (int i = 0; i < OffenderController.Instance.roster.Length; i++)
            {
                roster.Add(OffenderController.Instance.roster[i]);
            }
            for (int i = 0; i < roster.Count; i++)
            {
                skillRoster.Add(OffenderController.Instance.GetSkillRosterWithUnit(roster[i]));
            }
        }

        C_RoundReady packet = new C_RoundReady();
        packet.roomId = GameController.Instance.roomId;
        packet.playerType = (ushort)GameController.Instance.userType;
        packet.rosters = new List<C_RoundReady.Roster>();
        for (int i = 0; i < roster.Count; i++)
        {
            packet.rosters.Add(
                new C_RoundReady.Roster()
                {
                    unitIndex = roster[i],
                    attackSkill = attackSkill,
                    skillRosters = skillRoster[i]
                });
        }

        // 로스터 세팅이 끝났다고 패킷 전송
        NetworkManager.Instance.Send(packet.Write());
    }

    void TurnReadyEnd()
    {        
        List<List<int>> dices = new List<List<int>>();
        // 로스터 추가

        if (GameController.Instance.userType == UserType.Defender)
        {
            dices.Add(DefenderController.Instance.GetDicesWithUnit(DefenderController.Instance.monsterRoster));
        }
        else
        {
            for (int i = 0; i < OffenderController.Instance.roster.Length; i++)
            {
                dices.Add(OffenderController.Instance.GetDicesWithUnit(OffenderController.Instance.roster[i]));
            }
        }

        C_PlayRoundReady packet = new C_PlayRoundReady();
        packet.roomId = GameController.Instance.roomId;
        packet.playerType = (ushort)GameController.Instance.userType;
        packet.rosters = new List<C_PlayRoundReady.Roster>();
        for (int i = 0; i < dices.Count; i++)
        {
            packet.rosters.Add(
                new C_PlayRoundReady.Roster()
                {
                    unitIndex = i,
                    diceIndexs = dices[i]
                });
        }

        NetworkManager.Instance.Send(packet.Write());
        SetButtonInteract(false);
        /*
        isOn = false;
            
        if (coroutine != null) StopCoroutine(coroutine);

        if (GameController.Instance.progressRound == false)
        {
            isOn = !isOn;
            GameController.Instance.ReadyTurn(GameController.Instance.userType, isOn);
            coroutine = TurnProgress();
            StartCoroutine(coroutine);
        }*/
    }

    IEnumerator TurnProgress()
    {
        while (GameController.Instance.progressRound == false) yield return null;
        while (GameController.Instance.progressRound) yield return null;

        isOn = false;
    }
    #endregion

    #region Network
    void ConnectServer()
    {
        NetworkManager.Instance.ConnectToServer();
        SetButtonInteract(false);
        StartCoroutine(Connecting());
    }

    IEnumerator Connecting()
    {
        float time = 2.0f;

        while(time > 0)
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
