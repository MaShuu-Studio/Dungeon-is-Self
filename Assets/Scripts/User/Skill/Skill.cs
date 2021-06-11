using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class Skill
    {
        public int id { get; protected set; }
        public string name { get; protected set; }
        public int turn { get; protected set; }
        public int tier { get; protected set; }
        public string description { get; protected set; }
        public Dictionary<CrowdControl, int> ccList { get; protected set; } = new Dictionary<CrowdControl, int>();
        public bool move { get; protected set; }

    }
}
