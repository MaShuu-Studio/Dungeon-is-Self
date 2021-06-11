using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleResult
{
    public DateTime date;
    public string gameNumber;
    public string[] players;
    public Dictionary<string, int[]> roster;
    public Dictionary<int, string> roundWinner;
    public Dictionary<int, Dictionary<string, int[]>> roundUnit;
}

public class BattleResultUI : MonoBehaviour
{
    [SerializeField] private Text dateText;
    [SerializeField] private Text playTimeText;
    [SerializeField] private Text offenderName;
    [SerializeField] private Text defenderName;
    [SerializeField] private Transform offenderBenchTransform;
    [SerializeField] private Transform defenderBenchTransform;
    [SerializeField] private GameObject unitIconPrefab;
    [SerializeField] private Transform roundResultPrefab;
    [SerializeField] private GameObject resultPerRoundPrefab;

    public void SetResult(BattleResult result)
    {
        dateText.text = string.Format("{0:yyyy}.{0:MM}.{0:dd} {0:HH}:{0:mm}", result.date);
        playTimeText.text = "";
        offenderName.text = result.players[0];
        defenderName.text = result.players[1];
        int[] offenderBench = result.roster[result.players[0]];
        int[] defenderBench = result.roster[result.players[1]];

        for (int i = 0; i < result.roster[result.players[0]].Length; i++)
        {
            GameObject obj = Instantiate(unitIconPrefab);
            obj.transform.SetParent(offenderBenchTransform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            BattleResultUnitIcon icon = obj.GetComponent<BattleResultUnitIcon>();
            icon.SetImage(GameControl.UserType.Offender, offenderBench[i]);
        }

        for (int i = 0; i < result.roster[result.players[1]].Length; i++)
        {
            GameObject obj = Instantiate(unitIconPrefab);
            obj.transform.SetParent(defenderBenchTransform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            BattleResultUnitIcon icon = obj.GetComponent<BattleResultUnitIcon>();
            icon.SetImage(GameControl.UserType.Defender, defenderBench[i]);
        }

        foreach(int round in result.roundUnit.Keys)
        {
            if (result.roundWinner.ContainsKey(round))
            {
                GameObject obj = Instantiate(resultPerRoundPrefab);
                obj.transform.SetParent(roundResultPrefab);
                obj.transform.localScale = new Vector3(1, 1, 1);
                BattleResultPerRoundUI resultRound = obj.GetComponent<BattleResultPerRoundUI>();
                int[] offenderIndex = result.roundUnit[round][result.players[0]];
                int[] defenderIndex = result.roundUnit[round][result.players[1]];
                List<int> offenderUnits = new List<int>();
                List<int> defenderUnits = new List<int>();

                foreach (int off in offenderIndex)
                    offenderUnits.Add(offenderBench[off % 10]);

                foreach (int def in defenderIndex)
                    defenderUnits.Add(defenderBench[def % 10]);

                resultRound.SetResult(offenderUnits, defenderUnits);
            }
        }
    }
}
