using UnityEngine;

namespace ChineseZombieHunter
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponConfig config;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform firePoint;
        [SerializeField] private ParticleSystem muzzleFlash;

        private float nextFireTime;

        public bool TryFire()
        {
            if (config == null)
            {
                return false;
            }

            float fireDelay = 1f / Mathf.Max(0.01f, config.FireRate);
            if (Time.time < nextFireTime)
            {
                return false;
            }

            nextFireTime = Time.time + fireDelay;

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            if (config.ProjectilePrefab != null && firePoint != null)
            {
                SpawnProjectile();
            }
            else
            {
                FireHitscan();
            }

            return true;
        }

        private void FireHitscan()
        {
            Camera cameraToUse = aimCamera != null ? aimCamera : Camera.main;
            if (cameraToUse == null)
            {
                return;
            }

            Ray ray = cameraToUse.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, config.Range, config.HitMask, QueryTriggerInteraction.Ignore))
            {
                IDamageable damageable = DamageableUtility.FindInParents(hit.collider);
                damageable?.TakeDamage(config.Damage);
            }
        }

        private void SpawnProjectile()
        {
            GameObject projectileObject = Object.Instantiate(config.ProjectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(config.Damage, config.ProjectileSpeed, config.Range, config.HitMask);
            }
        }
    }
}
