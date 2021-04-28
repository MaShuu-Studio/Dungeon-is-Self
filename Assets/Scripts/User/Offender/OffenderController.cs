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
        public List<Character> characters { get; private set; } = new List<Character>();
        private List<List<bool>> gottenSkills = new List<List<bool>>();
        public List<int> skillPoints { get; private set; } = new List<int>();
        private List<CharacterSkill[]> dices = new List<CharacterSkill[]>();
        private int characterIndex = 0;
        public int[] roster { get; private set; } = new int[3];

        public void Init()
        {
            characterIndex = 0;

            characters.Clear();
            skillPoints.Clear();

            foreach (string name in selectedCharacterCandidates)
            {
                Character c = CharacterDatabase.Instance.GetCharacter(name);
                characters.Add(c);

                List<bool> list = new List<bool>();
                for (int i = 0; i < c.mySkills.Count; i++)
                {
                    if (c.mySkills[i].tier == 1) list.Add(true);
                    else list.Add(false);
                }
                gottenSkills.Add(list);
                skillPoints.Add(0);
            }

            for (int i = 0; i < 3; i++)
            {
                roster[i] = i;
            }

            dices.Clear();

            for (int i = 0; i < characters.Count; i++)
            {
                CharacterSkill[] dice = new CharacterSkill[6];
                characters[i].SetBasicDice(ref dice);

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

        public void ResetCandidates()
        {
            for (int i = 0; i < selectedCharacterCandidates.Length; i++)
            {
                selectedCharacterCandidates[i] = "";
            }
        }
        #endregion

        #region Ready Round
        public void AddSkillPoint(int point)
        {
            for (int i = 0; i < skillPoints.Count; i++)
            {
                skillPoints[i] += point;
            }

            GamePlayUIController.Instance.UpdateSkillPoint();
        }

        public int GetSkillPoint()
        {
            return skillPoints[characterIndex];
        }

        public bool IsSkillGotten(int index)
        {
            return gottenSkills[characterIndex][index];
        }

        public int LearnSkill(Skill skill)
        {
            Character c = characters[characterIndex];
            int index = c.mySkills.FindIndex(s => s.id == skill.id);

            if (index != -1)
            {
                if (skillPoints[characterIndex] <= 0) return -1;
                skillPoints[characterIndex] -= 1;
                gottenSkills[characterIndex][index] = true;
            }

            return index;
        }

        public void SelectCharacter(int index)
        {
            characterIndex = index;
        }

        public void SelectRoster(int index)
        {
            int i = 0;
            // 로스터 중복 체크
            for (; i < roster.Length; i++)
            {
                if (roster[i] == characterIndex) break;
            }

            if (i < roster.Length) roster[i] = roster[index];
            roster[index] = characterIndex;
        }

        public int GetMaxTier()
        {
            return characters[characterIndex].GetMaxTier();
        }

        public int SetDice(int index, CharacterSkill skill)
        {
            int overlabCount = 0;
            int basicSkill = 0;
            for (int i = 0; i < dices[characterIndex].Length; i++)
            {
                if (dices[characterIndex][i].id % 100 == 0) { basicSkill++; continue; }
                if (dices[characterIndex][i].id == skill.id) overlabCount++;
                
            }
            if ((basicSkill <= 1 && skill.id % 100 != 0) || (basicSkill == 2 && dices[characterIndex][index].id % 100 == 0 && skill.id % 100 != 0)) return 1;
            if (overlabCount > 2) return 2;

            dices[characterIndex][index] = skill;
            return 0;
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

        public Dictionary<int, CharacterSkill> DiceRoll(int[] roster)
        {
            Dictionary<int, CharacterSkill> skills = new Dictionary<int, CharacterSkill>();

            for (int i = 0; i < roster.Length; i++)
            {
                int diceIndex = Random.Range(0, 6);
                skills.Add(roster[i], dices[roster[i] % 10][diceIndex]);
            }

            return skills;
        }

        public List<string> GetCharacterRoster()
        {
            List<string> tmp = new List<string>();
            foreach (int i in roster)
            {
                tmp.Add(characters[i]._role);
            }
            return tmp;
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
