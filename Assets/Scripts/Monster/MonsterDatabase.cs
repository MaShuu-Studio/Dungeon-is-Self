using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDatabase : MonoBehaviour
{
    private List<Monster> monsterDB;
    [SerializeField] private MonsterSkillDataBase skillDB;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeDataBase();
    }

    private void InitializeDataBase()
    {
        monsterDB = new List<Monster>();

        monsterDB.Add(new Monster("SLIME", 10, Monster.MonsterType.ATTACKER, skillDB.GetSkill("SKILL1")));
    }

    public Monster GetMonster(string name)
    {
        return monsterDB.Find(monster => monster.name == name);
    }
}
