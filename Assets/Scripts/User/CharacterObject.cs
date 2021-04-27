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
    private bool isAnimation = false;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        canvas.worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        if (isAnimation)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("ATTACK") && stateInfo.normalizedTime >= 1.0f)
            {
                AnimationEnd();
                isAnimation = false;
            }
        }
    }

    public void SetSkill(Skill skill)
    {
        skillImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skill.name);
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
        isAnimation = true;
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

    public void AnimationEnd()
    {
        GameController.Instance.AnimationEnd(index);
        _animator.SetTrigger("Idle");
    }
}
