using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkill
{
    public string name { get; private set; }
    public int turn { get; private set; }

    public MonsterSkill(string name, int turn)
    {
        this.name = name;
        this.turn = turn;
    }
}
