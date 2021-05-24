﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using ServerCore;
using System;
using GameControl;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        const int PORT_NUMBER = 7777;
        static ServerSession session = new ServerSession();
        private int serverCount = 0;

        #region Instance
        private static NetworkManager instance;
        public static NetworkManager Instance
        {
            get
            {
                var obj = FindObjectOfType<NetworkManager>();
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

        private int playerId;
        public int totalUser { get; private set; } = 0;
        public int playingUser { get; private set; } = 0;
        public int waitDefenderUser { get; private set; } = 0;
        public int waitOffenderUser { get; private set; } = 0;
        IEnumerator checkCoroutine = null;

        // Update is called once per frame
        void Update()
        {
            // 게임 쓰레드에서 Pop하여 작동하는 부분.
            List<IPacket> packets = PacketQueue.Instance.PopAll();
            foreach (IPacket packet in packets)
            {
                PacketManager.Instance.HandlePacket(session, packet);
            }
            packets.Clear();
        }

        public void ConnectToServer()
        {
            Debug.Log("Try Connect to server");
            string host;
            IPHostEntry ipHost;
            IPAddress ipAddr;
            IPEndPoint endPoint;

            Connector connector;

            #region Local Test
            host = Dns.GetHostName();
            ipHost = Dns.GetHostEntry(host);
            ipAddr = ipHost.AddressList[0];
            endPoint = new IPEndPoint(ipAddr, PORT_NUMBER);

            connector = new Connector();

            connector.Connect(endPoint, () => { return session; }, 1);
            #endregion

            
            /*
            #region Live
            host = "ec2-3-36-132-112.ap-northeast-2.compute.amazonaws.com";
            ipHost = Dns.GetHostEntry(host);
            ipAddr = ipHost.AddressList[0];
            endPoint = new IPEndPoint(ipAddr, PORT_NUMBER);

            connector = new Connector();

            connector.Connect(endPoint, () => { return session; }, 1);
            #endregion
            */

        }

        public void Send(ArraySegment<byte> segment)
        {
            session.Send(segment);
        }

        public void SetPlayerId(int id)
        {
            Debug.Log("Connect Complete");
            playerId = id;
            SceneController.Instance.ChangeScene("Main");
            serverCount = 0;
            if (checkCoroutine != null)
            {
                StopCoroutine(checkCoroutine);
            }
            checkCoroutine = CheckPacket();
            StartCoroutine(checkCoroutine);
        }

        IEnumerator CheckPacket()
        {
            while (true)
            {
                Send(new C_CheckConnect() { playerId = playerId }.Write());
                serverCount++;
                if (serverCount > 5) break;
                yield return new WaitForSeconds(1f);
            }

            try
            {
                session.Disconnect();
            }
            catch (Exception e)
            {
                Debug.Log("Exception " + e);
            }
            finally
            {
                SceneController.Instance.ChangeScene("Title");
                session = new ServerSession();
            }
        }

        public void UpdateServer()
        {
            serverCount = 0;
        }

        public void SetUserInfo(int totalUser, int playingUser, int waitDefUser, int waitOffUser)
        {
            this.totalUser = totalUser;
            this.playingUser = playingUser;
            this.waitDefenderUser = waitDefUser;
            this.waitOffenderUser = waitOffUser;
        }

        public void MatchRequest(UserType type)
        {
            C_MatchRequest matchPacket = new C_MatchRequest();
            matchPacket.playerId = playerId;
            matchPacket.playerType = (ushort)type;

            Send(matchPacket.Write());
        }

        public void SingleGameRequest(UserType type)
        {
            C_SingleGameRequest singleGamePacket = new C_SingleGameRequest();
            singleGamePacket.playerId = playerId;
            singleGamePacket.playerType = (ushort)type;

            Send(singleGamePacket.Write());
        }

        public void MatchRequestCancel(UserType type)
        {
            C_MatchRequestCancel matchCancelPacket = new C_MatchRequestCancel();
            matchCancelPacket.playerId = playerId;
            matchCancelPacket.playerType = (ushort)type;

            Send(matchCancelPacket.Write());
        }
        public void GameEnd(int roomId)
        {
            C_GameEnd packet = new C_GameEnd
            {
                type = (ushort)GameController.Instance.userType,
                roomId = roomId
            };

            Send(packet.Write());

            SceneController.Instance.ChangeScene("Main");
        }

        public void RoundEnd(int roomId)
        {
            C_RoundEnd packet = new C_RoundEnd
            {
                type = (ushort)GameController.Instance.userType,
                roomId = roomId
            };

            Send(packet.Write());
        }

        private void OnApplicationQuit()
        {
            C_LeaveGame p = new C_LeaveGame();
            p.playerId = playerId;
            session.Send(p.Write());
        }
    }
}
