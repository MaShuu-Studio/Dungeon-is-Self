using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UsingSkillIcon : SkillIcon
{
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        if (GameControl.GameController.Instance.currentProgress == GameControl.GameProgress.ReadyRound)
            GamePlayUIController.Instance.RemoveSkillRoster(this);
        else if (GameControl.GameController.Instance.currentProgress == GameControl.GameProgress.PlayRound)
        {
            GamePlayUIController.Instance.AddOrRemoveDice(this);
        }
        SetColor(Color.white);
    }
}
