using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
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

        public float BorderTop => m_BorderTop;
        public float BorderBottom => m_BorderBottom;
        public float BorderLeft => m_BorderLeft;
        public float BorderRight => m_BorderRight;

        private float m_PaddingBottom = 0;
        private float m_PaddingUpper = 0;
        private float m_PaddingLeft = 0;
        private float m_PaddingRight = 0;

        private float m_BorderTop;
        private float m_BorderBottom;
        private float m_BorderLeft;
        private float m_BorderRight;

        private float m_ChartHeightPercent = 0.8f;
        private float m_LegendHeightPercent = 0.2f;

        public float YStart => m_PaddingUpper + m_BorderTop;
        public float YEnd => m_Height - m_PaddingBottom - m_BorderBottom;
        public float XStart => m_PaddingLeft + m_BorderLeft;
        public float XEnd => m_Width - m_PaddingRight - m_BorderRight;

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



        public ChartLayout(float labelMargin, float fontSize)
        {
            if (labelMargin < 0)
                throw new InvalidLayoutException("Label margin can't be negative");
            if (fontSize <= 0)
                throw new InvalidLayoutException("Font size should be a positive number");
            
            m_LabelMargin = labelMargin;
            m_FontSize = fontSize;
        }

        public float CaclulateWidthOffset(List<float> yTicks, VisualElement chart)
        {
            m_WidthOffset = FindLongestLabelLength(yTicks, chart) + m_LabelMargin;
            return m_WidthOffset;
        }

        public float FindLongestLabelLength(List<float> yTicks, VisualElement chart)
        {
            float ans = 0;

            foreach (var tick in yTicks)
            {
                var currLength = Utils.EstimateLabelDimensionInPixels(tick.ToString(), chart, (int)m_FontSize).x;
                if (ans < currLength)
                    ans = currLength;
            }

            return ans;
        }

        public void SetVisualElementDimension(float height, float width)
        {
            var padding = new DimensionData(m_PaddingUpper, m_PaddingBottom, m_PaddingRight, m_PaddingLeft);
            var border = new DimensionData(m_BorderTop, m_BorderBottom, m_BorderRight, m_BorderLeft);
            
            ValidateLayout("Invalid height or width", height, width, padding, border);
            
            m_Height = height;
            m_Width = width;
        }

        public void SetPadding(float upper, float bottom, float left, float right)
        {
            if (upper < 0 || bottom < 0 || left < 0 || right < 0)
                throw new ArgumentException("Padding can not be negative.");
            
            var padding = new DimensionData(upper, bottom, right, left);
            var border = new DimensionData(m_BorderTop, m_BorderBottom, m_BorderRight, m_BorderLeft);
            
            ValidateLayout("Padding values are invalid.", m_Height, m_Width, padding, border);

            m_PaddingBottom = bottom;
            m_PaddingUpper = upper;
            m_PaddingLeft = left;
            m_PaddingRight = right;
        }

        public void SetBorder(float top, float bottom, float left, float right)
        {
            if (top < 0 || bottom < 0 || left < 0 || right < 0)
                throw new ArgumentException("Border can not be negative.");
            
            var padding = new DimensionData(m_PaddingUpper, m_PaddingBottom, m_BorderRight, m_BorderLeft);
            var border = new DimensionData(top, bottom, right, left);
            
            ValidateLayout("Border values are invalid.", m_Height, m_Width, padding, border);
            
            m_BorderTop = top;
            m_BorderBottom = bottom;
            m_BorderLeft = left;
            m_BorderRight = right;
        }

        private void ValidateLayout(string errorMessage, float height, float width, DimensionData padding, DimensionData border)
        {
            LayoutValidator validator = new LayoutValidator(width, height, padding, border);

            if (!validator.ValidateLayout())
                throw new InvalidLayoutException($"{errorMessage}: {height}, {width}");
        }

        public Rect GetChartDataArea()
        {
            Rect ans = new Rect();

            ans.x = XStart + m_WidthOffset;
            ans.y = YStart;
            ans.width = XEnd - ans.x;
            ans.height = ChartHeight;

            return ans;
        }

        public void SetXStepLength(int datasetLength)
        {
            if (datasetLength < 1)
                throw new ArgumentOutOfRangeException(nameof(datasetLength));
            
            m_XStepLength = GetChartDataArea().width / datasetLength;
        }

        private struct DimensionData
        {
            public float upper, bottom, right, left;

            public DimensionData(float upper, float bottom, float right, float left)
            {
                this.upper = upper;
                this.bottom = bottom;
                this.right = right;
                this.left = left;
            }
        }
        
        private struct LayoutValidator
        {
            private DimensionData m_Padding;
            private DimensionData m_Border;
            private float m_Width;
            private float m_Height;

            public LayoutValidator(float width, float height, DimensionData padding, DimensionData border)
            {
                m_Padding = padding;
                m_Border = border;
                m_Width = width;
                m_Height = height;
            }

            public bool ValidateHeightAndWidth()
            {
                return m_Width > 0 && m_Height > 0;
            }

            public bool ValidateVerticalDimensions()
            {
                return m_Height > m_Padding.upper + m_Padding.bottom + m_Border.upper +
                    m_Border.bottom;
            }

            public bool ValidateHorizontalDimensions()
            {
                return m_Width > m_Padding.left + m_Padding.right + m_Border.left +
                    m_Border.right;
            }

            public bool ValidateLayout()
            {
                return ValidateHeightAndWidth() && ValidateHorizontalDimensions() && ValidateVerticalDimensions();
            }
        }
    }

    public class InvalidLayoutException : Exception
    {
        public InvalidLayoutException(string message) : base(message)
        {
        }

        public InvalidLayoutException()
        {
        }
    }
}