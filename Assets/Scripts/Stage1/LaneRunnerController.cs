using UnityEngine;

public class LaneRunnerController : MonoBehaviour
{
    [SerializeField] private float laneOffset = 1.6f;
    [SerializeField] private float laneChangeSpeed = 12f;

    private int targetLaneIndex = 1;
    private float currentLaneX;

    private void Start()
    {
        currentLaneX = transform.position.x;
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

        float targetLaneX = (targetLaneIndex - 1) * laneOffset;
        currentLaneX = Mathf.MoveTowards(currentLaneX, targetLaneX, laneChangeSpeed * Time.deltaTime);
        transform.position = new Vector3(currentLaneX, transform.position.y, transform.position.z);
    }
}

