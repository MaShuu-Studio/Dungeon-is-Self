using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControl;

namespace Data
{
    public class CharacterDatabase : MonoBehaviour
    {
        public static string facePath { get; private set; } = "Sprites/Classes/Faces/";
        private List<Character> characterDB = new List<Character>();
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
            characterDB.Add(new Character("기사", 101));
            characterDB.Add(new Character("궁수", 102));
            characterDB.Add(new Character("마법사", 103));
        }

        public Character GetCharacter(int id)
        {
            Character copiedCharacter = new Character(characterDB.Find(character => character.id == id));
            return copiedCharacter;
        }

        public void GetAllCharacterCandidatesList(ref List<int> characterRoles)
        {
            characterRoles.Clear();
            foreach (Character character in characterDB)
            {
                characterRoles.Add(character.id);
            }
        }
    }
}