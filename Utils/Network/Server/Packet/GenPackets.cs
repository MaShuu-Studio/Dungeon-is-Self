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
	C_LeaveGame = 5,
	C_SingleGameRequest = 6,
	C_MatchRequest = 7,
	C_MatchRequestCancel = 8,
	S_StartGame = 9,
	C_ReadyGame = 10,
	S_ReadyGameEnd = 11,
	C_RoundReady = 12,
	S_RoundReadyEnd = 13,
	C_PlayRoundReady = 14,
	S_ProgressTurn = 15,
	C_RoundEnd = 16,
	S_NewRound = 17,
	C_ReadyCancel = 18,
	C_GameEnd = 19,
	S_GameEnd = 20,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class C_CheckConnect : IPacket
{
    public int playerId;
    public ushort Protocol { get { return (ushort)PacketID.C_CheckConnect; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CheckConnect), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

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
    
    public ushort Protocol { get { return (ushort)PacketID.C_EnterGame; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_GivePlayerId : IPacket
{
    public int playerId;
    public ushort Protocol { get { return (ushort)PacketID.S_GivePlayerId; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GivePlayerId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_LeaveGame : IPacket
{
    public int playerId;
    public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_SingleGameRequest : IPacket
{
    public int playerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.C_SingleGameRequest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.playerType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_SingleGameRequest), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_MatchRequest : IPacket
{
    public int playerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.C_MatchRequest; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_MatchRequestCancel : IPacket
{
    public int playerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.C_MatchRequestCancel; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_StartGame : IPacket
{
    public int roomId;
	public int enemyPlayerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.S_StartGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.enemyPlayerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.enemyPlayerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.playerType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_ReadyGame : IPacket
{
    public int roomId;
	public ushort playerType;
	public List<int> candidates = new List<int>();
    public ushort Protocol { get { return (ushort)PacketID.C_ReadyGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
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
    public int roomId;
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
        
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
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
    public int roomId;
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
        
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
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
		public List<int> diceIndexs = new List<int>();
		public class CcWithTurn
		{
		    public class Cc
			{
			    public int ccid;
				public int ccturn;
				public int ccstack;
				public bool ccOn;
			
			    public void Read(ArraySegment<byte> segment, ref ushort count)
			    {
			        this.ccid = BitConverter.ToInt32(segment.Array, segment.Offset + count);
					count += sizeof(int);
					this.ccturn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
					count += sizeof(int);
					this.ccstack = BitConverter.ToInt32(segment.Array, segment.Offset + count);
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
    public int roomId;
	public ushort type;
    public ushort Protocol { get { return (ushort)PacketID.C_RoundEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
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
public class C_ReadyCancel : IPacket
{
    public int roomId;
	public ushort userType;
    public ushort Protocol { get { return (ushort)PacketID.C_ReadyCancel; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.userType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_GameEnd : IPacket
{
    public int roomId;
	public ushort type;
    public ushort Protocol { get { return (ushort)PacketID.C_GameEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_GameEnd : IPacket
{
    public ushort winner;
	public int winnerId;
    public ushort Protocol { get { return (ushort)PacketID.S_GameEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.winner = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.winnerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
		
		Array.Copy(BitConverter.GetBytes(this.winnerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
