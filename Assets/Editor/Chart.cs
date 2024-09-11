using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Chart : VisualElement
{
    private List<float> m_Dataset;
    public List<float> Dataset => m_Dataset;

    private float m_MinY => Mathf.Min(m_Dataset.Min(), 0);
    private float m_MaxY => m_Dataset.Max();
    private float m_MinX = 0;
    private float m_MaxX => m_Dataset.Count;

    private float m_ZeroOnYAxisPosition;

    private float m_ArrowSideLength = 10;
    private float m_ArrowHeadAngle = 30;

    public Chart()
    {
        generateVisualContent += UpdateWithOldDataset;
        m_Dataset = new List<float>();

        RepopulateDataset();
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

        DrawChartAxis(painter);
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
        painter.MoveTo(new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
            0));
        painter.LineTo(new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
            layout.height));

        m_ZeroOnYAxisPosition = (m_MaxY / (Mathf.Abs(m_MinY) + m_MaxY)) * layout.height;

        painter.MoveTo(new Vector2(0, m_ZeroOnYAxisPosition));
        painter.LineTo(new Vector2(layout.width, m_ZeroOnYAxisPosition));
        
        painter.Stroke();
        painter.ClosePath();
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

    public void RepopulateDataset()
    {
        m_Dataset.Clear();
        for (int i = 0; i < 10; i++)
        {
            m_Dataset.Add(Random.value * -10 + (Random.value * 5));
        }

        Debug.Log($"Min: {m_MinY}, Max: {m_MaxY}");
        MarkDirtyRepaint();
    }

    public new class UxmlFactory : UxmlFactory<Chart, UxmlTraits>
    {
    }
}