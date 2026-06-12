using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class StageResultPanel : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text messageText;
        [SerializeField] private Button primaryButton;
        [SerializeField] private Text primaryButtonText;

        public void ShowFailure(Action onRetry)
        {
            if (titleText != null)
            {
                titleText.text = "Try Again";
            }

            if (messageText != null)
            {
                messageText.text = "You ran out of lives. Give it another shot.";
            }

            if (primaryButtonText != null)
            {
                primaryButtonText.text = "Retry";
            }

            ConfigurePrimaryButton(onRetry);
        }

        public void ShowSuccess(Action onReplay)
        {
            if (titleText != null)
            {
                titleText.text = "Stage Clear!";
            }

            if (messageText != null)
            {
                messageText.text = "Nice work. Want to play it again?";
            }

            if (primaryButtonText != null)
            {
                primaryButtonText.text = "Play Again";
            }

            ConfigurePrimaryButton(onReplay);
        }

        private void ConfigurePrimaryButton(Action onClick)
        {
            if (primaryButton == null)
            {
                return;
            }

            primaryButton.onClick.RemoveAllListeners();
            primaryButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
