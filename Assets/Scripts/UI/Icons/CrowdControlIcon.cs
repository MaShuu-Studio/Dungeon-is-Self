using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrowdControlIcon : MonoBehaviour
{
    private Image frameImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text turnText;
    public int id { get; private set; }

    private void Awake()
    {
        frameImage = gameObject.GetComponent<Image>();
    }

    public void SetImage(int id)
    {
        this.id = id;
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Crowd Control/" + id);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        iconImage.rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void SetTurn(int turn, int stack = 0)
    {
        if (turn == -1)
        {
            turnText.text = stack.ToString();
            frameImage.color = Color.gray;
            iconImage.color = Color.gray;
        }
        else
        {
            turnText.text = turn.ToString();
            frameImage.color = Color.white;
            iconImage.color = Color.white;
        }

    }
}
