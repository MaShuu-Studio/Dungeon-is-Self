using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertObject : MonoBehaviour
{
    [SerializeField] private Text contentsText;

    void Start()
    {
        gameObject.SetActive(false);
    }
    public void ShowAlert(string str)
    {
        gameObject.SetActive(true);
        contentsText.text = str;
        StartCoroutine(Alert());
    }

    IEnumerator Alert()
    {
        float time = 0;
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
