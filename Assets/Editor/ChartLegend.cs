using System;
using System.Collections.Generic;
using System.Linq;
using UnityChart.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
{
    public class ChartLegend
    {
        private float m_Width;
        private float m_Height;

        private List<DataProviderLegend> m_Legends;
        private ChartLayout m_ChartLayout;
        private VisualElement m_Chart;
        private ChartLegendLayout m_LegendLayout;
        private LegendOverflowBox m_OverflowBox;

        private MeshGenerationContext m_Context;
        private Painter2D m_Painter;

        private Rect? m_MoreTextArea;
        private Vector2? m_MousePosition;

        private int m_HiddenLegendsCount;

        public ChartLegend(ChartLayout layout, VisualElement chart)
        {
            m_Legends = new List<DataProviderLegend>();
            m_ChartLayout = layout;
            m_Chart = chart;
            m_LegendLayout = new ChartLegendLayout(3f, 15f, 4f, 10f, chart, 8f);
            m_MoreTextArea = null;
            m_OverflowBox = new LegendOverflowBox(m_LegendLayout);
        }

        public void DrawLegends(MeshGenerationContext ctx)
        {
            int hiddenLegendsCount;
            float finalLength;
            int visibleLegendsCount = ComputeVisibleLegendCount(out hiddenLegendsCount, out finalLength);

            var widthMiddle = m_ChartLayout.AllowedWidth / 2;

            var currPos = new Vector2(
                m_ChartLayout.XStart + widthMiddle - (finalLength / 2),
                m_ChartLayout.YEnd - (m_ChartLayout.LegendHeight * 0.5f)
            );

            var textPos = new Vector2(
                m_LegendLayout.ColorIndicatorRadius + m_LegendLayout.TextAndColorSpacing,
                -m_LegendLayout.Fontsize / 1.5f
            );

// Draw visible legends
            for (int i = 0; i < visibleLegendsCount; i++)
            {
                DrawLegendEntry(m_Legends[i], ref currPos, textPos);
            }

// Draw "+ N more"
            if (hiddenLegendsCount > 0)
            {
                m_HiddenLegendsCount = hiddenLegendsCount;
                string moreLabel = $"+ {hiddenLegendsCount} more";
                var fakeLegend = new DataProviderLegend();


                DrawTheMoreText(hiddenLegendsCount, ref currPos,
                    new Vector2(m_LegendLayout.MoreTextLeftMargin, textPos.y));
            }
            else
            {
                m_MoreTextArea = null;
            }

            if (m_MousePosition != null)
            {
                DrawOverflowBox(ctx, m_MousePosition.Value);
            }
        }

        private float EstimateLegendLength()
        {
            float ans = 0;

            for (var i = 0; i < m_Legends.Count; i++)
            {
                var l = m_Legends[i];
                ans += m_LegendLayout.EstimateLegendWidth(l);

                if (i != m_Legends.Count - 1)
                    ans += m_LegendLayout.LegendSpacing;
            }

            return ans;
        }

        private int ComputeVisibleLegendCount(out int hiddenCount, out float finalWidthNeeded)
        {
            float allowed = m_ChartLayout.AllowedWidth - m_LegendLayout.LegendMargin * 2;

            // Width of “+ N more”
            string moreText = $"+ {m_Legends.Count} more";
            float moreWidth = Utils.EstimateLabelDimensionInPixels(
                moreText,
                m_Chart,
                (int)m_LegendLayout.Fontsize
            ).x + m_LegendLayout.ColorIndicatorRadius * 2 + m_LegendLayout.TextAndColorSpacing;

            float used = 0f;
            int visibleCount = 0;

            for (int i = 0; i < m_Legends.Count; i++)
            {
                float w = m_LegendLayout.EstimateLegendWidth(m_Legends[i]);

                // Would adding this legend + “more” overflow?
                if (used + w + m_LegendLayout.LegendSpacing + moreWidth > allowed)
                    break;

                used += w + m_LegendLayout.LegendSpacing;
                visibleCount++;
            }

            hiddenCount = m_Legends.Count - visibleCount;
            finalWidthNeeded = used + (hiddenCount > 0 ? moreWidth : 0);

            return visibleCount;
        }

        private void DrawLegendEntry(DataProviderLegend legend, ref Vector2 pos, Vector2 textOffset)
        {
            float width = m_LegendLayout.EstimateLegendWidth(legend);

            // Draw indicator circle
            m_Painter.MoveTo(pos);
            m_Painter.fillColor = legend.Color;
            m_Painter.strokeColor = legend.Color;
            m_Painter.BeginPath();
            m_Painter.Arc(pos, m_LegendLayout.ColorIndicatorRadius, 0, 360);
            m_Painter.Fill();
            m_Painter.Stroke();
            m_Painter.ClosePath();

            // Draw text
            m_Context.DrawText(legend.Name, pos + textOffset, (int)m_LegendLayout.Fontsize, Color.white);

            // Move cursor forward
            pos += new Vector2(
                width + m_LegendLayout.LegendSpacing - m_LegendLayout.ColorIndicatorRadius / 2,
                0
            );
        }

        private void DrawTheMoreText(int n, ref Vector2 pos, Vector2 textOffset)
        {
            var moreText = $"+{n} more";

            SetMoreTextArea(moreText, pos + textOffset);

            m_Context.DrawText(moreText, pos + textOffset, (int)m_LegendLayout.Fontsize,
                new Color(0.8f, 0.8f, 0.8f, 1));
        }

        private void SetMoreTextArea(string text, Vector2 position)
        {
            var rectDimension = Utils.EstimateLabelDimensionInPixels(text, m_Chart, (int)m_LegendLayout.Fontsize);
            m_MoreTextArea = new Rect(position.x, position.y, rectDimension.x, rectDimension.y);
        }

        public void DrawOverflowBox(MeshGenerationContext ctx, Vector2 mousePos)
        {
            var hiddenLegends = m_Legends.Skip(m_Legends.Count - m_HiddenLegendsCount).ToArray();
            var boxPadding = m_OverflowBox.GetLayout().BoxPadding;

            var height = boxPadding * 2 +
                         m_LegendLayout.ColorIndicatorRadius * hiddenLegends.Length * 2 +
                         m_LegendLayout.LegendSpacing * (hiddenLegends.Length - 1);
            
            var width = m_LegendLayout.GetLongestLegendWidth(hiddenLegends) + boxPadding * 2 +
                        m_LegendLayout.ColorIndicatorRadius * 2;

            m_OverflowBox.SetBoxDimensions(width, height);
            var boxRect = m_OverflowBox.DrawOverflowBox(ctx.painter2D, mousePos);

            m_OverflowBox.DrawBoxContent(hiddenLegends, boxRect, ctx);
        }

        public void UpdateMousePosition(Vector2 mousePos)
        {
            if (m_MoreTextArea == null)
                return;

            if (m_MoreTextArea.Value.Contains(mousePos))
            {
                m_MousePosition = mousePos;
            }
            else
            {
                m_MousePosition = null;
            }
        }


        private bool LegendFitsInElement(float width)
        {
            return width - m_ChartLayout.AllowedWidth < m_LegendLayout.LegendMargin * 2;
        }

        public void SetPainter(Painter2D painter)
        {
            m_Painter = painter;
        }

        public void SetContext(MeshGenerationContext ctx)
        {
            m_Context = ctx;
        }

        public void AddLegend(DataProviderLegend legend)
        {
            m_Legends.Add(legend);
        }

        public void RemoveLegend(DataProviderLegend legend)
        {
            m_Legends.Remove(legend);
        }

        public void ClearLegends()
        {
            m_Legends.Clear();
        }

        public void SetDimension(float height, float width)
        {
            m_Height = height;
            m_Width = width;
        }
    }
}