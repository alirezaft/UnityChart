using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityChart.Editor;
using UnityEngine.UIElements;

namespace UnityChart.Tests
{
    public class ChartLayoutTests
    {
        private VisualElement m_DummyChart;

        [SetUp]
        public void Init()
        {
            m_DummyChart = new VisualElement();
            var newLayout = new Rect(0, 0, 200, 400);
        }

        [TestCase(-3, 4, ExpectedResult = "Label margin can't be negative")]
        [TestCase(3, -4, ExpectedResult = "Font size should be a positive number")]
        [TestCase(-3, -4, ExpectedResult = "Label margin can't be negative")]
        [TestCase(0, -4, ExpectedResult = "Font size should be a positive number")]
        [TestCase(3, 0, ExpectedResult = "Font size should be a positive number")]
        [Test]
        public string ChartLayout_Constructor_InvalidArguments(int labelMargin, int fontSize)
        {
            var exception = Assert.Throws<InvalidLayoutException>(() =>
            {
                var latyout = new ChartLayout(labelMargin, fontSize);
            });

            return exception.Message;
        }

        [TestCase(1, 2)]
        [TestCase(0, 2)]
        [Test]
        public void ChartLayout_Constructor_ValidArguments(int labelMargin, int fontSize)
        {
            var layout = new ChartLayout(labelMargin, fontSize);
            var fontSizeField =
                typeof(ChartLayout).GetField("m_FontSize", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(layout.LabelMargin, labelMargin);
            Assert.AreEqual((float)fontSizeField.GetValue(layout), fontSize);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(10, 0, 0, 0)]
        [TestCase(0, 10, 0, 0)]
        [TestCase(0, 0, 10, 0)]
        [TestCase(0, 0, 0, 10)]
        [TestCase(10, 0, 0, 10)]
        [TestCase(0, 10, 0, 10)]
        [TestCase(0, 0, 10, 10)]
        [TestCase(10, 10, 0, 0)]
        [TestCase(10, 0, 10, 0)]
        [TestCase(0, 10, 10, 0)]
        [TestCase(10, 10, 10, 10)]
        [Test]
        public void ChartLayout_SetPadding_ValidArguments(float left, float right, float upper, float bottom)
        {
            var layout = new ChartLayout(3, 4);

            layout.SetVisualElementDimension(400, 200);
            layout.SetPadding(upper, bottom, left, right);
            
            Assert.AreEqual(left, layout.PaddingLeft);
            Assert.AreEqual(right, layout.PaddingRight);
            Assert.AreEqual(bottom, layout.PaddingBottom);
            Assert.AreEqual(upper, layout.PaddingUpper);
        }

        [TestCase(250, 0, 0, 0)]
        [TestCase(100, 150, 0, 0)]
        [TestCase(0, 0, 450, 0)]
        [TestCase(0, 0, 250, 155)]
        [Test]
        public void ChartLayout_SetPadding_InvalidArguments(float left, float right, float upper, float bottom)
        {
            var layout = new ChartLayout(3, 4);

            layout.SetVisualElementDimension(400, 200);
            Assert.Throws<InvalidLayoutException>(() => layout.SetPadding(upper, bottom, left, right));

        }

        [TestCase(-3, 0, 0, 0, ExpectedResult = "Padding can not be negative.")]
        [TestCase(0, -3, 0, 0, ExpectedResult = "Padding can not be negative.")]
        [TestCase(0, 0, -3, 0, ExpectedResult = "Padding can not be negative.")]
        [TestCase(0, 0, 0, -3, ExpectedResult = "Padding can not be negative.")]
        [Test]
        public string ChartLayout_SetPadding_NegativeArguments(float left, float right, float upper, float bottom)
        {
            var layout = new ChartLayout(3, 4);

            layout.SetVisualElementDimension(400, 200);
            var ex = Assert.Throws<ArgumentException>(() => layout.SetPadding(upper, bottom, left, right));

            return ex.Message;
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(10, 0, 0, 0)]
        [TestCase(0, 10, 0, 0)]
        [TestCase(0, 0, 10, 0)]
        [TestCase(0, 0, 0, 10)]
        [TestCase(10, 0, 0, 10)]
        [TestCase(0, 10, 0, 10)]
        [TestCase(0, 0, 10, 10)]
        [TestCase(10, 10, 0, 0)]
        [TestCase(10, 0, 10, 0)]
        [TestCase(0, 10, 10, 0)]
        [TestCase(10, 10, 10, 10)]
        [Test]
        public void ChartLayout_SetBorder_ValidArguments(float left, float right, float upper, float bottom)
        {
            var layout = new ChartLayout(3, 4);

            layout.SetVisualElementDimension(400, 200);
            layout.SetBorder(upper, bottom, left, right);
            
            Assert.AreEqual(left, layout.BorderLeft);
            Assert.AreEqual(right, layout.BorderRight);
            Assert.AreEqual(bottom, layout.BorderBottom);
            Assert.AreEqual(upper, layout.BorderTop);
        }

        [TestCase(250, 0, 0, 0)]
        [TestCase(100, 150, 0, 0)]
        [TestCase(0, 0, 450, 0)]
        [TestCase(0, 0, 250, 155)]
        [Test]
        public void ChartLayout_SetBorder_InvalidArguments(float left, float right, float upper, float bottom)
        {
            var layout = new ChartLayout(3, 4);

            layout.SetVisualElementDimension(400, 200);
            Assert.Throws<InvalidLayoutException>(() => layout.SetBorder(upper, bottom, left, right));

        }

        [TestCase(-3, 0, 0, 0, ExpectedResult = "Border can not be negative.")]
        [TestCase(0, -3, 0, 0, ExpectedResult = "Border can not be negative.")]
        [TestCase(0, 0, -3, 0, ExpectedResult = "Border can not be negative.")]
        [TestCase(0, 0, 0, -3, ExpectedResult = "Border can not be negative.")]
        [Test]
        public string ChartLayout_SetBorder_NegativeArguments(float left, float right, float upper, float bottom)
        {
            var layout = new ChartLayout(3, 4);

            layout.SetVisualElementDimension(400, 200);
            var ex = Assert.Throws<ArgumentException>(() => layout.SetBorder(upper, bottom, left, right));

            return ex.Message;
        }

        [TestCase(10, 10, 10, 10, 10, 10, 10, 10, true)]
        public void ChartLayout_BorderAndPadding_Valid(
            float leftPadding, float rightPadding,
            float topPadding, float bottomPadding,
            float leftBorder, float rightBorder,
            float topBorder, float bottomBorder,
            bool expectedValid)
        {
            var layout = new ChartLayout(3, 4);

            Assert.DoesNotThrow(() =>
            {
                layout.SetVisualElementDimension(400, 200);
                layout.SetPadding(leftPadding, rightPadding, topPadding, bottomPadding);
                layout.SetBorder(leftBorder, rightBorder, topBorder, bottomBorder);
            });
        }

        [TestCase(110, 100, 0, 0, 0, 0, 0, 0)]
        [TestCase(100, 100, 0, 0, 1, 0, 0, 0)]
        [TestCase(0, 0, 300, 100, 0, 0, 0, 0)]
        [TestCase(0, 0, 300, 100, 0, 0, 5, 5)]
        public void ChartLayout_BorderAndPadding_Invalid(
            float leftPadding, float rightPadding,
            float topPadding, float bottomPadding,
            float leftBorder, float rightBorder,
            float topBorder, float bottomBorder)
        {
            var layout = new ChartLayout(3, 4);

            Assert.Throws<InvalidLayoutException>(() =>
            {
                layout.SetVisualElementDimension(400, 200);
                layout.SetPadding(topPadding, bottomPadding, leftPadding, rightPadding);
                layout.SetBorder(topBorder, bottomBorder, leftBorder, rightBorder);
            });
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 5, 5, 5, 5, 5, 5, 5)]
        [TestCase(5, 0, 5, 0, 5, 0, 5, 0)]
        [TestCase(50, 50, 100, 100, 50, 49, 100, 99)]
        public void ChartLayout_Boundary_Valid(
            float leftPadding, float rightPadding,
            float topPadding, float bottomPadding,
            float leftBorder, float rightBorder,
            float topBorder, float bottomBorder)
        {
            var layout = new ChartLayout(4, 3);

            var height = 400;
            var width = 200;
            layout.SetVisualElementDimension(height, width);

            layout.SetPadding(topPadding, bottomPadding, leftPadding, rightPadding);
            layout.SetBorder(topBorder, bottomBorder, leftBorder, rightBorder);

            var xStart = leftPadding + leftBorder;
            var xEnd = width - rightPadding - rightBorder;
            var yStart = topPadding + topBorder;
            var yEnd = height - bottomBorder - bottomPadding;
            var allowedHeight = yEnd - yStart;
            var allowedWidth = xEnd - xStart;

            Assert.AreEqual(xStart, layout.XStart);
            Assert.AreEqual(xEnd, layout.XEnd);
            Assert.AreEqual(yStart, layout.YStart);
            Assert.AreEqual(yEnd, layout.YEnd);
            Assert.AreEqual(allowedHeight, layout.AllowedHeight);
            Assert.AreEqual(allowedWidth, layout.AllowedWidth);
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 5, 5, 5, 5, 5, 5, 5)]
        [TestCase(5, 0, 5, 0, 5, 0, 5, 0)]
        [TestCase(50, 50, 100, 100, 50, 49, 100, 99)]
        public void ChartLayout_GetChartDataArea_Valid(
            float leftPadding, float rightPadding,
            float topPadding, float bottomPadding,
            float leftBorder, float rightBorder,
            float topBorder, float bottomBorder)
        {
            var layout = new ChartLayout(4, 3);

            var width = 400;
            var height = 200;

            var rectX = layout.XStart + layout.WidthOffset;
            var rectY = layout.YStart;
            var rectWidth = layout.XEnd - rectX;
            var rectHeight = layout.ChartHeight;

            var dataAreaRect = layout.GetChartDataArea();

            Assert.AreEqual(rectX, dataAreaRect.x);
            Assert.AreEqual(rectY, dataAreaRect.y);
            Assert.AreEqual(rectWidth, dataAreaRect.width);
            Assert.AreEqual(rectHeight, dataAreaRect.height);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(1000000)]
        [TestCase(99)]
        [TestCase(534546848)]
        public void ChartLayout_SetXStepLength_Valid(int datasetLength)
        {
            var layout = new ChartLayout(4, 3);

            layout.SetVisualElementDimension(200, 400);

            if (datasetLength < 1)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => layout.SetXStepLength(datasetLength));
                return;
            }

            layout.SetXStepLength(datasetLength);
            var expectedStep = layout.GetChartDataArea().width / datasetLength;


            Assert.AreEqual(expectedStep, layout.XStepLength);
        }
    }
}