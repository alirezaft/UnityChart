using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityChart.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace UnityChart.Editor
{
    [UxmlElement]
    public partial class LineChart : VisualElement
    {
        private List<DataProvider> m_DataProviders;

        private string m_DataProviderIDs;
        [UxmlAttribute("data-providers")]
        public string DataProviderIDs
        {
            get => m_DataProviderIDs;
            set
            {
                m_DataProviderIDs = value;
                DataProviderRegistry.instance.OnDataProviderRemoved += UnsubscribeFromProvider;
                GetDataProviders();
            }
        }

        private float m_MinY;
        private float m_MaxY;
        private float m_MinX = 0;
        private float m_MaxX;
        private float m_NiceMaxY;
        private float m_NiceMinY;
        private float m_YLabelMargin = 4f;
        private Vector2 m_NoDataLength;

        private float m_FontSize = 10f;
        private List<float> m_XTicks;
        private List<float> m_YTicks;

        private bool m_IsChartInitiated;

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
            // GetDataProviders();
            generateVisualContent += DrawChart;
            m_NoDataLength = Vector2.zero;
        }

        private void GetDataProviders()
        {
            string[] ids = ParseIDs();
            
            m_DataProviders.Clear();

            foreach (var id in ids)
            {
                var provider = DataProviderRegistry.instance.GetDataProvider(id);
                if (provider == null)
                    continue;

                provider.OnDataChanged += MarkDirtyRepaint;
                m_DataProviders.Add(provider);
            }
        }

        private string[] ParseIDs()
        {
            return m_DataProviderIDs.Split(new[] { "," }, StringSplitOptions.None)
                .Select(s => s.Trim())
                .ToArray();
        }

        private void UnsubscribeFromProvider(DataProvider provider)
        {
            provider.OnDataChanged -= MarkDirtyRepaint;
        }

        private void RegisterChartEvents()
        {
            RegisterCallback<MouseMoveEvent>(UpdateMouseIndicatorPosition);
            RegisterCallback<MouseMoveEvent>(UpdateTooltipPosition);

            RegisterCallback<MouseLeaveEvent>(ResetMouseIndicator);

            RegisterCallback<AttachToPanelEvent>(ParseDataProviderAttribute);
            RegisterCallback<DetachFromPanelEvent>(UnsubscribeFromAllDataProviders);
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

        private void ParseDataProviderAttribute(AttachToPanelEvent evt)
        {
        }

        private void UnsubscribeFromAllDataProviders(DetachFromPanelEvent evt)
        {
            foreach (var provider in m_DataProviders)
            {
                provider.OnDataChanged -= MarkDirtyRepaint;
            }
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

        private void DrawChart(MeshGenerationContext ctx)
        {
            GetDataProviders();

            if (m_DataProviders.Count == 0)
            {
                if (m_NoDataLength.Equals(Vector2.zero))
                    m_NoDataLength = Utils.EstimateLabelDimensionInPixels("No data", this, 30);
                ctx.DrawText("No data",
                    new Vector2((layout.width / 2) - (m_NoDataLength.x / 2),
                        (layout.height / 2) - (m_NoDataLength.y / 2)), 30f,
                    new Color(0.65f, 0.65f, 0.65f));
                return;
            }

            if (!m_IsChartInitiated)
            {
                m_IsChartInitiated = true;
                m_ChartLayout = new ChartLayout(m_YLabelMargin, m_FontSize, this);
                m_PositionIndicator = new MousePositionIndicator(m_ChartLayout);

                foreach (var provider in m_DataProviders)
                {
                    m_PositionIndicator.AddDataPointList(provider.DataPointPositions);
                }
                
                m_Axis = new Axis(m_ChartLayout.ChartHeight, layout.width, m_ChartLayout);
                m_DataGraph = new DataGraph(m_ChartLayout, m_Axis, m_DataProviders);
                m_Ticks = new Ticks(m_Axis, m_DataProviders[0].Length, 4f, m_ChartLayout);
                m_Labels = new TickLabel(m_FontSize, m_Axis, m_ChartLayout);
                m_Legend = new ChartLegend(m_ChartLayout, this);
                m_Tooltip = new ChartTooltip(m_ChartLayout, m_DataProviders, this);

                RegisterChartEvents();
                // MarkDirtyRepaint();
            }

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
            m_Tooltip.DrawTooltip(painter, context, m_PositionIndicator.CurrentIndex,
                m_PositionIndicator.IndicatorXPosition);
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
            m_NiceMaxY = length > 1 ? niceScaleY.NiceMax : Mathf.Min(0, m_MinY);
            m_NiceMinY = length > 1 ? niceScaleY.NiceMin : Mathf.Max(0, m_MaxY);

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

        public void AddDataProvider(DataProvider provider)
        {
            m_DataProviders.Add(provider);
            m_PositionIndicator.AddDataPointList(provider.DataPointPositions);
        }
    }
}