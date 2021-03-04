using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offender : MonoBehaviour
{
    public SkillDB skilldb = new SkillDB();
    public enum Role {FIGHTER, MARKSMAN, MAGE}
    public List<Role> bench = new List<Role>();
    public List<Role> roster = new List<Role>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setBench(Role role)
    {
        if(bench.Count >= 3) return;
        else bench.Add(role);
    }

    protected virtual void setRoster(Role role)
    {
        if(roster.Count >= 3) return;
        else roster.Add(role);
    }
}