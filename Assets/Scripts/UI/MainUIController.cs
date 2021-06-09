using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using System.Text.RegularExpressions;

public class MainUIController : MonoBehaviour
{
    #region Instance
    private static MainUIController instance;
    public static MainUIController Instance
    {
        get
        {
            var obj = FindObjectOfType<MainUIController>();
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
    }
    #endregion


    [SerializeField] private Text curUserText;
    [SerializeField] private Text chattingText;

    [Header("User Info")]
    [SerializeField] private GameObject userInfoObject;
    [SerializeField] private Text userName;
    [SerializeField] private RectTransform contents;

    [Header("Private Room")]
    [SerializeField] private InputField privateRoomInputCode;
    [SerializeField] private GameObject privateRoomGameObject;
    [SerializeField] private Text privateRoomCodeText;
    [SerializeField] private List<Text> privateRoomUsersNameTexts;
    [SerializeField] private List<Text> privateRoomUsersIdTexts;
    [SerializeField] private List<Toggle> privateRoomReadyStates;
    [SerializeField] private Button StartButton;

    public string roomCode { get; set; } = "";
    private int userIndex = 0;

    private void Start()
    {
        privateRoomGameObject.SetActive(false);
        userInfoObject.SetActive(false);
    }
    private void Update()
    {
        SetConnectingUserInfo();
    }

    public void AdjustInputField(string str)
    {
        str = Regex.Replace(str, "0123456789", "");
        privateRoomInputCode.text = str.ToUpper();
    }

    public void SetConnectingUserInfo()
    {
        curUserText.text =
            $"현재 접속자 수: {NetworkManager.Instance.totalUser}명\n" +
            $"현재 게임 중인 유저 수: {NetworkManager.Instance.playingUser}명\n" +
            $"현재 공격자 매칭인원 : {NetworkManager.Instance.waitOffenderUser}명\n" +
            $"현재 방어자 매칭인원 : {NetworkManager.Instance.waitDefenderUser}명\n";
    }

    #region UserInfo
    public void ShowUserInfo()
    {
        userInfoObject.SetActive(!userInfoObject.activeSelf);

        if (userInfoObject.activeSelf)
        {
            userName.text = "USER NAME: " + NetworkManager.Instance.PlayerName + "\n" +
                "USER ID  : " + NetworkManager.Instance.PlayerId + "\n";

            // 배틀로그
        }
    }

    private void SetContentsSize()
    {
        contents.sizeDelta = new Vector2(contents.sizeDelta.x, contents.childCount * 100);
    }
    #endregion

    #region
    public void MakePrivateRoom()
    {
        NetworkManager.Instance.MakePrivateRoom();
    }

    public void JoinPrivateRoom()
    {
        NetworkManager.Instance.JoinPrivateRoom(roomCode);
    }

    public void ExitPrivateRoom()
    {
        NetworkManager.Instance.ExitPrivateRoom(roomCode);
    }

    public void UpdatePrivateRoom(S_UpdatePrivateRoom packet)
    {
        privateRoomGameObject.SetActive(true);
        roomCode = packet.roomCode;
        privateRoomCodeText.text = roomCode;
        for (int i = 0; i < privateRoomUsersIdTexts.Count; i++)
        {
            if (i >= 2) break;
            string name = "";
            string id = "";
            bool ready = false;
            if (i < packet.users.Count)
            {
                name = packet.users[i].playerName;
                id = packet.users[i].playerId;
                ready = packet.users[i].ready;
            }
            privateRoomUsersNameTexts[i].text = name;
            privateRoomUsersIdTexts[i].text = id;
            privateRoomReadyStates[i].isOn = ready;
            Debug.Log(privateRoomReadyStates[userIndex].isOn);
            if (i < packet.users.Count && packet.users[i].playerId == NetworkManager.Instance.PlayerId) userIndex = i;
        }
    }

    public void StartPrivateRoom()
    {
        NetworkManager.Instance.StartPrivateRoom(roomCode);
    }

    public void ReadyPrivateRoom()
    {
        privateRoomReadyStates[userIndex].isOn = !privateRoomReadyStates[userIndex].isOn;
        Debug.Log(privateRoomReadyStates[userIndex].isOn);
        NetworkManager.Instance.ReadyPrivateRoom(roomCode, privateRoomReadyStates[userIndex].isOn);
    }

    public void DestroyPrivateRoom()
    {
        roomCode = "";
        privateRoomInputCode.text = "";
        privateRoomGameObject.SetActive(false);
    }
    #endregion

}