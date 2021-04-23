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
        public List<Character> character {get; private set;} = new List<Character>();
        private List<CharacterSkill[]> dices = new List<CharacterSkill[]>();
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

            dices.Clear();
            attackSkills.Clear();

            for(int i = 0; i < character.Count; i++)
            {
                CharacterSkill[] dice = new CharacterSkill[6];
                character[i].SetBasicDice(ref dice);
                
                dices.Add(dice);              
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

        public bool SetDice(int index, CharacterSkill skill)
        {
            int count = 0;
            for (int i = 0; i < dices[characterIndex].Length; i++)
            {
                if (dices[characterIndex][i].id == skill.id) count++;
            }
            if (count > 1) return false;

            dices[characterIndex][index] = skill;
            return true;
        }

        public void SetAttackSkill(CharacterSkill skill)
        {
            attackSkills[characterIndex] = skill;
        }

        public CharacterSkill GetCharacterSkill(CharacterSkill skill)
        {
            return attackSkills[characterIndex];
        }
        
        public void SetRoster()
        {
            int[] unit = new int[1];
            unit[0] = characterIndex;
            GameController.Instance.SelectUnit(UserType.Offender, unit);
        }
        #endregion

        public CharacterSkill GetSelectedDice(int index)
        {
            return dices[characterIndex][index];
        }

        public CharacterSkill[] DiceRoll(int index)
        {
            CharacterSkill[] skills = new CharacterSkill[2];
            int diceIndex1 = Random.Range(0, 6);
            int diceIndex2 = Random.Range(0, 6);
            skills[0] = dices[index][diceIndex1];
            skills[1] = dices[index][diceIndex2];

            return skills;
        }

        public string GetCharacterRoster()
        {
            return character[characterIndex]._role;
        }
        /*public void SetBench(string name)
        {
            if (bench.Count >= 3) return;
            else bench.Add(name);
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

        }*/
    }
}
