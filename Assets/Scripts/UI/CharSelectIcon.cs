using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharSelectIcon : UIIcon
{
    [SerializeField] private int index;
    private GamePlayUIController gamePlayUI;

    protected override void Start()
    {
        base.Start();
        gamePlayUI = GameObject.FindWithTag("UI").GetComponent<GamePlayUIController>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        gamePlayUI.SetSelectedCharacterIndex(index);
    }
}
