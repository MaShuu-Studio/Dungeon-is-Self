using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffenderController : MonoBehaviour
{
    private GameController gameController;
    [Header("UI")]
    [SerializeField] private List<Toggle> characterToggles;
    [SerializeField] private List<GameObject> characterSkillTrees;
    [Space]

    private SkillDB skillDB;
    
    public enum Role {FIGHTER, MARKSMAN, MAGE}
    public List<Role> bench = new List<Role>();
    public List<Role> roster = new List<Role>();

    public Role fighter = Role.FIGHTER;
    public Role marksman = Role.MARKSMAN;
    public Role mage = Role.MAGE;

    private Fighter f;
    private Marksman m;
    private Mage ma;
    
    void Awake()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        skillDB = GameObject.FindWithTag("SkillDB").GetComponent<SkillDB>();        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 6; i++) characterSkillTrees[i].SetActive(characterToggles[i].isOn);
        
    }

    public void SetBench(Role role)
    {
        if(bench.Count >= 3) return;
        else bench.Add(role);
    }

    public void SetRoster(Role role)
    {
        if(roster.Count >= 3) return;
        else roster.Add(role);
    }

    public void DiceThrow()
    {
        f.DiceThrow();
        m.DiceThrow();
        ma.DiceThrow();
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
    }
}
