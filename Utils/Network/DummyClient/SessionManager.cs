﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
    // 실제 프로젝트에서는 필요X 
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in _sessions)
                {
                    Random rand = new Random();
                    C_Move movePacket = new C_Move();
                    movePacket.xPos = ((float)(rand.NextDouble() - 0.5f) * 10);
                    movePacket.yPos = ((float)(rand.NextDouble() - 0.5f) * 10);
                    movePacket.zPos = 0;

                    session.Send(movePacket.Write());
                }
            }
        }
    }
}
