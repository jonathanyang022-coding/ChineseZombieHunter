using UnityEngine;

public class Stage1WritingArea : MonoBehaviour
{
    [SerializeField] private Vector2 canvasSize = new Vector2(360f, 120f);
    [SerializeField] private Vector2 canvasMargin = new Vector2(16f, 12f);
    [SerializeField] private string promptText = "Draw the word";
    [SerializeField] private string submitLabel = "Submit";
    [SerializeField] private string clearLabel = "Clear";
    [SerializeField] private string collapseLabel = "Collapse";
    [SerializeField] private string expandLabel = "Expand";
    [SerializeField] private string specialAreaTitle = "Writing Area";
    [SerializeField] private int textureWidth = 512;
    [SerializeField] private int textureHeight = 256;
    [SerializeField] private Color canvasColor = Color.white;
    [SerializeField] private Color inkColor = Color.black;
    [SerializeField] private float brushSize = 8f;
    [SerializeField] private float expandedAreaHeight = 190f;
    [SerializeField] private float collapsedAreaHeight = 52f;
    [SerializeField] private float specialAreaPadding = 12f;
    [SerializeField] private float panelBottomOffset = 8f;
    [SerializeField] private float panelLiftPixelsPerPlayAreaUnit = 120f;

    private Texture2D canvasTexture;
    private bool isCollapsed;
    private bool isDrawing;
    private Vector2 lastCanvasPoint;
    private bool hasInk;
    private Stage1FirstLaneCharacter targetCharacter;

    private void Awake()
    {
        RefreshTargetCharacter();
        EnsureCanvas();
    }

    private void OnEnable()
    {
        RefreshTargetCharacter();
        EnsureCanvas();
    }

    private void OnDestroy()
    {
        if (canvasTexture != null)
        {
            Destroy(canvasTexture);
        }
    }

    private void RefreshTargetCharacter()
    {
        GameObject targetObject = GameObject.Find("Stage1FirstLaneCharacter");

        if (targetObject != null)
        {
            targetCharacter = targetObject.GetComponent<Stage1FirstLaneCharacter>();
        }
    }

    private void EnsureCanvas()
    {
        if (canvasTexture != null)
        {
            return;
        }

        canvasTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        canvasTexture.wrapMode = TextureWrapMode.Clamp;
        canvasTexture.filterMode = FilterMode.Bilinear;
        ClearCanvas();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (targetCharacter == null)
        {
            RefreshTargetCharacter();
        }

        EnsureCanvas();

        float areaWidth = Mathf.Min(Screen.width - canvasMargin.x * 2f, canvasSize.x + specialAreaPadding * 2f);
        float areaX = (Screen.width - areaWidth) * 0.5f;
        float areaHeight = isCollapsed ? collapsedAreaHeight : expandedAreaHeight;
        float areaY = Screen.height - areaHeight - canvasMargin.y - panelBottomOffset
            - (Stage1Bootstrap.CurrentPlayAreaVerticalOffset * panelLiftPixelsPerPlayAreaUnit);
        areaY = Mathf.Max(0f, areaY);

        Rect outerRect = new Rect(areaX, areaY, areaWidth, areaHeight);

        GUILayout.BeginArea(outerRect, GUI.skin.box);
        GUILayout.Space(6f);
        GUILayout.BeginHorizontal();
        GUILayout.Label(specialAreaTitle);
        GUILayout.FlexibleSpace();
        string toggleLabel = isCollapsed ? expandLabel : collapseLabel;
        if (GUILayout.Button(toggleLabel, GUILayout.Width(88f)))
        {
            isCollapsed = !isCollapsed;
            isDrawing = false;
        }
        GUILayout.EndHorizontal();

        if (isCollapsed)
        {
            GUILayout.EndArea();
            return;
        }

        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        string wordPrompt = promptText;
        if (targetCharacter != null)
        {
            wordPrompt = string.Format("{0}: {1}", promptText, targetCharacter.ChestWord);
        }

        GUILayout.Label(wordPrompt);

        Rect drawRect = GUILayoutUtility.GetRect(canvasSize.x, canvasSize.y);
        GUI.Box(drawRect, GUIContent.none);
        GUI.DrawTexture(drawRect, canvasTexture, ScaleMode.StretchToFill);

        HandleDrawing(drawRect);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(clearLabel, GUILayout.Height(30f)))
        {
            ClearCanvas();
        }

        if (GUILayout.Button(submitLabel, GUILayout.Height(30f)))
        {
            TrySubmit();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    private void HandleDrawing(Rect drawRect)
    {
        Event currentEvent = Event.current;
        if (currentEvent == null)
        {
            return;
        }

        Vector2 mousePosition = currentEvent.mousePosition;
        bool isInsideCanvas = drawRect.Contains(mousePosition);

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && isInsideCanvas)
        {
            isDrawing = true;
            lastCanvasPoint = ScreenToCanvas(drawRect, mousePosition);
            DrawBrush(lastCanvasPoint, lastCanvasPoint);
            currentEvent.Use();
        }
        else if (currentEvent.type == EventType.MouseDrag && isDrawing)
        {
            Vector2 canvasPoint = ScreenToCanvas(drawRect, mousePosition);
            DrawBrush(lastCanvasPoint, canvasPoint);
            lastCanvasPoint = canvasPoint;
            currentEvent.Use();
        }
        else if (currentEvent.type == EventType.MouseUp)
        {
            isDrawing = false;
        }
    }

    private Vector2 ScreenToCanvas(Rect drawRect, Vector2 guiPoint)
    {
        float x = Mathf.InverseLerp(drawRect.xMin, drawRect.xMax, guiPoint.x) * textureWidth;
        float y = (1f - Mathf.InverseLerp(drawRect.yMin, drawRect.yMax, guiPoint.y)) * textureHeight;
        return new Vector2(x, y);
    }

    private void DrawBrush(Vector2 from, Vector2 to)
    {
        int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(from, to)));
        for (int i = 0; i <= steps; i++)
        {
            Vector2 point = Vector2.Lerp(from, to, (float)i / steps);
            DrawCircle((int)point.x, (int)point.y, Mathf.RoundToInt(brushSize));
        }

        canvasTexture.Apply();
        hasInk = true;
    }

    private void DrawCircle(int centerX, int centerY, int radius)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > radius * radius)
                {
                    continue;
                }

                int pixelX = centerX + x;
                int pixelY = centerY + y;
                if (pixelX >= 0 && pixelX < textureWidth && pixelY >= 0 && pixelY < textureHeight)
                {
                    canvasTexture.SetPixel(pixelX, pixelY, inkColor);
                }
            }
        }
    }

    private void ClearCanvas()
    {
        if (canvasTexture == null)
        {
            return;
        }

        Color[] colors = new Color[textureWidth * textureHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = canvasColor;
        }

        canvasTexture.SetPixels(colors);
        canvasTexture.Apply();
        hasInk = false;
    }

    private void TrySubmit()
    {
        if (targetCharacter == null)
        {
            return;
        }

        if (!hasInk)
        {
            return;
        }

        if (targetCharacter.Solve())
        {
            ClearCanvas();
        }
    }
}
