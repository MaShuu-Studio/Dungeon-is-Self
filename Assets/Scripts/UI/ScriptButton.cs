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
    }
}
