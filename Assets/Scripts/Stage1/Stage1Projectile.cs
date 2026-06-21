using UnityEngine;

public class Stage1Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 16f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 1;

    private float age;
    private bool hasHit;

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

    private bool TryHitAlongPath(Vector3 startPosition, Vector3 endPosition, Vector3 movement)
    {
        float distance = movement.magnitude;
        if (distance <= 0f)
        {
            return false;
        }

        if (Physics.SphereCast(startPosition, 0.15f, transform.forward, out RaycastHit hitInfo, distance, ~0, QueryTriggerInteraction.Ignore))
        {
            Stage1GreenTileTarget target = hitInfo.collider.GetComponentInParent<Stage1GreenTileTarget>();
            if (target != null && TryConsumeHit())
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
                return true;
            }
        }

        Collider[] overlaps = Physics.OverlapSphere(endPosition, 0.2f, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < overlaps.Length; i++)
        {
            Stage1GreenTileTarget target = overlaps[i].GetComponentInParent<Stage1GreenTileTarget>();
            if (target != null && TryConsumeHit())
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
                return true;
            }
        }

        return false;
    }
}
