using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleIcon : Toggle
{
    protected Image frame;

    protected override void Start()
    {
        base.Start();
        frame = GetComponent<Image>();
    }

    protected virtual void SetColor(Color color)
    {
        frame.color = color;
    }
}
