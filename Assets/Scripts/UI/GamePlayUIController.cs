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

    [Header("PLAY ROUND")]
    [SerializeField] private SpriteRenderer map;
    private List<CharacterObject> charObjects = new List<CharacterObject>();
    private List<CharacterObject> enemyObjects = new List<CharacterObject>();

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

    #region Basic
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
                ClearCharacters();
                SetCharacters();
                break;
        }
    }
    
    public void Alert()
    {
        if (GameController.Instance.currentProgress == GameProgress.ReadyGame)
        {
            Debug.Log("Please Complete Setting Candidate");
        }
    }
    #endregion

    #region Ready Game
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

    #endregion

    #region Play Round

    private void SetCharacters()
    {
        Object prefab;
        GameObject obj;
        string charPath = (type == UserType.Defender) ? "Prefab/Monsters/" : "Prefab/Classes/";
        string enemyPath = (type == UserType.Defender) ? "Prefab/Classes/" : "Prefab/Monsters/";
        if (type == UserType.Defender)
        {
            // 자신 캐릭터 소환
            prefab = Resources.Load(charPath + "Dokkaebi");
            obj = Instantiate(prefab) as GameObject;
            obj.transform.position = new Vector3(-5, 0, 0);
            obj.transform.SetParent(map.transform);
            charObjects.Add(obj.GetComponent<CharacterObject>());

            // 적 캐릭터 소환
            for (int i = 0; i < 3; i++)
            {
                prefab = Resources.Load(enemyPath + "Knight");
                obj = Instantiate(prefab) as GameObject;
                obj.transform.position = new Vector3(2 + 2.5f * i, -1.5f, 0);
                obj.transform.SetParent(map.transform);
                enemyObjects.Add(obj.GetComponent<CharacterObject>());
            }
        }
        else
        {

        }

        for (int i = 0; i < charObjects.Count; i++)
            charObjects[i].SetFlip(false);
        for (int i = 0; i < enemyObjects.Count; i++)
            enemyObjects[i].SetFlip(true);
    }

    private void ClearCharacters()
    {
        for (int i = 0; i < charObjects.Count; i++)
            Destroy(charObjects[i]);
        for (int i = 0; i < enemyObjects.Count; i++)
            Destroy(enemyObjects[i]);

        charObjects.Clear();
        enemyObjects.Clear();
    }
    #endregion
}
