using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    [RequireComponent(typeof(RawImage))]
    public class HandwritingPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RawImage padImage;
        [SerializeField] private Color paperColor = Color.white;
        [SerializeField] private Color strokeColor = new Color(0.12f, 0.12f, 0.12f, 1f);
        [SerializeField] private int textureSize = 1024;
        [SerializeField] private int brushRadius = 14;
        [SerializeField] private bool clearOnEnable = true;

        private Texture2D texture;
        private RectTransform padRect;
        private Vector2? lastPoint;

        private void Awake()
        {
            if (padImage == null)
            {
                padImage = GetComponent<RawImage>();
            }

            padRect = GetComponent<RectTransform>();
            CreateTexture();
        }

        private void OnEnable()
        {
            if (clearOnEnable)
            {
                ClearCanvas();
            }
        }

        private void OnDestroy()
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }

        public void ClearCanvas()
        {
            if (texture == null)
            {
                CreateTexture();
            }

            Color[] pixels = new Color[texture.width * texture.height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = paperColor;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            if (padImage != null)
            {
                padImage.texture = texture;
            }

            lastPoint = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            lastPoint = ConvertToTexturePoint(eventData.position, eventData.pressEventCamera);
            if (lastPoint.HasValue)
            {
                DrawDot(lastPoint.Value);
                texture.Apply();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2? currentPoint = ConvertToTexturePoint(eventData.position, eventData.pressEventCamera);
            if (!currentPoint.HasValue)
            {
                return;
            }

            if (lastPoint.HasValue)
            {
                DrawLine(lastPoint.Value, currentPoint.Value);
            }
            else
            {
                DrawDot(currentPoint.Value);
                texture.Apply();
            }

            lastPoint = currentPoint;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            lastPoint = null;
        }

        private void CreateTexture()
        {
            if (texture != null)
            {
                Destroy(texture);
            }

            texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            if (padImage != null)
            {
                padImage.texture = texture;
            }

            ClearCanvas();
        }

        private Vector2? ConvertToTexturePoint(Vector2 screenPoint, Camera eventCamera)
        {
            if (padRect == null)
            {
                return null;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(padRect, screenPoint, eventCamera, out Vector2 localPoint))
            {
                return null;
            }

            Rect rect = padRect.rect;
            float normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
            float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

            if (normalizedX < 0f || normalizedX > 1f || normalizedY < 0f || normalizedY > 1f)
            {
                return null;
            }

            return new Vector2(
                normalizedX * (texture.width - 1),
                normalizedY * (texture.height - 1));
        }

        private void DrawLine(Vector2 start, Vector2 end)
        {
            int steps = Mathf.CeilToInt(Vector2.Distance(start, end));
            steps = Mathf.Max(1, steps);

            for (int i = 0; i <= steps; i++)
            {
                Vector2 point = Vector2.Lerp(start, end, (float)i / steps);
                DrawDot(point);
            }

            texture.Apply();
        }

        private void DrawDot(Vector2 point)
        {
            int centerX = Mathf.RoundToInt(point.x);
            int centerY = Mathf.RoundToInt(point.y);
            int radiusSquared = brushRadius * brushRadius;

            for (int y = -brushRadius; y <= brushRadius; y++)
            {
                for (int x = -brushRadius; x <= brushRadius; x++)
                {
                    if (x * x + y * y > radiusSquared)
                    {
                        continue;
                    }

                    int px = centerX + x;
                    int py = centerY + y;
                    if (px < 0 || py < 0 || px >= texture.width || py >= texture.height)
                    {
                        continue;
                    }

                    texture.SetPixel(px, py, strokeColor);
                }
            }
        }
    }
}
