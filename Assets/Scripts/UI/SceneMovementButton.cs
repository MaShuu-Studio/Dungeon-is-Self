using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;

public class SceneMovementButton : MonoBehaviour
{
    [SerializeField] private string moveScene = "";


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => SceneController.Instance.ChangeScene(moveScene));
        GetComponent<Button>().onClick.AddListener(() => SetUserType());
    }



    // 실제 게임에서는 서버에서 매칭관련 정보를 넘겨주면서 게임이 시작되기 때문에 필요 X
    // 임시용 변수와 메서드들
    [SerializeField] private UserType userType = UserType.Offender;
    void SetUserType()
    {
        GameController.Instance.SetUserType(userType);
    }

}
