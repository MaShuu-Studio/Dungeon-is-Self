using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;

public class DefenderController : MonoBehaviour
{
    #region Instance
    private static DefenderController instance;
    public static DefenderController Instance
    {
        get
        {
            var obj = FindObjectOfType<DefenderController>();
            instance = obj;
            return instance;
        }
    }
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public string[] selectedMonsterCandidates { get; private set; } = new string[6];
    private List<Monster> monsters = new List<Monster>();

    Dictionary<int, bool> Roster;

    // Start is called before the first frame update
    void Start()
    {
        //monsterDB = MonsterDatabase.Instance;
        //monsterDB = GameObject.FindWithTag("MonsterDB").GetComponent<MonsterDatabase>();
        monsters = new List<Monster>();
    }

    // 게임이 시작될 때 Defender에 대한 초기화 진행
    public void Init(List<string> monsterNames)
    {
        // 어떠한 몬스터를 가져갈지 결정을 해둔 뒤기 때문에
        // 바로 몬스터 리스트에 등록

        monsters.Clear();

        foreach (string name in monsterNames)
        {
            monsters.Add(MonsterDatabase.Instance.GetMonster(name));
        }

    }

    public void ViewDungeon()
    {

    }

    public void DiceRoll()
    {

    }

    public void SetMonsterRoster(int num, string name)
    {
        selectedMonsterCandidates[num] = name;

        for (int i = 0; i < 6; i++)
        {
            Debug.Log($"Monster {i}. {selectedMonsterCandidates[i]}");
        }
    }

    public bool CheckCadndidate()
    {
        foreach (string s in selectedMonsterCandidates)
        {
            if (string.IsNullOrEmpty(s)) return false;
        }
        return true;
    }
}
