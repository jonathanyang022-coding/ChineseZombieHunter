using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class JjGamesLauncher : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private CanvasGroup splashPanel;
        [SerializeField] private CanvasGroup resourcesPanel;
        [SerializeField] private CanvasGroup stage1Panel;

        [Header("Splash")]
        [SerializeField] private RectTransform logoRoot;
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Button continueButton;
        [SerializeField] private float splashDuration = 2.5f;

        [Header("Navigation")]
        [SerializeField] private Button stage1Button;
        [SerializeField] private Button backToResourcesButton;
        [SerializeField] private Stage1Manager stage1Manager;

        [Header("Animation")]
        [SerializeField] private float pulseSpeed = 2.2f;
        [SerializeField] private float pulseAmount = 0.06f;

        private Coroutine splashRoutine;
        private Vector3 logoBaseScale = Vector3.one;

        private void Awake()
        {
            if (titleText != null)
            {
                titleText.text = "J&J Games";
            }

            if (subtitleText != null)
            {
                subtitleText.text = "Play. Learn. Level up.";
            }

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(ShowResources);
            }

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
            if (logoRoot != null)
            {
                logoBaseScale = logoRoot.localScale;
            }

            ShowSplash();
        }

        private void OnDisable()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(ShowResources);
            }

            if (stage1Button != null)
            {
                stage1Button.onClick.RemoveListener(ShowStage1);
            }

            if (backToResourcesButton != null)
            {
                backToResourcesButton.onClick.RemoveListener(ShowResources);
            }
        }

        private void Update()
        {
            if (logoRoot == null || !IsSplashVisible())
            {
                return;
            }

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            logoRoot.localScale = logoBaseScale * pulse;
        }

        public void ShowSplash()
        {
            SetPanelState(splashPanel, true);
            SetPanelState(resourcesPanel, false);
            SetPanelState(stage1Panel, false);

            if (splashRoutine != null)
            {
                StopCoroutine(splashRoutine);
            }

            splashRoutine = StartCoroutine(AutoAdvance());
        }

        public void ShowResources()
        {
            if (splashRoutine != null)
            {
                StopCoroutine(splashRoutine);
                splashRoutine = null;
            }

            SetPanelState(splashPanel, false);
            SetPanelState(resourcesPanel, true);
            SetPanelState(stage1Panel, false);

            if (stage1Manager != null)
            {
                stage1Manager.ShowResourcesFromLauncher();
            }
        }

        public void ShowStage1()
        {
            if (splashRoutine != null)
            {
                StopCoroutine(splashRoutine);
                splashRoutine = null;
            }

            SetPanelState(splashPanel, false);
            SetPanelState(resourcesPanel, false);
            SetPanelState(stage1Panel, true);

            if (stage1Manager != null)
            {
                stage1Manager.StartStage();
            }
        }

        private IEnumerator AutoAdvance()
        {
            yield return new WaitForSecondsRealtime(splashDuration);
            ShowResources();
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

        private bool IsSplashVisible()
        {
            return splashPanel != null && splashPanel.alpha > 0.5f;
        }
    }
}
