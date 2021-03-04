using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marksman : Offender
{
    private List<Skill> dice = new List<Skill>();
    private List<string> skill = new List<string>();
    public Role marksman = Role.MARKSMAN;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        Skill diceThrow()
    {
        int i = Random.Range(0, 6);
        return dice[i];
    }
    
    void setDice(string skill)
    {
        if(dice.Count >= 6) return;
        else dice.Add(skilldb.GetSkill(skill));
    }
}
