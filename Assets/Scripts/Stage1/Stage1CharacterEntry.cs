using System;

namespace ChineseZombieHunter
{
    [Serializable]
    public struct Stage1CharacterEntry
    {
        public Stage1CharacterEntry(string character, string meaning)
        {
            Character = character;
            Meaning = meaning;
        }

        public string Character { get; }
        public string Meaning { get; }
    }
}
