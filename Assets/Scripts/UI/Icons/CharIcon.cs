using GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharIcon : UIIcon, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private bool isCandidate;
    private string characterName;
    private UserType type;


    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (isCandidate) base.OnPointerEnter(pointerEventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isCandidate) base.OnPointerDown(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isCandidate)
        {
            base.OnPointerClick(eventData);
            GamePlayUIController.Instance.SelectCandidate(characterName);
        }
    }
    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        if (isCandidate) base.OnPointerExit(pointerEventData);
    }

    public override void SetImage(UserType type, string name)
    {
        base.SetImage(type, name);
        characterName = name;
        this.type = type;
    }

    public void SetIsDead(bool isDead)
    {
        if (isDead) SetColor(Color.gray);
        else SetColor(Color.white);
    }
}
