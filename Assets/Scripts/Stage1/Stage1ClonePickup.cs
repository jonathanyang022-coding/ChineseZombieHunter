using UnityEngine;

public class Stage1ClonePickup : MonoBehaviour
{
    [SerializeField] private int cloneBonus = 1;
    [SerializeField] private Vector3 labelLocalPosition = new Vector3(0f, 0.08f, 0.02f);
    [SerializeField] private Color labelColor = Color.black;
    [SerializeField] private Color pickupColor = new Color(0.35f, 0.65f, 1f);
    [SerializeField] private Color pickupAccentColor = Color.white;
    [SerializeField] private float pulseSpeed = 3.5f;

    private bool collected;
    private Renderer pickupRenderer;
    private TextMesh labelText;
    private Color baseColor;
    private Vector3 baseScale;

    public int CloneBonus => Mathf.Max(1, cloneBonus);

    private void Awake()
    {
        pickupRenderer = GetComponent<Renderer>();
        baseScale = transform.localScale;

        if (pickupRenderer != null)
        {
            pickupRenderer.material.color = pickupColor;
            baseColor = pickupRenderer.material.color;
        }
    }

    private void Start()
    {
        EnsureLabel();
        UpdateLabel();
    }

    private void Update()
    {
        if (collected)
        {
            return;
        }

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.08f;
        transform.localScale = baseScale * pulse;

        if (pickupRenderer != null)
        {
            float tint = 0.5f + Mathf.Sin(Time.time * pulseSpeed * 1.35f) * 0.25f;
            pickupRenderer.material.color = Color.Lerp(baseColor, pickupAccentColor, tint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryApplyBonus(other);
    }

    private void TryApplyBonus(Collider other)
    {
        if (collected || other == null)
        {
            return;
        }

        if (!IsPlayerCollider(other))
        {
            return;
        }

        Stage1Shooter shooter = ResolveShooter(other);
        if (shooter == null)
        {
            return;
        }

        shooter.IncreaseBaseProjectileCount(CloneBonus);
        collected = true;
        Destroy(gameObject);
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
            shooter.IncreaseBaseProjectileCount(CloneBonus);
        }

        projectile.TryConsumeHit();
        collected = true;
        if (projectile != null)
        {
            Destroy(projectile.gameObject);
        }

        Destroy(gameObject);
    }

    private static bool IsPlayerCollider(Collider other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.CompareTag("Player"))
        {
            return true;
        }

        return other.GetComponentInParent<LaneRunnerController>() != null;
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

        GameObject labelObject = new GameObject("ClonePickupLabel");
        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = labelLocalPosition;

        labelText = labelObject.AddComponent<TextMesh>();
        labelText.anchor = TextAnchor.MiddleCenter;
        labelText.alignment = TextAlignment.Center;
        labelText.characterSize = 0.08f;
        labelText.fontSize = 42;
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
            labelText.text = string.Format("+{0}", Mathf.Max(1, cloneBonus));
        }
    }
}
