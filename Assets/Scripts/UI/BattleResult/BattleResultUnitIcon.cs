using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Data;

public class BattleResultUnitIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    public void SetImage(UserType type, int id)
    {
        string path = (type == UserType.Defender) ? MonsterDatabase.facePath : CharacterDatabase.facePath;
        image.sprite = Resources.Load<Sprite>(path + id.ToString());
    }
}
