﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Data;

public class GamePlayUIController : MonoBehaviour
{
    #region Value
    private UserType type;
    private UserType enemyType;
    private GameProgress progress;

    [Header("COMMONS")]
    [SerializeField] private List<GameObject> gameViews;
    [SerializeField] private CustomButton readyButton;
    [SerializeField] private AlertObject alert;

    [Space]
    [SerializeField] private List<CharIcon> userRosters;
    [SerializeField] private List<CharIcon> enemyRosters;
    [SerializeField] private List<RectTransform> rosterSelected;
    [SerializeField] private List<RectTransform> enemyRosterSelected;

    [Header("READY GAME")]
    // 후보 선택
    [SerializeField] private Transform candidatesTransform;
    [SerializeField] private Transform selectedListTransform;
    [SerializeField] private RectTransform selectedArrow;
    [SerializeField] private CharSelectIcon[] selectIcons = new CharSelectIcon[6];
    [SerializeField] private GameObject candidatePrefab;

    private int selectedCandidateIndex = 0;

    [Header("READY ROUND")]
    // 가장 기본 부분
    [SerializeField] private Text roundText;
    [SerializeField] private List<CharacterToggle> characterToggles;

    [Space]
    // 스킬트리의 바탕이 되는 부분
    [SerializeField] private GameObject defenderSkillTree;
    [SerializeField] private List<RectTransform> defenderSkillTiers;
    [SerializeField] private RectTransform offenderSkillTree;
    [SerializeField] private Text offenderSkillPointText;

    [Space]
    // 주사위 설정
    [SerializeField] private GameObject diceSkillIconPrefab;
    private List<SkillIcon> diceSkillIcons = new List<SkillIcon>();

    [SerializeField] private List<DiceIcon> diceIcons;
    [SerializeField] private RectTransform selectedDice;

    [SerializeField] private SkillDescription description;

    [Space]
    // 디펜더 유닛 추가 정보
    [SerializeField] private Text monsterNameText;
    [SerializeField] private Text monsterHpText;
    [SerializeField] private GameObject defenderSpecialView;
    [SerializeField] private List<SkillIcon> defenderAttackSkills;
    [SerializeField] private RectTransform selectedAttackSkill;
    [SerializeField] private Slider costSlider;
    [SerializeField] private Text costText;

    [Space]
    // 공격자 유닛 추가 정보
    [SerializeField] private GameObject offenderSpecialView;
    [SerializeField] private List<RosterIcon> offenderRosters;


    private int selectedDiceIndex;
    private int selectedCharacterIndex;


    [SerializeField] private Text turnText;
    [Header("PLAY ROUND")]
    [SerializeField] private GameObject playRoundView;
    [SerializeField] private Transform mapParent;
    [SerializeField] private Transform characterParent;
    private List<CharacterObject> charObjects = new List<CharacterObject>();
    private List<CharacterObject> enemyObjects = new List<CharacterObject>();
    private GameObject mapObject;

    [SerializeField] private GameObject dicePrefab;
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

        if (type == UserType.Defender) enemyType = UserType.Offender;
        else enemyType = UserType.Defender;
    }

    public void SetProgress()
    {
        progress = GameController.Instance.currentProgress;
    }

    private void UpdateRosterStatus()
    {
        for (int i = 0; i < userRosters.Count; i++)
        {
            string name = (type == UserType.Defender) ? DefenderController.Instance.selectedMonsterCandidates[i] : OffenderController.Instance.selectedCharacterCandidates[i];
            string enemyName = (type == UserType.Defender) ? OffenderController.Instance.selectedCharacterCandidates[i] : DefenderController.Instance.selectedMonsterCandidates[i];

            userRosters[i].SetImage(type, name);
            enemyRosters[i].SetImage(enemyType, enemyName);

            if (progress != GameProgress.ReadyGame)
            {
                bool isCharDead = (type == UserType.Defender) ? DefenderController.Instance.isDead[i] : OffenderController.Instance.isDead[i];
                bool isEnemyDead = (type == UserType.Defender) ? OffenderController.Instance.isDead[i] : DefenderController.Instance.isDead[i];
                userRosters[i].SetIsDead(isCharDead);
                enemyRosters[i].SetIsDead(isEnemyDead);
            }
        }
    }

    public void ChangeView()
    {
        readyButton.SetButtonInteract(true);
        roundText.gameObject.SetActive(false);
        turnText.gameObject.SetActive(false);
        SetProgress();
        ClearCharacters();
        UpdateRosterStatus();
        foreach (GameObject view in gameViews) view.SetActive(false);

        switch (GameController.Instance.currentProgress)
        {
            case GameProgress.ReadyGame:
                BlindSelectedRoster();
                gameViews[0].SetActive(true);
                ShowAllCandidates();
                break;

            case GameProgress.ReadyRound:
                roundText.gameObject.SetActive(true);
                roundText.text = "ROUND " + GameController.Instance.round.ToString();
                BlindSelectedRoster();
                gameViews[1].SetActive(true);
                if (type == UserType.Defender)
                {
                    defenderSkillTree.SetActive(true);
                    offenderSkillTree.gameObject.SetActive(false);
                    defenderSpecialView.SetActive(true);
                    offenderSpecialView.SetActive(false);
                    // 유닛 선택시 Defender에서 몬스터가 죽었는지 체크해야함.
                    // 기존에 선택했던 유닛부터 다시 세팅할 수 있게 해줘야 함.
                    int index = DefenderController.Instance.GetFirstAliveMonster();
                    characterToggles[index].toggle.isOn = true;
                }
                else
                {
                    defenderSkillTree.SetActive(false);
                    offenderSkillTree.gameObject.SetActive(true);
                    defenderSpecialView.SetActive(false);
                    offenderSpecialView.SetActive(true);
                    // 유닛 선택시 Offender에서 유닛이 죽었는지 체크해야 함.
                    // 기존에 선택했던 유닛부터 다시 세팅할 수 있게 해줘야 함.
                    int index = OffenderController.Instance.GetFirstAliveCharacter();
                    characterToggles[index].toggle.isOn = true;

                    List<int> aliveList = OffenderController.Instance.GetAliveCharacterList();
                    // 죽은애 알아서 잘 찾아갈 수 있게 조정
                    for (int i = 0; i < aliveList.Count && i < 3; i++)
                    {
                        OffenderController.Instance.SelectCharacter(aliveList[i]);
                        SetOffenderRoster(i);
                    }
                    OffenderController.Instance.SelectCharacter(0);
                }

                for (int i = 0; i < characterToggles.Count; i++)
                {
                    string name = (type == UserType.Defender) ? DefenderController.Instance.selectedMonsterCandidates[i] : OffenderController.Instance.selectedCharacterCandidates[i];
                    bool isCharDead = (type == UserType.Defender) ? DefenderController.Instance.isDead[i] : OffenderController.Instance.isDead[i];
                    characterToggles[i].SetFace(type, name);
                    characterToggles[i].CharacterDead(isCharDead);
                }
                SetAllDice();
                break;

            case GameProgress.PlayRound:
                turnText.gameObject.SetActive(true);
                gameViews[2].SetActive(true);
                playRoundView.SetActive(true);
                SetCharacters();
                break;
        }
    }

    public void Alert(int errorIndex)
    {
        string str = "";
        switch (errorIndex)
        {
            case 10:
                str = "캐릭터 세팅을 끝내주세요.";
                break;
            case 21:
                str = "몬스터의 주사위는 같은 스킬을 두 가지보다 많이 넣을 수 없습니다.";
                break;
            case 22:
                str = "몬스터의 주사위를 코스트보다 많이 넣을 수 없습니다.";
                break;
            case 26:
                str = "공격자의 주사위는 기본공격이 최소 2개는 있어야 합니다.";
                break;
            case 27:
                str = "공격자의 주사위는 같은 스킬을 최대 3개까지 넣을 수 있습니다.";
                break;
            case 28:
                str = "선행 스킬을 먼저 학습해야 합니다.";
                break;
            case 29:
                str = "스킬 포인트가 모자랍니다.";
                break;

        }

        if (!string.IsNullOrEmpty(str)) alert.ShowAlert(str);
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

        // 자동화 코드.
        // 자동화가 아니라 프리팹을 활용해야할지는 고민이 좀 필요할 듯
        // 공격자와 방어자를 나중에 합칠 생각

        selectedCharacterIndex = index;
        string name;
        if (type == UserType.Defender)
        {
            DefenderController.Instance.SelectMonster(selectedCharacterIndex);
            Monster monster = DefenderController.Instance.monsters[index];
            name = monster.name;
            monsterNameText.text = name;
            monsterHpText.text = monster.hp.ToString();
            List<MonsterSkill> diceSkills = SkillDatabase.Instance.GetMonsterDices(name);

            List<GameObject>[] diceTierList = new List<GameObject>[3];
            for (int i = 0; i < 3; i++)
                diceTierList[i] = new List<GameObject>();

            for (int i = 0; i < diceSkills.Count; i++)
            {
                int tier = diceSkills[i].tier;
                GameObject obj = Instantiate(diceSkillIconPrefab);
                obj.transform.SetParent(defenderSkillTiers[tier - 1]);
                obj.transform.localScale = new Vector3(1, 1, 1);
                SkillIcon diceIcon = obj.GetComponent<SkillIcon>();
                diceIcon.SetSkill(diceSkills[i], (GameController.Instance.round >= tier));

                diceTierList[tier - 1].Add(obj);
                diceSkillIcons.Add(diceIcon);
            }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < diceTierList[i].Count; j++)
                {
                    RectTransform rect = diceTierList[i][j].GetComponent<RectTransform>();

                    float x = 0;
                    float y = 0;
                    if (j % 2 == 1) x = 110;
                    if (diceTierList[i].Count > 1) y = -1 * (int)(j / 2) * (defenderSkillTiers[i].rect.height - 100) / ((diceTierList[i].Count >= 5) ? 2 : 1);
                    rect.anchoredPosition = new Vector3(x, y, 0);
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
            List<CharacterSkill> diceSkills = SkillDatabase.Instance.GetCharacterDices(name);

            int maxTier = OffenderController.Instance.GetMaxTier() + 1;
            List<GameObject>[] diceTierList = new List<GameObject>[maxTier];
            for (int i = 0; i < maxTier; i++)
                diceTierList[i] = new List<GameObject>();

            for (int i = 0; i < diceSkills.Count; i++)
            {
                int tier = diceSkills[i].tier;
                GameObject obj = Instantiate(diceSkillIconPrefab);
                obj.transform.SetParent(offenderSkillTree);
                obj.transform.localScale = new Vector3(1, 1, 1);
                SkillIcon diceIcon = obj.GetComponent<SkillIcon>();
                diceIcon.SetSkill(diceSkills[i], OffenderController.Instance.IsSkillGotten(i));

                diceTierList[tier].Add(obj);
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

            UpdateSkillPoint();
        }

        SetAllDice();
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
        int dmg = -1;
        if (skill.id / 10000 == 1)
        {
            CharacterSkill cs = skill as CharacterSkill;
            dmg = cs.damage;
        }
        description.SetDescription(skill.id, skill.name, "", "DESCRIPTION", dmg);
    }

    public void SetAllDice()
    {
        selectedDiceIndex = 0;
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
            diceIcons[selectedDiceIndex++].SetSkill(skill);

            if (selectedDiceIndex >= diceIcons.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);

            int totalCost = DefenderController.MAX_COST - DefenderController.Instance.GetDiceCost();
            costSlider.value = (totalCost >= 0) ? totalCost : 0;
            costText.text = totalCost.ToString();
        }
        else
        {
            CharacterSkill skill = OffenderController.Instance.GetSelectedDice(selectedDiceIndex);
            diceIcons[selectedDiceIndex++].SetSkill(skill);

            if (selectedDiceIndex >= diceIcons.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);
        }
    }
    public void SetDiceOnce(Skill skill, bool isOn = true)
    {
        if (isOn == false)
        {
            if (type == UserType.Offender)
            {
                int index = OffenderController.Instance.LearnSkill(skill as CharacterSkill);
                if (index > -1)
                {
                    diceSkillIcons[index].SetOnOff(true);
                    UpdateSkillPoint();
                }
                else if (index == -2)
                {
                    Alert(28);
                }
                else
                {
                    Alert(29);
                }
            }
            return;
        }

        if (type == UserType.Defender)
        {
            int check = DefenderController.Instance.SetDice(selectedDiceIndex, skill as MonsterSkill);
            if (check != 0)
            {
                Alert(check + 20);
                return;
            }

            diceIcons[selectedDiceIndex++].SetSkill(skill as MonsterSkill);

            if (selectedDiceIndex >= diceIcons.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);

            int totalCost = DefenderController.MAX_COST - DefenderController.Instance.GetDiceCost();
            costSlider.value = (totalCost >= 0) ? totalCost : 0;
            costText.text = totalCost.ToString();
        }
        else
        {
            int check = OffenderController.Instance.SetDice(selectedDiceIndex, skill as CharacterSkill);
            if (check != 0)
            {
                Alert(check + 25);
                return;
            }

            diceIcons[selectedDiceIndex++].SetSkill(skill as CharacterSkill);

            if (selectedDiceIndex >= diceIcons.Count) selectedDiceIndex -= 1;
            SelectDice(selectedDiceIndex);
        }
    }

    public void UpdateSkillPoint()
    {
        if (type == UserType.Offender) offenderSkillPointText.text = "SKILL POINT: " + OffenderController.Instance.GetSkillPoint().ToString();
        else offenderSkillPointText.text = "";
    }
    public void SelectDice(int index)
    {
        RectTransform rect = diceIcons[index].GetComponent<RectTransform>();
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
        Vector3 pos = new Vector3(rect.anchoredPosition.x + 6.25f, 0, 0);
        selectedAttackSkill.anchoredPosition = pos;
    }

    public void SetOffenderRoster(int index)
    {
        OffenderController.Instance.SelectRoster(index);
        SetRosterIcons();
    }

    private void SetRosterIcons()
    {
        for (int i = 0; i < offenderRosters.Count; i++)
        {
            int index = OffenderController.Instance.roster[i];
            string name = OffenderController.Instance.characters[index]._role;

            offenderRosters[i].SetImage(UserType.Offender, name);
            offenderRosters[i].SetNumber(index);
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

        if (type == UserType.Defender)
        {
            List<string> characterNames = OffenderController.Instance.GetCharacterRoster();
            // 자신 캐릭터 소환
            {
                Monster monster = DefenderController.Instance.GetMonsterRoster();
                MonsterSkill skill = DefenderController.Instance.GetAttackSkill();

                prefab = Resources.Load(charPath + monster.name);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterParent);
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);

                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetSkill(skill);
                character.SetCharacterIndex(GameController.Instance.defenderUnit);

                charObjects.Add(character);
            }

            // 적 캐릭터 소환
            for (int i = 0; i < 3; i++)
            {
                prefab = Resources.Load(enemyPath + characterNames[i]);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterParent);
                obj.transform.position = new Vector3(obj.transform.position.x + 2f * i, obj.transform.position.y, 0);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.offenderUnits[i]);
                enemyObjects.Add(character);
            }
            UpdateCharacters();

            prefab = Resources.Load("Prefab/Maps/" + "Forest");
            mapObject = Instantiate(prefab) as GameObject;
            mapObject.transform.SetParent(mapParent);
            mapObject.transform.SetAsLastSibling();
        }
        else
        {
            List<string> characterNames = OffenderController.Instance.GetCharacterRoster();

            Monster monster = DefenderController.Instance.GetMonsterRoster();
            MonsterSkill skill = DefenderController.Instance.GetAttackSkill();

            // 자신 캐릭터 소환
            for (int i = 0; i < characterNames.Count; i++)
            {
                prefab = Resources.Load(charPath + characterNames[i]);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterParent);
                obj.transform.position = new Vector3((obj.transform.position.x + 2f * i) * -1, obj.transform.position.y, 0);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.offenderUnits[i]);
                charObjects.Add(character);
            }

            // 적 캐릭터 소환
            {
                prefab = Resources.Load(enemyPath + monster.name);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterParent);
                obj.transform.position = new Vector3(obj.transform.position.x * -1, obj.transform.position.y, 0);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.defenderUnit);
                character.SetSkill(skill);
                enemyObjects.Add(character);
            }
            UpdateCharacters();

            prefab = Resources.Load("Prefab/Maps/" + "Forest");
            mapObject = Instantiate(prefab) as GameObject;
            mapObject.transform.SetParent(mapParent);
        }

        for (int i = 0; i < charObjects.Count; i++)
            charObjects[i].SetFlip(false);
        for (int i = 0; i < enemyObjects.Count; i++)
            enemyObjects[i].SetFlip(true);
    }

    public void UpdateCharacters()
    {
        int monHp = 0, monTurn = 0;
        DefenderController.Instance.GetMonsterInfo(ref monHp, ref monTurn);
        if (type == UserType.Defender)
        {
            for (int i = 0; i < charObjects.Count; i++)
                charObjects[i].UpdateCharacterInfo(monHp, monTurn);
        }

        else
        {
            for (int i = 0; i < enemyObjects.Count; i++)
                enemyObjects[i].UpdateCharacterInfo(monHp, monTurn);
        }
    }

    List<Dice> diceObjects = new List<Dice>();
    public void DiceRoll()
    {
        diceObjects.Clear();
        if (type == UserType.Defender)
        {
            for (int i = 0; i < charObjects.Count; i++)
            {
                GameObject dice = Instantiate(dicePrefab);
                dice.transform.position = new Vector3(charObjects[i].transform.position.x - 1, dice.transform.position.y, 0);
                Dice diceObj = dice.GetComponent<Dice>();
                diceObjects.Add(diceObj);
                GameObject dice2 = Instantiate(dicePrefab);
                dice2.transform.position = new Vector3(charObjects[i].transform.position.x + 1, dice2.transform.position.y, 0);
                diceObj = dice2.GetComponent<Dice>();
                diceObjects.Add(diceObj);
            }

            for (int i = 0; i < enemyObjects.Count; i++)
            {
                GameObject dice = Instantiate(dicePrefab);
                dice.transform.position = new Vector3(enemyObjects[i].transform.position.x, dice.transform.position.y, 0);
                Dice diceObj = dice.GetComponent<Dice>();
                diceObjects.Add(diceObj);
            }
        }
        else
        {
            for (int i = 0; i < enemyObjects.Count; i++)
            {
                GameObject dice = Instantiate(dicePrefab);
                dice.transform.position = new Vector3(enemyObjects[i].transform.position.x - 1, dice.transform.position.y, 0);
                Dice diceObj = dice.GetComponent<Dice>();
                diceObjects.Add(diceObj);
                GameObject dice2 = Instantiate(dicePrefab);
                dice2.transform.position = new Vector3(enemyObjects[i].transform.position.x + 1, dice2.transform.position.y, 0);
                diceObj = dice2.GetComponent<Dice>();
                diceObjects.Add(diceObj);
            }

            for (int i = 0; i < charObjects.Count; i++)
            {
                GameObject dice = Instantiate(dicePrefab);
                dice.transform.position = new Vector3(charObjects[i].transform.position.x, dice.transform.position.y, 0);
                Dice diceObj = dice.GetComponent<Dice>();
                diceObjects.Add(diceObj);
            }
        }

        foreach (Dice dice in diceObjects) dice.Roll();
    }

    public void SetDiceSkill(int index, int id)
    {
        diceObjects[index].SetSkill(id);
    }

    private void ClearCharacters()
    {
        for (int i = 0; i < charObjects.Count; i++)
            Destroy(charObjects[i].gameObject);
        for (int i = 0; i < enemyObjects.Count; i++)
            Destroy(enemyObjects[i].gameObject);

        charObjects.Clear();
        enemyObjects.Clear();
        Destroy(mapObject);
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

    public void SetTurn(int turn)
    {
        turnText.text = "TURN " + turn.ToString();
    }

    private void BlindSelectedRoster()
    {
        foreach (RectTransform rect in rosterSelected) rect.gameObject.SetActive(false);
        foreach (RectTransform rect in enemyRosterSelected) rect.gameObject.SetActive(false);
    }
    public void ShowSelectedRoster(int[] indexes, bool isEnemy = false)
    {
        if (isEnemy == false)
        {
            int i = 0;
            for (; i < indexes.Length; i++)
            {
                rosterSelected[i].transform.SetParent(userRosters[indexes[i] % 10].transform);
                rosterSelected[i].anchoredPosition = new Vector2(0, 0);
                rosterSelected[i].gameObject.SetActive(true);
            }
            for (; i < rosterSelected.Count; i++)
                rosterSelected[i].gameObject.SetActive(false);
        }
        else
        {
            int i = 0;
            for (; i < indexes.Length; i++)
            {
                enemyRosterSelected[i].transform.SetParent(enemyRosters[indexes[i] % 10].transform);
                enemyRosterSelected[i].anchoredPosition = new Vector2(0, 0);
                enemyRosterSelected[i].gameObject.SetActive(true);
            }
            for (; i < enemyRosterSelected.Count; i++)
                enemyRosterSelected[i].gameObject.SetActive(false);
        }

    }
    public void ShowSelectedRoster(int index, bool isEnemy = false)
    {
        if (isEnemy == false)
        {
            rosterSelected[0].transform.SetParent(userRosters[index % 10].transform);
            rosterSelected[0].anchoredPosition = new Vector2(0, 0);
            rosterSelected[0].gameObject.SetActive(true);

            for (int i = 1; i < rosterSelected.Count; i++)
            {
                rosterSelected[i].gameObject.SetActive(false);
            }
        }
        else
        {
            enemyRosterSelected[0].transform.SetParent(enemyRosters[index % 10].transform);
            enemyRosterSelected[0].anchoredPosition = new Vector2(0, 0);
            enemyRosterSelected[0].gameObject.SetActive(true);

            for (int i = 1; i < enemyRosterSelected.Count; i++)
            {
                enemyRosterSelected[i].gameObject.SetActive(false);
            }
        }

    }
    #endregion
}
