using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class CharacterSkill : Skill
    {
        public int damage { get; protected set; }
        public List<int> prior { get; protected set; }

        public CharacterSkill(int id, int tier, string name, bool move, int turn, int damage, string description, List<int> prior, List<System.Tuple<CrowdControl, int>> ccs = null)
        {
            this.id = id;
            this.name = name;
            this.move = move;
            this.turn = turn;
            this.damage = damage;
            this.prior = prior;
            this.tier = tier;
            this.description = description;

            if (ccs != null)
                for (int i = 0; i < ccs.Count; i++)
                {
                    CrowdControl cc = ccs[i].Item1;
                    ccList.Add(cc, ccs[i].Item2);
                }
        }
        public CharacterSkill(int id, int tier, string name, bool move, int turn, int damage, string description, List<System.Tuple<CrowdControl, int>> ccs = null)
        {
            this.id = id;
            this.tier = tier;
            this.name = name;
            this.move = move;
            this.turn = turn;
            this.damage = damage;
            this.prior = new List<int>();
            this.description = description;

            if (ccs != null)
                for (int i = 0; i < ccs.Count; i++)
                {
                    CrowdControl cc = ccs[i].Item1;
                    ccList.Add(cc, ccs[i].Item2);
                }
        }
    }
}
