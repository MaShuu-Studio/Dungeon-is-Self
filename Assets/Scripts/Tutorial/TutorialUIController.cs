using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControl;

public class TutorialUIController : MonoBehaviour
{
    public void StartTutorial(int type)
    {
        GameController.Instance.SetTutorial(true);
        GameController.Instance.StartGame("tutorial", UserType.Offender);
    }
}
