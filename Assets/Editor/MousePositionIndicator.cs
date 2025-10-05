using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class MousePositionIndicator
    {
        private ChartLayout m_ChartLayout;
        private Vector2? m_MousePosition;
        private float m_IndicatorLinePosition;

        private List<List<Vector2>> m_DataPoints;
        private bool m_FirstSearch;
        private int m_LastIndex;
        
        public int CurrentIndex => m_LastIndex;
        public float IndicatorXPosition => m_IndicatorLinePosition;

        public MousePositionIndicator(ChartLayout layout)
        {
            m_ChartLayout = layout;
            m_DataPoints = new List<List<Vector2>>();

            m_FirstSearch = true;
            m_LastIndex = -1;
        }

        public void Draw(Painter2D painter)
        {
            if (m_MousePosition == null)
                return;

            var mousePos = m_MousePosition.Value;

            var painterData = new PainterSnapshot
                { FillColor = painter.fillColor, StrokeColor = painter.strokeColor, Width = painter.lineWidth };

            painter.lineWidth = 0.5f;
            painter.strokeColor = Color.white;

            var pointList = m_DataPoints[0];
            Vector2 indicatorPosition;

            if (m_FirstSearch)
                indicatorPosition = FindNearestPointToMouse(pointList, mousePos.x);
            else
            {
                var pointBefore = pointList[Mathf.Max(0, m_LastIndex - 1)];
                var pointAfter = pointList[Mathf.Min(m_LastIndex + 1, m_DataPoints.Count - 1)];

                indicatorPosition = FindNearestPointToMouse(pointList[m_LastIndex], pointBefore, pointAfter, mousePos);
            }

            m_IndicatorLinePosition = indicatorPosition.x;

            painter.BeginPath();
            painter.MoveTo(new Vector2(indicatorPosition.x, m_ChartLayout.YStart));
            painter.LineTo(new Vector2(indicatorPosition.x, m_ChartLayout.ChartHeight));
            painter.Stroke();
            painter.ClosePath();

            painterData.RestorePainterData(painter);
        }

        private Vector2 AdjustMousePositionWithLayout(Vector2 pos)
        {
            return new Vector2(pos.x + m_ChartLayout.WidthOffset + m_ChartLayout.XStart, pos.y);
        }

        private bool IsMouseInDataArea(Vector2 position)
        {
            var dataRect = m_ChartLayout.GetChartDataArea();

            return dataRect.Contains(position);
        }

        public void UpdateMousePosition(Vector2 position)
        {
            if (IsMouseInDataArea(position))
                m_MousePosition = position;
            else
                m_MousePosition = null;
        }

        private Vector2 FindNearestPointToMouse(List<Vector2> points, float mouseX)
        {
            int low = 0;
            int high = points.Count - 1;

            while (low <= high)
            {
                int mid = (low + high) / 2;
                float midX = points[mid].x;

                if (midX < mouseX)
                    low = mid + 1;
                else
                    high = mid - 1;
            }

            
            if (low >= points.Count)
            {
                m_LastIndex = points.Count - 1;
                return points[^1];
            }
            if (low <= 0)
            {
                m_LastIndex = 0;
                return points[0];
            }

            float d1 = Mathf.Abs(points[low].x - mouseX);
            float d2 = Mathf.Abs(points[low - 1].x - mouseX);

            m_LastIndex = d1 < d2 ? low : low - 1;
            
            return points[m_LastIndex];

        }

        private Vector2 FindNearestPointToMouse(Vector2 currPoint, Vector2 pointBefore, Vector2 pointAfter,
            Vector2 mousePos)
        {
            var currPointDiff = Mathf.Abs(mousePos.x - currPoint.x);
            var beforePointDiff = Mathf.Abs(mousePos.x - pointBefore.x);
            var afterPointDiff = Mathf.Abs(mousePos.x - pointAfter.x);

            var x = Mathf.Min(currPointDiff, beforePointDiff, afterPointDiff);

            if (x == beforePointDiff)
                m_LastIndex--;
            else if (x == afterPointDiff)
                m_LastIndex++;

            return new Vector2(x, currPoint.y);
        }

        public void AddDataPointList(List<Vector2> list)
        {
            m_DataPoints.Add(list);
        }

        public void Reset()
        {
            m_MousePosition = null;
            m_FirstSearch = true;
            m_LastIndex = -1;
        }
    }
}