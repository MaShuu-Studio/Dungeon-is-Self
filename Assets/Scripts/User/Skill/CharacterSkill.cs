﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class CharacterSkill : Skill
    {
        public int damage { get; protected set; }
        public List<int> prior { get; protected set; }

        public CharacterSkill(int id, int tier, string name, int turn, int damage, List<int> prior)
        {
            this.id = id;
            this.name = name;
            this.turn = turn;
            this.damage = damage;
            this.prior = prior;
            this.tier = tier;
        }
        public CharacterSkill(int id, int tier, string name, int turn, int damage)
        {
            this.id = id;
            this.tier = tier;
            this.name = name;
            this.turn = turn;
            this.damage = damage;
            this.prior = new List<int>();
            
        }
    }
}
