using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace LogMonitor
{
    public class LiveLog : MonoBehaviour
    {
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
        public int FontSize = 20;

        public float ExtraLineHeight = 0f;
        public bool WordWrap = true;
        public TextAnchor TextAnchor = TextAnchor.MiddleLeft;

        public Color ContainerColor;
        public Color KeyColor;
        public Color ValueColor;

        private Dictionary<string, object> DebugInfo;
        private Dictionary<string, double> DebugInfoLifetime;

        public LiveLog()
        {
            DebugInfo = new Dictionary<string, object>();
            DebugInfoLifetime = new Dictionary<string, double>();
        }

        public void LogMessage(string key, object value)
        {
            DebugInfo[key] = value;
        }

        public void LogMessage(string key, object value, double lifeTime)
        {
            LogMessage(key, value);
            DebugInfoLifetime[key] = Time.timeAsDouble + lifeTime;
        }

        private void OnGUI()
        {
            if (!DebugInfo.Any())
            {
                return;
            }

            GUI.backgroundColor = ContainerColor;
            GUI.contentColor = KeyColor;
            GUIStyle labelStyle = new GUIStyle()
            {
                fontSize = FontSize,
                richText = true,
                padding = new RectOffset(Padding.x, Padding.x, 0, 0),
                clipping = TextClipping.Clip,
                alignment = TextAnchor,
                wordWrap = WordWrap
            };
            labelStyle.normal.textColor = KeyColor;

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
                float calculatedHeight = DebugInfo.Count() * (labelRect.height + Spacing);
                boxRect.height = calculatedHeight;
            }
            boxRect.height += (Padding.y * 2);

            GUI.Box(boxRect, string.Empty);

            foreach (var debugLine in DebugInfo)
            {
                GUI.Label(labelRect, $"{debugLine.Key}: {debugLine.Value}", labelStyle);
                labelRect.position += Vector2.up * (labelRect.height + Spacing);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < DebugInfo.Count; i++)
            {
                var keyValuePair = DebugInfo.ElementAt(i);
                var key = keyValuePair.Key;

                if (DebugInfoLifetime.TryGetValue(key, out var expires) && expires < Time.timeAsDouble)
                {
                    DebugInfo.Remove(key);
                    DebugInfoLifetime.Remove(key);
                    i--;
                }
            }
        }
    }
}