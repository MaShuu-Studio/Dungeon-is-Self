using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
namespace GameControl
{
    public class AIBot : MonoBehaviour
    {
        #region Instance

        private static AIBot instance;
        public static AIBot Instance
        {
            get
            {
                var obj = FindObjectOfType<AIBot>();
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
        public void StartGame()
        {
            if (GameController.Instance.userType == UserType.Defender)
            {
                List<int> offenderBot = new List<int>();
                CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref offenderBot);

                for (int i = 0; i < 6; i++)
                {
                    int a = UnityEngine.Random.Range(0, offenderBot.Count);
                    OffenderController.Instance.SetCharacterCandidate(i, offenderBot[a]);
                }
            }
            else
            {
                List<int> defenderBot = new List<int>();
                MonsterDatabase.Instance.GetAllMonsterCandidatesList(ref defenderBot);
                for (int i = 0; i < 6; i++)
                {
                    int a = UnityEngine.Random.Range(0, defenderBot.Count);
                    DefenderController.Instance.SetMonsterCandidate(i, defenderBot[a]);
                }
            }
        }

        public void SetRoster()
        {
            if (GameController.Instance.userType == UserType.Defender)
            {
                List<int> roster = OffenderController.Instance.GetAliveCharacterList();
                while (roster.Count > 3)
                {
                    int index = Random.Range(0, roster.Count);
                    roster.RemoveAt(index);
                }

                for (int i = 0; i < 3; i++)
                {
                    OffenderController.Instance.SelectCharacter(roster[i]);
                    OffenderController.Instance.SelectRoster(i);
                }

                GameController.Instance.SelectUnit(UserType.Offender, OffenderController.Instance.roster);
            }
            else
            {
                List<int> roster = DefenderController.Instance.GetAliveMonsterList();
                int unit = UnityEngine.Random.Range(0, roster.Count);
                DefenderController.Instance.SelectMonster(roster[unit]);
                DefenderController.Instance.SetRoster();
            }
        }

        public void LearnSkill()
        {
            for (int i = 0; i < 6; i++)
            {
                OffenderController.Instance.SelectCharacter(i);
                Character c = OffenderController.Instance.characters[i];
                List<int> upgradableSkill = new List<int>();

                while (OffenderController.Instance.skillPoints[i] > 0)
                {
                    upgradableSkill = OffenderController.Instance.GetUpgradableSkill();
                    int index = UnityEngine.Random.Range(0, upgradableSkill.Count);
                    int isLearn = OffenderController.Instance.LearnSkill(c.mySkills[upgradableSkill[index]]);
                }
            }
        }

        public void DefenderSetDice()
        {
            for (int i = 0; i < 6; i++)
            {
                DefenderController.Instance.SelectMonster(i);
                Monster m = DefenderController.Instance.monsters[i];

                int maxTier = GameController.Instance.round;
                List<MonsterSkill> usableSkill = DefenderController.Instance.GetUsableSkill(GameController.Instance.round);

                for (int j = 0; j < 8; j++)
                {

                    // maxTier 1개, 다 포함해서 나머지
                    // 10번 돌때마다 대기 주사위 추가
                    if (j == 0)
                    {
                        List<MonsterSkill> maxSkills = usableSkill.FindAll(skill => skill.tier == maxTier);
                        int n = 0;
                        do
                        {
                            n = UnityEngine.Random.Range(0, maxSkills.Count);

                        } while (maxSkills[n].id == 200110);
                        DefenderController.Instance.SetSkillRoster(maxSkills[n]);
                    }
                    else
                    {
                        int count = 10;
                        while (true)
                        {
                            int n = UnityEngine.Random.Range(1, usableSkill.Count);
                            if (DefenderController.Instance.SetSkillRoster(usableSkill[n]) == 0) break;
                            if (count <= 0)
                            {
                                DefenderController.Instance.SetSkillRoster(usableSkill[0]);
                                break;
                            }
                            count--;
                        }
                    }
                }
                for (int j = 0; j < 6; j++)
                {
                    if (j == 0) DefenderController.Instance.SetDice(true, j);
                    else
                    {
                        int n = Random.Range(0, 8-j);
                        DefenderController.Instance.SetDice(true, n);
                    }
                }
                int attackSkillIndex = Random.Range(0, m.attackSkills.Count);
                MonsterSkill atkSkill = SkillDatabase.Instance.GetMonsterSkill(m.attackSkills[attackSkillIndex]);
                DefenderController.Instance.SetAttackSkill(atkSkill);
            }

        }

        public void OffenderSetDice()
        {
            for (int i = 0; i < 6; i++)
            {
                OffenderController.Instance.SelectCharacter(i);
                Character c = OffenderController.Instance.characters[i];
                List<int> usableSkill = new List<int>();

                for (int j = 0; j < c.mySkills.Count; j++)
                {
                    if (OffenderController.Instance.IsSkillGotten(j)) usableSkill.Add(j);
                }

                int maxTier = OffenderController.Instance.GetGottenSkillsMaxTier();
                // 기본스킬 2개
                // maxTier 2개 maxTier보다 하나 낮은거 2개
                List<int> maxSkills = usableSkill.FindAll(index => c.mySkills[index].tier == maxTier);
                List<int> goodSkills = usableSkill.FindAll(index => c.mySkills[index].tier == (maxTier - 1));

                for (int j = 0; j < 8; j++)
                {
                    if (j < 2) OffenderController.Instance.SetSkillRoster(c.mySkills[usableSkill[0]]);
                    else
                    {
                        int n = 0;
                        if (j < 5)
                        {
                            int m = UnityEngine.Random.Range(0, maxSkills.Count);
                            n = maxSkills[m];
                        }
                        else
                        {
                            int m = UnityEngine.Random.Range(0, goodSkills.Count);
                            n = goodSkills[m];
                        }
                        OffenderController.Instance.SetSkillRoster(c.mySkills[n]);
                    }
                }
                for (int j = 0; j < 6; j++)
                {
                    if (j < 2) OffenderController.Instance.SetDice(true, j);
                    else
                    {
                        if (j < 4) OffenderController.Instance.SetDice(true, j);
                        else if (j >= 4) OffenderController.Instance.SetDice(true, j+1);
                    }
                }
            }

        }
    }
}