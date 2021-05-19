using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public class CharacterDatabase
    {
        static CharacterDatabase instance = new CharacterDatabase();
        public static CharacterDatabase Instance { get { return instance; } }

        private List<Character> characterDB = new List<Character>();
        
        CharacterDatabase()
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