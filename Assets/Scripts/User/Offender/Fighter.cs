using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Character
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "Stab"))
        {
            SetDice("Stab");
        }
    }
}