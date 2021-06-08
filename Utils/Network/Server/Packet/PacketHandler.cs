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
        #region Connect
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
        #endregion

        #region Match
        /*
        public static void C_SingleGameRequestHandler(PacketSession session, IPacket packet)
        {
            C_SingleGameRequest p = packet as C_SingleGameRequest;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;
            room.Push(() => room.SingleGameRequest(p.playerId, (UserType)p.playerType));
        }
        */
        public static void C_MatchRequestHandler(PacketSession session, IPacket packet)
        {
            C_MatchRequest p = packet as C_MatchRequest;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;
            room.Push(() => room.MatchRequest(p.playerId, (UserType)p.playerType));
        }
        public static void C_MatchRequestCancelHandler(PacketSession session, IPacket packet)
        {
            C_MatchRequestCancel p = packet as C_MatchRequestCancel;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;
            room.Push(() => room.MatchRequestCancel(p.playerId, (UserType)p.playerType));
        }
        public static void C_MakePrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_MakePrivateRoom p = packet as C_MakePrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;
            room.Push(() => room.MakePrivateRoom(p.playerId));
        }
        public static void C_JoinPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_JoinPrivateRoom p = packet as C_JoinPrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;
            room.Push(() => room.JoinPrivateRoom(p.playerId, p.roomCode));
        }
        public static void C_StartPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_StartPrivateRoom p = packet as C_StartPrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;
            room.Push(() => room.StartPrivateRoom(p.roomCode));
        }
        public static void C_DestroyPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_DestroyPrivateRoom p = packet as C_DestroyPrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;
            room.Push(() => room.DestroyPrivateRoom(p.playerId, p.roomCode));
        }
        #endregion

        #region Play Game
        public static void C_ReadyGameHandler(PacketSession session, IPacket packet)
        {
            C_ReadyGame p = packet as C_ReadyGame;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.ReadyGameEnd(p.roomId, (UserType)p.playerType, p.candidates));
        }
        public static void C_RoundReadyHandler(PacketSession session, IPacket packet)
        {
            C_RoundReady p = packet as C_RoundReady;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.RoundReadyEnd(p.roomId, (UserType)p.playerType, p.rosters));
        }
        public static void C_PlayRoundReadyHandler(PacketSession session, IPacket packet)
        {
            C_PlayRoundReady p = packet as C_PlayRoundReady;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.PlayRoundReadyEnd(p.roomId, (UserType)p.playerType, p.rosters));
        }
        public static void C_RoundEndHandler(PacketSession session, IPacket packet)
        {
            C_RoundEnd p = packet as C_RoundEnd;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.RoundEnd(p.roomId, (UserType)p.type));
        }
        public static void C_GameEndHandler(PacketSession session, IPacket packet)
        {
            C_GameEnd p = packet as C_GameEnd;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.GameEnd(p.roomId));
        }

        public static void C_ReadyCancelHandler(PacketSession session, IPacket packet)
        {
            C_ReadyCancel p = packet as C_ReadyCancel;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            room.Push(() => room.ReadyCancel(p.roomId, (UserType)p.userType));
        }
        #endregion
    }
}