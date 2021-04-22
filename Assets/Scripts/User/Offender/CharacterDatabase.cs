using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControl;

public class CharacterDatabase : MonoBehaviour
{
    //public static string facePath {get; private set;} = "Sprites/"
    private List<Character> characterDB;
    private static CharacterDatabase instance;
    public static CharacterDatabase Instance
    {
        get
        {
            var obj = FindObjectOfType<CharacterDatabase>();
            instance = obj;
            return instance;
        }
    }
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeDataBase();
    }

    // Update is called once per frame
    private void InitializeDataBase()
    {
        characterDB.Add(new Character(Role.FIGHTER));
        characterDB.Add(new Character(Role.MARKSMAN));
        characterDB.Add(new Character(Role.MAGE));
    }

    public Character GetCharacter(Role role)
    {
        return characterDB.Find(character => character._role == role);
    }

    public void GetAllCharacterCandidatesList(ref List<Role> characterRoles)
    {
        characterRoles.Clear();
        foreach (Character character in characterDB)
        {
            characterRoles.Add(character._role);
        }
    }
}
