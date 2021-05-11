using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    public CharIcon charIcon { get; private set;}= null;

    private void Start()
    {
        gameObject.SetActive(false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void SetImage(Sprite sprite, CharIcon icon)
    {
        iconImage.sprite = sprite;
        charIcon = icon;
    }
}
