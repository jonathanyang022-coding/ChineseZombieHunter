using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Stage1HandwritingRuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateHandwritingArea()
    {
        if (Object.FindAnyObjectByType<Stage1HandwritingRuntime>() != null)
        {
            return;
        }

        GameObject root = new GameObject("Stage1HandwritingRuntime");
        Object.DontDestroyOnLoad(root);
        root.AddComponent<Stage1HandwritingRuntime>();
    }
}

public class Stage1HandwritingRuntime : MonoBehaviour
{
    [SerializeField] private float handwritingViewportHeight = 0.2f;
    [SerializeField] private int textureWidth = 2048;
    [SerializeField] private int textureHeight = 384;
    [SerializeField] private int brushRadius = 8;
    [SerializeField] private Color brushColor = Color.black;
    [SerializeField] private Color padColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color frameColor = new Color(0.94f, 0.94f, 0.94f, 1f);
    [SerializeField] private Color borderColor = new Color(0.72f, 0.72f, 0.72f, 1f);
    [SerializeField] private Color toggleButtonColor = new Color(0.15f, 0.15f, 0.18f, 0.92f);
    [SerializeField] private Color toggleButtonTextColor = Color.white;

    private Camera mainCamera;
    private Canvas canvas;
    private RectTransform handwritingFrameRect;
    private RectTransform padRectTransform;
    private Texture2D padTexture;
    private GameObject handwritingFrameObject;
    private GameObject toggleButtonObject;
    private Button toggleButton;
    private Text toggleButtonText;
    private bool isDrawing;
    private bool isCameraConfigured;
    private bool handwritingVisible = true;
    private Vector2Int? lastStrokePoint;

    private void Start()
    {
        BuildUi();
        ClearPad();
        UpdateHandwritingVisibility(true);
        TryConfigureCamera();
    }

    private void Update()
    {
        if (!isCameraConfigured)
        {
            TryConfigureCamera();
        }

        HandlePointerInput();
    }

    private void BuildUi()
    {
        GameObject canvasObject = new GameObject("Stage1HandwritingCanvas");
        canvasObject.transform.SetParent(transform, false);

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        if (EventSystem.current == null)
        {
            GameObject eventSystemObject = new GameObject("Stage1HandwritingEventSystem");
            eventSystemObject.transform.SetParent(transform, false);
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        handwritingFrameObject = new GameObject("Stage1HandwritingFrame");
        handwritingFrameObject.transform.SetParent(canvasObject.transform, false);

        Image frameImage = handwritingFrameObject.AddComponent<Image>();
        frameImage.color = frameColor;

        Outline frameOutline = handwritingFrameObject.AddComponent<Outline>();
        frameOutline.effectColor = borderColor;
        frameOutline.effectDistance = new Vector2(2f, -2f);

        handwritingFrameRect = handwritingFrameObject.GetComponent<RectTransform>();
        handwritingFrameRect.anchorMin = new Vector2(0f, 0f);
        handwritingFrameRect.anchorMax = new Vector2(1f, handwritingViewportHeight);
        handwritingFrameRect.offsetMin = new Vector2(12f, 12f);
        handwritingFrameRect.offsetMax = new Vector2(-12f, -12f);

        GameObject padObject = new GameObject("Stage1HandwritingPad");
        padObject.transform.SetParent(handwritingFrameObject.transform, false);

        RawImage padImage = padObject.AddComponent<RawImage>();
        padImage.color = padColor;

        padRectTransform = padObject.GetComponent<RectTransform>();
        padRectTransform.anchorMin = new Vector2(0f, 0f);
        padRectTransform.anchorMax = new Vector2(1f, 1f);
        padRectTransform.offsetMin = new Vector2(8f, 8f);
        padRectTransform.offsetMax = new Vector2(-8f, -8f);

        padTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        padImage.texture = padTexture;

        toggleButtonObject = CreateToggleButton(canvasObject.transform);
        toggleButton = toggleButtonObject.GetComponent<Button>();
        toggleButton.onClick.AddListener(ToggleHandwritingVisibility);
    }

    private void TryConfigureCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        Rect rect = mainCamera.rect;
        rect.x = 0f;
        rect.y = handwritingViewportHeight;
        rect.width = 1f;
        rect.height = Mathf.Clamp01(1f - handwritingViewportHeight);
        mainCamera.rect = rect;
        isCameraConfigured = true;
    }

    private GameObject CreateToggleButton(Transform parent)
    {
        GameObject buttonObject = new GameObject("Stage1HandwritingToggle");
        buttonObject.transform.SetParent(parent, false);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = toggleButtonColor;

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = toggleButtonColor;
        colors.highlightedColor = new Color(0.22f, 0.22f, 0.26f, 0.95f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.12f, 0.95f);
        colors.selectedColor = toggleButtonColor;
        colors.disabledColor = new Color(0.12f, 0.12f, 0.14f, 0.6f);
        button.colors = colors;

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1f, 0f);
        buttonRect.anchorMax = new Vector2(1f, 0f);
        buttonRect.pivot = new Vector2(1f, 0f);
        buttonRect.anchoredPosition = new Vector2(-12f, 12f);
        buttonRect.sizeDelta = new Vector2(44f, 44f);

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);

        Text label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 24;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = toggleButtonTextColor;
        label.raycastTarget = false;
        label.text = "✎";

        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        toggleButtonText = label;
        return buttonObject;
    }

    private void ToggleHandwritingVisibility()
    {
        UpdateHandwritingVisibility(!handwritingVisible);
    }

    private void UpdateHandwritingVisibility(bool visible)
    {
        handwritingVisible = visible;
        isDrawing = false;
        lastStrokePoint = null;

        if (handwritingFrameObject != null)
        {
            handwritingFrameObject.SetActive(visible);
        }

        if (mainCamera != null)
        {
            Rect rect = mainCamera.rect;
            rect.x = 0f;
            rect.y = visible ? handwritingViewportHeight : 0f;
            rect.width = 1f;
            rect.height = visible ? Mathf.Clamp01(1f - handwritingViewportHeight) : 1f;
            mainCamera.rect = rect;
        }

        if (toggleButtonText != null)
        {
            toggleButtonText.text = visible ? "✎" : "▣";
        }
    }

    private void HandlePointerInput()
    {
        if (!handwritingVisible || padRectTransform == null)
        {
            return;
        }

        bool pressed = false;
        bool released = false;
        Vector2 pointerPosition = default;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            pointerPosition = touch.position;
            pressed = touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
            released = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
        }
        else
        {
            pointerPosition = Input.mousePosition;
            pressed = Input.GetMouseButton(0);
            released = Input.GetMouseButtonUp(0);
        }

        if (released)
        {
            isDrawing = false;
            lastStrokePoint = null;
        }

        if (!pressed)
        {
            return;
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(padRectTransform, pointerPosition, null))
        {
            if (released)
            {
                isDrawing = false;
                lastStrokePoint = null;
            }

            return;
        }

        if (!ScreenPointToTexturePoint(pointerPosition, out Vector2Int currentPoint))
        {
            return;
        }

        if (!isDrawing)
        {
            isDrawing = true;
            lastStrokePoint = currentPoint;
            DrawBrush(currentPoint.x, currentPoint.y);
            padTexture.Apply(false);
            return;
        }

        if (lastStrokePoint.HasValue)
        {
            DrawLine(lastStrokePoint.Value, currentPoint);
            padTexture.Apply(false);
        }

        lastStrokePoint = currentPoint;
    }

    private bool ScreenPointToTexturePoint(Vector2 screenPoint, out Vector2Int texturePoint)
    {
        texturePoint = default;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(padRectTransform, screenPoint, null, out Vector2 localPoint))
        {
            return false;
        }

        Rect rect = padRectTransform.rect;
        float normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        int x = Mathf.Clamp(Mathf.RoundToInt(normalizedX * (textureWidth - 1)), 0, textureWidth - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(normalizedY * (textureHeight - 1)), 0, textureHeight - 1);

        texturePoint = new Vector2Int(x, y);
        return true;
    }

    private void DrawLine(Vector2Int start, Vector2Int end)
    {
        int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(start, end)));

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(start.x, end.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(start.y, end.y, t));
            DrawBrush(x, y);
        }
    }

    private void DrawBrush(int centerX, int centerY)
    {
        int radiusSquared = brushRadius * brushRadius;
        int minX = Mathf.Max(0, centerX - brushRadius);
        int maxX = Mathf.Min(textureWidth - 1, centerX + brushRadius);
        int minY = Mathf.Max(0, centerY - brushRadius);
        int maxY = Mathf.Min(textureHeight - 1, centerY + brushRadius);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                if (dx * dx + dy * dy > radiusSquared)
                {
                    continue;
                }

                padTexture.SetPixel(x, y, brushColor);
            }
        }
    }

    private void ClearPad()
    {
        Color32[] pixels = new Color32[textureWidth * textureHeight];
        Color32 fillColor = padColor;

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = fillColor;
        }

        padTexture.SetPixels32(pixels);
        padTexture.Apply(false);
    }
}
