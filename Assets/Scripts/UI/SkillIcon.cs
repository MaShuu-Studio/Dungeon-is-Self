using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIcon : UIIcon, IPointerClickHandler
{
    [SerializeField] private Text costText;
    protected Skill skill;

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

    public virtual void OnPointerClick(PointerEventData pointerEventData)
    {
        GamePlayUIController.Instance.SetDiceOnce(skill as MonsterSkill);
    }

    public void SetSkill(MonsterSkill skill)
    {
        this.skill = skill; // 복사방법 조정
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.name);
        if (costText != null)
            costText.text = (skill.type == MonsterSkill.SkillType.DICE) ? skill.cost.ToString() : "";
    }
    public void SetSkill(CharacterSkill skill)
    {
        this.skill = skill; // 복사방법 조정
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.name);
        if (costText != null)
            costText.text = "";
    }
}
