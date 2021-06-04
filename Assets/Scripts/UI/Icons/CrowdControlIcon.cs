using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;

public class CrowdControlIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerClickHandler
{
    private Image frameImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text turnText;
    [SerializeField] private GameObject descriptionObject;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text descriptionDmgText;
    public int id { get; private set; }
    private CrowdControl cc;
    private int stack = 0;

    private void Awake()
    {
        frameImage = gameObject.GetComponent<Image>();
        descriptionObject.SetActive(false);
    }

    public void SetImage(int id)
    {
        this.id = id;
        iconImage.sprite = Resources.Load<Sprite>("Sprites/Crowd Control/" + id);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        iconImage.rectTransform.localScale = new Vector3(1, 1, 1);

        cc = SkillDatabase.Instance.GetCrowdControl(id);
    }

    public void SetTurn(int turn, int stack = 0)
    {
        if (turn == -1)
        {
            turnText.text = stack.ToString();
            this.stack = stack;
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

    private void SetDescription()
    {
        descriptionObject.SetActive(true);
        descriptionText.text =
            cc.name + "\n" +
            cc.description;
        if (cc.cc == CCType.DOTDAMAGE)
        {
            int dmg = cc.dotDamage;
            descriptionDmgText.gameObject.SetActive(true);
            descriptionDmgText.text = "DMG: " + dmg;
        }
        else
        {
            descriptionDmgText.gameObject.SetActive(false);
        }
    }

    #region For PC
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        SetDescription();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        descriptionObject.SetActive(false);
    }
    #endregion

    #region For Mobile

    #endregion

}
