using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameControl
{
    public enum GameProgress { ReadyGame = 0, ReadyRound, PlayRound };
    public enum UserType { Offender = 0, Defender };

    public class GameController : MonoBehaviour
    {
        #region Instance
        private static GameController instance;
        public static GameController Instance
        {
            get
            {
                var obj = FindObjectOfType<GameController>();
                instance = obj;
                return instance;
            }
        }
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        public int roomId { get; private set; } = -1;
        public GameProgress currentProgress { get; private set; }
        public UserType userType { get; private set; }
        public int round { get; private set; }
        public int turn { get; private set; }
        private bool[] readyState = new bool[2];
        public bool progressRound { get; private set; }

        public int defenderUnit { get; private set; }
        public int[] offenderUnits { get; private set; } = new int[3];

        private Dictionary<int, List<CrowdControl>> ccList = new Dictionary<int, List<CrowdControl>>();

        private Dictionary<int, System.Tuple<CharacterSkill, int>> offednerReadyTurn = new Dictionary<int, System.Tuple<CharacterSkill, int>>();
        private Dictionary<int, bool> offenderUnitIsDead = new Dictionary<int, bool>();
        private Dictionary<int, bool> animationEnd = new Dictionary<int, bool>();

        private bool isDiceRolled = false;

        private readonly int[] skillPointPerRound = new int[5] { 1, 2, 2, 0, 0 };
        private bool isRoundEnd = false;


        /*
        // Update is called once per frame
        void Update()
        {
            if (currentProgress == GameProgress.PlayRound && progressRound == false)
            {
                if (readyState[0] && readyState[1])
                {
                    progressRound = true;
                    ProgressTurn();
                }
            }
        }
        */

        #region Network

        public void SetUserType(UserType type)
        {
            userType = type;
        }
        public void StartGame(int id, UserType type)
        {
            roomId = id;
            userType = type;
            Debug.Log("You are: " + type.ToString());
            SceneController.Instance.ChangeScene("GamePlay");
        }

        public void ReadyGameEnd(int round, List<int> enemyCandidats)
        {
            if (userType == UserType.Defender)
            {
                for (int i = 0; i < enemyCandidats.Count; i++)
                    OffenderController.Instance.SetCharacterCandidate(i, enemyCandidats[i]);
            }
            else
            {
                for (int i = 0; i < enemyCandidats.Count; i++)
                    DefenderController.Instance.SetMonsterCandidate(i, enemyCandidats[i]);
            }

            DefenderController.Instance.Init();
            OffenderController.Instance.Init();

            ReadyRound(round);
        }

        public void ReadyRound(int round)
        {
            this.round = round;
            animationEnd.Clear();

            if (userType == UserType.Offender)
                OffenderController.Instance.AddSkillPoint(skillPointPerRound[round - 1]);
            else
                DefenderController.Instance.SetMonsterHp();

            currentProgress = GameProgress.ReadyRound;
        }

        public void StartRound(int round, List<S_RoundReadyEnd.EnemyRoster> enemys)
        {
            if (userType == UserType.Defender)
            {
                OffenderController.Instance.SetSkillRoster(enemys);
            }
            else
            {
                DefenderController.Instance.SetSkillRoster(enemys);
            }

            OffenderController.Instance.SetRoster();
            DefenderController.Instance.SetRoster();
            DefenderController.Instance.HealBattleMonster(defenderUnit);

            this.round = round;

            currentProgress = GameProgress.PlayRound;
            turn = 0;
            isRoundEnd = false;
            progressRound = false;
            animationEnd.Clear();
            offenderUnitIsDead.Clear();
            offednerReadyTurn.Clear();
            ccList.Clear();

            // 선공 확인해서 순서 조정
            animationEnd.Add(defenderUnit, true);
            ccList.Add(defenderUnit, new List<CrowdControl>());
            foreach (int key in offenderUnits)
            {
                animationEnd.Add(key, true);
                offenderUnitIsDead.Add(key, false);
                offednerReadyTurn.Add(key, null);
                ccList.Add(key, new List<CrowdControl>());
            }

            if (userType == UserType.Defender)
            {
                GamePlayUIController.Instance.ShowSelectedEnemy(offenderUnits);
            }
            else if (userType == UserType.Offender)
            {
                GamePlayUIController.Instance.ShowSelectedEnemy(new int[] { defenderUnit });
            }

            GamePlayUIController.Instance.UpdateCharacters();
        }

        public void ProgressTurn(int round, int turn, S_ProgressTurn packet)
        {
            progressRound = true;
            Dictionary<int, List<int>> dices = new Dictionary<int, List<int>>();
            Dictionary<int, int> selectedSkills = new Dictionary<int, int>();
            List<bool> isAttack = new List<bool>();
            List<bool> isWait = new List<bool>();

            Dictionary<int, List<List<CrowdControl>>> ccs = new Dictionary<int, List<List<CrowdControl>>>();

            foreach (S_ProgressTurn.Result item in packet.results)
            {
                selectedSkills.Add(item.unitIndex, item.diceResult);
                dices.Add(item.unitIndex, item.diceIndexs);
                
                if (item.diceResult == -1)
                {
                    animationEnd[item.unitIndex] = true;
                    isAttack.Add(false);
                }
                else
                {
                    animationEnd[item.unitIndex] = false;
                    isAttack.Add(true);
                }
                isWait.Add(false);

                List<List<CrowdControl>> ccsWithTurn = new List<List<CrowdControl>>();
                foreach (S_ProgressTurn.Result.CcWithTurn ccitem in item.ccWithTurns)
                {
                    List<CrowdControl> tmp = new List<CrowdControl>();
                    foreach (S_ProgressTurn.Result.CcWithTurn.Cc ccInfo in ccitem.ccs)
                    {
                        CrowdControl cc = SkillDatabase.Instance.GetCrowdControl(ccInfo.ccid);
                        cc.SetTurn(ccInfo.ccturn);
                        cc.SetStack(ccInfo.ccstack);
                        tmp.Add(cc);
                    }
                    ccsWithTurn.Add(tmp);
                }
                ccs.Add(item.unitIndex, ccsWithTurn);
            }

            GamePlayUIController.Instance.DiceRoll(isAttack, isWait);
            DiceRolled();
            foreach (bool b in isAttack)
            {
                if (b)
                {
                    isDiceRolled = true;
                    break;
                }
            }

            StartCoroutine(BattleAnimation(turn, dices, selectedSkills, packet.monsterHps, ccs));
        }
        IEnumerator BattleAnimation(int turn, Dictionary<int, List<int>> dices, Dictionary<int, int> diceSkills, List<int> monsterHps, Dictionary<int, List<List<CrowdControl>>> ccs)
        {
            while (isDiceRolled) yield return null;
            GamePlayUIController.Instance.ShowDices(dices);

            List<int> keys = animationEnd.Keys.ToList<int>();
            int i = 0;
            for (; i < animationEnd.Count; i++)
            {
                if (diceSkills[keys[i]] < 0) continue;

                GamePlayUIController.Instance.PlayAnimation(keys[i], "Attack");
                while (animationEnd[keys[i]] == false) yield return null;

                DefenderController.Instance.SetMonsterHp(monsterHps[i]);

                for (int j = 0; j < ccs.Count; j++)
                {
                    ShowCrowdControls(keys[j], ccs[keys[j]][i]);
                }
                GamePlayUIController.Instance.UpdateCharacters();
            }

            float time = 0.5f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            DefenderController.Instance.SetMonsterHp(monsterHps[i]);
            for (int j = 0; j < ccs.Count; j++)
            {
                ShowCrowdControls(keys[j], ccs[keys[j]][i]);
            }

            time = 0.5f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            progressRound = false;
            GamePlayUIController.Instance.RemoveDices();
            NextTurnInNetwork(turn);
        }

        public void ShowCrowdControls(int unitIndex, List<CrowdControl> ccs)
        {
            for(int i = 0; i < ccs.Count; i++)
            {
                bool isStackSkill = ccs[i].IsStackCC();

                if (isStackSkill && ccs[i].stack > 0)
                    GamePlayUIController.Instance.UpdateCrowdControl(unitIndex, ccs[i].id, -1, ccs[i].stack);
                else GamePlayUIController.Instance.UpdateCrowdControl(unitIndex, ccs[i].id, ccs[i].turn, ccs[i].stack);
            }
        }

        public void NextTurnInNetwork(int turn)
        {
            if (isRoundEnd) return;
            progressRound = false;
            GamePlayUIController.Instance.SetTurn(turn);
        }

        public void GameEnd(UserType winner)
        {
            StartCoroutine(ShowResult(winner, true));
        }

        IEnumerator ShowResult(UserType winner, bool gameEnd)
        {
            int alertIndex = 40;
            if (winner == UserType.Defender) alertIndex = 41;

            GamePlayUIController.Instance.Alert(alertIndex);

            float time = 1.5f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            if (gameEnd) SceneController.Instance.ChangeScene("Main"); // 씬 이동 임시
            /*
            else if (winner == UserType.Defender) ReadyRound(true);
            else ReadyRound(false);
            */
        }

        #endregion

        public void ReadyGame()
        {
            GamePlayUIController.Instance.SetUserType();

            currentProgress = GameProgress.ReadyGame;

            round = 0;

            DefenderController.Instance.ResetCandidates();
            OffenderController.Instance.ResetCandidates();

            GamePlayUIController.Instance.ChangeView();
        }

        public void ReadyRound(bool isOffenderDefeated = false)
        {
            if (roomId != -1)
            {
                if (isOffenderDefeated == false)
                {
                    OffenderController.Instance.AddSkillPoint(skillPointPerRound[round - 1]);
                    round++;
                }
                DefenderController.Instance.SetMonsterHp();

                animationEnd.Clear();
                currentProgress = GameProgress.ReadyRound;

                if (userType == UserType.Defender)
                {
                    AIBot.Instance.LearnSkill();
                    AIBot.Instance.OffenderSetDice();
                }
                else AIBot.Instance.DefenderSetDice();
            }
        }

        public void SelectUnit(UserType type, int[] units) // 서버 입장에서는 type 필요
        {
            if (type == UserType.Defender)
            {
                defenderUnit = units[0] + 20;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    offenderUnits[i] = units[i] + 10;
                }
            }
        }

        public void ReadyTurn(UserType type, bool ready) // 서버 입장에서는 type 필요
        {
            readyState[(short)type] = ready;
        }

        private void ProgressTurn()
        {
            if (isRoundEnd) return;

            List<MonsterSkill> monSkills = new List<MonsterSkill>();
            bool monIsParalysis = HasCrowdControl(defenderUnit, CCType.BLIND);

            /*
            for (int j = 0; j < 2; j++)
                monSkills.Add(DefenderController.Instance.DiceRoll(defenderUnit % 10, monIsParalysis));
            */

            List<bool> isAttack = new List<bool>();
            List<bool> isWait = new List<bool>();

            isAttack.Add(CanAttack(defenderUnit));
            isWait.Add(false);

            Dictionary<int, CharacterSkill> charSkills = new Dictionary<int, CharacterSkill>();
            for (int j = 0; j < offenderUnits.Length; j++)
            {
                isAttack.Add(CanAttack(offenderUnits[j]));
                if (offednerReadyTurn[offenderUnits[j]] != null)
                {
                    charSkills.Add(offenderUnits[j], offednerReadyTurn[offenderUnits[j]].Item1);
                    isWait.Add(true);
                }
                else
                {
                    //charSkills.Add(offenderUnits[j], OffenderController.Instance.DiceRoll(offenderUnits[j], HasCrowdControl(defenderUnit, CCType.BLIND)));
                    isWait.Add(false);
                }
            }

            for (int j = 0; j < offenderUnits.Length; j++)
            {
                if (offednerReadyTurn[offenderUnits[j]] == null && isAttack[j] && charSkills[offenderUnits[j]].turn > 0)
                {
                    int turn = charSkills[offenderUnits[j]].turn;
                    offednerReadyTurn[offenderUnits[j]] =
                        new System.Tuple<CharacterSkill, int>(charSkills[offenderUnits[j]], turn);
                }
            }


            GamePlayUIController.Instance.DiceRoll(isAttack, isWait);
            int i = 0;

            for (; i < monSkills.Count; i++)
            {
                if (isAttack[0]) GamePlayUIController.Instance.SetDiceSkill(i, monSkills[i].id);
            }

            foreach (CharacterSkill skill in charSkills.Values)
            {
                if (isAttack[i - 1]) GamePlayUIController.Instance.SetDiceSkill(i, skill.id);
                i++;
            }
            StartCoroutine(Battle(monSkills, charSkills, isAttack));
        }

        public void DiceRolled()
        {
            isDiceRolled = false;
        }

        IEnumerator Battle(List<MonsterSkill> monSkills, Dictionary<int, CharacterSkill> charSkills, List<bool> isAttack)
        {
            foreach (bool b in isAttack)
            {
                DiceRolled();
                if (b)
                {
                    isDiceRolled = true;
                    break;
                }
            }
            while (isDiceRolled) yield return null;

            // 순차적으로 공격을 누가 먼저 해서 진행될지 정할 필요 있음.
            if (isAttack[0]) animationEnd[defenderUnit] = false;
            else animationEnd[defenderUnit] = true;

            for (int i = 1; i < animationEnd.Count; i++)
            {
                int offenderUnit = offenderUnits[i - 1];
                // 전투불능 상태 체크해서 Animation End 이용
                if (isAttack[i]) animationEnd[offenderUnit] = false;
                else animationEnd[offenderUnit] = true;
            }

            bool defenderOk = (monSkills[0].id == monSkills[1].id);
            Debug.Log($"1: {monSkills[0].name} , 2: {monSkills[1].name} : {defenderOk && isAttack[0]}");
            Debug.Log($"1: {charSkills[offenderUnits[0]].name} : {charSkills[offenderUnits[0]].turn}, 2: {charSkills[offenderUnits[1]].name} : {charSkills[offenderUnits[1]].turn}, 3: {charSkills[offenderUnits[2]].name} : {charSkills[offenderUnits[2]].turn}");

            Debug.Log($"1: {isAttack[0]}, 2: {isAttack[1]}, 3: {isAttack[2]}, 4: {isAttack[3]}");

            // 전투 시 전투불능 확인해서 그 때 그 때 바꿔줘야함
            List<int> keys = animationEnd.Keys.ToList<int>();
            for (int i = 0; i < animationEnd.Count; i++)
            {
                if (animationEnd[keys[i]] == true) continue;

                bool isMonster = ((keys[i] / 10) == 2);

                if (isMonster && defenderOk == false)
                {
                    animationEnd[keys[i]] = true;
                    continue;
                }

                GamePlayUIController.Instance.PlayAnimation(keys[i], "Attack");

                while (animationEnd[keys[i]] == false) yield return null;
                // 전투 정보 전송
                {
                    int ccMultiplier = (HasCrowdControl(keys[i], CCType.MIRRORIMAGE)) ? 2 : 1;
                    if (isMonster == false)
                    {
                        int damage = charSkills[keys[i]].damage;

                        if (offednerReadyTurn[keys[i]] != null)
                        {
                            if (offednerReadyTurn[keys[i]].Item2 <= 0)
                            {
                                offednerReadyTurn[keys[i]] = null;
                            }
                            else
                            {
                                offednerReadyTurn[keys[i]] = new System.Tuple<CharacterSkill, int>(offednerReadyTurn[keys[i]].Item1, offednerReadyTurn[keys[i]].Item2 - 1);
                                GamePlayUIController.Instance.UpdateOffenderCharacter(keys[i], offednerReadyTurn[keys[i]]);
                                continue;
                            }
                        }
                        GamePlayUIController.Instance.UpdateOffenderCharacter(keys[i], offednerReadyTurn[keys[i]]);

                        if (HasCrowdControl(keys[i], CCType.ATTACKSTAT, CCTarget.SELF)) damage = (int)(damage * 1.5f);
                        if (HasCrowdControl(keys[i], CCType.MIRRORIMAGE)) damage = (int)(damage * 1.5f);
                        if (HasCrowdControl(keys[i], CCType.ATTACKSTAT, CCTarget.ENEMY)) damage = (int)(damage * 0.7f);

                        int target = defenderUnit;

                        if (HasCrowdControl(keys[i], CCType.CONFUSION))
                        {
                            List<int> aliveIndexes = new List<int>();

                            foreach (int key in offenderUnits)
                                if (offenderUnitIsDead[key] == false) aliveIndexes.Add(key);

                            int random = Random.Range(0, aliveIndexes.Count + 1);
                            if (random != 0) target = aliveIndexes[random - 1];

                            Debug.Log(keys[i] + "is Confused Attack " + target);
                            continue;
                        }
                        else
                        {
                            int restHp = DefenderController.Instance.MonsterDamaged(defenderUnit % 10, damage);
                            if (restHp <= 0)
                            {
                                DefenderController.Instance.Dead(defenderUnit);
                                DefenderDefeated();
                            }
                        }


                        if (charSkills[keys[i]].ccList.Count != 0)
                        {
                            foreach (CrowdControl cc in charSkills[keys[i]].ccList.Keys)
                            {
                                if (cc.target == CCTarget.ENEMY)
                                {
                                    if (target != defenderUnit)
                                    {
                                        foreach (int unit in offenderUnits)
                                        {
                                            CrowdControl tmp = ccList[unit].Find(crowdControl => crowdControl.cc == CCType.TAUNT);
                                            if (tmp != null) target = unit;
                                        }
                                    }
                                    AddCrowdControl(target, cc, charSkills[keys[i]].ccList[cc], keys[i], ccMultiplier);
                                }
                                else if (cc.target == CCTarget.SELF)
                                    AddCrowdControl(keys[i], cc, charSkills[keys[i]].ccList[cc], keys[i], ccMultiplier);
                                else
                                    AddCrowdControl(keys[i], cc, charSkills[keys[i]].ccList[cc], keys[i], ccMultiplier, true);
                            }
                        }
                    }
                    else
                    {
                        // 주사위 결과 발생
                        // 몬스터가 공격하는 입장.
                        int index = Random.Range(0, offenderUnits.Length);
                        if (monSkills[0].ccList.Count != 0)
                        {
                            foreach (CrowdControl cc in monSkills[0].ccList.Keys)
                            {
                                if (cc.target == CCTarget.ENEMY)
                                {
                                    int target = offenderUnits[index];
                                    foreach (int unit in offenderUnits)
                                    {
                                        CrowdControl tmp = ccList[unit].Find(crowdControl => crowdControl.cc == CCType.TAUNT);
                                        if (tmp != null) target = unit;
                                    }
                                    AddCrowdControl(target, cc, monSkills[0].ccList[cc], keys[i], ccMultiplier);
                                }
                                else if (cc.target == CCTarget.SELF)
                                    AddCrowdControl(keys[i], cc, monSkills[0].ccList[cc], keys[i], ccMultiplier);
                                else
                                    AddCrowdControl(defenderUnit, cc, monSkills[0].ccList[cc], defenderUnit, ccMultiplier, true);
                            }
                        }
                    }
                    GamePlayUIController.Instance.UpdateCharacters();
                }
            }

            float time = 0.3f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            progressRound = false;
            NextTurn();
        }

        private bool CanAttack(int index)
        {
            if (index / 10 == 1 && offenderUnitIsDead[index]) return false;
            if (HasCrowdControl(index, CCType.STUN)) return false;

            return true;
        }

        #region CrowdControl
        private void AddCrowdControl(int index, CrowdControl cc, int ccStack, int useIndex, int ccMultiplier, bool isAll = false)
        {
            bool isMonster = (index / 10 == 2);
            int ccTurn = cc.turn;

            if (ccMultiplier != 1)
            {
                ccStack *= ccMultiplier;
                ccTurn = cc.turn + 1;
            }

            List<int> indexes = new List<int>();
            if (isAll)
            {
                foreach (int key in offenderUnits)
                {
                    if (offenderUnitIsDead[key] == false) indexes.Add(key);
                }
            }
            else indexes.Add(index);

            for (int i = 0; i < indexes.Count; i++)
            {
                if (cc.cc == CCType.PURITY || cc.cc == CCType.INVINCIBLE)
                {
                    PurifyCrowdControl(indexes[i], true);
                    if (cc.cc == CCType.PURITY) continue;
                }
                else if (cc.cc == CCType.REMOVE)
                {
                    PurifyCrowdControl(indexes[i], false);
                    continue;
                }
                else if (cc.cc == CCType.DRAIN)
                {
                    DefenderController.Instance.HealBattleMonster(defenderUnit, ccStack);
                    GamePlayUIController.Instance.UpdateCharacters();
                    continue;
                }

                if (cc.target == CCTarget.ENEMY || (cc.target == CCTarget.ALL && isMonster))
                {
                    if (HasCrowdControl(indexes[i], CCType.BARRIER) || HasCrowdControl(indexes[i], CCType.INVINCIBLE))
                    {
                        Debug.Log("Guard " + cc.name);
                        continue;
                    }
                    else if (HasCrowdControl(indexes[i], CCType.REFLECT))
                    {
                        Debug.Log("Reflect " + cc.name);
                        if (HasCrowdControl(useIndex, CCType.REFLECT) == false) AddCrowdControl(useIndex, cc, ccStack, indexes[i], 1, false);
                        continue;
                    }
                }

                int ccIndex = ccList[indexes[i]].FindIndex(charCC => charCC.name == cc.name);

                if (ccIndex == -1)
                {
                    ccList[indexes[i]].Add(SkillDatabase.Instance.GetCrowdControl(cc.id));
                    ccList[indexes[i]][ccList[indexes[i]].Count - 1].SetTurn(ccTurn);

                    if (cc.cc == CCType.DOTDAMAGE)
                        ccList[indexes[i]][ccList[indexes[i]].Count - 1].SetDotDamage(ccStack);

                    bool isStackSkill = ccList[indexes[i]][ccList[indexes[i]].Count - 1].ControlCC(ccStack);

                    if (isStackSkill && ccList[indexes[i]][ccList[indexes[i]].Count - 1].stack > 0)
                        GamePlayUIController.Instance.UpdateCrowdControl(indexes[i], cc.id, -1, ccList[indexes[i]][ccList[indexes[i]].Count - 1].stack);
                    else GamePlayUIController.Instance.UpdateCrowdControl(indexes[i], cc.id, ccTurn, ccList[indexes[i]][ccList[indexes[i]].Count - 1].stack);

                }
                else
                {
                    CrowdControl curCC = ccList[indexes[i]][ccIndex];

                    if (curCC.cc == CCType.DOTDAMAGE && curCC.id == cc.id && curCC.dotDamage < ccStack)
                    {
                        curCC.SetDotDamage(ccStack);
                    }

                    bool isStackSkill = curCC.ControlCC(ccStack);

                    if (isStackSkill == false)
                    {
                        if (curCC.turn < ccTurn) curCC.SetTurn(ccTurn);
                    }
                    else
                    {
                        int curStack = curCC.stack;
                        // CC 발동 시점
                        if (curStack == 0)
                        {
                            // CC 발동
                            curCC.SetTurn(ccTurn);
                            GamePlayUIController.Instance.UpdateCrowdControl(indexes[i], cc.id, ccTurn, 0);
                        }
                        // CC 스택 쌓는 시점
                        else if (curStack > 0)
                        {
                            GamePlayUIController.Instance.UpdateCrowdControl(indexes[i], cc.id, -1, curCC.stack);
                        }
                        // CC 발동 후 이번 턴에 발동된 CC가 아니라면 스택 리셋 후 해당수치만큼 감소
                        else if (curCC.turn != curCC.GetCCBasicTurn())
                        {
                            curCC.ResetCCStack();
                            curCC.ControlCC(ccStack);
                        }
                    }
                }
            }
        }

        private void CrowdControlProgressTurn()
        {
            foreach (int key in ccList.Keys)
            {
                if (key / 10 == 1 && offenderUnitIsDead[key]) ccList[key].Clear();
                for (int i = 0; i < ccList[key].Count; i++)
                {
                    CrowdControl curCC = ccList[key][i];
                    bool isStackSkill = curCC.ControlCC(0);

                    if (isStackSkill && curCC.stack > 0 && curCC.turn == curCC.GetCCBasicTurn()) continue;

                    bool b = curCC.ProgressTurn();
                    if (curCC.cc == CCType.DOTDAMAGE)
                    {
                        int restHp = DefenderController.Instance.MonsterDamaged(key % 10, curCC.dotDamage);
                        GamePlayUIController.Instance.UpdateCharacters();
                        if (restHp < 0)
                        {
                            DefenderController.Instance.Dead(defenderUnit);
                            DefenderDefeated();
                        }
                    }
                    GamePlayUIController.Instance.UpdateCrowdControl(key, curCC.id, curCC.turn, curCC.stack, b);
                    if (b)
                    {
                        ccList[key].RemoveAt(i);
                        i--;
                        if (isStackSkill && curCC.stack > 0)
                        {
                            // 만약에 지웠는데 스택이 남았었다면 턴 초기화 후 그대로 다시 추가
                            curCC.SetTurn(curCC.GetCCBasicTurn());
                            ccList[key].Add(curCC);
                            GamePlayUIController.Instance.UpdateCrowdControl(key, curCC.id, -1, curCC.stack, true);
                        }
                    }
                }
            }
        }

        private bool HasCrowdControl(int index, CCType ccType, CCTarget target = CCTarget.ENEMY)
        {
            CrowdControl tmp = ccList[index].Find(cc => cc.cc == ccType);

            bool b = tmp != null;
            if (b)
            {
                if (ccType == CCType.ATTACKSTAT) b = (tmp.target == target);
                if (tmp.ControlCC(0))
                {
                    if (tmp.stack <= 0) b = true;
                    else b = false;
                }
            }

            return b;
        }

        private void PurifyCrowdControl(int index, bool isGood)
        {
            Debug.Log("Purify");
            // 몬스터
            if (index / 10 == 2)
            {
                for (int i = 0; i < ccList[index].Count; i++)
                {
                    if (isGood)
                    {
                        if (ccList[index][i].target == CCTarget.ENEMY)
                        {
                            GamePlayUIController.Instance.UpdateCrowdControl(index, ccList[index][i].id, 0, 0, true);
                            ccList[index].RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        if (ccList[index][i].target == CCTarget.ALL || ccList[index][i].target == CCTarget.SELF)
                        {
                            GamePlayUIController.Instance.UpdateCrowdControl(index, ccList[index][i].id, 0, 0, true);
                            ccList[index].RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            // 유닛들
            else
            {
                for (int i = 0; i < ccList[index].Count; i++)
                {
                    if (isGood)
                    {
                        if (ccList[index][i].target == CCTarget.ENEMY || ccList[index][i].target == CCTarget.ALL)
                        {
                            GamePlayUIController.Instance.UpdateCrowdControl(index, ccList[index][i].id, 0, 0, true);
                            ccList[index].RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        if (ccList[index][i].target == CCTarget.SELF)
                        {
                            GamePlayUIController.Instance.UpdateCrowdControl(index, ccList[index][i].id, 0, 0, true);
                            ccList[index].RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        #endregion

        public void AnimationEnd(int index)
        {
            animationEnd[index] = true;
        }

        public void NextTurn()
        {
            if (isRoundEnd) return;

            GamePlayUIController.Instance.SetTurn(++turn);

            bool isAttack = false;
            if (turn != 1)
            {
                isAttack = DefenderController.Instance.AttackSkillNextTurn();
                if (HasCrowdControl(defenderUnit, CCType.DECREASETURN)) isAttack = DefenderController.Instance.AttackSkillNextTurn();
            }

            for (int i = 0; i < offenderUnits.Length; i++)
            {
                GamePlayUIController.Instance.UpdateOffenderCharacter(offenderUnits[i], offednerReadyTurn[offenderUnits[i]]);
            }

            if (isAttack)
            {
                // 공격부분
                MonsterSkill skill = DefenderController.Instance.GetAttackSkill();

                // 죽을 유닛 선정
                List<int> aliveIndexes = new List<int>();
                int tauntIndex = -1;
                foreach (int key in offenderUnits)
                    if (offenderUnitIsDead[key] == false) aliveIndexes.Add(key);

                if (aliveIndexes.Count == 1)
                {
                    DefenderController.Instance.ResetAttackSkill();
                    OffenderDefeated();
                    return;
                }

                for (int i = 0; i < aliveIndexes.Count; i++)
                {
                    CrowdControl tmp = ccList[aliveIndexes[i]].Find(cc => cc.cc == CCType.TAUNT);
                    if (tmp != null) tauntIndex = i;
                }

                int deadUnit = Random.Range(0, aliveIndexes.Count);
                if (tauntIndex != -1) deadUnit = tauntIndex;

                switch (skill.type)
                {
                    case MonsterSkill.SkillType.AttackAll:
                        offednerReadyTurn.Clear();
                        ccList.Clear();
                        OffenderDefeated();
                        return;

                    case MonsterSkill.SkillType.AttackOne:
                        offenderUnitIsDead[aliveIndexes[deadUnit]] = true;
                        GamePlayUIController.Instance.DeadCharacter(offenderUnits[deadUnit]);
                        if (offednerReadyTurn[aliveIndexes[deadUnit]] != null) offednerReadyTurn[aliveIndexes[deadUnit]] = null;
                        ccList[aliveIndexes[deadUnit]].Clear();
                        break;
                    case MonsterSkill.SkillType.AttackOneStun:
                        offenderUnitIsDead[aliveIndexes[deadUnit]] = true;
                        GamePlayUIController.Instance.DeadCharacter(offenderUnits[deadUnit]);
                        ccList[aliveIndexes[deadUnit]].Clear();
                        if (offednerReadyTurn[aliveIndexes[deadUnit]] != null) offednerReadyTurn[aliveIndexes[deadUnit]] = null;
                        do
                        {
                            int stunUnit = Random.Range(0, aliveIndexes.Count);
                            if (stunUnit != deadUnit)
                            {
                                List<CrowdControl> ccs = skill.ccList.Keys.ToList();
                                AddCrowdControl(aliveIndexes[stunUnit], ccs[0], skill.ccList[ccs[0]], defenderUnit, 1);
                                break;
                            }
                        } while (true);
                        break;
                }

                DefenderController.Instance.ResetAttackSkill();
            }

            CrowdControlProgressTurn();
            GamePlayUIController.Instance.UpdateCharacters();

            readyState[0] = false;
            readyState[1] = false;
            if (userType == UserType.Defender) ReadyTurn(UserType.Offender, true);
            else if (userType == UserType.Offender) ReadyTurn(UserType.Defender, true);
        }
        private void DefenderDefeated()
        {
            isRoundEnd = true;
            // 전투가 넘어가기전에 애니메이션 등
            Debug.Log("Defender Defeated");

            List<int> keys = animationEnd.Keys.ToList();
            for (int i = 0; i < animationEnd.Count; i++)
            {
                animationEnd[keys[i]] = true;
            }

            readyState[0] = false;
            readyState[1] = false;

            progressRound = false;

            StopAllCoroutines();

            bool roundEnd = false;
            if (round >= 3)
            {
                OffenderController.Instance.ResetDead();
                DefenderController.Instance.ResetDead();
                roundEnd = true;
            }

            StartCoroutine(ShowResult(UserType.Offender, roundEnd));
        }

        private void OffenderDefeated()
        {
            isRoundEnd = true;
            // 전투가 넘어가기전에 애니메이션 등
            Debug.Log("Offender Defeated");

            List<int> keys = animationEnd.Keys.ToList();
            for (int i = 0; i < animationEnd.Count; i++)
            {
                animationEnd[keys[i]] = true;
            }

            readyState[0] = false;
            readyState[1] = false;

            progressRound = false;

            StopAllCoroutines();

            foreach (int index in offenderUnits)
                OffenderController.Instance.Dead(index);

            bool roundEnd = false;
            if (OffenderController.Instance.GetAliveCharacterList().Count == 0)
            {
                OffenderController.Instance.ResetDead();
                DefenderController.Instance.ResetDead();
                roundEnd = true;
            }
            else
            {
                int deadAmount = 1;
                if (OffenderController.Instance.GetAliveCharacterList().Count == 2) deadAmount = 2;

                int[] deadIndexes = new int[deadAmount];

                deadIndexes[0] = Random.Range(0, offenderUnits.Length);
                if (deadIndexes.Length > 1)
                    do
                    {
                        deadIndexes[1] = Random.Range(0, offenderUnits.Length);
                        if (deadIndexes[0] != deadIndexes[1]) break;
                    } while (true);

                for (int i = 0; i < offenderUnits.Length; i++)
                {
                    if (deadIndexes.Length > 1)
                    {
                        if (i != deadIndexes[0] && i != deadIndexes[1]) OffenderController.Instance.Alive(offenderUnits[i]);
                    }
                    else if (i != deadIndexes[0]) OffenderController.Instance.Alive(offenderUnits[i]);
                }
                DefenderController.Instance.HealBattleMonster(defenderUnit);
            }
            StartCoroutine(ShowResult(UserType.Defender, roundEnd));
        }

    }
}
