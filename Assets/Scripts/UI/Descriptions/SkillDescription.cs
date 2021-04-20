using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillDescription : UIDescription
{
    [SerializeField] private Image skillImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text descriptionText;

    protected override void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        skillImage.color = Color.black;
        nameText.text = "";
        typeText.text = "";
        descriptionText.text = "";
    }
    public void SetDescription(string name, string type, string description)
    {
        skillImage.color = Color.white;
        skillImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + name);
        nameText.text = name;
        typeText.text = type;
        descriptionText.text = description;
    }
}
