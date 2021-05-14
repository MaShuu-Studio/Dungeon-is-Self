﻿using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        int _playerNumber = 1;
        int _roomNumber = 1;

        List<ClientSession> _sessions = new List<ClientSession>();
        List<int> waitDefenderUserList = new List<int>();
        List<int> waitOffenderUserList = new List<int>();
        List<PlayRoom> playingRooms = new List<PlayRoom>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Flush()
        {
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            if (_pendingList.Count != 0) Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Enter(ClientSession session)
        {
            int id = _playerNumber++;
            session.Send(new S_GivePlayerId() { playerId = id }.Write());
            // 플레이어 추가
            _sessions.Add(session);
            session.Room = this;

            UpdateUserInfo();
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 나감
            _sessions.Remove(session);
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
                int defenderId = waitDefenderUserList[0];
                int offenderId = waitOffenderUserList[0];

                waitDefenderUserList.RemoveAt(0);
                waitOffenderUserList.RemoveAt(0);

                PlayRoom playRoom = new PlayRoom(_roomNumber++, defenderId, offenderId);
                playingRooms.Add(playRoom);
                Console.WriteLine(playingRooms.Count.ToString());
            }

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
                for (int i = 0; i < waitDefenderUserList.Count; i++)
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
        /*
        public void Move(ClientSession session, C_Move movePacket)
        {
            // 좌표 이동
            session.Xpos = movePacket.xPos;
            session.Ypos = movePacket.yPos;
            session.Zpos = movePacket.zPos;

            // Broadcast
            S_BroadcastMove broadcastMove = new S_BroadcastMove();
            broadcastMove.playerId = session.SessionId;
            broadcastMove.xPos = session.Xpos;
            broadcastMove.yPos = session.Ypos;
            broadcastMove.zPos = session.Zpos;
            Broadcast(broadcastMove.Write());
        }*/
    }
}
