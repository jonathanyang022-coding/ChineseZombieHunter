using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private Text healthText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject gameOverPanel;

        private PlayerHealth boundPlayerHealth;

        public void Bind(PlayerHealth playerHealth)
        {
            if (boundPlayerHealth != null)
            {
                boundPlayerHealth.HealthChanged -= HandleHealthChanged;
                boundPlayerHealth.Died -= HandlePlayerDied;
            }

            boundPlayerHealth = playerHealth;

            if (boundPlayerHealth == null)
            {
                return;
            }

            boundPlayerHealth.HealthChanged += HandleHealthChanged;
            boundPlayerHealth.Died += HandlePlayerDied;
            HandleHealthChanged(boundPlayerHealth.CurrentHealth, boundPlayerHealth.MaxHealth);

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (boundPlayerHealth == null)
            {
                return;
            }

            boundPlayerHealth.HealthChanged -= HandleHealthChanged;
            boundPlayerHealth.Died -= HandlePlayerDied;
        }

        private void HandleHealthChanged(int currentHealth, int maxHealth)
        {
            if (healthText != null)
            {
                healthText.text = $"HP {currentHealth}/{maxHealth}";
            }

            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }

        private void HandlePlayerDied()
        {
            ShowGameOver();
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }
}

