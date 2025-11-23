using UnityChart.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
{
    public abstract class TextOverflowBox<T>
    {
        private Rect? m_OverflowButtonArea;
        private Vector2? m_MousePosition;

        private float m_Width;
        private float m_Height;
        private bool m_IsHoveringOnOverflowButton;

        protected OverflowBoxLayout m_BoxLayout;

        public TextOverflowBox()
        {
            m_BoxLayout = new OverflowBoxLayout(8f, 8f);
        }

        public Rect DrawOverflowBox(Painter2D painter, Vector2 mousePosition)
        {
            var currPos = mousePosition;

            var painterSnapshot = new PainterSnapshot
                { FillColor = painter.fillColor, StrokeColor = painter.strokeColor, Width = painter.lineWidth };
            
            painter.lineWidth = 1f;
            painter.fillColor = new Color(0.22f, 0.22f, 0.22f, 1f);
            painter.strokeColor = new Color(0.4f, 0.4f, 0.4f, 1f);

            painter.BeginPath();

            painter.MoveTo(currPos);
            painter.LineTo(currPos + new Vector2(-m_Width, 0));
            currPos += new Vector2(-m_Width, 0);
            painter.LineTo(currPos + new Vector2(0, -m_Height));
            currPos += new Vector2(0, -m_Height);
            painter.LineTo(currPos + new Vector2(m_Width, 0));
            currPos += new Vector2(m_Width, 0);
            painter.LineTo(currPos + new Vector2(0, m_Height));

            painter.Fill();
            painter.Stroke();

            painter.ClosePath();

            painterSnapshot.RestorePainterData(painter);

            return new Rect(mousePosition.x, mousePosition.y, -m_Width, -m_Height);
        }

        public abstract void DrawBoxContent(T[] content, Rect boxArea, MeshGenerationContext ctx);

        public void SetBoxDimensions(float width, float height)
        {
            m_Width = width;
            m_Height = height;
        }

        public OverflowBoxLayout GetLayout()
        {
            return m_BoxLayout;
        }
    }

    public class OverflowBoxLayout
    {
        private float m_BoxPadding;
        public float BoxPadding => m_BoxPadding;

        private float m_ContentEntrySpacing;
        public float ContentEntrySpacing => m_ContentEntrySpacing;

        public OverflowBoxLayout(float padding, float contentEntrySpacing)
        {
            m_BoxPadding = padding;
            m_ContentEntrySpacing = contentEntrySpacing;
        }
    }
}