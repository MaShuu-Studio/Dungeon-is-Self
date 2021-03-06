
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
        makeFunc.Add((ushort)PacketID.C_CheckConnect, MakePacket<C_CheckConnect>);
        handler.Add((ushort)PacketID.C_CheckConnect, PacketHandler.C_CheckConnectHandler);
        makeFunc.Add((ushort)PacketID.C_EnterGame, MakePacket<C_EnterGame>);
        handler.Add((ushort)PacketID.C_EnterGame, PacketHandler.C_EnterGameHandler);
        makeFunc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
        handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);
        makeFunc.Add((ushort)PacketID.C_Chat, MakePacket<C_Chat>);
        handler.Add((ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler);
        makeFunc.Add((ushort)PacketID.C_MakePrivateRoom, MakePacket<C_MakePrivateRoom>);
        handler.Add((ushort)PacketID.C_MakePrivateRoom, PacketHandler.C_MakePrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.C_JoinPrivateRoom, MakePacket<C_JoinPrivateRoom>);
        handler.Add((ushort)PacketID.C_JoinPrivateRoom, PacketHandler.C_JoinPrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.C_ChangeTypePrivateRoom, MakePacket<C_ChangeTypePrivateRoom>);
        handler.Add((ushort)PacketID.C_ChangeTypePrivateRoom, PacketHandler.C_ChangeTypePrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.C_ReadyPrivateRoom, MakePacket<C_ReadyPrivateRoom>);
        handler.Add((ushort)PacketID.C_ReadyPrivateRoom, PacketHandler.C_ReadyPrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.C_StartPrivateRoom, MakePacket<C_StartPrivateRoom>);
        handler.Add((ushort)PacketID.C_StartPrivateRoom, PacketHandler.C_StartPrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.C_ExitPrivateRoom, MakePacket<C_ExitPrivateRoom>);
        handler.Add((ushort)PacketID.C_ExitPrivateRoom, PacketHandler.C_ExitPrivateRoomHandler);
        makeFunc.Add((ushort)PacketID.C_SingleGameRequest, MakePacket<C_SingleGameRequest>);
        handler.Add((ushort)PacketID.C_SingleGameRequest, PacketHandler.C_SingleGameRequestHandler);
        makeFunc.Add((ushort)PacketID.C_MatchRequest, MakePacket<C_MatchRequest>);
        handler.Add((ushort)PacketID.C_MatchRequest, PacketHandler.C_MatchRequestHandler);
        makeFunc.Add((ushort)PacketID.C_MatchRequestCancel, MakePacket<C_MatchRequestCancel>);
        handler.Add((ushort)PacketID.C_MatchRequestCancel, PacketHandler.C_MatchRequestCancelHandler);
        makeFunc.Add((ushort)PacketID.C_ReadyGame, MakePacket<C_ReadyGame>);
        handler.Add((ushort)PacketID.C_ReadyGame, PacketHandler.C_ReadyGameHandler);
        makeFunc.Add((ushort)PacketID.C_RoundReady, MakePacket<C_RoundReady>);
        handler.Add((ushort)PacketID.C_RoundReady, PacketHandler.C_RoundReadyHandler);
        makeFunc.Add((ushort)PacketID.C_PlayRoundReady, MakePacket<C_PlayRoundReady>);
        handler.Add((ushort)PacketID.C_PlayRoundReady, PacketHandler.C_PlayRoundReadyHandler);
        makeFunc.Add((ushort)PacketID.C_RoundEnd, MakePacket<C_RoundEnd>);
        handler.Add((ushort)PacketID.C_RoundEnd, PacketHandler.C_RoundEndHandler);
        makeFunc.Add((ushort)PacketID.C_ReadyCancel, MakePacket<C_ReadyCancel>);
        handler.Add((ushort)PacketID.C_ReadyCancel, PacketHandler.C_ReadyCancelHandler);
        makeFunc.Add((ushort)PacketID.C_Surrender, MakePacket<C_Surrender>);
        handler.Add((ushort)PacketID.C_Surrender, PacketHandler.C_SurrenderHandler);
        makeFunc.Add((ushort)PacketID.C_GameEnd, MakePacket<C_GameEnd>);
        handler.Add((ushort)PacketID.C_GameEnd, PacketHandler.C_GameEndHandler);

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