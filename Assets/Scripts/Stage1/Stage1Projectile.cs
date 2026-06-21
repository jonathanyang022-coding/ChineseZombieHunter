using UnityEngine;

public class Stage1Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifetime = 2.5f;

    private float deathTime;
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        deathTime = Time.time + lifetime;
    }

    private void OnEnable()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody>();
        }

        if (body != null)
        {
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.linearVelocity = transform.forward * speed;
        }
    }

    private void Update()
    {
        if (Time.time >= deathTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Stage1Obstacle obstacle = other.GetComponentInParent<Stage1Obstacle>();
        if (obstacle == null)
        {
            return;
        }

        obstacle.TakeHit();
        Destroy(gameObject);
    }
}
