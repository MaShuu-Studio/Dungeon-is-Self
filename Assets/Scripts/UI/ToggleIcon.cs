using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleIcon : Toggle
{
    void Update()
    {
        if (isOn) targetGraphic.color = Color.white;
        else targetGraphic.color = Color.gray;
    }
}
