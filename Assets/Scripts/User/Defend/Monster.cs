using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public enum Weakness { NORMAL }

    public string name { get; private set; }
    public Weakness weak { get; private set; }
    public int hp { get; private set; }
    public MonsterSkill skill { get; private set; }
    public string prior { get; private set; }

    public Monster(string name, int hp, Weakness weak, MonsterSkill skill, string prior = "")
    {
        this.name = name;
        this.hp = hp;
        this.weak = weak;
        this.skill = skill;
        this.prior = prior;
    }
}
