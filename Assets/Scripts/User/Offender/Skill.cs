using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public string name {get; private set;}
    public int readyTurn;
    public int damage;
    public string prior;

    public Skill(string name, int readyTurn, int damage, string prior)
    {
        this.name = name;
        this.readyTurn = readyTurn;
        this.damage = damage;
        this.prior = prior;
    }
    public Skill(string name, int readyTurn, int damage)
    {
        this.name = name;
        this.readyTurn = readyTurn;
        this.damage = damage;
        this.prior = "";
    }
}
