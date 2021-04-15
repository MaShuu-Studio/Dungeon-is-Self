using GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CandidateCharIcon : UIIcon, IPointerDownHandler, IPointerClickHandler
{
    private GamePlayUIController gamePlayUI;

    private string monsterName;

    protected override void Start()
    {
        base.Start();
        gamePlayUI = GameObject.FindWithTag("UI").GetComponent<GamePlayUIController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetColor(Color.gray);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gamePlayUI.SelectCandidate(monsterName);
        SetColor(Color.white);
    }

    public override void SetImage(UserType type, string name)
    {
        base.SetImage(type, name);
        monsterName = name;
    }
}
