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

        #region Tier 1
        monsterDB.Add(new Monster(
            1, "Minotaurs", 10, Monster.Weakness.NORMAL,
            new List<string>()
            {
                "SKILL1-1",
                "SKILL1-2",
                "SKILL1-3",
            }, 
            new List<string>()
            {
                "DICE1-1",
                "DICE1-2",
                "DICE1-4",
                "DICE2-1",
                "DICE2-2",
                "DICE3-2",
            }));

        monsterDB.Add(new Monster(
            1, "Kraken", 15, Monster.Weakness.NORMAL,
            new List<string>()
            {
                "SKILL1-1",
                "SKILL1-2",
                "SKILL1-3",
            },
            new List<string>()
            {
                "DICE1-1",
                "DICE1-2",
                "DICE1-3",
                "DICE1-4",
                "DICE2-1",
                "DICE2-2",
                "DICE2-3",
                "DICE3-1",
                "DICE3-2",
                "DICE3-3",
            }));

        monsterDB.Add(new Monster(
            1, "Harpy", 12, Monster.Weakness.NORMAL,
            new List<string>()
            {
                "SKILL1-1",
                "SKILL1-2",
                "SKILL1-3",
            },
            new List<string>()
            {
                "DICE1-2",
                "DICE1-3",
                "DICE1-4",
                "DICE2-2",
                "DICE2-3",
                "DICE3-3",
            }));

        monsterDB.Add(new Monster(
            1, "Snake", 12, Monster.Weakness.NORMAL,
            new List<string>()
            {
                "SKILL1-1",
                "SKILL1-2",
                "SKILL1-3",
            },
            new List<string>()
            {
                "DICE1-1",
                "DICE1-2",
                "DICE1-3",
                "DICE2-1",
                "DICE2-2",
                "DICE2-3",
                "DICE3-1",
                "DICE3-2",
                "DICE3-3",
            }));

        monsterDB.Add(new Monster(
            1, "Ninetail", 12, Monster.Weakness.NORMAL,
            new List<string>()
            {
                "SKILL1-1",
                "SKILL1-2",
                "SKILL1-3",
            },
            new List<string>()
            {
                "DICE1-1",
                "DICE1-2",
                "DICE1-3",
                "DICE2-1",
                "DICE2-2",
                "DICE2-3",
                "DICE3-1",
                "DICE3-2",
                "DICE3-3",
            }));

        monsterDB.Add(new Monster(
            1, "Dokkaebi", 12, Monster.Weakness.NORMAL,
            new List<string>()
            {
                "SKILL1-1",
                "SKILL1-2",
                "SKILL1-3",
            },
            new List<string>()
            {
                "DICE1-1",
                "DICE1-2",
                "DICE1-3",
                "DICE2-1",
                "DICE2-2",
                "DICE2-3",
                "DICE3-1",
                "DICE3-2",
                "DICE3-3",
            }));
        #endregion
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
