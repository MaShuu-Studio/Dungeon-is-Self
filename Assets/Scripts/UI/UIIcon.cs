using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIDescription description;
    [SerializeField] private Vector3 pos;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        description.ShowDecription(true, pos);
    }
    
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        description.ShowDecription(false, pos);
    }
}
