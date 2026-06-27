using UnityEngine;

public class Stage1GoalGate : MonoBehaviour
{
    [SerializeField] private int multiplierFactor = 2;
    [SerializeField] private Vector3 labelLocalPosition = new Vector3(0f, 0.22f, 0.02f);
    [SerializeField] private Color labelColor = Color.black;
    [SerializeField] private Color gateColor = new Color(1f, 0.55f, 0.12f);
    [SerializeField] private Color gateAccentColor = Color.white;
    [SerializeField] private float pulseSpeed = 2.2f;

    private bool collected;
    private Renderer gateRenderer;
    private Collider gateCollider;
    private TextMesh labelText;
    private Color baseColor;
    private Vector3 baseScale;

    public int MultiplierFactor => Mathf.Max(2, multiplierFactor);

    private void Awake()
    {
        gateRenderer = GetComponent<Renderer>();
        gateCollider = GetComponent<Collider>();
        baseScale = transform.localScale;

        if (gateRenderer != null)
        {
            gateRenderer.material.color = gateColor;
            baseColor = gateRenderer.material.color;
        }

        EnsureLabel();
        UpdateLabel();
    }

    private void Update()
    {
        if (collected)
        {
            return;
        }

        TryApplyMultiplierFromOverlap();

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.05f;
        transform.localScale = new Vector3(baseScale.x, baseScale.y * pulse, baseScale.z);

        if (gateRenderer != null)
        {
            float tint = 0.5f + Mathf.Sin(Time.time * pulseSpeed * 1.2f) * 0.2f;
            gateRenderer.material.color = Color.Lerp(baseColor, gateAccentColor, tint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryApplyMultiplier(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryApplyMultiplier(collision.collider);
    }

    private void TryApplyMultiplierFromOverlap()
    {
        if (collected || gateCollider == null)
        {
            return;
        }
    }

    private void TryApplyMultiplier(Collider other)
    {
        // Projectile hits are handled by ApplyProjectileHit.
    }

    public void ApplyProjectileHit(Stage1Projectile projectile, Vector3 impactPoint, Vector3 travelDirection)
    {
        if (collected || projectile == null)
        {
            return;
        }

        Stage1Shooter shooter = projectile.GetOwnerShooter();
        if (shooter == null)
        {
            shooter = ResolveShooter(GetComponent<Collider>());
        }

        if (shooter != null)
        {
            shooter.MultiplyShooters(2);
        }

        projectile.TryConsumeHit();
        collected = true;
        if (projectile != null)
        {
            Destroy(projectile.gameObject);
        }

        Destroy(gameObject);
    }

    private static Stage1Shooter ResolveShooter(Collider other)
    {
        Stage1Shooter shooter = other.GetComponentInParent<Stage1Shooter>();
        if (shooter != null)
        {
            return shooter;
        }

        return Object.FindAnyObjectByType<Stage1Shooter>();
    }

    private void EnsureLabel()
    {
        if (labelText != null)
        {
            return;
        }

        GameObject labelObject = new GameObject("GoalGateLabel");
        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = labelLocalPosition;

        labelText = labelObject.AddComponent<TextMesh>();
        labelText.anchor = TextAnchor.MiddleCenter;
        labelText.alignment = TextAlignment.Center;
        labelText.characterSize = 0.08f;
        labelText.fontSize = 44;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = labelColor;
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        MeshRenderer meshRenderer = labelObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = 40;
        }
    }

    private void UpdateLabel()
    {
        if (labelText != null)
        {
            labelText.text = string.Format("x{0}", Mathf.Max(2, multiplierFactor));
        }
    }
}
