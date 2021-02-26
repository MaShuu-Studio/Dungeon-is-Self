using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDB : MonoBehaviour
{
    private List<Skill> skillDB;
    // Start is called before the first frame update
    void Awake()
    {
        InitializeDataBase();
    }

    void Start()
    {
        SkillDB asd = GameObject.FindWithTag("Player").GetComponent<SkillDB>();
        asd.GetSkill("asd");
    }

    private void InitializeDataBase()
    {
        skillDB = new List<Skill>();
        skillDB.Add(new Skill("STAB", 0, 70));
    }

    public Skill GetSkill(string name)
    {
        return skillDB.Find(skill => skill.name == name);
    }
        // Update is called once per frame 
}
