using UnityEngine;

namespace ChineseZombieHunter
{
    public class SimpleEnemyBrain : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private float moveSpeed = 2.2f;
        [SerializeField] private float stoppingDistance = 4f;
        [SerializeField] private float attackRange = 1.6f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackCooldown = 1.2f;

        private float nextAttackTime;

        private void Start()
        {
            if (target == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag(targetTag);
                if (playerObject != null)
                {
                    target = playerObject.transform;
                }
            }
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            Vector3 toTarget = target.position - transform.position;
            float distance = toTarget.z;

            if (distance > stoppingDistance)
            {
                transform.position += Vector3.back * (moveSpeed * Time.deltaTime);

                Vector3 faceTarget = target.position - transform.position;
                faceTarget.y = 0f;
                if (faceTarget.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(faceTarget.normalized);
                }
            }
            else if (distance <= attackRange)
            {
                TryAttack();
            }
        }

        private void TryAttack()
        {
            if (Time.time < nextAttackTime)
            {
                return;
            }

            nextAttackTime = Time.time + attackCooldown;

            IDamageable damageable = DamageableUtility.FindInParents(target);
            damageable?.TakeDamage(Mathf.RoundToInt(attackDamage));
        }
    }
}
