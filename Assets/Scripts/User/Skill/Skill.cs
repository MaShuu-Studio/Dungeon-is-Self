using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public int number { get; protected set; }
    public string name { get; protected set; }
    public int turn { get; protected set; }
    public List<int> prior { get; protected set; }
}
