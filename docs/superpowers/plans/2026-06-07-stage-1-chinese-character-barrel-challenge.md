# Stage 1 Chinese Character Barrel Challenge Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build stage 1 of the learning game: teach `一`, `二`, `三` with their meanings, then run a one-lane zombie barrel challenge where the player has three lives and can retry forever.

**Architecture:** Keep the gameplay split into a small pure C# state machine and a thin Unity UI/controller layer. The pure state machine owns lesson order, barrel-label selection, answer checking, lives, failure, and retry reset so it can be unit tested without Unity. Unity MonoBehaviours will only render lesson cards, route button clicks, and switch between splash, lesson, challenge, and result panels.

**Tech Stack:** Unity C#, Unity UI (`Canvas`, `Button`, `Text`), NUnit/EditMode tests for pure logic, ScriptableObject or serialized data for the three-character mapping.

---

### Task 1: Add the stage 1 core logic and unit tests

**Files:**
- Create: `Assets/Scripts/Stage1/Stage1CharacterEntry.cs`
- Create: `Assets/Scripts/Stage1/Stage1StageState.cs`
- Create: `Assets/Tests/EditMode/Stage1StageStateTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using NUnit.Framework;

namespace ChineseZombieHunter.Tests
{
    public class Stage1StageStateTests
    {
        [Test]
        public void StartStage_InitializesThreeLives_AndLessonOrder()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            Assert.AreEqual(3, state.Lives);
            Assert.AreEqual(Stage1Phase.Lesson, state.Phase);
            Assert.AreEqual("一", state.CurrentLesson.Character);
            Assert.AreEqual("1", state.CurrentLesson.Meaning);
        }

        [Test]
        public void SubmitAnswer_CorrectChoice_ClearsBarrelAndKeepsLives()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("2");
            var result = state.SubmitAnswer("二");

            Assert.AreEqual(Stage1AnswerResult.Correct, result);
            Assert.AreEqual(3, state.Lives);
            Assert.AreEqual(Stage1Phase.Challenge, state.Phase);
        }

        [Test]
        public void SubmitAnswer_WrongChoice_LosesOneLife()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("3");
            var result = state.SubmitAnswer("一");

            Assert.AreEqual(Stage1AnswerResult.Incorrect, result);
            Assert.AreEqual(2, state.Lives);
            Assert.AreEqual(Stage1Phase.Challenge, state.Phase);
        }

        [Test]
        public void SubmitAnswer_WrongChoiceAtZeroLives_EndsInFailure()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("1");
            state.SubmitAnswer("二");
            state.SetCurrentBarrelLabel("2");
            state.SubmitAnswer("三");
            state.SetCurrentBarrelLabel("3");
            var result = state.SubmitAnswer("一");

            Assert.AreEqual(Stage1AnswerResult.Failed, result);
            Assert.AreEqual(0, state.Lives);
            Assert.AreEqual(Stage1Phase.Failed, state.Phase);
        }

        [Test]
        public void Retry_ResetsLivesAndReturnsToLessonPhase()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("1");
            state.SubmitAnswer("二");
            state.Retry();

            Assert.AreEqual(3, state.Lives);
            Assert.AreEqual(Stage1Phase.Lesson, state.Phase);
            Assert.AreEqual("一", state.CurrentLesson.Character);
        }
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test` or the Unity EditMode test runner equivalent for `Assets/Tests/EditMode/Stage1StageStateTests.cs`

Expected: FAIL because `Stage1StageState`, `Stage1CharacterEntry`, and the enum types do not exist yet.

- [ ] **Step 3: Write minimal implementation**

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IReadOnlyList<Stage1CharacterEntry> entries;
        private int lessonIndex;
        private int lives;
        private string currentBarrelLabel;

        private Stage1StageState(IReadOnlyList<Stage1CharacterEntry> entries)
        {
            this.entries = entries;
            lives = 3;
            Phase = Stage1Phase.Lesson;
        }

        public static Stage1StageState CreateDefault(IReadOnlyList<Stage1CharacterEntry> entries)
        {
            return new Stage1StageState(entries);
        }

        public int Lives => lives;
        public Stage1Phase Phase { get; private set; }
        public Stage1CharacterEntry CurrentLesson => entries[Mathf.Clamp(lessonIndex, 0, entries.Count - 1)];

        public void BeginChallenge()
        {
            Phase = Stage1Phase.Challenge;
        }

        public void SetCurrentBarrelLabel(string label)
        {
            currentBarrelLabel = label;
        }

        public Stage1AnswerResult SubmitAnswer(string character)
        {
            if (Phase == Stage1Phase.Failed)
            {
                return Stage1AnswerResult.Failed;
            }

            if (character == currentBarrelLabel)
            {
                return Stage1AnswerResult.Correct;
            }

            lives = Math.Max(0, lives - 1);
            if (lives == 0)
            {
                Phase = Stage1Phase.Failed;
                return Stage1AnswerResult.Failed;
            }

            return Stage1AnswerResult.Incorrect;
        }

        public void Retry()
        {
            lives = 3;
            lessonIndex = 0;
            currentBarrelLabel = null;
            Phase = Stage1Phase.Lesson;
        }
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test` or the Unity EditMode test runner equivalent for `Assets/Tests/EditMode/Stage1StageStateTests.cs`

Expected: PASS with all assertions green.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Stage1/Stage1CharacterEntry.cs Assets/Scripts/Stage1/Stage1StageState.cs Assets/Tests/EditMode/Stage1StageStateTests.cs
git commit -m "feat: add stage 1 state logic"
```

### Task 2: Build the stage 1 Unity UI and gameplay flow

**Files:**
- Create: `Assets/Scripts/Stage1/Stage1Manager.cs`
- Create: `Assets/Scripts/Stage1/CharacterLessonPanel.cs`
- Create: `Assets/Scripts/Stage1/BarrelChallengePanel.cs`
- Create: `Assets/Scripts/Stage1/LifeDisplay.cs`
- Create: `Assets/Scripts/Stage1/StageResultPanel.cs`
- Create: `Assets/Scripts/Stage1/Stage1Barrel.cs`
- Modify: `Assets/Scripts/UI/JjGamesLauncher.cs`

- [ ] **Step 1: Write the failing test**

Create a small PlayMode-independent controller test for the manager’s startup flow:

```csharp
using NUnit.Framework;

namespace ChineseZombieHunter.Tests
{
    public class Stage1ManagerTests
    {
        [Test]
        public void CreateDefaultFlow_StartsOnLessonOne()
        {
            var manager = Stage1Manager.CreateForTests();

            Assert.AreEqual("一", manager.CurrentLessonCharacter);
            Assert.AreEqual("1", manager.CurrentLessonMeaning);
            Assert.AreEqual(3, manager.CurrentLives);
        }
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test` or the Unity EditMode test runner equivalent for `Assets/Tests/EditMode/Stage1ManagerTests.cs`

Expected: FAIL because `Stage1Manager.CreateForTests()` does not exist yet.

- [ ] **Step 3: Write minimal implementation**

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class Stage1Manager : MonoBehaviour
    {
        [SerializeField] private CharacterLessonPanel lessonPanel;
        [SerializeField] private BarrelChallengePanel challengePanel;
        [SerializeField] private LifeDisplay lifeDisplay;
        [SerializeField] private StageResultPanel resultPanel;
        [SerializeField] private Stage1Barrel barrel;

        private Stage1StageState state;

        public static Stage1Manager CreateForTests()
        {
            var manager = new GameObject("Stage1Manager").AddComponent<Stage1Manager>();
            manager.state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });
            return manager;
        }

        public string CurrentLessonCharacter => state.CurrentLesson.Character;
        public string CurrentLessonMeaning => state.CurrentLesson.Meaning;
        public int CurrentLives => state.Lives;
    }
}
```

Then expand it to:

```csharp
public void StartStage()
public void ShowNextLesson()
public void BeginChallenge()
public void OnChoiceSelected(string chosenCharacter)
public void RetryStage()
```

The manager should:

- show the three intro cards in order
- move to the barrel challenge after the lesson flow
- choose barrel labels from `一`, `二`, `三` mapped to `1`, `2`, `3`
- subtract lives on wrong answers
- trigger barrel explode / zombie clear on correct answers
- show a fail panel at zero lives
- support unlimited retries by resetting state and restarting the lesson flow

The panel scripts should stay dumb:

- `CharacterLessonPanel` only renders a character, meaning, and prompt
- `BarrelChallengePanel` only renders the current barrel label and three buttons
- `LifeDisplay` only updates the visual life indicators
- `StageResultPanel` only shows retry/success/failure controls
- `Stage1Barrel` only stores the current label and exposes `Explode()`

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test` or the Unity EditMode test runner equivalent for the manager test and any added pure-logic tests.

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Stage1 Assets/Scripts/UI/JjGamesLauncher.cs
git commit -m "feat: add stage 1 ui flow"
```

### Task 3: Wire launcher navigation and update documentation

**Files:**
- Modify: `Assets/Scripts/UI/JjGamesLauncher.cs`
- Modify: `README.md`
- Modify: `docs/features/2026-06-07-stage-1-chinese-character-barrel-challenge.md`
- Create or modify: `docs/superpowers/plans/2026-06-07-stage-1-chinese-character-barrel-challenge.md` only if the implementation reveals a spec mismatch

- [ ] **Step 1: Write the failing test**

Launcher navigation is mostly Unity UI wiring, so the important verification is the inspector binding itself:

```csharp
// continueButton -> ShowResources()
// stage1Button -> ShowStage1()
// backToResourcesButton -> ShowResources()
```

- [ ] **Step 2: Run test to verify it fails**

Run: `git diff --check`

Expected: PASS with no whitespace or formatting errors.

- [ ] **Step 3: Write minimal implementation**

Update the launcher so it can show:

- splash panel
- resources panel
- stage 1 entry button or stage 1 panel button

Update the README to explain:

- how to open the stage 1 scene flow
- how to wire the three lesson cards
- how to wire the three answer buttons
- how to set up the one-lane barrel encounter

- [ ] **Step 4: Run test to verify it passes**

Run: `git diff --check` and the available Unity/EditMode tests.

Expected: no whitespace errors, no broken references in the updated docs, and all available tests pass.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/UI/JjGamesLauncher.cs README.md docs/features/2026-06-07-stage-1-chinese-character-barrel-challenge.md
git commit -m "docs: finalize stage 1 implementation wiring"
```

## Spec Coverage Check

- Character recognition and meanings: Task 1 and Task 2
- One-lane barrel challenge: Task 2
- Correct/wrong answer behavior: Task 1 and Task 2
- Three lives and retry loop: Task 1 and Task 2
- Playful game-like presentation: Task 2 and Task 3
- Documentation and versioning: Task 3

## Risks / Notes

- The repository currently does not contain a Unity project file set, so the implementation may need to keep the logic split into pure C# classes that are easy to drop into a Unity project.
- If any Unity UI type or test harness is unavailable in the current workspace, prefer keeping the logic in pure classes and make the MonoBehaviours thin wrappers around that logic.
