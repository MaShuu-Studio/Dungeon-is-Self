using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class MonsterSkill : Skill
    {
        public enum SkillType { Dice = 1, AttackAll, AttackOne, AttackOneStun, }

        public SkillType type { get; private set; }
        public int cost { get; private set; }

        public MonsterSkill(int id, string name, int turn, int cost, string description, List<System.Tuple<CrowdControl, int>> ccs = null)
        {
            // ID 구조 2aabcd
            // 2는 몬스터스킬임을 표기
            // aa는 몬스터 종류
            // b는 SkillType을 표기. 1: Dice, 2 > Attack -> 2: AttackAll, 3: AttackOne 4: AttackOne And Stun
            // c는 티어
            // d는 고유 넘버
            this.id = id;

            int tmp = id;
            tmp /= 10;
            tier = tmp % 10;
            tmp /= 10;

            this.type = (SkillType)(tmp % 10);
            this.name = name;
            this.turn = turn;
            this.cost = cost;

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