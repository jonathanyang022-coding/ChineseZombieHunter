using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Stage1TrackBuilder : MonoBehaviour
{
    [SerializeField] private int segmentCount = 16;
    [SerializeField] private float segmentLength = 4f;
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private float recycleThresholdFactor = 0.0f;
    [SerializeField] private float trackWidth = 6f;
    [SerializeField] private float wallThickness = 0.25f;
    [SerializeField] private float wallHeight = 1.8f;
    [SerializeField] private float laneOffset = 1.6f;
    [SerializeField] private float trackYOffset = 1f;

    [SerializeField] private Color floorColor = new Color(0.22f, 0.23f, 0.26f);
    [SerializeField] private Color obstacleColor = new Color(0.08f, 0.55f, 0.08f);
    [SerializeField] private Color clonePickupColor = new Color(0.35f, 0.65f, 1f);
    [SerializeField] private Color goalGateColor = new Color(1f, 0.55f, 0.12f);

    private readonly List<Transform> segments = new List<Transform>();
    private int nextSegmentId;

    private void OnEnable()
    {
        RebuildTrack();
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall -= DeferredRebuildTrack;
            EditorApplication.delayCall += DeferredRebuildTrack;
            return;
        }
#endif

        RebuildTrack();
    }

    private void Awake()
    {
        RebuildTrack();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            ScrollTrack();
        }
    }

    private void RebuildTrack()
    {
        nextSegmentId = 0;
        ClearTrack();
        BuildTrack();
    }

    private void ClearTrack()
    {
        segments.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void BuildTrack()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            CreateSegment(nextSegmentId++, i * GetSegmentStep());
        }
    }

    private void CreateSegment(int segmentId, float zPosition)
    {
        GameObject segmentRoot = new GameObject(string.Format("TrackSegment_{0:00}", segmentId));
        segmentRoot.transform.SetParent(transform, false);
        segmentRoot.transform.localPosition = new Vector3(0f, trackYOffset, zPosition);
        segments.Add(segmentRoot.transform);

        CreateFloor(segmentRoot.transform);
        CreateWalls(segmentRoot.transform);
        CreateLaneMarkers(segmentRoot.transform);
        CreateEnemyLaneWave(segmentRoot.transform, segmentId);
        CreateCloneLaneWave(segmentRoot.transform, segmentId);
        CreateGoalGateWave(segmentRoot.transform, segmentId);
    }

    private void CreateFloor(Transform parent)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(parent, false);
        floor.transform.localScale = new Vector3(trackWidth, 0.2f, segmentLength);
        floor.transform.localPosition = new Vector3(0f, -0.1f, 0f);
        SetColor(floor, floorColor);
    }

    private void CreateWalls(Transform parent)
    {
        float halfWidth = trackWidth * 0.5f + wallThickness * 0.5f;

        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "LeftWall";
        leftWall.transform.SetParent(parent, false);
        leftWall.transform.localScale = new Vector3(wallThickness, wallHeight, segmentLength);
        leftWall.transform.localPosition = new Vector3(-halfWidth, wallHeight * 0.5f, 0f);
        SetColor(leftWall, new Color(0.65f, 0.65f, 0.68f));

        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "RightWall";
        rightWall.transform.SetParent(parent, false);
        rightWall.transform.localScale = new Vector3(wallThickness, wallHeight, segmentLength);
        rightWall.transform.localPosition = new Vector3(halfWidth, wallHeight * 0.5f, 0f);
        SetColor(rightWall, new Color(0.65f, 0.65f, 0.68f));
    }

    private void CreateLaneMarkers(Transform parent)
    {
        for (int laneIndex = -1; laneIndex <= 1; laneIndex++)
        {
            if (laneIndex == 0)
            {
                continue;
            }

            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = string.Format("LaneMarker_{0}", laneIndex);
            marker.transform.SetParent(parent, false);
            marker.transform.localScale = new Vector3(0.05f, 0.02f, segmentLength);
            marker.transform.localPosition = new Vector3(laneIndex * laneOffset, 0.01f, 0f);
            SetColor(marker, new Color(0.72f, 0.72f, 0.72f));
        }
    }

    private void CreateObstacle(Transform parent, float xPosition, float zPosition)
    {
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.name = "GreenTile";
        obstacle.transform.SetParent(parent, false);
        obstacle.transform.localScale = new Vector3(0.8f, 1.0f, 0.8f);
        obstacle.transform.localPosition = new Vector3(xPosition, 0.5f, zPosition);

        Collider collider = obstacle.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        if (obstacle.GetComponent<Stage1Obstacle>() == null)
        {
            obstacle.AddComponent<Stage1Obstacle>();
        }

        SetColor(obstacle, obstacleColor);
        obstacle.AddComponent<Stage1GreenTileTarget>();
    }

    private void CreateObstacle(Transform parent, int laneIndex)
    {
        float xPosition = laneIndex * laneOffset;
        float zPosition = segmentLength * 0.5f;
        CreateObstacle(parent, xPosition, zPosition);
    }

    private void CreateObstacle(Transform parent, int laneIndex, float zPosition)
    {
        float xPosition = laneIndex * laneOffset;
        CreateObstacle(parent, xPosition, zPosition);
    }

    private void CreateEnemyLaneWave(Transform parent, int segmentId)
    {
        if (segmentId < 2)
        {
            return;
        }

        if (segmentId % 2 != 0)
        {
            return;
        }

        CreateObstacle(parent, 0, segmentLength * 0.42f);

        if (segmentId % 6 == 0)
        {
            CreateObstacle(parent, 0, segmentLength * 0.74f);
        }
    }

    private void CreateCloneLaneWave(Transform parent, int segmentId)
    {
        if (segmentId != 1 && segmentId % 5 != 0)
        {
            return;
        }

        CreateClonePickup(parent, -1, segmentLength * 0.28f);
    }

    private void CreateGoalGateWave(Transform parent, int segmentId)
    {
        if (segmentId != 2 && segmentId % 3 != 1)
        {
            return;
        }

        CreateGoalGate(parent, 1, segmentLength * 0.62f);
    }

    private void CreateClonePickup(Transform parent, int laneIndex)
    {
        CreateClonePickup(parent, laneIndex, segmentLength * 0.38f);
    }

    private void CreateClonePickup(Transform parent, int laneIndex, float zPosition)
    {
        GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickup.name = "ClonePickup";
        pickup.transform.SetParent(parent, false);
        pickup.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        pickup.transform.localPosition = new Vector3(laneIndex * laneOffset, 0.45f, zPosition);

        Collider collider = pickup.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        Rigidbody body = pickup.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = pickup.AddComponent<Rigidbody>();
        }

        body.useGravity = false;
        body.isKinematic = true;

        SetColor(pickup, clonePickupColor);

        if (pickup.GetComponent<Stage1ClonePickup>() == null)
        {
            pickup.AddComponent<Stage1ClonePickup>();
        }
    }

    private void CreateGoalGate(Transform parent, int laneIndex)
    {
        CreateGoalGate(parent, laneIndex, segmentLength * 0.55f);
    }

    private void CreateGoalGate(Transform parent, int laneIndex, float zPosition)
    {
        GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gate.name = "GoalGate";
        gate.transform.SetParent(parent, false);
        gate.transform.localScale = new Vector3(0.9f, wallHeight * 1.45f, 0.8f);
        gate.transform.localPosition = new Vector3(laneIndex * laneOffset, wallHeight * 0.725f, zPosition);

        Collider collider = gate.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        Rigidbody body = gate.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = gate.AddComponent<Rigidbody>();
        }

        body.useGravity = false;
        body.isKinematic = true;

        SetColor(gate, goalGateColor);

        if (gate.GetComponent<Stage1GoalGate>() == null)
        {
            gate.AddComponent<Stage1GoalGate>();
        }
    }

    private void ScrollTrack()
    {
        float step = GetSegmentStep();
        float moveDistance = scrollSpeed * Time.deltaTime;
        float recycleThreshold = -segmentLength * Mathf.Clamp01(recycleThresholdFactor);

        for (int i = 0; i < segments.Count; i++)
        {
            Transform segment = segments[i];
            Vector3 localPosition = segment.localPosition;
            localPosition.z -= moveDistance;
            segment.localPosition = localPosition;
        }

        while (segments.Count > 0 && segments[0].localPosition.z <= recycleThreshold)
        {
            RecycleFrontSegment(step);
        }
    }

    private float GetSegmentStep()
    {
        return Mathf.Max(0.01f, segmentLength);
    }

    private void RecycleFrontSegment(float step)
    {
        if (segments.Count <= 1)
        {
            return;
        }

        Transform recycledSegment = segments[0];
        segments.RemoveAt(0);

        Transform lastSegment = segments[segments.Count - 1];
        float recycledZ = lastSegment.localPosition.z + step;

        recycledSegment.gameObject.SetActive(false);
        Destroy(recycledSegment.gameObject);

        CreateSegment(nextSegmentId++, recycledZ);
    }

    private static void SetColor(GameObject target, Color color)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            Stage1Visuals.SetColor(renderer, color);
        }
    }

#if UNITY_EDITOR
    private void DeferredRebuildTrack()
    {
        if (this == null)
        {
            return;
        }

        EditorApplication.delayCall -= DeferredRebuildTrack;
        if (!Application.isPlaying)
        {
            RebuildTrack();
        }
    }
#endif
}
