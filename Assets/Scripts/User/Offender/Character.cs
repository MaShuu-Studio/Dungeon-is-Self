using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum Role { FIGHTER, MARKSMAN, MAGE }
    public class Character
    {
        public List<CharacterSkill> dice = new List<CharacterSkill>();
        private Role _role;
        private int skillpoint;
        public List<CharacterSkill> myskill = new List<CharacterSkill>();
        //private List<string> skill = new List<string>();
        // Start is called before the first frame update
        public void SetSkillUpdate()
        {
            if(skillpoint > 0)
            {

            }
        }

        public int GetSkillPoint()
        {
            return skillpoint;
        }

        public Character(Role role)
        {
            _role = role;
            if(_role == Role.FIGHTER) {for(int i = 100; i < 112; i++) {myskill.Add(SkillDatabase.Instance.GetCharacterSkill(i));}}
            else if(_role == Role.MARKSMAN) {for(int i = 200; i < 210; i++) {myskill.Add(SkillDatabase.Instance.GetCharacterSkill(i));}}
            else {for(int i = 300; i < 312; i++) {myskill.Add(SkillDatabase.Instance.GetCharacterSkill(i));}}
        }
        /*public string GetDiceInfo()
        {
            //return dice[0].name;
        }*/
    }
    
}