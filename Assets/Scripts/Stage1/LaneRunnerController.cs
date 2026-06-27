using UnityEngine;

public class LaneRunnerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private Vector2 xBounds = new Vector2(-2.4f, 2.4f);
    private Rigidbody playerBody;
    private Stage1GameManager gameManager;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        if (playerBody == null)
        {
            playerBody = gameObject.AddComponent<Rigidbody>();
        }

        playerBody.useGravity = false;
        playerBody.isKinematic = true;
        playerBody.constraints = RigidbodyConstraints.FreezeRotation;

        gameManager = Object.FindAnyObjectByType<Stage1GameManager>();
        if (gameManager == null)
        {
            GameObject managerObject = new GameObject("Stage1GameManager");
            gameManager = managerObject.AddComponent<Stage1GameManager>();
        }

    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver)
        {
            return;
        }

        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        movement = Vector3.ClampMagnitude(movement, 1f);
        transform.position += movement * moveSpeed * Time.deltaTime;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, xBounds.x, xBounds.y);
        transform.position = clampedPosition;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameManager != null && gameManager.IsGameOver)
        {
            return;
        }

        if (other.GetComponentInParent<Stage1Obstacle>() != null)
        {
            if (gameManager != null)
            {
                gameManager.TriggerGameOver();
            }
        }
    }
}
