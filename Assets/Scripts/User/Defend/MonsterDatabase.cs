using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class MonsterDatabase : MonoBehaviour
    {
        public static string facePath { get; private set; } = "Sprites/Monsters/Faces/";
        public static string charPath { get; private set; } = "Sprites/Monsters/Chars/";

        private List<Monster> monsterDB;

        #region Instace

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
        #endregion

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
                201, 1, "미노타우르스", 
                new List<int>()
                {
                    250,
                    400,
                    650,
                }, Element.NORMAL,
                new List<int>()
                {
                    22101,
                    23101,
                    24101,
                },
                new List<int>()
                {
                    21101,
                    21102,
                    21103,
                    21104,
                    21104,
                    21201,
                    21202,
                    21203,
                    21301,
                    21302,
                    21303,
                }));
            monsterDB.Add(new Monster(
                202, 1, "하피", new List<int>()
                {
                    250,
                    400,
                    650,
                }, Element.NORMAL,
                new List<int>()
                {
                    22101,
                    23101,
                    24101,
                },
                new List<int>()
                {
                    21101,
                    21102,
                    21103,
                    21104,
                    21104,
                    21201,
                    21202,
                    21203,
                    21301,
                    21302,
                    21303,
                }));

            monsterDB.Add(new Monster(
                203, 1, "스네이크", new List<int>()
                {
                    250,
                    400,
                    650,
                }, Element.NORMAL,
                new List<int>()
                {
                    22101,
                    23101,
                    24101,
                },
                new List<int>()
                {
                    21101,
                    21102,
                    21103,
                    21104,
                    21104,
                    21201,
                    21202,
                    21203,
                    21301,
                    21302,
                    21303,
                }));

            monsterDB.Add(new Monster(
                204, 1, "구미호", new List<int>()
                {
                    250,
                    400,
                    650,
                }, Element.NORMAL,
                new List<int>()
                {
                    22101,
                    23101,
                    24101,
                },
                new List<int>()
                {
                    21101,
                    21102,
                    21103,
                    21104,
                    21104,
                    21201,
                    21202,
                    21203,
                    21301,
                    21302,
                    21303,
                }));

            monsterDB.Add(new Monster(
                205, 1, "도깨비", new List<int>()
                {
                    250,
                    400,
                    650,
                }, Element.NORMAL,
                new List<int>()
                {
                    22101,
                    23101,
                    24101,
                },
                new List<int>()
                {
                    21101,
                    21102,
                    21103,
                    21104,
                    21104,
                    21201,
                    21202,
                    21203,
                    21301,
                    21302,
                    21303,
                }));
            /*
            monsterDB.Add(new Monster(
                1, "Kraken", 35, Element.NORMAL,
                new List<int>()
                {
                    22101,
                    23101,
                    24101,
                },
                new List<int>()
                {
                    21101,
                    21102,
                    21103,
                    21104,
                    21104,
                    21201,
                    21202,
                    21203,
                    21301,
                    21302,
                    21303,
                }));
            */
            #endregion
        }

        public Monster GetMonster(int id)
        {
            Monster copiedMonster = new Monster(monsterDB.Find(monster => monster.id == id));
            return copiedMonster;
        }

        public void GetAllMonsterCandidatesList(ref List<int> monsterId)
        {
            monsterId.Clear();
            foreach (Monster monster in monsterDB)
            {
                monsterId.Add(monster.id);
            }
        }
    }
}
