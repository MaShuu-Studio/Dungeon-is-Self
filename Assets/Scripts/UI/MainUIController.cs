using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private Text curUserText;
    [SerializeField] private Text chattingText;

    private void Update()
    {
        SetUserInfo();
    }
    public void SetUserInfo()
    {
        curUserText.text = 
            $"현재 접속자 수: {NetworkManager.Instance.totalUser}명\n" +
            $"현재 게임 중인 유저 수: {NetworkManager.Instance.playingUser}명\n" +
            $"현재 공격자 매칭인원 : {NetworkManager.Instance.waitOffenderUser}명\n" +
            $"현재 방어자 매칭인원 : {NetworkManager.Instance.waitDefenderUser}명\n";
    }
}
