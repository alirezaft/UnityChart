using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class Axis
    {
        private float m_Height;
        private float m_Width;
        private float m_NiceMinY;
        private float m_NiceMaxY;
        private float m_WidthOffset;
        private float m_ZeroOnYAxisPosition;
        public float ZeroOnYAxis => m_ZeroOnYAxisPosition;


        private bool m_AreAllNegative;
        private bool m_AreAllPositive;

        private Painter2D m_Painter;
        private NiceScale m_Scale;
        private ChartLayout m_ChartLayout;

        public Axis(float chartHeight, float chartWidth, ChartLayout layout)
        {
            m_Height = chartHeight;
            m_Width = chartWidth;
            m_ChartLayout = layout;
        }

        public void DrawChartAxis()
        {
            m_Painter.BeginPath();
            m_Painter.lineWidth = 2f;

            DrawAxisLines();
            // DrawAxisArrows(painter);

            m_Painter.MoveTo(Vector2.zero);


            m_Painter.Stroke();
            m_Painter.ClosePath();
        }

        private void DrawAxisLines()
        {
            m_Painter.strokeColor = Color.white;
            // m_WidthOffset = m_ChartLayout.CaclulateWidthOffset()

            DrawVerticalAxisLine();
            DrawHorizontalAxisLine();

            m_Painter.Stroke();
            m_Painter.ClosePath();
        }

        private void DrawVerticalAxisLine()
        {
            var originalWidth = m_Painter.lineWidth;
            m_Painter.lineWidth = 1f;

            var offset = m_ChartLayout.WidthOffset + m_ChartLayout.XStart;

            m_Painter.MoveTo(new Vector2(offset,
                m_ChartLayout.YStart));
            m_Painter.LineTo(new Vector2(offset,
                m_ChartLayout.ChartHeight + m_ChartLayout.YStart));

            m_Painter.lineWidth = originalWidth;
        }

        private void DrawHorizontalAxisLine()
        {
            if (m_AreAllNegative)
            {
                m_ZeroOnYAxisPosition = m_ChartLayout.YStart;
            }
            else if (m_AreAllPositive)
            {
                m_ZeroOnYAxisPosition = m_ChartLayout.YEnd;
            }

            float originalWidth = m_Painter.lineWidth;
            m_Painter.lineWidth = 1f;
            
            float t = m_NiceMaxY / (Mathf.Abs(m_NiceMinY) + m_NiceMaxY);
            m_ZeroOnYAxisPosition = m_ChartLayout.YStart + t * m_ChartLayout.ChartHeight;
            
            Debug.Log($"zero pos: {m_ZeroOnYAxisPosition}");


            m_Painter.MoveTo(new Vector2(m_ChartLayout.WidthOffset + m_ChartLayout.XStart, m_ZeroOnYAxisPosition));
            m_Painter.LineTo(new Vector2(m_ChartLayout.XEnd, m_ZeroOnYAxisPosition));
            m_Painter.lineWidth = originalWidth;
        }

        public void SetPainter(Painter2D painter)
        {
            m_Painter = painter ?? throw new NullReferenceException("Painter cannot be assigned to null.");
        }

        public void SetDimensions(float height, float width)
        {
            m_Height = height;
            m_Width = width;
        }

        public void SetMinAndMax(float min, float max)
        {
            m_NiceMinY = min;
            m_NiceMaxY = max;
        }

        public void AllDataAreNegative(bool b)
        {
            m_AreAllNegative = b;
        }

        public void AllDataArePositive(bool b)
        {
            m_AreAllPositive = b;
        }
    }
}