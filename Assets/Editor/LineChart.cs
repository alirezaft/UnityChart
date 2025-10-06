using System.Collections.Generic;
using System.Text;
using UnityChart.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
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
        private MousePositionIndicator m_PositionIndicator;
        private DataGraph m_DataGraph;
        private ChartTooltip m_Tooltip;

        public LineChart()
        {
            m_DataProviders = new List<DataProvider>();
            generateVisualContent += UpdateWithOldDataset;


            m_ChartLayout = new ChartLayout(m_YLabelMargin, m_FontSize, this);
            m_PositionIndicator = new MousePositionIndicator(m_ChartLayout);
            RepopulateDataset();
            m_Axis = new Axis(m_ChartLayout.ChartHeight, layout.width, m_ChartLayout);
            m_DataGraph = new DataGraph(m_ChartLayout, m_Axis, m_DataProviders);
            m_Ticks = new Ticks(m_Axis, m_DataProviders[0].Length, 4f, m_ChartLayout);
            m_Labels = new TickLabel(m_FontSize, m_Axis, m_ChartLayout);
            m_Legend = new ChartLegend(m_ChartLayout, this);
            m_Tooltip = new ChartTooltip(m_ChartLayout, m_DataProviders, this);

            RegisterChartEvents();
        }

        

        private void RegisterChartEvents()
        {
            RegisterCallback<MouseMoveEvent>(UpdateMouseIndicatorPosition);
            RegisterCallback<MouseMoveEvent>(UpdateTooltipPosition);
            
            RegisterCallback<MouseLeaveEvent>(ResetMouseIndicator);
        }

        private void ResetMouseIndicator(MouseLeaveEvent evt)
        {
            m_PositionIndicator.Reset();
            m_Tooltip.Reset();
            MarkDirtyRepaint();
        }

        private void UpdateMouseIndicatorPosition(MouseMoveEvent evt)
        {
            m_PositionIndicator.UpdateMousePosition(evt.localMousePosition);
            MarkDirtyRepaint();
        }

        private void UpdateTooltipPosition(MouseMoveEvent evt)
        {
            m_Tooltip.UpdateMousePosition(evt.localMousePosition);
            MarkDirtyRepaint();
        }

        protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight,
            MeasureMode heightMode)
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

            DrawDataGraph(painter);
            DrawChartLegend(painter, ctx);

            m_PositionIndicator.Draw(painter);
            DrawTooltip(painter, ctx);
        }

        private void InitLayout()
        {
            this.style.minWidth = new StyleLength(m_ChartLayout.MinWidth);
            this.style.minHeight = new StyleLength(m_ChartLayout.MinHeight);
            m_ChartLayout.SetVisualElementDimension(layout.height, layout.width);

            var style = this.style;
            m_ChartLayout.SetPadding(Utils.LengthToFloat(resolvedStyle.paddingTop),
                Utils.LengthToFloat(resolvedStyle.paddingBottom),
                Utils.LengthToFloat(resolvedStyle.paddingLeft),
                Utils.LengthToFloat(resolvedStyle.paddingRight));

            m_ChartLayout.SetBorder(resolvedStyle.borderTopWidth, resolvedStyle.borderBottomWidth,
                resolvedStyle.borderLeftWidth, resolvedStyle.borderRightWidth);
            
            m_ChartLayout.SetXStepLength(Utils.GetMaxDataProviderLength(m_DataProviders));
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

        private void DrawTooltip(Painter2D painter, MeshGenerationContext context)
        {
            m_Tooltip.DrawTooltip(painter, context, m_PositionIndicator.CurrentIndex, m_PositionIndicator.IndicatorXPosition);
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

        private void DrawDataGraph(Painter2D painter)
        {
            m_DataGraph.SetMinAndMax(m_NiceMinY, m_NiceMaxY);
            m_DataGraph.DrawDataGraphs(painter);
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

            m_PositionIndicator.AddDataPointList(m_DataProviders[0].DataPointPositions);

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
            m_PositionIndicator.AddDataPointList(m_DataProviders[1].DataPointPositions);


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
            m_PositionIndicator.AddDataPointList(provider.DataPointPositions);
        }
    }
}