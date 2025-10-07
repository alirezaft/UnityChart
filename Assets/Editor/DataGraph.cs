using System.Collections.Generic;
using UnityChart.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
{
    public class DataGraph
    {
        private List<DataProvider> m_DataProviders;
        private float m_NiceMaxY;
        private float m_NiceMinY;
        private Axis m_Axis;
        private ChartLayout m_ChartLayout;

        public DataGraph(ChartLayout chartLayout, Axis axis, List<DataProvider> provider)
        {
            m_DataProviders = provider;
            m_Axis = axis;
            m_ChartLayout = chartLayout;
        }

        public void DrawDataGraphs(Painter2D painter)
        {
            var offset = m_ChartLayout.WidthOffset;
            var xSteps = (m_ChartLayout.AllowedWidth - offset) / (Utils.GetMaxDataProviderLength(m_DataProviders) - 1);

            foreach (var provider in m_DataProviders)
            {
                if(provider.Dataset.Count == 0)
                    continue;
                
                var latestPointOnXAxis = offset + m_ChartLayout.XStart;

                painter.BeginPath();
                painter.lineWidth = 1f;
                painter.strokeColor = provider.Color;
                painter.fillColor = new Color(provider.Color.r, provider.Color.g, provider.Color.b, 0.3f);
                painter.MoveTo(new Vector2(offset + m_ChartLayout.XStart, m_Axis.ZeroOnYAxis));
                var YPos = FindValueOnChartYAxis(provider.Dataset[0],
                    m_NiceMinY,
                    m_NiceMaxY, m_ChartLayout.YStart, m_ChartLayout.ChartHeight + m_ChartLayout.YStart);


                var currPos = new Vector2(offset + m_ChartLayout.XStart, YPos);
                provider.DataPointPositions[0] = currPos;
                painter.LineTo(currPos);
                var dataset = provider.Dataset;

                for (int i = 1; i < dataset.Count; i++)
                {
                    var dataPointY = FindValueOnChartYAxis(dataset[i], m_NiceMinY,
                        m_NiceMaxY,
                        m_ChartLayout.YStart, m_ChartLayout.ChartHeight + m_ChartLayout.YStart);

                    if (Utils.DoValuesHaveDifferentSigns(dataset[i], dataset[i - 1]))
                    {
                        var nextPoint = new Vector2(currPos.x + xSteps, dataPointY);
                        var intersectionPoint = FindIntersectionWithXAxis(currPos, nextPoint);

                        painter.LineTo(intersectionPoint);
                        painter.LineTo(new Vector2(latestPointOnXAxis, m_Axis.ZeroOnYAxis));
                        painter.ClosePath();
                        painter.Stroke();
                        painter.Fill();

                        painter.BeginPath();
                        painter.MoveTo(intersectionPoint);
                        painter.LineTo(new Vector2(currPos.x + xSteps, dataPointY));

                        currPos = new Vector2(currPos.x + xSteps, dataPointY);
                        provider.DataPointPositions[i] = currPos;
                        latestPointOnXAxis = intersectionPoint.x;
                    }
                    else
                    {
                        painter.LineTo(new Vector2(currPos.x + xSteps, dataPointY));
                        currPos = new Vector2(currPos.x + xSteps, dataPointY);
                        provider.DataPointPositions[i] = currPos;
                    }
                }

                painter.LineTo(new Vector2(currPos.x, m_Axis.ZeroOnYAxis));
                painter.ClosePath();
                painter.Stroke();
                painter.Fill();
            }

            painter.lineWidth = 2;
        }

        private Vector2 FindIntersectionWithXAxis(Vector2 point1, Vector2 point2)
        {
            var slope = (point1.y - point2.y) / (point1.x - point2.x);

            var yIntercept = new Vector2(point1.y - (slope * point1.x), m_Axis.ZeroOnYAxis);

            return new Vector2((m_Axis.ZeroOnYAxis - yIntercept.x) / slope, m_Axis.ZeroOnYAxis);
        }

        private float FindValueOnChartYAxis(float value, float sourceMin, float sourceMax,
            float destinationMin, float destinationMax)
        {
            float t = (value - sourceMin) / (sourceMax - sourceMin);
            
            return Mathf.Lerp(destinationMax, destinationMin, t);
        }

        public void SetMinAndMax(float minY, float maxY)
        {
            m_NiceMaxY = maxY;
            m_NiceMinY = minY;
        }
        
    }
}