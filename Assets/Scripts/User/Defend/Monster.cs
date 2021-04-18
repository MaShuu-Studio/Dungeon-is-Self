using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public enum Weakness { NORMAL }

    public string name { get; private set; }
    public int hp { get; private set; }
    public Weakness weak { get; private set; }
    public List<string> skills { get; private set; }
    public string prior { get; private set; }
    public int tier { get; private set; }

    public Monster(int tier, string name, int hp, Weakness weak, List<string> skills, string prior = "")
    {
        this.name = name;
        this.hp = hp;
        this.weak = weak;
        this.skills = new List<string>(skills);
        this.prior = prior;
        this.tier = tier;
    }
}
