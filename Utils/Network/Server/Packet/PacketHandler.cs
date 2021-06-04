using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

// 핸들러를 만들때에는 용도를 명확히 구분하는 것이 좋음.
// 클라 -> 서버, 서버 -> 클라 혹은 A서버 -> B서버 등등
namespace Network
{
    class PacketHandler
    {
        /*
        public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
        {
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.Leave(clientSession));
        }
        public static void C_MoveHandler(PacketSession session, IPacket packet)
        {
            C_Move movePacket = packet as C_Move;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.Move(clientSession, movePacket));
            //Console.WriteLine($"{clientSession.SessionId}: {clientSession.Xpos}, {clientSession.Ypos}");
        }*/
        public static void C_CheckConnectHandler(PacketSession session, IPacket packet)
        {
            C_CheckConnect p = packet as C_CheckConnect;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.UpdateSession(p.playerId));
        }
        public static void C_EnterGameHandler(PacketSession session, IPacket packet)
        {
            C_EnterGame p = packet as C_EnterGame;
            ClientSession clientSession = session as ClientSession;

            Program.Room.Push(() => Program.Room.Enter(clientSession, p.token, p.playerId));
            /*
            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.Enter(clientSession, p.token, p.playerId));*/
        }
        public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
        {
            C_LeaveGame p = packet as C_LeaveGame;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.Leave(p.playerId));
            clientSession.Disconnect();
        }
        public static void C_SingleGameRequestHandler(PacketSession session, IPacket packet)
        {
            C_SingleGameRequest p = packet as C_SingleGameRequest;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;
            room.SingleGameRequest(p.playerId, (UserType)p.playerType);
        }
        public static void C_MatchRequestHandler(PacketSession session, IPacket packet)
        {
            C_MatchRequest p = packet as C_MatchRequest;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;
            room.MatchRequest(p.playerId, (UserType)p.playerType);
        }
        public static void C_MatchRequestCancelHandler(PacketSession session, IPacket packet)
        {
            C_MatchRequestCancel p = packet as C_MatchRequestCancel;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;
            room.MatchRequestCancel(p.playerId, (UserType)p.playerType);
        }
        public static void C_ReadyGameHandler(PacketSession session, IPacket packet)
        {
            C_ReadyGame p = packet as C_ReadyGame;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.ReadyGameEnd(p.roomId, (UserType)p.playerType, p.candidates);
        }
        public static void C_RoundReadyHandler(PacketSession session, IPacket packet)
        {
            C_RoundReady p = packet as C_RoundReady;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.RoundReadyEnd(p.roomId, (UserType)p.playerType, p.rosters);
        }
        public static void C_PlayRoundReadyHandler(PacketSession session, IPacket packet)
        {
            C_PlayRoundReady p = packet as C_PlayRoundReady;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.PlayRoundReadyEnd(p.roomId, (UserType)p.playerType, p.rosters);
        }
        public static void C_RoundEndHandler(PacketSession session, IPacket packet)
        {
            C_RoundEnd p = packet as C_RoundEnd;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.RoundEnd(p.roomId, (UserType)p.type);
        }
        public static void C_GameEndHandler(PacketSession session, IPacket packet)
        {
            C_GameEnd p = packet as C_GameEnd;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.GameEnd(p.roomId);
        }

        public static void C_ReadyCancelHandler(PacketSession session, IPacket packet)
        {
            C_ReadyCancel p = packet as C_ReadyCancel;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.ReadyCancel(p.roomId, (UserType)p.userType);
        }
    }
}