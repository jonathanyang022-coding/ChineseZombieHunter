using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class JjGamesLauncher : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private CanvasGroup resourcesPanel;
        [SerializeField] private CanvasGroup stage1Panel;

        [Header("Navigation")]
        [SerializeField] private Button stage1Button;
        [SerializeField] private Button backToResourcesButton;
        [SerializeField] private Stage1Manager stage1Manager;

        private void Awake()
        {
            if (stage1Button != null)
            {
                stage1Button.onClick.AddListener(ShowStage1);
            }

            if (backToResourcesButton != null)
            {
                backToResourcesButton.onClick.AddListener(ShowResources);
            }
        }

        private void OnEnable()
        {
            ShowStage1(true);
        }

        private void OnDisable()
        {
            if (stage1Button != null)
            {
                stage1Button.onClick.RemoveListener(ShowStage1);
            }

            if (backToResourcesButton != null)
            {
                backToResourcesButton.onClick.RemoveListener(ShowResources);
            }
        }

        public void ShowResources()
        {
            SetPanelState(resourcesPanel, true);
            SetPanelState(stage1Panel, false);

            if (stage1Manager != null)
            {
                stage1Manager.ShowResourcesFromLauncher();
            }
        }

        public void ShowStage1()
        {
            ShowStage1(false);
        }

        public void ShowStage1(bool skipLessonFlow)
        {
            SetPanelState(resourcesPanel, false);
            SetPanelState(stage1Panel, true);

            if (stage1Manager != null)
            {
                stage1Manager.StartStage(skipLessonFlow);
            }
        }

        private void SetPanelState(CanvasGroup panel, bool visible)
        {
            if (panel == null)
            {
                return;
            }

            panel.gameObject.SetActive(visible);
            panel.alpha = visible ? 1f : 0f;
            panel.interactable = visible;
            panel.blocksRaycasts = visible;
        }
    }
}
