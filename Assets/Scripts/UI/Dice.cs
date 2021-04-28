using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControl;

public class Dice : MonoBehaviour
{
    [SerializeField] private SpriteRenderer skillSprite;
    private Animator _animator;
    private int skillId;
    private bool isShowed = false;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        skillSprite.gameObject.SetActive(false);
        isShowed = false;
        skillId = 0;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("ROLL") && stateInfo.normalizedTime >= 1f)
        {
            if (isShowed == false) ShowSkill();
            if (stateInfo.normalizedTime >= 3f) AnimationEnd();
        }
    }
    
    private void ShowSkill()
    {
        isShowed = true;
        skillSprite.sprite = Resources.Load<Sprite>("Sprites/Skills/" + skillId.ToString());
        skillSprite.gameObject.SetActive(true);
    }

    public void SetSkill(int id)
    {
        skillId = id;
    }

    public void Roll()
    {
        _animator.SetTrigger("Roll");
    }

    public void AnimationEnd()
    {
        GameController.Instance.DiceRolled();
        Destroy(gameObject);
    }
}
