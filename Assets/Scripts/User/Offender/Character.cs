using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class Character
    {
        public int id { get; private set; }
        public string _role { get; private set; }
        public List<CharacterSkill> mySkills { get; private set; }
        public Character(string role, int id)
        {
            _role = role;
            this.id = id;

            mySkills = new List<CharacterSkill>(SkillDatabase.Instance.GetCharacterAllSkills(id % 100));
        }

        public Character(Character character)
        {
            _role = character._role;
            id = character.id;

            mySkills = new List<CharacterSkill>(SkillDatabase.Instance.GetCharacterAllSkills(id % 100));
        }

        public void SetBasicDice(ref CharacterSkill[] dice)
        {
            List<CharacterSkill> tier0dices = new List<CharacterSkill>();
            List<CharacterSkill> tier1dices = new List<CharacterSkill>();

            foreach (CharacterSkill skill in mySkills)
            {
                if (skill != null && skill.tier == 0) tier0dices.Add(skill);
                if (skill != null && skill.tier == 1) tier1dices.Add(skill);
            }

            dice[0] = tier0dices[0];
            dice[1] = tier0dices[0];

            for (int i = 2; i < dice.Length; i++)
            {
                dice[i] = tier1dices[i % (tier1dices.Count)];
            }
        }

        public int GetMaxTier()
        {
            int max = 0;
            foreach (CharacterSkill skill in mySkills)
            {
                if (max < skill.tier) max = skill.tier;
            }

            return max;
        }
    }
}