using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDescription : MonoBehaviour
{
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);    
        rectTransform = GetComponent<RectTransform>();    
    }


    public void ShowDecription(bool isShow, Vector3 pos)
    {
        gameObject.SetActive(isShow);
        rectTransform.anchoredPosition = pos;
    }
}
