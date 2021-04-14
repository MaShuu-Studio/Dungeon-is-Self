using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMovementButton : MonoBehaviour
{
    [SerializeField] private string moveScene = "";


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => SceneController.Instance.ChangeScene(moveScene));
        if (isPlay)
            GetComponent<Button>().onClick.AddListener(() => GameStart());
    }

    void GameStart()
    {
        StartCoroutine(WaitSceneMove());
    }


    // 실제 게임에서는 서버에서 매칭관련 정보를 넘겨주면서 게임이 시작되기 때문에 필요 X
    // 임시용 변수와 메서드들
    [SerializeField] private bool isPlay;

    IEnumerator WaitSceneMove()
    {
        while(SceneController.Instance.CurrentScene != "OFFEND" || SceneController.Instance.CurrentScene != "DEFEND")
        {
            yield return null;
        }

        Debug.Log(SceneController.Instance.CurrentScene);
        Debug.Log(GameObject.FindWithTag("Defender"));
        Debug.Log(GameObject.FindWithTag("Offender"));
        GameController.Instance.StartGame();
    }
}
