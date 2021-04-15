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

    public void ShowCandidate()
    {
        
    }

    public void ViewDungeon()
    {

    }

    public void DiceRoll()
    {

    }
}
