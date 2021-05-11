using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconBar : MonoBehaviour, IDropHandler
{
    private enum BarType { ROSTER, BENCH }
    [SerializeField] private BarType barType;
    [SerializeField] private DragAndDropIcon dragIcon;
    public void OnDrop(PointerEventData eventData)
    {
        if (dragIcon.charIcon == null) return;

        switch (barType)
        {
            case BarType.ROSTER:
                break;
            case BarType.BENCH:
                break;
        }

        dragIcon.charIcon.transform.SetParent(this.transform);
    }
}
