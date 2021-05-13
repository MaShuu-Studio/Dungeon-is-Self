using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    S_BroadcastConnectUser = 0,
	C_EnterGame = 1,
	C_LeaveGame = 2,
	C_MatchGame = 3,
	S_StartGame = 4,
	C_ReadyGame = 5,
	S_GameState = 6,
	C_RoundReadyEnd = 7,
	C_PlayRoundReady = 8,
	S_ProgressTurn = 9,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
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
    public int playerId;
    public ushort Protocol { get { return (ushort)PacketID.C_EnterGame; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
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
public class C_MatchGame : IPacket
{
    public int playerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.C_MatchGame; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_MatchGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
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
    public int enemyPlayerId;
	public ushort playerType;
    public ushort Protocol { get { return (ushort)PacketID.S_StartGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
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
    public int playerId;
	public ushort playerType;
	public List<int> candidates = new List<int>();
    public ushort Protocol { get { return (ushort)PacketID.C_ReadyGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
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
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
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
public class S_GameState : IPacket
{
    public ushort currentProgress;
	public int round;
    public ushort Protocol { get { return (ushort)PacketID.S_GameState; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.currentProgress = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.round = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    { 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GameState), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.currentProgress), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(this.round), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_RoundReadyEnd : IPacket
{
    public class Roster
	{
	    public int unitIndex;
		public List<int> skillRosterss = new List<int>();
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.unitIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			skillRosterss.Clear();
			ushort skillRostersLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < skillRostersLen; i++)
			{
			    int a = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			    count += sizeof(int);
			    this.skillRosterss.Add(a);
			}
	    }
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.unitIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes((ushort)skillRosterss.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(int skillRosters in skillRosterss)
			{
			    Array.Copy(BitConverter.GetBytes(skillRosters), 0, segment.Array, segment.Offset + count, sizeof(int));
			    count += sizeof(int);
			}
	        return success;
	    }
	}
	public List<Roster> rosters = new List<Roster>();
    public ushort Protocol { get { return (ushort)PacketID.C_RoundReadyEnd; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RoundReadyEnd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes((ushort)rosters.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Roster roster in rosters)
		    roster.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_PlayRoundReady : IPacket
{
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
    public bool isRoundEnd;
	public bool isGameEnd;
	public ushort winner;
	public int turn;
	public int monsterHp;
	public int monsterTurn;
	public class Result
	{
	    public int unitIndex;
		public List<int> diceIndexs = new List<int>();
		public class Cc
		{
		    public int ccIndex;
			public int ccTurn;
			public int ccStack;
		
		    public void Read(ArraySegment<byte> segment, ref ushort count)
		    {
		        this.ccIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
				count += sizeof(int);
				this.ccTurn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
				count += sizeof(int);
				this.ccStack = BitConverter.ToInt32(segment.Array, segment.Offset + count);
				count += sizeof(int);
		    }
		    public bool Write(ArraySegment<byte> segment, ref ushort count)
		    {
		        bool success = true;
		        Array.Copy(BitConverter.GetBytes(this.ccIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
				count += sizeof(int);
				
				Array.Copy(BitConverter.GetBytes(this.ccTurn), 0, segment.Array, segment.Offset + count, sizeof(int));
				count += sizeof(int);
				
				Array.Copy(BitConverter.GetBytes(this.ccStack), 0, segment.Array, segment.Offset + count, sizeof(int));
				count += sizeof(int);
				
		        return success;
		    }
		}
		public List<Cc> ccs = new List<Cc>();
	
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
	        Array.Copy(BitConverter.GetBytes(this.unitIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes((ushort)diceIndexs.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(int diceIndex in diceIndexs)
			{
			    Array.Copy(BitConverter.GetBytes(diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			    count += sizeof(int);
			}
			Array.Copy(BitConverter.GetBytes((ushort)ccs.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach(Cc cc in ccs)
			    cc.Write(segment, ref count);
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
        
        this.isRoundEnd = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.isGameEnd = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.winner = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.turn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.monsterHp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        
        Array.Copy(BitConverter.GetBytes(this.isRoundEnd), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(this.isGameEnd), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(this.winner), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(this.turn), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.monsterHp), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
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
