using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Server
{
    static class Bot
    {
        public static void SetCandidate(ref Offender user)
        {
            List<int> units = new List<int>();
            CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref units);

            Random rand = new Random();
            List<int> candidates = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                int index = rand.Next(0, units.Count);
                candidates.Add(units[index]);
            }

            user.SetCandidate(candidates);
        }

        public static void SetCandidate(ref Defender user)
        {
            List<int> units = new List<int>();
            MonsterDatabase.Instance.GetAllMonsterCandidatesList(ref units);

            Random rand = new Random();
            List<int> candidates = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                int index = rand.Next(0, units.Count);
                candidates.Add(units[index]);
            }

            user.SetCandidate(candidates);
        }

        public static void SetRoster(ref Offender user)
        {
            List<int> roster = new List<int>(user.Candidates);
            List<List<int>> skillRoster = new List<List<int>>();
            Random rand = new Random();
            while (roster.Count > 3)
            {
                int index = rand.Next(0, roster.Count);
                roster.RemoveAt(index);
            }

            user.SetRoster(roster, skillRoster);
        }
        public static void SetRoster(ref Defender user, int round)
        {
            List<int> roster = new List<int>(user.Candidates);
            List<List<int>> skillRoster = new List<List<int>>();
            List<int> attackSkill = new List<int>();
            Random rand = new Random();
            while (roster.Count > 1)
            {
                int index = rand.Next(0, roster.Count);
                roster.RemoveAt(index);
            }

            user.SetRoster(roster, skillRoster, attackSkill, round);
        }

        /*
        public static void SetDice(ref Offender user)
        {
           List<List<int>> diceIndexes = new List<List<int>>();

            for (int i = 0; i < user.Rosters.Count; i++) {
                int charId = user.Candidates[user.Rosters[i]%10];
                for (int j = 0; j < 6; j++)
                {
                    if (j < 2)
                    {
                        diceIndexes[i].Add(SkillDatabase.Instance.GetCharacterBasicSkill(charId).id);
                    }
                }
            }
        }*/
    }
}
