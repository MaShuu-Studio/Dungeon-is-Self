using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPerRoundUI : MonoBehaviour
{
    [SerializeField] private Transform offenderTransform;
    [SerializeField] private Transform defenderTransform;
    [SerializeField] private GameObject unitIconPrefab;

    public void SetResult(List<int> offenderUnit, List<int> defenderUnit)
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
    }
}
