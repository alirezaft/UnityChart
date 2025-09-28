using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class ChartLayout
    {
        private float m_Height;
        private float m_Width;

        public float PaddingBottom => m_PaddingBottom;
        public float PaddingUpper => m_PaddingUpper;
        public float PaddingLeft => m_PaddingLeft;
        public float PaddingRight => m_PaddingRight;
        
        private float m_PaddingBottom = 0;
        private float m_PaddingUpper = 0;
        private float m_PaddingLeft = 0;
        private float m_PaddingRight = 0;

        private float m_ChartHeightPercent = 0.8f;
        private float m_LegendHeightPercent = 0.2f;


        
        
        public float YStart => m_PaddingUpper;
        public float YEnd => m_Height - m_PaddingBottom;
        public float XStart => m_PaddingLeft;
        public float XEnd => m_Width - m_PaddingRight;

        public float AllowedHeight => YEnd - YStart;
        public float AllowedWidth => XEnd - XStart;


        public float ChartHeight => AllowedHeight * m_ChartHeightPercent;
        public float LegendHeight => AllowedHeight * m_LegendHeightPercent;
        
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

        public void SetVisualElementDimension(float height, float width)
        {
            m_Height = height;
            m_Width = width;
        }

        public void SetPadding(float upper, float bottom, float left, float right)
        {
            m_PaddingBottom = bottom;
            m_PaddingUpper = upper;
            m_PaddingLeft = left;
            m_PaddingRight = right;
        }
    }
}