using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

// 핸들러를 만들때에는 용도를 명확히 구분하는 것이 좋음.
// 클라 -> 서버, 서버 -> 클라 혹은 A서버 -> B서버 등등
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
    public static void C_EnterGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null) return;
        GameRoom room = clientSession.Room;

        room.Push(() => room.Enter(clientSession));
    }
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        C_MatchRequest p = packet as C_MatchRequest;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null) return;
        GameRoom room = clientSession.Room;

        room.Push(() => room.Leave(clientSession));
        clientSession.Disconnect();
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
        C_MatchRequest p = packet as C_MatchRequest;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null) return;
        GameRoom room = clientSession.Room;
        room.MatchRequestCancel(p.playerId, (UserType)p.playerType);
    }
    public static void C_ReadyGameHandler(PacketSession session, IPacket packet)    
    { 
    }
    public static void C_RoundReadyEndHandler(PacketSession session, IPacket packet)    
    { 
    }
    public static void C_PlayRoundReadyHandler(PacketSession session, IPacket packet)    
    {   
    }
}