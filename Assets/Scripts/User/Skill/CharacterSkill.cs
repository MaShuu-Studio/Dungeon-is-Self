using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : Skill
{
    public int damage { get; protected set;}

    public CharacterSkill(int number, string name, int turn, int damage, List<int> prior)
    {
        this.number = number;
        this.name = name;
        this.turn = turn;
        this.damage = damage;
        this.prior = prior;
    }
    public CharacterSkill(int number, string name, int turn, int damage)
    {
        this.number = number;
        this.name = name;
        this.turn = turn;
        this.damage = damage;
        this.prior = null;
    }
}
