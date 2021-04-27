using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum Element { NORMAL }
    public class Monster
    {
        public string name { get; private set; }
        public int hp { get; private set; }
        public Element weak { get; private set; }
        public List<string> diceSkills { get; private set; }
        public List<string> attackSkills { get; private set; }
        public string prior { get; private set; }
        public int tier { get; private set; }

        public Monster(int tier, string name, int hp, Element weak, List<string> atkSkills, List<string> dices, string prior = "")
        {
            this.name = name;
            this.hp = hp;
            this.weak = weak;

            attackSkills = new List<string>(atkSkills);
            diceSkills = new List<string>(dices);
            // 각 몬스터의 특징을 가진 개별 스킬 이름 목록
            // TUPLE LIST로 받아도 괜찮을듯

            this.prior = prior;
            this.tier = tier;
        }


        public void SetBasicDice(ref MonsterSkill[] dice)
        {
            List<MonsterSkill> tier1dices = new List<MonsterSkill>();

            foreach (string name in diceSkills)
            {
                MonsterSkill skill = SkillDatabase.Instance.GetMonsterSkill(name);
                if (skill != null && skill.tier == 1)
                {
                    tier1dices.Add(skill);
                }
            }

            for (int i = 0; i < dice.Length; i++)
            {
                dice[i] = tier1dices[i % (tier1dices.Count)];
            }
        }

        public MonsterSkill GetBasicSkill()
        {
            MonsterSkill skill = SkillDatabase.Instance.GetMonsterSkill(attackSkills[0]);
            return skill;
        }

        public void Damaged(CharacterSkill skill)
        {
            hp -= skill.damage;
        }
    }
}
