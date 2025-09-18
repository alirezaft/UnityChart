using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
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

        private float m_ZeroOnYAxisPosition;

        private float m_ArrowSideLength = 10f;
        private float m_ArrowHeadAngle = 30f;
        private float m_TickLength = 6f;
        private float m_FontSize = 10f;

        public LineChart()
        {
            generateVisualContent += UpdateWithOldDataset;
            m_DataProviders = new List<DataProvider>();

            RepopulateDataset();
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
            var painter = ctx.painter2D;

            CalculateMinAndMaxValues();
            DrawChartAxis(painter);
            DrawTicks(painter, ctx);
            DrawDataGraphs(painter);
        }

        private void DrawChartAxis(Painter2D painter)
        {
            painter.BeginPath();
            painter.lineWidth = 2f;

            DrawAxisLines(painter);
            // DrawAxisArrows(painter);

            painter.MoveTo(Vector2.zero);


            painter.Stroke();
            painter.ClosePath();
        }

        private void DrawAxisLines(Painter2D painter)
        {
            painter.strokeColor = Color.white;

            DrawVerticalAxisLine(painter);
            DrawHorizontalAxisLine(painter);

            painter.Stroke();
            painter.ClosePath();
        }

        private void DrawVerticalAxisLine(Painter2D painter)
        {
            var originalWidth = painter.lineWidth;
            painter.lineWidth = 1f;

            painter.MoveTo(new Vector2(0,
                0));
            painter.LineTo(new Vector2(0,
                layout.height));

            painter.lineWidth = originalWidth;
        }

        private void DrawHorizontalAxisLine(Painter2D painter)
        {
            var originalWidth = painter.lineWidth;
            painter.lineWidth = 1f;

            m_ZeroOnYAxisPosition = (m_MaxY / (Mathf.Abs(m_MinY) + m_MaxY)) * layout.height;

            if (AreAllElementsNegative())
            {
                m_ZeroOnYAxisPosition = 0f;
            }
            else if (AreAllElementsPositive())
            {
                m_ZeroOnYAxisPosition = layout.height;
            }

            painter.MoveTo(new Vector2(0, m_ZeroOnYAxisPosition));
            painter.LineTo(new Vector2(layout.width, m_ZeroOnYAxisPosition));
            painter.lineWidth = originalWidth;
        }

        private bool AreAllElementsNegative()
        {
            var answer = true;

            foreach (var provider in m_DataProviders)
            {
                answer = answer & provider.Dataset.TrueForAll(item => item < 0);
            }

            return answer;
        }

        private bool AreAllElementsPositive()
        {
            var answer = true;

            foreach (var provider in m_DataProviders)
            {
                answer = answer & provider.Dataset.TrueForAll(item => item > 0);
            }

            return answer;
        }

        private void DrawAxisArrows(Painter2D painter)
        {
            DrawHorizontalAxisArrow(painter);
            DrawVerticalAxisArrow(painter);
        }

        private void DrawHorizontalAxisArrow(Painter2D painter)
        {
            var xAxisEnd = new Vector2(layout.width, m_ZeroOnYAxisPosition);
            painter.MoveTo(xAxisEnd);

            painter.LineTo(new Vector2(
                Mathf.Cos((180 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.x,
                Mathf.Sin((180 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.y));

            painter.MoveTo(xAxisEnd);
            painter.LineTo(new Vector2(
                Mathf.Cos((180 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.x,
                Mathf.Sin((180 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + xAxisEnd.y));

            painter.Stroke();
            painter.ClosePath();
        }

        private void DrawVerticalAxisArrow(Painter2D painter)
        {
            var yAxisEnd = new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
                0);
            painter.MoveTo(yAxisEnd);

            painter.LineTo(new Vector2(
                -Mathf.Cos((270 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.x,
                -Mathf.Sin((270 - (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.y));

            painter.MoveTo(yAxisEnd);
            painter.LineTo(new Vector2(
                -Mathf.Cos((270 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.x,
                -Mathf.Sin((270 + (m_ArrowHeadAngle / 2)) * Mathf.Deg2Rad) * m_ArrowSideLength + yAxisEnd.y));

            painter.Stroke();
            painter.ClosePath();
        }

        private void DrawDataGraphs(Painter2D painter)
        {
            foreach (var provider in m_DataProviders)
            {
                var latestPointOnXAxis = 0f;
                var xSteps = layout.width / (provider.Length - 1);
                Debug.Log($"data steps x2 {xSteps}");

                painter.BeginPath();
                painter.lineWidth = 1.5f;
                painter.strokeColor = provider.Color;
                painter.fillColor = new Color(provider.Color.r, provider.Color.g, provider.Color.b, 0.3f);
                painter.MoveTo(new Vector2(0, m_ZeroOnYAxisPosition));
                var currPos = new Vector2(0, m_ZeroOnYAxisPosition);
                var YPos = FindValueOnChartYAxis(provider.Dataset[0],
                    m_NiceMinY,
                    m_NiceMaxY, 0, layout.height);


                painter.LineTo(new Vector2(0, YPos));
                currPos = new Vector2(0, YPos);
                var dataset = provider.Dataset;

                for (int i = 1; i < dataset.Count; i++)
                {
                    var dataPointY = FindValueOnChartYAxis(dataset[i], AreAllElementsPositive() ? 0 : m_NiceMinY,
                        AreAllElementsNegative() ? 0 : m_NiceMaxY,
                        0, layout.height);

                    if (Utils.DoValuesHaveDifferentSigns(dataset[i], dataset[i - 1]))
                    {
                        var nextPoint = new Vector2(currPos.x + xSteps, dataPointY);
                        var intersectionPoint = FindIntersectionWithXAxis(currPos, nextPoint);

                        painter.LineTo(intersectionPoint);
                        painter.ClosePath();
                        painter.Stroke();
                        painter.Fill();

                        painter.BeginPath();
                        painter.MoveTo(intersectionPoint);
                        painter.LineTo(new Vector2(currPos.x + xSteps, dataPointY));

                        currPos = new Vector2(currPos.x + xSteps, dataPointY);
                        latestPointOnXAxis = intersectionPoint.y;
                    }
                    else
                    {
                        painter.LineTo(new Vector2(currPos.x + xSteps, dataPointY));
                        currPos = new Vector2(currPos.x + xSteps, dataPointY);
                    }
                }

                painter.LineTo(new Vector2(currPos.x, m_ZeroOnYAxisPosition));
                painter.ClosePath();
                painter.Stroke();
                painter.Fill();
            }

            painter.lineWidth = 2;
        }

        private Vector2 FindIntersectionWithXAxis(Vector2 point1, Vector2 point2)
        {
            var slope = (point1.y - point2.y) / (point1.x - point2.x);

            var yIntercept = new Vector2(point1.y - (slope * point1.x), m_ZeroOnYAxisPosition);

            return new Vector2((m_ZeroOnYAxisPosition - yIntercept.x) / slope, m_ZeroOnYAxisPosition);
        }

        private void DrawTicks(Painter2D painter, MeshGenerationContext context)
        {
            var length = m_DataProviders[0].Length;
            var numOfDigits = Utils.GetNumberOfDigits(length);

            var niceScaleX = new NiceScale(1, length, true);
            var niceScaleY = new NiceScale(m_MinY, m_MaxY, false);

            var xTicks = niceScaleX.GetTicks();
            var yTicks = niceScaleY.GetTicks();

            var pixelsForLargestLabel = Utils.EstimateLabelLengthInPixels(numOfDigits, m_FontSize);
            var maxXTicksPossible = layout.width / pixelsForLargestLabel;
            var maxYTicksPossible = layout.height / (m_FontSize * 2);
            niceScaleY.SetMaxTicks(10);
            // niceScaleX.SetMaxTicks(maxTicksPossible);
            m_NiceMaxY = niceScaleY.NiceMax;
            m_NiceMinY = niceScaleY.NiceMin;
            Debug.Log($"Min: {m_NiceMinY}, Max: {m_NiceMaxY}");
            
            var rangeY = Utils.FindMaxAmongAllDataProviders(m_DataProviders) -
                         Utils.FindMinAmongAllDataProviders(m_DataProviders);
            var stepSizeX = length / niceScaleX.TickSpacing;

            var numberOfTicksX = length / stepSizeX;
            Debug.Log($"Tick Params: Length: {length}, step size: {stepSizeX}");

            // PlaceTicksOnXAxis(xTicks, painter);
            // PlaceXAxisTickLabels(xTicks, context);
            PlaceTicksOnYAxis(yTicks, painter);
            PlaceYAxisTickLabels(yTicks, context);
        }

        private void PlaceTicksOnXAxis(List<float> ticksList, Painter2D painter)
        {
            var numberOfTicks = ticksList.Count - 1;

            var dataSteps = layout.width / (m_DataProviders[0].Length - 1);
            var dataToTickRatio = ((float)m_DataProviders[0].Length - 1) / (ticksList.Count - 1);


            Debug.Log("Num of Ticks: " + numberOfTicks);
            var tickDistance = dataSteps * dataToTickRatio;
            Debug.Log($"Tick distance: {tickDistance}");
            var painterMovementVector = new Vector2(tickDistance, -m_TickLength);
            var tickLengthVector = new Vector2(0, m_TickLength);
            var currPosition = new Vector2(0, m_ZeroOnYAxisPosition) + tickLengthVector;

            painter.MoveTo(new Vector2(0, m_ZeroOnYAxisPosition) + tickLengthVector);
            painter.strokeColor = Color.white;

            for (int i = 0; i < numberOfTicks; i++)
            {
                Debug.Log(currPosition);
                painter.BeginPath();
                painter.MoveTo(currPosition + painterMovementVector);
                currPosition += painterMovementVector;
                painter.LineTo(currPosition + (tickLengthVector / 2));
                painter.LineTo(currPosition - tickLengthVector / 2);
                currPosition += tickLengthVector;
                painter.Stroke();
                painter.ClosePath();
            }
        }

        private void PlaceXAxisTickLabels(List<float> ticks, MeshGenerationContext context)
        {
            var numberOfLabels = ticks.Count;
            var dataSteps = layout.width / (m_DataProviders[0].Length - 1);
            var dataToTickRatio = ((float)m_DataProviders[0].Length - 1) / (ticks.Count - 1);
            var labelPositionStep = dataSteps * dataToTickRatio;

            for (int i = 0; i < ticks.Count; i++)
            {
                var labelContent = (int)ticks[i];
                var labelPosition =
                    new Vector2(i * labelPositionStep,
                        m_ZeroOnYAxisPosition);
                var labelAdjustmentVector = new Vector2(-(((int)Mathf.Log10(labelContent) + 1) * m_FontSize) / 2, 4);

                labelPosition += i == ticks.Count - 1
                    ? new Vector2(labelAdjustmentVector.x * 2, labelAdjustmentVector.y)
                    : labelAdjustmentVector;

                context.DrawText(labelContent.ToString(), labelPosition, m_FontSize, Color.white);
            }
        }

        private void PlaceTicksOnYAxis(List<float> ticks, Painter2D painter)
        {
            var tickDistance = layout.height / (ticks.Count - 1);
            var tickVector = new Vector2(m_TickLength, 0);
            var painterStepVector = new Vector2(-m_TickLength, -tickDistance);

            var currPos = new Vector2(0, layout.height);
            painter.MoveTo(currPos);

            for (int i = 0; i < ticks.Count; i++)
            {
                painter.BeginPath();
                painter.strokeColor = Color.white;
                painter.MoveTo(currPos + painterStepVector);
                currPos += painterStepVector;
                painter.LineTo(currPos + tickVector * 1.5f);
                currPos += tickVector;
                painter.Stroke();
                painter.ClosePath();
            }
        }

        private void PlaceYAxisTickLabels(List<float> ticks, MeshGenerationContext context)
        {
            var labelDistance = layout.height / (ticks.Count - 1);
            var painterMovementVector = new Vector2(0, -labelDistance);
            var currPos = new Vector2(0, layout.height);

            for (int i = 0; i < ticks.Count; i++)
            {
                context.DrawText(ticks[i].ToString(), currPos + new Vector2(0, -1.1f * m_FontSize / 2), m_FontSize,
                    Color.white);
                currPos += painterMovementVector;
            }
        }

        private float FindValueOnChartYAxis(float value, float sourceMin, float sourceMax, float destinationMin,
            float destinationMax)
        {
            // if (AreAllElementsNegative())
            // {
            //     (sourceMax, sourceMin) = (sourceMin, sourceMax);
            // }

            var t = value - sourceMin;
            t = t / (sourceMax - sourceMin) * (destinationMax - destinationMin);

            var yMidpoint = (destinationMax + destinationMin) / 2;
            // yMidpoint = t > yMidpoint ? -yMidpoint : yMidpoint;
            t = t < yMidpoint ? (2 * (yMidpoint - t)) + t : t - (2 * (t - yMidpoint));


            return Mathf.Abs(t);
            // return destinationMax - t * (+destinationMax - destinationMin);
        }

        public void RepopulateDataset()
        {
            m_DataProviders.Clear();
            m_DataProviders.Add(new DataProvider(Color.green, "Test"));
            // m_DataProviders[0].Dataset = new List<float>() { 1, 0, 1, 2, -3, 5 };
            // m_DataProviders.Add(new DataProvider(Color.red, "Test2"));
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
                m_DataProviders[0].AddDataPoint((Random.value * 1003) - 400);
                builder.Append(m_DataProviders[0].Dataset[i] + ", ");
            }

            builder.Append("]");


            // for (int i = 0; i < 3; i++)
            // {
            //     m_DataProviders[1].AddDataPoint(Random.value * -5);
            //     builder.Append(m_DataProviders[1].Dataset[i] + ", ");
            // }
            //
            // builder.Append("]");
            Debug.Log(builder.ToString());

            MarkDirtyRepaint();
        }

        public void AddDataProvider(DataProvider provider)
        {
            m_DataProviders.Add(provider);
        }
    }
}