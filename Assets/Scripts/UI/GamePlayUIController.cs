using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Data;

public class GamePlayUIController : MonoBehaviour
{
    UserType type;
    GameProgress progress;

    [Header("COMMONS")]
    [SerializeField] private List<GameObject> gameViews;
    [SerializeField] private List<CharIcon> userCharacters;
    [SerializeField] private CustomButton readyButton;

    [Header("READY GAME")]
    [SerializeField] private Transform candidatesTransform;
    [SerializeField] private Transform selectedListTransform;
    [SerializeField] private RectTransform selectedArrow;
    [SerializeField] private CharSelectIcon[] selectIcons = new CharSelectIcon[6];
    [SerializeField] private GameObject candidatePrefab;

    private int selectedCandidateIndex = 0;

    [Header("READY ROUND")]
    [SerializeField] private List<CharacterToggle> characterToggles;

    [SerializeField] private GameObject defenderSkillTree;
    [SerializeField] private List<RectTransform> defenderSkillTiers;
    [SerializeField] private GameObject diceSkillIconPrefab;
    [SerializeField] private SkillDescription description;
    private List<GameObject> diceSkillIcons = new List<GameObject>();

    [SerializeField] private List<DiceIcon> dices;
    [SerializeField] private RectTransform selectedDice;
    [SerializeField] private List<SkillIcon> defenderAttackSkills;
    [SerializeField] private RectTransform selectedAttackSkill;
    [SerializeField] private Slider costSlider;
    [SerializeField] private Text costText;

    private int selectedDiceIndex;
    private int selectedMonsterIndex;


    [Header("PLAY ROUND")]
    [SerializeField] private Transform mapParent;
    [SerializeField] private Transform characterParent;
    private List<CharacterObject> charObjects = new List<CharacterObject>();
    private List<CharacterObject> enemyObjects = new List<CharacterObject>();
    private GameObject mapObject;

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

        if (progress == GameProgress.ReadyRound)
        {
            for (int i = 0; i < characterToggles.Count; i++)
            {
                string name = (type == UserType.Defender) ? DefenderController.Instance.selectedMonsterCandidates[i] : OffenderController.Instance.selectedCharacterCandidates[i];
                userCharacters[i].SetImage(type, name);
            }
        }

        if (progress == GameProgress.PlayRound)
        {
            if (GameController.Instance.progressRound) readyButton.SetButtonInteract(false);
            else readyButton.SetButtonInteract(true);
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
                selectedMonsterIndex = 0;
                characterToggles[selectedMonsterIndex].toggle.isOn = true;
                DefenderController.Instance.SelectMonster(selectedMonsterIndex);
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
        else if (GameController.Instance.currentProgress == GameProgress.ReadyRound)
        {
            Debug.Log("주사위는 같은 스킬을 두 가지보다 많이 넣을 수 없습니다.");
            Debug.Log("코스트보다 많이 넣을 수 없습니다.");
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
        else CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref candidates);

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
        int size = 0;
        selectIcons[selectedCandidateIndex].SetImage(type, name);
        if (type == UserType.Defender)
        {
            DefenderController.Instance.SetMonsterCandidate(selectedCandidateIndex, name);
            size = DefenderController.Instance.selectedMonsterCandidates.Length;
        }
        else
        {
            OffenderController.Instance.SetCharacterCandidate(selectedCandidateIndex, name);
            size = OffenderController.Instance.selectedCharacterCandidates.Length;
        }

        bool existEmpty = false;
        for (int i = selectedCandidateIndex + 1; i < size; i++)
        {
            if ((type == UserType.Defender && string.IsNullOrEmpty(DefenderController.Instance.selectedMonsterCandidates[i]))
             || (type == UserType.Offender && string.IsNullOrEmpty(OffenderController.Instance.selectedCharacterCandidates[i])))
            {
                existEmpty = true;
                SetSelectedCharacterIndex(i);
                break;
            }
        }

        if (existEmpty == false) SetSelectedCharacterIndex(selectedCandidateIndex + 1);
    }

    public void SetSelectedCharacterIndex(int n)
    {
        if (n >= selectIcons.Length) return;

        selectedCandidateIndex = n;
        RectTransform rect = selectIcons[selectedCandidateIndex].GetComponent<RectTransform>();
        selectedArrow.anchoredPosition = rect.anchoredPosition;
    }

    #endregion

    #region Ready Round

    public void SetSkillTree(int index)
    {
        selectedMonsterIndex = index;
        DefenderController.Instance.SelectMonster(selectedMonsterIndex);
        ClearSkillTree();
        SetAllDice();
        // 자동화 코드.
        // 자동화가 아니라 프리팹을 활용해야할지는 고민이 좀 필요할 듯

        string name;
        if (type == UserType.Defender)
        {
            name = DefenderController.Instance.monsters[index].name;
            List<MonsterSkill> dices = SkillDatabase.Instance.GetMonsterDices(name);

            List<GameObject>[] diceTierList = new List<GameObject>[3];
            for (int i = 0; i < 3; i++)
                diceTierList[i] = new List<GameObject>();

            for (int i = 0; i < dices.Count; i++)
            {
                int tier = dices[i].tier;
                GameObject obj = Instantiate(diceSkillIconPrefab);
                obj.transform.SetParent(defenderSkillTiers[tier - 1]);
                obj.transform.localScale = new Vector3(1, 1, 1);
                SkillIcon diceIcon = obj.GetComponent<SkillIcon>();
                diceIcon.SetSkill(dices[i], (GameController.Instance.round >= tier));


                diceTierList[tier - 1].Add(obj);
                diceSkillIcons.Add(obj);
            }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < diceTierList[i].Count; j++)
                {
                    RectTransform rect = diceTierList[i][j].GetComponent<RectTransform>();

                    float y = 0;
                    if (diceTierList[i].Count > 1) y = -1 * (j * 100 + j * (defenderSkillTiers[i].rect.height - diceTierList[i].Count * 100) / (diceTierList[i].Count - 1));
                    rect.anchoredPosition = new Vector3(0, y, 0);
                }

            List<MonsterSkill> attackSkills = SkillDatabase.Instance.GetMonsterAttackSkills(name, GameController.Instance.round);
            MonsterSkill charAttackSkill = DefenderController.Instance.GetAttackSkill();
            for (int i = 0; i < defenderAttackSkills.Count; i++)
            {
                defenderAttackSkills[i].SetSkill(attackSkills[i]);
                if (charAttackSkill.id == attackSkills[i].id) SelectAttackSkill(i);
            }

        }
        else
        {
            name = OffenderController.Instance.character[index]._role;
            List<CharacterSkill> dices = SkillDatabase.Instance.GetCharacterDices(name);

            List<GameObject>[] diceTierList = new List<GameObject>[3];
            for (int i = 0; i < 3; i++)
                diceTierList[i] = new List<GameObject>();

            for (int i = 0; i < dices.Count; i++)
            {
                int tier = dices[i].tier;
                GameObject obj = Instantiate(diceSkillIconPrefab);
                obj.transform.SetParent(defenderSkillTiers[tier - 1]);
                obj.transform.localScale = new Vector3(1, 1, 1);
                SkillIcon diceIcon = obj.GetComponent<SkillIcon>();
                diceIcon.SetSkill(dices[i], (GameController.Instance.round >= tier));


                diceTierList[tier - 1].Add(obj);
                diceSkillIcons.Add(obj);
            }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < diceTierList[i].Count; j++)
                {
                    RectTransform rect = diceTierList[i][j].GetComponent<RectTransform>();

                    float y = 0;
                    if (diceTierList[i].Count > 1) y = -1 * (j * 100 + j * (defenderSkillTiers[i].rect.height - diceTierList[i].Count * 100) / (diceTierList[i].Count - 1));
                    rect.anchoredPosition = new Vector3(0, y, 0);
                }
        }    
    }

    private void ClearSkillTree()
    {
        for (int i = 0; i < diceSkillIcons.Count; i++)
        {
            Destroy(diceSkillIcons[i].gameObject);
        }

        diceSkillIcons.Clear();
    }

    public void ShowDescription(Skill skill)
    {
        if (skill == null) return;
        description.SetDescription(skill.name, "", "DESCRIPTION");
    }

    public void SetAllDice()
    {
        for (int i = 0; i < 6; i++)
        {
            SetDiceOnce();
        }
        SelectDice(0);
    }
    public void SetDiceOnce()
    {
        if (type == UserType.Defender)
        {
            MonsterSkill skill = DefenderController.Instance.GetSelectedDice(selectedDiceIndex);
            dices[selectedDiceIndex++].SetSkill(skill);

            if (selectedDiceIndex >= dices.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);

            int totalCost = DefenderController.MAX_COST - DefenderController.Instance.GetDiceCost();
            costSlider.value = (totalCost >= 0) ? totalCost : 0;
            costText.text = totalCost.ToString();
        }
    }
    public void SetDiceOnce(MonsterSkill skill)
    {
        if (type == UserType.Defender)
        {
            bool b = DefenderController.Instance.SetDice(selectedDiceIndex, skill);
            if (b == false)
            {
                Alert();
                return;
            }

            dices[selectedDiceIndex++].SetSkill(skill);

            if (selectedDiceIndex >= dices.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);

            int totalCost = DefenderController.MAX_COST - DefenderController.Instance.GetDiceCost();
            costSlider.value = (totalCost >= 0) ? totalCost : 0;
            costText.text = totalCost.ToString();
        }
    }

    public void SelectDice(int index)
    {
        RectTransform rect = dices[index].GetComponent<RectTransform>();
        Vector3 pos = new Vector3(rect.anchoredPosition.x - 6.25f, rect.anchoredPosition.y + 6.25f, 0);
        selectedDiceIndex = index;
        selectedDice.anchoredPosition = pos;
    }

    public void SetAttackSkill(int index, MonsterSkill skill)
    {
        DefenderController.Instance.SetAttackSkill(skill);
        SelectAttackSkill(index);
    }

    public void SelectAttackSkill(int index)
    {
        RectTransform rect = defenderAttackSkills[index].GetComponent<RectTransform>();
        Vector3 pos = new Vector3(0, rect.anchoredPosition.y + 6.25f, 0);
        selectedAttackSkill.anchoredPosition = pos;
    }

    #endregion

    #region Play Round

    private void SetCharacters()
    {
        Object prefab;
        GameObject obj;
        string charPath = (type == UserType.Defender) ? "Prefab/Monsters/" : "Prefab/Classes/";
        string enemyPath = (type == UserType.Defender) ? "Prefab/Classes/" : "Prefab/Monsters/";

        string monsterName = DefenderController.Instance.GetMonsterRoster();
        if (type == UserType.Defender)
        {
            {
                // 자신 캐릭터 소환
                prefab = Resources.Load(charPath + monsterName);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
                obj.transform.SetParent(characterParent);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.defenderUnit);
                charObjects.Add(character);
            }

            // 적 캐릭터 소환
            for (int i = 0; i < 3; i++)
            {
                prefab = Resources.Load(enemyPath + "Knight");
                obj = Instantiate(prefab) as GameObject;
                obj.transform.position = new Vector3(obj.transform.position.x + 2f * i, obj.transform.position.y, 0);
                obj.transform.SetParent(characterParent);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.offenderUnits[i]);
                enemyObjects.Add(character);
            }

            prefab = Resources.Load("Prefab/Maps/" + "Forest");
            mapObject = Instantiate(prefab) as GameObject;
            mapObject.transform.SetParent(mapParent);
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

    public void PlayAnimation(int index, string anim)
    {
        foreach (CharacterObject c in charObjects)
        {
            if (c.CheckIndex(index))
            {
                c.SetAnimation(anim);
                return;
            }
        }

        foreach (CharacterObject c in enemyObjects)
        {
            if (c.CheckIndex(index))
            {
                c.SetAnimation(anim);
                return;
            }
        }
    }
    #endregion
}
