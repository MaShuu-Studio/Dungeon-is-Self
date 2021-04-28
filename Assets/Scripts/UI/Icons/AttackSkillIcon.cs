using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Data;

public class AttackSkillIcon : SkillIcon
{
    [SerializeField] private int index;
    protected override void Start()
    {
        base.Start();
        isSkill = false;
    }
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        base.OnPointerClick(pointerEventData);
        GamePlayUIController.Instance.SetAttackSkill(index, (MonsterSkill)skill);
    }
}
