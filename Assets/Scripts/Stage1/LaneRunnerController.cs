using UnityEngine;

public class LaneRunnerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private Vector2 xBounds = new Vector2(-2.4f, 2.4f);
    [SerializeField] private Vector2 zBounds = new Vector2(0.25f, 4.5f);
    [SerializeField] private float fireInterval = 0.6f;
    [SerializeField] private Vector3 projectileOffset = new Vector3(0f, 0.2f, 0.9f);
    [SerializeField] private Color projectileColor = new Color(1f, 0.92f, 0.2f);

    private float fireTimer;
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

        gameManager = FindAnyObjectByType<Stage1GameManager>();
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

        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        movement = Vector3.ClampMagnitude(movement, 1f);
        transform.position += movement * moveSpeed * Time.deltaTime;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, xBounds.x, xBounds.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, zBounds.x, zBounds.y);
        transform.position = clampedPosition;

        fireTimer += Time.deltaTime;
        while (fireTimer >= fireInterval)
        {
            fireTimer -= fireInterval;
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = "Stage1Projectile";
        projectile.transform.position = transform.position + projectileOffset;
        projectile.transform.localScale = Vector3.one * 0.22f;
        projectile.transform.rotation = Quaternion.identity;

        Renderer renderer = projectile.GetComponent<Renderer>();
        if (renderer != null)
        {
            Stage1Visuals.SetColor(renderer, projectileColor);
        }

        SphereCollider collider = projectile.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();
        if (projectileBody == null)
        {
            projectileBody = projectile.AddComponent<Rigidbody>();
        }

        projectileBody.useGravity = false;
        projectileBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        projectileBody.linearVelocity = transform.forward * 18f;

        Stage1Projectile projectileLogic = projectile.GetComponent<Stage1Projectile>();
        if (projectileLogic == null)
        {
            projectile.AddComponent<Stage1Projectile>();
        }
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
