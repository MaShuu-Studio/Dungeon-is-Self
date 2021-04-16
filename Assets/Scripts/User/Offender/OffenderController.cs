using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;

namespace Offender
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
    public enum Role {FIGHTER, MARKSMAN, MAGE}
    public List<Role> bench = new List<Role>();
    public List<Role> roster = new List<Role>();

    public Role fighter = Role.FIGHTER;
    public Role marksman = Role.MARKSMAN;
    public Role mage = Role.MAGE;

    private Fighter f;
    private Marksman m;
    private Mage ma;
    
    public void SetBench(Role role)
    {
        if(bench.Count >= 3) return;
        else bench.Add(role);
    }

        public CharacterSkill OneDiceThrow(int n)
        {
            int i = Random.Range(0, 6);
            return character[n].dice[i];
        }

        public void AllDiceThrow(int a, int b, int c)
        {
            OneDiceThrow(a);
            OneDiceThrow(b);
            OneDiceThrow(c);
        }

        public void SetSkillUpdate()
        {

        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 50, 50), "Fighter"))
            {
                SetBench(fighter);
            }

            if (GUI.Button(new Rect(10, 80, 50, 50), "Marksman"))
            {
                SetBench(marksman);
            }

            if (GUI.Button(new Rect(80, 80, 50, 50), "Mage"))
            {
                SetBench(mage);
            }

            /*if (GUI.Button(new Rect(150, 150, 50, 50), "SetDice0"))
            {
                SetDice(0, 100);
                
            }    
            if(character.Count > 0)
            {
                if(character[0].dice.Count > 0) {
                    GUI.Box(new Rect(210, 210, 100, 50), character[0].dice[0].name);
                }
            } */
            //GUI.Box(new Rect(210, 210, 100, 50), "sex");

        }
        */
    }
}
