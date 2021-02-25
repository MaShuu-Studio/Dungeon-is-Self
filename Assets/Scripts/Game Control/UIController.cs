using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public List<Text> button_texts;
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

    #endregion
}
