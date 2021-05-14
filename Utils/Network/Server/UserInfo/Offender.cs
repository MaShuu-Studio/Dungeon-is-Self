using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Offender
    {
        List<int> _candidates = new List<int>();
        public List<int> Candidates { get { return _candidates; } }

        public void SetCandidate(List<int> cs)
        {
            _candidates.Clear();
            for (int i = 0; i < cs.Count; i++)
            {
                _candidates.Add(cs[i]);
            }
        }
    }
}
