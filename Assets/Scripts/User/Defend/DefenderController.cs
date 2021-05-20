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

        private List<List<int>> dices = new List<List<int>>();
        private List<List<MonsterSkill>> skillRoster = new List<List<MonsterSkill>>();
        private List<MonsterSkill> attackSkills = new List<MonsterSkill>();
        private int attackSkillTurn;
        private int monsterIndex;
        public int monsterRoster { get; private set; } = 0;
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
                List<MonsterSkill> skillCand = new List<MonsterSkill>();
                List<int> diceCand = new List<int>();
                MonsterSkill attackSkill;
                attackSkill = monsters[i].GetBasicSkill();

                skillRoster.Add(skillCand);
                dices.Add(diceCand);
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
            for (int i = 0; i < monsters.Count; i++)
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

        public void SelectRoster(int index)
        {
            if (isDead[index]) return;
            monsterRoster = index;
        }

        public bool HasDice(int i)
        {
            return (dices[monsterIndex].FindIndex(index => index == i) != -1);
        }

        public int SetDice(bool isRosterToDice, int skillIdx)
        {
            if (isRosterToDice)
            {
                dices[monsterIndex].Add(skillIdx);
            }
            else
            {
                dices[monsterIndex].RemoveAt(skillIdx);
            }
            return 0;
        }

        public int SetSkillRoster(MonsterSkill skill)
        {
            if (skillRoster[monsterIndex].Count > 8) return 30;
            int count = 0;

            for (int i = 0; i < skillRoster[monsterIndex].Count; i++)
            {
                if (skillRoster[monsterIndex][i].id == 200110) continue;
                if (skillRoster[monsterIndex][i].id == skill.id) count++;
            }
            if (count > 1) return 21;

            int totalCost = MAX_COST - GetDiceCost();
            totalCost = totalCost - skill.cost;

            if (totalCost < 0) return 22;

            skillRoster[monsterIndex].Add(skill);
            return 0;
        }

        public List<int> GetSkillRosterWithUnit(int unitIndex)
        {
            List<int> skills = new List<int>();
            foreach (MonsterSkill skill in skillRoster[unitIndex])
            {
                skills.Add(skill.id);
            }
            return skills;
        }

        public MonsterSkill GetSkillRoster(int i)
        {
            return skillRoster[monsterIndex][i];
        }

        public int GetSkillRosterSize()
        {
            return skillRoster[monsterIndex].Count;
        }

        public List<int> GetDicesWithUnit(int unitIndex)
        {
            return dices[unitIndex];
        }

        public int GetDiceSize()
        {
            return dices[monsterIndex].Count;
        }

        public void RemoveSkillRoster(int index)
        {
            if (skillRoster[monsterIndex].Count <= index + 2) return;
            skillRoster[monsterIndex].RemoveAt(index);
        }
        public void SetSkillRoster(List<S_RoundReadyEnd.EnemyRoster> infos)
        {
            for (int i = 0; i < infos.Count; i++)
            {
                monsterIndex = infos[i].unitIndex % 10;
                skillRoster[i].Clear();
                for (int j = 0; j < infos[i].skillRosters.Count; j++)
                {
                    skillRoster[monsterIndex].Add(SkillDatabase.Instance.GetMonsterSkill(infos[i].skillRosters[j]));
                }
            }
        }

        public int GetDiceCost()
        {
            int cost = 0;
            foreach (int rosterIndex in dices[monsterIndex])
            {
                cost += skillRoster[monsterIndex][rosterIndex].cost;
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
        public MonsterSkill GetAttackSkillWithUnit(int unitIndex)
        {
            return attackSkills[unitIndex];
        }

        public void SetRoster()
        {
            int[] unit = new int[1];
            unit[0] = monsterRoster;
            ResetAttackSkill();
            GameController.Instance.SelectUnit(UserType.Defender, unit);
        }

        public void AddRoster(int index, int selected)
        {
            if (index == 0) monsterIndex = selected;
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

        public void SetMonsterHp(int hp)
        {
            monsters[monsterRoster].SetHP(hp);
        }
        public int GetSelectedDice(int index)
        {
            if (dices.Count <= monsterIndex) return 0;
            return dices[monsterIndex][index];
        }
        /*
        public MonsterSkill DiceRoll(int roster, bool isParalysis)
        {
            int[] result = new int[6] { 0, 0, 0, 0, 0, 0 };
            int diceIndex = new int();
            for (int i = 0; i < 3; i++)
            {
                diceIndex = Random.Range(0, dices[roster % 10].Count);
                if (isParalysis)
                {
                    int blindAmount = 3;
                    List<int> blind = new List<int>();
                    for (int j = 0; j < blindAmount; j++)
                        while (true)
                        {
                            int index = Random.Range(0, dices[roster % 10].Count);
                            if (blind.FindIndex(n => n == index) == -1) break;
                        }
                    while (true)
                    {
                        diceIndex = Random.Range(0, dices[roster % 10].Count);
                        if (blind.FindIndex(n => n == diceIndex) == -1) break;
                    }
                }
                int check = 0;
                for (int j = 0; j < 6; j++)
                {
                    if (result[j] > 0 && (dices[roster % 10][j].id == dices[roster % 10][diceIndex].id)) { check = 1; result[j] += 1; break; }
                }
                if (check == 0) result[diceIndex] += 1;

                if (i >= 2)
                {
                    int max = result[0];
                    for (int j = 1; j < 6; j++)
                    {
                        if (result[j] > max) max = result[j];
                    }
                    if (max >= 2)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            if (result[j] == max) { diceIndex = j; break; }
                        }
                    }
                    else
                    {
                        List<int> tmp = new List<int>();
                        for (int j = 0; j < 6; j++)
                        {
                            if (result[j] == 1) tmp.Add(j);
                        }
                        int n = Random.Range(0, tmp.Count);
                        diceIndex = result[n];
                    }
                }
            }


            return skillRoster[roster % 10][dices[roster % 10][diceIndex]];
        }*/

        public Monster GetMonsterRoster()
        {
            return monsters[monsterRoster];
        }

        public int MonsterDamaged(int index, int damage)
        {
            monsters[index].Damaged(damage);
            return monsters[index].hp;
        }

        public void GetMonsterInfo(ref int hp, ref int turn)
        {
            hp = monsters[monsterRoster].hp;
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
            attackSkillTurn = attackSkills[monsterRoster].turn;
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
