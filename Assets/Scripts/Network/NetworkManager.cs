using System.Collections;
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
        private bool isConnected = false;

        // Update is called once per frame
        void Update()
        {
            if (isConnected)
            {
                // 게임 쓰레드에서 Pop하여 작동하는 부분.
                List<IPacket> packets = PacketQueue.Instance.PopAll();
                foreach (IPacket packet in packets)
                {
                    PacketManager.Instance.HandlePacket(session, packet);
                }
                packets.Clear();
            }
        }

        public void ConnectToServer()
        {
            Debug.Log("Connect");
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, PORT_NUMBER);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return session; }, 1);

            isConnected = true;
        }

        public void Send(ArraySegment<byte> segment)
        {
            session.Send(segment);
        }

        public void SetPlayerId(int id)
        {
            playerId = id;
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

        private void OnApplicationQuit()
        {
            C_LeaveGame p = new C_LeaveGame();
            p.playerId = playerId;
            session.Send(p.Write());
        }
    }
}
