using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class Character
    {
        public int id;
        public string _role { get; private set; }
        public List<CharacterSkill> mySkills { get; private set; }
        public Character(string role, int id)
        {
            _role = role;
            this.id = id;

            mySkills = SkillDatabase.Instance.GetCharacterAllSkills(id);
        }

        public Character(Character character)
        {
            _role = character._role;
            id = character.id;

            mySkills = SkillDatabase.Instance.GetCharacterAllSkills(id);
        }

        public void SetBasicDice(ref CharacterSkill[] dice)
        {
            List<CharacterSkill> tier1dices = new List<CharacterSkill>();

            foreach (CharacterSkill skill in mySkills)
            {
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