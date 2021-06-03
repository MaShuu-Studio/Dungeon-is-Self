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
    protected bool isSkill = true;

    protected override void Start()
    {
        base.Start();
        pos.x = rect.anchoredPosition.x - 15;
        pos.y = rect.anchoredPosition.y + 15;
        isSkill = true;
    }


    /*
    #region for PC
    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        //base.OnPointerEnter(pointerEventData);
        //skillDescription.SetDescription(skill.name, "", "DESCRIPTION");
        GamePlayUIController.Instance.ShowDescription(skill);
    }
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        if (isSkill)
        {
            GamePlayUIController.Instance.AddSkillRoster(skill, isOn);
        }
        if (isOn) base.OnPointerClick(pointerEventData);
    }
    #endregion
    */
    
    #region for Mobile
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        if (isSkill)
        {
            if (GamePlayUIController.Instance.DescriptionIsSelected(skill.id))
                GamePlayUIController.Instance.AddSkillRoster(skill, isOn);
            else GamePlayUIController.Instance.ShowDescription(skill);
            
        }
        if (isOn) base.OnPointerClick(pointerEventData);
    }
    #endregion
    

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        if (isOn) base.OnPointerExit(pointerEventData);
    }

    public void SetSkill(GameControl.UserType type, Skill skill, bool isOn = true)
    {
        this.skill = skill; // 복사방법 조정
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.id.ToString());

        if (costText != null)
        {
            if (type == GameControl.UserType.Defender)
            {
                MonsterSkill monSkill = skill as MonsterSkill;
                costText.text = (monSkill.type == MonsterSkill.SkillType.Dice) ? monSkill.cost.ToString() : "";
            }
            else costText.text = "";
        }

        SetOnOff(isOn);
    }

    public void SetOnOff(bool isOn)
    {
        this.isOn = isOn;
        if (isOn) SetColor(Color.white);
        else SetColor(Color.gray);
    }
}
