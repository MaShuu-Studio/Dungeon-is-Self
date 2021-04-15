using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIDescription description;
    [SerializeField] private Vector3 pos;

    [SerializeField] private Image faceImage;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (description != null) description.ShowDecription(true, pos);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (description != null) description.ShowDecription(false, pos);
    }

    public void SetImage(GameControl.UserType type, string name)
    {
        string path = (type == GameControl.UserType.Defender) ? MonsterDatabase.facePath : "";

        Sprite sprite = Resources.Load<Sprite>(path + name);

        faceImage.sprite = sprite;
    }
}
