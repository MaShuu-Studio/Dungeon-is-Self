using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceIcon : SkillIcon, IPointerClickHandler
{
    [SerializeField] private int index;
    public override void OnPointerClick(PointerEventData eventData)
    {
        GamePlayUIController.Instance.SelectDice(index);
    }
}
