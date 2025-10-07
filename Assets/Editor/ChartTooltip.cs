using System.Collections.Generic;
using System.Data;
using UnityChart.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
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
            m_TooltipLayout = new ChartTooltipLayout(4f, 2f, 6f, 3f, 10f, 12f, 2f);
        }

        public void DrawTooltip(Painter2D painter, MeshGenerationContext ctx, int dataIndex, float indicatorX)
        {
            if (m_MousePosition == null)
                return;

            var dataTexts = GetDataTexts(dataIndex);
            var titleText = $"#{dataIndex + 1}";

            var longestLength = FindLongestText(dataTexts, titleText);
            var isOnRight = IsToolTipOnRight(longestLength);
            var sign = isOnRight ? 1 : -1;

            var height = GetTooltipHeight(dataTexts, titleText);
            var width = GetTooltipWidth(longestLength);

            Vector2 boxStartPoint;

            var titleHeight =
                Utils.EstimateLabelDimensionInPixels(titleText, m_Chart, (int)m_TooltipLayout.TitleFontSize).y;


            if (isOnRight)
                boxStartPoint = new Vector2(indicatorX + (m_TooltipLayout.Margin * sign),
                    m_ChartLayout.YStart + m_TooltipLayout.Margin);
            else
                boxStartPoint = new Vector2(indicatorX + ((width + m_TooltipLayout.Margin) * sign),
                    m_ChartLayout.YStart + m_TooltipLayout.Margin);


            DrawTooltipBox(width, height, sign, indicatorX, painter);
            DrawTitle(titleText, ctx, boxStartPoint);

            var dataColors = new Color[m_DataProviders.Count];

            for (var i = 0; i < m_DataProviders.Count; i++)
            {
                dataColors[i] = m_DataProviders[i].Color;
            }

            DrawDataTexts(dataTexts, dataColors,
                boxStartPoint + new Vector2(0, titleHeight + m_TooltipLayout.LineSpacing), ctx, painter);
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
                if (index < m_DataProviders[i].Dataset.Count)
                    ans[i] = m_DataProviders[i].Dataset[index].ToString();
                else
                    ans[i] = "-";
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
                ans += Mathf.Max(m_TooltipLayout.ColorIndicatorRadius * 2,
                    Utils.EstimateLabelDimensionInPixels(dataTexts[i], m_Chart, (int)m_TooltipLayout.FontSize).y);

                if (i != dataTexts.Length - 1)
                    ans += m_TooltipLayout.LineSpacing;
            }

            ans += m_TooltipLayout.Padding * 2;

            return ans;
        }

        private float GetTooltipWidth(float longestTextLength)
        {
            var ans = longestTextLength;
            ans += m_TooltipLayout.ColorIndicatorRadius * 2;
            ans += m_TooltipLayout.Padding * 2;
            ans += m_TooltipLayout.ColorAndTextSapcing;

            return ans;
        }

        private void DrawTooltipBox(float width, float height, int isOnRight, float indicatorX, Painter2D painter)
        {
            var painterSnapshot = new PainterSnapshot
                { FillColor = painter.fillColor, StrokeColor = painter.strokeColor, Width = painter.lineWidth };

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

        private void DrawTitle(string title, MeshGenerationContext ctx, Vector2 boxBeginningPoint)
        {
            var position = boxBeginningPoint + new Vector2(m_TooltipLayout.Padding, m_TooltipLayout.Padding);
            ctx.DrawText(title, position, m_TooltipLayout.TitleFontSize, Color.white);
        }

        private void DrawDataTexts(string[] dataTexts, Color[] colors, Vector2 startingPoint, MeshGenerationContext ctx,
            Painter2D painter)
        {
            var painterSnapshot = new PainterSnapshot
                { Width = painter.lineWidth, FillColor = painter.fillColor, StrokeColor = painter.strokeColor };
            var currPos = startingPoint + new Vector2(m_TooltipLayout.Padding, m_TooltipLayout.LineSpacing);
            var textPositionVector =
                new Vector2(m_TooltipLayout.ColorIndicatorRadius + m_TooltipLayout.ColorAndTextSapcing,
                    -m_TooltipLayout.FontSize / 1.5f);

            for (int i = 0; i < dataTexts.Length; i++)
            {
                painter.BeginPath();
                painter.fillColor = colors[i];
                painter.strokeColor = colors[i];

                painter.MoveTo(currPos);
                painter.Arc(currPos + new Vector2(m_TooltipLayout.ColorIndicatorRadius, 0),
                    m_TooltipLayout.ColorIndicatorRadius, 0, 360);
                painter.Fill();
                painter.Stroke();
                painter.ClosePath();

                currPos += new Vector2(2 * m_TooltipLayout.ColorIndicatorRadius + m_TooltipLayout.ColorAndTextSapcing,
                    0);
                ctx.DrawText(dataTexts[i], currPos + textPositionVector, m_TooltipLayout.FontSize, Color.white);

                if (i != dataTexts.Length - 1)
                    currPos = new Vector2(startingPoint.x + m_TooltipLayout.Padding,
                        currPos.y + (m_TooltipLayout.LineSpacing + 2 * m_TooltipLayout.ColorIndicatorRadius));
            }

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