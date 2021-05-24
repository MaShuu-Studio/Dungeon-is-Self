using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        List<Tuple<int, ArraySegment<byte>>> _packetList = new List<Tuple<int, ArraySegment<byte>>>();

        int _playerNumber = 1;
        int _singleRoomId = -2;
        int _roomNumber = 1;

        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        Dictionary<int, int> _sessionCount = new Dictionary<int, int>();
        List<int> waitDefenderUserList = new List<int>();
        List<int> waitOffenderUserList = new List<int>();
        Dictionary<int, PlayRoom> playingRooms = new Dictionary<int, PlayRoom>();
        Dictionary<int, PlayRoom> playingSingleGameRooms = new Dictionary<int, PlayRoom>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Send(int id, IPacket packet)
        {
            _packetList.Add(new Tuple<int, ArraySegment<byte>>(id, packet.Write()));
        }

        public void CheckSession()
        {
            List<int> keys = _sessionCount.Keys.ToList<int>();
            for (int i = 0; i < keys.Count; i++)
            {
                _sessionCount[keys[i]]++;
                if (_sessionCount[keys[i]] > 5) Leave(keys[i]);
            }
        }

        public void UpdateSession(int id)
        {
            if (_sessionCount.ContainsKey(id)) _sessionCount[id] = 0;
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Flush()
        {
            foreach (Tuple<int, ArraySegment<byte>> packet in _packetList)
                if (_sessions.ContainsKey(packet.Item1))
                {
                    _sessions[packet.Item1].Send(packet.Item2);
                    Console.WriteLine($"Send Packet to {packet.Item1}");
                }
                else
                {
                    Console.WriteLine($"Fail Sending Packet to {packet.Item1}");
                }

            if (_sessions.Count == 0) _pendingList.Clear();
            foreach (ClientSession s in _sessions.Values)
                s.Send(_pendingList);

            //if (_pendingList.Count + _packetList.Count != 0) Console.WriteLine($"Flushed {_pendingList.Count + _packetList.Count} items");
            _pendingList.Clear();
            _packetList.Clear();
        }

        public void Enter(ClientSession session)
        {
            int id = _playerNumber++;
            Console.WriteLine($"Enter User : {id}");
            session.Send(new S_GivePlayerId() { playerId = id }.Write());

            // 플레이어 추가
            _sessionCount.Add(id, 0);
            _sessions.Add(id, session);
            session.Room = this;

            UpdateUserInfo();
        }

        public void Leave(int id)
        {
            Console.WriteLine($"Leave User {id}");

            if (_sessions.ContainsKey(id))
            {
                _sessions[id].Disconnect();
                _sessions.Remove(id);
            }
            if (_sessionCount.ContainsKey(id))
            {
                _sessionCount.Remove(id);
            }

            PlayingRoomAbnormalExit(id);
            MatchRequestCancel(id);
            // 플레이어 나감
            // 모든 플레이어에게 퇴장을 브로드캐스트
            UpdateUserInfo();
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 나감
            int findIndex = 0;
            List<int> keys = _sessions.Keys.ToList();
            List<ClientSession> values = _sessions.Values.ToList();

            for (; findIndex < keys.Count; findIndex++)
            {
                if (values[findIndex] == session) break;
            }

            if (findIndex >= keys.Count) return;

            _sessions.Remove(keys[findIndex]);
            _sessionCount.Remove(keys[findIndex]);
            // 모든 플레이어에게 퇴장을 브로드캐스트
            UpdateUserInfo();
        }

        public void UpdateUserInfo()
        {
            // 모든 플레이어에게 입장을 브로드캐스트
            S_BroadcastConnectUser broadcast = new S_BroadcastConnectUser();
            broadcast.totalUser = _sessions.Count;
            broadcast.playingUser = playingRooms.Count * 2;
            broadcast.waitDefUser = waitDefenderUserList.Count;
            broadcast.waitOffUser = waitOffenderUserList.Count;

            Broadcast(broadcast.Write());
        }

        public void MatchRequest(int playerId, UserType type)
        {
            if (type == UserType.Defender)
                waitDefenderUserList.Add(playerId);
            else
                waitOffenderUserList.Add(playerId);

            if (waitDefenderUserList.Count > 0 && waitOffenderUserList.Count > 0)
            {
                int[] playerIds = new int[2];
                playerIds[(ushort)UserType.Defender] = waitDefenderUserList[0];
                playerIds[(ushort)UserType.Offender] = waitOffenderUserList[0];

                waitDefenderUserList.RemoveAt(0);
                waitOffenderUserList.RemoveAt(0);

                int roomId = _roomNumber++;
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
                        }
                        );
                }
            }

            UpdateUserInfo();
        }

        public void SingleGameRequest(int playerId, UserType type)
        {
            int roomId = _singleRoomId--;
            PlayRoom playRoom = new PlayRoom(roomId, playerId, type, this);
            playingSingleGameRooms.Add(roomId, playRoom);

            S_StartGame packet = new S_StartGame()
            {
                roomId = playRoom.RoomId,
                enemyPlayerId = -1,
                playerType = (ushort)type
            };

            Send(playerId, packet);

            UpdateUserInfo();
        }

        public void MatchRequestCancel(int playerId, UserType type)
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

        public void MatchRequestCancel(int playerId)
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

        public void ReadyGameEnd(int roomId, UserType type, List<int> candidates)
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

        public void RoundEnd(int roomId, UserType type)
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

        public void RoundReadyEnd(int roomId, UserType type, List<C_RoundReady.Roster> rosters)
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
        public void PlayRoundReadyEnd(int roomId, UserType type, List<C_PlayRoundReady.Roster> rosters)
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

        public void GameEnd(int roomId, UserType type)
        {
            if (playingRooms.ContainsKey(roomId))
            {
                playingRooms.Remove(roomId);
            }
            else if (playingSingleGameRooms.ContainsKey(roomId))
            {
                playingSingleGameRooms.Remove(roomId);
            }
            else
            {

            }

            UpdateUserInfo();
        }

        private void PlayingRoomAbnormalExit(int id)
        {
            List<int> keys = playingRooms.Keys.ToList();
            for (int i = 0; i < playingRooms.Count; i++)
            {
                int findPlayer = playingRooms[keys[i]].PlayerInRoom(id);
                if (findPlayer != -1)
                {
                    UserType winner = ((UserType)findPlayer == UserType.Defender) ? UserType.Offender : UserType.Defender;
                    int winnerId = playingRooms[keys[i]].GetPlayerId(winner);

                    S_GameEnd packet = new S_GameEnd();
                    packet.winner = (ushort)winner;
                    packet.winnerId = winnerId;

                    Send(id, packet);
                    Send(winnerId, packet);
                    playingRooms.Remove(keys[i]);
                    break;
                }
            }
        }
    }
}
