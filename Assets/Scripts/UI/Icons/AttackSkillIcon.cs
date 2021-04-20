using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackSkillIcon : SkillIcon
{
    [SerializeField] private int index;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        base.OnPointerClick(pointerEventData);
        GamePlayUIController.Instance.SetAttackSkill(index, (MonsterSkill)skill);
    }
}
