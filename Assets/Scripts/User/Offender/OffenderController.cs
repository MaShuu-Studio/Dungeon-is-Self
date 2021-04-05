using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Offender
{
    public enum Role { FIGHTER, MARKSMAN, MAGE }
    public class OffenderController : MonoBehaviour
    {
        private List<Character> character = new List<Character>();
        public List<Role> roster = new List<Role>();
        public List<Role> bench = new List<Role>();

        public Role fighter = Role.FIGHTER;
        public Role marksman = Role.MARKSMAN;
        public Role mage = Role.MAGE;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public void SetBench(Role role)
        {
            if(bench.Count >= 3) return;
            else {bench.Add(role); character.Add(new Character(role));}
        }
        public void SetRoster(Role role)
        {
            if(roster.Count >= 3) return;
            else{ roster.Add(role); character.Add(new Character(role));}
        }

        public void SetDice(int n, int id)
        {
            //if (character[n].dice.Count >= 6) return;
            //else 
            character[n].dice.Add(SkillDatabase.GetCharacterSkill(id));
        }

        public void ResetDice(int n)
        {
            character[n].dice.RemoveRange(0, 6);
        }

        public CharacterSkill OneDiceThrow(int n)
        {
            int i = Random.Range(0, 6);
            return character[n].dice[i];
        }

        public void AllDiceThrow(int a, int b, int c)
        {
            OneDiceThrow(a);
            OneDiceThrow(b);
            OneDiceThrow(c);
        }

        public void SetSkillUpdate()
        {

        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 50, 50), "Fighter"))
            {
                SetBench(fighter);
            }

            if (GUI.Button(new Rect(10, 80, 50, 50), "Marksman"))
            {
                SetBench(marksman);
            }

            if (GUI.Button(new Rect(80, 80, 50, 50), "Mage"))
            {
                SetBench(mage);
            }

            /*if (GUI.Button(new Rect(150, 150, 50, 50), "SetDice0"))
            {
                SetDice(0, 100);
                
            }    
            if(character.Count > 0)
            {
                if(character[0].dice.Count > 0) {
                    GUI.Box(new Rect(210, 210, 100, 50), character[0].dice[0].name);
                }
            } */
            //GUI.Box(new Rect(210, 210, 100, 50), "sex");

        }
    }
}
