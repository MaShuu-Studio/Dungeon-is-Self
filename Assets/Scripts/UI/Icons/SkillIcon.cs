using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;

public class SkillIcon : UIIcon, IPointerClickHandler
{
    [SerializeField] private Text costText;
    protected Skill skill;
    protected bool isOn = true;

    protected override void Start()
    {
        base.Start();
        pos.x = rect.anchoredPosition.x - 15;
        pos.y = rect.anchoredPosition.y + 15;
    }
    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        //base.OnPointerEnter(pointerEventData);
        //skillDescription.SetDescription(skill.name, "", "DESCRIPTION");
        GamePlayUIController.Instance.ShowDescription(skill);
    }

    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        if (isOn)
        {
            GamePlayUIController.Instance.SetDiceOnce(skill as MonsterSkill);
            base.OnPointerClick(pointerEventData);
        }
    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        if (isOn) base.OnPointerExit(pointerEventData);
    }

    public void SetSkill(MonsterSkill skill, bool isOn = true)
    {
        this.skill = skill; // 복사방법 조정
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.name);
        if (costText != null)
            costText.text = (skill.type == MonsterSkill.SkillType.DICE) ? skill.cost.ToString() : "";

        this.isOn = isOn;
        if (isOn) SetColor(Color.white);
        else SetColor(Color.gray);
    }
    public void SetSkill(CharacterSkill skill, bool isOn = true)
    {
        this.skill = skill; // 복사방법 조정
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.name);
        if (costText != null)
            costText.text = "";

        this.isOn = isOn;
        if (isOn) SetColor(Color.white);
        else SetColor(Color.gray);
    }
}
