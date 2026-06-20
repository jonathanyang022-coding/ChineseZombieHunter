# Stage 1 Lane Runner Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a minimal portrait lane runner scene with a fixed forward camera, modular road pieces, and basic lane-switching movement.

**Architecture:** Keep the first pass primitive-based and scene-driven. A single scene owns the track layout, a small `LaneRunnerController` handles lane movement, and a `Stage1Bootstrap` script sets up the camera framing and scene constants. This keeps the system tiny now while preserving the Roll a Ball-style modularity for later prefabs.

**Tech Stack:** Unity 6000.4.10f1, built-in 3D primitives, C# scripts, Unity scene/prefab assets.

---

### Task 1: Create the Stage 1 project structure and test scene

**Files:**
- Create: `Assets/Scenes/Stage1.unity`
- Create: `Assets/Scenes/Stage1.unity.meta`
- Create: `Assets/Scripts/Stage1/Stage1Bootstrap.cs`
- Create: `Assets/Scripts/Stage1/Stage1Bootstrap.cs.meta`

- [ ] **Step 1: Create the scene and bootstrap script**

```csharp
using UnityEngine;

public class Stage1Bootstrap : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    private void Awake()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        if (sceneCamera != null)
        {
            sceneCamera.transform.SetPositionAndRotation(new Vector3(0f, 3.5f, -8f), Quaternion.Euler(12f, 0f, 0f));
        }
    }
}
```

- [ ] **Step 2: Add the scene to build settings**

```yaml
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
PlayerSettings:
  m_CompanyName: DefaultCompany
```

- [ ] **Step 3: Verify the new files exist**

Run: `find Assets -maxdepth 3 -type f | sort`
Expected: `Assets/Scenes/Stage1.unity` and `Assets/Scripts/Stage1/Stage1Bootstrap.cs` are listed.

### Task 2: Add the lane runner controller

**Files:**
- Create: `Assets/Scripts/Stage1/LaneRunnerController.cs`
- Create: `Assets/Scripts/Stage1/LaneRunnerController.cs.meta`

- [ ] **Step 1: Write the controller**

```csharp
using UnityEngine;

public class LaneRunnerController : MonoBehaviour
{
    [SerializeField] private float laneOffset = 1.5f;
    [SerializeField] private float laneChangeSpeed = 12f;

    private int targetLaneIndex;
    private float currentLaneX;

    private void Start()
    {
        currentLaneX = transform.position.x;
        targetLaneIndex = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetLaneIndex = Mathf.Max(0, targetLaneIndex - 1);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetLaneIndex = Mathf.Min(2, targetLaneIndex + 1);
        }

        float targetX = (targetLaneIndex - 1) * laneOffset;
        currentLaneX = Mathf.MoveTowards(currentLaneX, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = new Vector3(currentLaneX, transform.position.y, transform.position.z);
    }
}
```

- [ ] **Step 2: Attach the controller to the player placeholder in the scene**

### Task 3: Build the road, walls, and placeholder obstacles

**Files:**
- Create: `Assets/Scripts/Stage1/Stage1TrackBuilder.cs`
- Create: `Assets/Scripts/Stage1/Stage1TrackBuilder.cs.meta`

- [ ] **Step 1: Write the track builder**

```csharp
using UnityEngine;

public class Stage1TrackBuilder : MonoBehaviour
{
    [SerializeField] private int segmentCount = 12;
    [SerializeField] private float segmentLength = 4f;

    private void Awake()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            CreateFloorSegment(i);
        }
    }

    private void CreateFloorSegment(int index)
    {
        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.transform.SetParent(transform, false);
        segment.transform.localScale = new Vector3(6f, 0.2f, segmentLength);
        segment.transform.localPosition = new Vector3(0f, -0.1f, index * segmentLength);
    }
}
```

- [ ] **Step 2: Add simple wall and obstacle placeholders in the scene**

### Task 4: Verify the first playable loop

**Files:**
- Modify: `ProjectSettings/EditorBuildSettings.asset`
- Modify: `README.md`

- [ ] **Step 1: Set `Stage1.unity` as the active scene in build settings**
- [ ] **Step 2: Update `README.md` with a one-line Stage 1 note**
- [ ] **Step 3: Open the scene in Unity and confirm the player can switch lanes**

