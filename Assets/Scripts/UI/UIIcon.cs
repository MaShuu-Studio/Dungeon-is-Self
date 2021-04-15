using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIDescription description;
    [SerializeField] private Vector3 pos;

    [SerializeField] protected Image faceImage;
    protected Image frameImage;
    protected RectTransform rect;

    protected void Start()
    {
        frameImage = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (description != null) description.ShowDecription(true, pos);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (description != null) description.ShowDecription(false, pos);
        SetColor(Color.white);
    }

    public void SetImage(GameControl.UserType type, string name)
    {
        string path = (type == GameControl.UserType.Defender) ? MonsterDatabase.facePath : "";

        Sprite sprite = Resources.Load<Sprite>(path + name);

        faceImage.sprite = sprite;
    }

    protected void SetColor(Color color)
    {
        if (faceImage == null || frameImage == null) return;

        faceImage.color = color;
        frameImage.color = color;
    }
}
