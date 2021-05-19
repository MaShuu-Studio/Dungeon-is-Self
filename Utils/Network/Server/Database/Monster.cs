using System;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public enum Element { NORMAL }
    public class Monster
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public int hp { get; private set; }
        public List<int> tierHp { get; private set; }
        public Element weak { get; private set; }
        public List<int> diceSkills { get; private set; }
        public List<int> attackSkills { get; private set; }
        public string prior { get; private set; }
        public int tier { get; private set; }

        public Monster(int id, int tier, string name, List<int> tierHp, Element weak, List<int> atkSkills, string prior = "")
        {
            this.id = id;
            this.name = name;
            this.tierHp = new List<int>(tierHp);
            this.weak = weak;

            attackSkills = new List<int>(atkSkills);
            diceSkills = new List<int>(SkillDatabase.Instance.GetMonsterAllSkills(id));
            // 각 몬스터의 특징을 가진 개별 스킬 이름 목록
            // TUPLE LIST로 받아도 괜찮을듯

            this.prior = prior;
            this.tier = tier;
        }
        public Monster(Monster monster)
        {
            this.id = monster.id;
            this.name = monster.name;
            this.tierHp = new List<int>(monster.tierHp);
            this.weak = monster.weak;

            attackSkills = new List<int>(monster.attackSkills);
            diceSkills = new List<int>(monster.diceSkills);
            // 각 몬스터의 특징을 가진 개별 스킬 이름 목록
            // TUPLE LIST로 받아도 괜찮을듯

            this.prior = monster.prior;
            this.tier = monster.tier;
        }

        /*public void SetBasicDice(ref MonsterSkill[] dice)
        {
            List<MonsterSkill> tier1dices = new List<MonsterSkill>();

            foreach (int id in diceSkills)
            {
                MonsterSkill skill = SkillDatabase.Instance.GetMonsterSkill(id);
                if (skill != null && skill.tier == 1)
                {
                    tier1dices.Add(skill);
                }
            }

            for (int i = 0; i < dice.Length; i++)
            {
                dice[i] = tier1dices[i % (tier1dices.Count)];
            }
        }*/

        public MonsterSkill GetBasicSkill()
        {
            MonsterSkill skill = SkillDatabase.Instance.GetMonsterSkill(attackSkills[0]);
            return skill;
        }

        public void Damaged(int damage)
        {
            hp -= damage;
        }

        public void Heal(int round)
        {
            tier = round - 1;
            if (round < 0) tier = 0;
            if (tier >= 3) tier = 2;
            hp = tierHp[tier];
        }
        public void Cure(int amount)
        {
            hp += amount;
        }
    }
}
