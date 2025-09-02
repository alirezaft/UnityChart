using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Chart : VisualElement
{ 
    private List<DataProvider> m_DataProviders;

    private float m_MinY;
    private float m_MaxY;
    private float m_MinX = 0;
    private float m_MaxX;

    private float m_ZeroOnYAxisPosition;

    private float m_ArrowSideLength = 10;
    private float m_ArrowHeadAngle = 30;

    public Chart()
    {
        generateVisualContent += UpdateWithOldDataset;
        m_DataProviders = new List<DataProvider>();

        RepopulateDataset();
    }

    private void CalculateMinAndMaxValues()
    {
        CalculateMaxY();
        CalculateMinY();
        CalculateMaxX();
    }

    private void CalculateMaxY()
    {
        var answer = float.NegativeInfinity;
        foreach (var provider in m_DataProviders)
        {
            if (provider.MaxValue > answer)
                answer = provider.MaxValue;
        }

        m_MaxY = answer;
    }

    private void CalculateMinY()
    {
        var answer = float.PositiveInfinity;
        foreach (var provider in m_DataProviders)
        {
            if (provider.MinValue < answer)
                answer = provider.MinValue;
        }

        m_MinY = answer;
    }

    private void CalculateMaxX()
    {
        var answer = 0f;

        foreach (var provider in m_DataProviders)
        {
            if (provider.Length > answer)
                answer = provider.Length;
        }

        m_MaxX = answer;
    }

    public void DrawChart()
    {
        //TODO: Update the old dataset
        MarkDirtyRepaint();
    }

    private void UpdateWithOldDataset(MeshGenerationContext ctx)
    {
        var chartHeight = layout.height;
        var chartWidth = layout.width;

        var painter = ctx.painter2D;

        CalculateMinAndMaxValues();
        DrawChartAxis(painter);
        DrawDataGraphs(painter);
    }

    private void DrawChartAxis(Painter2D painter)
    {
        painter.BeginPath();
        painter.lineWidth = 2f;

        DrawAxisLines(painter);
        DrawAxisArrows(painter);

        painter.MoveTo(Vector2.zero);


        painter.Stroke();
        painter.ClosePath();
    }

    private void DrawAxisLines(Painter2D painter)
    {
        painter.strokeColor = Color.white;

        DrawVerticalAxisLine(painter);
        DrawHorizontalAxisLine(painter);

        painter.Stroke();
        painter.ClosePath();
    }

    private void DrawVerticalAxisLine(Painter2D painter)
    {
        painter.MoveTo(new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
            0));
        painter.LineTo(new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
            layout.height));
    }

    private void DrawHorizontalAxisLine(Painter2D painter)
    {
        m_ZeroOnYAxisPosition = (m_MaxY / (Mathf.Abs(m_MinY) + m_MaxY)) * layout.height;

        if (AreAllElementsNegative())
        {
            Debug.Log(AreAllElementsNegative());
            m_ZeroOnYAxisPosition = -Mathf.Sin(180 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength;
        }

        painter.MoveTo(new Vector2(0, m_ZeroOnYAxisPosition));
        painter.LineTo(new Vector2(layout.width, m_ZeroOnYAxisPosition));
    }

    private bool AreAllElementsNegative()
    {
        var answer = true;

        foreach (var provider in m_DataProviders)
        {
            answer = answer & provider.Dataset.ToList().TrueForAll(item => item < 0);
        }

        return answer;
    }

    private void DrawAxisArrows(Painter2D painter)
    {
        DrawHorizontalAxisArrow(painter);
        DrawVerticalAxisArrow(painter);
    }

    private void DrawHorizontalAxisArrow(Painter2D painter)
    {
        var xAxisEnd = new Vector2(layout.width, m_ZeroOnYAxisPosition);
        painter.MoveTo(xAxisEnd);

        painter.LineTo(new Vector2(
            Mathf.Cos((180 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.x,
            Mathf.Sin((180 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.y));

        painter.MoveTo(xAxisEnd);
        painter.LineTo(new Vector2(
            Mathf.Cos((180 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.x,
            Mathf.Sin((180 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.y));

        painter.Stroke();
        painter.ClosePath();
    }

    private void DrawVerticalAxisArrow(Painter2D painter)
    {
        var yAxisEnd = new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
            0);
        painter.MoveTo(yAxisEnd);

        painter.LineTo(new Vector2(
            -Mathf.Cos((270 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.x,
            -Mathf.Sin((270 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.y));

        painter.MoveTo(yAxisEnd);
        painter.LineTo(new Vector2(
            -Mathf.Cos((270 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.x,
            -Mathf.Sin((270 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.y));

        painter.Stroke();
        painter.ClosePath();
    }

    private void DrawDataGraphs(Painter2D painter)
    {
        foreach (var provider in m_DataProviders)
        {
            
            var dataset = provider.Dataset;
            var xAxisStepSize = layout.width / dataset.Length;

            painter.strokeColor = provider.Color;
            painter.lineWidth = 1.2f;
            painter.fillColor = painter.fillColor = new Color(provider.Color.r, provider.Color.g, provider.Color.b, 0.3f);

            float startX = -Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength;
            float baselineY = m_ZeroOnYAxisPosition;

            float prevX = startX;
            float prevY = FindValueOnCharYAxis(dataset[0], provider.MinValue, provider.MaxValue, 0, layout.height);

            painter.BeginPath();
            painter.MoveTo(new Vector2(prevX, baselineY));
            painter.LineTo(new Vector2(prevX, prevY));

            for (int i = 1; i < dataset.Length; i++)
            {
                float currX = startX + i * xAxisStepSize;
                float currY = FindValueOnCharYAxis(dataset[i], provider.MinValue, provider.MaxValue, 0, layout.height);

                bool prevAbove = prevY >= baselineY;
                bool currAbove = currY >= baselineY;

                if (prevAbove != currAbove)
                {
                    float t = (baselineY - prevY) / (currY - prevY);
                    float ix = prevX + t * (currX - prevX);
                    float iy = baselineY;

                    painter.LineTo(new Vector2(ix, iy));
                    painter.LineTo(new Vector2(prevX, baselineY));
                    painter.Fill();
                    painter.Stroke();

                    painter.BeginPath();
                    painter.MoveTo(new Vector2(ix, baselineY));
                    painter.LineTo(new Vector2(ix, currY));
                }
                else
                {
                    painter.LineTo(new Vector2(currX, currY));
                }

                prevX = currX;
                prevY = currY;
            }

            painter.LineTo(new Vector2(prevX, baselineY));
            painter.Fill();
            painter.Stroke();
        }
    }

    private float FindValueOnCharYAxis(float value, float sourceMin, float sourceMax, float destinationMin, float destinationMax)
    {
        
        float t = (value - sourceMin) / (sourceMax - sourceMin);

        Debug.Log("Actual Y: " + (destinationMin - t * (-destinationMax + destinationMin)));
        return destinationMax - t * (+destinationMax - destinationMin);
 
    }

    public void RepopulateDataset()
    {
        m_DataProviders.Clear();
        m_DataProviders.Add(new DataProvider(Color.green, "Test"));
        m_DataProviders.Add(new DataProvider(Color.red, "Test2"));
        // m_DataProviders[0].AddDataPoint(0);
        // m_DataProviders[0].AddDataPoint(1);
        // m_DataProviders[0].AddDataPoint(3);
        // m_DataProviders[0].AddDataPoint(-2);
        
        // m_DataProviders[0].AddDataPoint(-3);
        for (int i = 0; i < 50; i++)
        {
            m_DataProviders[0].AddDataPoint(Random.Range(-2, 2) * 5);
        }
        
        for (int i = 0; i < 32; i++)
        {
            m_DataProviders[1].AddDataPoint(Random.value * -5);
        }

        MarkDirtyRepaint();
    }

    public void AddDataProvider(DataProvider provider)
    {
        m_DataProviders.Add(provider);
    }
}