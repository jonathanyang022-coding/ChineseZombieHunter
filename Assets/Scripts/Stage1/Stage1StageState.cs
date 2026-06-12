using System;
using System.Collections.Generic;

namespace ChineseZombieHunter
{
    public enum Stage1Phase
    {
        Lesson,
        Challenge,
        Cleared,
        Failed
    }

    public enum Stage1AnswerResult
    {
        Correct,
        Incorrect,
        Failed
    }

    public sealed class Stage1StageState
    {
        private const int StartingLives = 3;

        private readonly IReadOnlyList<Stage1CharacterEntry> entries;
        private int lessonIndex;
        private int lives;
        private string currentBarrelLabel;
        private Stage1Phase phase;

        private Stage1StageState(IReadOnlyList<Stage1CharacterEntry> entries)
        {
            this.entries = entries ?? throw new ArgumentNullException(nameof(entries));
            if (entries.Count == 0)
            {
                throw new ArgumentException("At least one entry is required.", nameof(entries));
            }

            lives = StartingLives;
            lessonIndex = 0;
            phase = Stage1Phase.Lesson;
        }

        public static Stage1StageState CreateDefault(IReadOnlyList<Stage1CharacterEntry> entries)
        {
            return new Stage1StageState(entries);
        }

        public int Lives => lives;
        public Stage1Phase Phase => phase;
        public Stage1CharacterEntry CurrentLesson => entries[lessonIndex];
        public string CurrentBarrelLabel => currentBarrelLabel;
        public int LessonIndex => lessonIndex;
        public int LessonCount => entries.Count;

        public bool AdvanceLesson()
        {
            if (lessonIndex >= entries.Count - 1)
            {
                return false;
            }

            lessonIndex++;
            return true;
        }

        public void BeginChallenge()
        {
            phase = Stage1Phase.Challenge;
        }

        public void SetCurrentBarrelLabel(string label)
        {
            currentBarrelLabel = label;
        }

        public string GetExpectedCharacterForCurrentBarrel()
        {
            if (string.IsNullOrWhiteSpace(currentBarrelLabel))
            {
                return null;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Meaning == currentBarrelLabel)
                {
                    return entries[i].Character;
                }
            }

            return null;
        }

        public Stage1AnswerResult SubmitAnswer(string chosenCharacter)
        {
            if (phase == Stage1Phase.Failed)
            {
                return Stage1AnswerResult.Failed;
            }

            string expectedCharacter = GetExpectedCharacterForCurrentBarrel();
            if (string.IsNullOrWhiteSpace(expectedCharacter))
            {
                return Stage1AnswerResult.Failed;
            }

            if (string.Equals(chosenCharacter, expectedCharacter, StringComparison.Ordinal))
            {
                return Stage1AnswerResult.Correct;
            }

            lives = Math.Max(0, lives - 1);
            if (lives == 0)
            {
                phase = Stage1Phase.Failed;
                return Stage1AnswerResult.Failed;
            }

            return Stage1AnswerResult.Incorrect;
        }

        public void MarkCleared()
        {
            phase = Stage1Phase.Cleared;
        }

        public void Retry()
        {
            lives = StartingLives;
            lessonIndex = 0;
            currentBarrelLabel = null;
            phase = Stage1Phase.Lesson;
        }
    }
}
