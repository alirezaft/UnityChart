using UnityChart.Editor;
using UnityChart.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class LegendOverflowBox : TextOverflowBox<DataProviderLegend>
{
    private ChartLegendLayout m_LegendLayout;

    public LegendOverflowBox(ChartLegendLayout layout) : base()
    {
        m_LegendLayout = layout;
        Debug.Log(m_BoxLayout);
    }

    public override void DrawBoxContent(DataProviderLegend[] content, Rect boxArea, MeshGenerationContext ctx)
    {
        var painter = ctx.painter2D;

        var painterSnapshot = new PainterSnapshot
            { FillColor = painter.fillColor, StrokeColor = painter.strokeColor, Width = painter.lineWidth };
        var currPos = new Vector2(boxArea.x + boxArea.width + m_BoxLayout.BoxPadding + m_LegendLayout.ColorIndicatorRadius,
            boxArea.y + boxArea.height + m_BoxLayout.BoxPadding + m_LegendLayout.ColorIndicatorRadius);

        for (int i = 0; i < content.Length; i++)
        {
            painter.BeginPath();
            painter.fillColor = content[i].Color;
            painter.strokeColor = content[i].Color;

            painter.MoveTo(currPos);
            painter.Arc(currPos, m_LegendLayout.ColorIndicatorRadius, 0, 360);
            painter.Fill();
            painter.Stroke();

            painter.ClosePath();
            painterSnapshot.RestorePainterData(painter);

            ctx.DrawText(content[i].Name,
                new Vector2(currPos.x + m_LegendLayout.TextAndColorSpacing + m_LegendLayout.ColorIndicatorRadius,
                    currPos.y - m_LegendLayout.Fontsize / 1.5f),
                m_LegendLayout.Fontsize, Color.white);

            currPos += new Vector2(0, m_LegendLayout.LegendSpacing);
        }
    }
}