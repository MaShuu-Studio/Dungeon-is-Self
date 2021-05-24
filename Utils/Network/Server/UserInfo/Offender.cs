using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Server
{
    class Offender
    {
        List<int> _candidates = new List<int>();
        List<int> _rosters = new List<int>();
        List<List<int>> _skillRosters = new List<List<int>>();
        List<List<int>> _dices = new List<List<int>>();
        List<int> _deadUnits = new List<int>();
        Dictionary<int, List<CrowdControl>> _ccList = new Dictionary<int, List<CrowdControl>>();

        public List<int> Candidates { get { return _candidates; } }
        public List<int> Rosters { get { return _rosters; } }
        public List<List<int>> SkillRosters { get { return _skillRosters; } }
        public List<List<int>> Dices { get { return _dices; } }
        public Dictionary<int, List<CrowdControl>> CCList { get { return _ccList; } }
        public List<int> DeadUnits { get { return _deadUnits; } }

        public void SetCandidate(List<int> cs)
        {
            _candidates.Clear();
            for (int i = 0; i < cs.Count; i++)
            {
                _candidates.Add(cs[i]);
            }
        }

        public void SetRoster(List<int> rosters, List<List<int>> skillRosters)
        {
            _rosters.Clear();
            _skillRosters.Clear();
            _ccList.Clear();
            for (int i = 0; i < rosters.Count; i++)
            {
                _rosters.Add(rosters[i] + 10);
                _ccList.Add(rosters[i] + 10, new List<CrowdControl>());
                _skillRosters.Add(skillRosters[i]);
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

                if (IsDead(_rosters[i]) == false)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        int diceId;
                        int diceIndex = rand.Next(0, _dices[i].Count);
                        if (_dices[i][j] == -1) diceId = SkillDatabase.Instance.GetCharacterBasicSkill(Candidates[_rosters[i] % 10]).id;
                        else diceId = SkillRosters[i][diceIndex];

                        dices.Add(diceId);
                    }
                }

                diceResults.Add(_rosters[i], dices);
            }

            return diceResults;
        }

        public void ProgressTurn()
        {
            CrowdControlProgressTurn();
        }

        public void KillUnit(int unit)
        {
            _deadUnits.Add(unit);
        }

        public List<int> GetAlives()
        {
            List<int> alives = new List<int>(Rosters);

            for (int i = 0; i < _deadUnits.Count; i++)
            {
                if (alives.Contains(_deadUnits[i]))
                    alives.Remove(_deadUnits[i]);
            }

            return alives;
        }

        public void NextRound(bool isWin)
        {
            if (isWin)
            {
                for (int i = 0; i < _rosters.Count; i++)
                {
                    if (_deadUnits.Contains(_rosters[i]))
                        _deadUnits.Remove(_rosters[i]);
                }
            }
            else
            {
                Random rand = new Random();
                if (_deadUnits.Count == 3)
                {
                    // 2명 살려줌
                    for (int i = 0; i < 2; i++)
                    {
                        int index = rand.Next(0, _deadUnits.Count);
                        _deadUnits.RemoveAt(index);
                    }
                }
                else if (_deadUnits.Count == 4)
                {
                    // 1명 살려줌
                    int index = rand.Next(0, _deadUnits.Count);
                    _deadUnits.RemoveAt(index);
                }
            }
        }

        public bool IsDead(int unit)
        {
            return _deadUnits.FindIndex(index => index == unit) != -1;
        }

        public List<CrowdControl> GetCCList(int unit)
        {
            List<CrowdControl> ccs = new List<CrowdControl>();

            for (int i = 0; i < _ccList[unit].Count; i++)
            {
                ccs.Add(new CrowdControl(_ccList[unit][i]));
            }

            return ccs;
        }

        #region CrowdControl
        public void AddCrowdControl(int target, CrowdControl cc, int ccStack, int usingUnit, int ccMultiplier, Defender defender, bool isAll = false)
        {
            int ccTurn = cc.turn;

            if (ccMultiplier != 1)
            {
                ccStack *= ccMultiplier;
                ccTurn = cc.turn + 1;
            }

            List<int> targets = new List<int>();
            if (isAll)
            {
                foreach (int key in _rosters)
                {
                    if (_deadUnits.FindIndex(index => index == key) != -1) targets.Add(key);
                }
            }
            else targets.Add(target);

            for (int i = 0; i < targets.Count; i++)
            {
                if (cc.cc == CCType.PURITY || cc.cc == CCType.INVINCIBLE)
                {
                    PurifyCrowdControl(targets[i], true);
                    if (cc.cc == CCType.PURITY) continue;
                }
                else if (cc.cc == CCType.REMOVE)
                {
                    PurifyCrowdControl(targets[i], false);
                    continue;
                }

                if (cc.target == CCTarget.ENEMY)
                {
                    if (HasCrowdControl(targets[i], CCType.BARRIER) || HasCrowdControl(targets[i], CCType.INVINCIBLE))
                    {
                        continue;
                    }
                    else if (HasCrowdControl(targets[i], CCType.REFLECT))
                    {

                        if (defender.HasCrowdControl(usingUnit, CCType.REFLECT) == false)
                            defender.AddCrowdControl(cc, ccStack, targets[i], 1, this); 
                        continue;
                    }
                }

                int ccIndex = _ccList[targets[i]].FindIndex(charCC => charCC.name == cc.name);

                if (ccIndex == -1)
                {
                    _ccList[targets[i]].Add(SkillDatabase.Instance.GetCrowdControl(cc.id));
                    _ccList[targets[i]][_ccList[targets[i]].Count - 1].SetTurn(ccTurn);

                    bool isStackSkill = _ccList[targets[i]][_ccList[targets[i]].Count - 1].ControlCC(ccStack);
                }
                else
                {
                    CrowdControl curCC = _ccList[targets[i]][ccIndex];

                    bool isStackSkill = curCC.ControlCC(ccStack);

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
        }

        private void CrowdControlProgressTurn()
        {
            foreach (int key in _ccList.Keys)
            {
                if (_deadUnits.FindIndex(index => index == key) == -1) _ccList[key].Clear();

                for (int i = 0; i < _ccList[key].Count; i++)
                {
                    CrowdControl curCC = _ccList[key][i];
                    bool isStackSkill = curCC.ControlCC(0);

                    if (isStackSkill && curCC.stack > 0 && curCC.turn == curCC.GetCCBasicTurn()) continue;

                    bool b = curCC.ProgressTurn();
                    
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
                    if (_ccList[index][i].target == CCTarget.ENEMY || _ccList[index][i].target == CCTarget.ALL)
                    {
                        _ccList[index].RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    if (_ccList[index][i].target == CCTarget.SELF)
                    {
                        _ccList[index].RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #endregion

    }
}
