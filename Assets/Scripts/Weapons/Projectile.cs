using UnityEngine;

namespace ChineseZombieHunter
{
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 4f;

        private int damage;
        private float speed;
        private float range;
        private LayerMask hitMask;
        private Vector3 startPosition;
        private bool initialized;

        public void Initialize(int damageAmount, float projectileSpeed, float maxRange, LayerMask mask)
        {
            damage = damageAmount;
            speed = projectileSpeed;
            range = maxRange;
            hitMask = mask;
            startPosition = transform.position;
            initialized = true;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            transform.position += transform.forward * (speed * Time.deltaTime);

            if (Vector3.Distance(startPosition, transform.position) >= range)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & hitMask.value) == 0)
            {
                return;
            }

            IDamageable damageable = DamageableUtility.FindInParents(other);
            damageable?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
