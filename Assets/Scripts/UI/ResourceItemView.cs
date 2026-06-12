using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class ResourceItemView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button actionButton;
        [SerializeField] private Text actionButtonText;

        private string url;

        public void Bind(ResourceEntry entry, System.Action<string> onAction)
        {
            if (entry == null)
            {
                return;
            }

            url = entry.DownloadUrl;

            if (iconImage != null)
            {
                iconImage.sprite = entry.Icon;
                iconImage.enabled = entry.Icon != null;
            }

            if (titleText != null)
            {
                titleText.text = entry.Title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = entry.Description;
            }

            if (actionButtonText != null)
            {
                actionButtonText.text = string.IsNullOrWhiteSpace(entry.ButtonLabel) ? "Download" : entry.ButtonLabel;
            }

            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(() => onAction?.Invoke(url));
            }
        }
    }
}
