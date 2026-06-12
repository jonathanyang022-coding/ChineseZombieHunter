using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChineseZombieHunter
{
    public class MatchBootstrapper : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private HudController hudController;

        private void Awake()
        {
            if (playerHealth == null)
            {
                playerHealth = FindObjectOfType<PlayerHealth>();
            }

            if (hudController == null)
            {
                hudController = FindObjectOfType<HudController>();
            }
        }

        private void Start()
        {
            if (playerHealth != null && hudController != null)
            {
                hudController.Bind(playerHealth);
            }

            if (playerHealth != null)
            {
                playerHealth.Died += HandlePlayerDied;
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= HandlePlayerDied;
            }
        }

        private void HandlePlayerDied()
        {
            Time.timeScale = 0f;
            if (hudController != null)
            {
                hudController.ShowGameOver();
            }
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

