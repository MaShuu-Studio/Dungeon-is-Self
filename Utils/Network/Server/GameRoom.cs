using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ServerCore;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    public struct PrivateRoomUserInfo
    {
        public string userName;
        public bool readyState;
    }
    class GameRoom : IJobQueue
    {
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        List<Tuple<string, ArraySegment<byte>>> _packetList = new List<Tuple<string, ArraySegment<byte>>>();

        int _roomNumber = 1;

        Dictionary<string, ClientSession> _sessions = new Dictionary<string, ClientSession>();
        Dictionary<string, int> _sessionCount = new Dictionary<string, int>();
        List<string> waitDefenderUserList = new List<string>();
        List<string> waitOffenderUserList = new List<string>();
        Dictionary<string, Dictionary<string, PrivateRoomUserInfo>> privateRooms = new Dictionary<string, Dictionary<string, PrivateRoomUserInfo>>();
        Dictionary<string, PlayRoom> playingRooms = new Dictionary<string, PlayRoom>();
        Dictionary<string, PlayRoom> playingSingleGameRooms = new Dictionary<string, PlayRoom>();

        #region Basic
        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Send(string id, IPacket packet)
        {
            if (_sessions.ContainsKey(id))
                _sessions[id].Send(packet.Write());
        }

        public void CheckSession()
        {
            List<string> keys = _sessionCount.Keys.ToList();
            for (int i = 0; i < _sessionCount.Count; i++)
            {
                _sessionCount[keys[i]]++;
                if (_sessionCount[keys[i]] > 6)
                {
                    Push(() => Leave(keys[i]));
                }
                
            }
        }

        public void UpdateSession(string id)
        {
            if (_sessionCount.ContainsKey(id)) _sessionCount[id] = 0;
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Flush()
        {
            /*
            foreach(Tuple<string, ArraySegment<byte>> packet in _packetList)
                if (_sessions.ContainsKey(packet.Item1))
                    _sessions[packet.Item1].Send(packet.Item2);
            */

            if (_sessions.Count == 0) _pendingList.Clear();
            foreach (ClientSession s in _sessions.Values)
                s.Send(_pendingList);

            //if (_pendingList.Count + _packetList.Count != 0) Console.WriteLine($"Flushed {_pendingList.Count + _packetList.Count} items");
            _pendingList.Clear();
            _packetList.Clear();
        }
        #endregion

        #region Connect
        public void Enter(ClientSession session, string token, string pId)
        {
            string id = RequestJoinClient(token);
            if (_sessions.Keys.Contains(id) == false && id == pId)
            {
                Console.WriteLine($"Enter User : {id}");
                session.Send(new S_GivePlayerId() { playerId = id }.Write());
                session.Room = this;

                // 플레이어 추가
                _sessionCount.Add(id, 0);
                _sessions.Add(id, session);

                UpdateUserInfo();
            }
            else
            {
                session.Send(new S_FailConnect().Write());
                session.Disconnect();
            }
        }
        private string RequestJoinClient(string token)
        {
            string urlString = "http://ec2-54-180-153-249.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/authorization";
            int nStartTime = 0;
            string result = "";
            string strMsg = string.Empty;
            nStartTime = Environment.TickCount;

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                Uri url = new Uri(urlString);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;

                request.Headers.Add("Authorization", "Bearer " + token);
                //request.Headers["Authentication"] = "Bearer " + token;
                request.Timeout = 5000;

                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                response.Close();

                JObject start = JObject.Parse(result);
                result = start["memberCode"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = "false";
            }
            return result;
        }

        public void Leave(string id)
        {
            Console.WriteLine($"Leave User {id}");
            Session session = _sessions[id];

            if (_sessions.ContainsKey(id))
            {
                _sessions.Remove(id);
            }
            if (_sessionCount.ContainsKey(id))
            {
                _sessionCount.Remove(id);
            }

            Push(() => ExitPrivateRoom(id));
            Push(() => PlayingRoomAbnormalExit(id));
            Push(() => MatchRequestCancel(id));

            session.Disconnect();
            // 플레이어 나감
            // 모든 플레이어에게 퇴장을 브로드캐스트
            UpdateUserInfo();
        }

        public void Leave(ClientSession session)
        {
            Console.WriteLine($"Leave User ");
            // 플레이어 나감
            int findIndex = 0;
            List<string> keys = _sessions.Keys.ToList();
            List<ClientSession> values = _sessions.Values.ToList();

            for (; findIndex < keys.Count; findIndex++)
            {
                if (values[findIndex] == session) break;
            }

            if (findIndex >= keys.Count) return;

            string id = keys[findIndex];
            _sessions.Remove(id);
            _sessionCount.Remove(id);

            Push(() => ExitPrivateRoom(id));
            Push(() => PlayingRoomAbnormalExit(id));
            Push(() => MatchRequestCancel(id));
            session.Disconnect();
            // 모든 플레이어에게 퇴장을 브로드캐스트
            UpdateUserInfo();
        }

        private void UpdateUserInfo()
        {
            // 모든 플레이어에게 입장을 브로드캐스트
            S_BroadcastConnectUser broadcast = new S_BroadcastConnectUser();
            broadcast.totalUser = _sessions.Count;
            broadcast.playingUser = playingRooms.Count * 2;
            broadcast.waitDefUser = waitDefenderUserList.Count;
            broadcast.waitOffUser = waitOffenderUserList.Count;

            Push(() => Broadcast(broadcast.Write()));
        }

        #endregion

        #region Match Game
        public void MatchRequest(string playerId, UserType type)
        {
            if (type == UserType.Defender)
                waitDefenderUserList.Add(playerId);
            else
                waitOffenderUserList.Add(playerId);

            if (waitDefenderUserList.Count > 0 && waitOffenderUserList.Count > 0)
            {
                string[] playerIds = new string[2];
                playerIds[(ushort)UserType.Defender] = waitDefenderUserList[0];
                playerIds[(ushort)UserType.Offender] = waitOffenderUserList[0];

                waitDefenderUserList.RemoveAt(0);
                waitOffenderUserList.RemoveAt(0);

                int roomNumber = _roomNumber++;
                string roomId = string.Format("{0:yy}{0:MM}{0:dd}{1:D8}", DateTime.Now, roomNumber);

                PlayRoom playRoom = new PlayRoom(roomId, playerIds, this);
                playingRooms.Add(roomId, playRoom);

                for (ushort i = 0; i < 2; i++)
                {
                    Send(playerIds[i],
                        new S_StartGame()
                        {
                            roomId = roomId,
                            enemyPlayerId = playerIds[i],
                            playerType = i
                        });
                }
            }

            UpdateUserInfo();
        }

        public void SingleGameRequest(string playerId, UserType type)
        {
            int roomNumber = _roomNumber++;
            string roomId = string.Format("{0:yy}{0:MM}{0:dd}{1:D8}", DateTime.Now, roomNumber);

            PlayRoom playRoom = new PlayRoom(roomId, playerId, type, this);
            playingSingleGameRooms.Add(roomId, playRoom);

            S_StartGame packet = new S_StartGame()
            {
                roomId = playRoom.RoomId,
                enemyPlayerId = "",
                playerType = (ushort)type
            };

            Send(playerId, packet);

            UpdateUserInfo();
        }

        public void MatchRequestCancel(string playerId, UserType type)
        {
            if (type == UserType.Defender)
            {
                for (int i = 0; i < waitDefenderUserList.Count; i++)
                {
                    if (waitDefenderUserList[i] == playerId)
                    {
                        waitDefenderUserList.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < waitOffenderUserList.Count; i++)
                {
                    if (waitOffenderUserList[i] == playerId)
                    {
                        waitOffenderUserList.RemoveAt(i);
                        break;
                    }
                }
            }

            UpdateUserInfo();
        }

        public void MatchRequestCancel(string playerId)
        {
            for (int i = 0; i < waitDefenderUserList.Count; i++)
            {
                if (waitDefenderUserList[i] == playerId)
                {
                    waitDefenderUserList.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < waitOffenderUserList.Count; i++)
            {
                if (waitOffenderUserList[i] == playerId)
                {
                    waitOffenderUserList.RemoveAt(i);
                    break;
                }
            }

            UpdateUserInfo();
        }

        #endregion

        #region Private Room

        public void MakePrivateRoom(string userId, string userName)
        {
            string roomCode = MakePrivateKey();
            Dictionary<string, PrivateRoomUserInfo> newUser = new Dictionary<string, PrivateRoomUserInfo>();
            PrivateRoomUserInfo userInfo = new PrivateRoomUserInfo()
            {
                userName = userName,
                readyState = false
            };
            newUser.Add(userId, userInfo);
            privateRooms.Add(roomCode, newUser);

            UpdatePrivateRoom(userId, roomCode);
        }

        public void JoinPrivateRoom(string userId, string userName, string roomCode)
        {
            if (privateRooms.ContainsKey(roomCode))
            {
                bool canJoin = privateRooms[roomCode].Count == 1;
                if (canJoin)
                {
                    Dictionary<string, PrivateRoomUserInfo> newUser = new Dictionary<string, PrivateRoomUserInfo>();
                    PrivateRoomUserInfo userInfo = new PrivateRoomUserInfo()
                    {
                        userName = userName,
                        readyState = false
                    };
                    privateRooms[roomCode].Add(userId, userInfo);
                }
            }

            UpdatePrivateRoom(userId, roomCode);
        }

        private void UpdatePrivateRoom(string userId, string roomCode)
        {
            S_UpdatePrivateRoom packet = new S_UpdatePrivateRoom();
            packet.roomCode = roomCode;

            List<string> users = new List<string>();
            if (privateRooms.ContainsKey(roomCode))
            {
                foreach (string id in privateRooms[roomCode].Keys)
                {
                    packet.users.Add(new S_UpdatePrivateRoom.User()
                    {
                        playerId = id,
                        playerName = privateRooms[roomCode][id].userName,
                        ready = privateRooms[roomCode][id].readyState
                    });
                    users.Add(id);
                }

                foreach (string id in users)
                    Send(id, packet);
            }
            // 방이 없으면 방 터트려주기
            else
            {
                Send(userId, new S_DestroyPrivateRoom());
            }
        }

        public void ReadyPrivateRoom(string userId, string roomCode, bool ready)
        {
            if (privateRooms.ContainsKey(roomCode) && privateRooms[roomCode].ContainsKey(userId))
            {
                PrivateRoomUserInfo info = privateRooms[roomCode][userId];
                info.readyState = ready;
                privateRooms[roomCode][userId] = info;
            }

            UpdatePrivateRoom(userId, roomCode);
        }

        public void ExitPrivateRoom(string userId)
        {
            foreach (string roomCode in privateRooms.Keys)
            {
                if (privateRooms[roomCode].ContainsKey(userId))
                {
                    List<string> pIds = privateRooms[roomCode].Keys.ToList();
                    if (pIds[0] == userId)
                    {
                        DestroyPrivateRoom(roomCode);
                    }
                    else
                    {
                        privateRooms[roomCode].Remove(userId);
                        UpdatePrivateRoom(userId, roomCode);
                    }
                }
            }
        }

        public void ExitPrivateRoom(string userId, string roomCode)
        {
            Dictionary<string, PrivateRoomUserInfo> users;
            if (privateRooms.TryGetValue(roomCode, out users))
            {
                List<string> pIds = users.Keys.ToList();
                if (pIds[0] == userId)
                {
                    DestroyPrivateRoom(roomCode);
                }
                else
                {
                    if (privateRooms[roomCode].ContainsKey(userId))
                        privateRooms[roomCode].Remove(userId);
                    UpdatePrivateRoom(userId, roomCode);
                }
            }
        }

        private void DestroyPrivateRoom(string roomCode)
        {
            S_DestroyPrivateRoom packet = new S_DestroyPrivateRoom();
            if (privateRooms.ContainsKey(roomCode))
            {
                foreach (string id in privateRooms[roomCode].Keys)
                {
                    Send(id, packet);
                }
                privateRooms.Remove(roomCode);
            }
        }

        public void StartPrivateRoom(string roomCode)
        {
            Dictionary<string, PrivateRoomUserInfo> users;
            if (privateRooms.TryGetValue(roomCode, out users))
            {
                if (users.Count != 2) return;
                
                bool allReady = true;
                foreach (PrivateRoomUserInfo user in users.Values)
                {
                    allReady = user.readyState;
                    if (allReady == false) break;
                }
                if (allReady)
                {
                    List<string> pIds = users.Keys.ToList();
                    string[] playerIds = new string[2] { pIds[0], pIds[1] };

                    int roomNumber = _roomNumber++;
                    string roomId = string.Format("{0:yy}{0:MM}{0:dd}{1:D8}", DateTime.Now, roomNumber);

                    PlayRoom playRoom = new PlayRoom(roomId, playerIds, this);
                    playingRooms.Add(roomId, playRoom);

                    for (ushort i = 0; i < 2; i++)
                    {
                        Send(playerIds[i],
                            new S_StartGame()
                            {
                                roomId = roomId,
                                enemyPlayerId = playerIds[i],
                                playerType = i
                            });
                    }
                    DestroyPrivateRoom(roomCode);
                }
            }
        }

        private string MakePrivateKey()
        {
            string keyCand = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string roomCode;

            Random rand = new Random();

            do
            {
                roomCode = "";
                for (int i = 0; i < 5; i++)
                {
                    int index = rand.Next(0, keyCand.Length);
                    roomCode += keyCand[index];
                }
            }
            while (privateRooms.ContainsKey(roomCode));

            return roomCode;
        }
        #endregion

        #region Play Game

        public void ReadyGameEnd(string roomId, UserType type, List<int> candidates)
        {
            if (playingRooms.ContainsKey(roomId))
            {
                playingRooms[roomId].ReadyRoundState(type, candidates);
            }
            else if (playingSingleGameRooms.ContainsKey(roomId))
            {
                playingSingleGameRooms[roomId].ReadyRoundState(type, candidates);
            }
            else
            {

            }
        }

        public void RoundEnd(string roomId, UserType type)
        {
            if (playingRooms.ContainsKey(roomId))
            {
                playingRooms[roomId].ReadyRoundState(type);
            }
            else if (playingSingleGameRooms.ContainsKey(roomId))
            {
                playingSingleGameRooms[roomId].ReadyRoundState(type);
            }
            else
            {

            }
        }

        public void RoundReadyEnd(string roomId, UserType type, List<C_RoundReady.Roster> rosters)
        {
            if (playingRooms.ContainsKey(roomId))
            {
                playingRooms[roomId].RoundReadyEnd(type, rosters);
            }
            else if (playingSingleGameRooms.ContainsKey(roomId))
            {
                playingSingleGameRooms[roomId].RoundReadyEnd(type, rosters);
            }
            else
            {

            }
        }
        public void PlayRoundReadyEnd(string roomId, UserType type, List<C_PlayRoundReady.Roster> rosters)
        {
            if (playingRooms.ContainsKey(roomId))
            {
                playingRooms[roomId].PlayRoundReadyEnd(type, rosters);
            }
            else if (playingSingleGameRooms.ContainsKey(roomId))
            {
                playingSingleGameRooms[roomId].PlayRoundReadyEnd(type, rosters);
            }
            else
            {

            }
        }

        public void ReadyCancel(string roomId, UserType type)
        {
            if (playingRooms.ContainsKey(roomId))
            {
                playingRooms[roomId].ReadyCancel(type);
            }
            else if (playingSingleGameRooms.ContainsKey(roomId))
            {
                playingSingleGameRooms[roomId].ReadyCancel(type);
            }
            else
            {

            }
        }

        public void GameEnd(string roomId)
        {
            if (playingRooms.ContainsKey(roomId))
            {
                playingRooms[roomId].DestroyRoom();
                playingRooms[roomId] = null;
                playingRooms.Remove(roomId);
            }
            else if (playingSingleGameRooms.ContainsKey(roomId))
            {
                playingSingleGameRooms[roomId].DestroyRoom();
                playingSingleGameRooms[roomId] = null;
                playingSingleGameRooms.Remove(roomId);
            }
            else
            {

            }

            UpdateUserInfo();
        }

        private void PlayingRoomAbnormalExit(string id)
        {
            List<string> keys = playingRooms.Keys.ToList();
            for (int i = 0; i < playingRooms.Count; i++)
            {
                int findPlayer = playingRooms[keys[i]].PlayerInRoom(id);
                if (findPlayer != -1)
                {
                    UserType winner = ((UserType)findPlayer == UserType.Defender) ? UserType.Offender : UserType.Defender;
                    string winnerId = playingRooms[keys[i]].GetPlayerId(winner);

                    S_GameEnd packet = new S_GameEnd();
                    packet.winner = (ushort)winner;
                    packet.winnerId = winnerId;

                    Send(id, packet);
                    Send(winnerId, packet);
                    Push(() => GameEnd(keys[i]));
                    break;
                }
            }
        }
        #endregion
    }
}
