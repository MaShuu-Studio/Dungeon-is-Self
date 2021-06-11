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

    [Header("User Info")]
    [SerializeField] private GameObject userInfoObject;
    [SerializeField] private Text userName;
    [SerializeField] private RectTransform contents;
    [SerializeField] private GameObject battleResultPrefab;

    [Header("Chat")]
    [SerializeField] private Text chatContents;
    [SerializeField] private InputField chatInput;
    [SerializeField] private Animator chatAnimator;
    [SerializeField] private Image chatButtonImage;
    [SerializeField] private List<Sprite> chatButtonSprites;
    private List<string> chatList = new List<string>();
    int chatSpriteIndex = 0;

    [Header("Private Room")]
    [SerializeField] private InputField privateRoomInputCode;
    [SerializeField] private GameObject privateRoomGameObject;
    [SerializeField] private Text privateRoomCodeText;
    [SerializeField] private List<Text> privateRoomUsersNameTexts;
    [SerializeField] private List<Text> privateRoomUsersIdTexts;
    [SerializeField] private List<Toggle> privateRoomReadyStates;
    [SerializeField] private RectTransform readyButton;
    [SerializeField] private RectTransform startButton;

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

    public void ResetScene()
    {
        privateRoomGameObject.SetActive(false);
        userInfoObject.SetActive(false);
        chatList.Clear();
        chatContents.text = "";
        chatInput.text = "";
        privateRoomInputCode.text = "";
    }

    public void AdjustInputField(string str)
    {
        Debug.Log(str);
        str = Regex.Replace(str, "0123456789", "");
        Debug.Log(str);
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

        for (int i = 0; i< contents.transform.childCount; i++)
        {
            Destroy(contents.GetChild(i).gameObject);
        }
        

        if (userInfoObject.activeSelf)
        {
            userName.text = "USER NAME: " + NetworkManager.Instance.PlayerName + "\n" +
                "USER ID  : " + NetworkManager.Instance.PlayerId + "\n";

            // 배틀로그
            string log = NetworkManager.Instance.GetUserBattleInfo();

            if (log != "false")
            {
                BattleResult[] results = HTTPRequestController.DesrializeObject<BattleResult[]>(log);

                for (int i = 0; i < results.Length; i++)
                {
                    GameObject resultObj = Instantiate(battleResultPrefab);
                    resultObj.transform.SetParent(contents);
                    resultObj.transform.localScale = new Vector3(1, 1, 1);
                    BattleResultUI resultUI = resultObj.GetComponent<BattleResultUI>();
                    resultUI.SetResult(results[i]);
                }

                SetContentsSize();
            }
            else
            {
            }
        }
    }

    private void SetContentsSize()
    {
        contents.sizeDelta = new Vector2(contents.sizeDelta.x, contents.childCount * 250);
    }
    #endregion

    #region Chat
    public void SendChat(bool isButton)
    {
        if (chatInput.text == "") return;

        if (Input.GetButtonDown("Submit") || isButton)
        {
            NetworkManager.Instance.SendChat(chatInput.text);
            chatInput.text = "";
            chatInput.Select();
        }
    }

    public void ShowChat(string name, string chat)
    {
        string str = name + ": " + chat;
        this.chatList.Add(str);

        if (chatList.Count > 15) chatList.RemoveAt(0);

        chatContents.text = "";

        for(int i = 0; i < chatList.Count; i++)
        {
            chatContents.text += chatList[i];
            if (i != chatList.Count - 1) chatContents.text += "\n";
        }
    }

    public void ChatAnimate()
    {
        chatAnimator.SetTrigger("Animation");
        chatButtonImage.sprite = chatButtonSprites[(++chatSpriteIndex) % 2];
    }

    #endregion

    #region Private Room
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

            if (i < packet.users.Count && packet.users[i].playerId == NetworkManager.Instance.PlayerId)
            {
                userIndex = i;
                if (userIndex != 0)
                {
                    readyButton.pivot = new Vector2(0.5f, 1);
                    readyButton.anchorMin = new Vector2(0.5f, 1);
                    readyButton.anchorMax = new Vector2(0.5f, 1);
                    readyButton.anchoredPosition = new Vector2(0, 0);
                    startButton.gameObject.SetActive(false);
                }
                else
                {
                    readyButton.pivot = new Vector2(0f, 1);
                    readyButton.anchorMin = new Vector2(0, 1);
                    readyButton.anchorMax = new Vector2(0, 1);
                    readyButton.anchoredPosition = new Vector2(0, 0);
                    startButton.gameObject.SetActive(true);
                }
            }
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