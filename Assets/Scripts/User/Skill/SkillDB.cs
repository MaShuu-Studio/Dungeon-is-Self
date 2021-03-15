using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDB : MonoBehaviour
{
    private List<CharacterSkill> charSkillDB;
    private List<MonsterSkill> monSkillDB;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeCharacterSkill();
        InitializeMonsterSkill();
    }

    void Start()
    {
        
    }

    private void InitializeCharacterSkill()
    {
        charSkillDB = new List<CharacterSkill>();

        charSkillDB.Add(new CharacterSkill("STAB", 0, 70));
    }

    private void InitializeMonsterSkill()
    {
        monSkillDB = new List<MonsterSkill>();

        monSkillDB.Add(new MonsterSkill("SKILL1", 5));
    }

    public CharacterSkill GetCharacterSkill(string name)
    {
        return charSkillDB.Find(skill => skill.name == name);
    }
    
    public MonsterSkill GetMonsterSkill(string name)
    {
        return monSkillDB.Find(skill => skill.name == name);
    }
}
