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
            charSkillDB.Add(new CharacterSkill(10100, "FAttack", 0, 3));
            charSkillDB.Add(new CharacterSkill(10101, "SteelShield", 1, 0));
            charSkillDB.Add(new CharacterSkill(10102, "Stab", 1, 7));
            charSkillDB.Add(new CharacterSkill(10103, "Mockery", 1, 0));
            charSkillDB.Add(new CharacterSkill(10104, "ShieldAttack", 1, 5, (new List<int> { 10101 }), 2));
            charSkillDB.Add(new CharacterSkill(10105, "Harpoon", 1, 100, (new List<int> { 10102 }), 2));
            charSkillDB.Add(new CharacterSkill(10106, "CounterAttack", 2, 4, (new List<int> { 10101, 10102 }), 2));
            charSkillDB.Add(new CharacterSkill(10107, "BrutalCrying", 0, 0, (new List<int> { 10103 }), 2));
            charSkillDB.Add(new CharacterSkill(10108, "Brave", 2, 0, (new List<int> { 10104 }), 3));
            charSkillDB.Add(new CharacterSkill(10109, "Roar", 1, 0, (new List<int> { 10107 }), 3));
            charSkillDB.Add(new CharacterSkill(10110, "Deadlock", 3, 4, (new List<int> { 10104, 10106 }), 3));
            charSkillDB.Add(new CharacterSkill(10111, "SuperArmor", 3, 0, (new List<int> { 10108 }), 3));
            #endregion
            #region Marksman Skill
            //marksman skill
            charSkillDB.Add(new CharacterSkill(10200, "MarkAttack", 0, 3));
            charSkillDB.Add(new CharacterSkill(10201, "NetArrow", 0, 4));
            charSkillDB.Add(new CharacterSkill(10202, "DoubleShot", 1, 9));
            charSkillDB.Add(new CharacterSkill(10203, "SawedArrow", 1, 6));
            charSkillDB.Add(new CharacterSkill(10204, "PowerShot", 1, 10, (new List<int> { 10201, 10202 }), 2));
            charSkillDB.Add(new CharacterSkill(10205, "PoisonArrow", 1, 7, (new List<int> { 10203 }), 2));
            charSkillDB.Add(new CharacterSkill(10206, "RainArrow", 2, 12, (new List<int> { 10203, 10204 }), 2));
            charSkillDB.Add(new CharacterSkill(10207, "BowMaster", 2, 15, (new List<int> { 10204 }), 2));
            charSkillDB.Add(new CharacterSkill(10208, "Scarecrow", 1, 0, (new List<int> { 10205 }), 3));
            charSkillDB.Add(new CharacterSkill(10209, "ParalyticArrow", 2, 10, (new List<int> { 10204, 10205 }), 3));
            #endregion
            #region Mage Skill
            //mage skill
            charSkillDB.Add(new CharacterSkill(10300, "MageAttack", 0, 2));
            charSkillDB.Add(new CharacterSkill(10301, "ElectricShock", 2, 14));
            charSkillDB.Add(new CharacterSkill(10302, "MagneticPrison", 1, 8));
            charSkillDB.Add(new CharacterSkill(10303, "Lightening", 1, 10));
            charSkillDB.Add(new CharacterSkill(10304, "Purify", 2, 0));
            charSkillDB.Add(new CharacterSkill(10305, "LighteningClouds", 2, 18, (new List<int> { 10301, 10303 }), 2));
            charSkillDB.Add(new CharacterSkill(10306, "FrozenPrison", 1, 10, (new List<int> { 10302, 10303 }), 2));
            charSkillDB.Add(new CharacterSkill(10307, "Blaze", 1, 14, (new List<int> { 10301, 10303 }), 2));
            charSkillDB.Add(new CharacterSkill(10308, "Imprison", 1, 2, (new List<int> { 10304 }), 2));
            charSkillDB.Add(new CharacterSkill(10309, "ThunderDragon", 3, 25, (new List<int> { 10305, 10307 }), 3));
            charSkillDB.Add(new CharacterSkill(10310, "FrozenGraves", 2, 18, (new List<int> { 10306 }), 3));
            charSkillDB.Add(new CharacterSkill(10311, "FrostDecoy", 2, 0, (new List<int> { 10306 }), 3));
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
            monSkillDB.Add(new MonsterSkill(21201, "DICE2-1", 5, 2));
            monSkillDB.Add(new MonsterSkill(21202, "DICE2-2", 5, 2));
            monSkillDB.Add(new MonsterSkill(21203, "DICE2-3", 5, 3));
            monSkillDB.Add(new MonsterSkill(21301, "DICE3-1", 5, 5));
            monSkillDB.Add(new MonsterSkill(21302, "DICE3-2", 5, 6));
            monSkillDB.Add(new MonsterSkill(21303, "DICE3-3", 5, 7));
            #endregion

            #region Attack One
            monSkillDB.Add(new MonsterSkill(22101, "SKILL1-1", 5, 0));
            monSkillDB.Add(new MonsterSkill(23101, "SKILL1-2", 2, 0));
            monSkillDB.Add(new MonsterSkill(23102, "SKILL1-3", 5, 0));
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
