using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameControl;

public class SkillIcon : UIIcon
{
    SkillDescription skillDescription;
    private Skill skill;

    protected override void Start()
    {
        base.Start();
        pos.x = rect.anchoredPosition.x - 15;
        pos.y = rect.anchoredPosition.y + 15;
        skillDescription = description as SkillDescription;
        SetSkill(UserType.Defender, "SKILL1");
    }
    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        base.OnPointerEnter(pointerEventData);
        skillDescription.SetDescription(skill.name, "", "DESCRIPTION");
    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
    }

    public void SetSkill(UserType type, string skillName)
    {
        switch (type)
        {
            case UserType.Defender:
                skill = SkillDatabase.Instance.GetMonsterSkill(skillName);
                break;
            case UserType.Offender:
                skill = SkillDatabase.Instance.GetCharacterSkill(skillName);
                break;
        }

        iconImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.name);
    }
}
