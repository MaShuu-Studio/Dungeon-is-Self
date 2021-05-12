using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameControl;

public class RosterIcon : UIIcon, IDropHandler
{
    [SerializeField] private Text numberText;
    [SerializeField] private DragAndDropIcon dragIcon;
    public int index { get; private set; }
    public int characterId { get; private set; } = -1;
    public void OnDrop(PointerEventData eventData)
    {
        if (GameController.Instance.currentProgress != GameProgress.ReadyRound) return;
        if (dragIcon.charIcon == null) return;
        GamePlayUIController.Instance.ChangeRoster(this, dragIcon.charIcon);
        GamePlayUIController.Instance.SelectCharacter(index, characterId);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (GameController.Instance.currentProgress == GameProgress.ReadyRound)
        {
            base.OnPointerClick(eventData);
            GamePlayUIController.Instance.SelectCharacter(index, characterId);
        }
    }

    public override void SetImage(UserType type, int id)
    {
        base.SetImage(type, id);
        characterId = id;
    }

    public void SetRosterNumber(int index)
    {
        this.index = index;
        numberText.text = index.ToString();
    }
}
