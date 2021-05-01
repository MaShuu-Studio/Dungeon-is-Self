using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum CCtype { TAUNT = 1, BARRIER, REFLECT, PURITY, INVINCIBLE, BLIND, STUN, DOTDAMAGE, ATTACKSTAT, MIRRORIMAGE, }

    public enum CCTarget { ENEMY, SELF, ALL }
    public class CrowdControl
    {
        public int id { get; private set; }
        public CCtype cc { get; private set; }
        public CCTarget target { get; private set; }
        public int stack { get; private set; }
        public string name { get; private set; }
        public int turn { get; private set; }
        // public string name { get; private set; }

        public CrowdControl(int id, string name, CCTarget target)
        {
            this.id = id;
            this.name = name;
            this.cc = (CCtype)((id / 100) % 100);
            this.stack = GetCCBasicStack();
            this.target = target;
            this.turn = GetCCBasicTurn();
        }
        public CrowdControl(CrowdControl crowdControl)
        {
            this.id = crowdControl.id;
            this.cc = crowdControl.cc;
            this.stack = crowdControl.stack;
            this.name = crowdControl.name;
            this.target = crowdControl.target;
            this.turn = GetCCBasicTurn();
        }

        public bool ProgressTurn()
        {
            turn -= 1;
            if (turn <= 0) return true;
            return false;
        }

        public bool ControlCC(int stack)
        {
            bool isStackSkill = false;

            switch (cc)
            {
                case CCtype.BLIND:
                case CCtype.STUN:
                    this.stack -= stack;
                    isStackSkill = true;
                    break;
            }
            return isStackSkill;
        }
        
        public void SetTurn(int turn)
        {
            this.turn = turn;
        }

        public int GetCCBasicTurn()
        {
            int turn = 0;
            switch(cc)
            {
                case CCtype.TAUNT:
                    turn = 2;
                    break;
                case CCtype.BARRIER:
                    turn = 1;
                    break;
                case CCtype.REFLECT:
                    turn = 1;
                    break;
                case CCtype.PURITY:
                    turn = 1;
                    break;
                case CCtype.INVINCIBLE:
                    turn = 1;
                    break;
                case CCtype.BLIND:
                    turn = 1;
                    break;
                case CCtype.STUN:
                    turn = 1;
                    break;
                case CCtype.DOTDAMAGE:
                    turn = 3;
                    break;
                case CCtype.ATTACKSTAT:
                    turn = 1;
                    break;
                case CCtype.MIRRORIMAGE:
                    turn = 1;
                    break;
            }
            turn += 1;

            return turn;
        }

        public int GetCCBasicStack()
        {
            int stack = 0;
            switch (cc)
            {
                case CCtype.BLIND:
                case CCtype.STUN:
                    stack = 2;
                    break;
                default:
                    stack = 0;
                    break;
            }

            return stack;
        }

        public void ResetCCStack()
        {
            stack = GetCCBasicStack();
        }
    }
}
