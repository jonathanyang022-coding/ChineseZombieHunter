using UnityEngine;

public class Stage1Shooter : MonoBehaviour
{
    [SerializeField] private KeyCode fireKey = KeyCode.Space;
    [SerializeField] private float projectileSpeed = 16f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private Vector3 fireOffset = new Vector3(0f, -0.45f, 1.0f);
    [SerializeField] private float projectileForwardSpacing = 1f;
    [SerializeField] private int maxProjectilesPerShot = 8;
    [SerializeField] private Color projectileColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private string fireButtonLabel = "AUTO FIRE";
    [SerializeField] private string fireButtonActiveLabel = "AUTO FIRE ON";
    [SerializeField] private Vector2 fireButtonSize = new Vector2(140f, 72f);
    [SerializeField] private Vector2 fireButtonMargin = new Vector2(24f, 24f);

    private float nextFireTime;
    private int baseProjectileCount = 1;
    private bool autoFireEnabled;
    private Stage1PlayerCloneEffect cloneEffect;

    private void Awake()
    {
        cloneEffect = GetComponent<Stage1PlayerCloneEffect>();
        autoFireEnabled = true;
    }

    private void Start()
    {
        maxProjectilesPerShot = Mathf.Max(1, maxProjectilesPerShot);
        baseProjectileCount = Mathf.Clamp(baseProjectileCount, 1, maxProjectilesPerShot);

        autoFireEnabled = true;
    }

    public void AddClones(int cloneCount)
    {
        IncreaseBaseProjectileCount(cloneCount);
    }

    public void SetBaseProjectileCount(int count)
    {
        baseProjectileCount = Mathf.Clamp(Mathf.Max(1, count), 1, maxProjectilesPerShot);
        SyncCloneEffect();
    }

    public void IncreaseBaseProjectileCount(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        baseProjectileCount = Mathf.Clamp(baseProjectileCount + amount, 1, maxProjectilesPerShot);
        SyncCloneEffect();
    }

    public void MultiplyShooters(int multiplier)
    {
        if (multiplier <= 1)
        {
            return;
        }

        baseProjectileCount = Mathf.Clamp(baseProjectileCount * multiplier, 1, maxProjectilesPerShot);
        SyncCloneEffect();
    }

    public void SpawnProjectileClones(Vector3 impactPoint, Vector3 forwardDirection, int projectileCount)
    {
        Vector3 normalizedForward = forwardDirection.sqrMagnitude > 0.0001f ? forwardDirection.normalized : transform.forward;
        Vector3 spawnPosition = impactPoint + normalizedForward * 0.28f;
        SpawnProjectile(spawnPosition, normalizedForward);
    }

    private Stage1PlayerCloneEffect GetCloneEffect()
    {
        if (cloneEffect == null)
        {
            cloneEffect = GetComponent<Stage1PlayerCloneEffect>();
        }

        return cloneEffect;
    }

    private void SyncCloneEffect()
    {
        Stage1PlayerCloneEffect currentCloneEffect = GetCloneEffect();
        if (currentCloneEffect != null)
        {
            currentCloneEffect.SetBaseProjectileCount(baseProjectileCount);
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(fireKey))
        {
            TryFireProjectile();
        }

        if (autoFireEnabled)
        {
            TryFireProjectile();
        }
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Rect fireButtonRect = new Rect(
            Screen.width - fireButtonSize.x - fireButtonMargin.x,
            Screen.height - fireButtonSize.y - fireButtonMargin.y,
            fireButtonSize.x,
            fireButtonSize.y);

        string buttonLabel = autoFireEnabled ? fireButtonActiveLabel : fireButtonLabel;
        if (GUI.Button(fireButtonRect, buttonLabel))
        {
            autoFireEnabled = !autoFireEnabled;
            if (autoFireEnabled)
            {
                TryFireProjectile();
            }
        }
    }

    private void TryFireProjectile()
    {
        if (Time.time < nextFireTime)
        {
            return;
        }

        if (baseProjectileCount <= 0)
        {
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        FireProjectile();
    }

    private void FireProjectile()
    {
        Vector3 fireDirection = transform.forward;
        Vector3 worldPosition = transform.TransformPoint(fireOffset);
        Vector3 forwardOffset = fireDirection.normalized * projectileForwardSpacing;
        int projectileCount = Mathf.Clamp(baseProjectileCount, 1, maxProjectilesPerShot);

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 spawnOffset = forwardOffset * i;
            SpawnProjectile(worldPosition + spawnOffset, fireDirection);
        }
    }

    private void SpawnProjectile(Vector3 worldPosition, Vector3 fireDirection)
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = "Stage1Projectile";
        projectile.transform.position = worldPosition;
        projectile.transform.localScale = Vector3.one * 0.22f;
        projectile.transform.rotation = Quaternion.LookRotation(fireDirection, Vector3.up);

        Renderer renderer = projectile.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = projectileColor;
        }

        TrailRenderer trail = projectile.AddComponent<TrailRenderer>();
        trail.time = 0.18f;
        trail.startWidth = 0.12f;
        trail.endWidth = 0.0f;
        trail.startColor = projectileColor;
        trail.endColor = new Color(projectileColor.r, projectileColor.g, projectileColor.b, 0f);
        Shader trailShader = Shader.Find("Sprites/Default");
        if (trailShader != null)
        {
            trail.material = new Material(trailShader);
        }

        SphereCollider projectileCollider = projectile.GetComponent<SphereCollider>();
        if (projectileCollider != null)
        {
            projectileCollider.isTrigger = true;
        }

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();
        if (projectileBody == null)
        {
            projectileBody = projectile.AddComponent<Rigidbody>();
        }

        projectileBody.useGravity = false;
        projectileBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        projectileBody.linearVelocity = fireDirection.normalized * projectileSpeed;

        Stage1Projectile projectileBehavior = projectile.AddComponent<Stage1Projectile>();
        projectileBehavior.SetSpeed(projectileSpeed);
        projectileBehavior.SetLifetime(projectileLifetime);
        projectileBehavior.SetDamage(projectileDamage);
        projectileBehavior.SetOwnerShooter(this);
    }
}
