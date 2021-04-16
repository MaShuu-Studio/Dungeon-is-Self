using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDatabase : MonoBehaviour
{
    public static string facePath { get; private set; } = "Sprites/Monsters/Faces/";
    public static string charPath { get; private set; } = "Sprites/Monsters/Chars/";
    
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

        monsterDB.Add(new Monster("Minotaurs", 10, Monster.Weakness.NORMAL, SkillDatabase.Instance.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("Kraken", 15, Monster.Weakness.NORMAL, SkillDatabase.Instance.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("Harpy", 12, Monster.Weakness.NORMAL, SkillDatabase.Instance.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("Snake", 12, Monster.Weakness.NORMAL, SkillDatabase.Instance.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("Virus", 12, Monster.Weakness.NORMAL, SkillDatabase.Instance.GetMonsterSkill("SKILL1")));
        monsterDB.Add(new Monster("Dokkaebi", 12, Monster.Weakness.NORMAL, SkillDatabase.Instance.GetMonsterSkill("SKILL1")));
    }

    public Monster GetMonster(string name)
    {
        return monsterDB.Find(monster => monster.name == name);
    }

    public void GetAllMonsterCandidatesList(ref List<string> monsterNames)
    {
        monsterNames.Clear();
        foreach (Monster monster in monsterDB)
        {
            monsterNames.Add(monster.name);
        }
    }
}
