using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Data;

public class GamePlayUIController : MonoBehaviour
{
    #region Variables
    private UserType type;
    private UserType enemyType;
    private GameProgress progress;

    [Header("COMMONS")]
    [SerializeField] private List<GameObject> gameViews;
    [SerializeField] private CustomButton readyButton;
    [SerializeField] private AlertObject alert;

    [Space]
    [SerializeField] private Transform userRosterTransform;
    [SerializeField] private Transform userBenchTransform;
    [SerializeField] private List<RosterIcon> userRosters;
    [SerializeField] private List<CharIcon> userBenchs;
    [SerializeField] private List<CharIcon> enemyRosters;
    [SerializeField] private RectTransform rosterSelected;
    [SerializeField] private RectTransform benchSelected;
    [SerializeField] private List<RectTransform> enemyRosterSelected;

    [SerializeField] private Image selectedCharacterImage;
    [SerializeField] private Text selectedCharacterText;

    [SerializeField] private Transform skillRosterTransform;
    [SerializeField] private Transform diceTransform;

    [SerializeField] private GameObject usingSkillIconPrefab;

    private List<UsingSkillIcon> skillRosters = new List<UsingSkillIcon>();
    private List<UsingSkillIcon> usingDices = new List<UsingSkillIcon>();

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

    [Space]
    // 스킬트리의 바탕이 되는 부분
    [SerializeField] private GameObject defenderSkillTree;
    [SerializeField] private List<RectTransform> defenderSkillTiers;
    [SerializeField] private Image offenderSkillTree;
    [SerializeField] private RectTransform offenderSkillTreeRect;
    [SerializeField] private Text offenderSkillPointText;

    [Space]
    // 주사위 설정
    [SerializeField] private GameObject diceSkillIconPrefab;
    private List<SkillIcon> diceSkillIcons = new List<SkillIcon>();

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

    private int selectedCharacterIndex;


    [SerializeField] private Text turnText;
    [Header("PLAY ROUND")]
    [SerializeField] private GameObject playRoundView;
    [SerializeField] private Transform mapTransform;
    [SerializeField] private Transform characterTransform;
    private List<CharacterObject> charObjects = new List<CharacterObject>();
    private List<CharacterObject> enemyObjects = new List<CharacterObject>();
    private GameObject mapObject;

    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private CrowdControlIcon crowdControlIconPrefab;
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
        for (int i = 0; i < userBenchs.Count; i++)
        {
            int id = (type == UserType.Defender) ? DefenderController.Instance.selectedMonsterCandidates[i] : OffenderController.Instance.selectedCharacterCandidates[i];
            int enemyId = (type == UserType.Defender) ? OffenderController.Instance.selectedCharacterCandidates[i] : DefenderController.Instance.selectedMonsterCandidates[i];

            userBenchs[i].SetImage(type, id);
            enemyRosters[i].SetImage(enemyType, enemyId);

            if (progress != GameProgress.ReadyGame)
            {
                bool isCharDead = (type == UserType.Defender) ? DefenderController.Instance.isDead[i] : OffenderController.Instance.isDead[i];
                bool isEnemyDead = (type == UserType.Defender) ? OffenderController.Instance.isDead[i] : DefenderController.Instance.isDead[i];
                userBenchs[i].SetIsDead(isCharDead);
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
        alert.RemoveAlert();
        foreach (GameObject view in gameViews) view.SetActive(false);

        switch (GameController.Instance.currentProgress)
        {
            case GameProgress.ReadyGame:
                for (int i = 0; i < userRosters.Count; i++)
                {
                    userRosters[i].SetImage(type, -1);
                }
                BlindSelectedRoster();
                gameViews[0].SetActive(true);
                ShowAllCandidates();
                if (type == UserType.Defender)
                {
                    userRosters[1].gameObject.SetActive(false);
                    userRosters[2].gameObject.SetActive(false);
                }
                else
                {
                    userRosters[1].gameObject.SetActive(true);
                    userRosters[2].gameObject.SetActive(true);
                }
                break;

            case GameProgress.ReadyRound:
                benchSelected.gameObject.SetActive(false);
                rosterSelected.gameObject.SetActive(false);
                ShowCharacterRoster();
                roundText.gameObject.SetActive(true);
                roundText.text = "ROUND " + GameController.Instance.round.ToString();
                BlindSelectedRoster();
                gameViews[1].SetActive(true);
                if (type == UserType.Defender)
                {
                    defenderSkillTree.SetActive(true);
                    offenderSkillTree.gameObject.SetActive(false);
                    defenderSpecialView.SetActive(true);
                    // 유닛 선택시 Defender에서 몬스터가 죽었는지 체크해야함.
                    // 기존에 선택했던 유닛부터 다시 세팅할 수 있게 해줘야 함.
                    //int index = DefenderController.Instance.GetFirstAliveMonster();
                    userRosters[0].SetRosterNumber(DefenderController.Instance.monsterIndex);
                }
                else
                {
                    defenderSkillTree.SetActive(false);
                    offenderSkillTree.gameObject.SetActive(false);
                    defenderSpecialView.SetActive(false);

                    for (int i = 0; i < userRosters.Count; i++)
                    {
                        userRosters[i].SetRosterNumber(OffenderController.Instance.roster[i]);
                    }
                    /*
                    // 유닛 선택시 Offender에서 유닛이 죽었는지 체크해야 함.
                    // 기존에 선택했던 유닛부터 다시 세팅할 수 있게 해줘야 함.
                    int index = OffenderController.Instance.GetFirstAliveCharacter();

                    List<int> aliveList = OffenderController.Instance.GetAliveCharacterList();
                    // 죽은애 알아서 잘 찾아갈 수 있게 조정
                    for (int i = 0; i < aliveList.Count && i < 3; i++)
                    {
                        OffenderController.Instance.SelectCharacter(aliveList[i]);
                    }
                    OffenderController.Instance.SelectCharacter(0);
                    */
                }

                ShowCharacterSkillsInPanel();
                break;

            case GameProgress.PlayRound:
                benchSelected.gameObject.SetActive(false);
                rosterSelected.gameObject.SetActive(false);
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
                str = "몬스터의 스킬 로스터에 코스트보다 많이 넣을 수 없습니다.";
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
            case 30:
                str = "스킬 로스터에는 8개까지만 넣을 수 있습니다.";
                break;
            case 40:
                str = "승자는 공격자입니다.";
                break;
            case 41:
                str = "승자는 방어자입니다.";
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

        List<int> candidates = new List<int>();
        if (type == UserType.Defender) MonsterDatabase.Instance.GetAllMonsterCandidatesList(ref candidates);
        else CharacterDatabase.Instance.GetAllCharacterCandidatesList(ref candidates);

        foreach (int id in candidates)
        {
            GameObject gameObject = Instantiate(candidatePrefab) as GameObject;
            gameObject.transform.SetParent(candidatesTransform);
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            UIIcon uiIcon = gameObject.GetComponent<UIIcon>();
            uiIcon.SetImage(type, id);
        }
    }

    public void SelectCandidate(int id)
    {
        int size = 0;
        selectIcons[selectedCandidateIndex].SetImage(type, id);
        if (type == UserType.Defender)
        {
            DefenderController.Instance.SetMonsterCandidate(selectedCandidateIndex, id);
            size = DefenderController.Instance.selectedMonsterCandidates.Length;
        }
        else
        {
            OffenderController.Instance.SetCharacterCandidate(selectedCandidateIndex, id);
            size = OffenderController.Instance.selectedCharacterCandidates.Length;
        }

        bool existEmpty = false;
        for (int i = selectedCandidateIndex + 1; i < size; i++)
        {
            if ((type == UserType.Defender && DefenderController.Instance.selectedMonsterCandidates[i] == -1)
             || (type == UserType.Offender && OffenderController.Instance.selectedCharacterCandidates[i] == -1))
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

    private bool IsRoster(int i)
    {
        if (type == UserType.Defender)
        {
            return DefenderController.Instance.monsterIndex == i;
        }
        else
        {
            return System.Array.Exists(OffenderController.Instance.roster, index => index == i);
        }
    }

    private void ShowCharacterRoster()
    {
        for (int i = 0; i < userBenchs.Count; i++)
        {
            if (IsRoster(i))
            {
                userRosters[i].SetImage(type, userBenchs[i].characterId);
                userBenchs[i].SelectUnit(true);
            }
            else userBenchs[i].SelectUnit(false);
        }
    }

    public void ChangeRoster(RosterIcon roster, CharIcon bench)
    {
        int rosterIndex = -1;
        int benchIndex = -1;

        for (int i = 0; i < userBenchs.Count; i++)
        {
            if (userBenchs[i] == bench)
            {
                benchIndex = i;
                break;
            }
        }

        for (int i = 0; i <  userRosters.Count; i++)
        {
            if (userRosters[i] == roster)
            {
                rosterIndex = i;
                break;
            }
        }

        userBenchs[roster.index].SelectUnit(false);
        bench.SelectUnit(true);

        for (int i = 0; i < userRosters.Count; i++)
        {
            if (benchIndex == userRosters[i].index)
            {
                userRosters[i].SetImage(type, roster.characterId);
                userRosters[i].SetRosterNumber(roster.index);
                userBenchs[roster.index].SelectUnit(true);

                if (type == UserType.Defender) DefenderController.Instance.SelectMonster(roster.index);
                else OffenderController.Instance.AddRoster(i, roster.index);

                break;
            }
        }


        roster.SetImage(type, bench.characterId);
        roster.SetRosterNumber(benchIndex);

        if (type == UserType.Defender) DefenderController.Instance.SelectMonster(benchIndex);
        else OffenderController.Instance.AddRoster(rosterIndex, benchIndex);

    }

    public void SelectCharacter(int index, int id)
    {
        if (index >= userBenchs.Count) return;
        selectedCharacterIndex = index;
        string path = (type == UserType.Defender) ? MonsterDatabase.facePath : CharacterDatabase.facePath;
        string name = (type == UserType.Defender) ? MonsterDatabase.Instance.GetMonster(id).name : CharacterDatabase.Instance.GetCharacter(id)._role;
        Sprite sprite = Resources.Load<Sprite>(path + id.ToString());

        selectedCharacterImage.sprite = sprite;
        selectedCharacterText.text = name;

        if (IsRoster(index))
        {
            benchSelected.gameObject.SetActive(true);
            rosterSelected.gameObject.SetActive(true);
            for (int i = 0; i < userRosters.Count; i++)
            {
                if (userRosters[i].index == index)
                {
                    rosterSelected.transform.SetParent(userRosters[i].transform);
                    rosterSelected.anchoredPosition = Vector2.zero;
                    break;
                }
            }
            benchSelected.transform.SetParent(userBenchs[index].transform);
            benchSelected.anchoredPosition = Vector2.zero;
        }
        else
        {
            rosterSelected.gameObject.SetActive(false);
            benchSelected.gameObject.SetActive(true);
            benchSelected.transform.SetParent(userBenchs[index].transform);
            benchSelected.anchoredPosition = Vector2.zero;
        }

        if (type == UserType.Defender) DefenderController.Instance.SelectMonster(selectedCharacterIndex);
        else OffenderController.Instance.SelectCharacter(selectedCharacterIndex);


        if (GameController.Instance.currentProgress == GameProgress.ReadyRound) SetSkillTree();
        else if (GameController.Instance.currentProgress == GameProgress.PlayRound) ShowCharacterSkillsInPanel();
    }

    private void SetSkillTree()
    {
        ClearSkillTree();

        // 자동화 코드.
        // 자동화가 아니라 프리팹을 활용해야할지는 고민이 좀 필요할 듯
        // 공격자와 방어자를 나중에 합칠 생각

        int id;
        if (type == UserType.Defender)
        {
            Monster monster = DefenderController.Instance.monsters[selectedCharacterIndex];
            id = monster.id;
            monsterNameText.text = monster.name;
            monsterHpText.text = monster.hp.ToString();
            List<MonsterSkill> diceSkills = SkillDatabase.Instance.GetMonsterDices(id);

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
                diceIcon.SetSkill(type, diceSkills[i], (GameController.Instance.round >= tier));

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

            List<MonsterSkill> attackSkills = SkillDatabase.Instance.GetMonsterAttackSkills(id, GameController.Instance.round);
            MonsterSkill charAttackSkill = DefenderController.Instance.GetAttackSkill();
            for (int i = 0; i < defenderAttackSkills.Count; i++)
            {
                defenderAttackSkills[i].SetSkill(type, attackSkills[i]);
                if (charAttackSkill.id == attackSkills[i].id) SelectAttackSkill(i);
            }

        }
        else
        {
            offenderSkillTree.gameObject.SetActive(true);

            id = OffenderController.Instance.characters[selectedCharacterIndex].id;
            offenderSkillTree.sprite = Resources.Load<Sprite>("Sprites/UI/Offender Skill Tree/" + id);
            List<CharacterSkill> diceSkills = SkillDatabase.Instance.GetCharacterDices(id);

            int maxTier = OffenderController.Instance.GetMaxTier() + 1;
            List<GameObject>[] diceTierList = new List<GameObject>[maxTier];
            for (int i = 0; i < maxTier; i++)
                diceTierList[i] = new List<GameObject>();

            for (int i = 0; i < diceSkills.Count; i++)
            {
                int tier = diceSkills[i].tier;

                #region 스킬 배치를 위한 조정부
                if (diceSkills[i].id == 10107) diceTierList[tier].Add(null);
                else if (diceSkills[i].id == 10211)
                {
                    diceTierList[tier].Add(null);
                    diceTierList[tier].Add(null);
                }
                #endregion

                GameObject obj = Instantiate(diceSkillIconPrefab);
                obj.transform.SetParent(offenderSkillTreeRect);
                obj.transform.localScale = new Vector3(1, 1, 1);
                SkillIcon diceIcon = obj.GetComponent<SkillIcon>();
                diceIcon.SetSkill(type, diceSkills[i], OffenderController.Instance.IsSkillGotten(i));

                diceTierList[tier].Add(obj);
                diceSkillIcons.Add(diceIcon);

                #region 스킬 배치를 위한 조정부2
                if (diceSkills[i].id == 10211) diceTierList[tier].Add(null);
                #endregion
            }

            for (int i = 0; i < maxTier; i++)
                for (int j = 0; j < diceTierList[i].Count; j++)
                {
                    if (diceTierList[i][j] == null) continue;
                    RectTransform rect = diceTierList[i][j].GetComponent<RectTransform>();

                    float x = 0;
                    x = i * (100 + ((offenderSkillTreeRect.rect.width - 100 * maxTier) / (maxTier - 1)));
                    float y = 0;
                    if (diceTierList[i].Count > 1) y = -1 * (j * 100 + j * (offenderSkillTreeRect.rect.height - diceTierList[i].Count * 100) / (diceTierList[i].Count - 1));
                    else y = (offenderSkillTreeRect.rect.height - 100) / -2;
                    rect.anchoredPosition = new Vector3(x, y, 0);
                }

            UpdateSkillPoint();
        }
        ShowCharacterSkillsInPanel();
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
        if (skill.id / 10000 == 1)
        {
            CharacterSkill cs = skill as CharacterSkill;
            description.SetDescription(skill.id, skill.name, "", skill.description, "DMG: " + cs.damage, skill.ccList, skill.turn);
        }
        else
        {
            MonsterSkill ms = skill as MonsterSkill;
            string str = "";
            if (ms.type == MonsterSkill.SkillType.Dice) str = "COST: " + ms.cost;
            else str = "TURN: " + ms.turn;
            description.SetDescription(skill.id, skill.name, "", skill.description, str, skill.ccList);
        }
    }

    public void ShowCharacterSkillsInPanel()
    {
        // 캐릭터가 가진 스킬 리스트, 주사위를 띄워주는 패널

        for (int i = 0; i < skillRosters.Count; i++)
        {
            Destroy(skillRosters[i].gameObject);
        }
        skillRosters.Clear();

        for (int i = 0; i < usingDices.Count; i++)
        {
            Destroy(usingDices[i].gameObject);
        }
        usingDices.Clear();

        // 여기서 띄워주기
        // 어떤 유닛이 선택됐는지는 selectedCharacterIndex에 저장되어있음
        int size = 0; // 가지고있는 스킬의 갯수
        if (type == UserType.Offender)
        {
            size = OffenderController.Instance.GetSkillRosterSize();
            for (int i = 0; i < size; i++)
            {
                Skill skill = OffenderController.Instance.GetSkillRoster(i); // n번째 스킬

                GameObject obj = Instantiate(usingSkillIconPrefab);
                obj.transform.SetParent(skillRosterTransform);
                obj.transform.localScale = new Vector3(1, 1, 1);
                UsingSkillIcon usingSkillIcon = obj.GetComponent<UsingSkillIcon>();
                usingSkillIcon.SetSkill(type, skill, true);

                skillRosters.Add(usingSkillIcon);
            }
            size = OffenderController.Instance.GetDiceSize();
            for (int i = 0; i < size; i++)
            {
                Skill skill = OffenderController.Instance.GetDiceSkill(i); // n번째 스킬

                GameObject obj = Instantiate(usingSkillIconPrefab);
                obj.transform.SetParent(diceTransform);
                obj.transform.localScale = new Vector3(1, 1, 1);
                UsingSkillIcon usingSkillIcon = obj.GetComponent<UsingSkillIcon>();
                usingSkillIcon.SetSkill(type, skill, true);
                
                usingDices.Add(usingSkillIcon);
            }
        }
        else
        {
            size = DefenderController.Instance.GetSkillRosterSize();
            for (int i = 0; i < size; i++)
            {
                Skill skill = DefenderController.Instance.GetSkillRoster(i);

                GameObject obj = Instantiate(usingSkillIconPrefab);
                obj.transform.SetParent(skillRosterTransform);
                obj.transform.localScale = new Vector3(1, 1, 1);
                UsingSkillIcon usingSkillIcon = obj.GetComponent<UsingSkillIcon>();
                usingSkillIcon.SetSkill(type, skill, true);

                skillRosters.Add(usingSkillIcon);
            }
            size = DefenderController.Instance.GetDiceSize();
            for (int i = 0; i < size; i++)
            {
                Skill skill = DefenderController.Instance.GetSelectedDice(i);

                GameObject obj = Instantiate(usingSkillIconPrefab);
                obj.transform.SetParent(diceTransform);
                obj.transform.localScale = new Vector3(1, 1, 1);
                UsingSkillIcon usingSkillIcon = obj.GetComponent<UsingSkillIcon>();
                usingSkillIcon.SetSkill(type, skill, true);

                usingDices.Add(usingSkillIcon);
            }
        } 
    }

    public void AddSkillRoster(Skill skill, bool isOn)
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
        else
        {
            // 알림 설정 good!
            // 실제 로스터에 추가 good!
            if (type == UserType.Offender)
            {
                int n = OffenderController.Instance.SetSkillRoster(skill as CharacterSkill);
                if (n > 0) 
                {
                    Alert(n);
                    return;
                }
            }
            else
            {
                int n = DefenderController.Instance.SetSkillRoster(skill as MonsterSkill);
                if (n > 0)
                {
                    Alert(n);
                    return;
                }
            }
            

            GameObject obj = Instantiate(usingSkillIconPrefab);
            obj.transform.SetParent(skillRosterTransform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            UsingSkillIcon usingSkillIcon = obj.GetComponent<UsingSkillIcon>();
            usingSkillIcon.SetSkill(type, skill, true);

            skillRosters.Add(usingSkillIcon);
        }
    }

    public void RemoveSkillRoster(UsingSkillIcon icon)
    {
        for (int i = 0; i < skillRosters.Count; i++)
        {
            if (skillRosters[i] == icon)
            {
                // 실제 로스터에서 삭제 good!
                if (type == UserType.Offender)
                {
                    OffenderController.Instance.RemoveSkillRoster(i);
                }
                else
                {
                    DefenderController.Instance.RemoveSkillRoster(i);
                }

                Destroy(skillRosters[i].gameObject);
                skillRosters.RemoveAt(i);
                break;
            }
        }    
    }

    public void AddOrRemoveDice(UsingSkillIcon icon)
    {
        for (int i = 0; i < skillRosters.Count; i++)
        {
            if (skillRosters[i] == icon)
            {
                // 실제 로스터에서 주사위로
                if (type == UserType.Offender)
                {
                    if (OffenderController.Instance.GetDiceSize() < 2)
                    {
                        for (int j = OffenderController.Instance.GetDiceSize(); j < 2; j++)
                        {
                            OffenderController.Instance.SetDice(true, 9);
                        }
                    }
                    else OffenderController.Instance.SetDice(true, i);
                }
                else
                {
                    DefenderController.Instance.SetDice(true, i);
                }

                skillRosters[i].transform.SetParent(diceTransform);
                skillRosters.RemoveAt(i);
                usingDices.Add(icon);
                return;
            }
        }

        for (int i = 2; i < usingDices.Count; i++)
        {
            if (usingDices[i] == icon)
            {
                // 실제 주사위에서 로스터로
                if (type == UserType.Offender)
                {
                    OffenderController.Instance.SetDice(false, i);
                }
                else
                {
                    DefenderController.Instance.SetDice(false, i);
                }

                usingDices[i].transform.SetParent(skillRosterTransform);
                usingDices.RemoveAt(i);
                skillRosters.Add(icon);
                return;
            }
        }
    }

    public void UpdateSkillPoint()
    {
        if (type == UserType.Offender) offenderSkillPointText.text = "SKILL POINT: " + OffenderController.Instance.GetSkillPoint().ToString();
        else offenderSkillPointText.text = "";
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
            List<int> characterIds = OffenderController.Instance.GetCharacterRoster();
            // 자신 캐릭터 소환
            {
                Monster monster = DefenderController.Instance.GetMonsterRoster();
                MonsterSkill skill = DefenderController.Instance.GetAttackSkill();

                prefab = Resources.Load(charPath + monster.id);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterTransform);
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);

                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetSkill(skill);
                character.SetCharacterIndex(GameController.Instance.defenderUnit);

                charObjects.Add(character);
            }

            // 적 캐릭터 소환
            for (int i = 0; i < 3; i++)
            {
                prefab = Resources.Load(enemyPath + characterIds[i]);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterTransform);
                obj.transform.position = new Vector3(obj.transform.position.x + 2f * i, obj.transform.position.y, 0);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.offenderUnits[i]);
                enemyObjects.Add(character);
            }
            UpdateCharacters();

            prefab = Resources.Load("Prefab/Maps/" + "Forest");
            mapObject = Instantiate(prefab) as GameObject;
            mapObject.transform.SetParent(mapTransform);
            mapObject.transform.SetAsLastSibling();
        }
        else
        {
            List<int> characterNames = OffenderController.Instance.GetCharacterRoster();

            Monster monster = DefenderController.Instance.GetMonsterRoster();
            MonsterSkill skill = DefenderController.Instance.GetAttackSkill();

            // 자신 캐릭터 소환
            for (int i = 0; i < characterNames.Count; i++)
            {
                prefab = Resources.Load(charPath + characterNames[i]);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterTransform);
                obj.transform.position = new Vector3((obj.transform.position.x + 2f * i) * -1, obj.transform.position.y, 0);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.offenderUnits[i]);
                charObjects.Add(character);
            }

            // 적 캐릭터 소환
            {
                prefab = Resources.Load(enemyPath + monster.id);
                obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(characterTransform);
                obj.transform.position = new Vector3(obj.transform.position.x * -1, obj.transform.position.y, 0);
                CharacterObject character = obj.GetComponent<CharacterObject>();
                character.SetCharacterIndex(GameController.Instance.defenderUnit);
                character.SetSkill(skill);
                enemyObjects.Add(character);
            }
            UpdateCharacters();

            prefab = Resources.Load("Prefab/Maps/" + "Forest");
            mapObject = Instantiate(prefab) as GameObject;
            mapObject.transform.SetParent(mapTransform);
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
                if (charObjects[i] != null) charObjects[i].UpdateCharacterInfo(monHp, monTurn);
        }
        else
        {
            for (int i = 0; i < enemyObjects.Count; i++)
                if (enemyObjects[i] != null) enemyObjects[i].UpdateCharacterInfo(monHp, monTurn);            
        }
    }

    public void UpdateOffenderCharacter(int index, System.Tuple<CharacterSkill, int> readyTurn)
    {
        if (type == UserType.Defender)
        {
            for (int i = 0; i < enemyObjects.Count; i++)
                if (enemyObjects[i] != null && enemyObjects[i].GetIndex() == index)
                {
                    if (readyTurn != null)
                    {
                        enemyObjects[i].SetSkill(readyTurn.Item1);
                        enemyObjects[i].UpdateCharacterInfo(0, readyTurn.Item2);
                    }
                    else enemyObjects[i].UpdateCharacterInfo(0, 0, true);
                    break;
                }
        }
        else
        {
            for (int i = 0; i < charObjects.Count; i++)
                if (charObjects[i] != null && charObjects[i].GetIndex() == index)
                {
                    if (readyTurn != null)
                    {
                        charObjects[i].SetSkill(readyTurn.Item1);
                        charObjects[i].UpdateCharacterInfo(0, readyTurn.Item2);
                    }
                    else charObjects[i].UpdateCharacterInfo(0, 0, true);
                    break;
                }
        }
    }

    public void DeadCharacter(int index)
    {
        if (type == UserType.Defender)
        {
            for (int i = 0; i < enemyObjects.Count; i++)
                if (enemyObjects[i].GetIndex() == index)
                {
                    enemyObjects[i].Dead();
                    enemyObjects[i] = null;
                    break;
                }
        }
        else
        {
            for (int i = 0; i < charObjects.Count; i++)
                if (charObjects[i].GetIndex() == index)
                {
                    charObjects[i].Dead();
                    charObjects[i] = null;
                    break;
                }
        }
    }

    List<Dice> diceObjects = new List<Dice>();
    public void DiceRoll(List<bool> isRolled, List<bool> isReady)
    {
        diceObjects.Clear();
        if (type == UserType.Defender)
        {
            if (isRolled[0])
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
            }
            else
            {
                diceObjects.Add(null);
                diceObjects.Add(null);
            }

            for (int i = 0; i < enemyObjects.Count; i++)
            {
                if (isRolled[i + 1] && isReady[i + 1] == false && enemyObjects[i] != null)
                {
                    GameObject dice = Instantiate(dicePrefab);
                    dice.transform.position = new Vector3(enemyObjects[i].transform.position.x, dice.transform.position.y, 0);
                    Dice diceObj = dice.GetComponent<Dice>();
                    diceObjects.Add(diceObj);
                }
                else
                    diceObjects.Add(null);
            }
        }
        else
        {
            if (isRolled[0])
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
            }
            else
            {
                diceObjects.Add(null);
                diceObjects.Add(null);
            }


            for (int i = 0; i < charObjects.Count; i++)
            {
                if (isRolled[i + 1] && isReady[i + 1] == false && charObjects[i] != null)
                {
                    GameObject dice = Instantiate(dicePrefab);
                    dice.transform.position = new Vector3(charObjects[i].transform.position.x, dice.transform.position.y, 0);
                    Dice diceObj = dice.GetComponent<Dice>();
                    diceObjects.Add(diceObj);
                }
                else
                    diceObjects.Add(null);
            }
        }

        foreach (Dice dice in diceObjects)
            if (dice != null) dice.Roll();
    }

    public void SetDiceSkill(int index, int id)
    {
        if (diceObjects[index] != null) diceObjects[index].SetSkill(id);
    }

    public void UpdateCrowdControl(int index, int id, int turn, int stack, bool isRemove = false)
    {
        for (int i = 0; i < charObjects.Count; i++)
        {
            if (charObjects[i] != null && index == charObjects[i].GetIndex())
            {
                charObjects[i].UpdateCrowdControl(id, isRemove, turn, stack, crowdControlIconPrefab);
                return;
            }
        }

        for (int i = 0; i < enemyObjects.Count; i++)
        {
            if (enemyObjects[i] != null && index == enemyObjects[i].GetIndex())
            {
                enemyObjects[i].UpdateCrowdControl(id, isRemove, turn, stack, crowdControlIconPrefab);
                return;
            }
        }
    }

    private void ClearCharacters()
    {
        for (int i = 0; i < charObjects.Count; i++)
            if (charObjects[i] != null) Destroy(charObjects[i].gameObject);
        for (int i = 0; i < enemyObjects.Count; i++)
            if (enemyObjects[i] != null) Destroy(enemyObjects[i].gameObject);

        charObjects.Clear();
        enemyObjects.Clear();
        Destroy(mapObject);
    }

    public void PlayAnimation(int index, string anim)
    {
        foreach (CharacterObject c in charObjects)
        {
            if (c != null && c.CheckIndex(index))
            {
                c.SetAnimation(anim);
                return;
            }
        }

        foreach (CharacterObject c in enemyObjects)
        {
            if (c != null && c.CheckIndex(index))
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
    public void ShowSelectedEnemy(int[] indexes)
    {
        int i = 0;
        for (; i < indexes.Length; i++)
        {
            enemyRosterSelected[i].transform.SetParent(enemyRosters[indexes[i] % 10].transform);
            enemyRosterSelected[i].anchoredPosition = Vector2.zero;
            enemyRosterSelected[i].gameObject.SetActive(true);
        }
        for (; i < enemyRosterSelected.Count; i++)
            enemyRosterSelected[i].gameObject.SetActive(false);
    }
    #endregion
}
