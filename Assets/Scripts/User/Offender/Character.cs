using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private SkillDB skilldb;
    private List<CharacterSkill> dice = new List<CharacterSkill>();
    private List<string> skill = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CharacterSkill DiceThrow()
    {
        int i = Random.Range(0, 6);
        return dice[i];
    }

    protected void SetDice(string skill)
    {
        if(dice.Count >= 6) return;
        else dice.Add(skilldb.GetCharacterSkill(skill));
    }
}