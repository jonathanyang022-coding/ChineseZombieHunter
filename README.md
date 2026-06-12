# ChineseZombieHunter

Unity skeleton for an entertaining mobile learning game about reading and writing Chinese characters.

## What is included

- A clean folder layout for gameplay code
- Core scripts for a splash screen, resource hub, and card-based download page
- A lightweight UI flow that can be wired to mobile controls later
- A resource catalog system for downloadable character sheets, stroke guides, and audio files
- A stage 1 lesson flow that teaches `一`, `二`, and `三` and then runs a one-lane barrel challenge

## Suggested Unity setup

1. Create a new Unity 3D project in Unity Hub.
2. Copy the `Assets/` folder from this repo into the Unity project root.
3. Open the project in Unity.
4. Create these scene objects:
   - `GameFlow` with `JjGamesLauncher`
   - `ResourceHub` with `ResourceHubController`
   - `Stage1Root` with `Stage1Manager`
   - A `Canvas` containing three panels:
     - `SplashPanel`
     - `ResourcesPanel`
     - `Stage1Panel`
   - A `ResourceCatalog` asset with downloadable items
5. Assign references in the inspector.
6. Set the splash panel to show the `J&J Games` title and a playful subtitle.
7. Create a reusable resource card prefab with `ResourceItemView`.
8. Point each resource entry to a downloadable file or web link.
9. Add a `Stage1` panel containing:
   - `CharacterLessonPanel`
   - `BarrelChallengePanel`
   - `LifeDisplay`
   - `StageResultPanel`
   - `Stage1Barrel`
10. Wire `JjGamesLauncher` so the splash can open the resource hub or the stage 1 panel.

## Recommended hierarchy

```text
Scene
  GameFlow
  ResourceHub
  Stage1Root
  Canvas
    SplashPanel
    ResourcesPanel
    Stage1Panel
```

## Next steps

- Add playful character-tracing mini-games
- Add stroke-order practice pages
- Add audio pronunciation downloads
- Add progress badges, unlocks, and streak rewards
- Add a second stage for the next set of Chinese characters

## Notes

- This scaffold is source-first, so Unity will generate `.meta` files and project settings when you open it.
- The launcher uses standard Unity UI components, so it stays easy to customize without extra packages.
- If you keep the older combat scripts around, treat them as legacy prototype code that is no longer part of the learning flow.
- Stage 1 is intentionally designed to be replayable without a hard retry limit.
