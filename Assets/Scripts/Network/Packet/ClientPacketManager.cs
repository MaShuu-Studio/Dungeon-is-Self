
using ServerCore;
using System;
using System.Collections.Generic;
using Network;

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
        makeFunc.Add((ushort)PacketID.S_FailConnect, MakePacket<S_FailConnect>);
        handler.Add((ushort)PacketID.S_FailConnect, PacketHandler.S_FailConnectHandler);
        makeFunc.Add((ushort)PacketID.S_Chat, MakePacket<S_Chat>);
        handler.Add((ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler);
        makeFunc.Add((ushort)PacketID.S_UpdatePrivateRoom, MakePacket<S_UpdatePrivateRoom>);
        handler.Add((ushort)PacketID.S_UpdatePrivateRoom, PacketHandler.S_UpdatePrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.S_DestroyPrivateRoom, MakePacket<S_DestroyPrivateRoom>);
        handler.Add((ushort)PacketID.S_DestroyPrivateRoom, PacketHandler.S_DestroyPrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.S_StartGame, MakePacket<S_StartGame>);
        handler.Add((ushort)PacketID.S_StartGame, PacketHandler.S_StartGameHandler);
        makeFunc.Add((ushort)PacketID.S_ReadyGameEnd, MakePacket<S_ReadyGameEnd>);
        handler.Add((ushort)PacketID.S_ReadyGameEnd, PacketHandler.S_ReadyGameEndHandler);
        makeFunc.Add((ushort)PacketID.S_RoundReadyEnd, MakePacket<S_RoundReadyEnd>);
        handler.Add((ushort)PacketID.S_RoundReadyEnd, PacketHandler.S_RoundReadyEndHandler);
        makeFunc.Add((ushort)PacketID.S_ProgressTurn, MakePacket<S_ProgressTurn>);
        handler.Add((ushort)PacketID.S_ProgressTurn, PacketHandler.S_ProgressTurnHandler);
        makeFunc.Add((ushort)PacketID.S_NewRound, MakePacket<S_NewRound>);
        handler.Add((ushort)PacketID.S_NewRound, PacketHandler.S_NewRoundHandler);
        makeFunc.Add((ushort)PacketID.S_Timeout, MakePacket<S_Timeout>);
        handler.Add((ushort)PacketID.S_Timeout, PacketHandler.S_TimeoutHandler);
        makeFunc.Add((ushort)PacketID.S_GameEnd, MakePacket<S_GameEnd>);
        handler.Add((ushort)PacketID.S_GameEnd, PacketHandler.S_GameEndHandler);

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

            // ?????????????????? ?????????????????? ??????????????? ?????????????????? ????????? ??? ?????? ????????????.
            // ????????? ?????? ??????????????? ??????????????? ???????????? ????????? ?????? ?????? ??? ????????????????????? ??????
            // ?????? ???????????? ????????? ???????????? ???????????? ????????? ??????.

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