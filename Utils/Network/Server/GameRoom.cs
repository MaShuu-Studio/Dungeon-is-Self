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
            session.Room = this;

            // 입장 플레이어에게 모든 플레이어목록 전달
            S_PlayerList playerList = new S_PlayerList();
            foreach(ClientSession s in _sessions)
            {
                playerList.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.SessionId,
                    xPos = s.Xpos,
                    yPos = s.Ypos,
                    zPos = s.Zpos
                });
            }

            session.Send(playerList.Write());

            // 모든 플레이어에게 입장을 브로드캐스트
            S_BroadcastEnterGame broadcastEnter = new S_BroadcastEnterGame();
            broadcastEnter.playerId = session.SessionId;
            broadcastEnter.xPos = 0;
            broadcastEnter.yPos = 0;
            broadcastEnter.zPos = 0;
            Broadcast(broadcastEnter.Write());

        }

        public void Leave(ClientSession session)
        {
            // 플레이어 나감
            _sessions.Remove(session);
            // 모든 플레이어에게 퇴장을 브로드캐스트
            S_BroadcastLeaveGame broadcastLeave = new S_BroadcastLeaveGame();
            broadcastLeave.playerId = session.SessionId;
            Broadcast(broadcastLeave.Write());
        }

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
        }
    }
}
