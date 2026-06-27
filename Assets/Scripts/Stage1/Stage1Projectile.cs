using UnityEngine;

public class Stage1Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 16f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 1;

    private float age;
    private bool hasHit;
    private Stage1Shooter ownerShooter;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
    }

    public void SetDamage(int newDamage)
    {
        damage = Mathf.Max(1, newDamage);
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetOwnerShooter(Stage1Shooter shooter)
    {
        ownerShooter = shooter;
    }

    public Stage1Shooter GetOwnerShooter()
    {
        return ownerShooter;
    }

    public bool TryConsumeHit()
    {
        if (hasHit)
        {
            return false;
        }

        hasHit = true;
        return true;
    }

    private void Update()
    {
        Vector3 startPosition = transform.position;
        Vector3 movement = transform.forward * (speed * Time.deltaTime);
        Vector3 endPosition = startPosition + movement;

        if (TryHitAlongPath(startPosition, endPosition, movement))
        {
            return;
        }

        transform.position = endPosition;

        age += Time.deltaTime;
        if (age >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHitCollider(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHitCollider(collision.collider);
    }

    private bool TryHitAlongPath(Vector3 startPosition, Vector3 endPosition, Vector3 movement)
    {
        float distance = movement.magnitude;
        if (distance <= 0f)
        {
            return false;
        }

        if (Physics.SphereCast(startPosition, 0.15f, transform.forward, out RaycastHit hitInfo, distance, ~0, QueryTriggerInteraction.Collide))
        {
            if (TryHandleHit(hitInfo.collider, hitInfo.point, transform.forward))
            {
                return true;
            }
        }

        Collider[] overlaps = Physics.OverlapSphere(endPosition, 0.2f, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < overlaps.Length; i++)
        {
            if (TryHandleHit(overlaps[i], endPosition, transform.forward))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryHitCollider(Collider collider)
    {
        if (collider == null || hasHit)
        {
            return false;
        }

        if (TryHandleHit(collider, transform.position, transform.forward))
        {
            return true;
        }

        return false;
    }

    private bool TryHandleHit(Collider collider, Vector3 hitPoint, Vector3 travelDirection)
    {
        Stage1GoalGate goalGate = collider.GetComponentInParent<Stage1GoalGate>();
        if (goalGate != null && !hasHit)
        {
            goalGate.ApplyProjectileHit(this, hitPoint, travelDirection);
            if (TryConsumeHit())
            {
                Destroy(gameObject);
            }
            return true;
        }

        Stage1ClonePickup clonePickup = collider.GetComponentInParent<Stage1ClonePickup>();
        if (clonePickup != null && !hasHit)
        {
            clonePickup.ApplyProjectileHit(this, hitPoint, travelDirection);
            if (TryConsumeHit())
            {
                Destroy(gameObject);
            }
            return true;
        }

        Stage1GreenTileTarget target = collider.GetComponentInParent<Stage1GreenTileTarget>();
        if (target != null && TryConsumeHit())
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
