using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharSelectIcon : UIIcon, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private int number;
    private GamePlayUIController gamePlayUI;

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
        gamePlayUI.SetSelectedCharacterNumber(number);
    }
}
