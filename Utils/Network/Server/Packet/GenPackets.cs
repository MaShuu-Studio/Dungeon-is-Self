using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    C_CheckConnect = 0,
	S_CheckConnect = 1,
	S_BroadcastConnectUser = 2,
	C_EnterGame = 3,
	S_GivePlayerId = 4,
	S_FailConnect = 5,
	C_LeaveGame = 6,
	C_Chat = 7,
	S_Chat = 8,
	C_MakePrivateRoom = 9,
	C_JoinPrivateRoom = 10,
	C_ChangeTypePrivateRoom = 11,
	S_UpdatePrivateRoom = 12,
	C_ReadyPrivateRoom = 13,
	C_StartPrivateRoom = 14,
	C_ExitPrivateRoom = 15,
	S_DestroyPrivateRoom = 16,
	C_MatchRequest = 17,
	C_MatchRequestCancel = 18,
	S_StartGame = 19,
	C_ReadyGame = 20,
	S_ReadyGameEnd = 21,
	C_RoundReady = 22,
	S_RoundReadyEnd = 23,
	C_PlayRoundReady = 24,
	S_ProgressTurn = 25,
	C_RoundEnd = 26,
	S_NewRound = 27,
	S_Timeout = 28,
	C_ReadyCancel = 29,
	C_Surrender = 30,
	C_GameEnd = 31,
	S_GameEnd = 32,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class C_CheckConnect : IPacket
{
    public string playerId;
    public ushort Protocol { get { return (ushort)PacketID.C_CheckConnect; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CheckConnect), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_CheckConnect : IPacket
{
    
    public ushort Protocol { get { return (ushort)PacketID.S_CheckConnect; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_CheckConnect), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_BroadcastConnectUser : IPacket
{
    public int totalUser;
	public int playingUser;
	public int waitDefUser;
	public int waitOffUser;
    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastConnectUser; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.totalUser = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.playingUser = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.waitDefUser = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.waitOffUser = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastConnectUser), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.totalUser), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.playingUser), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.waitDefUser), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.waitOffUser), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_EnterGame : IPacket
{
    public string token;
	public string playerId;
    public ushort Protocol { get { return (ushort)PacketID.C_EnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort tokenLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.token = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, tokenLen);
		count += tokenLen;
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort tokenLen = (ushort)Encoding.Unicode.GetBytes(this.token, 0, this.token.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(tokenLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += tokenLen;
		
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_GivePlayerId : IPacket
{
    public string playerId;
    public ushort Protocol { get { return (ushort)PacketID.S_GivePlayerId; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GivePlayerId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_FailConnect : IPacket
{
    
    public ushort Protocol { get { return (ushort)PacketID.S_FailConnect; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_FailConnect), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_LeaveGame : IPacket
{
    public string roomId;
	public string playerId;
    public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_Chat : IPacket
{
    public string playerId;
	public string playerName;
	public string chat;
    public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		ushort playerNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerNameLen);
		count += playerNameLen;
		ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
		count += chatLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		ushort playerNameLen = (ushort)Encoding.Unicode.GetBytes(this.playerName, 0, this.playerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerNameLen;
		
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_Chat : IPacket
{
    public string playerName;
	public string chat;
    public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerNameLen);
		count += playerNameLen;
		ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
		count += chatLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerNameLen = (ushort)Encoding.Unicode.GetBytes(this.playerName, 0, this.playerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerNameLen;
		
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_MakePrivateRoom : IPacket
{
    public string playerId;
	public string playerName;
    public ushort Protocol { get { return (ushort)PacketID.C_MakePrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		ushort playerNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerNameLen);
		count += playerNameLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_MakePrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		ushort playerNameLen = (ushort)Encoding.Unicode.GetBytes(this.playerName, 0, this.playerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerNameLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_JoinPrivateRoom : IPacket
{
    public string playerId;
	public string playerName;
	public string roomCode;
    public ushort Protocol { get { return (ushort)PacketID.C_JoinPrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		ushort playerNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerNameLen);
		count += playerNameLen;
		ushort roomCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomCodeLen);
		count += roomCodeLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_JoinPrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		ushort playerNameLen = (ushort)Encoding.Unicode.GetBytes(this.playerName, 0, this.playerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerNameLen;
		
		ushort roomCodeLen = (ushort)Encoding.Unicode.GetBytes(this.roomCode, 0, this.roomCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomCodeLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_ChangeTypePrivateRoom : IPacket
{
    public string playerId;
	public string roomCode;
	public ushort index;
	public ushort type;
    public ushort Protocol { get { return (ushort)PacketID.C_ChangeTypePrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		ushort roomCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomCodeLen);
		count += roomCodeLen;
		this.index = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.type = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ChangeTypePrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		ushort roomCodeLen = (ushort)Encoding.Unicode.GetBytes(this.roomCode, 0, this.roomCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomCodeLen;
		
		Array.Copy(BitConverter.GetBytes(this.index), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_UpdatePrivateRoom : IPacket
{
    public string roomCode;
	public class User
	{
	    public string playerId;
		public string playerName;
		public ushort type;
		public bool ready;
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
			count += playerIdLen;
			ushort playerNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerNameLen);
			count += playerNameLen;
			this.type = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.ready = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
	    }
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerIdLen;
			
			ushort playerNameLen = (ushort)Encoding.Unicode.GetBytes(this.playerName, 0, this.playerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerNameLen;
			
			Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			
			Array.Copy(BitConverter.GetBytes(this.ready), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
	        return success;
	    }
	}
	public List<User> users = new List<User>();
    public ushort Protocol { get { return (ushort)PacketID.S_UpdatePrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomCodeLen);
		count += roomCodeLen;
		users.Clear();
		ushort userLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < userLen; i++)
		{
		    User user = new User();
		    user.Read(segment, ref count);
		    this.users.Add(user);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_UpdatePrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomCodeLen = (ushort)Encoding.Unicode.GetBytes(this.roomCode, 0, this.roomCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomCodeLen;
		
		Array.Copy(BitConverter.GetBytes((ushort)users.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(User user in users)
		    user.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_ReadyPrivateRoom : IPacket
{
    public string roomCode;
	public string playerId;
	public bool ready;
    public ushort Protocol { get { return (ushort)PacketID.C_ReadyPrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomCodeLen);
		count += roomCodeLen;
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		this.ready = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ReadyPrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomCodeLen = (ushort)Encoding.Unicode.GetBytes(this.roomCode, 0, this.roomCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomCodeLen;
		
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.ready), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_StartPrivateRoom : IPacket
{
    public string roomCode;
    public ushort Protocol { get { return (ushort)PacketID.C_StartPrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomCodeLen);
		count += roomCodeLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_StartPrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomCodeLen = (ushort)Encoding.Unicode.GetBytes(this.roomCode, 0, this.roomCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomCodeLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_ExitPrivateRoom : IPacket
{
    public string playerId;
	public string roomCode;
    public ushort Protocol { get { return (ushort)PacketID.C_ExitPrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		ushort roomCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomCodeLen);
		count += roomCodeLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ExitPrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		ushort roomCodeLen = (ushort)Encoding.Unicode.GetBytes(this.roomCode, 0, this.roomCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomCodeLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_DestroyPrivateRoom : IPacket
{
    
    public ushort Protocol { get { return (ushort)PacketID.S_DestroyPrivateRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_DestroyPrivateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_MatchRequest : IPacket
{
    public string playerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.C_MatchRequest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		this.playerType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_MatchRequest), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_MatchRequestCancel : IPacket
{
    public string playerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.C_MatchRequestCancel; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		this.playerType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_MatchRequestCancel), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_StartGame : IPacket
{
    public string roomId;
	public string enemyPlayerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.S_StartGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		ushort enemyPlayerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.enemyPlayerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, enemyPlayerIdLen);
		count += enemyPlayerIdLen;
		this.playerType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_StartGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		ushort enemyPlayerIdLen = (ushort)Encoding.Unicode.GetBytes(this.enemyPlayerId, 0, this.enemyPlayerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(enemyPlayerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += enemyPlayerIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_ReadyGame : IPacket
{
    public string roomId;
	public ushort playerType;
	public List<int> candidates = new List<int>();
    public ushort Protocol { get { return (ushort)PacketID.C_ReadyGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		this.playerType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		candidates.Clear();
		ushort candidateLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < candidateLen; i++)
		{
		    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		    count += sizeof(int);
		    this.candidates.Add(a);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ReadyGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes((ushort)candidates.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(int candidate in candidates)
		{
		    Array.Copy(BitConverter.GetBytes(candidate), 0, segment.Array, segment.Offset + count, sizeof(int));
		    count += sizeof(int);
		}

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_ReadyGameEnd : IPacket
{
    public ushort currentProgress;
	public int round;
	public List<int> enemyCandidates = new List<int>();
    public ushort Protocol { get { return (ushort)PacketID.S_ReadyGameEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.currentProgress = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.round = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		enemyCandidates.Clear();
		ushort enemyCandidateLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < enemyCandidateLen; i++)
		{
		    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		    count += sizeof(int);
		    this.enemyCandidates.Add(a);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_ReadyGameEnd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.currentProgress), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(this.round), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes((ushort)enemyCandidates.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(int enemyCandidate in enemyCandidates)
		{
		    Array.Copy(BitConverter.GetBytes(enemyCandidate), 0, segment.Array, segment.Offset + count, sizeof(int));
		    count += sizeof(int);
		}

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_RoundReady : IPacket
{
    public string roomId;
	public ushort playerType;
	public class Roster
	{
	    public int unitIndex;
		public int attackSkill;
		public List<int> skillRosters = new List<int>();
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.unitIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.attackSkill = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			skillRosters.Clear();
			ushort skillRosterLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < skillRosterLen; i++)
			{
			    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			    count += sizeof(int);
			    this.skillRosters.Add(a);
			}
	    }
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.unitIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(this.attackSkill), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes((ushort)skillRosters.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(int skillRoster in skillRosters)
			{
			    Array.Copy(BitConverter.GetBytes(skillRoster), 0, segment.Array, segment.Offset + count, sizeof(int));
			    count += sizeof(int);
			}
	        return success;
	    }
	}
	public List<Roster> rosters = new List<Roster>();
    public ushort Protocol { get { return (ushort)PacketID.C_RoundReady; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		this.playerType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		rosters.Clear();
		ushort rosterLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < rosterLen; i++)
		{
		    Roster roster = new Roster();
		    roster.Read(segment, ref count);
		    this.rosters.Add(roster);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RoundReady), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes((ushort)rosters.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Roster roster in rosters)
		    roster.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_RoundReadyEnd : IPacket
{
    public ushort currentProgress;
	public int round;
	public class EnemyRoster
	{
	    public int unitIndex;
		public List<int> skillRosters = new List<int>();
		public int attackSkill;
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.unitIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			skillRosters.Clear();
			ushort skillRosterLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < skillRosterLen; i++)
			{
			    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			    count += sizeof(int);
			    this.skillRosters.Add(a);
			}
			this.attackSkill = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
	    }
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.unitIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes((ushort)skillRosters.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(int skillRoster in skillRosters)
			{
			    Array.Copy(BitConverter.GetBytes(skillRoster), 0, segment.Array, segment.Offset + count, sizeof(int));
			    count += sizeof(int);
			}
			Array.Copy(BitConverter.GetBytes(this.attackSkill), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
	        return success;
	    }
	}
	public List<EnemyRoster> enemyRosters = new List<EnemyRoster>();
    public ushort Protocol { get { return (ushort)PacketID.S_RoundReadyEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.currentProgress = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.round = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		enemyRosters.Clear();
		ushort enemyRosterLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < enemyRosterLen; i++)
		{
		    EnemyRoster enemyRoster = new EnemyRoster();
		    enemyRoster.Read(segment, ref count);
		    this.enemyRosters.Add(enemyRoster);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_RoundReadyEnd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.currentProgress), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(this.round), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes((ushort)enemyRosters.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(EnemyRoster enemyRoster in enemyRosters)
		    enemyRoster.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_PlayRoundReady : IPacket
{
    public string roomId;
	public ushort playerType;
	public class Roster
	{
	    public int unitIndex;
		public List<int> diceIndexs = new List<int>();
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.unitIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			diceIndexs.Clear();
			ushort diceIndexLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < diceIndexLen; i++)
			{
			    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			    count += sizeof(int);
			    this.diceIndexs.Add(a);
			}
	    }
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.unitIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes((ushort)diceIndexs.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(int diceIndex in diceIndexs)
			{
			    Array.Copy(BitConverter.GetBytes(diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			    count += sizeof(int);
			}
	        return success;
	    }
	}
	public List<Roster> rosters = new List<Roster>();
    public ushort Protocol { get { return (ushort)PacketID.C_PlayRoundReady; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		this.playerType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		rosters.Clear();
		ushort rosterLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < rosterLen; i++)
		{
		    Roster roster = new Roster();
		    roster.Read(segment, ref count);
		    this.rosters.Add(roster);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_PlayRoundReady), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes((ushort)rosters.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Roster roster in rosters)
		    roster.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_ProgressTurn : IPacket
{
    public bool isGameEnd;
	public ushort winner;
	public int endTurn;
	public int round;
	public int turn;
	public int resetTurn;
	public List<int> monsterHps = new List<int>();
	public int monsterTurn;
	public class Result
	{
	    public bool isDead;
		public int deadTurn;
		public int unitIndex;
		public int diceResult;
		public int remainTurn;
		public List<int> diceIndexs = new List<int>();
		public class CcWithTurn
		{
		    public class Cc
			{
			    public int ccid;
				public int ccturn;
				public int ccstack;
				public int ccdotdmg;
				public bool ccOn;
			
			    public void Read(ArraySegment<byte> segment, ref ushort count)
			    {
			        this.ccid = BitConverter.ToInt32(segment.Array, segment.Offset + count);
					count += sizeof(int);
					this.ccturn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
					count += sizeof(int);
					this.ccstack = BitConverter.ToInt32(segment.Array, segment.Offset + count);
					count += sizeof(int);
					this.ccdotdmg = BitConverter.ToInt32(segment.Array, segment.Offset + count);
					count += sizeof(int);
					this.ccOn = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
					count += sizeof(bool);
			    }
			    public bool Write(ArraySegment<byte> segment, ref ushort count)
			    {
			        bool success = true;
			        Array.Copy(BitConverter.GetBytes(this.ccid), 0, segment.Array, segment.Offset + count, sizeof(int));
					count += sizeof(int);
					
					Array.Copy(BitConverter.GetBytes(this.ccturn), 0, segment.Array, segment.Offset + count, sizeof(int));
					count += sizeof(int);
					
					Array.Copy(BitConverter.GetBytes(this.ccstack), 0, segment.Array, segment.Offset + count, sizeof(int));
					count += sizeof(int);
					
					Array.Copy(BitConverter.GetBytes(this.ccdotdmg), 0, segment.Array, segment.Offset + count, sizeof(int));
					count += sizeof(int);
					
					Array.Copy(BitConverter.GetBytes(this.ccOn), 0, segment.Array, segment.Offset + count, sizeof(bool));
					count += sizeof(bool);
					
			        return success;
			    }
			}
			public List<Cc> ccs = new List<Cc>();
		
		    public void Read(ArraySegment<byte> segment, ref ushort count)
		    {
		        ccs.Clear();
				ushort ccLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
				count += sizeof(ushort);
				for (int i = 0; i < ccLen; i++)
				{
				    Cc cc = new Cc();
				    cc.Read(segment, ref count);
				    this.ccs.Add(cc);
				}
		    }
		    public bool Write(ArraySegment<byte> segment, ref ushort count)
		    {
		        bool success = true;
		        Array.Copy(BitConverter.GetBytes((ushort)ccs.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
				count += sizeof(ushort);
				foreach(Cc cc in ccs)
				    cc.Write(segment, ref count);
		        return success;
		    }
		}
		public List<CcWithTurn> ccWithTurns = new List<CcWithTurn>();
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.isDead = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.deadTurn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.unitIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.diceResult = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.remainTurn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			diceIndexs.Clear();
			ushort diceIndexLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < diceIndexLen; i++)
			{
			    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			    count += sizeof(int);
			    this.diceIndexs.Add(a);
			}
			ccWithTurns.Clear();
			ushort ccWithTurnLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < ccWithTurnLen; i++)
			{
			    CcWithTurn ccWithTurn = new CcWithTurn();
			    ccWithTurn.Read(segment, ref count);
			    this.ccWithTurns.Add(ccWithTurn);
			}
	    }
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.isDead), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(this.deadTurn), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(this.unitIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(this.diceResult), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(this.remainTurn), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes((ushort)diceIndexs.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(int diceIndex in diceIndexs)
			{
			    Array.Copy(BitConverter.GetBytes(diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			    count += sizeof(int);
			}
			Array.Copy(BitConverter.GetBytes((ushort)ccWithTurns.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(CcWithTurn ccWithTurn in ccWithTurns)
			    ccWithTurn.Write(segment, ref count);
	        return success;
	    }
	}
	public List<Result> results = new List<Result>();
    public ushort Protocol { get { return (ushort)PacketID.S_ProgressTurn; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.isGameEnd = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.winner = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.endTurn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.round = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.turn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.resetTurn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		monsterHps.Clear();
		ushort monsterHpLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < monsterHpLen; i++)
		{
		    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		    count += sizeof(int);
		    this.monsterHps.Add(a);
		}
		this.monsterTurn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		results.Clear();
		ushort resultLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < resultLen; i++)
		{
		    Result result = new Result();
		    result.Read(segment, ref count);
		    this.results.Add(result);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_ProgressTurn), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.isGameEnd), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(this.winner), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(this.endTurn), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.round), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.turn), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.resetTurn), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes((ushort)monsterHps.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(int monsterHp in monsterHps)
		{
		    Array.Copy(BitConverter.GetBytes(monsterHp), 0, segment.Array, segment.Offset + count, sizeof(int));
		    count += sizeof(int);
		}
		Array.Copy(BitConverter.GetBytes(this.monsterTurn), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes((ushort)results.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Result result in results)
		    result.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_RoundEnd : IPacket
{
    public string roomId;
	public ushort type;
    public ushort Protocol { get { return (ushort)PacketID.C_RoundEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		this.type = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RoundEnd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_NewRound : IPacket
{
    public int round;
	public class UserInfo
	{
	    public ushort type;
		public List<int> deadUnits = new List<int>();
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.type = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			deadUnits.Clear();
			ushort deadUnitLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < deadUnitLen; i++)
			{
			    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			    count += sizeof(int);
			    this.deadUnits.Add(a);
			}
	    }
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			
			Array.Copy(BitConverter.GetBytes((ushort)deadUnits.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(int deadUnit in deadUnits)
			{
			    Array.Copy(BitConverter.GetBytes(deadUnit), 0, segment.Array, segment.Offset + count, sizeof(int));
			    count += sizeof(int);
			}
	        return success;
	    }
	}
	public List<UserInfo> userInfos = new List<UserInfo>();
    public ushort Protocol { get { return (ushort)PacketID.S_NewRound; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.round = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		userInfos.Clear();
		ushort userInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < userInfoLen; i++)
		{
		    UserInfo userInfo = new UserInfo();
		    userInfo.Read(segment, ref count);
		    this.userInfos.Add(userInfo);
		}
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_NewRound), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.round), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes((ushort)userInfos.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(UserInfo userInfo in userInfos)
		    userInfo.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_Timeout : IPacket
{
    public int time;
	public ushort currentProgress;
    public ushort Protocol { get { return (ushort)PacketID.S_Timeout; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.time = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentProgress = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Timeout), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.time), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.currentProgress), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_ReadyCancel : IPacket
{
    public string roomId;
	public ushort userType;
    public ushort Protocol { get { return (ushort)PacketID.C_ReadyCancel; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		this.userType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ReadyCancel), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.userType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_Surrender : IPacket
{
    public string roomId;
	public string playerId;
    public ushort Protocol { get { return (ushort)PacketID.C_Surrender; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Surrender), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_GameEnd : IPacket
{
    public string roomId;
	public ushort type;
    public ushort Protocol { get { return (ushort)PacketID.C_GameEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort roomIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomIdLen);
		count += roomIdLen;
		this.type = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_GameEnd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        ushort roomIdLen = (ushort)Encoding.Unicode.GetBytes(this.roomId, 0, this.roomId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomIdLen;
		
		Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_GameEnd : IPacket
{
    public ushort winner;
	public string winnerId;
    public ushort Protocol { get { return (ushort)PacketID.S_GameEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.winner = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		ushort winnerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.winnerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, winnerIdLen);
		count += winnerIdLen;
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GameEnd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.winner), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		ushort winnerIdLen = (ushort)Encoding.Unicode.GetBytes(this.winnerId, 0, this.winnerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(winnerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += winnerIdLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
