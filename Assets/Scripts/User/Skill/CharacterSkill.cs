using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : Skill
{
    public int damage { get; protected set;}

    public CharacterSkill(string name, int turn, int damage, string prior)
    {
        this.name = name;
        this.turn = turn;
        this.damage = damage;
        this.prior = prior;
    }
    public CharacterSkill(string name, int turn, int damage)
    {
        this.name = name;
        this.turn = turn;
        this.damage = damage;
        this.prior = "";
    }
}
