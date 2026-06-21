using UnityEngine;
using UnityEngine.UI;

public class Stage1GameManager : MonoBehaviour
{
    private const string LoseMessage = "you lose";
    private const string WinMessage = "stage clear";

    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.6f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private float stageDuration = 30f;

    private Canvas overlayCanvas;
    private Text overlayText;
    private bool gameOver;
    private bool stageCompleted;
    private float stageStartTime = -1f;

    private void Awake()
    {
        EnsureOverlay();
    }

    private void Update()
    {
        if (!Application.isPlaying || gameOver)
        {
            return;
        }

        if (stageStartTime < 0f)
        {
            stageStartTime = Time.time;
        }

        if (!stageCompleted && Time.time - stageStartTime >= stageDuration)
        {
            TriggerStageComplete();
        }
    }

    public bool IsGameOver => gameOver;

    public void TriggerGameOver()
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;
        EnsureOverlay();
        if (overlayCanvas != null)
        {
            overlayCanvas.gameObject.SetActive(true);
        }

        if (overlayText != null)
        {
            overlayText.text = LoseMessage;
        }

        if (Application.isPlaying)
        {
            Time.timeScale = 0f;
        }
    }

    public void TriggerStageComplete()
    {
        if (gameOver)
        {
            return;
        }

        stageCompleted = true;
        gameOver = true;
        EnsureOverlay();
        if (overlayCanvas != null)
        {
            overlayCanvas.gameObject.SetActive(true);
        }

        if (overlayText != null)
        {
            overlayText.text = WinMessage;
        }

        if (Application.isPlaying)
        {
            Time.timeScale = 0f;
        }
    }

    private void EnsureOverlay()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (overlayCanvas == null)
        {
            GameObject canvasObject = GameObject.Find("Stage1LoseCanvas");
            if (canvasObject == null)
            {
                canvasObject = new GameObject("Stage1LoseCanvas");
                canvasObject.transform.SetParent(transform, false);
            }

            overlayCanvas = canvasObject.GetComponent<Canvas>();
            if (overlayCanvas == null)
            {
                overlayCanvas = canvasObject.AddComponent<Canvas>();
            }

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvasObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);

            GraphicRaycaster raycaster = canvasObject.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            GameObject panelObject = GameObject.Find("Stage1LosePanel");
            if (panelObject == null)
            {
                panelObject = new GameObject("Stage1LosePanel");
                panelObject.transform.SetParent(canvasObject.transform, false);
            }

            Image panel = panelObject.GetComponent<Image>();
            if (panel == null)
            {
                panel = panelObject.AddComponent<Image>();
            }

            panel.color = overlayColor;

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            GameObject textObject = GameObject.Find("Stage1LoseText");
            if (textObject == null)
            {
                textObject = new GameObject("Stage1LoseText");
                textObject.transform.SetParent(panelObject.transform, false);
            }

            overlayText = textObject.GetComponent<Text>();
            if (overlayText == null)
            {
                overlayText = textObject.AddComponent<Text>();
            }

            overlayText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            overlayText.text = LoseMessage;
            overlayText.color = textColor;
            overlayText.alignment = TextAnchor.MiddleCenter;
            overlayText.fontSize = 96;
            overlayText.raycastTarget = false;

            RectTransform textRect = overlayText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(1000f, 300f);

            overlayCanvas.gameObject.SetActive(false);
            return;
        }

        if (overlayText == null)
        {
            overlayText = GameObject.Find("Stage1LoseText")?.GetComponent<Text>();
        }
    }
}
