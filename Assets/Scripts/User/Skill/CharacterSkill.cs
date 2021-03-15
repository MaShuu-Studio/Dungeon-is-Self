using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill
{
    public string name {get; private set;}
    public int readyTurn;
    public int damage;
    public string prior;

    public CharacterSkill(string name, int readyTurn, int damage, string prior)
    {
        this.name = name;
        this.readyTurn = readyTurn;
        this.damage = damage;
        this.prior = prior;
    }
    public CharacterSkill(string name, int readyTurn, int damage)
    {
        this.name = name;
        this.readyTurn = readyTurn;
        this.damage = damage;
        this.prior = "";
    }
}
