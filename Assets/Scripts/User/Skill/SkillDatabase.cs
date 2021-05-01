using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class SkillDatabase : MonoBehaviour
    {
        #region CrowdControl
        private List<CrowdControl> ccDatabase = new List<CrowdControl>();

        private void InitializeCrowdControl()
        {
            // TAUNT = 1, GUARD = 2, REFLECT = 3 , PURITY = 4,
            // INVINCIBLE = 5, BLIND = 6, STUN = 7, DOTDAMAGE = 8,
            // ATTACKSTAT = 9, MIRRORIMAGE = 10,

            ccDatabase.Add(new CrowdControl(30101, "도발", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30201, "방어", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30301, "반사", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30401, "무효화", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30501, "무적", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30601, "실명", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30602, "마비", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30701, "스턴", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30801, "출혈", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30802, "중독", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30803, "화상", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30804, "감전", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30901, "공격력 증가", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30902, "공격력 감소", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(31001, "분신", CCTarget.SELF));
        }

        public CrowdControl GetCrowdControl(int id)
        {
            CrowdControl tmp = ccDatabase.Find(cc => cc.id == id);
            CrowdControl crowdControl = new CrowdControl(tmp);

            return crowdControl;
        }
        #endregion
        private static List<CharacterSkill> charSkillDB;
        private static List<MonsterSkill> monSkillDB;

        #region Instace
        private static SkillDatabase instance;
        public static SkillDatabase Instance
        {
            get
            {
                var obj = FindObjectOfType<SkillDatabase>();
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
            InitializeCrowdControl();
            InitializeCharacterSkill();
            InitializeMonsterSkill();
        }
        #endregion

        private void InitializeCharacterSkill()
        {
            charSkillDB = new List<CharacterSkill>();
            #region Knight Skill
            charSkillDB.Add(new CharacterSkill(10100, 0, "베기", turn: 0, damage: 7));

            charSkillDB.Add(new CharacterSkill(10101, 1, "찌르기", turn: 0, 9));

            charSkillDB.Add(new CharacterSkill(10102, 1, "도발", turn: 0, damage: 0, 
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30101), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10103, 2, "작살베기", turn: 0, damage: 18));

            charSkillDB.Add(new CharacterSkill(10104, 2, "방패찍기", turn: 0, damage: 13,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));

            charSkillDB.Add(new CharacterSkill(10105, 2, "강철방패", turn: 1, damage: 0,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30201), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10106, 2, "야성의외침", turn: 0, damage: 0,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30401), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10107, 3, "반격", turn: 1, damage: 20, 
                new List<int> { 10104, 10105 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30201), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));

            charSkillDB.Add(new CharacterSkill(10108, 3, "용기", turn: 1, damage: 0, 
                new List<int> { 10106 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30401), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30901), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10109, 4, "가시방패", turn: 2, damage: 30,
                new List<int> { 10103, 10107 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30301), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10110, 4, "데드락", turn: 1, damage: 30, 
                new List<int> { 10107 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));

            charSkillDB.Add(new CharacterSkill(10111, 4, "포효", turn: 2, damage: 10, 
                new List<int> { 10108 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30501), 0),
                }));

            #endregion

            #region Marksman Skill
            charSkillDB.Add(new CharacterSkill(10200, 0, "화살쏘기", turn: 0, damage: 9));
            charSkillDB.Add(new CharacterSkill(10201, 1, "더블샷", turn: 0, damage: 11));
            charSkillDB.Add(new CharacterSkill(10202, 1, "그물화살", turn: 0, damage: 8,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10203, 2, "트리플샷", turn: 0, damage: 20));
            charSkillDB.Add(new CharacterSkill(10204, 2, "톱날화살", turn: 0, damage: 16,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30801), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10205, 2, "파워샷", turn: 1, damage: 32,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10206, 2, "화살촉 강화", turn: 0, damage: 0,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30901), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10207, 3, "화살비", turn: 1, damage: 60,
                new List<int> { 10203 }));
            charSkillDB.Add(new CharacterSkill(10208, 3, "독화살", turn: 0, damage: 24, 
                new List<int> { 10204 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30802), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10209, 3, "저격", turn: 1, damage: 60, 
                new List<int> { 10205, 10206 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            charSkillDB.Add(new CharacterSkill(10210, 3, "매 소환", turn: 0, damage: 0, 
                new List<int> { 10206 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31001), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10211, 4, "헤드샷", turn: 2, damage: 150, 
                new List<int> { 10209 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            #endregion

            #region Mage Skill
            charSkillDB.Add(new CharacterSkill(10300, 0, "기본공격", turn: 0, damage: 5));
            charSkillDB.Add(new CharacterSkill(10301, 1, "라이트닝",turn: 0, damage: 13));
            charSkillDB.Add(new CharacterSkill(10302, 1, "고드름", turn: 0, damage: 13));
            charSkillDB.Add(new CharacterSkill(10303, 1, "파이어볼", turn: 0, damage: 13));
            charSkillDB.Add(new CharacterSkill(10304, 2, "번개구름", turn: 0, damage: 20,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10305, 2, "빙결감옥", turn: 1, damage: 35,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10306, 2, "블레이즈", turn: 1, damage: 48,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30803), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10307, 3, "전격폭발", turn: 0, damage: 30, 
                new List<int> { 10304 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10308, 3, "블리자드", turn: 1, damage: 50, 
                new List<int> { 10305 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10309, 3, "파이어월", turn: 2, damage: 105, 
                new List<int> { 10306 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30803), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10310, 4, "뇌룡", turn: 2, damage: 180, 
                new List<int> { 10307, 10309 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30804), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10311, 4, "빙하기", turn: 2, damage: 180, 
                new List<int> { 10307, 10308 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            charSkillDB.Add(new CharacterSkill(10312, 4, "메테오", turn: 2, damage: 180, 
                new List<int> { 10308, 10309 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30803), 0),
                }));
            #endregion
        }

        private void InitializeMonsterSkill()
        {
            monSkillDB = new List<MonsterSkill>();

            #region Dice
            monSkillDB.Add(new MonsterSkill(21101, "DICE1-1", 5, 0));
            monSkillDB.Add(new MonsterSkill(21102, "DICE1-2", 5, 0));
            monSkillDB.Add(new MonsterSkill(21103, "DICE1-3", 5, 1));
            monSkillDB.Add(new MonsterSkill(21104, "DICE1-4", 5, 1));
            monSkillDB.Add(new MonsterSkill(21104, "DICE1-5", 5, 1));
            monSkillDB.Add(new MonsterSkill(21201, "DICE2-1", 5, 2));
            monSkillDB.Add(new MonsterSkill(21202, "DICE2-2", 5, 2));
            monSkillDB.Add(new MonsterSkill(21203, "DICE2-3", 5, 3));
            monSkillDB.Add(new MonsterSkill(21301, "DICE3-1", 5, 5));
            monSkillDB.Add(new MonsterSkill(21302, "DICE3-2", 5, 6));
            monSkillDB.Add(new MonsterSkill(21303, "DICE3-3", 5, 7));
            #endregion

            #region Attack
            monSkillDB.Add(new MonsterSkill(22101, "Attack All", 10, 0));
            monSkillDB.Add(new MonsterSkill(23101, "Attack One", 4, 0));
            monSkillDB.Add(new MonsterSkill(24101, "Attack One and Stun", 6, 0,
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            #endregion


        }

        public CharacterSkill GetCharacterSkill(int id)
        {
            return charSkillDB.Find(skill => skill.id == id);
        }

        public MonsterSkill GetMonsterSkill(int id)
        {
            return monSkillDB.Find(skill => skill.id == id);
        }

        public List<CharacterSkill> GetCharacterAllSkills(int charNumber)
        {
            // charNumber 
            // 1: Knight, 2: Marksman, 3: Mage
            charNumber += 100;
            return charSkillDB.FindAll(skill => skill.id/100 == charNumber);
        }

        public List<CharacterSkill> GetCharacterDices(int id)
        {
            Character character = CharacterDatabase.Instance.GetCharacter(id);

            if(character == null) return null;

            List<CharacterSkill> characterSkills = new List<CharacterSkill>();
            foreach (CharacterSkill skill in character.mySkills)
            {
                characterSkills.Add(skill);
            }

            return characterSkills;
        }
        public List<MonsterSkill> GetMonsterDices(int id)
        {
            Monster monster = MonsterDatabase.Instance.GetMonster(id);

            if (monster == null) return null;

            List<MonsterSkill> monsterSkills = new List<MonsterSkill>();
            foreach (int skillId in monster.diceSkills)
            {
                monsterSkills.Add(GetMonsterSkill(skillId));
            }

            return monsterSkills;
        }

        public List<MonsterSkill> GetMonsterAttackSkills(int id, int round)
        {
            Monster monster = MonsterDatabase.Instance.GetMonster(id);

            if (monster == null) return null;

            List<MonsterSkill> monsterSkills = new List<MonsterSkill>();
            foreach (int skillId in monster.attackSkills)
            {
                MonsterSkill skill = GetMonsterSkill(skillId);
                if (skill.tier <= round)
                    monsterSkills.Add(skill);

                if (monsterSkills.Count >= 3) break;
            }

            return monsterSkills;
        }
    }
}
