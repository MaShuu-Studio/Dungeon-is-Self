using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RosterIcon : UIIcon
{
    [SerializeField] private int index;
    [SerializeField] private Text numberText;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        base.OnPointerClick(pointerEventData);
        GamePlayUIController.Instance.SetOffenderRoster(index);
    }

    public override void SetImage(GameControl.UserType type, string name)
    {
        base.SetImage(type, name);
    }

    public void SetNumber(int n)
    {
        numberText.text = n.ToString();
    }
}
