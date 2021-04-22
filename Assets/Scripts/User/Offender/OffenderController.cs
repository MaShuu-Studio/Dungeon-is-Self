using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace GameControl
{
    public class OffenderController : MonoBehaviour
    {
        #region Instance

        
        private static OffenderController instance;
        public static OffenderController Instance
        {
            get
            {
                var obj = FindObjectOfType<OffenderController>();
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
        }
        #endregion
        public string[] selectedCharacterCandidates {get; private set;} = new string[6];
        private List<Character> character = new List<Character>();
        private List<CharacterSkill> attackSkills = new List<CharacterSkill>();
        private int characterIndex;
        public List<string> bench = new List<string>();
        public List<string> roster = new List<string>();
        
        public void Init()
        {
            character.Clear();

            foreach(string name in selectedCharacterCandidates)
            {
                character.Add(CharacterDatabase.Instance.GetCharacter(name));
            }

            characterIndex = 0;

            attackSkills.Clear();

            for(int i = 0; i < character.Count; i++)
            {
                CharacterSkill[] dice = new CharacterSkill[6];
                CharacterSkill attackSkill;
                //character[i].SetBasicDice(ref dice);
                //attackSkill = character[i].GetBasicSkill();

                //dices.Add(dice);
                //attackSkill.Add(attackSkill);
                
            }
        }

        #region Ready Game
        public void SetCharacterCandidate(int num, string name)
        {
            selectedCharacterCandidates[num] = name;
        }

        public bool CheckCadndidate()
        {
            foreach (string s in selectedCharacterCandidates)
            {
                if (string.IsNullOrEmpty(s)) return false;
            }
            return true;
        }
        #endregion
        
        #region Ready Round
        public void SelectCharacter(int index)
        {
            characterIndex = index;
        }

        public void SetDice(int n, int id)
        {
            int count = 0;
            character[n].dice.Add(SkillDatabase.Instance.GetCharacterSkill(id));
        }
        #endregion
        
        public void SetBench(string name)
        {
            if (bench.Count >= 3) return;
            else bench.Add(name);
        }
        public void SetRoster(string name)
        {
            if (roster.Count >= 3) return;
            else { roster.Add(name); character.Add(new Character(name)); }
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

        // Roster �ֻ���
        public void AllDiceThrow(int a, int b, int c)
        {
            OneDiceThrow(a);
            OneDiceThrow(b);
            OneDiceThrow(c);
        }

        public void SetSkillUpdate()
        {

        }
    }
}
