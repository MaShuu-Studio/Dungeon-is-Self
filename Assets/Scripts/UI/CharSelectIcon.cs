using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharSelectIcon : UIIcon, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GamePlayUIController gamePlayUI;
    [SerializeField] private int number;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetColor(Color.gray);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gamePlayUI.SetSelectedCharacterNumber(number, rect.anchoredPosition);
        SetColor(Color.white);
    }
}
