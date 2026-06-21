using System.Collections.Generic;
using UnityEngine;

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

        int obstaclePattern = segmentId % 9;
        if (obstaclePattern == 4 || obstaclePattern == 7 || obstaclePattern == 8)
        {
            CreateObstacle(segmentRoot.transform, (segmentId % 3) - 1);
        }
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

        if (Application.isPlaying)
        {
            recycledSegment.gameObject.SetActive(false);
            Destroy(recycledSegment.gameObject);
        }
        else
        {
            DestroyImmediate(recycledSegment.gameObject);
        }

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
}
