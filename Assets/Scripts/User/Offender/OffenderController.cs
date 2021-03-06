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
        private List<List<int>> dices = new List<List<int>>();
        private List<List<CharacterSkill>> skillRoster = new List<List<CharacterSkill>>();
        private int characterIndex = 0;
        public int[] roster { get; private set; } = new int[3];

        public void Reset()
        {
            characterIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                roster[i] = i;
            }

            ResetCandidates();

            characters.Clear();
            skillPoints.Clear();
            gottenSkills.Clear();

            isDead.Clear();

            dices.Clear();
            skillRoster.Clear();
        }
        public void ResetRound()
        {
            for (int i = 0; i < skillRoster.Count; i++)
            {
                dices[i].Clear();
                skillRoster[i].Clear();

                dices[i].Add(-1);
                dices[i].Add(-1);
            }
        }

        public void ResetTurn()
        {
            for (int i = 0; i < skillRoster.Count; i++)
            {
                dices[i].Clear();

                dices[i].Add(-1);
                dices[i].Add(-1);
            }
        }
        public void Init()
        {
            characterIndex = 0;

            characters.Clear();
            skillPoints.Clear();
            gottenSkills.Clear();

            isDead.Clear();
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
            skillRoster.Clear();

            for (int i = 0; i < characters.Count; i++)
            {
                List<CharacterSkill> skillCand = new List<CharacterSkill>();
                List<int> diceCand = new List<int>();

                skillRoster.Add(skillCand);
                dices.Add(diceCand);
                
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

        public void CandidatesTimeOut()
        {
            List<int> list = new List<int>();
            CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref list);

            for (int i = 0; i < selectedCharacterCandidates.Length; i++)
            {
                int random = Random.Range(0, list.Count);
                if (selectedCharacterCandidates[i] == -1)
                    selectedCharacterCandidates[i] = list[random];
            }
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
            // ????????? ?????? ??????
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

        public bool HasDice(int i)
        {
            return (dices[characterIndex].FindIndex(index => index == i) != -1);
        }

        public bool SetDice(bool isRosterToDice, int skillIdx)
        {
            if (isRosterToDice == true)//roster to dice
            {
                if (GetDiceSize() >= 6) return false;

                dices[characterIndex].Add(skillIdx);

                return true;
            }
            else//dice to roster
            {
                if (GetDiceSize() <= 0) return false;
                if (GetDiceSize() < skillIdx + 1) return false;
                if (skillIdx < 2) return false;
                dices[characterIndex].RemoveAt(skillIdx);

                return true;
            }            
        }

        public int SetSkillRoster(CharacterSkill skill)
        {
            if (skillRoster[characterIndex].Count >= 8) return 30;

            int overlabCount = 0;
            for (int i = 0; i < skillRoster[characterIndex].Count; i++)
            {
                if (skillRoster[characterIndex][i].id == skill.id && skill.id % 100 != 0) overlabCount++;
            }
            if (overlabCount > 2) return 27;

            skillRoster[characterIndex].Add(skill);
            return 0;
        }

        public List<int> GetSkillRosterWithUnit(int unitIndex)
        {
            List<int> skills = new List<int>();
            foreach (CharacterSkill skill in skillRoster[unitIndex])
            {
                skills.Add(skill.id);
            }
            return skills;
        }

        public void RosterTimeOut()
        {
            int selectedUnit = characterIndex;
            for (int i = 0; i < 3; i++)
            {
                SelectCharacter(roster[i]);
                Character c = characters[roster[i]];
                List<int> usableSkill = new List<int>();

                for (int j = 0; j < c.mySkills.Count; j++)
                {
                    if (IsSkillGotten(j))
                    {
                        usableSkill.Add(j);
                    }
                }

                for (int j = GetSkillRosterSize(); j < 8; j++)
                {

                    while (true)
                    {
                        int n = UnityEngine.Random.Range(0, usableSkill.Count);
                        if (SetSkillRoster(c.mySkills[usableSkill[n]]) == 0) break;
                    }
                }
            }

            characterIndex = selectedUnit;
        }

        public void DiceTimeOut()
        {
            int selectedUnit = characterIndex;
            for (int i = 0; i < 3; i++)
            {
                SelectCharacter(roster[i]);
                Character c = characters[roster[i]];

                for (int j = GetDiceSize(); j < 6; j++)
                {
                    while (true)
                    {
                        int n = UnityEngine.Random.Range(0, GetSkillRosterSize());
                        if (dices[roster[i]].Contains(n)) continue; 
                        if (SetDice(true, n)) break;
                    }
                }
            }
            characterIndex = selectedUnit;
        }

        public void ResetDices()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                dices[i].Clear();
                dices[i].Add(-1);
                dices[i].Add(-1);
            }
        }

        public CharacterSkill GetSkillRoster(int i)
        {
            if (i < 0) return SkillDatabase.Instance.GetCharacterBasicSkill(selectedCharacterCandidates[characterIndex]);
            return skillRoster[characterIndex][i];
        }

        public int GetSkillRosterSize()
        {
            return skillRoster[characterIndex].Count;
        }

        public List<int> GetDicesWithUnit(int unitIndex)
        {
            return dices[unitIndex];
        }

        public int GetDiceSize()
        {
            return dices[characterIndex].Count;
        }

        public void RemoveSkillRoster(int index) 
        {
            if (GetSkillRosterSize() < index + 1) return;
            skillRoster[characterIndex].RemoveAt(index);
        }

        public void SetSkillRoster(List<S_RoundReadyEnd.EnemyRoster> infos)
        {
            for (int i = 0; i < infos.Count; i++)
            {
                roster[i] = infos[i].unitIndex % 10;
                skillRoster[i].Clear();
                for (int j = 0; j < infos[i].skillRosters.Count; j++)
                {
                    skillRoster[roster[i]].Add(SkillDatabase.Instance.GetCharacterSkill(infos[i].skillRosters[j]));
                }
            }
        }

        public void SetRoster()
        {
            GameController.Instance.SelectUnit(UserType.Offender, roster);
        }
        #endregion

        #region Play Round
        public int GetSelectedDice(int index)
        {
            if (dices.Count <= characterIndex) return 0;
            return dices[characterIndex][index];
        }

        /*        
        public CharacterSkill DiceRoll(int roster, bool isParalysis)
        {
            
            int[] result = new int[6]{0, 0, 0, 0, 0, 0};
            int diceIndex = new int();
            
            for (int k = 0; k < 5; k++)
            {
                diceIndex = Random.Range(0, dices[roster % 10].Count);
                if (isParalysis)
                {
                    int blindAmount = 3;
                    List<int> blind = new List<int>();
                    for (int i = 0; i < blindAmount; i++)
                        while (true)
                        {
                            int index = Random.Range(0, dices[roster % 10].Count);
                            if (blind.FindIndex(n => n == index) == -1) break;
                        }
                    while (true)
                    {
                        diceIndex = Random.Range(0, dices[roster % 10].Count);
                        if (blind.FindIndex(n => n == diceIndex) == -1) break;
                    }
                }
                int check = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (result[i] > 0 && 
                        (dices[roster % 10][i].id == dices[roster % 10][diceIndex].id))
                    {
                        check = 1;
                        result[i] += 1;
                        break;
                    }
                }
                if (check == 0) result[diceIndex] += 1;

                if (k >= 4)
                {
                    int max = result[0];
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
            return skillRoster[0][0];
        }
        */
        

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

        public void KillUnits(List<int> deadUnits)
        {
            for (int i = 0; i < deadUnits.Count; i++)
            {
                isDead[deadUnits[i] % 10] = true;
            }
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
