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


        public string[] selectedCharacterCandidates { get; private set; } = new string[6];
        public List<Character> character { get; private set; } = new List<Character>();
        private List<CharacterSkill[]> dices = new List<CharacterSkill[]>();
        private int characterIndex;
        public int[] roster { get; private set; } = new int[3];

        public void Init()
        {
            character.Clear();

            foreach (string name in selectedCharacterCandidates)
            {
                character.Add(CharacterDatabase.Instance.GetCharacter(name));
            }

            for (int i = 0; i < 3; i++)
            {
                roster[i] = i;
            }

            characterIndex = 0;

            dices.Clear();

            for (int i = 0; i < character.Count; i++)
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

        public void SelectRoster(int index)
        {
            // 로스터 중복 체크
            roster[index] = characterIndex;
        }

        public int GetMaxTier()
        {
            return character[characterIndex].GetMaxTier();
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

        public void SetRoster()
        {
            GameController.Instance.SelectUnit(UserType.Offender, roster);
        }
        #endregion

        public CharacterSkill GetSelectedDice(int index)
        {
            if (dices.Count <= characterIndex) return null;
            return dices[characterIndex][index];
        }

        public List<CharacterSkill> DiceRoll(int[] roster)
        {
            List<CharacterSkill> skills = new List<CharacterSkill>();

            for (int i = 0; i < roster.Length; i++)
            {
                int diceIndex = Random.Range(0, 6);
                skills.Add(dices[roster[i]][diceIndex]);
            }

            return skills;
        }

        public List<string> GetCharacterRoster()
        {
            List<string> characters = new List<string>();
            foreach (int i in roster)
            {
                characters.Add(character[i]._role);
            }
            return characters;
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
