using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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

        Timer timer = null;
        int time = 0;

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

            StartTimer(30);
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
            StartTimer(30);
        }

        public void DestroyRoom()
        {
            StopTimer();
        }
        private void StartTimer(int t)
        {
            StopTimer();
            time = t;
            timer = new Timer();
            timer.Interval = 1000; // 1초
            timer.Elapsed += new ElapsedEventHandler(SendTimer);
            timer.Start();
        }

        private void StopTimer()
        {
            if (timer != null) timer.Stop();
        }

        private void SendTimer(object sender, EventArgs e)
        {
            S_Timeout packet = new S_Timeout()
            {
                time = time,
                currentProgress = (ushort)currentProgress
            };

            for (int i = 0; i < _playerId.Length; i++)
                if (_playerId[i] != -1) _room.Send(_playerId[i], packet);

            time--;
            if (time < 0) time = 0;
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

        public void ReadyRoundState(UserType type, List<int> candidates = null)
        {
            _playerReady[(ushort)type] = true;
            if (candidates != null)
            {
                if (currentProgress != GameProgress.ReadyGame) return;

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

                    StartTimer(60);
                }
            }
            else if (_playerReady[(ushort)UserType.Defender] && _playerReady[(ushort)UserType.Offender])
            {
                currentProgress = GameProgress.ReadyRound;
                S_NewRound packet = new S_NewRound();
                packet.round = ++round;
                packet.userInfos = new List<S_NewRound.UserInfo>();
                
                for (int i = 0; i < _playerId.Count(); i++)
                {
                    packet.userInfos.Add(new S_NewRound.UserInfo()
                    {
                        type = (ushort)i,
                        deadUnits = (i == (int)UserType.Defender) ? defender.DeadUnits : offender.DeadUnits,
                    });
                }

                if (_playerId[(ushort)UserType.Offender] != -1)
                    _room.Send(_playerId[(ushort)UserType.Offender], packet);
                if (_playerId[(ushort)UserType.Defender] != -1)
                    _room.Send(_playerId[(ushort)UserType.Defender], packet);

                for (int i = 0; i < _playerReady.Length; i++)
                    if (_playerId[i] == -1) _playerReady[i] = true;
                    else _playerReady[i] = false;
            }
        }

        public void RoundReadyEnd(UserType type, List<C_RoundReady.Roster> rosters)
        {
            if (currentProgress != GameProgress.ReadyRound) return;

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
                currentProgress = GameProgress.PlayRound;
                S_RoundReadyEnd p = new S_RoundReadyEnd();
                p.currentProgress = (ushort)currentProgress;
                p.round = round;
                turn = 1;

                p.enemyRosters = new List<S_RoundReadyEnd.EnemyRoster>();

                if (_playerId[(ushort)UserType.Offender] != -1)
                {
                    for (int i = 0; i < defender.Rosters.Count; i++)
                    {
                        S_RoundReadyEnd.EnemyRoster enemy = new S_RoundReadyEnd.EnemyRoster();
                        enemy.unitIndex = defender.Rosters[i];
                        enemy.skillRosters = defender.SkillRosters[i];
                        enemy.attackSkill = defender.AttackSkill.id;
                        p.enemyRosters.Add(enemy);
                    }

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

                    _room.Send(_playerId[(ushort)UserType.Defender], p);
                }

                for (int i = 0; i < _playerReady.Length; i++)
                    if (_playerId[i] == -1) _playerReady[i] = true;
                    else _playerReady[i] = false;

                StartTimer(30);
            }
        }
        public void PlayRoundReadyEnd(UserType type, List<C_PlayRoundReady.Roster> rosters)
        {
            if (currentProgress != GameProgress.PlayRound) return;

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
                List<Dictionary<int, List<CrowdControl>>> ccResultWithTurn
                    = new List<Dictionary<int, List<CrowdControl>>>();
                Dictionary<int, Tuple<bool, int>> deadUnit = new Dictionary<int, Tuple<bool, int>>();

                foreach (int unit in diceLists.Keys)
                {
                    if (unit / 10 == 1) deadUnit.Add(unit, new Tuple<bool, int>(offender.IsDead(unit), 0));
                    else deadUnit.Add(unit, new Tuple<bool, int>(false, 0));
                }


                int endTurn = Battle(diceLists, ref monsterHps, ref diceResults, ref ccResultWithTurn, ref deadUnit);
                int isGameEnd = IsGameEnd();

                S_ProgressTurn p = new S_ProgressTurn();
                p.isGameEnd = (isGameEnd != 0);
                p.winner = _winCount[round - 1];
                p.endTurn = endTurn;
                p.round = round;
                p.turn = turn;
                p.monsterHps = monsterHps;
                p.monsterTurn = defender.MonsterAttackTurn;
                if (defender.MonsterAttackTurn <= 0) defender.ResetAttackTurn();

                p.resetTurn = defender.MonsterAttackTurn;
                p.results = new List<S_ProgressTurn.Result>();
                foreach (int key in diceResults.Keys)
                {
                    List<S_ProgressTurn.Result.CcWithTurn> ccsWithTurn = new List<S_ProgressTurn.Result.CcWithTurn>();

                    for (int i = 0; i < ccResultWithTurn.Count; i++)
                    {
                        List<S_ProgressTurn.Result.CcWithTurn.Cc> ccs = new List<S_ProgressTurn.Result.CcWithTurn.Cc>();

                        for (int j = 0; j < ccResultWithTurn[i][key].Count; j++)
                        {
                            ccs.Add(new S_ProgressTurn.Result.CcWithTurn.Cc()
                            {
                                ccid = ccResultWithTurn[i][key][j].id,
                                ccstack = ccResultWithTurn[i][key][j].stack,
                                ccturn = ccResultWithTurn[i][key][j].turn
                            });
                        }

                        ccsWithTurn.Add(new S_ProgressTurn.Result.CcWithTurn()
                        {
                            ccs = ccs
                        });
                    }

                    S_ProgressTurn.Result result = new S_ProgressTurn.Result()
                    {
                        unitIndex = key,
                        isDead = deadUnit[key].Item1,
                        deadTurn = deadUnit[key].Item2,
                        diceResult = diceResults[key],
                        diceIndexs = diceLists[key],
                        ccWithTurns = ccsWithTurn
                    };

                    p.results.Add(result);
                }

                for (int i = 0; i < _playerId.Length; i++)
                    if (_playerId[i] != -1) _room.Send(_playerId[i], p);

                for (int i = 0; i < _playerReady.Length; i++)
                    if (_playerId[i] == -1) _playerReady[i] = true;
                    else _playerReady[i] = false;

                StartTimer(40);
            }
        }

        public void ReadyCancel(UserType type)
        {
            _playerReady[(ushort)type] = false;
        }

        private int Battle(Dictionary<int, List<int>> dices,
            ref List<int> monsterHps, ref Dictionary<int, int> diceResults,
            ref List<Dictionary<int, List<CrowdControl>>> ccResultsWithTurn,
            ref Dictionary<int, Tuple<bool, int>> deadUnit)
        {
            turn++;

            monsterHps.Clear();
            List<int> units = dices.Keys.ToList();

            int i = 0;
            for (; i <= units.Count; i++)
            {
                if (i < units.Count)
                {
                    if (deadUnit[units[i]].Item1 == false)
                    {
                        int selectedDice = SelectDice(dices[units[i]]);
                        int target;
                        int usingUnit = units[i];
                        if (units[i] / 10 == 2)
                        {
                            // Defender
                            MonsterSkill skill = SkillDatabase.Instance.GetMonsterSkill(selectedDice);

                            List<int> alives = offender.GetAlives();

                            Random rand = new Random();
                            int index = rand.Next(0, alives.Count);

                            target = alives[index];

                            foreach (int unit in offender.Rosters)
                            {
                                if (offender.HasCrowdControl(unit, CCType.TAUNT)) target = unit;
                            }

                            AddCrowdControl(skill, usingUnit, target);
                        }
                        else
                        {
                            // Offender
                            CharacterSkill skill = SkillDatabase.Instance.GetCharacterSkill(selectedDice);

                            int damage = skill.damage;
                            target = defender.Rosters[0];

                            if (offender.HasCrowdControl(units[i], CCType.ATTACKSTAT, CCTarget.SELF)) damage = (int)(damage * 1.5f);
                            if (offender.HasCrowdControl(units[i], CCType.MIRRORIMAGE)) damage = (int)(damage * 1.5f);
                            if (offender.HasCrowdControl(units[i], CCType.ATTACKSTAT, CCTarget.ENEMY)) damage = (int)(damage * 0.7f);

                            if (offender.HasCrowdControl(units[i], CCType.CONFUSION))
                            {
                                List<int> alives = offender.GetAlives();

                                Random rand = new Random();
                                int index = rand.Next(0, alives.Count + 1);
                                if (index != alives.Count)
                                {
                                    foreach (int unit in offender.Rosters)
                                    {
                                        if (offender.HasCrowdControl(unit, CCType.TAUNT)) target = unit;
                                    }
                                }
                            }

                            if (target / 10 == 2) defender.Damaged(damage);

                            AddCrowdControl(skill, usingUnit, target);
                        }
                        diceResults.Add(units[i], selectedDice);
                    }
                    else diceResults.Add(units[i], -1);
                }
                else
                {
                    defender.ProgressTurn();
                    offender.ProgressTurn();
                }

                if (defender.MonsterAttackTurn <= 0)
                {
                    // 죽을 유닛 선정
                    List<int> alives = offender.GetAlives();
                    Random rand = new Random();
                    int target = rand.Next(0, alives.Count);

                    for (int j = 0; j < alives.Count; j++)
                    {
                        if (offender.HasCrowdControl(alives[j], CCType.TAUNT))
                            target = j;
                    }

                    switch (defender.AttackSkill.type)
                    {
                        case MonsterSkill.SkillType.AttackAll:
                            for (int j = 0; j < alives.Count; j++)
                                offender.KillUnit(alives[j]);
                            break;

                        case MonsterSkill.SkillType.AttackOne:
                            offender.KillUnit(alives[target]);
                            break;
                        case MonsterSkill.SkillType.AttackOneStun:
                            offender.KillUnit(alives[target]);
                            alives = offender.GetAlives();

                            target = rand.Next(0, alives.Count);

                            for (int j = 0; j < alives.Count; j++)
                            {
                                if (offender.HasCrowdControl(alives[j], CCType.TAUNT))
                                    target = j;
                            }

                            List<CrowdControl> ccs = defender.AttackSkill.ccList.Keys.ToList();
                            offender.AddCrowdControl(alives[target], ccs[0], defender.AttackSkill.ccList[ccs[0]], defender.Rosters[0], 1, defender);

                            break;
                    }
                }

                monsterHps.Add(defender.MonsterHp);
                ccResultsWithTurn.Add(new Dictionary<int, List<CrowdControl>>());
                for (int j = 0; j < units.Count; j++)
                {
                    if (units[j] / 10 == 1) ccResultsWithTurn[i].Add(units[j], offender.GetCCList(units[j]));
                    else ccResultsWithTurn[i].Add(units[j], defender.GetCCList(units[j]));
                }

                for (int j = 0; j < units.Count; j++)
                {
                    if (units[j] / 10 == 1)
                    {
                        if (deadUnit[units[j]].Item1 == false && offender.IsDead(units[j]))
                            deadUnit[units[j]] = new Tuple<bool, int>(true, i);
                    }
                    else
                    {
                        if (deadUnit[units[j]].Item1 == false && defender.IsDead())
                            deadUnit[units[j]] = new Tuple<bool, int>(true, i);
                    }
                }

                if (RoundEnd()) break;
            }

            return i;
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

        private void AddCrowdControl(Skill skill, int usingUnit, int target)
        {
            if (skill.ccList.Count != 0)
            {
                foreach (CrowdControl cc in skill.ccList.Keys)
                {
                    // Offender CC 발동
                    if (usingUnit / 10 == 1)
                    {
                        int ccMultiplier = (offender.HasCrowdControl(usingUnit, CCType.MIRRORIMAGE)) ? 2 : 1;
                        if (cc.target == CCTarget.ENEMY)
                        {
                            // 몬스터에 쓰는 스킬이 타겟이 바뀜
                            if (target / 10 != 2)
                            {
                                offender.AddCrowdControl(target, cc, skill.ccList[cc], usingUnit, ccMultiplier, defender);
                            }
                            else
                            {
                                defender.AddCrowdControl(cc, skill.ccList[cc], usingUnit, ccMultiplier, offender);
                            }

                        }
                        else if (cc.target == CCTarget.SELF)
                            offender.AddCrowdControl(usingUnit, cc, skill.ccList[cc], usingUnit, ccMultiplier, defender);
                        else
                            offender.AddCrowdControl(usingUnit, cc, skill.ccList[cc], usingUnit, ccMultiplier, defender, true);
                    }
                    else
                    {
                        int ccMultiplier = (defender.HasCrowdControl(usingUnit, CCType.MIRRORIMAGE)) ? 2 : 1;
                        if (cc.target == CCTarget.ENEMY)
                        {
                            // 몬스터에 쓰는 스킬이 타겟이 바뀜
                            if (target / 10 != 2)
                            {
                                offender.AddCrowdControl(target, cc, skill.ccList[cc], usingUnit, ccMultiplier, defender);
                            }
                        }
                        else if (cc.target == CCTarget.SELF)
                            defender.AddCrowdControl(cc, skill.ccList[cc], usingUnit, ccMultiplier, offender);
                        else
                            offender.AddCrowdControl(target, cc, skill.ccList[cc], usingUnit, ccMultiplier, defender, true);
                    }
                }
            }
        }

        private bool RoundEnd()
        {
            if (defender.MonsterHp <= 0)
            {
                // 방어자 패배
                _winCount[round - 1] = (ushort)UserType.Offender;
                offender.NextRound(true);
                return true;
            }
            else if (offender.GetAlives().Count == 0)
            {
                // 공격자 패배
                _winCount[round - 1] = (ushort)UserType.Defender;
                offender.NextRound(false);
                return true;
            }

            return false;
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

