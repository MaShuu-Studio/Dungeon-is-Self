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


    protected override void Start()
    {
        base.Start();
        gamePlayUI = GameObject.FindWithTag("UI").GetComponent<GamePlayUIController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCandidate)
        {
            SetColor(Color.gray);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isCandidate)
        {
            gamePlayUI.SelectCandidate(characterName);
            SetColor(Color.white);
        }
    }

    public override void SetImage(UserType type, string name)
    {
        base.SetImage(type, name);
        characterName = name;
    }
}
