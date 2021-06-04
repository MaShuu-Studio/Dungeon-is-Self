using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Data;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject skillObject;
    [SerializeField] private Image skillImage;
    [SerializeField] private GameObject diceObject;
    [SerializeField] private Image diceImage;
    [SerializeField] private Text turnText;
    [SerializeField] private Text hpText;
    [SerializeField] private Transform CCList;
    [SerializeField] private Transform diceList;

    private SpriteRenderer _sprite;
    private Animator _animator;
    private int index;

    private List<CrowdControlIcon> ccIcons = new List<CrowdControlIcon>();
    private List<GameObject> rolledDices = new List<GameObject>();

    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        canvas.worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (skillObject != null) skillObject.SetActive(false);
        if (diceObject != null) diceObject.SetActive(false);
    }

    private void Update()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("IDLE") && coroutine != null)
        {
            StartCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void SetSkill(Skill skill)
    {
        if (skillObject != null) skillObject.SetActive(true);
        skillImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.id);
    }
    public void SetSkill(int id, int turn)
    {
        if (skillObject != null) skillObject.SetActive(true);
        skillImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + id);
        turnText.text = turn.ToString();
    }

    public void UpdateCharacterInfo(int hp, int turn, bool isOff = false)
    {
        if (hpText != null) hpText.text = hp.ToString();
        if (turnText != null) turnText.text = turn.ToString();
        if (isOff && skillObject != null) skillObject.SetActive(false);
    }

    public void UpdateCrowdControl(int id, bool isRemove, int turn, int stack, int dotdmg, CrowdControlIcon prefab = null)
    {
        bool isFind = false;
        for (int i = 0; i < ccIcons.Count; i++)
        {
            if (ccIcons[i].id == id)
            {
                isFind = true;
                if (isRemove)
                {
                    Destroy(ccIcons[i].gameObject);
                    ccIcons.RemoveAt(i);
                }
                else
                {
                    ccIcons[i].SetTurn(turn, stack);
                    ccIcons[i].SetDotDmg(dotdmg);
                }
                break;
            }
        }

        if (isFind == false)
        {
            GameObject obj = Instantiate(prefab.gameObject);
            obj.transform.SetParent(CCList);

            CrowdControlIcon crowdControlIcon = obj.GetComponent<CrowdControlIcon>();
            crowdControlIcon.SetImage(id);
            crowdControlIcon.SetTurn(turn, stack);
            crowdControlIcon.SetDotDmg(dotdmg);

            ccIcons.Add(crowdControlIcon);
        }
    }

    public void UpdateDiceList(List<int> dices, int result, RolledDiceIcon prefab)
    {
        for (int i = 0; i < dices.Count; i++)
        {
            GameObject obj = Instantiate(prefab.gameObject);
            obj.transform.SetParent(diceList);

            RolledDiceIcon dice = obj.GetComponent<RolledDiceIcon>();
            dice.SetImage(dices[i]);

            rolledDices.Add(obj);
        }

        if (result != -1)
        {
            diceObject.SetActive(true);
            diceImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + result);
        }
    }

    public void RemoveDices()
    {
        for (int i = 0; i < rolledDices.Count; i++)
            Destroy(rolledDices[i]);

        rolledDices.Clear();
        diceObject.SetActive(false);
    }

    public void SetCharacterIndex(int n)
    {
        index = n;
    }

    public void SetAnimation(string name)
    {
        _animator.SetTrigger(name);
        coroutine = Animation(name);
    }

    public void SetFlip(bool isFlip)
    {
        _sprite.flipX = isFlip;
    }

    public bool CheckIndex(int i)
    {
        return (index == i);
    }

    public int GetIndex()
    {
        return index;
    }

    IEnumerator Animation(string name)
    {
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName(name.ToUpper())) yield return null;

        float time = 0.2f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        GameController.Instance.AnimationEnd(index);
    }

    public void Dead()
    {
        Destroy(gameObject);
    }
}
