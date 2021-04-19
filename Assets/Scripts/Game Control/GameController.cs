using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        private int round;
        private int turn;
        private bool[] readyState = new bool[2];
        public bool progressRound { get; private set; }

        private int defenderUnit;
        private List<int> offenderUnit;

        

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
                    StartCoroutine(ProgressTurn());
                    progressRound = true;
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
                currentProgress = GameProgress.ReadyRound;
            }
        }

        public void SelectUnit(UserType type, List<int> units) // 서버 입장에서는 type 필요
        {
            if (type == UserType.Defender) defenderUnit = units[0];
            else offenderUnit = new List<int>(units);

        }

        public void StartRound()
        {
            currentProgress = GameProgress.PlayRound;
            turn = 0;
            progressRound = false;
            NextTurn();
        }

        public void ReadyTurn(UserType type, bool ready) // 서버 입장에서는 type 필요
        {
            readyState[(short)type] = ready;
        }

        IEnumerator ProgressTurn()
        {
            MonsterSkill[] monSkills = DefenderController.Instance.DiceRoll(defenderUnit);
            bool b = (monSkills[0].id == monSkills[1].id);
            Debug.Log($"1: {monSkills[0].name} , 2: {monSkills[1].name} : {b}");
            //OffenderController.Instance.AllDiceThrow();
            //while (true) // 공격모션 등 모든게 다 지나갈때까지 대기
            {
                yield return null;
            }
            progressRound = false;
            NextTurn();
        }

        public void NextTurn()
        {
            turn++;
            readyState[0] = true;
            readyState[1] = false;
        }

        #region GUI

        private void OnGUI()
        {
            /*
            if (GUI.Button(new Rect(10, 10, 50, 50), "TITLE"))
            {
                SceneController.Instance.ChangeScene("Title");
            }
            if (GUI.Button(new Rect(80, 10, 50, 50), "OFFEND"))
            {
                SceneController.Instance.ChangeScene("Offend");
            }
            if (GUI.Button(new Rect(150, 10, 50, 50), "DEFEND"))
            {
                SceneController.Instance.ChangeScene("DEFEND");
            }
            */
            if (SceneController.Instance.CurrentScene == "OFFEND" || SceneController.Instance.CurrentScene == "DEFEND")
            {
                /*
                if (GUI.Button(new Rect(10, 80, 50, 50), "Ready"))
                {
                    StartGame();
                }*/

                if (GUI.Button(new Rect(80, 80, 50, 50), "Ready"))
                {
                    ReadyRound();
                }

                if (GUI.Button(new Rect(150, 80, 50, 50), "Round"))
                {
                    StartRound();
                }
            }
        }
        #endregion
    }
}
