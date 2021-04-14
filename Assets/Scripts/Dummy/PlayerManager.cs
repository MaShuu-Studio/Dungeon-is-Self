using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    MyPlayer _player;
    Dictionary<int, Player> players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        foreach (S_PlayerList.Player player in packet.players)
        {
            Object prefab = Resources.Load("Player");
            GameObject obj = Object.Instantiate(prefab) as GameObject;
            obj.name = $"Player {player.playerId}";
            obj.transform.position = new Vector3(player.xPos, player.yPos, player.zPos);

            if (player.isSelf)
            {
                _player = obj.AddComponent<MyPlayer>();
                _player.PlayerId = player.playerId;
                Debug.Log($"My player is {player.playerId}");
            }
            else
            {
                Player p = obj.AddComponent<Player>();
                players.Add(player.playerId, p);
            }
        }
    }

    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _player.PlayerId) return;
        if (players.ContainsKey(packet.playerId)) return;

        Object prefab = Resources.Load("Player");
        GameObject obj = Object.Instantiate(prefab) as GameObject;
        obj.name = $"Player {packet.playerId}";
        Player p = obj.AddComponent<Player>();

        obj.transform.position = new Vector3(packet.xPos, packet.yPos, packet.zPos);
        players.Add(packet.playerId, p);
        Debug.Log($"Player {packet.playerId} Entered");
    }
    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if (_player.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_player.gameObject);
            _player = null;
        }
        else
        {
            Player p = null;

            if (players.TryGetValue(packet.playerId, out p))
            {
                GameObject.Destroy(p.gameObject);
                players.Remove(packet.playerId);
            }
        }
    }
    public void Move(S_BroadcastMove packet)
    {
        if (_player.PlayerId == packet.playerId)
        {
            _player.transform.position = new Vector3(packet.xPos, packet.yPos, packet.zPos);
        }
        else
        {
            Player p = null;

            if (players.TryGetValue(packet.playerId, out p))
            {
                p.transform.position = new Vector3(packet.xPos, packet.yPos, packet.zPos);
            }
        }
    }
}
