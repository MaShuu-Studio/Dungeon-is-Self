using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Defender
    {
        List<int> _candidates = new List<int>();
        List<int> _rosters = new List<int>();
        List<List<int>> _skillRosters = new List<List<int>>();
        public List<int> Candidates { get { return _candidates; } }
        public List<int> Rosters { get { return _rosters; } }
        public List<List<int>> SkillRosters { get { return _skillRosters; } }

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
            for (int i = 0; i < rosters.Count; i++)
            {
                _rosters.Add(rosters[i] + 20);
                _skillRosters.Add(skillRosters[i]);
            }
        }
    }
}
