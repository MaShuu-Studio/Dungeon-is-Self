using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

// 핸들러를 만들때에는 용도를 명확히 구분하는 것이 좋음.
// 클라 -> 서버, 서버 -> 클라 혹은 A서버 -> B서버 등등
class PacketHandler
{
    /*
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1)
        //Console.WriteLine(chatPacket.chat);
    }
    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1)
        //Console.WriteLine(chatPacket.chat);
    }
    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1)
        //Console.WriteLine(chatPacket.chat);
    }
    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1)
        //Console.WriteLine(chatPacket.chat);
    }*/

    public static void S_BroadcastWaitUserHandler(PacketSession arg1, IPacket arg2)
    {
    }

    public static void S_GameStateHandler(PacketSession arg1, IPacket arg2)
    {
    }

    public static void S_ProgressTurnHandler(PacketSession arg1, IPacket arg2)
    {
    }

    public static void S_StartGameHandler(PacketSession arg1, IPacket arg2)
    {
    }
}