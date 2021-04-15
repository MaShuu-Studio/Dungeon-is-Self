using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;

public class GamePlayUIController : MonoBehaviour
{
    UserType type;
    GameProgress progress;

    [Header("COMMONS")]
    [SerializeField] private List<GameObject> gameViews;
    [SerializeField] private List<CharIcon> userCharacters;

    [Header("READY GAME")]
    [SerializeField] private Transform candidatesTransform;
    [SerializeField] private Transform selectedListTransform;
    [SerializeField] private RectTransform selectedArrow;

    [SerializeField] private CharSelectIcon[] selectIcons = new CharSelectIcon[6];
    private int selectedNumber = 0;

    [Header("READY ROUND")]
    [SerializeField] private List<CharacterToggle> characterToggles;
    [SerializeField] private List<GameObject> characterSkillTrees;

    void Update()
    {
        if (progress != GameController.Instance.currentProgress)
        {
            ChangeView();
        }

        for (int i = 0; i < characterToggles.Count; i++)
        {
            string name = DefenderController.Instance.selectedMonsterCandidates[i];
            userCharacters[i].SetImage(type, name);
        }

        if (progress == GameProgress.ReadyRound)
            for (int i = 0; i < characterSkillTrees.Count; i++) characterSkillTrees[i].SetActive(characterToggles[i].toggle.isOn);
    }

    public void SetUserType()
    {
        type = GameController.Instance.userType;
    }
    
    public void SetProgress()
    {
        progress = GameController.Instance.currentProgress;
    }

    public void ChangeView()
    {
        SetProgress();
        foreach (GameObject view in gameViews)
        {
            view.SetActive(false);
        }

        switch (GameController.Instance.currentProgress)
        {
            case GameProgress.ReadyGame:
                gameViews[0].SetActive(true);
                ShowAllCandidates();
                break;

            case GameProgress.ReadyRound:
                gameViews[1].SetActive(true);
                characterToggles[0].toggle.isOn = true;
                for (int i = 0; i < characterToggles.Count; i++)
                {
                    string name = DefenderController.Instance.selectedMonsterCandidates[i];
                    characterToggles[i].SetFace(type, name);
                }
                break;

            case GameProgress.PlayRound:
                gameViews[2].SetActive(true);
                break;
        }
    }

    private void ShowAllCandidates()
    {
        foreach (Transform child in candidatesTransform)
        {
            Destroy(child.gameObject);
        }

        Object facePrefab = Resources.Load("Prefab/Candidate Monster");

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

    public void SelectCandidate(string name)
    {
        selectIcons[selectedNumber].SetImage(type, name);
        if (type == UserType.Defender)
        {
            DefenderController.Instance.SetMonsterRoster(selectedNumber, name);
        }
    }

    public void SetSelectedCharacterNumber(int n, Vector3 pos)
    {
        selectedNumber = n;
        selectedArrow.anchoredPosition = pos;
    }

    public void Alert()
    {
        if (GameController.Instance.currentProgress == GameProgress.ReadyGame)
        {
            Debug.Log("Please Complete Setting Candidate");
        }
    }
}
