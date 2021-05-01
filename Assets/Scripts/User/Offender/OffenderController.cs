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

        public int[] selectedCharacterCandidates { get; private set; } = new int[6];
        public List<Character> characters { get; private set; } = new List<Character>();
        public List<bool> isDead { get; private set; } = new List<bool>();
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
            gottenSkills.Clear();

            foreach (int id in selectedCharacterCandidates)
            {
                Character c = CharacterDatabase.Instance.GetCharacter(id);
                characters.Add(c);
                isDead.Add(false);

                List<bool> list = new List<bool>();
                for (int i = 0; i < c.mySkills.Count; i++)
                {
                    if (c.mySkills[i].tier <= 1) list.Add(true);
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

        public void SetCharacterCandidate(int num, int id)
        {
            selectedCharacterCandidates[num] = id;
        }

        public bool CheckCadndidate()
        {
            foreach (int id in selectedCharacterCandidates)
            {
                if (id == -1) return false;
            }
            return true;
        }

        public void ResetCandidates()
        {
            for (int i = 0; i < selectedCharacterCandidates.Length; i++)
            {
                selectedCharacterCandidates[i] = -1;
            }
        }

        public void ResetDead()
        {
            for (int i = 0; i < isDead.Count; i++) isDead[i] = false;
        }
        #endregion

        #region Ready Round
        public int GetFirstAliveCharacter()
        {
            int i = 0;
            for (; i < characters.Count; i++)
            {
                if (isDead[i] == false) break;
            }
            return i;
        }
        public List<int> GetAliveCharacterList()
        {
            List<int> indexes = new List<int>();
            int i = 0;
            for (; i < characters.Count; i++)
            {
                if (isDead[i] == false)
                {
                    indexes.Add(i);
                }
            }
            return indexes;
        }
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

        public int LearnSkill(CharacterSkill skill)
        {
            Character c = characters[characterIndex];
            int index = c.mySkills.FindIndex(s => s.id == skill.id);

            if (index != -1)
            {
                if (skillPoints[characterIndex] <= 0) return -1;
                foreach (int i in skill.prior)
                {
                    if (!IsSkillGotten(i % 100)) return -2;
                }
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
                if (dices[characterIndex][i].id == skill.id && i != index) overlabCount++;

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

        #region Play Round
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

        public List<int> GetCharacterRoster()
        {
            List<int> tmp = new List<int>();
            foreach (int i in roster)
            {
                tmp.Add(characters[i].id);
            }
            return tmp;
        }

        public List<int> GetUpgradableSkill()
        {
            List<int> upgradableSkills = new List<int>();
            bool check;
            for (int i = 0; i < characters[characterIndex].mySkills.Count; i++)
            {
                if (!IsSkillGotten(i))
                {
                    check = true;
                    foreach (int j in characters[characterIndex].mySkills[i].prior)
                        if (IsSkillGotten(j % 100) == false) check = false;
                    if (check == true) upgradableSkills.Add(i);
                }
            }

            return upgradableSkills;
        }

        public void Dead(int index)
        {
            isDead[index % 10] = true;
        }
        public void Alive(int index)
        {
            isDead[index % 10] = false;
        }

        #endregion
    }
}
