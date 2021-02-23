using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillDataBase : MonoBehaviour
{
    List<MonsterSkill> skillDB;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeDataBase();
    }

    private void InitializeDataBase()
    {
        skillDB = new List<MonsterSkill>();

        skillDB.Add(new MonsterSkill("SKILL1", 5));
    }

    public MonsterSkill GetSkill(string name)
    {
        return skillDB.Find(skill => skill.name == name);
    }


}
