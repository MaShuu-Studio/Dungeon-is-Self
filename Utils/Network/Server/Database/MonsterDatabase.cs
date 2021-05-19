using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public class MonsterDatabase
    {
        static MonsterDatabase instance = new MonsterDatabase();
        public static MonsterDatabase Instance { get { return instance; } }

        private List<Monster> monsterDB;
        
        MonsterDatabase()
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
                    280,
                    450,
                    700,
                }, Element.NORMAL,
                new List<int>()
                {
                    200211,
                    200311,
                    200411,
                }));
            /*
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
            */
            monsterDB.Add(new Monster(
                203, 1, "스네이크", new List<int>()
                {
                    200,
                    400,
                    550,
                }, Element.NORMAL,
                new List<int>()
                {
                    200211,
                    200311,
                    200411,
                }));

            monsterDB.Add(new Monster(
                204, 1, "구미호", new List<int>()
                {
                    250,
                    400,
                    600,
                }, Element.NORMAL,
                new List<int>()
                {
                    200211,
                    200311,
                    200411,
                }));
            /*
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
            */
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
