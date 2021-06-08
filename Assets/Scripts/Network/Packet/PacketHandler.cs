using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GameControl;

// 핸들러를 만들때에는 용도를 명확히 구분하는 것이 좋음.
// 클라 -> 서버, 서버 -> 클라 혹은 A서버 -> B서버 등등
namespace Network
{
    class PacketHandler
    {
        public static void S_CheckConnectHandler(PacketSession session, IPacket packet)
        {
            NetworkManager.Instance.UpdateServer();
        }
        public static void S_FailConnectHandler(PacketSession session, IPacket packet)
        {
            NetworkManager.Instance.Disconnect();
        }
        public static void S_BroadcastConnectUserHandler(PacketSession session, IPacket packet)
        {
            S_BroadcastConnectUser p = packet as S_BroadcastConnectUser;
            ServerSession serverSession = session as ServerSession;

            NetworkManager.Instance.SetConnectingUserInfo(p.totalUser, p.playingUser, p.waitDefUser, p.waitOffUser);
        }
        public static void S_GivePlayerIdHandler(PacketSession session, IPacket packet)
        {
            S_GivePlayerId p = packet as S_GivePlayerId;
            ServerSession serverSession = session as ServerSession;

            NetworkManager.Instance.SetPlayerId(p.playerId);
        }

        public static void S_UpdatePrivateRoomHandler(PacketSession session, IPacket packet)
        {
            S_UpdatePrivateRoom p = packet as S_UpdatePrivateRoom;
        }
        public static void S_DestroyPrivateRoomHandler(PacketSession session, IPacket packet)
        {
            S_DestroyPrivateRoom p = packet as S_DestroyPrivateRoom;
        }

        public static void S_StartGameHandler(PacketSession session, IPacket packet)
        {
            S_StartGame p = packet as S_StartGame;
            ServerSession serverSession = session as ServerSession;

            GameController.Instance.StartGame(p.roomId, (UserType)p.playerType);
        }
        public static void S_ReadyGameEndHandler(PacketSession session, IPacket packet)
        {
            S_ReadyGameEnd p = packet as S_ReadyGameEnd;
            ServerSession serverSession = session as ServerSession;

            GameController.Instance.ReadyGameEnd(p.round, p.enemyCandidates);
        }
        public static void S_RoundReadyEndHandler(PacketSession session, IPacket packet)
        {
            S_RoundReadyEnd p = packet as S_RoundReadyEnd;
            ServerSession serverSession = session as ServerSession;

            GameController.Instance.StartRound(p.round, p.enemyRosters);
        }
        public static void S_ProgressTurnHandler(PacketSession session, IPacket packet)
        {
            S_ProgressTurn p = packet as S_ProgressTurn;
            ServerSession serverSession = session as ServerSession;

            GameController.Instance.ProgressTurn(p.round, p.turn, p);
        }
        public static void S_NewRoundHandler(PacketSession session, IPacket packet)
        {
            S_NewRound p = packet as S_NewRound;
            ServerSession serverSession = session as ServerSession;

            GameController.Instance.ReadyNewRound(p.round, p.userInfos);

        }
        public static void S_GameEndHandler(PacketSession session, IPacket packet)
        {
            S_GameEnd p = packet as S_GameEnd;
            ServerSession serverSession = session as ServerSession;

            GameController.Instance.GameEnd(p.winner);
        }
        public static void S_TimeoutHandler(PacketSession session, IPacket packet)
        {
            S_Timeout p = packet as S_Timeout;
            ServerSession serverSession = session as ServerSession;

            GamePlayUIController.Instance.SetTimer(p.time);

            if (p.time <= 0)
            {
                switch((GameProgress)p.currentProgress)
                {
                    case GameProgress.ReadyGame:
                        bool b = false;
                        NetworkManager.Instance.GameReadyEnd(ref b);
                        break;
                    case GameProgress.ReadyRound:
                        NetworkManager.Instance.RoundReadyEnd();
                        break;
                    case GameProgress.PlayRound:
                        NetworkManager.Instance.TurnReadyEnd();
                        break;
                }
                GamePlayUIController.Instance.SetButtonInteract(false);
            }
        }
    }
}