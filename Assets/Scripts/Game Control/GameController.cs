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
        private bool isPlay;
        public GameProgress currentProgress { get; private set; }
        public UserType userType { get; private set; }
        public int round { get; private set; }
        public int turn { get; private set; }
        private bool[] readyState = new bool[2];
        public bool progressRound { get; private set; }

        public int defenderUnit { get; private set; }
        public int[] offenderUnits { get; private set; } = new int[3];

        private Dictionary<int, List<CrowdControl>> ccList = new Dictionary<int, List<CrowdControl>>();

        private Dictionary<int, bool> offenderUnitIsDead = new Dictionary<int, bool>();
        private Dictionary<int, bool> animationEnd = new Dictionary<int, bool>();

        private bool isDiceRolled = false;

        private readonly int[] skillPointPerRound = new int[3] { 1, 2, 2 };
        private bool isRoundEnd = false;

        // Start is called before the first frame update
        void Start()
        {
            isPlay = false;
        }

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

        public void SetUserType(UserType type)
        {
            userType = type;
            Debug.Log("You are: " + type.ToString());
        }

        public void ReadyGame()
        {
            currentProgress = GameProgress.ReadyGame;

            isPlay = true;
            round = 0;

            DefenderController.Instance.ResetCandidates();
            OffenderController.Instance.ResetCandidates();

            GamePlayUIController.Instance.SetUserType();
            GamePlayUIController.Instance.ChangeView();

        }

        public void StartGame()
        {
            // Defender, Offender의 Init (selected Candidates에서 목록 뽑아옴)
            // Init 전에 Candidate 생성

            AIBot.Instance.StartGame();

            /*if (userType == UserType.Defender) DefenderController.Instance.Init();
            else OffenderController.Instance.Init();*/

            DefenderController.Instance.Init();
            OffenderController.Instance.Init();

            ReadyRound();
        }

        public void ReadyRound(bool isOffenderDefeated = false)
        {
            if (isPlay)
            {
                if (isOffenderDefeated == false)
                {
                    OffenderController.Instance.AddSkillPoint(skillPointPerRound[round]);
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

        public void StartRound()
        {
            // 로스터 세팅
            // 주사위 세팅
            AIBot.Instance.SetRoster();

            currentProgress = GameProgress.PlayRound;
            turn = 0;
            isRoundEnd = false;
            progressRound = false;
            animationEnd.Clear();
            offenderUnitIsDead.Clear();
            ccList.Clear();

            // 선공 확인해서 순서 조정
            animationEnd.Add(defenderUnit, true);
            ccList.Add(defenderUnit, new List<CrowdControl>());
            foreach (int key in offenderUnits)
            {
                animationEnd.Add(key, true);
                offenderUnitIsDead.Add(key, false);
                ccList.Add(key, new List<CrowdControl>());
            }

            if (userType == UserType.Defender)
            {
                GamePlayUIController.Instance.ShowSelectedRoster(defenderUnit);
                GamePlayUIController.Instance.ShowSelectedRoster(offenderUnits, true);
            }
            else if (userType == UserType.Offender)
            {
                GamePlayUIController.Instance.ShowSelectedRoster(offenderUnits);
                GamePlayUIController.Instance.ShowSelectedRoster(defenderUnit, true);
            }

            NextTurn();
        }

        public void ReadyTurn(UserType type, bool ready) // 서버 입장에서는 type 필요
        {
            readyState[(short)type] = ready;
        }

        private void ProgressTurn()
        {
            List<MonsterSkill> monSkills = new List<MonsterSkill>();
            bool monIsParalysis = IsParalysis(defenderUnit);
            for (int j = 0; j < 2; j++)
                monSkills.Add(DefenderController.Instance.DiceRoll(defenderUnit % 10, monIsParalysis));

            Dictionary<int, CharacterSkill> charSkills = new Dictionary<int, CharacterSkill>();
            for (int j = 0; j < offenderUnits.Length; j++)
                charSkills.Add(offenderUnits[j], OffenderController.Instance.DiceRoll(offenderUnits[j], IsParalysis(offenderUnits[j])));

            List<bool> isAttack = new List<bool>();

            isAttack.Add(CanAttack(defenderUnit));

            for (int j = 0; j < offenderUnits.Length; j++)
                isAttack.Add(CanAttack(offenderUnits[j]));

            GamePlayUIController.Instance.DiceRoll(isAttack);
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
            isDiceRolled = true;
            StartCoroutine(Battle(monSkills, charSkills, isAttack));
        }

        public void DiceRolled()
        {
            isDiceRolled = false;
        }

        IEnumerator Battle(List<MonsterSkill> monSkills, Dictionary<int, CharacterSkill> charSkills, List<bool> isAttack)
        {
            while (isDiceRolled) yield return null;

            bool defenderOk = (monSkills[0].id == monSkills[1].id);
            Debug.Log($"1: {monSkills[0].name} , 2: {monSkills[1].name} : {defenderOk && isAttack[0]}");
            Debug.Log($"1: {charSkills[offenderUnits[0]].name} | {isAttack[1]}, 2: {charSkills[offenderUnits[1]].name} | {isAttack[2]}, 3: {charSkills[offenderUnits[2]].name} | {isAttack[3]}");

            // 순차적으로 공격을 누가 먼저 해서 진행될지 정할 필요 있음.
            if (defenderOk && isAttack[0]) animationEnd[defenderUnit] = false;
            for (int i = 1; i < animationEnd.Count; i++)
            {
                int offenderUnit = offenderUnits[i - 1];
                // 전투불능 상태 체크해서 Animation End 이용
                if (isAttack[i]) animationEnd[offenderUnit] = false;
                else animationEnd[offenderUnit] = true;
            }

            // 전투 시 전투불능 확인해서 그 때 그 때 바꿔줘야함
            List<int> keys = animationEnd.Keys.ToList<int>();
            for (int i = 0; i < animationEnd.Count; i++)
                if (animationEnd[keys[i]] == false)
                {
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
                        if (isMonster == false)
                        {
                            int restHp = DefenderController.Instance.MonsterDamaged(defenderUnit % 10, charSkills[keys[i]]);
                            if (restHp <= 0)
                            {
                                DefenderController.Instance.Dead(defenderUnit);
                                DefenderDefeated();
                            }

                            if (charSkills[keys[i]].ccList.Count != 0)
                            {
                                foreach (CrowdControl cc in charSkills[keys[i]].ccList.Keys)
                                {
                                    if (cc.target == CCTarget.ENEMY)
                                        AddCrowdControl(defenderUnit, cc, charSkills[keys[i]].ccList[cc]);
                                    else if (cc.target == CCTarget.SELF)
                                        AddCrowdControl(keys[i], cc, charSkills[keys[i]].ccList[cc]);
                                    else
                                        AddCrowdControl(keys[i], cc, charSkills[keys[i]].ccList[cc], true);
                                }
                            }
                        }
                        else
                        {
                            // 주사위 결과 발생
                            int index = Random.Range(0, offenderUnits.Length);
                            if (monSkills[0].ccList.Count != 0)
                            {
                                foreach (CrowdControl cc in monSkills[0].ccList.Keys)
                                {
                                    if (cc.target == CCTarget.ENEMY)
                                        AddCrowdControl(offenderUnits[index], cc, monSkills[0].ccList[cc]);
                                    else if (cc.target == CCTarget.SELF)
                                        AddCrowdControl(keys[i], cc, monSkills[0].ccList[cc]);
                                    else
                                        AddCrowdControl(defenderUnit, cc, monSkills[0].ccList[cc], true);
                                }
                            }
                        }
                        GamePlayUIController.Instance.UpdateCharacters();
                    }
                }

            progressRound = false;
            NextTurn();
        }

        private bool CanAttack(int index)
        {
            if (index / 10 == 1 && offenderUnitIsDead[index]) return false;

            foreach (CrowdControl cc in ccList[index])
            {
                if (cc.cc == CCtype.STUN)
                {
                    if (cc.turn < cc.GetCCBasicTurn()) return false;
                }
            }

            return true;
        }

        #region CrowdControl
        private void AddCrowdControl(int index, CrowdControl cc, int ccStack, bool isAll = false)
        {
            bool isMonster = (index / 10 == 2);

            int ccIndex = ccList[index].FindIndex(charCC => charCC.name == cc.name);

            if (ccIndex == -1)
            {
                List<int> indexes = new List<int>();
                if (isAll)
                {
                    foreach (int key in offenderUnits)
                        indexes.Add(key);
                }
                else indexes.Add(index);

                for (int i = 0; i < indexes.Count; i++)
                {
                    ccList[indexes[i]].Add(SkillDatabase.Instance.GetCrowdControl(cc.id));
                    bool isStackSkill = ccList[indexes[i]][ccList[indexes[i]].Count - 1].ControlCC(ccStack);

                    if (isStackSkill && ccList[indexes[i]][ccList[indexes[i]].Count - 1].stack > 0)
                        GamePlayUIController.Instance.UpdateCrowdControl(indexes[i], cc.id, -1, ccList[indexes[i]][ccList[indexes[i]].Count - 1].stack);
                    else GamePlayUIController.Instance.UpdateCrowdControl(indexes[i], cc.id, cc.turn, ccList[indexes[i]][ccList[indexes[i]].Count - 1].stack);
                }
            }
            else
            {
                List<int> indexes = new List<int>();
                if (isAll)
                {
                    foreach (int key in offenderUnits)
                        indexes.Add(key);
                }
                else indexes.Add(index);

                for (int i = 0; i < indexes.Count; i++)
                {
                    CrowdControl curCC = ccList[indexes[i]][ccIndex];
                    bool isStackSkill = curCC.ControlCC(ccStack);

                    if (isStackSkill == false)
                    {
                        if (curCC.turn < cc.turn)curCC.SetTurn(cc.turn);
                    }
                    else
                    {
                        int curStack = curCC.stack;
                        // CC 발동 시점
                        if (curStack == 0)
                        {
                            // CC 발동
                            curCC.SetTurn(cc.turn);
                            GamePlayUIController.Instance.UpdateCrowdControl(indexes[i], cc.id, cc.turn, 0);
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
                for (int i = 0; i < ccList[key].Count; i++)
                {
                    CrowdControl curCC = ccList[key][i];
                    bool isStackSkill = curCC.ControlCC(0);

                    if (isStackSkill && curCC.stack > 0 && curCC.turn == curCC.GetCCBasicTurn()) continue;

                    bool b = curCC.ProgressTurn();
                    GamePlayUIController.Instance.UpdateCrowdControl(key, curCC.id, curCC.turn, curCC.stack, b);
                    if (b)
                    {
                        ccList[key].RemoveAt(i);
                        i--;
                        if (isStackSkill && curCC.stack > 0)
                        {
                            // 만약에 지웠는데 스택이 남았었다면 턴 초기화 후 그대로 다시 추가
                            curCC.SetTurn(curCC.GetCCBasicTurn());
                            ccList[key].Add(curCC); GamePlayUIController.Instance.UpdateCrowdControl(key, curCC.id, -1, curCC.stack, true);
                        }
                    }
                }
            }
        }

        private bool IsParalysis(int index)
        {
            CrowdControl tmp = ccList[index].Find(cc => cc.cc == CCtype.BLIND);
            return tmp != null;
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
            if (turn != 1) isAttack = DefenderController.Instance.AttackSkillNextTurn();

            GamePlayUIController.Instance.UpdateCharacters();

            if (isAttack)
            {
                // 공격부분
                MonsterSkill skill = DefenderController.Instance.GetAttackSkill();

                // 죽을 유닛 선정
                List<int> aliveIndexes = new List<int>();
                foreach (int key in offenderUnits)
                {
                    if (offenderUnitIsDead[key] == false) aliveIndexes.Add(key);
                }

                if (aliveIndexes.Count == 1)
                {
                    DefenderController.Instance.ResetAttackSkill();
                    OffenderDefeated();
                    return;
                }

                int deadUnit = Random.Range(0, aliveIndexes.Count);

                switch (skill.type)
                {
                    case MonsterSkill.SkillType.AttackAll:
                        OffenderDefeated();
                        return;

                    case MonsterSkill.SkillType.AttackOne:
                        offenderUnitIsDead[aliveIndexes[deadUnit]] = true;
                        break;
                    case MonsterSkill.SkillType.AttackOneStun:
                        offenderUnitIsDead[aliveIndexes[deadUnit]] = true;
                        do
                        {
                            int stunUnit = Random.Range(0, aliveIndexes.Count);
                            if (stunUnit != deadUnit)
                            {
                                List<CrowdControl> ccs = skill.ccList.Keys.ToList();
                                AddCrowdControl(aliveIndexes[stunUnit], ccs[0], skill.ccList[ccs[0]]);
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

            if (round >= 3)
            {
                OffenderController.Instance.ResetDead();
                DefenderController.Instance.ResetDead();
                SceneController.Instance.ChangeScene("Main"); // 씬 이동 임시
            }
            else ReadyRound();
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

            if (OffenderController.Instance.GetAliveCharacterList().Count == 0)
            {
                OffenderController.Instance.ResetDead();
                DefenderController.Instance.ResetDead();
                SceneController.Instance.ChangeScene("Main"); // 씬 이동 임시
            }
            else
            {
                int deadIndex = Random.Range(0, offenderUnits.Length);
                for (int i = 0; i < offenderUnits.Length; i++)
                {
                    if (i != deadIndex) OffenderController.Instance.Alive(offenderUnits[i]);
                }
                DefenderController.Instance.HealBattleMonster(defenderUnit);
                ReadyRound(true);
            }
        }
    }
}
