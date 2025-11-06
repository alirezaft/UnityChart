using System;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityChart.Editor;
using UnityEngine;

namespace UnityChart.Tests
{
    public class NiceScaleTests
    {

        [TestCase(0f, 0f)]
        [TestCase(10f, 10f)]
        [TestCase(-5f, -5f)]
        public void Constructor_SameMinMax_ShouldExpandRange(float min, float max)
        {
            var scale = new NiceScale(min, max, false);

            Assert.Greater(scale.NiceMax, scale.NiceMin, "Range should expand when min == max");
            Assert.Greater(scale.TickSpacing, 0, "Tick spacing should be positive");
        }

        [TestCase(0f, 10f)]
        [TestCase(-10f, 10f)]
        [TestCase(10f, 1000f)]
        [TestCase(-1000f, -100f)]
        public void Constructor_ValidRange_ProducesReasonableTicks(float min, float max)
        {
            var scale = new NiceScale(min, max, false);
            var ticks = scale.GetTicks();

            Assert.That(ticks.Count, Is.GreaterThan(1));
            Assert.That(scale.NiceMin, Is.LessThan(scale.NiceMax));
            Assert.That(scale.TickSpacing, Is.GreaterThan(0));
        }

        [Test]
        public void AtLeastOne_ShouldForceNiceMinAtLeastOne()
        {
            var scale = new NiceScale(0, 5, true);

            Assert.GreaterOrEqual(scale.NiceMin, 1);
            Assert.Greater(scale.NiceMax, scale.NiceMin);
        }

        [Test]
        public void AtLeastOne_ShouldAdjustIfNiceMinEqualsNiceMax()
        {
            var scale = new NiceScale(1, 1.01f, true);
            Assert.Greater(scale.NiceMax, scale.NiceMin);
        }

        [Test]
        public void SetMinMaxPoints_ShouldRecalculate()
        {
            var scale = new NiceScale(0, 10, false);
            float oldSpacing = scale.TickSpacing;

            scale.SetMinMaxPoints(0, 100);

            Assert.That(scale.TickSpacing, Is.Not.EqualTo(oldSpacing));
            Assert.That(scale.NiceMax, Is.GreaterThan(scale.NiceMin));
        }

        [Test]
        public void SetMinMaxPoints_SameValue_ShouldExpandRange()
        {
            var scale = new NiceScale(5, 5, false);
            scale.SetMinMaxPoints(5, 5);
            Assert.That(scale.NiceMax, Is.GreaterThan(scale.NiceMin));
        }

        [TestCase(5f)]
        [TestCase(20f)]
        public void SetMaxTicks_ShouldChangeTickDensity(float newMaxTicks)
        {
            var scale = new NiceScale(0, 10, false);
            float oldSpacing = scale.TickSpacing;

            scale.SetMaxTicks(newMaxTicks);
            Assert.That(scale.TickSpacing, Is.Not.EqualTo(oldSpacing));
        }

        [Test]
        public void SetMaxTicks_ExtremelySmall_ShouldStillWork()
        {
            var scale = new NiceScale(0, 10, false);
            Assert.DoesNotThrow(() => scale.SetMaxTicks(0.1f));
        }

        [TestCase(0f)]
        [TestCase(-10f)]
        public void RangeLessThanOrEqualZero_ShouldReturnZeroTickSpacing(float range)
        {
            var scale = new NiceScale(range, range, false);
            Assert.That(scale.TickSpacing, Is.GreaterThan(0)); // recovered by Calculate()
        }

        [Test]
        public void GetTicks_ShouldReturnSequentialValues()
        {
            var scale = new NiceScale(0, 10, false);
            var ticks = scale.GetTicks();

            for (int i = 1; i < ticks.Count; i++)
            {
                Assert.That(ticks[i], Is.GreaterThan(ticks[i - 1]));
            }
        }

        [Test]
        public void GetTicks_ShouldRoundToSixDecimals()
        {
            var scale = new NiceScale(0, 1.000001f, false);
            var ticks = scale.GetTicks();

            foreach (float tick in ticks)
            {
                string s = tick.ToString("F6");
                Assert.That(float.Parse(s), Is.EqualTo(tick).Within(1e-6));
            }
        }

        [Test]
        public void GetTicks_ShouldIncludeBothNiceMinAndNiceMax()
        {
            var scale = new NiceScale(0, 10, false);
            var ticks = scale.GetTicks();

            Assert.That(ticks[0], Is.EqualTo(scale.NiceMin).Within(1e-6));
            Assert.That(ticks[^1], Is.EqualTo(scale.NiceMax).Within(1e-6));
        }

        [Test]
        public void GetTicks_ShouldHandleNegativeRanges()
        {
            var scale = new NiceScale(-10, -5, false);
            var ticks = scale.GetTicks();

            Assert.That(ticks.Count, Is.GreaterThan(0));
            Assert.That(ticks[0], Is.EqualTo(scale.NiceMin).Within(1e-6));
        }

        [Test]
        public void GetTicks_ShouldHandleInvertedInput()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var scale = new NiceScale(10, 0, false);
            });
        }
        
        [Test]
        public void LargeRange_ShouldWork()
        {
            var scale = new NiceScale(-1_000_000, 1_000_000, false);
            var ticks = scale.GetTicks();

            Assert.That(ticks.Count, Is.LessThan(50));
            Assert.That(scale.TickSpacing, Is.GreaterThan(0));
        }

        [Test]
        public void TinyRange_ShouldWork()
        {
            var scale = new NiceScale(0.0001f, 0.0002f, false);
            var ticks = scale.GetTicks();
            Assert.That(ticks.Count, Is.GreaterThan(0));
        }

        [Test]
        public void NegativeToPositiveRange_ShouldCrossZero()
        {
            var scale = new NiceScale(-5, 5, false);
            var ticks = scale.GetTicks();

            Assert.That(ticks.Exists(t => Mathf.Approximately(t, 0f)));
        }

        [Test]
        public void TickSpacing_ShouldIncrease_WhenRangeIncreases()
        {
            var small = new NiceScale(0, 10, false);
            var large = new NiceScale(0, 1000, false);

            Assert.That(large.TickSpacing, Is.GreaterThan(small.TickSpacing));
        }
    }
}

