using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDatabase : MonoBehaviour
{
    private static List<Monster> monsterDB;

    private static MonsterDatabase instance;
    public static MonsterDatabase Instance
    {
        get
        {
            var obj = FindObjectOfType<MonsterDatabase>();
            instance = obj;
            return instance;
        }
    }
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
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

        monsterDB.Add(new Monster("SLIME", 10, Monster.MonsterType.ATTACKER, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME2", 15, Monster.MonsterType.ATTACKER, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME3", 12, Monster.MonsterType.SPECIAL, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME4", 20, Monster.MonsterType.GUARD, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME5", 18, Monster.MonsterType.GUARD, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME6", 10, Monster.MonsterType.ATTACKER, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME7", 15, Monster.MonsterType.ATTACKER, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME8", 12, Monster.MonsterType.SPECIAL, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME9", 20, Monster.MonsterType.GUARD, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("SLIME10", 18, Monster.MonsterType.GUARD, SkillDatabase.GetMonsterSkill("SKILL1")));
    }

    public static Monster GetMonster(string name)
    {
        return monsterDB.Find(monster => monster.name == name);
    }

    public static Monster GetRandomMonster()
    {
        int index = Random.Range(0, monsterDB.Count);
        return monsterDB[index];
    }
}
