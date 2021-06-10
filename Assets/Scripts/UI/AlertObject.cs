using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlertObject : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text contentsText;
    [SerializeField] private Image background;
    IEnumerator coroutine = null;

    float alpha;

    void Start()
    {
        gameObject.SetActive(false);
        alpha = background.color.a;
    }
    public void RemoveAlert()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        gameObject.SetActive(false);
    }
    public void ShowAlert(string str, float time)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        gameObject.SetActive(true);
        contentsText.text = str;

        nameText.color = Color.white;
        contentsText.color = Color.white;
        background.color = new Color(0, 0, 0, alpha);

        coroutine = Alert(time);
        StartCoroutine(coroutine);
    }

    IEnumerator Alert(float maxTime)
    {
        float max = maxTime;
        float colorTime = maxTime / 7;
        float time = 0;
        while (time < max)
        {
            time += Time.deltaTime;
            if (time >= max - colorTime)
            {
                nameText.color = new Color(1, 1, 1, (max - time) / colorTime);
                contentsText.color = new Color(1, 1, 1, (max - time) / colorTime);
                background.color = new Color(0, 0, 0, (max - time) / colorTime * alpha);
            }
            yield return null;
        }
        
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            gameObject.SetActive(false);
        }
    }
}
