using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UsingSkillIcon : SkillIcon
{
    public int index { get; private set; } = -1;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        if (isOn)
        {
            if (GameControl.GameController.Instance.currentProgress == GameControl.GameProgress.ReadyRound)
            {
                GamePlayUIController.Instance.RemoveSkillRoster(this);
            }
            else if (GameControl.GameController.Instance.currentProgress == GameControl.GameProgress.PlayRound)
            {
                GamePlayUIController.Instance.AddOrRemoveDice(this, skill);
            }
            if (isOn) SetColor(Color.white);
        }
    }

    public void SetRosterIndex(int i)
    {
        index = i;
    }
}
