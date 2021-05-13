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
        private List<List<CharacterSkill>> skillRoster = new List<List<CharacterSkill>>();
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

        public void AddRoster(int pos, int newRoster)
        {
            if (isDead[newRoster]) return;

            if (pos >= 3 || pos < 0) return;
            roster[pos] = newRoster;
        }

        public int GetMaxTier()
        {
            return characters[characterIndex].GetMaxTier();
        }

        public int GetGottenSkillsMaxTier()
        {
            int tier = 0;
            for (int i = characters[characterIndex].mySkills.Count - 1; i > 0; i--)
            {
                if (IsSkillGotten(i))
                {
                    tier = characters[characterIndex].mySkills[i].tier;
                    break;
                }
            }

            return tier;
        }

        public int SetDice(int index, int skillRosteridx)
        {
            //int overlabCount = 0;
            int basicSkill = 0;
            for (int i = 0; i < dices[characterIndex].Length; i++)
            {
                if (dices[characterIndex][i].id % 100 == 0) { basicSkill++; continue; }
                //if (dices[characterIndex][i].id == skill.id && i != index) overlabCount++;

            }
            if ((basicSkill <= 1 && skillRoster[characterIndex][skillRosteridx].id % 100 != 0) || (basicSkill == 2 && dices[characterIndex][index].id % 100 == 0 && skillRoster[characterIndex][skillRosteridx].id % 100 != 0)) return 1;
            //if (overlabCount > 2) return 2;
            skillRoster[characterIndex].Add(dices[characterIndex][index]);
            dices[characterIndex][index] = skillRoster[characterIndex][skillRosteridx];
            skillRoster[characterIndex].RemoveAt(skillRosteridx);
            //dices[characterIndex][index] = skill;
            return 0;
        }

        public int SetSkillRoster(CharacterSkill skill)
        {
            int overlabCount = 0;
            for (int i = 0; i < skillRoster.Count; i++)
            {
                if (skillRoster[characterIndex][i].id == skill.id) overlabCount++;
            }
            if (overlabCount > 2) return 2;

            skillRoster[characterIndex].Add(skill);
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

        public CharacterSkill DiceRoll(int roster, bool isParalysis)
        {
            
            int[] result = new int[6]{0, 0, 0, 0, 0, 0};
            int diceIndex = new int();
            
            for (int n = 0; n < 5; n++)
            {
                diceIndex = Random.Range(0, dices[roster % 10].Length);
                if (isParalysis)
                {
                    int blindAmount = 3;
                    List<int> blind = new List<int>();
                    for (int i = 0; i < blindAmount; i++)
                        while (true)
                        {
                            int index = Random.Range(0, dices[roster % 10].Length);
                            if (blind.FindIndex(n => n == index) == -1) break;
                        }
                    while (true)
                    {
                        diceIndex = Random.Range(0, dices[roster % 10].Length);
                        if (blind.FindIndex(n => n == diceIndex) == -1) break;
                    }
                }
                int check = 0;
                for (int i = 0; i < 6; i++)
                {
                    if(result[i] > 0 && (dices[roster % 10][i].id == dices[roster % 10][diceIndex].id)) { check = 1; result[i] += 1; break;}
                }
                if (check == 0) result[diceIndex] += 1;

                if (n >= 4)
                {
                    int max = 0;
                    for (int i = 1; i < 6; i++)
                    {
                        if (result[i] > max) max = result[i];
                    }
                    if (max >= 3)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (result[i] == max) { diceIndex = i; break; }
                        }
                    }
                    else if (max == 2)
                    {
                        int[] tmp = new int[2]{-1, -1};
                        for (int i = 0; i < 6; i++)
                        {
                            if (tmp[0] == -1 && result[i] == max) tmp[0] = i;
                            else if (tmp[0] > -1 && result[i] == max) tmp[1] = i;
                        }
                        if (tmp[1] == -1) diceIndex = tmp[0];
                        else
                        {
                            int t = Random.Range(0, 2);
                            diceIndex = tmp[t];
                        }
                    }
                    else if (max == 1)
                    {
                        while(true)
                        {
                            int t = Random.Range(0, 6);
                            if (result[t] == 1) { diceIndex = t; break;}
                        }
                    }
                }
            }
            return dices[roster % 10][diceIndex];
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

        public int GetDeadUnitCount()
        {
            int count = 0;
            foreach (bool b in isDead)
                if (b) count++;

            return count;
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
