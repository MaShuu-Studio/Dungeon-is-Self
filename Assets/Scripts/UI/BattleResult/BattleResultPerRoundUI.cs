using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPerRoundUI : MonoBehaviour
{
    [SerializeField] private RectTransform offenderTransform;
    [SerializeField] private RectTransform defenderTransform;
    [SerializeField] private GameObject unitIconPrefab;
    [SerializeField] private GameObject winCountPrefab;

    public void SetResult(List<int> offenderUnit, List<int> defenderUnit, int winner)
    {
        for (int i = 0; i < offenderUnit.Count; i++)
        {
            GameObject obj = Instantiate(unitIconPrefab);
            obj.transform.SetParent(offenderTransform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            BattleResultUnitIcon icon = obj.GetComponent<BattleResultUnitIcon>();
            icon.SetImage(GameControl.UserType.Offender, offenderUnit[i]);
        }

        for (int i = 0; i < defenderUnit.Count; i++)
        {
            GameObject obj = Instantiate(unitIconPrefab);
            obj.transform.SetParent(defenderTransform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            BattleResultUnitIcon icon = obj.GetComponent<BattleResultUnitIcon>();
            icon.SetImage(GameControl.UserType.Defender, defenderUnit[i]);
        }

        GameObject winObj = Instantiate(winCountPrefab);
        winObj.transform.SetParent(transform);
        winObj.transform.localScale = new Vector3(1, 1, 1);
        RectTransform rect = winObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(20, 20);

        float x = 0;

        if (winner == 0)
        {
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            float y = (offenderTransform.sizeDelta.y / 2 - rect.rect.height / 2) + offenderTransform.anchoredPosition.y;
            rect.anchoredPosition = new Vector2(x, y);
        }
        else
        {
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            float y = -1 * (defenderTransform.sizeDelta.y / 2 - rect.rect.height / 2) + defenderTransform.anchoredPosition.y;
            rect.anchoredPosition = new Vector2(x, y);
        }
    }
}
