using UnityEngine;

public class Stage1WritingArea : MonoBehaviour
{
    private void Awake()
    {
        EnsureHandwritingRuntimeExists();
    }

    private void OnEnable()
    {
        EnsureHandwritingRuntimeExists();
    }

    private static void EnsureHandwritingRuntimeExists()
    {
        if (Object.FindAnyObjectByType<Stage1HandwritingRuntime>() != null)
        {
            return;
        }

        GameObject root = new GameObject("Stage1HandwritingRuntime");
        root.AddComponent<Stage1HandwritingRuntime>();
    }
}
