using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class ResourceHubController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private ResourceCatalog catalog;

        [Header("UI")]
        [SerializeField] private Transform contentRoot;
        [SerializeField] private ResourceItemView itemPrefab;
        [SerializeField] private Text emptyStateText;

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (contentRoot == null || itemPrefab == null)
            {
                return;
            }

            for (int i = contentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(contentRoot.GetChild(i).gameObject);
            }

            if (catalog == null || catalog.Resources.Count == 0)
            {
                if (emptyStateText != null)
                {
                    emptyStateText.text = "No goodies unlocked yet.";
                    emptyStateText.gameObject.SetActive(true);
                }

                return;
            }

            if (emptyStateText != null)
            {
                emptyStateText.gameObject.SetActive(false);
            }

            foreach (ResourceEntry entry in catalog.Resources)
            {
                ResourceItemView view = Instantiate(itemPrefab, contentRoot);
                view.Bind(entry, OpenResource);
            }
        }

        public void OpenResource(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            Application.OpenURL(url);
        }
    }
}
