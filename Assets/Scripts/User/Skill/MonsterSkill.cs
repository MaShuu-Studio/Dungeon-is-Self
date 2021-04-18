using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkill : Skill
{
    public enum SkillType { ATTACK = 1, DICE }

    public SkillType type { get; private set; }
    public int cost { get; private set; }

    public MonsterSkill(int id, string name, int turn)
    {
        // ID 구조 2abcc
        // 2는 몬스터스킬임을 표기
        // a는 SkillType을 표기. 1: ATTACK, 2: DICE
        // bbb는 스킬의 고유 넘버를 표기
        // 결과적으로 abbb가 전체적으로 스킬 전체 고유 넘버가 되는 것.
        this.id = id;

        int tmp = id;
        tmp /= 100;
        tier = tmp % 10;
        tmp /= 10;

        this.type = (SkillType)(tmp % 10);
        this.tier = tier;
        this.name = name;
        this.turn = turn;
    }
}
