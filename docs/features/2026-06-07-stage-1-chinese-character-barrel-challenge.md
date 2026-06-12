# Stage 1: Chinese Character Barrel Challenge

Version: `0.1.0`

## Summary

Stage 1 teaches the player to recognize and understand the Chinese characters `一`, `二`, and `三` by pairing them with their meanings `1`, `2`, and `3`, then immediately applying that knowledge in a one-lane zombie barrel challenge.

The stage should feel playful, fast, and game-like. It should not feel like a formal education app.

## Goals

- Teach the player to recognize `一`, `二`, and `三`.
- Teach the meaning of each character as the numbers `1`, `2`, and `3`.
- Reinforce the lesson in a simple action challenge.
- Give the player immediate feedback for correct and incorrect answers.
- Allow unlimited retries so the player can replay the stage as often as needed.

## High-Level Functional Requirements

### 1. Character introduction

- The stage must introduce exactly three characters: `一`, `二`, and `三`.
- The stage must show the meaning of each character as `1`, `2`, and `3`.
- The teaching flow should happen before the zombie challenge begins.
- The presentation should be simple and readable on mobile screens.
- The lesson should use playful game-style UI language and visuals.

### 2. One-lane zombie challenge

- The actual challenge must use only one lane.
- Zombies must move slowly toward the player while pushing a barrel.
- Each barrel must display exactly one of these labels: `1`, `2`, or `3`.
- The label on the barrel must be randomized per encounter.
- The stage must present a short sequence of three barrel encounters and clear when the player answers all three correctly.
- The player must be shown exactly three choice buttons:
  - `一`
  - `二`
  - `三`
- The player selects the Chinese character that matches the barrel label.

### 3. Correct answer behavior

- If the player selects the correct character, the barrel must explode.
- When the barrel explodes, zombies behind it must die.
- The stage should give clear positive feedback for a correct answer.

### 4. Wrong answer behavior

- If the player selects the wrong character, the player loses one life.
- The player must start the stage with three lives.
- The stage must fail when the player reaches zero lives.
- The player should receive clear feedback that the answer was wrong.

### 5. Retry behavior

- The player must be able to retry the stage unlimited times.
- After stage failure, the retry flow must be available immediately.
- Retrying should restart the stage state cleanly, including lives and encounter state.

## Lightweight Planning / Implementation Details

### Suggested stage flow

1. Show a short intro card for `一 = 1`.
2. Show a short intro card for `二 = 2`.
3. Show a short intro card for `三 = 3`.
4. Transition into the one-lane challenge.
5. Spawn one zombie barrel encounter at a time or in a simple repeatable sequence.
6. Present the three character buttons.
7. Resolve the choice.
8. Repeat until the stage is cleared or the player loses all lives.

### Suggested Unity structure

- `Stage1Manager`
  - owns the lesson flow, encounter flow, lives, win/fail state, and retries
- `CharacterLessonPanel`
  - shows the character, number meaning, and a short prompt
- `BarrelChallengePanel`
  - shows the lane challenge, current barrel label, and the three answer buttons
- `LifeDisplay`
  - shows three lives visually
- `StageResultPanel`
  - shows success/failure and retry actions

### Suggested data model

- A small data object should map each character to its meaning:
  - `一` -> `1`
  - `二` -> `2`
  - `三` -> `3`
- Barrel labels should be generated from the same mapping so the lesson and challenge stay aligned.

### Suggested presentation tone

- Use bold, playful UI.
- Avoid classroom-style wording like “lesson” or “module” in the player-facing UI.
- Prefer words like `tap`, `choose`, `unlock`, `try again`, and `level up`.

### Out of scope for stage 1

- Multiple lanes
- Timers
- Combo scoring
- Progression systems
- Additional Chinese characters
- Complex enemy AI
- Meta economy or upgrades

## Acceptance Criteria

- The stage introduces `一`, `二`, and `三` before gameplay starts.
- The stage clearly teaches that `一 = 1`, `二 = 2`, and `三 = 3`.
- The challenge uses one lane only.
- Barrels display a random label chosen from `1`, `2`, or `3`.
- The stage clears after the player correctly answers all three barrel encounters.
- The player is given exactly three answer buttons: `一`, `二`, and `三`.
- Correct answers explode the barrel and kill the zombies behind it.
- Wrong answers subtract one life.
- The player begins with three lives.
- The player fails the stage at zero lives.
- The player can retry the stage any number of times.
- The implementation remains playful and game-like rather than formal or academic.
