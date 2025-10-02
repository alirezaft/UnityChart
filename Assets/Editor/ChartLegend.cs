using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class ChartLegend
    {
        private float m_Width;
        private float m_Height;

        private List<DataProviderLegend> m_Legends;
        private ChartLayout m_ChartLayout;
        private ChartLegendLayout m_LegendLayout;
        private MeshGenerationContext m_Context;
        private Painter2D m_Painter;

        public ChartLegend(ChartLayout layout, VisualElement chart)
        {
            m_Legends = new List<DataProviderLegend>();
            m_ChartLayout = layout;
            m_LegendLayout = new ChartLegendLayout(3f, 15f, 4f, 10f, chart);
        }

        public void DrawLegends()
        {
            if (m_Painter is null || m_Context is null)
                throw new NullReferenceException("Provide mesh generation context and painter first.");

            var widthMiddle = m_ChartLayout.AllowedWidth / 2;
            var legendLength = EstimateLegendLength();

            var currPos = new Vector2(m_ChartLayout.XStart + widthMiddle - (legendLength / 2), m_ChartLayout.YEnd - (m_ChartLayout.LegendHeight * 0.5f));
            var textPositionVector =
                new Vector2(m_LegendLayout.ColorIndicatorRadius + m_LegendLayout.TextAndColorSpacing, -m_LegendLayout.Fontsize / 1.5f);

            for (int i = 0; i < m_Legends.Count; i++)
            {
                var length = m_LegendLayout.EstimateLegendWidth(m_Legends[i]);
                var painterMovementVector =
                    new Vector2(length - (m_LegendLayout.ColorIndicatorRadius / 2) + m_LegendLayout.LegendSpacing, 0);
                m_Painter.MoveTo(currPos);
                m_Painter.fillColor = m_Legends[i].Color;
                m_Painter.strokeColor = m_Legends[i].Color;
                m_Painter.BeginPath();
                m_Painter.Arc(currPos, m_LegendLayout.ColorIndicatorRadius, 0, 360);
                m_Painter.Fill();
                m_Painter.Stroke();
                m_Painter.ClosePath();

                m_Context.DrawText(m_Legends[i].Name, currPos + textPositionVector, (int)m_LegendLayout.Fontsize,
                    Color.white);
                m_Painter.MoveTo(currPos + painterMovementVector);
                currPos += painterMovementVector;
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