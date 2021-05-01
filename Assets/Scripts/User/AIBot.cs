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

                for (int j = 0; j < 6; j++)
                {
                    List<MonsterSkill> usableSkill = new List<MonsterSkill>();
                    usableSkill = DefenderController.Instance.GetUsableSkill(GameController.Instance.round);
                    
                    while (true)
                    {
                        int n = UnityEngine.Random.Range(0, usableSkill.Count);
                        if (DefenderController.Instance.SetDice(j, usableSkill[n]) == 0) break;
                    }
                }
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

                for (int j = 0; j < 6; j++)
                {
                    if (j < 2) OffenderController.Instance.SetDice(j, c.mySkills[usableSkill[0]]);
                    else
                    {
                        //Debug.Log(usableSkill[n]);
                        while (true)
                        {
                            int n = UnityEngine.Random.Range(0, usableSkill.Count);
                            if (OffenderController.Instance.SetDice(j, c.mySkills[usableSkill[n]]) == 0) break;
                        }
                    }
                    //Debug.Log(c._role);
                }
            }
            
        }
    }
}