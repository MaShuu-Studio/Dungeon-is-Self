using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offender : MonoBehaviour
{
    public SkillDB skilldb = new SkillDB();
    public enum Role {FIGHTER, MARKSMAN, MAGE}
    public GameObject[] bench = new GameObject[3];
    public GameObject[] roster = new GameObject[3];
    public Role offender_role = Role.FIGHTER;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Bench()
    {
        
    }

    protected virtual void Roster()
    {
        
    }
}

public class Fighter : Offender
{
    private List<Skill> dice = new List<Skill>();
    private List<string> skill = new List<string>();
    
    
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

public class Marksman : Offender
{

}

public class Mage : Offender
{

}