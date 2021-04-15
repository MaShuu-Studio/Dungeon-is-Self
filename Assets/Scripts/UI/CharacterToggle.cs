using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterToggle : MonoBehaviour
{
    [SerializeField] private Image face;
    public Toggle toggle { get; private set; }
    private Image frame;

    private void Start()
    {
        frame = GetComponent<Image>();
        toggle = GetComponent<Toggle>();
    }
    void Update()
    {
        if (toggle.isOn) SetColor(Color.white);
        else SetColor(Color.gray);
    }

    public void SetFace(GameControl.UserType type, string name)
    {
        string path = (type == GameControl.UserType.Defender) ? MonsterDatabase.facePath : "";

        Sprite sprite = Resources.Load<Sprite>(path + name);
        face.sprite = sprite;
    }
    private void SetColor(Color color)
    {
        frame.color = color;
        if (face != null) face.color = color;
    }
}
