using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum CCType { TAUNT = 1, BARRIER, REFLECT, PURITY, INVINCIBLE, BLIND, STUN, DOTDAMAGE, ATTACKSTAT, MIRRORIMAGE, DRAIN, DECREASETURN, CONFUSION, REMOVE }

    public enum CCTarget { ENEMY, SELF, ALL }
    public class CrowdControl
    {
        public int id { get; private set; }
        public CCType cc { get; private set; }
        public CCTarget target { get; private set; }
        public int stack { get; private set; }
        public string name { get; private set; }
        public int turn { get; private set; }
        public int dotDamage { get; private set; }
        // public string name { get; private set; }

        public CrowdControl(int id, string name, CCTarget target)
        {
            this.id = id;
            this.name = name;
            this.cc = (CCType)((id / 100) % 100);
            this.stack = GetCCBasicStack();
            this.target = target;
            this.turn = GetCCBasicTurn();
            this.dotDamage = 0;
        }
        public CrowdControl(CrowdControl crowdControl)
        {
            this.id = crowdControl.id;
            this.cc = crowdControl.cc;
            this.stack = crowdControl.stack;
            this.name = crowdControl.name;
            this.target = crowdControl.target;
            this.turn = crowdControl.turn;
            this.dotDamage = crowdControl.dotDamage;
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
                case CCType.BLIND:
                case CCType.STUN:
                    this.stack -= stack;
                    isStackSkill = true;
                    break;
            }
            return isStackSkill;
        }

        public void SetDotDamage(int damage)
        {
            dotDamage = damage;
        }
        public void SetTurn(int turn)
        {
            this.turn = turn;
        }

        public void SetStack(int stack)
        {
            this.stack = stack;
        }

        public bool IsStackCC()
        {
            bool isStackSkill = false;

            switch (cc)
            {
                case CCType.BLIND:
                case CCType.STUN:
                    isStackSkill = true;
                    break;
            }

            return isStackSkill;
        }

        public int GetCCBasicTurn()
        {
            int turn = 0;
            switch(cc)
            {
                case CCType.TAUNT:
                    turn = 2;
                    break;
                case CCType.BARRIER:
                    turn = 1;
                    break;
                case CCType.REFLECT:
                    turn = 1;
                    break;
                case CCType.PURITY:
                    turn = 0;
                    break;
                case CCType.INVINCIBLE:
                    turn = 1;
                    break;
                case CCType.BLIND:
                    turn = 1;
                    break;
                case CCType.STUN:
                    turn = 1;
                    break;
                case CCType.DOTDAMAGE:
                    turn = 3;
                    break;
                case CCType.ATTACKSTAT:
                    turn = 1;
                    break;
                case CCType.MIRRORIMAGE:
                    turn = 1;
                    break;
                case CCType.DRAIN:
                    turn = 0;
                    break;
                case CCType.DECREASETURN:
                    turn = 0;
                    break;
                case CCType.CONFUSION:
                    turn = 1;
                    break;
                case CCType.REMOVE:
                    turn = 0;
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
                case CCType.BLIND:
                case CCType.STUN:
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
