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

        private List<Character> character = new List<Character>();
        public List<Role> bench = new List<Role>();
        public List<Role> roster = new List<Role>();

        public Role fighter = Role.FIGHTER;
        public Role marksman = Role.MARKSMAN;
        public Role mage = Role.MAGE;

        public void SetBench(Role role)
        {
            if (bench.Count >= 3) return;
            else bench.Add(role);
        }
        public void SetRoster(Role role)
        {
            if (roster.Count >= 3) return;
            else { roster.Add(role); character.Add(new Character(role)); }
        }
        public void ResetDice(int n)
        {
            character[n].dice.RemoveRange(0, 6);
        }

        public void SetDice(int n, int id)
        {
            character[n].dice.Add(SkillDatabase.Instance.GetCharacterSkill(id));
        }

        public CharacterSkill OneDiceThrow(int n)
        {
            int i = Random.Range(0, 6);
            return character[n].dice[i];
        }

        // Roster ¡÷ªÁ¿ß
        public void AllDiceThrow(int a, int b, int c)
        {
            OneDiceThrow(a);
            OneDiceThrow(b);
            OneDiceThrow(c);
        }

        public void SetSkillUpdate()
        {

        }
    }
}
