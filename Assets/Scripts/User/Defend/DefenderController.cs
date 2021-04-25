using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace GameControl
{
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
        public const int MAX_COST = 10;

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

        public bool SetDice(int index, MonsterSkill skill)
        {
            int count = 0;
            for (int i = 0; i < dices[monsterIndex].Length; i++)
            {
                if (dices[monsterIndex][i].id == skill.id) count++;
            }
            if (count > 1) return false;

            int totalCost = MAX_COST - GetDiceCost();
            totalCost = totalCost + dices[monsterIndex][index].cost - skill.cost;

            if (totalCost < 0) return false;

            dices[monsterIndex][index] = skill;
            return true;
        }

        public int GetDiceCost()
        {
            int cost = 0;
            foreach (MonsterSkill skill in dices[monsterIndex])
            {
                cost += skill.cost;
            }

            return cost;
        }

        public void SetAttackSkill(MonsterSkill skill)
        {
            attackSkills[monsterIndex] = skill;
        }

        public MonsterSkill GetAttackSkill()
        {
            return attackSkills[monsterIndex];
        }

        public void SetRoster()
        {
            int[] unit = new int[1];
            unit[0] = monsterIndex;
            GameController.Instance.SelectUnit(UserType.Defender, unit);
        }
        #endregion
        public MonsterSkill GetSelectedDice(int index)
        {
            if (dices.Count <= monsterIndex) return null;
            return dices[monsterIndex][index];
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

        public Monster GetMonsterRoster()
        {
            return monsters[monsterIndex];
        }
    }
}
