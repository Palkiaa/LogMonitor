using UnityEngine;

namespace LogMonitor
{
    [ExecuteAlways]
    public class LogMonitorDisplay : MonoBehaviour, ILogMonitor
    {
        public LogMonitorChannel LiveLogChannel;
        public bool AlwaysShowInEditor;
        public bool AlwaysShowInGame;

        public Rect DisplayArea;
        public bool AbsolutePosition = true;
        public bool AbsoluteSize = true;

        [Space]
        public bool FlexibleHeight = true;

        //public bool FlexibleWidth;

        [Space]
        public Vector2Int Padding = Vector2Int.zero;

        public float Spacing = 5f;

        [Header("Text")]
        public string FormatString = "{0}: {1}";

        public TextAnchor TextAnchor = TextAnchor.MiddleLeft;
        public int FontSize = 20;
        public float ExtraLineHeight = 0f;
        public bool WordWrap = true;

        [Space]
        public Color ContainerColor = Color.gray;

        public Color DefaultColor = Color.white;
        public Color KeyColor = Color.white;
        public Color ValueColor = Color.white;

        private Color _oldContainerColor;
        private GUIStyle _currentStyle = null;

        public void Log(string key, object value, Component context = null)
        {
            if (LiveLogChannel == null)
                return;

            LiveLogChannel.Log(key, value, context);
        }

        public void Log(string key, object value, double lifeTime, Component context = null)
        {
            if (LiveLogChannel == null)
                return;

            LiveLogChannel.Log(key, value, lifeTime, context);
        }

        private void Start()
        {
            _currentStyle = null;
        }

        private void OnGUI()
        {
            if (LiveLogChannel == null)
                return;

            if (Application.isEditor)
            {
                if (Application.isPlaying)
                {
                    if (!AlwaysShowInGame && !LiveLogChannel.AnyMessages)
                        return;
                }
                else if (!AlwaysShowInEditor && !LiveLogChannel.AnyMessages)
                {
                    return;
                }
            }
            else if (!AlwaysShowInGame && !LiveLogChannel.AnyMessages)
                return;

            GUIStyle labelStyle = new()
            {
                fontSize = FontSize,
                richText = true,
                padding = new RectOffset(Padding.x, Padding.x, 0, 0),
                clipping = TextClipping.Clip,
                alignment = TextAnchor,
                wordWrap = WordWrap
            };

            Rect drawArea = DisplayArea;
            if (!AbsolutePosition)
            {
                drawArea.x *= Screen.width;
                drawArea.y *= Screen.height;
            }

            if (!AbsoluteSize)
            {
                drawArea.width *= Screen.width;
                drawArea.height *= Screen.height;
            }

            Rect labelRect = drawArea;
            labelRect.position += Vector2.up * Padding.y;
            labelRect.height = FontSize + ExtraLineHeight;
            labelRect.width = drawArea.width;

            Rect boxRect = drawArea;

            if (FlexibleHeight)
            {
                float calculatedHeight = LiveLogChannel.MessageCount * (labelRect.height + Spacing);
                boxRect.height = calculatedHeight;
            }
            boxRect.height += (Padding.y * 2);

            InitStyles(ContainerColor);
            GUI.Box(boxRect, string.Empty, _currentStyle);

            if (!LiveLogChannel.AnyMessages)
                return;

            var defaultColorHex = ColorUtility.ToHtmlStringRGBA(DefaultColor);
            var keyColorHex = ColorUtility.ToHtmlStringRGBA(KeyColor);
            var valueColorHex = ColorUtility.ToHtmlStringRGBA(ValueColor);

            var colorFormatString = FormatString
                                    .Replace("{0}", $"<color=#{keyColorHex}>{{0}}</color>")
                                    .Replace("{1}", $"<color=#{valueColorHex}>{{1}}</color>");

            foreach (var debugLine in LiveLogChannel.GetMessages())
            {
                var message = colorFormatString
                              .Replace("{0}", debugLine.Key)
                              .Replace("{1}", debugLine.Value?.ToString());
                message = $"<color=#{defaultColorHex}>{message}</color>";

                GUI.Label(labelRect, message, labelStyle);
                labelRect.position += Vector2.up * (labelRect.height + Spacing);
            }
        }

        private void LateUpdate()
        {
            if (LiveLogChannel == null)
                return;

            LiveLogChannel.Tick();
        }

        private void InitStyles(Color32 color)
        {
            if (_currentStyle != null && color == _oldContainerColor)
            {
                return;
            }

            _currentStyle = new GUIStyle(GUI.skin.box);
            _currentStyle.normal.background = MakeTex(1, 1, color);
            _oldContainerColor = color;
        }

        private Texture2D MakeTex(int width, int height, Color32 col)
        {
            var pix = new Color32[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels32(pix);
            result.Apply();
            return result;
        }

    }
}
