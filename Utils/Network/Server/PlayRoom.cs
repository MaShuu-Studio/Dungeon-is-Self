using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Data;

namespace Server
{
    public class BattleResult
    {
        public DateTime date;
        public int winner;
        public string gameNumber;
        public string[] players;
        public Dictionary<string, int[]> roster;
        public Dictionary<int, string> roundWinner;
        public Dictionary<int, Dictionary<string, int[]>> roundUnit;
    }

    public enum GameProgress { ReadyGame = 0, ReadyRound, PlayRound };
    public enum UserType { Offender = 0, Defender };
    class PlayRoom
    {
        GameRoom _room;
        public string RoomId { get { return _roomId; } }

        readonly string _roomId;

        readonly string[] _playerId = new string[2];

        Offender offender;
        Defender defender;

        GameProgress currentProgress;

        bool[] _playerReady = new bool[2] { false, false };

        int round = 0;
        int turn = 0;

        Timer timer = null;
        int time = 0;

        ushort[] _winCount = new ushort[5] { 9, 9, 9, 9, 9 };
        int winner = 9;
        Dictionary<int, Dictionary<string, int[]>> roundUnits;

        public PlayRoom(string roomId, string[] playerId, GameRoom room)
        {
            _roomId = roomId;
            for (int i = 0; i < _playerId.Length; i++)
                _playerId[i] = playerId[i];

            currentProgress = GameProgress.ReadyGame;
            offender = new Offender();
            defender = new Defender();

            round = 0;

            _room = room;
            roundUnits = new Dictionary<int, Dictionary<string, int[]>>();
            for (int i = 1; i <= 5; i++)
            {
                roundUnits.Add(i, new Dictionary<string, int[]>()
                {
                    { _playerId[0], new int[3] },
                    { _playerId[1], new int[1] },
                });
            }

            StartTimer(30);
        }
        public PlayRoom(string roomId, string playerId, UserType type, GameRoom room)
        {
            _roomId = roomId;

            for (int i = 0; i < _playerId.Length; i++)
                if ((UserType)i == type) _playerId[i] = playerId;
                else
                {
                    _playerId[i] = "";
                    _playerReady[i] = true;
                }

            currentProgress = GameProgress.ReadyGame;
            offender = new Offender();
            defender = new Defender();
            round = 0;

            _room = room;
            StartTimer(30);
        }
        public void SetAbnormalWinner(ushort winner)
        {
            this.winner = winner;
        }

        public void DestroyRoom()
        {
            StopTimer();
            GameResult();
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
                if (string.IsNullOrEmpty(_playerId[i]) == false) _room.Send(_playerId[i], packet);

            time--;
            if (time < 0) time = 0;
        }

        public int PlayerInRoom(string id)
        {
            for (int i = 0; i < _playerId.Length; i++)
                if (_playerId[i] == id) return i;
            return -1;
        }

        public string GetPlayerId(UserType type)
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
                    if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Offender]))
                        Bot.SetCandidate(ref offender);
                }
                else
                {
                    offender.SetCandidate(candidates);
                    if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Defender]))
                        Bot.SetCandidate(ref defender);
                }

                if (_playerReady[(ushort)UserType.Defender] && _playerReady[(ushort)UserType.Offender])
                {
                    currentProgress = GameProgress.ReadyRound;
                    S_ReadyGameEnd packet = new S_ReadyGameEnd();
                    packet.currentProgress = (ushort)currentProgress;
                    packet.round = ++round;
                    packet.enemyCandidates = defender.Candidates;

                    if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Offender]) == false)
                        _room.Send(_playerId[(ushort)UserType.Offender], packet);

                    packet.enemyCandidates = offender.Candidates;

                    if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Defender]) == false)
                        _room.Send(_playerId[(ushort)UserType.Defender], packet);

                    for (int i = 0; i < _playerReady.Length; i++)
                        if (string.IsNullOrEmpty(_playerId[i])) _playerReady[i] = true;
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

                if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Offender]) == false)
                    _room.Send(_playerId[(ushort)UserType.Offender], packet);
                if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Defender]) == false)
                    _room.Send(_playerId[(ushort)UserType.Defender], packet);

                for (int i = 0; i < _playerReady.Length; i++)
                    if (string.IsNullOrEmpty(_playerId[i])) _playerReady[i] = true;
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
                if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Offender]))
                    Bot.SetRoster(ref offender);
            }
            else
            {
                offender.SetRoster(units, skills);
                if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Defender]))
                    Bot.SetRoster(ref defender, round);
            }

            if (_playerReady[(ushort)UserType.Defender] && _playerReady[(ushort)UserType.Offender])
            {
                currentProgress = GameProgress.PlayRound;
                S_RoundReadyEnd p = new S_RoundReadyEnd();
                p.currentProgress = (ushort)currentProgress;
                p.round = round;
                turn = 1;

                roundUnits[round][_playerId[0]] = offender.Rosters.ToArray();
                roundUnits[round][_playerId[1]] = defender.Rosters.ToArray();

                p.enemyRosters = new List<S_RoundReadyEnd.EnemyRoster>();

                if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Offender]) == false)
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
                if (string.IsNullOrEmpty(_playerId[(ushort)UserType.Defender]) == false)
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
                    if (string.IsNullOrEmpty(_playerId[i])) _playerReady[i] = true;
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
                Dictionary<int, int> remainTurns = new Dictionary<int, int>();
                List<Dictionary<int, List<CrowdControl>>> ccResultWithTurn
                    = new List<Dictionary<int, List<CrowdControl>>>();
                Dictionary<int, Tuple<bool, int>> deadUnit = new Dictionary<int, Tuple<bool, int>>();

                foreach (int unit in diceLists.Keys)
                {
                    if (unit / 10 == 1) deadUnit.Add(unit, new Tuple<bool, int>(offender.IsDead(unit), 0));
                    else deadUnit.Add(unit, new Tuple<bool, int>(false, 0));
                }

                int endTurn = Battle(diceLists, ref monsterHps, ref diceResults, ref remainTurns, ref ccResultWithTurn, ref deadUnit);
                ushort isGameEnd = IsGameEnd();
                if (isGameEnd != 0) winner = (ushort)isGameEnd;

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
                                ccturn = ccResultWithTurn[i][key][j].turn,
                                ccdotdmg = ccResultWithTurn[i][key][j].dotDamage
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
                        remainTurn = remainTurns[key],
                        diceIndexs = diceLists[key],
                        ccWithTurns = ccsWithTurn
                    };

                    p.results.Add(result);
                }

                for (int i = 0; i < _playerId.Length; i++)
                    if (string.IsNullOrEmpty(_playerId[i]) == false) _room.Send(_playerId[i], p);

                for (int i = 0; i < _playerReady.Length; i++)
                    if (string.IsNullOrEmpty(_playerId[i])) _playerReady[i] = true;
                    else _playerReady[i] = false;

                StartTimer(40);
            }
        }

        public void ReadyCancel(UserType type)
        {
            _playerReady[(ushort)type] = false;
        }

        private int Battle(Dictionary<int, List<int>> dices,
            ref List<int> monsterHps, ref Dictionary<int, int> diceResults, ref Dictionary<int, int> remainTurns,
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
                        int remainTurn = 0;
                        if (units[i] / 10 == 2)
                        {
                            if (selectedDice != -1)
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
                        }
                        else
                        {
                            // Offender
                            CharacterSkill skill = null;
                            bool isActive = false;
                            if (selectedDice != -1)
                            {
                                skill = SkillDatabase.Instance.GetCharacterSkill(selectedDice);
                                offender.SetSkill(units[i], skill);
                            }
                            else
                            {
                                skill = offender.GetSkill(units[i]);
                                selectedDice = skill.id;
                            }

                            if (offender.GetSkillTurn(units[i]) == 0) isActive = true;
                            remainTurn = offender.GetSkillTurn(units[i]);

                            // 발동
                            if (isActive)
                            {
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
                        }
                        diceResults.Add(units[i], selectedDice);
                        remainTurns.Add(units[i], remainTurn);
                    }
                    else
                    {
                        diceResults.Add(units[i], -1);
                        remainTurns.Add(units[i], 0);
                    }
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
                    else
                        ccResultsWithTurn[i].Add(units[j], defender.GetCCList(units[j]));
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
            if (dices.Count == 0) return diceId;

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

        private ushort IsGameEnd()
        {
            ushort defWinCount = 0;
            ushort offWinCount = 0;

            foreach (int winner in _winCount)
            {
                if (winner == (ushort)UserType.Defender) defWinCount++;
                else if (winner == (ushort)UserType.Offender) offWinCount++;
            }

            if (defWinCount >= 3) return (ushort)UserType.Defender;
            if (offWinCount >= 3) return (ushort)UserType.Offender;

            return 0;
        }

        public void GameResult()
        {
            Dictionary<string, int[]> roster = new Dictionary<string, int[]>();
            if (offender.Candidates.Count != 0)
                roster.Add(_playerId[0], offender.Candidates.ToArray());
            else 
                roster.Add(_playerId[0], new int[6]);

            if (defender.Candidates.Count != 0)
                roster.Add(_playerId[1], defender.Candidates.ToArray());
            else
                roster.Add(_playerId[1], new int[6]);

            Dictionary<int, string> roundWinner = new Dictionary<int, string>();
            for (int i = 0; i < _winCount.Length; i++)
            {
                if (_winCount[i] != 9)
                    roundWinner.Add(i + 1, _playerId[_winCount[i]]);
                else
                    roundWinner.Add(i + 1, "");
            }

            BattleResult result = new BattleResult()
            {
                date = DateTime.Now,
                gameNumber = _roomId,
                players = _playerId,
                winner = winner,
                roster = roster,
                roundWinner = roundWinner,
                roundUnit = roundUnits
            };

            string jsonString = HttpSend.SerializeObject(result);
            Console.WriteLine(jsonString);
            HttpSend.SendPost(jsonString,
                "http://ec2-13-209-42-66.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/battlereport/save");
        }
    }
}

