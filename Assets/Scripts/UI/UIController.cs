using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public List<Text> button_texts;
    public Text monster_text; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Defender
    public void ShowMonsterList(List<Monster> monsters)
    {
        for (int i = 0; i < button_texts.Count; i++)
        {
            button_texts[i].text = monsters[i].name;
        }
    }

    public void ViewMonster(List<Monster> monsters)
    {
        string str = "";
        foreach(Monster mon in monsters)
        {
            str += mon.name + " ";
        }
        monster_text.text = str;
    }

    #endregion
}
