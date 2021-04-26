using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class Character
    {
        public string _role {get; private set;}
        private int skillpoint;
        public List<CharacterSkill> mySkills {get; private set;} = new List<CharacterSkill>();

        public int GetSkillPoint()
        {
            return skillpoint;
        }

        public Character(string role)
        {
            _role = role;
            if(_role == "FIGHTER") {for(int i = 10100; i < 10112; i++) {mySkills.Add(SkillDatabase.Instance.GetCharacterSkill(i));}}
            else if(_role == "MARKSMAN") {for(int i = 10200; i < 10210; i++) {mySkills.Add(SkillDatabase.Instance.GetCharacterSkill(i));}}
            else {for(int i = 10300; i < 10312; i++) {mySkills.Add(SkillDatabase.Instance.GetCharacterSkill(i));}}
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

            for(int i = 0; i < dice.Length; i++)
            {
                dice[i] = tier1dices[i % (tier1dices.Count)];
            }
        }
    }
    
}