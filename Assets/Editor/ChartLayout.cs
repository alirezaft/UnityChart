using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class ChartLayout
    {
        private float m_Height;
        private float m_Width;
        
        private float m_WidthOffset;
        public float WidthOffset => m_WidthOffset;
        
        private float m_LabelMargin;
        public float LabelMargin => m_LabelMargin;
        
        private float m_FontSize;

        private VisualElement m_Chart;

        public ChartLayout(float labelMargin, float fontSize, VisualElement chart)
        {
            m_LabelMargin = labelMargin;
            m_FontSize = fontSize;
            m_Chart = chart;
        }

        public float CaclulateWidthOffset(List<float> yTicks)
        {
            m_WidthOffset = FindLongestLabelLength(yTicks) + m_LabelMargin;
            return m_WidthOffset;
        }

        private float FindLongestLabelLength(List<float> yTicks)
        {
            float ans = 0;

            foreach (var tick in yTicks)
            {
                var currLength = Utils.EstimateLabelLengthInPixels(tick.ToString(), m_Chart, (int)m_FontSize);
                if (ans < currLength)
                    ans = currLength;
            }

            return ans;
        }
    }
}