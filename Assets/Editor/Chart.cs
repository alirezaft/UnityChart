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
    public partial class Chart : VisualElement
    {
        private List<DataProvider> m_DataProviders;

        private float m_MinY;
        private float m_MaxY;
        private float m_MinX = 0;
        private float m_MaxX;

        private float m_ZeroOnYAxisPosition;

        private float m_ArrowSideLength = 10f;
        private float m_ArrowHeadAngle = 30f;
        private float m_TickLength = 6f;

        public Chart()
        {
            generateVisualContent += UpdateWithOldDataset;
            m_DataProviders = new List<DataProvider>();

            RepopulateDataset();
        }

        private void CalculateMinAndMaxValues()
        {
            CalculateMaxY();
            CalculateMinY();
            CalculateMaxX();
        }

        private void CalculateMaxY()
        {
            var answer = float.NegativeInfinity;
            foreach (var provider in m_DataProviders)
            {
                if (provider.MaxValue > answer)
                    answer = provider.MaxValue;
            }

            m_MaxY = answer;
        }

        private void CalculateMinY()
        {
            var answer = float.PositiveInfinity;
            foreach (var provider in m_DataProviders)
            {
                if (provider.MinValue < answer)
                    answer = provider.MinValue;
            }

            m_MinY = answer;
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
            var chartHeight = layout.height;
            var chartWidth = layout.width;

            var painter = ctx.painter2D;

            CalculateMinAndMaxValues();
            DrawChartAxis(painter);
            DrawDataGraphs(painter);
            DrawTicks(painter);
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
            painter.MoveTo(new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
                0));
            painter.LineTo(new Vector2(-Mathf.Sin(270 - ((m_ArrowHeadAngle / 2) * Mathf.Deg2Rad)) * m_ArrowSideLength,
                layout.height));
        }

        private void DrawHorizontalAxisLine(Painter2D painter)
        {
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
                var xSteps = layout.width / provider.Length;

                painter.BeginPath();
                painter.strokeColor = provider.Color;
                painter.fillColor = new Color(provider.Color.r, provider.Color.g, provider.Color.b, 0.3f);
                painter.MoveTo(new Vector2(0, m_ZeroOnYAxisPosition));
                var currPos = new Vector2(0, m_ZeroOnYAxisPosition);
                var YPos = FindValueOnChartYAxis(provider.Dataset[0],
                    AreAllElementsPositive() ? 0 : provider.MinValue,
                    AreAllElementsNegative() ? 0 : provider.MaxValue, 0, layout.height);

                Debug.Log($"0Pos: {m_ZeroOnYAxisPosition}, DataPos: {YPos}");

                painter.LineTo(new Vector2(0, YPos));
                currPos = new Vector2(0, YPos);
                var dataset = provider.Dataset;

                for (int i = 1; i < dataset.Count; i++)
                {
                    var dataPointY = FindValueOnChartYAxis(dataset[i], AreAllElementsPositive() ? 0 : provider.MinValue,
                        AreAllElementsNegative() ? 0 : provider.MaxValue,
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
        }

        private Vector2 FindIntersectionWithXAxis(Vector2 point1, Vector2 point2)
        {
            var slope = (point1.y - point2.y) / (point1.x - point2.x);

            var yIntercept = new Vector2(point1.y - (slope * point1.x), m_ZeroOnYAxisPosition);

            return new Vector2((m_ZeroOnYAxisPosition - yIntercept.x) / slope, m_ZeroOnYAxisPosition);
        }

        private void DrawTicks(Painter2D painter)
        {
            var length = m_DataProviders[0].Length;
            var numOfDigits = Utils.GetNumberOfDigits(length);

            var pixelsForLargestLabel = Utils.EstimateLabelLengthInPixels(numOfDigits);
            Debug.Log($"numPixels: {pixelsForLargestLabel}");
            var maxTicksPossible = layout.width / pixelsForLargestLabel;

            var rangeY = Utils.FindMaxAmongAllDataProviders(m_DataProviders) -
                         Utils.FindMinAmongAllDataProviders(m_DataProviders);
            var stepSizeX = Utils.GetNiceStep(length, (int)maxTicksPossible);
            Debug.Log($"maxticks: {maxTicksPossible}, range: {length}, stepsize: {stepSizeX}");

            var numberOfTicksX = length / stepSizeX;

            PlaceTicksOnXAxis((int)numberOfTicksX, painter);
        }

        private void PlaceTicksOnXAxis(int numberOfTicks, Painter2D painter)
        {
            Debug.Log(numberOfTicks);
            var tickDistance = layout.width / numberOfTicks;
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

            Debug.Log($"Chartval: {t} for value: {value}");

            return Mathf.Abs(t);
            // return destinationMax - t * (+destinationMax - destinationMin);
        }

        public void RepopulateDataset()
        {
            m_DataProviders.Clear();
            m_DataProviders.Add(new DataProvider(Color.green, "Test"));
            m_DataProviders[0].Dataset = new List<float>() { 2, -1, 3 };
            // m_DataProviders.Add(new DataProvider(Color.red, "Test2"));
            // m_DataProviders[0].AddDataPoint(0);
            // m_DataProviders[0].AddDataPoint(1);
            // m_DataProviders[0].AddDataPoint(3);
            // m_DataProviders[0].AddDataPoint(-2);

            // m_DataProviders[0].AddDataPoint(-3);
            StringBuilder builder = new StringBuilder();
            builder.Append("Dataset 1: [");
            foreach (var f in m_DataProviders[0].Dataset)
            {
                builder.Append(f + ", ");
            }

            // for (int i = 0; i < 3; i++)
            // {
            //     m_DataProviders[0].AddDataPoint(Random.Range(-2, 2) * 5);
            //     builder.Append(m_DataProviders[0].Dataset[i] + ", ");
            // }

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