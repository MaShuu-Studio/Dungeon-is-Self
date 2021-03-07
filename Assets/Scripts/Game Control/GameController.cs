using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum GameProgress { Ready, GamePlay };

    private DefenderController defender;
    private OffenderController offender;

    private bool is_Play;
    GameProgress currentProgress;
    int turn;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        is_Play = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        defender = GameObject.FindWithTag("Defender").GetComponent<DefenderController>();
        offender = GameObject.FindWithTag("Offender").GetComponent<OffenderController>();
    }
}
