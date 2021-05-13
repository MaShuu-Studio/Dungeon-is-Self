using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        int _totalUser = 0;
        int _playinguser = 0;
        int _waitDefenderUser = 0;
        int _waitOffenderUser = 0;

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
            // 플레이어 추가
            _sessions.Add(session);
            _totalUser = _sessions.Count;
            session.Room = this;

            UpdateUserInfo();
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 나감
            _sessions.Remove(session);
            _totalUser = _sessions.Count;
            session.Disconnect();
            // 모든 플레이어에게 퇴장을 브로드캐스트
            UpdateUserInfo();
        }

        public void UpdateUserInfo()
        {
            // 모든 플레이어에게 입장을 브로드캐스트
            S_BroadcastConnectUser broadcast = new S_BroadcastConnectUser();
            broadcast.totalUser = _totalUser;
            broadcast.playingUser = _playinguser;
            broadcast.waitDefUser = _waitDefenderUser;
            broadcast.waitOffUser = _waitOffenderUser;

            Broadcast(broadcast.Write());
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
