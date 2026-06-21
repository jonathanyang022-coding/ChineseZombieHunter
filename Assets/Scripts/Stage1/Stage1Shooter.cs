using UnityEngine;

public class Stage1Shooter : MonoBehaviour
{
    [SerializeField] private KeyCode fireKey = KeyCode.Space;
    [SerializeField] private float projectileSpeed = 16f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float fireCooldown = 0.2f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private Vector3 fireOffset = new Vector3(0f, 0.55f, 1.0f);
    [SerializeField] private Color projectileColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private string fireButtonLabel = "AUTO FIRE";
    [SerializeField] private string fireButtonActiveLabel = "AUTO FIRE ON";
    [SerializeField] private Vector2 fireButtonSize = new Vector2(140f, 72f);
    [SerializeField] private Vector2 fireButtonMargin = new Vector2(24f, 24f);

    private float nextFireTime;
    private bool autoFireEnabled;

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

        nextFireTime = Time.time + fireCooldown;
        FireProjectile();
    }

    private void FireProjectile()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = "Stage1Projectile";
        projectile.transform.position = transform.TransformPoint(fireOffset);
        projectile.transform.localScale = Vector3.one * 0.25f;

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
            Destroy(projectileCollider);
        }

        Stage1Projectile projectileBehavior = projectile.AddComponent<Stage1Projectile>();
        projectileBehavior.SetSpeed(projectileSpeed);
        projectileBehavior.SetLifetime(projectileLifetime);
        projectileBehavior.SetDamage(projectileDamage);
    }
}
