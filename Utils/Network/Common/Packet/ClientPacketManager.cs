
using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    static PacketManager instance = new PacketManager();
    public static PacketManager Instance { get { return instance; } }

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {
        makeFunc.Add((ushort)PacketID.S_CheckConnect, MakePacket<S_CheckConnect>);
        handler.Add((ushort)PacketID.S_CheckConnect, PacketHandler.S_CheckConnectHandler);
        makeFunc.Add((ushort)PacketID.S_BroadcastConnectUser, MakePacket<S_BroadcastConnectUser>);
        handler.Add((ushort)PacketID.S_BroadcastConnectUser, PacketHandler.S_BroadcastConnectUserHandler);
        makeFunc.Add((ushort)PacketID.S_GivePlayerId, MakePacket<S_GivePlayerId>);
        handler.Add((ushort)PacketID.S_GivePlayerId, PacketHandler.S_GivePlayerIdHandler);
        makeFunc.Add((ushort)PacketID.S_StartGame, MakePacket<S_StartGame>);
        handler.Add((ushort)PacketID.S_StartGame, PacketHandler.S_StartGameHandler);
        makeFunc.Add((ushort)PacketID.S_ReadyGameEnd, MakePacket<S_ReadyGameEnd>);
        handler.Add((ushort)PacketID.S_ReadyGameEnd, PacketHandler.S_ReadyGameEndHandler);
        makeFunc.Add((ushort)PacketID.S_RoundReadyEnd, MakePacket<S_RoundReadyEnd>);
        handler.Add((ushort)PacketID.S_RoundReadyEnd, PacketHandler.S_RoundReadyEndHandler);
        makeFunc.Add((ushort)PacketID.S_ProgressTurn, MakePacket<S_ProgressTurn>);
        handler.Add((ushort)PacketID.S_ProgressTurn, PacketHandler.S_ProgressTurnHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        int count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);

            // 유니티에서는 게임쓰레드가 아닌곳에서 유니티코드를 불러올 수 없게 되어있음.
            // 따라서 패킷 생성파트와 핸들파트를 분리하고 작업을 큐에 보관 후 게임쓰레드에서 실행
            // 이를 옵션으로 받아서 패킷큐에 푸시하는 식으로 진행.

            if (onRecvCallback != null) onRecvCallback.Invoke(session, packet);
            else HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);

        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }
}