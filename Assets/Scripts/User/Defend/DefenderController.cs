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
        public List<bool> isDead { get; private set; } = new List<bool>();
        public List<Monster> monsters { get; private set; } = new List<Monster>();

        private List<MonsterSkill[]> dices = new List<MonsterSkill[]>();
        private List<MonsterSkill> attackSkills = new List<MonsterSkill>();
        private int attackSkillTurn;
        private int monsterIndex = 0;
        public const int MAX_COST = 10;

        // 게임이 시작될 때 Defender에 대한 초기화 진행
        public void Init()
        {
            monsterIndex = 0;
            
            monsters.Clear();

            foreach (string name in selectedMonsterCandidates)
            {
                monsters.Add(MonsterDatabase.Instance.GetMonster(name));
                isDead.Add(false);
            }


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

        public void ResetCandidates()
        {
            for (int i = 0; i < selectedMonsterCandidates.Length; i++)
            {
                selectedMonsterCandidates[i] = "";
            }
        }
        #endregion

        #region Ready Round
        public int GetFirstAliveMonster()
        {
            int i = 0;
            for (; i < monsters.Count; i++)
            {
                if (isDead[i] == false) break;
            }
            return i;
        }
        public void SelectMonster(int index)
        {
            if (isDead[index]) return;
            monsterIndex = index;
        }

        public int SetDice(int index, MonsterSkill skill)
        {
            int count = 0;
            for (int i = 0; i < dices[monsterIndex].Length; i++)
            {
                if (dices[monsterIndex][i].id == skill.id) count++;
            }
            if (count > 1) return 1;

            int totalCost = MAX_COST - GetDiceCost();
            totalCost = totalCost + dices[monsterIndex][index].cost - skill.cost;

            if (totalCost < 0) return 2;

            dices[monsterIndex][index] = skill;
            return 0;
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
            ResetAttackSkill();
            GameController.Instance.SelectUnit(UserType.Defender, unit);
        }
        #endregion

        #region Play Round
        public MonsterSkill GetSelectedDice(int index)
        {
            if (dices.Count <= monsterIndex) return null;
            return dices[monsterIndex][index];
        }

        public List<MonsterSkill> DiceRoll(int index)
        {
            List<MonsterSkill> skills = new List<MonsterSkill>();

            for (int i = 0; i < 2; i ++)
            {
                int diceIndex = Random.Range(0, 6);
                skills.Add(dices[index][diceIndex]);
            }

            return skills;
        }

        public Monster GetMonsterRoster()
        {
            return monsters[monsterIndex];
        }

        public int MonsterDamaged(int index, CharacterSkill skill)
        {
            monsters[index].Damaged(skill);

            return monsters[index].hp;
        }

        public void GetMonsterInfo(ref int hp, ref int turn)
        {
            hp = monsters[monsterIndex].hp;
            turn = attackSkillTurn;
        }

        public bool AttackSkillNextTurn()
        {
            attackSkillTurn--;
            if (attackSkillTurn <= 0)
            {
                attackSkillTurn = 0;
                return true;
            }
            return false;
        }

        public void ResetAttackSkill()
        {
            attackSkillTurn = attackSkills[monsterIndex].turn;
        }

        public void Dead(int index)
        {
            isDead[index % 10] = true;
        }

        #endregion
    }
}
