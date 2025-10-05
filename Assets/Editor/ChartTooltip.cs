using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class ChartTooltip
    {
        private LineChart m_Chart;
        private ChartLayout m_ChartLayout;
        private ChartTooltipLayout m_TooltipLayout;
        private List<DataProvider> m_DataProviders;

        private Vector2? m_MousePosition;
        private float m_VerticalLineX;

        public ChartTooltip(ChartLayout layout, List<DataProvider> providers, LineChart chart)
        {
            m_ChartLayout = layout;
            m_DataProviders = providers;
            m_TooltipLayout = new ChartTooltipLayout(4f, 2f, 3f, 3f, 10f, 12f, 2f);
        }

        public void DrawTooltip(Painter2D painter, MeshGenerationContext ctx, int dataIndex, float indicatorX)
        {
            if (m_MousePosition == null)
                return;

            var dataTexts = GetDataTexts(dataIndex);
            var titleText = $"#{dataIndex}";

            var longestLength = FindLongestText(dataTexts, titleText);
            var isOnRight = IsToolTipOnRight(longestLength);
            var sign = isOnRight ? 1 : -1;

            var height = GetTooltipHeight(dataTexts, titleText);
            var width = GetTooltipWidth(longestLength);
            
            DrawTooltipBox(width, height, sign, indicatorX, painter);
        }

        private bool IsToolTipOnRight(float longestLabel)
        {
            var availableSpace = m_ChartLayout.XEnd - m_MousePosition.Value.x;

            return availableSpace > GetTooltipWidth(longestLabel);
        }

        private string[] GetDataTexts(int index)
        {
            var ans = new string[m_DataProviders.Count];

            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = m_DataProviders[i].Dataset[index].ToString();
            }

            return ans;
        }

        private float FindLongestText(string[] datTexts, string title)
        {
            var ans = Utils
                .EstimateLabelDimensionInPixels(title, m_Chart, (int)m_TooltipLayout.TitleFontSize, FontStyle.Bold).x;

            foreach (var text in datTexts)
            {
                var textSize = Utils.EstimateLabelDimensionInPixels(text, m_Chart, (int)m_TooltipLayout.FontSize).x;
                if (ans < textSize)
                    ans = textSize;
            }

            return ans;
        }

        private float GetTooltipHeight(string[] dataTexts, string title)
        {
            var ans = Utils
                .EstimateLabelDimensionInPixels(title, m_Chart, (int)m_TooltipLayout.TitleFontSize, FontStyle.Bold).y;

            for (int i = 0; i < dataTexts.Length; i++)
            {
                ans += Utils.EstimateLabelDimensionInPixels(dataTexts[i], m_Chart, (int)m_TooltipLayout.FontSize).y;

                if (i != dataTexts.Length - 1)
                    ans += m_TooltipLayout.LineSpacing;

                ans += m_TooltipLayout.ColorIndicatorRadius * 2;
            }

            ans += m_TooltipLayout.Padding * 2;

            return ans;
        }

        private float GetTooltipWidth(float longestTextLength)
        {
            var ans = longestTextLength;
            ans += m_TooltipLayout.ColorIndicatorRadius * 2;
            ans += m_TooltipLayout.Padding;
            ans += m_TooltipLayout.ColorAndTextSapcing;

            return ans;
        }

        private void DrawTooltipBox(float width, float height, int isOnRight, float indicatorX, Painter2D painter)
        {
            var painterSnapshot = new PainterSnapshot
                { FillColor = painter.fillColor, StrokeColor = painter.strokeColor, Width = painter.lineWidth };
            Debug.Log(indicatorX);    
            
            painter.BeginPath();
            painter.lineWidth = 1f;
            painter.strokeColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            painter.fillColor = new Color(0.22f, 0.22f, 0.22f, 1f);

            var currPos = new Vector2(indicatorX + (m_TooltipLayout.Margin * isOnRight),
                m_ChartLayout.YStart + m_TooltipLayout.Margin);

            painter.MoveTo(currPos);
            painter.LineTo(currPos + new Vector2(width * isOnRight, 0));
            currPos += new Vector2(width * isOnRight, 0);
            painter.LineTo(currPos + new Vector2(0, height));
            currPos += new Vector2(0, height);
            painter.LineTo(currPos + new Vector2(-width * isOnRight, 0));
            currPos += new Vector2(-width * isOnRight, 0);
            painter.LineTo(currPos + new Vector2(0, -height));
            
            painter.Stroke();
            painter.Fill();
            painter.ClosePath();
            
            painterSnapshot.RestorePainterData(painter);
        }

        public void UpdateMousePosition(Vector2 position)
        {
            var dataArea = m_ChartLayout.GetChartDataArea();

            m_MousePosition = dataArea.Contains(position) ? position : null;
        }

        public void Reset()
        {
            m_MousePosition = null;
        }
    }

    public class ChartTooltipLayout
    {
        private float m_TooltipMargin;
        private float m_TooltipPadding;
        private float m_LineSpacing;
        private float m_ColorIndicatorRadius;
        private float m_FontSize;
        private float m_TitleFontSize;
        private float m_ColorAndTextSpacing;

        public float Margin => m_TooltipMargin;
        public float FontSize => m_FontSize;
        public float TitleFontSize => m_TitleFontSize;
        public float Padding => m_TooltipPadding;
        public float ColorIndicatorRadius => m_ColorIndicatorRadius;
        public float LineSpacing => m_LineSpacing;
        public float ColorAndTextSapcing => m_ColorAndTextSpacing;

        public ChartTooltipLayout(float padding, float margin, float lineSpacing, float colorRadius, float fontSize,
            float titleFontSize, float colorTextSpacing)
        {
            m_TooltipMargin = margin;
            m_TooltipPadding = padding;
            m_LineSpacing = lineSpacing;
            m_ColorIndicatorRadius = colorRadius;
            m_FontSize = fontSize;
            m_TitleFontSize = titleFontSize;
            m_ColorAndTextSpacing = colorTextSpacing;
        }
    }
}