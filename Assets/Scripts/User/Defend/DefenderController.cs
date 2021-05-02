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

        public int[] selectedMonsterCandidates { get; private set; } = new int[6];
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
            isDead.Clear();

            foreach (int id in selectedMonsterCandidates)
            {
                monsters.Add(MonsterDatabase.Instance.GetMonster(id));
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
        public void SetMonsterCandidate(int num, int id)
        {
            selectedMonsterCandidates[num] = id;
        }

        public bool CheckCadndidate()
        {
            foreach (int id in selectedMonsterCandidates)
            {
                if (id == -1) return false;
            }
            return true;
        }

        public void ResetCandidates()
        {
            for (int i = 0; i < selectedMonsterCandidates.Length; i++)
            {
                selectedMonsterCandidates[i] = -1;
            }
        }
        public void ResetDead()
        {
            for (int i = 0; i < isDead.Count; i++) isDead[i] = false;
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

        public List<int> GetAliveMonsterList()
        {
            List<int> alives = new List<int>();
            for (int i = 0 ; i < monsters.Count; i++)
            {
                if (isDead[i] == false) alives.Add(i);
            }

            return alives;
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
                if (dices[monsterIndex][i].id == 200110) continue;
                if (dices[monsterIndex][i].id == skill.id && index != i) count++;
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

        public void SetMonsterHp()
        {
            foreach (Monster monster in monsters)
            {
                monster.Heal(GameController.Instance.round);
            }
        }
        #endregion

        #region Play Round
        public MonsterSkill GetSelectedDice(int index)
        {
            if (dices.Count <= monsterIndex) return null;
            return dices[monsterIndex][index];
        }

        public MonsterSkill DiceRoll(int roster, bool isParalysis)
        {
            int diceIndex = Random.Range(0, dices[roster % 10].Length);

            if (isParalysis)
            {
                int blindAmount = 3;
                List<int> blind = new List<int>();
                for (int i = 0; i < blindAmount; i++)
                    while (true)
                    {
                        int index = Random.Range(0, dices[roster % 10].Length);
                        if (blind.FindIndex(n => n == index) == -1) break;
                    }
                while (true)
                {
                    diceIndex = Random.Range(0, dices[roster % 10].Length);
                    if (blind.FindIndex(n => n == diceIndex) == -1) break;
                }
            }

            return dices[roster % 10][diceIndex];
        }

        public Monster GetMonsterRoster()
        {
            return monsters[monsterIndex];
        }

        public int MonsterDamaged(int index, int damage)
        {
            monsters[index].Damaged(damage);
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

        public void HealBattleMonster(int index)
        {
            monsters[index % 10].Heal(GameController.Instance.round);
        }

        public void HealBattleMonster(int index, int amount)
        {
            monsters[index % 10].Cure(amount);
        }

        #endregion

        public List<MonsterSkill> GetUsableSkill(int round)
        {
            List<MonsterSkill> usableSkill = new List<MonsterSkill>();
            usableSkill = SkillDatabase.Instance.GetMonsterDices(monsters[monsterIndex].id);
            
            for (int i = 0; i < usableSkill.Count; i++)
            {
                if (usableSkill[i].tier > round)
                {
                    usableSkill.RemoveAt(i);
                    i--;
                }
            }

            return usableSkill;
        }
    }
}
