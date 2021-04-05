using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDatabase : MonoBehaviour
{
    private static List<CharacterSkill> charSkillDB;
    private static List<MonsterSkill> monSkillDB;
    // Start is called before the first frame update
    private static SkillDatabase instance;
    public static SkillDatabase Instance
    {
        get
        {
            var obj = FindObjectOfType<SkillDatabase>();
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
        InitializeCharacterSkill();
        InitializeMonsterSkill();
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

    public static CharacterSkill GetCharacterSkill(string name)
    {
        return charSkillDB.Find(skill => skill.name == name);
    }
    
    public static MonsterSkill GetMonsterSkill(string name)
    {
        return monSkillDB.Find(skill => skill.name == name);
    }
}
