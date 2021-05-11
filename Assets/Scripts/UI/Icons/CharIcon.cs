using GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharIcon : UIIcon, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private DragAndDropIcon dragIcon;
    [SerializeField] private bool isCandidate;
    private int characterId = -1;
    private bool isDragging = false;
    private RectTransform draggingRect;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isCandidate) return;
        if (dragIcon == null) return;
        if (characterId == -1) return;

        dragIcon.gameObject.SetActive(true);
        dragIcon.SetImage(iconImage.sprite, this);
        draggingRect = dragIcon.GetComponent<RectTransform>();

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            draggingRect.anchoredPosition = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isCandidate) return;
        if (dragIcon == null) return;
        if (characterId == -1) return;

        dragIcon.gameObject.SetActive(false);
        dragIcon.SetImage(null, null);
        isDragging = false;
    }

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
            GamePlayUIController.Instance.SelectCandidate(characterId);
        }
        else if (GameController.Instance.currentProgress == GameProgress.ReadyRound)
        {
            base.OnPointerClick(eventData);
            GamePlayUIController.Instance.SelectCharacter(this, characterId, rect.anchoredPosition);
            GamePlayUIController.Instance.SetSkillTree();
        }
    }
    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        if (isCandidate) base.OnPointerExit(pointerEventData);
    }

    public override void SetImage(UserType type, int id)
    {
        base.SetImage(type, id);
        characterId = id;
    }

    public void SetIsDead(bool isDead)
    {
        if (isDead) SetColor(Color.gray);
        else SetColor(Color.white);
    }
}
