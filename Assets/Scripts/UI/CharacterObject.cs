using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterObject : MonoBehaviour
{
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCharacter(string name)
    {
        string path = MonsterDatabase.charPath;
        Sprite sprite = Resources.Load<Sprite>(path + name);

        image.sprite = sprite;
    }
}
