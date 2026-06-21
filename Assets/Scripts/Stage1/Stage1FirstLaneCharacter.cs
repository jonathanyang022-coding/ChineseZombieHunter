using System.Collections;
using UnityEngine;

public class Stage1FirstLaneCharacter : MonoBehaviour
{
    [SerializeField] private string chestWord = "中";
    [SerializeField] private string requiredLetter = "Z";
    [SerializeField] private int playerCloneMultiplier = 3;
    [SerializeField] private Vector3 chestLabelLocalPosition = new Vector3(0f, 0.2f, 0.48f);
    [SerializeField] private Vector3 chestLabelShadowOffset = new Vector3(0.02f, -0.02f, 0f);
    [SerializeField] private Color chestTextColor = Color.white;
    [SerializeField] private Color chestShadowColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private int chestFontSize = 48;
    [SerializeField] private float chestCharacterSize = 0.06f;
    [SerializeField] private float vanishDuration = 0.14f;

    private bool isSolved;
    private Renderer bodyRenderer;
    private Material bodyMaterial;
    private Collider bodyCollider;
    private Vector3 baseScale;
    private Quaternion baseRotation;
    private Color baseColor;

    public string RequiredLetter => requiredLetter;
    public string ChestWord => chestWord;
    public bool IsSolved => isSolved;

    private void Awake()
    {
        bodyRenderer = GetComponent<Renderer>();
        bodyCollider = GetComponent<Collider>();
        baseScale = transform.localScale;
        baseRotation = transform.localRotation;

        if (bodyRenderer != null)
        {
            bodyMaterial = bodyRenderer.material;
            baseColor = bodyMaterial.color;
        }

        EnsureChestText();
        RefreshChestText();
    }

    public bool TrySolve(string input)
    {
        if (isSolved)
        {
            return false;
        }

        if (string.Equals(input?.Trim(), requiredLetter, System.StringComparison.OrdinalIgnoreCase))
        {
            StartCoroutine(VanishAndDisappear());
            return true;
        }

        return false;
    }

    public bool Solve()
    {
        if (isSolved)
        {
            return false;
        }

        StartCoroutine(VanishAndDisappear());
        return true;
    }

    private void EnsureChestText()
    {
        if (transform.Find("ChestTextRoot") != null)
        {
            return;
        }

        GameObject rootObject = new GameObject("ChestTextRoot");
        rootObject.transform.SetParent(transform, false);
        rootObject.transform.localPosition = chestLabelLocalPosition;

        GameObject shadowObject = new GameObject("ChestTextShadow");
        shadowObject.transform.SetParent(rootObject.transform, false);
        shadowObject.transform.localPosition = chestLabelShadowOffset;

        TextMesh shadowText = shadowObject.AddComponent<TextMesh>();
        ConfigureLabel(shadowText, chestShadowColor, chestWord);

        GameObject textObject = new GameObject("ChestText");
        textObject.transform.SetParent(rootObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh label = textObject.AddComponent<TextMesh>();
        ConfigureLabel(label, chestTextColor, chestWord);

        MeshRenderer textRenderer = textObject.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            textRenderer.sortingOrder = 30;
        }

        MeshRenderer shadowRenderer = shadowObject.GetComponent<MeshRenderer>();
        if (shadowRenderer != null)
        {
            shadowRenderer.sortingOrder = 29;
        }
    }

    private void ConfigureLabel(TextMesh label, Color color, string text)
    {
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.fontSize = chestFontSize;
        label.characterSize = chestCharacterSize;
        label.fontStyle = FontStyle.Bold;
        label.color = color;
        label.text = text;
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private void RefreshChestText()
    {
        TextMesh[] textMeshes = GetComponentsInChildren<TextMesh>(true);
        for (int i = 0; i < textMeshes.Length; i++)
        {
            if (textMeshes[i] != null)
            {
                textMeshes[i].text = chestWord;
            }
        }
    }

    private IEnumerator VanishAndDisappear()
    {
        isSolved = true;

        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }

        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < vanishDuration)
        {
            float t = elapsed / vanishDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            if (bodyMaterial != null)
            {
                bodyMaterial.color = Color.Lerp(baseColor, Color.white, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        SpawnPlayerClones();
        Destroy(gameObject);
    }

    public string GetRequiredLetter()
    {
        return requiredLetter;
    }

    private void SpawnPlayerClones()
    {
        GameObject playerObject = GameObject.Find("Stage1Player");
        if (playerObject == null)
        {
            return;
        }

        Stage1PlayerCloneEffect cloneEffect = playerObject.GetComponent<Stage1PlayerCloneEffect>();
        if (cloneEffect != null)
        {
            cloneEffect.SpawnClones(Mathf.Max(0, playerCloneMultiplier - 1));
        }
    }
}
