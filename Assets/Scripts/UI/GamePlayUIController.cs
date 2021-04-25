using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Data;

public class GamePlayUIController : MonoBehaviour
{
    #region Value
    private UserType type;
    private GameProgress progress;

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
    [SerializeField] private RectTransform offenderSkillTree;
    [SerializeField] private Text offenderSkillPointText;

    [Space]
    [SerializeField] private GameObject diceSkillIconPrefab;
    private List<SkillIcon> diceSkillIcons = new List<SkillIcon>();

    [SerializeField] private List<DiceIcon> dices;
    [SerializeField] private RectTransform selectedDice;

    [SerializeField] private SkillDescription description;

    [Space]
    [SerializeField] private GameObject defenderSpecialView;
    [SerializeField] private List<SkillIcon> defenderAttackSkills;
    [SerializeField] private RectTransform selectedAttackSkill;
    [SerializeField] private Slider costSlider;
    [SerializeField] private Text costText;

    [Space]
    [SerializeField] private GameObject offenderSpecialView;
    [SerializeField] private List<RosterIcon> offenderRosters;


    private int selectedDiceIndex;
    private int selectedCharacterIndex;


    [Header("PLAY ROUND")]
    [SerializeField] private Transform mapParent;
    [SerializeField] private Transform characterParent;
    private List<CharacterObject> charObjects = new List<CharacterObject>();
    private List<CharacterObject> enemyObjects = new List<CharacterObject>();
    private GameObject mapObject;
    #endregion

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

        foreach (GameObject view in gameViews)
        {
            view.SetActive(false);
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
                if (type == UserType.Defender)
                {
                    defenderSkillTree.SetActive(true);
                    offenderSkillTree.gameObject.SetActive(false);
                    defenderSpecialView.SetActive(true);
                    offenderSpecialView.SetActive(false);
                    // 유닛 선택시 Defender에서 몬스터가 죽었는지 체크해야함.
                    // 기존에 선택했던 유닛부터 다시 세팅할 수 있게 해줘야 함.
                    characterToggles[0].toggle.isOn = true;
                }
                else
                {
                    defenderSkillTree.SetActive(false);
                    offenderSkillTree.gameObject.SetActive(true);
                    defenderSpecialView.SetActive(false);
                    offenderSpecialView.SetActive(true);
                    // 유닛 선택시 Offender에서 유닛이 죽었는지 체크해야 함.
                    // 기존에 선택했던 유닛부터 다시 세팅할 수 있게 해줘야 함.
                    characterToggles[0].toggle.isOn = true;

                    for (int i = 0; i < 3; i++)
                    {
                        OffenderController.Instance.SelectCharacter(i);
                        SetOffenderRoster(i);
                    }
                    OffenderController.Instance.SelectCharacter(0);
                }

                for (int i = 0; i < characterToggles.Count; i++)
                {
                    string name = (type == UserType.Defender) ? DefenderController.Instance.selectedMonsterCandidates[i] : OffenderController.Instance.selectedCharacterCandidates[i];
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
        ClearSkillTree();
        SetAllDice();

        // 자동화 코드.
        // 자동화가 아니라 프리팹을 활용해야할지는 고민이 좀 필요할 듯
        // 공격자와 방어자를 나중에 합칠 생각

        selectedCharacterIndex = index;
        string name;
        if (type == UserType.Defender)
        {
            DefenderController.Instance.SelectMonster(selectedCharacterIndex);

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
                diceSkillIcons.Add(diceIcon);
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
            OffenderController.Instance.SelectCharacter(selectedCharacterIndex);

            name = OffenderController.Instance.characters[index]._role;
            List<CharacterSkill> dices = SkillDatabase.Instance.GetCharacterDices(name);

            int maxTier = OffenderController.Instance.GetMaxTier();
            List<GameObject>[] diceTierList = new List<GameObject>[maxTier];
            for (int i = 0; i < maxTier; i++)
                diceTierList[i] = new List<GameObject>();

            for (int i = 0; i < dices.Count; i++)
            {
                int tier = dices[i].tier;
                GameObject obj = Instantiate(diceSkillIconPrefab);
                obj.transform.SetParent(offenderSkillTree);
                obj.transform.localScale = new Vector3(1, 1, 1);
                SkillIcon diceIcon = obj.GetComponent<SkillIcon>();
                diceIcon.SetSkill(dices[i], OffenderController.Instance.IsSkillGotten(i));

                diceTierList[tier - 1].Add(obj);
                diceSkillIcons.Add(diceIcon);
            }

            for (int i = 0; i < maxTier; i++)
                for (int j = 0; j < diceTierList[i].Count; j++)
                {
                    RectTransform rect = diceTierList[i][j].GetComponent<RectTransform>();

                    float x = 0;
                    x = i * (100 + ((offenderSkillTree.rect.width - 100 * maxTier) / (maxTier - 1)));
                    float y = 0;
                    if (diceTierList[i].Count > 1) y = -1 * (j * 100 + j * (offenderSkillTree.rect.height - diceTierList[i].Count * 100) / (diceTierList[i].Count - 1));
                    rect.anchoredPosition = new Vector3(x, y, 0);
                }

            offenderSkillPointText.text = "SKILL POINT: " + OffenderController.Instance.GetSkillPoint().ToString();
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
        else
        {
            CharacterSkill skill = OffenderController.Instance.GetSelectedDice(selectedDiceIndex);
            dices[selectedDiceIndex++].SetSkill(skill);

            if (selectedDiceIndex >= dices.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);
        }
    }
    public void SetDiceOnce(Skill skill, bool isOn = true)
    {
        if (isOn == false && type == UserType.Offender)
        {
            int index = OffenderController.Instance.LearnSkill(skill);
            if (index != -1)
            {
                diceSkillIcons[index].SetOnOff(true);
                offenderSkillPointText.text = "SKILL POINT: " + OffenderController.Instance.GetSkillPoint().ToString();
            }
            else
            {
                Debug.Log("스킬포인트가 모자랍니다.");
            }
            return;
        }

        if (type == UserType.Defender)
        {
            bool b = DefenderController.Instance.SetDice(selectedDiceIndex, skill as MonsterSkill);
            if (b == false)
            {
                Alert();
                return;
            }

            dices[selectedDiceIndex++].SetSkill(skill as MonsterSkill);

            if (selectedDiceIndex >= dices.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);

            int totalCost = DefenderController.MAX_COST - DefenderController.Instance.GetDiceCost();
            costSlider.value = (totalCost >= 0) ? totalCost : 0;
            costText.text = totalCost.ToString();
        }
        else
        {
            bool b = OffenderController.Instance.SetDice(selectedDiceIndex, skill as CharacterSkill);
            if (b == false)
            {
                Alert();
                return;
            }

            dices[selectedDiceIndex++].SetSkill(skill as CharacterSkill);

            if (selectedDiceIndex >= dices.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);
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

    public void SetOffenderRoster(int index)
    {
        string name = OffenderController.Instance.SelectRoster(index);

        if (string.IsNullOrEmpty(name) == false)
        {
            offenderRosters[index].SetImage(UserType.Offender, name);
            offenderRosters[index].SetNumber(OffenderController.Instance.roster[index]);
        }
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
