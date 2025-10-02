using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    [UxmlElement]
    public partial class LineChart : VisualElement
    {
        private List<DataProvider> m_DataProviders;

        private float m_MinY;
        private float m_MaxY;
        private float m_MinX = 0;
        private float m_MaxX;
        private float m_NiceMaxY;
        private float m_NiceMinY;
        private float m_YLabelMargin = 4f;

        private float m_FontSize = 10f;
        private List<float> m_XTicks;
        private List<float> m_YTicks;

        private Axis m_Axis;
        private Ticks m_Ticks;
        private TickLabel m_Labels;
        private ChartLayout m_ChartLayout;
        private ChartLegend m_Legend;

        public LineChart()
        {
            m_DataProviders = new List<DataProvider>();
            RepopulateDataset();
            generateVisualContent += UpdateWithOldDataset;
            

            m_ChartLayout = new ChartLayout(m_YLabelMargin, m_FontSize, this);
            m_Axis = new Axis(m_ChartLayout.ChartHeight, layout.width, m_ChartLayout);
            m_Ticks = new Ticks(m_Axis, m_DataProviders[0].Length, 4f, m_ChartLayout);
            m_Labels = new TickLabel(m_FontSize, m_Axis, m_ChartLayout);
            m_Legend = new ChartLegend(m_ChartLayout, this);
        }

        protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
        { 
            var maxYLabelWidth = m_ChartLayout.FindLongestLabelLength(m_YTicks);
            var legendHeight = m_ChartLayout.LegendHeight;
            var contentWidth = 40 + maxYLabelWidth + 300;
            var contentHeight = 20 + 200 + legendHeight;

            return new Vector2(contentWidth, contentHeight);
        }

        private void CalculateMinAndMaxValues()
        {
            m_MaxY = Utils.FindMaxAmongAllDataProviders(m_DataProviders);
            m_MinY = Utils.FindMinAmongAllDataProviders(m_DataProviders);
            CalculateMaxX();
        }

        private void CalculateMaxX()
        {
            var answer = 0f;

            foreach (var provider in m_DataProviders)
            {
                if (provider.Length > answer)
                    answer = provider.Length;
            }

            m_MaxX = answer;
        }

        public void DrawChart()
        {
            //TODO: Update the old dataset
            MarkDirtyRepaint();
        }

        private void UpdateWithOldDataset(MeshGenerationContext ctx)
        {
            InitLayout();
            var painter = ctx.painter2D;
            
            CalculateMinAndMaxValues();
            CalculateAxisScaleAndOffset();
            DrawAxes(ctx.painter2D);

            DrawTicks(painter, ctx);
            DrawLabels(ctx);

            DrawDataGraphs(painter);
            DrawChartLegend(painter, ctx);
        }

        private void InitLayout()
        {
            this.style.minWidth = new StyleLength(m_ChartLayout.MinWidth);
            this.style.minHeight = new StyleLength(m_ChartLayout.MinHeight);
            m_ChartLayout.SetVisualElementDimension(layout.height, layout.width);
            
            var style = this.style;
            Debug.Log($"paddingTop: {resolvedStyle.paddingTop}");
            m_ChartLayout.SetPadding(Utils.LengthToFloat(resolvedStyle.paddingTop), Utils.LengthToFloat(resolvedStyle.paddingBottom),
                Utils.LengthToFloat(resolvedStyle.paddingLeft),
                Utils.LengthToFloat(resolvedStyle.paddingRight));
        }

        private void DrawChartLegend(Painter2D painter, MeshGenerationContext ctx)
        {
            m_Legend.ClearLegends();
            foreach (var provider in m_DataProviders)
            {
                m_Legend.AddLegend(provider.GetLegend());
            }

            m_Legend.SetContext(ctx);
            m_Legend.SetPainter(painter);
            m_Legend.SetDimension(layout.height, layout.width);

            m_Legend.DrawLegends();
        }

        private void DrawAxes(Painter2D painter)
        {
            m_Axis.SetPainter(painter);
            m_Axis.SetDimensions(m_ChartLayout.ChartHeight, layout.width);
            m_Axis.AllDataAreNegative(Utils.AreAllElementsNegative(m_DataProviders));
            m_Axis.AllDataArePositive(Utils.AreAllElementsPositive(m_DataProviders));

            m_Axis.SetMinAndMax(m_NiceMinY, m_NiceMaxY);
            m_Axis.DrawChartAxis();
            
        }

        private void DrawLabels(MeshGenerationContext ctx)
        {
            m_Labels.SetDimensions(m_ChartLayout.ChartHeight, layout.width);
            m_Labels.SetMeshGenerationContext(ctx);
            m_Labels.SetDataLength(m_DataProviders[0].Length);

            m_Labels.PlaceYAxisTickLabels(m_YTicks);
        }

        private void DrawDataGraphs(Painter2D painter)
        {
            foreach (var provider in m_DataProviders)
            {
                var offset = m_ChartLayout.CaclulateWidthOffset(m_YTicks);
                var latestPointOnXAxis = offset + m_ChartLayout.XStart;
                var xSteps = (m_ChartLayout.AllowedWidth - offset) / (provider.Length - 1);
                Debug.Log($"data steps x2 {xSteps}");

                painter.BeginPath();
                painter.lineWidth = 1.5f;
                painter.strokeColor = provider.Color;
                painter.fillColor = new Color(provider.Color.r, provider.Color.g, provider.Color.b, 0.3f);
                painter.MoveTo(new Vector2(offset + m_ChartLayout.XStart,  m_Axis.ZeroOnYAxis));
                var YPos = FindValueOnChartYAxis(provider.Dataset[0],
                    m_NiceMinY,
                    m_NiceMaxY, m_ChartLayout.YStart, m_ChartLayout.ChartHeight + m_ChartLayout.YStart);


                var currPos = new Vector2(offset + m_ChartLayout.XStart, YPos);
                painter.LineTo(currPos);
                var dataset = provider.Dataset;

                for (int i = 1; i < dataset.Count; i++)
                {
                    var dataPointY = FindValueOnChartYAxis(dataset[i], m_NiceMinY,
                        m_NiceMaxY,
                        m_ChartLayout.YStart, m_ChartLayout.ChartHeight + m_ChartLayout.YStart);

                    if (Utils.DoValuesHaveDifferentSigns(dataset[i], dataset[i - 1]))
                    {
                        var nextPoint = new Vector2(currPos.x + xSteps, dataPointY);
                        var intersectionPoint = FindIntersectionWithXAxis(currPos, nextPoint);
                        // Debug.Log($"intersection {intersectionPoint}");

                        painter.LineTo(intersectionPoint);
                        painter.LineTo(new Vector2(latestPointOnXAxis, m_Axis.ZeroOnYAxis));
                        painter.ClosePath();
                        painter.Stroke();
                        painter.Fill();

                        painter.BeginPath();
                        painter.MoveTo(intersectionPoint);
                        painter.LineTo(new Vector2(currPos.x + xSteps, dataPointY));

                        currPos = new Vector2(currPos.x + xSteps, dataPointY);
                        latestPointOnXAxis = intersectionPoint.x;
                    }
                    else
                    {
                        painter.LineTo(new Vector2(currPos.x + xSteps, dataPointY));
                        currPos = new Vector2(currPos.x + xSteps, dataPointY);
                    }
                }

                painter.LineTo(new Vector2(currPos.x, m_Axis.ZeroOnYAxis));
                painter.ClosePath();
                painter.Stroke();
                painter.Fill();
            }

            painter.lineWidth = 2;
        }

        private Vector2 FindIntersectionWithXAxis(Vector2 point1, Vector2 point2)
        {
            var slope = (point1.y - point2.y) / (point1.x - point2.x);

            var yIntercept = new Vector2(point1.y - (slope * point1.x), m_Axis.ZeroOnYAxis);

            return new Vector2((m_Axis.ZeroOnYAxis - yIntercept.x) / slope, m_Axis.ZeroOnYAxis);
        }

        private void CalculateAxisScaleAndOffset()
        {
            var length = Utils.GetMaxDataProviderLength(m_DataProviders);
            var numOfDigits = Utils.GetNumberOfDigits(length);

            var niceScaleX = new NiceScale(1, length, true);
            var niceScaleY = new NiceScale(m_MinY, m_MaxY, false);
            niceScaleY.SetMaxTicks(10);
            m_NiceMaxY = niceScaleY.NiceMax;
            m_NiceMinY = niceScaleY.NiceMin;

            m_XTicks = niceScaleX.GetTicks();
            m_YTicks = niceScaleY.GetTicks();
            m_ChartLayout.CaclulateWidthOffset(m_YTicks);
        }

        private void DrawTicks(Painter2D painter, MeshGenerationContext context)
        {
            m_Ticks.SetDimensions(m_ChartLayout.ChartHeight, layout.width);
            m_Ticks.SetPainter(painter);
            m_Ticks.PlaceTicksOnYAxis(m_YTicks.Count);
        }

        private float FindValueOnChartYAxis(float value, float sourceMin, float sourceMax,
            float destinationMin, float destinationMax)
        {
            float t = (value - sourceMin) / (sourceMax - sourceMin);
            // flip because UI Toolkit Y increases downward
            return Mathf.Lerp(destinationMax, destinationMin, t);
        }

        public void RepopulateDataset()
        {
            m_DataProviders.Clear();
            m_DataProviders.Add(new DataProvider(Color.green, "Test"));
            // m_DataProviders[0].Dataset = new List<float>() { 1, 0, 1, 2, -3, 5 };
            m_DataProviders.Add(new DataProvider(Color.red, "Test2"));
            // m_DataProviders[0].AddDataPoint(0);
            // m_DataProviders[0].AddDataPoint(1);
            // m_DataProviders[0].AddDataPoint(3);
            // m_DataProviders[0].AddDataPoint(-2);

            // m_DataProviders[0].AddDataPoint(-3);
            StringBuilder builder = new StringBuilder();
            builder.Append("Dataset 1: [");
            // foreach (var f in m_DataProviders[0].Dataset)
            // {
            //     builder.Append(f + ", ");
            // }
            //
            for (int i = 0; i < 20; i++)
            {
                m_DataProviders[0].AddDataPoint(Random.value * -10);
                builder.Append(m_DataProviders[0].Dataset[i] + ", ");
            }

            // m_DataProviders[0].Dataset = new List<float>()
            // {
            //     8.781604f, 0.364883f, 1.010233f, 3.683865f, 2.140091f, 3.60898f, 1.958959f, 4.292889f, 8.128839f,
            //     8.731843f, 4.387875f, 0.8151687f, 0.2233648f, 6.665278f, 5.709448f, 6.145482f, 3.034684f, 7.503264f,
            //     1.147786f, 5.586436f
            // };

            m_DataProviders[1].Dataset = new List<float>()
            {
                -2.749644f, -0.6666476f, 1.935474f, 4.462473f, -4.529138f, 0.2189499f, 3.807502f, 0.1693755f,
                -0.4716486f, -1.000425f, 0.9232992f, -3.673697f, 0.6259388f, 1.672141f, -2.982634f, 1.078195f,
                -2.848732f, -0.5303007f, -3.71349f, -2.054001f
            };


            builder.Append("]");


            // for (int i = 0; i < 20; i++)
            // {
            //     m_DataProviders[1].AddDataPoint((Random.value * 10) - 5);
            //     builder.Append(m_DataProviders[1].Dataset[i] + ", ");
            // }

            builder.Append("]");
            Debug.Log(builder.ToString());

            MarkDirtyRepaint();
        }

        public void AddDataProvider(DataProvider provider)
        {
            m_DataProviders.Add(provider);
        }
    }
}