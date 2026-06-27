using System.Collections;
using UnityEngine;

public class Stage1GreenTileTarget : MonoBehaviour
{
    [SerializeField] private int minSpawnHealth = 2;
    [SerializeField] private int maxSpawnHealth = 5;
    [SerializeField] private float destroyAnimationDuration = 0.16f;
    [SerializeField] private float respawnDelay = 0.03f;
    [SerializeField] private float respawnAnimationDuration = 0.05f;
    [SerializeField] private float popScaleMultiplier = 1.2f;
    [SerializeField] private Vector3 healthLabelLocalPosition = Vector3.zero;
    [SerializeField] private Color healthLabelColor = Color.white;
    [SerializeField] private Color healthLabelShadowColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private int healthLabelFontSize = 42;
    [SerializeField] private float healthLabelCharacterSize = 0.08f;

    private int currentHealth;
    private bool isDying;
    private Transform healthLabelRoot;
    private TextMesh healthLabel;
    private Renderer targetRenderer;
    private Material targetMaterial;
    private Collider targetCollider;
    private Vector3 baseScale;
    private Quaternion baseRotation;
    private Color baseColor;

    private void Awake()
    {
        CacheTileReferences();
        baseScale = transform.localScale;
        baseRotation = transform.localRotation;
        RollSpawnHealth();
        EnsureHealthLabel();
        RestoreTileVisuals();
        UpdateHealthLabel();
    }

    public void TakeDamage(int damage)
    {
        if (isDying || damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthLabel();

        if (currentHealth <= 0)
        {
            isDying = true;
            StartCoroutine(PlayDestroyAndRespawn());
        }
    }

    public void Hit()
    {
        TakeDamage(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        Stage1Projectile projectile = other.GetComponentInParent<Stage1Projectile>();
        if (projectile != null && projectile.TryConsumeHit())
        {
            TakeDamage(projectile.GetDamage());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Stage1Projectile projectile = collision.collider.GetComponentInParent<Stage1Projectile>();
        if (projectile != null && projectile.TryConsumeHit())
        {
            TakeDamage(projectile.GetDamage());
        }
    }

    private void CacheTileReferences()
    {
        targetCollider = GetComponent<Collider>();
        targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
            baseColor = targetMaterial.color;
        }
    }

    private void EnsureHealthLabel()
    {
        if (healthLabelRoot != null)
        {
            return;
        }

        GameObject rootObject = new GameObject("HealthLabel");
        rootObject.transform.SetParent(transform, false);
        rootObject.transform.localPosition = healthLabelLocalPosition;
        healthLabelRoot = rootObject.transform;

        GameObject shadowObject = new GameObject("HealthLabelShadow");
        shadowObject.transform.SetParent(healthLabelRoot, false);
        shadowObject.transform.localPosition = new Vector3(0.02f, -0.02f, 0f);

        TextMesh shadowText = shadowObject.AddComponent<TextMesh>();
        ConfigureLabelText(shadowText, healthLabelShadowColor);

        GameObject labelObject = new GameObject("HealthLabelText");
        labelObject.transform.SetParent(healthLabelRoot, false);
        labelObject.transform.localPosition = Vector3.zero;

        healthLabel = labelObject.AddComponent<TextMesh>();
        ConfigureLabelText(healthLabel, healthLabelColor);

        MeshRenderer labelRenderer = labelObject.GetComponent<MeshRenderer>();
        if (labelRenderer != null)
        {
            labelRenderer.sortingOrder = 20;
        }

        MeshRenderer shadowRenderer = shadowObject.GetComponent<MeshRenderer>();
        if (shadowRenderer != null)
        {
            shadowRenderer.sortingOrder = 19;
        }
    }

    private void ConfigureLabelText(TextMesh label, Color color)
    {
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.characterSize = healthLabelCharacterSize;
        label.fontSize = healthLabelFontSize;
        label.fontStyle = FontStyle.Bold;
        label.color = color;
        label.text = currentHealth.ToString();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private void UpdateHealthLabel()
    {
        if (healthLabel != null)
        {
            healthLabel.text = currentHealth.ToString();
        }
    }

    private void RestoreTileVisuals()
    {
        if (targetCollider != null)
        {
            targetCollider.enabled = true;
        }

        if (targetRenderer != null)
        {
            targetRenderer.enabled = true;
            if (targetMaterial != null)
            {
                targetMaterial.color = baseColor;
            }
        }

        transform.localScale = baseScale;
        transform.localRotation = baseRotation;

        if (healthLabelRoot != null)
        {
            healthLabelRoot.gameObject.SetActive(true);
        }
    }

    private IEnumerator PlayDestroyAndRespawn()
    {
        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < destroyAnimationDuration)
        {
            float t = elapsed / destroyAnimationDuration;
            float pulse = Mathf.Sin(t * Mathf.PI);
            transform.localScale = startScale * (1f + pulse * (popScaleMultiplier - 1f));
            transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, pulse * 12f);

            if (targetMaterial != null)
            {
                targetMaterial.color = Color.Lerp(baseColor, Color.white, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (targetRenderer != null)
        {
            targetRenderer.enabled = false;
        }

        if (healthLabelRoot != null)
        {
            healthLabelRoot.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(respawnDelay);

        Destroy(gameObject);
    }

    private void RollSpawnHealth()
    {
        int minHealth = Mathf.Min(minSpawnHealth, maxSpawnHealth);
        int maxHealth = Mathf.Max(minSpawnHealth, maxSpawnHealth);
        currentHealth = Random.Range(minHealth, maxHealth + 1);
    }
}
