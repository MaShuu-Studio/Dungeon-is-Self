using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceIcon : SkillIcon
{
    [SerializeField] private int index;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        GamePlayUIController.Instance.SelectDice(index);
    }
}
