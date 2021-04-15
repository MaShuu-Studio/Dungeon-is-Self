using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameControl
{
    public enum GameProgress { ReadyGame, ReadyRound, PlayRound };
    public enum UserType { Offender, Defender };

    public class GameController : MonoBehaviour
    {

        private DefenderController defender;
        private OffenderController offender;
        private UIController uIController;

        private bool isPlay;
        public UserType userType { get; private set; }

        public GameProgress currentProgress { get; private set; }
        private int round;

        private List<Monster> curRoundMonster;
        //private List<Character> curRoundCharacter;

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

        // Start is called before the first frame update
        void Start()
        {
            isPlay = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUserType(UserType type)
        {
            userType = type;
        }

        public void StartGame()
        {
            defender = GameObject.FindWithTag("Defender").GetComponent<DefenderController>();
            offender = GameObject.FindWithTag("Offender").GetComponent<OffenderController>();
            uIController = GameObject.FindWithTag("UI").GetComponent<UIController>();

            currentProgress = GameProgress.ReadyGame;
            curRoundMonster = new List<Monster>();
            //curRoundCharacter = new List<Character>();

            isPlay = true;
            round = 0;

            uIController.SetUserType();
            uIController.SetView();
        }

        public void ReadyRound()
        {
            if (isPlay)
            {
                round++;
                currentProgress = GameProgress.ReadyRound;
               uIController.SetView();

                curRoundMonster.Clear();
                //curRoundCharacter.Clear();         
            }
        }

        public void StartRound()
        {
            currentProgress = GameProgress.PlayRound;
            uIController.SetView();
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
