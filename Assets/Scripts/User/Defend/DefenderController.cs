using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenderController : MonoBehaviour
{
    [SerializeField] private DungeonController dungeonController;
    private MonsterDatabase monsterDB;

    private List<Monster> monsterCandidates;
    private int round;

    // Start is called before the first frame update
    void Start()
    {
        monsterDB = GameObject.FindWithTag("MonsterDB").GetComponent<MonsterDatabase>();
        monsterCandidates = new List<Monster>();
    }
    
    public void SetMonsterCandidate(int num)
    {
        monsterCandidates.Clear();

        for(int i = 0; i < num; i++)
        {
            while(true)
            {
                Monster monster = monsterDB.GetRandomMonster();
                if (monsterCandidates.Exists(mon => mon.name == monster.name) == false)
                {
                    monsterCandidates.Add(monster);
                    break;
                }
            }
            Debug.Log(monsterCandidates[i].name);
        }
    }

    public void ShowCandidate()
    {
        
    }

    public void ViewDungeon()
    {
        List<Monster> monsters = dungeonController.GetMonsterList(0);
    }

    public void SetRound(int round)
    {
        this.round = round;
    }

#region GUI
    private void OnGUI()
    {
        for (int i = 0; i < monsterCandidates.Count; i++)
        {
            if (GUI.Button(new Rect(10 + (i*70), 500, 50, 50), monsterCandidates[i].name))
            {
                dungeonController.AddMonster(round-1, monsterCandidates[i]);
                monsterCandidates.RemoveAt(i);
                break;
            }
        }
    }

#endregion
}
