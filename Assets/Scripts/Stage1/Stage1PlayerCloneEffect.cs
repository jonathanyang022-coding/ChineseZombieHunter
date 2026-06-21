using UnityEngine;

public class Stage1PlayerCloneEffect : MonoBehaviour
{
    [SerializeField] private Color cloneColor = new Color(1f, 1f, 1f, 0.45f);
    [SerializeField] private float cloneOffsetX = 0.45f;
    [SerializeField] private float cloneOffsetZ = -0.4f;
    [SerializeField] private float cloneScaleMultiplier = 0.92f;

    private int activeCloneCount;

    public void SpawnClones(int cloneCount)
    {
        if (cloneCount <= 0)
        {
            return;
        }

        ClearClones();

        for (int i = 0; i < cloneCount; i++)
        {
            int side = (i % 2 == 0) ? -1 : 1;
            float offsetIndex = (i / 2) + 1;
            Vector3 offset = new Vector3(side * cloneOffsetX * offsetIndex, 0f, cloneOffsetZ * offsetIndex);
            CreateClone(offset);
        }

        activeCloneCount = cloneCount;
    }

    public void ClearClones()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Stage1PlayerClone"))
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        activeCloneCount = 0;
    }

    private void CreateClone(Vector3 localOffset)
    {
        GameObject cloneObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        activeCloneCount++;
        cloneObject.name = string.Format("Stage1PlayerClone_{0}", activeCloneCount);
        cloneObject.transform.SetParent(transform, false);
        cloneObject.transform.localPosition = localOffset;
        cloneObject.transform.localRotation = Quaternion.identity;
        cloneObject.transform.localScale = Vector3.one * cloneScaleMultiplier;

        Renderer renderer = cloneObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = cloneColor;
        }

        Collider collider = cloneObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}
