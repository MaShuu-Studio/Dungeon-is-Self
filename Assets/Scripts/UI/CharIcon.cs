using GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharIcon : UIIcon, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private bool isCandidate;
    private GamePlayUIController gamePlayUI;
    private string characterName;
    private UserType type;


    protected override void Start()
    {
        base.Start();
        gamePlayUI = GameObject.FindWithTag("UI").GetComponent<GamePlayUIController>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isCandidate)
        {
            base.OnPointerDown(eventData);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isCandidate)
        {
            base.OnPointerClick(eventData);
            gamePlayUI.SelectCandidate(characterName);
        }
    }
    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        base.OnPointerExit(pointerEventData);
        SetColor(Color.white);
    }

    public override void SetImage(UserType type, string name)
    {
        base.SetImage(type, name);
        characterName = name;
        this.type = type;
    }
}
