using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum GameProgress { Wait, Ready, GamePlay };

    private DefenderController defender;
    private OffenderController offender;

    private bool isPlay;
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

    public void StartGame()
    {
        defender = GameObject.FindWithTag("Defender").GetComponent<DefenderController>();
        offender = GameObject.FindWithTag("Offender").GetComponent<OffenderController>();

        currentProgress = GameProgress.Wait;
        curRoundMonster = new List<Monster>();
        //curRoundCharacter = new List<Character>();

        isPlay = true;
        round = 0;
        
        offender.SetView();
    }

    public void ReadyRound()
    {
        if (isPlay)
        {
            round++;
            currentProgress = GameProgress.Ready;
            offender.SetView();

            curRoundMonster.Clear();
            //curRoundCharacter.Clear();            

            defender.SetMonsterCandidate(4);
            defender.SetRound(round);
        }
    }

    public void StartRound()
    {
        currentProgress = GameProgress.GamePlay;
        offender.SetView();
    }

#region GUI

    private void OnGUI()
    {
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

        if (SceneController.Instance.CurrentScene == "OFFEND" || SceneController.Instance.CurrentScene == "DEFEND")
        {
            if (GUI.Button(new Rect(10, 80, 50, 50), "Ready"))
            {
                StartGame();
            }

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
