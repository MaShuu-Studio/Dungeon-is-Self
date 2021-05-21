using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RolledDiceIcon : MonoBehaviour
{
    private Image frameImage;
    [SerializeField] private Image iconImage;
    public int id { get; private set; }

    private void Awake()
    {
        frameImage = gameObject.GetComponent<Image>();
    }

    public void SetImage(int id)
    {
        this.id = id;
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + id);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        iconImage.rectTransform.localScale = new Vector3(1, 1, 1);
    }
}
