using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIController ui_controller;
    [SerializeField] private DungeonController dungeon_controller;


    private MonsterDatabase monster_db;

    // Start is called before the first frame update
    void Start()
    {
        monster_db = GameObject.FindWithTag("MonsterDB").GetComponent<MonsterDatabase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMonsterCandidate(int num)
    {
        List<Monster> monsters = new List<Monster>();

        for(int i = 0; i < num; i++)
        {
            monsters.Add(monster_db.GetRandomMonster());
        }

        ui_controller.ShowMonsterList(monsters);
    }

    public void SetDungeon(Text text)
    {
        dungeon_controller.AddMonster(0, monster_db.GetMonster(text.text));
    }
}
