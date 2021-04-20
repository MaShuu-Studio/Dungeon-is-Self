using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] protected UIDescription description;
    [SerializeField] protected Vector3 pos;

    [SerializeField] protected Image iconImage;
    protected Image frameImage;
    protected RectTransform rect;

    protected virtual void Awake()
    {
        frameImage = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
    protected virtual void Start()
    {
        if (description != null) description.gameObject.SetActive(false);
    }

    public virtual void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (description != null) description.ShowDecription(true, pos);
    }

    public virtual void OnPointerExit(PointerEventData pointerEventData)
    {
        if (description != null) description.ShowDecription(false, pos);
        SetColor(Color.white);
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        SetColor(Color.gray);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        SetColor(Color.white);
    }

    public virtual void SetImage(GameControl.UserType type, string name)
    {
        string path = (type == GameControl.UserType.Defender) ? MonsterDatabase.facePath : "";

        Sprite sprite = Resources.Load<Sprite>(path + name);

        iconImage.sprite = sprite;
    }

    protected void SetColor(Color color)
    {
        if (iconImage == null || frameImage == null) return;
        iconImage.color = color;
        frameImage.color = color;
    }
}
