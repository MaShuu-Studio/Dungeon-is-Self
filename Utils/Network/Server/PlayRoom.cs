using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Server
{
    public enum GameProgress { ReadyGame = 0, ReadyRound, PlayRound };
    public enum UserType { Offender = 0, Defender };
    class PlayRoom
    {
        GameRoom _room;
        public int RoomId { get { return _roomId; } }

        readonly int _roomId;

        readonly int[] _playerId = new int[2];

        Offender offender;
        Defender defender;

        GameProgress currentProgress;

        bool[] _playerReady = new bool[2] { false, false };

        int round = 0;
        int turn = 0;

        ushort[] _winCount = new ushort[5] { 0, 0, 0, 0, 0 };

        public PlayRoom(int roomId, int[] playerId, GameRoom room)
        {
            _roomId = roomId;
            for (int i = 0; i < _playerId.Length; i++)
                _playerId[i] = playerId[i];

            currentProgress = GameProgress.ReadyGame;
            offender = new Offender();
            defender = new Defender();

            round = 0;

            _room = room;
        }
        public PlayRoom(int roomId, int playerId, UserType type, GameRoom room)
        {
            _roomId = roomId;

            for (int i = 0; i < _playerId.Length; i++)
                if ((UserType)i == type) _playerId[i] = playerId;
                else
                {
                    _playerId[i] = -1;
                    _playerReady[i] = true;
                }

            currentProgress = GameProgress.ReadyGame;
            offender = new Offender();
            defender = new Defender();
            round = 0;

            _room = room;
        }

        public int PlayerInRoom(int id)
        {
            for (int i = 0; i < _playerId.Length; i++)
                if (_playerId[i] == id) return i;
            return -1;
        }

        public int GetPlayerId(UserType type)
        {
            return _playerId[(ushort)type];
        }

        public void ReadyGameEnd(UserType type, List<int> candidates)
        {
            _playerReady[(ushort)type] = true;
            if (type == UserType.Defender)
            {
                defender.SetCandidate(candidates);
                if (_playerId[(ushort)UserType.Offender] == -1)
                    Bot.SetCandidate(ref offender);
            }
            else
            {
                offender.SetCandidate(candidates);
                if (_playerId[(ushort)UserType.Defender] == -1)
                    Bot.SetCandidate(ref defender);
            }

            if (_playerReady[(ushort)UserType.Defender] && _playerReady[(ushort)UserType.Offender])
            {
                currentProgress = GameProgress.ReadyRound;
                S_ReadyGameEnd packet = new S_ReadyGameEnd();
                packet.currentProgress = (ushort)currentProgress;
                packet.round = ++round;
                packet.enemyCandidates = defender.Candidates;

                if (_playerId[(ushort)UserType.Offender] != -1)
                    _room.Send(_playerId[(ushort)UserType.Offender], packet);

                packet.enemyCandidates = offender.Candidates;

                if (_playerId[(ushort)UserType.Defender] != -1)
                    _room.Send(_playerId[(ushort)UserType.Defender], packet);

                for (int i = 0; i < _playerReady.Length; i++)
                    if (_playerId[i] == -1) _playerReady[i] = true;
                    else _playerReady[i] = false;
            }
        }
        public void RoundReadyEnd(UserType type, List<C_RoundReady.Roster> rosters)
        {
            List<int> units = new List<int>();
            List<List<int>> skills = new List<List<int>>();
            List<int> attackSkills = new List<int>();
            for (int i = 0; i < rosters.Count; i++)
            {
                units.Add(rosters[i].unitIndex);
                skills.Add(rosters[i].skillRosters);
                attackSkills.Add(rosters[i].attackSkill);
            }

            _playerReady[(ushort)type] = true;
            if (type == UserType.Defender)
            {
                defender.SetRoster(units, skills, attackSkills, round);
                if (_playerId[(ushort)UserType.Offender] == -1)
                    Bot.SetRoster(ref offender);
            }
            else
            {
                offender.SetRoster(units, skills);
                if (_playerId[(ushort)UserType.Defender] == -1)
                    Bot.SetRoster(ref defender, round);
            }

            if (_playerReady[(ushort)UserType.Defender] && _playerReady[(ushort)UserType.Offender])
            {
                Console.WriteLine("Send Round Ready End");
                currentProgress = GameProgress.PlayRound;
                S_RoundReadyEnd p = new S_RoundReadyEnd();
                p.currentProgress = (ushort)currentProgress;
                p.round = round;

                p.enemyRosters = new List<S_RoundReadyEnd.EnemyRoster>();

                if (_playerId[(ushort)UserType.Offender] != -1)
                {
                    for (int i = 0; i < defender.Rosters.Count; i++)
                    {
                        S_RoundReadyEnd.EnemyRoster enemy = new S_RoundReadyEnd.EnemyRoster();
                        enemy.unitIndex = defender.Rosters[i];
                        enemy.skillRosters = defender.SkillRosters[i];
                        p.enemyRosters.Add(enemy);
                    }
                    Console.WriteLine($"Defender {p.enemyRosters.Count}");

                    _room.Send(_playerId[(ushort)UserType.Offender], p);
                }

                p.enemyRosters.Clear();
                if (_playerId[(ushort)UserType.Defender] != -1)
                {
                    for (int i = 0; i < offender.Rosters.Count; i++)
                    {
                        S_RoundReadyEnd.EnemyRoster enemy = new S_RoundReadyEnd.EnemyRoster();
                        enemy.unitIndex = offender.Rosters[i];
                        enemy.skillRosters = offender.SkillRosters[i];
                        p.enemyRosters.Add(enemy);
                    }

                    Console.WriteLine($"Defender {p.enemyRosters.Count}");
                    _room.Send(_playerId[(ushort)UserType.Defender], p);
                }

                for (int i = 0; i < _playerReady.Length; i++)
                    if (_playerId[i] == -1) _playerReady[i] = true;
                    else _playerReady[i] = false;
            }
        }
        public void PlayRoundReadyEnd(UserType type, List<C_PlayRoundReady.Roster> rosters)
        {
            List<List<int>> dices = new List<List<int>>();
            for (int i = 0; i < rosters.Count; i++)
            {
                dices.Add(rosters[i].diceIndexs);
            }

            _playerReady[(ushort)type] = true;
            if (type == UserType.Defender)
            {
                defender.SetDice(dices);
            }
            else
            {
                offender.SetDice(dices);
            }

            if (_playerReady[(ushort)UserType.Defender] && _playerReady[(ushort)UserType.Offender])
            {
                // 다이스 굴리기
                // 결과 조정
                // 결과 전송
                Dictionary<int, List<int>> diceLists = new Dictionary<int, List<int>>();
                Dictionary<int, List<int>> tmp;

                tmp = defender.DiceRoll();
                foreach (int key in tmp.Keys)
                    diceLists.Add(key, tmp[key]);

                tmp = offender.DiceRoll();
                foreach (int key in tmp.Keys)
                    diceLists.Add(key, tmp[key]);

                List<int> monsterHps = new List<int>();
                Dictionary<int, int> diceResults = new Dictionary<int, int>();

                bool isRoundEnd = Battle(diceLists, ref monsterHps, ref diceResults);
                int isGameEnd = IsGameEnd();

                S_ProgressTurn p = new S_ProgressTurn();
                p.isRoundEnd = isRoundEnd;
                p.winner = _winCount[round - 1];
                p.round = round;
                p.turn = turn;
                p.monsterHps = monsterHps;

                p.results = new List<S_ProgressTurn.Result>();
                foreach (int key in diceResults.Keys)
                {
                    p.results.Add(new S_ProgressTurn.Result()
                    {
                        unitIndex = key,
                        diceResult = diceResults[key],
                        diceIndexs = diceLists[key],
                    });
                }

                for (int i = 0; i < _playerId.Length; i++) 
                    if (_playerId[i] != -1) _room.Send(_playerId[i], p);

                for (int i = 0; i < _playerReady.Length; i++)
                    if (_playerId[i] == -1) _playerReady[i] = true;
                    else _playerReady[i] = false;
            }
        }

        private bool Battle(Dictionary<int, List<int>> dices, ref List<int> monsterHps, ref Dictionary<int, int> diceResults)
        {
            turn++;

            monsterHps.Clear();
            foreach (int key in dices.Keys)
            {
                int selectedDice = SelectDice(dices[key]);
                if (key / 10 == 2)
                {
                    // Defender
                }
                else
                {
                    // Offender
                    defender.Damaged(SkillDatabase.Instance.GetCharacterSkill(selectedDice).damage);
                }
                monsterHps.Add(defender.MonsterHp);
                diceResults.Add(key, selectedDice);
            }
            if (defender.MonsterHp <= 0)
            {
                round++;
                return true;
            }
            else
            {
                // 오펜더가 졌을 경우 else if문
            }
            return false;
        }

        private int SelectDice(List<int> dices)
        {
            // 행동불능 상태에서는 -1
            int diceId = -1;
            Dictionary<int, int> diceAmount = new Dictionary<int, int>();

            foreach (int dice in dices)
            {
                if (diceAmount.ContainsKey(dice)) diceAmount[dice]++;
                else diceAmount.Add(dice, 0);
            }
            int max = 0;
            List<int> maxKeys = new List<int>();

            foreach (int key in diceAmount.Keys)
            {
                int amount = diceAmount[key];
                if (max < amount)
                {
                    maxKeys.Clear();
                    max = amount;
                    maxKeys.Add(key);
                }
                else if (max == amount)
                {
                    maxKeys.Add(key);
                }
            }

            if (maxKeys.Count == 1)
            {
                diceId = maxKeys[0];
            }
            else if (maxKeys.Count == 2)
            {
                Random rand = new Random();
                diceId = maxKeys[rand.Next(0, maxKeys.Count)];
            }
            else
            {
                //나중에 특수효과같은거
                Random rand = new Random();
                diceId = maxKeys[rand.Next(0, maxKeys.Count)];
            }

            return diceId;
        }

        private int IsGameEnd()
        {
            int defWinCount = 0;
            int offWinCount = 0;

            foreach (int winner in _winCount)
            {
                if (winner == (ushort)UserType.Defender) defWinCount++;
                else if (winner == (ushort)UserType.Offender) offWinCount++;
            }

            if (defWinCount >= 3) return (ushort)UserType.Defender;
            if (offWinCount >= 3) return (ushort)UserType.Offender;
            return 0;
        }

    }
}

