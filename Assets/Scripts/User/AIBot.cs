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
                List<string> offenderBot = new List<string>();
                CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref offenderBot);
                
                for(int i = 0; i < 6; i++)
                {
                    int a = UnityEngine.Random.Range(0, offenderBot.Count);
                    OffenderController.Instance.SetCharacterCandidate(i, offenderBot[a]);
                }
            }
            else
            {
                List<string> defenderBot = new List<string>();
                MonsterDatabase.Instance.GetAllMonsterCandidatesList(ref defenderBot);
                for(int i = 0; i < 6; i++)
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
                GameController.Instance.SelectUnit(UserType.Offender, OffenderController.Instance.roster);
            }
            else
            {
                int unit = UnityEngine.Random.Range(0, 6);
                DefenderController.Instance.SelectMonster(unit);
                DefenderController.Instance.SetRoster();
            }
        }

        /*public int LearnSkill()
        {
            if (GameController.Instance.userType == UserType.Defender)
            {
                for (int i = 0; i < 6; i++)
                {
                    Character c = OffenderController.Instance.characters[i];
                    int index = c.mySkills.
                }
                
            }
        }*/
    }
}