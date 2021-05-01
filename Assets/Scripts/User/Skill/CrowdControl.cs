using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum CCtype { TAUNT = 1, BARRIER, REFLECT, PURITY, INVINCIBLE, BLIND, STUN, DOTDAMAGE, ATTACKSTAT, MIRRORIMAGE, }
    public class CrowdControl
    {
        public int id { get; private set; }
        public CCtype cc { get; private set; }
        public int stack { get; private set; }
        public string name { get; private set; }
        public int turn { get; private set; }
        // public string name { get; private set; }

        public CrowdControl(int id, int stack = 0)
        {
            this.id = id;
            this.cc = (CCtype)(this.id % 100);
            this.stack = stack;
            this.turn = 0;
        }
        public CrowdControl(CrowdControl crowdControl, string name)
        {
            this.id = crowdControl.id;
            this.cc = crowdControl.cc;
            this.stack = crowdControl.stack;
            this.name = name;
            SetCCTurn();
        }

        public bool AddStack(int stack)
        {
            this.stack -= stack;
            if (this.stack == 0) return true;
            return false;
        }

        private void SetCCTurn()
        {
            switch(cc)
            {
                case CCtype.TAUNT:
                    this.turn = 2;
                    break;
                case CCtype.BARRIER:
                    this.turn = 1;
                    break;
                case CCtype.REFLECT:
                    this.turn = 1;
                    break;
                case CCtype.PURITY:
                    this.turn = 1;
                    break;
                case CCtype.INVINCIBLE:
                    this.turn = 1;
                    break;
                case CCtype.BLIND:
                    this.turn = 1;
                    break;
                case CCtype.STUN:
                    this.turn = 1;
                    break;
                case CCtype.DOTDAMAGE:
                    this.turn = 3;
                    break;
                case CCtype.ATTACKSTAT:
                    this.turn = 1;
                    break;
                case CCtype.MIRRORIMAGE:
                    this.turn = 1;
                    break;

            }
        }
    }
}
