using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class ChartLayout
    {
        private float m_Height;
        private float m_Width;

        public readonly float MinWidth = 250f;
        public readonly float MinHeight = 150f;

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
        private float m_XStepLength;
        public float XStepLength => m_XStepLength;

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

        public float FindLongestLabelLength(List<float> yTicks)
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
            LayoutValidator validator = new LayoutValidator(m_Width, m_Height);
            
            if (!validator.ValidateHorizontalPadding(left, right))
                throw new InvalidLayoutException("Left and right paddings sum is larger than width.");

            if (!validator.ValidateVerticalPadding(upper, bottom))
                throw new InvalidLayoutException("Upper and bottom paddings sum is larger than height");
            
            m_PaddingBottom = bottom;
            m_PaddingUpper = upper;
            m_PaddingLeft = left;
            m_PaddingRight = right;
        }

        public Rect GetChartDataArea()
        {
            Rect ans = new Rect();

            ans.x = XStart + m_WidthOffset;
            ans.y = PaddingUpper;
            ans.width = XEnd - ans.x;
            ans.height = ChartHeight;

            return ans;
        }

        public void SetXStepLength(int datasetLength)
        {
            m_XStepLength = GetChartDataArea().width / datasetLength;
        }
        
        private struct LayoutValidator
        {
            private float m_Height;
            private float m_Width;
            
            public LayoutValidator(float height, float width)
            {
                m_Height = height;
                m_Width = width;
            }

            public bool ValidateVerticalPadding(float upper, float bottom)
            {
                return m_Height > upper + bottom;
            }
            
            public bool ValidateHorizontalPadding(float left, float right)
            {
                return m_Width > left + right;
            }
        }
    }

    public class InvalidLayoutException : Exception
    {
        public InvalidLayoutException(string message):base(message){}
        public InvalidLayoutException(){}
    }
}