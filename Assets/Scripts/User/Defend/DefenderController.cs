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
    public List<Monster> monsters { get; private set; } = new List<Monster>();

    private List<MonsterSkill[]> dices = new List<MonsterSkill[]>();
    private List<MonsterSkill> attackSkills = new List<MonsterSkill>();
    private int monsterIndex;

    // Start is called before the first frame update
    void Start()
    {
        //monsterDB = MonsterDatabase.Instance;
        //monsterDB = GameObject.FindWithTag("MonsterDB").GetComponent<MonsterDatabase>();
        monsters = new List<Monster>();
    }

    // 게임이 시작될 때 Defender에 대한 초기화 진행
    public void Init()
    {
        monsters.Clear();

        foreach (string name in selectedMonsterCandidates)
        {
            monsters.Add(MonsterDatabase.Instance.GetMonster(name));
        }

        monsterIndex = 0;

        dices.Clear();
        attackSkills.Clear();

        for (int i = 0; i < monsters.Count; i++)
        {
            MonsterSkill[] dice = new MonsterSkill[6];
            MonsterSkill attackSkill;
            monsters[i].SetBasicDice(ref dice);
            attackSkill = monsters[i].GetBasicSkill();

            dices.Add(dice);
            attackSkills.Add(attackSkill);
        }
    }

    public void ViewDungeon()
    {

    }

    public MonsterSkill[] DiceRoll(int index)
    {
        MonsterSkill[] skills = new MonsterSkill[2];
        int diceIndex1 = Random.Range(0, 6);
        int diceIndex2 = Random.Range(0, 6);
        skills[0] = dices[index][diceIndex1];
        skills[1] = dices[index][diceIndex2];

        return skills;
    }

    #region Ready Game
    public void SetMonsterCandidate(int num, string name)
    {
        selectedMonsterCandidates[num] = name;
    }

    public bool CheckCadndidate()
    {
        foreach (string s in selectedMonsterCandidates)
        {
            if (string.IsNullOrEmpty(s)) return false;
        }
        return true;
    }
    #endregion

    #region Ready Round
    public void SelectMonster(int index)
    {
        monsterIndex = index;
    }

    public MonsterSkill GetSelectedDice(int index)
    {
        return dices[monsterIndex][index];
    }

    public void SetRoster()
    {
        List<int> unit = new List<int>();
        unit.Add(monsterIndex);
        GameController.Instance.SelectUnit(UserType.Defender, unit);
    }

    #endregion
}
