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
            user.SetCharacters();
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
            user.SetMonsters();
        }

        public static void SetRoster(ref Offender user, int round)
        {
            int[] skillPoints = new int[5] { 1, 2, 2, 0, 0 };
            List<int> roster = new List<int>() { 0, 1, 2, 3, 4, 5 };
            List<List<int>> skillRoster;

            Random rand = new Random();

            for (int i = 0; i < user.DeadUnits.Count; i++)
            {
                roster.Remove(user.DeadUnits[i] % 10);
            }

            while (roster.Count > 3)
            {
                int index = rand.Next(0, roster.Count);
                roster.RemoveAt(index);
            }

            user.AddSkillPoint(skillPoints[round - 1]);
            SetSkillRoster(ref user, roster, out skillRoster);
            user.SetRoster(roster, skillRoster);
        }
        public static void SetRoster(ref Defender user, int round)
        {
            List<int> roster = new List<int> { 0, 1, 2, 3, 4, 5 };
            List<List<int>> skillRoster;
            List<int> attackSkill;

            for (int i = 0; i < user.DeadUnits.Count; i++)
            {
                roster.Remove(user.DeadUnits[i] % 10);
            }

            Random rand = new Random();
            while (roster.Count > 1)
            {
                int index = rand.Next(0, roster.Count);
                roster.RemoveAt(index);
            }
            SetSkillRoster(ref user, roster, round, out skillRoster, out attackSkill);
            user.SetRoster(roster, skillRoster, attackSkill, round);
        }

        private static void SetSkillRoster(ref Defender user, List<int> roster, int round, out List<List<int>> skillRoster, out List<int> attackSkill)
        {
            skillRoster = new List<List<int>>();
            attackSkill = new List<int>();
            for (int i = 0; i < roster.Count; i++)
            {
                int monIndex = roster[i];
                Monster m = user.Monsters[monIndex];
                List<MonsterSkill> usableSkill = user.GetUsableSkill(monIndex, round);

                List<int> unitSkillRosters = new List<int>();

                Random rand = new Random();
                int n;

                for (int j = 0; j < 8; j++)
                {
                    do
                    {
                        n = rand.Next(0, usableSkill.Count);
                        if(CheckSkillRosterCondition(unitSkillRosters, usableSkill[n].id, UserType.Defender))
                            unitSkillRosters.Add(usableSkill[n].id);
                    } while (unitSkillRosters.Count <= j);
                }
                skillRoster.Add(unitSkillRosters);

                List<MonsterSkill> attackSkills = SkillDatabase.Instance.GetMonsterAttackSkills(m.id, round);
                n = rand.Next(0, attackSkills.Count);
                attackSkill.Add(attackSkills[n].id);
            }
        }

        private static void SetSkillRoster(ref Offender user, List<int> roster, out List<List<int>> skillRoster)
        {
            skillRoster = new List<List<int>>();
            for (int i = 0; i < roster.Count; i++)
            {
                Random rand = new Random();
                int charIndex = roster[i];
                Character c = user.Characters[charIndex];

                List<int> upgradableSkill = new List<int>();
                while (user.SkillPoints[charIndex] > 0)
                {
                    upgradableSkill = user.GetUpgradableSkill(charIndex);
                    int index = rand.Next(0, upgradableSkill.Count);
                    CharacterSkill skill = c.mySkills.Find(skill => skill.id == upgradableSkill[index]);
                    if (skill == null) continue;
                    user.LearnSkill(charIndex, skill);
                }

                List<int> usableSkill = user.UsableSkills[charIndex];
                List<int> unitSkillRosters = new List<int>();

                for (int j = 0; j < 8; j++)
                {
                    int n;
                    do
                    {
                        n = rand.Next(0, usableSkill.Count);
                        if (CheckSkillRosterCondition(unitSkillRosters, usableSkill[n], UserType.Offender))
                            unitSkillRosters.Add(usableSkill[n]);
                    } while (unitSkillRosters.Count <= j);
                }

                skillRoster.Add(unitSkillRosters);
            }
        }

        private static bool CheckSkillRosterCondition(List<int> list, int id, UserType type)
        {
            bool b = false;
            int count = 0;

            if (type == UserType.Defender)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == 200110) continue;
                    if (list[i] == id) count++;
                }

                if (count <= 1) b = true;
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] % 10 == 0) continue;
                    if (list[i] == id) count++;
                }
                if (count <= 2) b = true;
            }
            return b;
        }

        public static void SetDice(ref Offender user)
        {
            List<List<int>> diceIndexes = new List<List<int>>();

            for (int i = 0; i < user.Rosters.Count; i++)
            {
                diceIndexes.Add(new List<int>());
                Random rand = new Random();
                for (int j = 0; j < 6; j++)
                {
                    if (j < 2)
                    {
                        diceIndexes[i].Add(-1);
                    }
                    else
                    {
                        int index;
                        do
                        {
                            index = rand.Next(0, user.SkillRosters[i].Count);
                        } while (diceIndexes[i].Contains(index));
                        diceIndexes[i].Add(index);
                    }
                }
            }

            user.SetDice(diceIndexes);
        }
        public static void SetDice(ref Defender user)
        {
            List<List<int>> diceIndexes = new List<List<int>>();

            for (int i = 0; i < user.Rosters.Count; i++)
            {
                diceIndexes.Add(new List<int>());
                Random rand = new Random();
                for (int j = 0; j < 6; j++)
                {
                    int index;
                    do
                    {
                        index = rand.Next(0, user.SkillRosters[i].Count);
                    } while (diceIndexes[i].Contains(index));
                    diceIndexes[i].Add(index);
                }
            }

            user.SetDice(diceIndexes);
        }
    }
}
