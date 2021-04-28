﻿using System.Collections;
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
                List<string> offenderBot = new List<string>();
                CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref offenderBot);

                for (int i = 0; i < 6; i++)
                {
                    int a = UnityEngine.Random.Range(0, offenderBot.Count);
                    OffenderController.Instance.SetCharacterCandidate(i, offenderBot[a]);
                }
            }
            else
            {
                List<string> defenderBot = new List<string>();
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
                List<int> roster = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    while (true)
                    {
                        int index = UnityEngine.Random.Range(0, 6);

                        int check = roster.FindIndex(a => a == index);
                        if (check == -1)
                        {
                            OffenderController.Instance.SelectCharacter(index);
                            OffenderController.Instance.SelectRoster(i);
                            roster.Add(index);
                            break;
                        }
                    }
                }
                GameController.Instance.SelectUnit(UserType.Offender, OffenderController.Instance.roster);
            }
            else
            {
                int unit = UnityEngine.Random.Range(0, 6);
                DefenderController.Instance.SelectMonster(unit);
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
                    OffenderController.Instance.LearnSkill(c.mySkills[upgradableSkill[index]]);
                }
            }
        }

        public void SetDice()
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
    }
}