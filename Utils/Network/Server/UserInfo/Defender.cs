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
        List<int> _isDead = new List<int>();

        Monster _monster = null;
        MonsterSkill _attackSkill = null;
        int _attackSkillTurn;

        public List<int> Candidates { get { return _candidates; } }
        public List<int> Rosters { get { return _rosters; } }
        public List<List<int>> SkillRosters { get { return _skillRosters; } }
        public List<List<int>> Dices { get { return _dices; } }
        public MonsterSkill AttackSkill { get { return _attackSkill; } }
        public int MonsterHp { get { return _monster.hp; } }

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
            for (int i = 0; i < rosters.Count; i++)
            {
                _rosters.Add(rosters[i] + 20);
                _monster = MonsterDatabase.Instance.GetMonster(_candidates[_rosters[i]/10]);
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
                for (int j = 0; j < 3; j++)
                {
                    int diceIndex = rand.Next(0, _dices[i].Count);

                    dices.Add(SkillRosters[i][diceIndex]);
                }
                diceResults.Add(_rosters[i], dices);
            }

            return diceResults;
        }

        public void Damaged(int damage)
        {
            _monster.Damaged(damage);
        }

        public void ProgressTurn()
        {
            _attackSkillTurn--;
        }
    }
}
