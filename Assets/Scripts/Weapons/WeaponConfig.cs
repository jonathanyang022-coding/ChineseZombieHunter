using UnityEngine;

namespace ChineseZombieHunter
{
    [CreateAssetMenu(menuName = "ChineseZombieHunter/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private string weaponName = "Pistol";
        [SerializeField] private int damage = 20;
        [SerializeField] private float fireRate = 4f;
        [SerializeField] private float range = 120f;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 60f;

        public string WeaponName => weaponName;
        public int Damage => damage;
        public float FireRate => fireRate;
        public float Range => range;
        public LayerMask HitMask => hitMask;
        public GameObject ProjectilePrefab => projectilePrefab;
        public float ProjectileSpeed => projectileSpeed;
    }
}

