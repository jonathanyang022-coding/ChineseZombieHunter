using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class LifeDisplay : MonoBehaviour
    {
        [SerializeField] private Image[] lifeIcons = new Image[3];
        [SerializeField] private int maxLives = 3;

        public void SetLives(int lives)
        {
            if (lifeIcons == null)
            {
                return;
            }

            int visibleLives = Mathf.Clamp(lives, 0, maxLives);
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (lifeIcons[i] != null)
                {
                    lifeIcons[i].enabled = i < visibleLives;
                }
            }
        }
    }
}
