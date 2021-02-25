using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon
{
    public List<Monster> monsters { get; private set; }

    public Dungeon()
    {
        monsters = new List<Monster>();
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

}
public class DungeonController : MonoBehaviour
{
    private Dungeon[] dungeons;

    private void Start()
    {
        dungeons = new Dungeon[3];
        for (int i = 0; i < dungeons.Length; i++)
        {
            dungeons[i] = new Dungeon();
        }
    }

    public void AddMonster(int index, List<Monster> monsters)
    {
        dungeons[index].AddMonster(monsters);
    }

    public void AddMonster(int index, Monster monster)
    {
        dungeons[index].AddMonster(monster);
    }
    
}
