using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public enum MonsterType { ATTACKER, GUARD, SPECIAL }

    public string name { get; private set; }
    public MonsterType type { get; private set; }
    public int hp { get; private set; }
    public MonsterSkill skill { get; private set; }

    public Monster(string name, int hp, MonsterType type, MonsterSkill skill)
    {
        this.name = name;
        this.hp = hp;
        this.type = type;
        this.skill = skill;
    }
}
