using UnityEngine.UIElements;
using UnityChart.Runtime;

namespace UnityChart.Editor
{
    public class ChartLegendLayout
    {
        private float m_ColorIndicatorRadius;
        public float ColorIndicatorRadius => m_ColorIndicatorRadius;
        
        private float m_LegendSpacing;
        public float LegendSpacing => m_LegendSpacing;
        
        private float m_TextAndColorSpacing;
        public float TextAndColorSpacing => m_TextAndColorSpacing;
        
        private float m_FontSize;
        public float Fontsize => m_FontSize;

        private VisualElement m_ChartElement;

        public ChartLegendLayout(float colorIndicatorRadius, float legendSpacing, float textColorSpacing,
            float fontSize, VisualElement chart)
        {
            m_ColorIndicatorRadius = colorIndicatorRadius;
            m_LegendSpacing = legendSpacing;
            m_TextAndColorSpacing = textColorSpacing;
            m_FontSize = fontSize;
            m_ChartElement = chart;
        }

        public float EstimateLegendWidth(DataProviderLegend legend)
        {
            var ans = m_ColorIndicatorRadius * 2;
            ans += m_TextAndColorSpacing;
            ans += Utils.EstimateLabelDimensionInPixels(legend.Name, m_ChartElement, (int)m_FontSize).x;

            return ans;
        }
    }
}