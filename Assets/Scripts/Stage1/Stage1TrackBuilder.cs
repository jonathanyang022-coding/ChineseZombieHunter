using System.Collections.Generic;
using UnityEngine;

public class Stage1TrackBuilder : MonoBehaviour
{
    [SerializeField] private int segmentCount = 16;
    [SerializeField] private float segmentLength = 4f;
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private float trackWidth = 6f;

    [SerializeField] private Color floorColor = new Color(0.22f, 0.23f, 0.26f);
    [SerializeField] private Color obstacleColor = new Color(0.08f, 0.55f, 0.08f);

    private readonly List<Transform> segments = new List<Transform>();
    private readonly List<int> segmentCycles = new List<int>();

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
        ClearTrack();
        BuildTrack();
    }

    private void ClearTrack()
    {
        segments.Clear();
        segmentCycles.Clear();

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
            CreateSegment(i);
        }
    }

    private void CreateSegment(int index)
    {
        GameObject segmentRoot = new GameObject(string.Format("TrackSegment_{0:00}", index));
        segmentRoot.transform.SetParent(transform, false);
        segmentRoot.transform.localPosition = new Vector3(0f, 0f, index * segmentLength);
        segments.Add(segmentRoot.transform);
        segmentCycles.Add(0);

        CreateFloor(segmentRoot.transform);
        CreateObstacleField(segmentRoot.transform, index, 0);
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

    private void CreateObstacleField(Transform parent, int segmentIndex, int cycleIndex)
    {
        if (segmentIndex < 2)
        {
            return;
        }

        float[] xPositions = { -2.0f, -1.0f, 0f, 1.0f, 2.0f };
        float[] zOffsets = { 0.85f, 1.65f, 2.45f, 3.25f };
        int obstacleCount = 2 + ((segmentIndex + cycleIndex) % 3);
        System.Random random = CreateRandom(parent.name, segmentIndex, cycleIndex);
        List<int> availableLanes = new List<int> { 0, 1, 2, 3, 4 };

        for (int i = 0; i < obstacleCount && availableLanes.Count > 0; i++)
        {
            int lanePick = random.Next(availableLanes.Count);
            int laneIndex = availableLanes[lanePick];
            availableLanes.RemoveAt(lanePick);

            int depthPick = random.Next(zOffsets.Length);
            CreateObstacle(parent, xPositions[laneIndex], zOffsets[depthPick]);
        }
    }

    private static System.Random CreateRandom(string segmentName, int segmentIndex, int cycleIndex)
    {
        int seed = (segmentIndex + 1) * 1009 + cycleIndex * 131 + segmentName.GetHashCode();
        return new System.Random(seed);
    }

    private void CreateObstacle(Transform parent, float xPosition, float zPosition)
    {
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.name = "Obstacle";
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
    }

    private void ScrollTrack()
    {
        float moveDistance = scrollSpeed * Time.deltaTime;
        float wrapDistance = segmentCount * segmentLength;
        float recycleThreshold = -segmentLength;

        for (int i = 0; i < segments.Count; i++)
        {
            Transform segment = segments[i];
            Vector3 localPosition = segment.localPosition;
            localPosition.z -= moveDistance;

            while (localPosition.z <= recycleThreshold)
            {
                localPosition.z += wrapDistance;
                segmentCycles[i]++;
                RefreshSegment(segment, i, segmentCycles[i]);
            }

            segment.localPosition = localPosition;
        }
    }

    private void RefreshSegment(Transform segment, int segmentIndex, int cycleIndex)
    {
        for (int i = segment.childCount - 1; i >= 0; i--)
        {
            DestroySegmentChild(segment.GetChild(i).gameObject);
        }

        CreateFloor(segment);
        CreateObstacleField(segment, segmentIndex, cycleIndex);
    }

    private static void DestroySegmentChild(GameObject child)
    {
        if (Application.isPlaying)
        {
            UnityEngine.Object.Destroy(child);
        }
        else
        {
            UnityEngine.Object.DestroyImmediate(child);
        }
    }

    private static void SetColor(GameObject target, Color color)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            Stage1Visuals.SetColor(renderer, color);
        }
    }
}
