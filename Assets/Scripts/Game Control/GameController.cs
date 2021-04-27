using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;
using System;
using System.Linq;

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
        private Dictionary<int, bool> animationEnd = new Dictionary<int, bool>();

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
        }

        public void ReadyGame()
        {
            currentProgress = GameProgress.ReadyGame;

            isPlay = true;
            round = 0;

            GamePlayUIController.Instance.SetUserType();
            GamePlayUIController.Instance.ChangeView();
        }

        public void StartGame()
        {
            // Defender, Offender의 Init (selected Candidates에서 목록 뽑아옴)
            // Init 전에 Candidate 생성

            /*if (userType == UserType.Defender) DefenderController.Instance.Init();
            else OffenderController.Instance.Init();*/

            if (userType == UserType.Defender)
            {
                List<string> offenderBot = new List<string>();
                CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref offenderBot);
                Debug.Log(offenderBot);

                for (int i = 0; i < 6; i++)
                {
                    int a = UnityEngine.Random.Range(0, offenderBot.Count);
                    OffenderController.Instance.SetCharacterCandidate(i, offenderBot[a]);
                }
            }
            else
            {
                List<string> defenderBot = new List<string>();
                MonsterDatabase.Instance.GetAllMonsterCandidatesList(ref defenderBot);
                for (int i = 0; i < 6; i++)
                {
                    int a = UnityEngine.Random.Range(0, defenderBot.Count);
                    DefenderController.Instance.SetMonsterCandidate(i, defenderBot[a]);
                }
            }
            DefenderController.Instance.Init();
            OffenderController.Instance.Init();

            ReadyRound();
        }

        public void ReadyRound()
        {
            if (isPlay)
            {
                round++;
                animationEnd.Clear();
                currentProgress = GameProgress.ReadyRound;
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

            GamePlayUIController.Instance.ShowSelectedRoster(units);
        }

        public void StartRound()
        {
            // 로스터 세팅
            // 주사위 세팅

            currentProgress = GameProgress.PlayRound;
            turn = 0;
            progressRound = false;
            animationEnd.Clear();

            // 선공 확인해서 순서 조정
            animationEnd.Add(defenderUnit, true);
            foreach (int key in offenderUnits)
                animationEnd.Add(key, true);

            if (userType == UserType.Defender) GamePlayUIController.Instance.ShowSelectedRoster(defenderUnit);
            else if (userType == UserType.Offender) GamePlayUIController.Instance.ShowSelectedRoster(offenderUnits);

            NextTurn();
        }

        public void ReadyTurn(UserType type, bool ready) // 서버 입장에서는 type 필요
        {
            readyState[(short)type] = ready;
        }

        private void ProgressTurn()
        {
            List<MonsterSkill> monSkills = DefenderController.Instance.DiceRoll(defenderUnit % 10);
            List<CharacterSkill> charSkills = OffenderController.Instance.DiceRoll(offenderUnits);

            StartCoroutine(Battle(monSkills, charSkills));
        }

        IEnumerator Battle(List<MonsterSkill> monSkills, List<CharacterSkill> charSkills)
        {
            bool defenderOk = (monSkills[0].id == monSkills[1].id);
            Debug.Log($"1: {monSkills[0].name} , 2: {monSkills[1].name} : {defenderOk}");
            Debug.Log($"1: {charSkills[0].name}, 2: {charSkills[1].name}, 3: {charSkills[2].name}");

            // 순차적으로 공격을 누가 먼저 해서 진행될지 정할 필요 있음.
            if (defenderOk) animationEnd[defenderUnit] = false;
            for (int i = 1; i < animationEnd.Count; i++)
            {
                // 전투불능 상태 체크해서 Animation End 이용
                animationEnd[offenderUnits[i - 1]] = false;
            }

            // 전투 시 전투불능 확인해서 그 때 그 때 바꿔줘야함
            List<int> keys = animationEnd.Keys.ToList<int>();
            for (int i = 0; i < animationEnd.Count; i++)
                if (animationEnd[keys[i]] == false)
                {
                    if ((keys[i] / 10) == 2 && defenderOk == false) continue;
                    GamePlayUIController.Instance.PlayAnimation(keys[i], "Attack");

                    while (animationEnd[keys[i]] == false) yield return null;
                }

            progressRound = false;
            NextTurn();
        }

        public void AnimationEnd(int index)
        {
            animationEnd[index] = true;
        }

        public void NextTurn()
        {
            GamePlayUIController.Instance.SetTurn(++turn);
            readyState[0] = false;
            readyState[1] = false;
            if (userType == UserType.Defender) ReadyTurn(UserType.Offender, true);
            else if (userType == UserType.Offender) ReadyTurn(UserType.Defender, true);
        }
    }
}
