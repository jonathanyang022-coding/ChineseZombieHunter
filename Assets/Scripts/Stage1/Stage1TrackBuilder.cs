using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Stage1TrackBuilder : MonoBehaviour
{
    [SerializeField] private int segmentCount = 14;
    [SerializeField] private float segmentLength = 4f;
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private float trackWidth = 6f;
    [SerializeField] private float wallThickness = 0.25f;
    [SerializeField] private float wallHeight = 1.8f;
    [SerializeField] private float laneOffset = 1.6f;

    [SerializeField] private Color floorColor = new Color(0.35f, 0.36f, 0.39f);
    [SerializeField] private Color wallColor = new Color(0.16f, 0.47f, 0.82f);
    [SerializeField] private Color laneMarkColor = new Color(0.82f, 0.82f, 0.82f);
    [SerializeField] private Color obstacleColor = new Color(0.24f, 0.92f, 0.24f);

    private readonly List<Transform> segments = new List<Transform>();

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
        if (!Application.isPlaying)
        {
            return;
        }

        ScrollTrack();
    }

    private void RebuildTrack()
    {
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
            CreateSegment(i);
        }
    }

    private void CreateSegment(int index)
    {
        GameObject segmentRoot = new GameObject(string.Format("TrackSegment_{0:00}", index));
        segmentRoot.transform.SetParent(transform, false);
        segmentRoot.transform.localPosition = new Vector3(0f, 0f, index * segmentLength);
        segments.Add(segmentRoot.transform);

        CreateFloor(segmentRoot.transform);
        CreateWalls(segmentRoot.transform);
        CreateLaneMarkers(segmentRoot.transform);

        if (index == 4 || index == 8 || index == 11)
        {
            CreateObstacle(segmentRoot.transform, index % 3 - 1);
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

    private void CreateWalls(Transform parent)
    {
        float wallX = trackWidth * 0.5f + wallThickness * 0.5f;
        CreateWall(parent, "LeftWall", -wallX);
        CreateWall(parent, "RightWall", wallX);
    }

    private void CreateWall(Transform parent, string wallName, float xPosition)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = wallName;
        wall.transform.SetParent(parent, false);
        wall.transform.localScale = new Vector3(wallThickness, wallHeight, segmentLength);
        wall.transform.localPosition = new Vector3(xPosition, wallHeight * 0.5f, 0f);
        SetColor(wall, wallColor);
    }

    private void CreateLaneMarkers(Transform parent)
    {
        CreateLaneMarker(parent, -laneOffset * 0.5f);
        CreateLaneMarker(parent, laneOffset * 0.5f);
    }

    private void CreateLaneMarker(Transform parent, float xPosition)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = "LaneMarker";
        marker.transform.SetParent(parent, false);
        marker.transform.localScale = new Vector3(0.08f, 0.05f, segmentLength);
        marker.transform.localPosition = new Vector3(xPosition, 0.03f, 0f);
        SetColor(marker, laneMarkColor);
    }

    private void CreateObstacle(Transform parent, int laneIndex)
    {
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.name = "Obstacle";
        obstacle.transform.SetParent(parent, false);
        obstacle.transform.localScale = new Vector3(0.8f, 1.0f, 0.8f);
        obstacle.transform.localPosition = new Vector3(laneIndex * laneOffset, 0.5f, 0.2f);
        SetColor(obstacle, obstacleColor);
    }

    private void ScrollTrack()
    {
        float moveDistance = scrollSpeed * Time.deltaTime;
        float wrapDistance = segmentCount * segmentLength;

        for (int i = 0; i < segments.Count; i++)
        {
            Transform segment = segments[i];
            Vector3 localPosition = segment.localPosition;
            localPosition.z -= moveDistance;

            while (localPosition.z <= -segmentLength)
            {
                localPosition.z += wrapDistance;
            }

            segment.localPosition = localPosition;
        }
    }

    private static void SetColor(GameObject target, Color color)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
