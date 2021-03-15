using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public SkillDB skilldb = new SkillDB();
    private List<Skill> dice = new List<Skill>();
    private List<string> skill = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Skill DiceThrow()
    {
        int i = Random.Range(0, 6);
        return dice[i];
    }

    protected void SetDice(string skill)
    {
        if(dice.Count >= 6) return;
        else dice.Add(skilldb.GetSkill(skill));
    }
}