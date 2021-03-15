using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDescription : MonoBehaviour
{
    private RectTransform rectTransform;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();    
        gameObject.SetActive(false);    
    }

    public void ShowDecription(bool isShow, Vector3 pos)
    {
        gameObject.SetActive(isShow);
        rectTransform.anchoredPosition = pos;
    }
}
