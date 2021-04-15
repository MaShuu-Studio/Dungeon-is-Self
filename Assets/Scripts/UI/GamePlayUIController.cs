using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;

public class GamePlayUIController : MonoBehaviour
{
    UserType type;
    GameProgress progress;

    [Header("READY GAME")]
    [SerializeField] private Transform candidatesTransform;
    [SerializeField] private Transform selectedListTransform;

    [Header("OFFENDER")]
    [SerializeField] private GameObject offenderViewGameObject;
    [SerializeField] private List<GameObject> offenderViews;
    [SerializeField] private List<Toggle> characterToggles;
    [SerializeField] private List<GameObject> characterSkillTrees;

    [Header("DEFENDER")]
    [SerializeField] private GameObject defenderViewGameObject;
    [SerializeField] private List<GameObject> defenderViews;

    void Update()
    {
        if (progress != GameController.Instance.currentProgress)
        {
            ChangeView();
        }
    }

    public void SetUserType()
    {
        type = GameController.Instance.userType;
        if (type == UserType.Offender)
        {
            offenderViewGameObject.SetActive(true);
            defenderViewGameObject.SetActive(false);
        }
        else 
        {
            offenderViewGameObject.SetActive(false);
            defenderViewGameObject.SetActive(true);
        }

    }
    
    public void SetProgress()
    {
        progress = GameController.Instance.currentProgress;
    }

    public void ChangeView()
    {
        SetProgress();

        if (progress == GameProgress.ReadyRound)
            for (int i = 0; i < characterSkillTrees.Count; i++) characterSkillTrees[i].SetActive(characterToggles[i].isOn);
    
    }

    public void SetView()
    {
        foreach (GameObject view in offenderViews)
        {
            view.SetActive(false);
        }

        foreach (GameObject view in defenderViews)
        {
            view.SetActive(false);
        }

        switch (GameController.Instance.currentProgress)
        {
            case GameProgress.ReadyGame:
                ShowAllCandidates();
                if (type == UserType.Offender) offenderViews[0].SetActive(true);
                else defenderViews[0].SetActive(true);
                break;

            case GameProgress.ReadyRound:
                if (type == UserType.Offender) offenderViews[1].SetActive(true);
                else defenderViews[1].SetActive(true);
                break;

            case GameProgress.PlayRound:
                if (type == UserType.Offender) offenderViews[2].SetActive(true);
                else defenderViews[2].SetActive(true);
                break;
        }
    }

    private void ShowAllCandidates()
    {
        Object facePrefab = Resources.Load("Prefab/Frame");

        List<string> candidates = new List<string>();
        if (type == UserType.Defender) MonsterDatabase.Instance.GetAllMonsterCandidatesList(ref candidates);

        foreach (string name in candidates)
        {
            GameObject gameObject = Instantiate(facePrefab) as GameObject;
            gameObject.transform.SetParent(candidatesTransform);
            gameObject.transform.localScale = new Vector3(1,1,1);

            UIIcon uiIcon = gameObject.GetComponent<UIIcon>();
            uiIcon.SetImage(type, name);
        }
    }
}
