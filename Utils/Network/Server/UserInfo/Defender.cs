using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Server
{
    class Defender
    {
        List<int> _candidates = new List<int>();
        List<int> _rosters = new List<int>();

        List<List<int>> _skillRosters = new List<List<int>>();
        List<List<int>> _dices = new List<List<int>>();
        List<int> _deadUnits = new List<int>();

        Dictionary<int, List<CrowdControl>> _ccList = new Dictionary<int, List<CrowdControl>>();

        Monster _monster = null;
        MonsterSkill _attackSkill = null;
        int _attackSkillTurn;

        public List<int> Candidates { get { return _candidates; } }
        public List<int> Rosters { get { return _rosters; } }
        public List<List<int>> SkillRosters { get { return _skillRosters; } }
        public List<List<int>> Dices { get { return _dices; } }
        public MonsterSkill AttackSkill { get { return _attackSkill; } }
        public Dictionary<int, List<CrowdControl>> CCList { get { return _ccList; } }
        public List<int> DeadUnits { get { return _deadUnits; } }
        public int MonsterHp { get { return _monster.hp; } }
        public int MonsterAttackTurn { get { return _attackSkillTurn; } }

        public void SetCandidate(List<int> cs)
        {
            _candidates.Clear();
            for (int i = 0; i < cs.Count; i++)
            {
                _candidates.Add(cs[i]);
            }
        }

        public void SetRoster(List<int> rosters, List<List<int>> skillRosters, List<int> attackSkill, int round)
        {
            _rosters.Clear();
            _skillRosters.Clear();
            _ccList.Clear();

            for (int i = 0; i < rosters.Count; i++)
            {
                _rosters.Add(rosters[i] + 20);
                _ccList.Add(rosters[i] + 20, new List<CrowdControl>());
                _monster = MonsterDatabase.Instance.GetMonster(_candidates[_rosters[i] / 10]);
                _skillRosters.Add(skillRosters[i]);
                _attackSkill = SkillDatabase.Instance.GetMonsterSkill(attackSkill[i]);
                _attackSkillTurn = _attackSkill.turn;
                _monster.Heal(round);
            }
        }

        public void SetDice(List<List<int>> dices)
        {
            _dices.Clear();
            for (int i = 0; i < dices.Count; i++)
            {
                _dices.Add(dices[i]);
            }
        }

        public Dictionary<int, List<int>> DiceRoll()
        {
            Dictionary<int, List<int>> diceResults = new Dictionary<int, List<int>>();
            for (int i = 0; i < _rosters.Count; i++)
            {
                List<int> dices = new List<int>();
                Random rand = new Random();
                if (IsDead() == false)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int diceId;
                        int diceIndex = rand.Next(0, _dices[i].Count);
                        if (_dices[i][diceIndex] == -1) diceId = SkillDatabase.Instance.GetCharacterBasicSkill(Candidates[_rosters[i] % 10]).id;
                        else diceId = SkillRosters[i][_dices[i][diceIndex]];

                        dices.Add(diceId);
                    }
                }
                diceResults.Add(_rosters[i], dices);
            }

            return diceResults;
        }

        public int Damaged(int damage)
        {
            _monster.Damaged(damage);
            return _monster.hp;
        }

        public bool IsDead()
        {
            if (_monster.hp <= 0)
            {
                DeadUnits.Add(_rosters[0]);
                return true;
            }
            return false;
        }

        public void ProgressTurn()
        {
            _attackSkillTurn--;
            CrowdControlProgressTurn();
        }

        public void ResetAttackTurn()
        {
            _attackSkillTurn = _attackSkill.turn;
        }

        private void HealUnit(int amount)
        {
            _monster.Cure(amount);
        }

        #region Crowd Control

        public List<CrowdControl> GetCCList(int unit)
        {
            List<CrowdControl> ccs = new List<CrowdControl>();

            for (int i = 0; i < _ccList[unit].Count; i++)
            {
                ccs.Add(new CrowdControl(_ccList[unit][i]));
            }

            return ccs;
        }

        public void AddCrowdControl(CrowdControl cc, int ccStack, int usingUnit, int ccMultiplier, Offender offender)
        {
            int ccTurn = cc.turn;

            if (ccMultiplier != 1)
            {
                ccStack *= ccMultiplier;
                ccTurn = cc.turn + 1;
            }

            if (cc.cc == CCType.PURITY || cc.cc == CCType.INVINCIBLE)
            {
                PurifyCrowdControl(_rosters[0], true);
                if (cc.cc == CCType.PURITY) return;
            }
            else if (cc.cc == CCType.REMOVE)
            {
                PurifyCrowdControl(_rosters[0], false);
                return;
            }
            else if (cc.cc == CCType.DRAIN)
            {
                HealUnit(ccStack);
                return;
            }

            if (HasCrowdControl(_rosters[0], CCType.BARRIER) || HasCrowdControl(_rosters[0], CCType.INVINCIBLE))
            {
                return;
            }
            else if (HasCrowdControl(_rosters[0], CCType.REFLECT))
            {
                if (offender.HasCrowdControl(usingUnit, CCType.REFLECT) == false)
                    offender.AddCrowdControl(usingUnit, cc, ccStack, _rosters[0], 1, this, false);
                return;
            }

            int ccIndex = _ccList[_rosters[0]].FindIndex(charCC => charCC.name == cc.name);

            if (ccIndex == -1)
            {
                _ccList[_rosters[0]].Add(SkillDatabase.Instance.GetCrowdControl(cc.id));
                _ccList[_rosters[0]][_ccList[_rosters[0]].Count - 1].SetTurn(ccTurn);

                if (cc.cc == CCType.DOTDAMAGE)
                    _ccList[_rosters[0]][_ccList[_rosters[0]].Count - 1].SetDotDamage(ccStack);

                bool isStackSkill = _ccList[_rosters[0]][_ccList[_rosters[0]].Count - 1].ControlCC(ccStack);
            }
            else
            {
                CrowdControl curCC = _ccList[_rosters[0]][ccIndex];

                if (curCC.cc == CCType.DOTDAMAGE && curCC.id == cc.id && curCC.dotDamage < ccStack)
                {
                    curCC.SetDotDamage(ccStack);
                }

                bool isStackSkill = curCC.ControlCC(ccStack); // 스택 자동으로 쌓음

                if (isStackSkill == false)
                {
                    if (curCC.turn < ccTurn) curCC.SetTurn(ccTurn);
                }
                else
                {
                    int curStack = curCC.stack;
                    // CC 발동 시점
                    if (curStack == 0)
                    {
                        // CC 발동
                        curCC.SetTurn(ccTurn);
                    }
                    // CC 스택 쌓는 시점
                    else if (curStack > 0)
                    {
                    }
                    // CC 발동 후 이번 턴에 발동된 CC가 아니라면 스택 리셋 후 해당수치만큼 감소
                    else if (curCC.turn != curCC.GetCCBasicTurn())
                    {
                        curCC.ResetCCStack();
                        curCC.ControlCC(ccStack);
                    }
                }
            }
        }

        private bool CrowdControlProgressTurn()
        {
            foreach (int key in _ccList.Keys)
            {
                for (int i = 0; i < _ccList[key].Count; i++)
                {
                    CrowdControl curCC = _ccList[key][i];
                    bool isStackSkill = curCC.ControlCC(0);

                    if (isStackSkill && curCC.stack > 0 && curCC.turn == curCC.GetCCBasicTurn()) continue;

                    bool b = curCC.ProgressTurn();
                    if (curCC.cc == CCType.DOTDAMAGE)
                    {
                        int restHp = Damaged(curCC.dotDamage);
                        if (restHp < 0)
                        {
                            return true;
                        }
                    }

                    if (b)
                    {
                        _ccList[key].RemoveAt(i);
                        i--;
                        if (isStackSkill && curCC.stack > 0)
                        {
                            // 만약에 지웠는데 스택이 남았었다면 턴 초기화 후 그대로 다시 추가
                            curCC.SetTurn(curCC.GetCCBasicTurn());
                            _ccList[key].Add(curCC);
                        }
                    }
                }
            }
            return false;
        }

        public bool HasCrowdControl(int index, CCType ccType, CCTarget target = CCTarget.ENEMY)
        {
            CrowdControl tmp = _ccList[index].Find(cc => cc.cc == ccType);

            bool b = tmp != null;
            if (b)
            {
                if (ccType == CCType.ATTACKSTAT) b = (tmp.target == target);
                if (tmp.ControlCC(0))
                {
                    if (tmp.stack <= 0) b = true;
                    else b = false;
                }
            }

            return b;
        }

        private void PurifyCrowdControl(int index, bool isGood)
        {
            for (int i = 0; i < _ccList[index].Count; i++)
            {
                if (isGood)
                {
                    if (_ccList[index][i].target == CCTarget.ENEMY)
                    {
                        _ccList[index].RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    if (_ccList[index][i].target == CCTarget.ALL || _ccList[index][i].target == CCTarget.SELF)
                    {
                        _ccList[index].RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        #endregion


        #region For Bot

        List<Monster> _monsters = new List<Monster>();
        public List<Monster> Monsters { get { return _monsters; } }

        public void SetMonsters()
        {
            _monsters.Clear();
            for (int i = 0; i < _candidates.Count; i++)
            {
                Monster c = MonsterDatabase.Instance.GetMonster(_candidates[i]);
                _monsters.Add(c);
            }
        }

        public List<MonsterSkill> GetUsableSkill(int monsterIndex, int round)
        {
            List<MonsterSkill> usableSkill = new List<MonsterSkill>();
            usableSkill = SkillDatabase.Instance.GetMonsterDices(_monsters[monsterIndex].id);

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
        #endregion

    }
}
