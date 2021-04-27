using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceIcon : SkillIcon
{
    [SerializeField] private int index;

    protected override void Start()
    {
        base.Start();
        isSkill = false;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        GamePlayUIController.Instance.SelectDice(index);
    }
}
