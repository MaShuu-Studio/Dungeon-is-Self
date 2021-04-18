using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDescription : MonoBehaviour
{
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();    
        gameObject.SetActive(false);    
    }

    public virtual void ShowDecription(bool isShow, Vector3 pos)
    {
        transform.SetAsFirstSibling();
        gameObject.SetActive(isShow);
        if (isShow) rectTransform.anchoredPosition = pos;
    }
}
