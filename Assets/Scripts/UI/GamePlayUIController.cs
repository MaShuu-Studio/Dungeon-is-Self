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
    [SerializeField] private GameObject candidatePrefab;

    private int selectedCandidateNumber = 0;

    [Header("READY ROUND")]
    [SerializeField] private List<CharacterToggle> characterToggles;

    [SerializeField] private GameObject defenderSkillTree;
    [SerializeField] private List<RectTransform> defenderSkillTiers;
    [SerializeField] private GameObject skillIconPrefab;
    [SerializeField] private List<Transform> defenderDescView; // 나중에 없앨 부분 임시
    [SerializeField] private SkillDescription description;
    private List<GameObject> skillIcons = new List<GameObject>();

    [SerializeField] private List<DiceIcon> dices;
    [SerializeField] private RectTransform selectedDice;

    private int selectedDiceNumber;
    private int selectedMonsterNumber;


    [Header("PLAY ROUND")]
    [SerializeField] private SpriteRenderer map;
    private List<CharacterObject> charObjects = new List<CharacterObject>();
    private List<CharacterObject> enemyObjects = new List<CharacterObject>();

    #region Instance
    private static GamePlayUIController instance;
    public static GamePlayUIController Instance
    {
        get
        {
            var obj = FindObjectOfType<GamePlayUIController>();
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
    }
    #endregion

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
                selectedMonsterNumber = 0;
                characterToggles[selectedMonsterNumber].toggle.isOn = true;
                DefenderController.Instance.SelectMonster(selectedMonsterNumber);
                for (int i = 0; i < characterToggles.Count; i++)
                {
                    string name = DefenderController.Instance.selectedMonsterCandidates[i];
                    characterToggles[i].SetFace(type, name);
                }
                SetAllDice();
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

        List<string> candidates = new List<string>();
        if (type == UserType.Defender) MonsterDatabase.Instance.GetAllMonsterCandidatesList(ref candidates);

        foreach (string name in candidates)
        {
            GameObject gameObject = Instantiate(candidatePrefab) as GameObject;
            gameObject.transform.SetParent(candidatesTransform);
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            UIIcon uiIcon = gameObject.GetComponent<UIIcon>();
            uiIcon.SetImage(type, name);
        }
    }

    public void SelectCandidate(string name)
    {
        selectIcons[selectedCandidateNumber].SetImage(type, name);
        if (type == UserType.Defender)
        {
            DefenderController.Instance.SetMonsterCandidate(selectedCandidateNumber, name);
        }

        bool existEmpty = false;
        for (int i = selectedCandidateNumber + 1; i < DefenderController.Instance.selectedMonsterCandidates.Length; i++)
        {
            if (string.IsNullOrEmpty(DefenderController.Instance.selectedMonsterCandidates[i]))
            {
                existEmpty = true;
                SetSelectedCharacterNumber(i);
                break;
            }
        }

        if (existEmpty == false) SetSelectedCharacterNumber(selectedCandidateNumber + 1);
    }

    public void SetSelectedCharacterNumber(int n)
    {
        if (n >= selectIcons.Length) return;

        selectedCandidateNumber = n;
        RectTransform rect = selectIcons[selectedCandidateNumber].GetComponent<RectTransform>();
        selectedArrow.anchoredPosition = rect.anchoredPosition;
    }

    #endregion

    #region Ready Round

    public void SetSkillTree(int index)
    {
        selectedMonsterNumber = index;
        DefenderController.Instance.SelectMonster(selectedMonsterNumber);
        ClearSkillTree();
        SetAllDice();
        // 자동화 코드.
        // 자동화가 아니라 프리팹을 활용해야할지는 고민이 좀 필요할 듯

        if (type == UserType.Defender)
        {
            string monsterName = DefenderController.Instance.monsters[index].name;

            List<MonsterSkill> skills = SkillDatabase.Instance.GetMonsterDices(monsterName);

            List<GameObject>[] skillTierList = new List<GameObject>[3];
            for (int i = 0; i < 3; i++)
                skillTierList[i] = new List<GameObject>();

            for (int i = 0; i < skills.Count; i++)
            {
                int tier = skills[i].tier;
                GameObject obj = Instantiate(skillIconPrefab);
                obj.transform.SetParent(defenderSkillTiers[tier - 1]);
                obj.transform.localScale = new Vector3(1, 1, 1);
                SkillIcon skillIcon = obj.GetComponent<SkillIcon>();
                skillIcon.SetSkill(skills[i]);

                skillTierList[tier - 1].Add(obj);
                skillIcons.Add(obj);
            }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < skillTierList[i].Count; j++)
                {
                    RectTransform rect = skillTierList[i][j].GetComponent<RectTransform>();

                    rect.anchoredPosition = new Vector3(0,
                        -1 * (j * 130 +
                        j * (defenderSkillTiers[i].rect.height - skillTierList[i].Count * 130) / (skillTierList[i].Count - 1)), 0);
                }
        }

    }

    private void ClearSkillTree()
    {
        for (int i = 0; i < skillIcons.Count; i++)
        {
            Destroy(skillIcons[i].gameObject);
        }

        skillIcons.Clear();
    }

    public void ShowDescription(Skill skill, Vector2 pos)
    {
        if (skill == null) return; 
        description.transform.SetParent(defenderDescView[skill.tier - 1]);
        description.SetDescription(skill.name, "", "DESCRIPTION");
        description.ShowDecription(true, pos);
    }

    public void SetAllDice()
    {
        for (selectedDiceNumber = 0; selectedDiceNumber < 6; selectedDiceNumber++)
        {
            SetDiceOnce();
        }
        SelectDice(0);
    }
    public void SetDiceOnce()
    {
        if (type == UserType.Defender)
        {
            MonsterSkill skill = DefenderController.Instance.GetSelectedDice(selectedDiceNumber);
            dices[selectedDiceNumber].SetSkill(skill);
        }
    }

    public void SelectDice(int index)
    {
        RectTransform rect = dices[index].GetComponent<RectTransform>();
        Vector3 pos = new Vector3(rect.anchoredPosition.x - 6.25f, rect.anchoredPosition.y + 6.25f, 0);
        selectedDiceNumber = index;
        selectedDice.anchoredPosition = pos;
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
