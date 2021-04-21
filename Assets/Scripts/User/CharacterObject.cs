using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControl;

public class CharacterObject : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Animator _animator;
    private int index;
    private bool isAnimation = false;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isAnimation)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"{stateInfo.IsName("ATTACK")} : {stateInfo.normalizedTime}");
            if (stateInfo.IsName("ATTACK") && stateInfo.normalizedTime >= 1.0f)
            {
                Debug.Log("Animation End");
                AnimationEnd();
                isAnimation = false;
            }
        }
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
    }
}
