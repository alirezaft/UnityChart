using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
{
    public struct PainterSnapshot
    {
        public float Width;
        public Color FillColor;
        public Color StrokeColor;

        public void RestorePainterData(Painter2D painter)
        {
            painter.strokeColor = StrokeColor;
            painter.fillColor = FillColor;
            painter.lineWidth = Width;
        }
    }
}