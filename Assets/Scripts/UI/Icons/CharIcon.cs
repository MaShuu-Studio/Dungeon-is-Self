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
    [SerializeField] private GameObject deadIcon;
    [SerializeField] private int index;
    [SerializeField] private bool isEnemy = false;
    public int characterId { get; private set; } = -1;
    private bool isRoster = false;
    private bool isDragging = false;
    private bool isDead = false;
    private RectTransform draggingRect;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;
        if (dragIcon.gameObject.activeSelf == true) dragIcon.gameObject.SetActive(false);

        if (isDead) return;
        if (GameController.Instance.currentProgress != GameProgress.ReadyRound) return;

        dragIcon.gameObject.SetActive(true);
        dragIcon.SetImage(iconImage.sprite, this);
        draggingRect = dragIcon.GetComponent<RectTransform>();

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            draggingRect.anchoredPosition = ResolutionManager.Instance.AdjustPosWithRect(Input.mousePosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;
        if (dragIcon.gameObject.activeSelf == true) dragIcon.gameObject.SetActive(false);

        if (isDead) return;
        if (GameController.Instance.currentProgress != GameProgress.ReadyRound) return;

        dragIcon.gameObject.SetActive(false);
        dragIcon.SetImage(null, null);
        isDragging = false;
    }

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (isEnemy) return;
        if (isDead) return;

        if (isCandidate) base.OnPointerEnter(pointerEventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isEnemy) return;
        if (isDead) return;

        if (isCandidate) base.OnPointerDown(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isEnemy) return;
        if (isDead) return;

        if (isCandidate)
        {
            base.OnPointerClick(eventData);
            GamePlayUIController.Instance.SelectCandidate(characterId);
        }
        else if (GameController.Instance.currentProgress == GameProgress.ReadyRound)
        {
            SelectUnit(isRoster);
            GamePlayUIController.Instance.SelectCharacter(index, characterId);
        }
    }
    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        if (isEnemy) return;

        if (isCandidate) SelectUnit(isRoster);
    }

    public void Reset()
    {
        characterId = -1;
    }

    public override void SetImage(UserType type, int id)
    {
        base.SetImage(type, id);
        characterId = id;
    }

    public void SelectUnit(bool isSelected)
    {
        isRoster = isSelected;
        if (isSelected) SetColor(Color.gray);
        else SetColor(Color.white);
    }

    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead;
        if (isDead) SetColor(Color.gray);
        else SetColor(Color.white);
        deadIcon.SetActive(this.isDead);
    }
}
