using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class SkillDatabase : MonoBehaviour
    {
        private static List<CharacterSkill> charSkillDB;
        private static List<MonsterSkill> monSkillDB;
        // Start is called before the first frame update
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
            InitializeCharacterSkill();
            InitializeMonsterSkill();
        }

        private void InitializeCharacterSkill()
        {
            charSkillDB = new List<CharacterSkill>();
            #region Fighter Skill
            //fighter skill
            charSkillDB.Add(new CharacterSkill(10100, 0, "FAttack", 0, 3));
            charSkillDB.Add(new CharacterSkill(10101, 1, "Stab", 1, 0));
            charSkillDB.Add(new CharacterSkill(10102, 1, "Mockery", 1, 7));
            charSkillDB.Add(new CharacterSkill(10103, 2, "SteelShield", 1, 0));
            charSkillDB.Add(new CharacterSkill(10104, 2, "ShieldAttack", 1, 5));
            charSkillDB.Add(new CharacterSkill(10105, 2, "Harpoon", 1, 100));
            charSkillDB.Add(new CharacterSkill(10106, 2, "BrutalCrying", 2, 4));
            charSkillDB.Add(new CharacterSkill(10107, 3, "CounterAttack", 0, 0, (new List<int> { 10103, 10104 })));
            charSkillDB.Add(new CharacterSkill(10108, 3, "Brave", 2, 0, (new List<int> { 10106 })));
            charSkillDB.Add(new CharacterSkill(10109, 4, "Deadlock", 1, 0, (new List<int> { 10107 })));
            charSkillDB.Add(new CharacterSkill(10110, 4, "Roar", 3, 4, (new List<int> { 10108 })));
            charSkillDB.Add(new CharacterSkill(10111, 4, "ThornArmor", 3, 0, (new List<int> { 10105, 10107 })));
            #endregion
            #region Marksman Skill
            //marksman skill
            charSkillDB.Add(new CharacterSkill(10200, 0, "MarkAttack", 0, 3));
            charSkillDB.Add(new CharacterSkill(10201, 1, "NetArrow", 0, 4));
            charSkillDB.Add(new CharacterSkill(10202, 1, "DoubleShot", 1, 9));
            charSkillDB.Add(new CharacterSkill(10203, 2, "SawedArrow", 1, 6));
            charSkillDB.Add(new CharacterSkill(10204, 2, "PowerShot", 1, 10));
            charSkillDB.Add(new CharacterSkill(10205, 2, "TripleShot", 1, 7));
            charSkillDB.Add(new CharacterSkill(10206, 2, "ArrowheadChange", 2, 12));
            charSkillDB.Add(new CharacterSkill(10207, 3, "PoisonArrow", 2, 15, (new List<int> { 10203 })));
            charSkillDB.Add(new CharacterSkill(10208, 3, "Sniping", 1, 0, (new List<int> { 10204, 10206 })));
            charSkillDB.Add(new CharacterSkill(10209, 3, "RainArrow", 2, 10, (new List<int> { 10205 })));
            charSkillDB.Add(new CharacterSkill(10210, 3, "Scarecrow", 2, 10, (new List<int> { 10206 })));
            charSkillDB.Add(new CharacterSkill(10211, 4, "HeadShot", 2, 10, (new List<int> { 10208 })));
            #endregion
            #region Mage Skill
            //mage skill
            charSkillDB.Add(new CharacterSkill(10300, 0, "MageAttack", 0, 2));
            charSkillDB.Add(new CharacterSkill(10301, 1, "Lightening", 2, 14));
            charSkillDB.Add(new CharacterSkill(10302, 1, "Icicle", 1, 8));
            charSkillDB.Add(new CharacterSkill(10303, 1, "FireBall", 1, 10));
            charSkillDB.Add(new CharacterSkill(10304, 2, "LighteningClouds", 2, 0));
            charSkillDB.Add(new CharacterSkill(10305, 2, "FrozenPrison", 2, 18));
            charSkillDB.Add(new CharacterSkill(10306, 2, "Blaze", 1, 10));
            charSkillDB.Add(new CharacterSkill(10307, 3, "ElectricShock", 1, 14, (new List<int> { 10304 })));
            charSkillDB.Add(new CharacterSkill(10308, 3, "Blizzard", 1, 2, (new List<int> { 10305 })));
            charSkillDB.Add(new CharacterSkill(10309, 3, "FireWall", 3, 25, (new List<int> { 10306 })));
            charSkillDB.Add(new CharacterSkill(10310, 4, "ThunderDragon", 2, 18, (new List<int> { 10307, 10309 })));
            charSkillDB.Add(new CharacterSkill(10311, 4, "IceAge", 2, 0, (new List<int> { 10307, 10308 })));
            charSkillDB.Add(new CharacterSkill(10312, 4, "Meteor", 2, 0, (new List<int> { 10308, 10309 })));
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

            #region Attack One
            monSkillDB.Add(new MonsterSkill(22101, "SKILL1-1", 10, 0));
            monSkillDB.Add(new MonsterSkill(23101, "SKILL1-2", 4, 0));
            monSkillDB.Add(new MonsterSkill(23102, "SKILL1-3", 6, 0));
            #endregion


        }

        public CharacterSkill GetCharacterSkill(string name)
        {
            return charSkillDB.Find(skill => skill.name == name);
        }
        public CharacterSkill GetCharacterSkill(int id)
        {
            return charSkillDB.Find(skill => skill.id == id);
        }

        public MonsterSkill GetMonsterSkill(string name)
        {
            return monSkillDB.Find(skill => skill.name == name);
        }

        public List<CharacterSkill> GetCharacterDices(string name)
        {
            Character character = CharacterDatabase.Instance.GetCharacter(name);

            if(character == null) return null;

            List<CharacterSkill> characterSkills = new List<CharacterSkill>();
            foreach (CharacterSkill skill in character.mySkills)
            {
                characterSkills.Add(skill);
            }

            return characterSkills;
        }
        public List<MonsterSkill> GetMonsterDices(string name)
        {
            Monster monster = MonsterDatabase.Instance.GetMonster(name);

            if (monster == null) return null;

            List<MonsterSkill> monsterSkills = new List<MonsterSkill>();
            foreach (string skillName in monster.diceSkills)
            {
                monsterSkills.Add(GetMonsterSkill(skillName));
            }

            return monsterSkills;
        }

        public List<MonsterSkill> GetMonsterAttackSkills(string name, int round)
        {
            Monster monster = MonsterDatabase.Instance.GetMonster(name);

            if (monster == null) return null;

            List<MonsterSkill> monsterSkills = new List<MonsterSkill>();
            foreach (string skillName in monster.attackSkills)
            {
                MonsterSkill skill = GetMonsterSkill(skillName);
                if (skill.tier <= round)
                    monsterSkills.Add(skill);

                if (monsterSkills.Count >= 3) break;
            }

            return monsterSkills;
        }
    }
}
