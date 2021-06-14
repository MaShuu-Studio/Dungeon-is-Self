using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using ServerCore;
using System;
using GameControl;
using System.Linq;

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

        private string playerId = "";
        private string playerName = "";
        private bool connectRequest = false;

        public string PlayerId { get { return playerId; } }
        public string PlayerName { get { return playerName; } }
        public bool ConnectRequest { get { return connectRequest; } }
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

        public void Send(ArraySegment<byte> segment)
        {
            session.Send(segment);
        }

        #region Connect
        IEnumerator connecting = null;
        public void ConnectToServer(string token, string pid)
        {
            connectRequest = true;

            Debug.Log("Try Connect to server");
            string host;
            IPHostEntry ipHost;
            IPAddress ipAddr;
            IPEndPoint endPoint;

            Connector connector = new Connector();
            
            #region Local Test
            host = Dns.GetHostName();
            ipHost = Dns.GetHostEntry(host);
            ipAddr = ipHost.AddressList[0];
            endPoint = new IPEndPoint(ipAddr, PORT_NUMBER);

            connector.Connect(endPoint, () => { return session; }, 1);
            #endregion
            /*
            #region Live
            host = "ec2-13-124-208-197.ap-northeast-2.compute.amazonaws.com";
            ipHost = Dns.GetHostEntry(host);
            ipAddr = ipHost.AddressList[0];
            endPoint = new IPEndPoint(ipAddr, PORT_NUMBER);
            connector.Connect(endPoint, () => { return session; }, 1);
            #endregion
            */
            if (connecting != null)
            {
                StopCoroutine(connecting);
            }
            connecting = WaitConnecting(token, pid, connector);
            StartCoroutine(connecting);
        }

        IEnumerator WaitConnecting(string token, string pid, Connector connector)
        {
            while (session.Connected == false && connector.connectionFailed == false) yield return null;

            if (connector.connectionFailed)
            {
                connectRequest = false;
            }
            else
            {
                C_EnterGame packet = new C_EnterGame();
                packet.token = token;
                packet.playerId = pid;
                Send(packet.Write());
            }
        }

        public void SetPlayerId(string id)
        {
            Debug.Log("Connect Complete");
            connectRequest = false;
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
                if (serverCount > 6) break;
                yield return new WaitForSeconds(1f);
            }

            Disconnect();
        }

        public void UpdateServer()
        {
            serverCount = 0;
        }
        public void SetUserInfo(string userId, string userName)
        {
            this.playerId = userId;
            this.playerName = userName;
        }

        public void SetConnectingUserInfo(int totalUser, int playingUser, int waitDefUser, int waitOffUser)
        {
            this.totalUser = totalUser;
            this.playingUser = playingUser;
            this.waitDefenderUser = waitDefUser;
            this.waitOffenderUser = waitOffUser;
        }

        #endregion

        public void SendChat(string chat)
        {
            C_Chat packet = new C_Chat();
            packet.playerId = playerId;
            packet.playerName = playerName;
            packet.chat = chat;
            Send(packet.Write());
        }

        public string GetUserBattleInfo()
        {
            string url = "http://ec2-13-209-42-66.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/battlereport/find/" + playerId;
            return HTTPRequestController.Instance.SendHTTPGet(url);
        }

        #region Match
        public void MatchRequest(UserType type)
        {
            C_MatchRequest matchPacket = new C_MatchRequest();
            matchPacket.playerId = playerId;
            matchPacket.playerType = (ushort)type;

            Send(matchPacket.Write());
        }

        /*
        public void SingleGameRequest(UserType type)
        {
            C_SingleGameRequest singleGamePacket = new C_SingleGameRequest();
            singleGamePacket.playerId = playerId;
            singleGamePacket.playerType = (ushort)type;

            Send(singleGamePacket.Write());
        }
        */

        public void MatchRequestCancel(UserType type)
        {
            C_MatchRequestCancel matchCancelPacket = new C_MatchRequestCancel();
            matchCancelPacket.playerId = playerId;
            matchCancelPacket.playerType = (ushort)type;

            Send(matchCancelPacket.Write());
        }
        #endregion

        #region Private Room
        public void MakePrivateRoom()
        {
            C_MakePrivateRoom packet = new C_MakePrivateRoom();
            packet.playerId = playerId;
            packet.playerName = playerName;
            Send(packet.Write());
        }

        public void JoinPrivateRoom(string roomCode)
        {
            C_JoinPrivateRoom packet = new C_JoinPrivateRoom();
            packet.playerId = playerId;
            packet.playerName = playerName;
            packet.roomCode = roomCode;
            Send(packet.Write());
        }

        public void ChangeTypePrivateRoom(string roomCode, ushort index, ushort type)
        {
            C_ChangeTypePrivateRoom packet = new C_ChangeTypePrivateRoom();
            packet.playerId = playerId;
            packet.roomCode = roomCode;
            packet.index = index;
            packet.type = (ushort)type;
            Send(packet.Write());
        }

        public void ReadyPrivateRoom(string roomCode, bool ready)
        {
            C_ReadyPrivateRoom packet = new C_ReadyPrivateRoom();
            packet.playerId = playerId;
            packet.roomCode = roomCode;
            packet.ready = ready;
            Send(packet.Write());
        }

        public void StartPrivateRoom(string roomCode)
        {
            C_StartPrivateRoom packet = new C_StartPrivateRoom();
            packet.roomCode = roomCode;
            Send(packet.Write());
        }

        public void ExitPrivateRoom(string roomCode)
        {
            C_ExitPrivateRoom packet = new C_ExitPrivateRoom();
            packet.roomCode = roomCode;
            packet.playerId = playerId;
            Send(packet.Write());
        }
        #endregion

        #region Play Game
        public void GameReadyEnd(ref bool isReady)
        {
            if (GameController.Instance.userType == UserType.Defender) DefenderController.Instance.CandidatesTimeOut();
            else OffenderController.Instance.CandidatesTimeOut();

            GamePlayUIController.Instance.ShowAllSelectedCandidates();
            bool isReadyEnd = (GameController.Instance.userType == UserType.Defender) ? (DefenderController.Instance.CheckCadndidate()) : (OffenderController.Instance.CheckCadndidate());

            if (isReadyEnd)
            {
                C_ReadyGame packet = new C_ReadyGame();
                packet.roomId = GameController.Instance.roomId;
                packet.playerType = (ushort)GameController.Instance.userType;

                if (GameController.Instance.userType == UserType.Defender)
                    packet.candidates = DefenderController.Instance.selectedMonsterCandidates.ToList();
                else
                    packet.candidates = OffenderController.Instance.selectedCharacterCandidates.ToList();

                Send(packet.Write());
            }
            else
            {
                GamePlayUIController.Instance.Alert(10);
                isReady = true;
            }
        }

        public void RoundReadyEnd()
        {
            List<int> roster = new List<int>();
            List<List<int>> skillRoster = new List<List<int>>();
            int attackSkill = 0;
            // 로스터 추가

            if (GameController.Instance.userType == UserType.Defender)
            {
                DefenderController.Instance.RosterTimeOut();
                roster.Add(DefenderController.Instance.monsterRoster);
                for (int i = 0; i < roster.Count; i++)
                {
                    skillRoster.Add(DefenderController.Instance.GetSkillRosterWithUnit(roster[i]));
                    attackSkill = DefenderController.Instance.GetAttackSkillWithUnit(roster[i]).id;
                }
            }
            else
            {
                OffenderController.Instance.RosterTimeOut();
                for (int i = 0; i < OffenderController.Instance.roster.Length; i++)
                {
                    roster.Add(OffenderController.Instance.roster[i]);
                }
                for (int i = 0; i < roster.Count; i++)
                {
                    skillRoster.Add(OffenderController.Instance.GetSkillRosterWithUnit(roster[i]));
                }
            }

            GamePlayUIController.Instance.ShowCharacterSkillsInPanel();
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
            Send(packet.Write());
        }

        public void TurnReadyEnd()
        {
            List<List<int>> dices = new List<List<int>>();
            // 로스터 추가

            if (GameController.Instance.userType == UserType.Defender)
            {
                DefenderController.Instance.DiceTimeOut();
                dices.Add(DefenderController.Instance.GetDicesWithUnit(DefenderController.Instance.monsterRoster));
            }
            else
            {
                OffenderController.Instance.DiceTimeOut();
                for (int i = 0; i < OffenderController.Instance.roster.Length; i++)
                {
                    dices.Add(OffenderController.Instance.GetDicesWithUnit(OffenderController.Instance.roster[i]));
                }
            }
            GamePlayUIController.Instance.ShowCharacterSkillsInPanel();

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

            Send(packet.Write());
        }

        public void ReadyCancel()
        {
            C_ReadyCancel packet = new C_ReadyCancel();
            packet.roomId = GameController.Instance.roomId;
            packet.userType = (ushort)GameController.Instance.userType;
            Send(packet.Write());
        }

        public void RoundEnd(string roomId)
        {
            C_RoundEnd packet = new C_RoundEnd
            {
                type = (ushort)GameController.Instance.userType,
                roomId = roomId
            };

            Send(packet.Write());
        }

        public void GameEnd(string roomId)
        {
            C_GameEnd packet = new C_GameEnd
            {
                type = (ushort)GameController.Instance.userType,
                roomId = roomId
            };

            Send(packet.Write());

            SceneController.Instance.ChangeScene("Main");
        }

        public void Surrender(string roomId)
        {
            C_Surrender packet = new C_Surrender()
            {
                roomId = roomId,
                playerId = playerId
            };

            Send(packet.Write());
        }
        #endregion

        public void Disconnect()
        {
            try
            {
                C_LeaveGame p = new C_LeaveGame();
                p.playerId = playerId;
                p.roomId = GameController.Instance.roomId;
                session.Send(p.Write());
                session.Disconnect();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            finally
            {
                connectRequest = false;
                playerId = "";
                playerName = "";
                session = new ServerSession();
                SceneController.Instance.ChangeScene("Title");
                StopAllCoroutines();
            }
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}
