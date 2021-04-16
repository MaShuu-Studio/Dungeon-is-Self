﻿using System.Collections;
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
#region Fighter Skill
        //fighter skill
        charSkillDB.Add(new CharacterSkill(100, "FAttack", 0, 13));
        charSkillDB.Add(new CharacterSkill(101, "SteelShield", 1, 0));
        charSkillDB.Add(new CharacterSkill(102, "Stab", 1, 70));
        charSkillDB.Add(new CharacterSkill(103, "Mockery", 1, 0));
        charSkillDB.Add(new CharacterSkill(104, "ShieldAttack", 1, 50, (new List<int>{101})));
        charSkillDB.Add(new CharacterSkill(105, "Harpoon", 1, 100, (new List<int>{102})));
        charSkillDB.Add(new CharacterSkill(106, "CounterAttack", 2, 40, (new List<int>{101, 102})));
        charSkillDB.Add(new CharacterSkill(107, "BrutalCrying", 0, 0 , (new List<int>{103})));
        charSkillDB.Add(new CharacterSkill(108, "Brave", 2, 0, (new List<int>{104})));
        charSkillDB.Add(new CharacterSkill(109, "Roar", 1, 0, (new List<int>{107})));
        charSkillDB.Add(new CharacterSkill(110, "Deadlock", 3, 40, (new List<int>{104, 106})));
        charSkillDB.Add(new CharacterSkill(111, "SuperArmor", 3, 0, (new List<int>{108})));
#endregion
#region Marksman Skill
        //marksman skill
        charSkillDB.Add(new CharacterSkill(200, "MarkAttack", 0, 10));
        charSkillDB.Add(new CharacterSkill(201, "NetArrow", 0, 40));
        charSkillDB.Add(new CharacterSkill(202, "DoubleShot", 1, 90));
        charSkillDB.Add(new CharacterSkill(203, "SawedArrow", 1, 60));
        charSkillDB.Add(new CharacterSkill(204, "PowerShot", 1, 100, (new List<int>{201, 202})));
        charSkillDB.Add(new CharacterSkill(205, "PoisonArrow", 1, 70, (new List<int>{203})));
        charSkillDB.Add(new CharacterSkill(206, "RainArrow", 2, 120, (new List<int>{203, 204})));
        charSkillDB.Add(new CharacterSkill(207, "BowMaster", 2, 150, (new List<int>{204})));
        charSkillDB.Add(new CharacterSkill(208, "Scarecrow", 1, 0, (new List<int>{205})));
        charSkillDB.Add(new CharacterSkill(209, "ParalyticArrow", 2, 100, (new List<int>{204, 205})));
#endregion
#region Mage Skill
        //mage skill
        charSkillDB.Add(new CharacterSkill(300, "MageAttack", 0, 7));
        charSkillDB.Add(new CharacterSkill(301, "ElectricShock", 2, 140));
        charSkillDB.Add(new CharacterSkill(302, "MagneticPrison", 1, 80));
        charSkillDB.Add(new CharacterSkill(303, "Lightening", 1, 100));
        charSkillDB.Add(new CharacterSkill(304, "Purify", 2, 0));
        charSkillDB.Add(new CharacterSkill(305, "LighteningClouds", 2, 180, (new List<int>{301, 303})));
        charSkillDB.Add(new CharacterSkill(306, "FrozenPrison", 1, 100, (new List<int>{302, 303})));
        charSkillDB.Add(new CharacterSkill(307, "Blaze", 1, 140, (new List<int>{301, 303})));
        charSkillDB.Add(new CharacterSkill(308, "Imprison", 1, 20, (new List<int>{304})));
        charSkillDB.Add(new CharacterSkill(309, "ThunderDragon", 3, 250, (new List<int>{305, 307})));
        charSkillDB.Add(new CharacterSkill(310, "FrozenGraves", 2, 180, (new List<int>{306})));
        charSkillDB.Add(new CharacterSkill(311, "FrostDecoy", 2, 0, (new List<int>{306})));
#endregion
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
    public static CharacterSkill GetCharacterSkill(int id)
    {
        return charSkillDB.Find(skill => skill.id == id);
    }
    
    public MonsterSkill GetMonsterSkill(string name)
    {
        return monSkillDB.Find(skill => skill.name == name);
    }
}
