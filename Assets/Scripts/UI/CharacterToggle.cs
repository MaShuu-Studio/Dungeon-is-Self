using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class CharacterToggle : MonoBehaviour
{
    [SerializeField] private GameObject deadIcon;
    [SerializeField] private Image face;
    [SerializeField] private int index;
    public Toggle toggle { get; private set; }
    private Image frame;

    private void Awake()
    {
        deadIcon.SetActive(false);
        frame = GetComponent<Image>();
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(SetSkillTree); 
    }
    void Update()
    {
        if (toggle.isOn) SetColor(Color.white);
        else SetColor(Color.gray);
    }

    public void SetFace(GameControl.UserType type, int id)
    {
        string path = (type == GameControl.UserType.Defender) ? MonsterDatabase.facePath : CharacterDatabase.facePath;

        Sprite sprite = Resources.Load<Sprite>(path + id.ToString());
        face.sprite = sprite;
    }
    private void SetColor(Color color)
    {
        frame.color = color;
        if (face != null) face.color = color;
    }

    private void SetSkillTree(bool b)
    {
        if (b)
        {
            GamePlayUIController.Instance.SetSkillTree(index);
        }
    }

    public void CharacterDead(bool isDead)
    {
        if (isDead)
        {
            toggle.interactable = false;
            deadIcon.SetActive(true);
            SetColor(Color.gray);
        }
    }
}
