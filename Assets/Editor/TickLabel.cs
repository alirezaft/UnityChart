using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class TickLabel
    {
        private float m_Height;
        private float m_Width;
        private float m_FontSize;
        private float m_LabelMargin;
        private int m_DataLength;
        
        private List<float> m_Numbers;
        private Painter2D m_Painter;
        private Axis m_Axis;
        private MeshGenerationContext m_MeshGenerationContext;

        public TickLabel(float fontSize, Axis axis)
        {
            m_FontSize = fontSize;
            m_Axis = axis;
        }
        
        public void PlaceYAxisTickLabels(List<float> ticks)
        {
            var labelDistance = m_Height / (ticks.Count - 1);
            var painterMovementVector = new Vector2(0, -labelDistance);
            var currPos = new Vector2(0, m_Height);

            for (int i = 0; i < ticks.Count; i++)
            {
                var text = ticks[i].ToString();
                var labelLength = Utils.EstimateLabelLengthInPixels(text, m_MeshGenerationContext.visualElement, (int)m_FontSize);
                var offset = new Vector2(/*m_WidthOffset*/ - labelLength - m_LabelMargin, 0);

                if(i < ticks.Count - 1){
                    m_MeshGenerationContext.DrawText(text, currPos + new Vector2(0, -1.1f * m_FontSize / 2) + offset, m_FontSize,
                        Color.white);
                }
                else
                {
                    m_MeshGenerationContext.DrawText(text, currPos + new Vector2(0, 0.006f * m_FontSize / 2) + offset, m_FontSize,
                        Color.white);
                }
                currPos += painterMovementVector;
            }
        }
        
        private void PlaceXAxisTickLabels(List<float> ticks, MeshGenerationContext context)
        {
            var dataSteps = m_Width / ((float)m_DataLength - 1);
            var dataToTickRatio = ((float)m_DataLength - 1) / (ticks.Count - 1);
            var labelPositionStep = dataSteps * dataToTickRatio;

            for (int i = 0; i < ticks.Count; i++)
            {
                var labelContent = (int)ticks[i];
                var labelPosition =
                    new Vector2(i * labelPositionStep,
                        m_Axis.ZeroOnYAxis);
                var labelAdjustmentVector = new Vector2(-(((int)Mathf.Log10(labelContent) + 1) * m_FontSize) / 2, 4);

                labelPosition += i == ticks.Count - 1
                    ? new Vector2(labelAdjustmentVector.x * 2, labelAdjustmentVector.y)
                    : labelAdjustmentVector;

                context.DrawText(labelContent.ToString(), labelPosition, m_FontSize, Color.white);
            }
        }

        public void SetMeshGenerationContext(MeshGenerationContext ctx)
        {
            m_MeshGenerationContext = ctx;
        }

        public void SetDimensions(float height, float width)
        {
            m_Height = height;
            m_Width = width;
        }

        public void SetDataLength(int length)
        {
            m_DataLength = length;
        }
    }
}