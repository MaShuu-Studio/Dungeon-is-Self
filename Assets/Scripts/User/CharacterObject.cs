using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameControl;
using Data;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image skillImage;
    [SerializeField] private Text hpText;
    [SerializeField] private Text turnText;
    [SerializeField] private Transform CCList;

    private SpriteRenderer _sprite;
    private Animator _animator;
    private int index;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        canvas.worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    public void SetSkill(Skill skill)
    {
        skillImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.id);
    }
    public void UpdateCharacterInfo(int hp, int turn)
    {
        if (hpText.text != null) hpText.text = hp.ToString();
        if (turnText.text != null) turnText.text = turn.ToString();
    }

    public void AddCrowdControl()
    {

    }

    public void SetCharacterIndex(int n)
    {
        index = n;
    }

    public void SetAnimation(string name)
    {
        _animator.SetTrigger(name);
        StartCoroutine(Animation(name));
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
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("IDLE")) yield return null;
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName(name.ToUpper())) yield return null;

        float time = 0.2f;
        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        GameController.Instance.AnimationEnd(index);
    }
}
