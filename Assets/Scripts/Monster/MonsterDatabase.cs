using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDatabase : MonoBehaviour
{
    private List<Monster> monsterDB;

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

        monsterDB.Add(new Monster("미노타우르스", 10, Monster.Weakness.NORMAL, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("바다괴물", 15, Monster.Weakness.NORMAL, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("하피", 12, Monster.Weakness.NORMAL, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("뱀", 12, Monster.Weakness.NORMAL, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("바이러스", 12, Monster.Weakness.NORMAL, SkillDatabase.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("요괴", 12, Monster.Weakness.NORMAL, SkillDatabase.GetMonsterSkill("SKILL1")));
    }

    public Monster GetMonster(string name)
    {
        return monsterDB.Find(monster => monster.name == name);
    }
}
