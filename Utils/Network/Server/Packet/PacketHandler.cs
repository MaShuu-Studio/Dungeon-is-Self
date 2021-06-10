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

            string playerId = p.playerId;
            room.Push(() => room.UpdateSession(playerId));
        }
        public static void C_EnterGameHandler(PacketSession session, IPacket packet)
        {
            C_EnterGame p = packet as C_EnterGame;
            ClientSession clientSession = session as ClientSession;

            string token = p.token;
            string playerId = p.playerId;

            Program.Room.Push(() => Program.Room.Enter(clientSession, token, playerId));
        }
        public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
        {
            C_LeaveGame p = packet as C_LeaveGame;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string playerId = p.playerId;

            room.Push(() => room.Leave(playerId));
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

            string playerId = p.playerId;
            UserType type = (UserType)p.playerType;

            room.Push(() => room.MatchRequest(playerId, type));
        }
        public static void C_MatchRequestCancelHandler(PacketSession session, IPacket packet)
        {
            C_MatchRequestCancel p = packet as C_MatchRequestCancel;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string playerId = p.playerId;
            UserType type = (UserType)p.playerType;

            room.Push(() => room.MatchRequestCancel(playerId, type));
        }

        public static void C_MakePrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_MakePrivateRoom p = packet as C_MakePrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;

            string playerId = p.playerId;
            string playerName = p.playerName;

            room.Push(() => room.MakePrivateRoom(playerId, playerName));
        }
        public static void C_JoinPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_JoinPrivateRoom p = packet as C_JoinPrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;

            string playerId = p.playerId;
            string playerName = p.playerName;
            string roomCode = p.roomCode;

            room.Push(() => room.JoinPrivateRoom(playerId, playerName, roomCode));
        }
        public static void C_ReadyPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_ReadyPrivateRoom p = packet as C_ReadyPrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;

            string playerId = p.playerId;
            string roomCode = p.roomCode;
            bool ready = p.ready;

            room.Push(() => room.ReadyPrivateRoom(playerId, roomCode, ready));
        }
        public static void C_StartPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_StartPrivateRoom p = packet as C_StartPrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;

            string roomCode = p.roomCode;

            room.Push(() => room.StartPrivateRoom(roomCode));
        }
        public static void C_ExitPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            C_ExitPrivateRoom p = packet as C_ExitPrivateRoom;
            ClientSession clientSession = session as ClientSession;
            if (clientSession.Room == null) return;

            string roomCode = p.roomCode;
            string playerId = p.playerId;

            GameRoom room = clientSession.Room;
            room.Push(() => room.ExitPrivateRoom(playerId, roomCode));
        }
        #endregion

        #region Play Game
        public static void C_ReadyGameHandler(PacketSession session, IPacket packet)
        {
            C_ReadyGame p = packet as C_ReadyGame;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string roomId = p.roomId;
            UserType type = (UserType)p.playerType;
            List<int> candidates = p.candidates;

            room.Push(() => room.ReadyGameEnd(roomId, type, candidates));
        }
        public static void C_RoundReadyHandler(PacketSession session, IPacket packet)
        {
            C_RoundReady p = packet as C_RoundReady;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string roomId = p.roomId;
            UserType type = (UserType)p.playerType;
            List<C_RoundReady.Roster> rosters = p.rosters;

            room.Push(() => room.RoundReadyEnd(roomId, type, rosters));
        }
        public static void C_PlayRoundReadyHandler(PacketSession session, IPacket packet)
        {
            C_PlayRoundReady p = packet as C_PlayRoundReady;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string roomId = p.roomId;
            UserType type = (UserType)p.playerType;
            List<C_PlayRoundReady.Roster> rosters = p.rosters;

            room.Push(() => room.PlayRoundReadyEnd(roomId, type, rosters));
        }
        public static void C_RoundEndHandler(PacketSession session, IPacket packet)
        {
            C_RoundEnd p = packet as C_RoundEnd;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string roomId = p.roomId;
            UserType type = (UserType)p.type;

            room.Push(() => room.RoundEnd(roomId, type));
        }
        public static void C_GameEndHandler(PacketSession session, IPacket packet)
        {
            C_GameEnd p = packet as C_GameEnd;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string roomId = p.roomId;

            room.Push(() => room.GameEnd(roomId));
        }

        public static void C_SurrenderHandler(PacketSession session, IPacket packet)
        {
            C_Surrender p = packet as C_Surrender;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string playerId = p.playerId;
            string roomId = p.roomId;

            room.Push(() => room.PlayingRoomAbnormalExit(playerId, roomId));
        }

        public static void C_ReadyCancelHandler(PacketSession session, IPacket packet)
        {
            C_ReadyCancel p = packet as C_ReadyCancel;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;
            GameRoom room = clientSession.Room;

            string roomId = p.roomId;
            UserType type = (UserType)p.userType;
            room.Push(() => room.ReadyCancel(roomId, type));
        }
        #endregion
    }
}