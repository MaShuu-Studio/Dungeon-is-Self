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

        private GamePlayUIController gamePlayUI;

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
                    StartCoroutine(ProgressTurn());
                }
            }
        }

        public void SetUserType(UserType type)
        {
            userType = type;
        }

        public void ReadyGame()
        {
            gamePlayUI = GameObject.FindWithTag("UI").GetComponent<GamePlayUIController>();

            currentProgress = GameProgress.ReadyGame;

            isPlay = true;
            round = 0;

            gamePlayUI.SetUserType();
            gamePlayUI.ChangeView();
        }

        public void StartGame()
        {
            DefenderController.Instance.Init();

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
            Debug.Log($"Set Index {units[0]}");
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
            currentProgress = GameProgress.PlayRound;
            turn = 0;
            progressRound = false;
            animationEnd.Clear();
            animationEnd.Add(defenderUnit, true);
            //foreach (int key in offenderUnits)
            //    animationEnd.Add(key, true);

            NextTurn();
        }

        public void ReadyTurn(UserType type, bool ready) // 서버 입장에서는 type 필요
        {
            readyState[(short)type] = ready;
        }

        IEnumerator ProgressTurn()
        {
            MonsterSkill[] monSkills = DefenderController.Instance.DiceRoll(defenderUnit % 10);
            bool defenderOk = (monSkills[0].id == monSkills[1].id);
            Debug.Log($"1: {monSkills[0].name} , 2: {monSkills[1].name} : {defenderOk}");
            //OffenderController.Instance.AllDiceThrow();
            // 순차적으로 공격을 누가 먼저 해서 진행될지 정할 필요 있음.
            if (defenderOk)
            {
                GamePlayUIController.Instance.PlayAnimation(defenderUnit, "Attack");
                animationEnd[defenderUnit] = false;
            }
            bool isLoop = true;
            while (isLoop) // 공격모션 등 모든게 다 지나갈때까지 대기
            {
                isLoop = false;
                yield return null;
                foreach (int key in animationEnd.Keys)
                {
                    Debug.Log($"{key} : {animationEnd[key]}");
                    if (animationEnd[key] == false) isLoop = true;
                }
            }

            Debug.Log("Animation End");

            progressRound = false;
            NextTurn();
        }

        public void AnimationEnd(int index)
        {
            animationEnd[index] = true;
            Debug.Log($"Animation End {index}");
        }

        public void NextTurn()
        {
            turn++;
            readyState[0] = true;
            readyState[1] = false;
        }
    }
}
