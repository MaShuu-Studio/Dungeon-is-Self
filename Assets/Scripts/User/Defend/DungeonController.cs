using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon
{
    public List<Monster> monsters { get; private set; }
    public int maxMonsterCount { get; private set; }

    public Dungeon(int max)
    {
        monsters = new List<Monster>();
        maxMonsterCount = max;
    }

    public void AddMonster(List<Monster> monsters)
    {
        foreach (Monster monster in monsters)
        {
            Debug.Log("[Defender] Add Monster " + monster.name);
            this.monsters.Add(monster);
        }
    }

    public void AddMonster(Monster monster)
    {
        Debug.Log("[Defender] Add Monster " + monster.name);
        this.monsters.Add(monster);
    }

    public bool MonsterIsFull()
    {
        if (monsters.Count == maxMonsterCount) return true;
        return false;
    }

}
public class DungeonController : MonoBehaviour
{
    private Dungeon[] dungeons;

    private void Start()
    {
        dungeons = new Dungeon[3];
        for (int i = 0; i < dungeons.Length; i++)
        {
            dungeons[i] = new Dungeon(3);
        }
    }

    public void AddMonster(int index, List<Monster> monsters)
    {
        dungeons[index].AddMonster(monsters);
    }

    public bool AddMonster(int index, Monster monster)
    {
        if (monster != null && dungeons[index].MonsterIsFull() == false)
        {
            dungeons[index].AddMonster(monster);
            return true;
        }
        return false;
    }

    public List<Monster> GetMonsterList(int index)
    {
        return dungeons[index].monsters;
    }
    
    #region GUI
    private void OnGUI()
    {
        for (int i = 0; i < dungeons[0].monsters.Count; i++)
        {
            if (GUI.Button(new Rect(10 + (i*70), 600, 50, 50), dungeons[0].monsters[i].name))
            {
                //dungeonController.AddMonster(round, monsterCandidates[i]);
                //monsterCandidates.RemoveAt(i);
                break;
            }
        }
    }

#endregion
}
