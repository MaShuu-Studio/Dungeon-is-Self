using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrowdControlIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text turnText;
    public int id { get; private set; }

    public void SetImage(int id)
    {
        this.id = id;
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Crowd Control/" + id);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        iconImage.rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void SetTurn(int turn)
    {
        turnText.text = turn.ToString();
    }
}
