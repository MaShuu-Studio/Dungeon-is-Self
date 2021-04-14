using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

// 핸들러를 만들때에는 용도를 명확히 구분하는 것이 좋음.
// 클라 -> 서버, 서버 -> 클라 혹은 A서버 -> B서버 등등
class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;

        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null) return;
        GameRoom room = clientSession.Room;

        clientSession.Room.Push(
            () => clientSession.Room.Broadcast(clientSession, chatPacket.chat));
    }
}