using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityChart
{
    public class NiceScale
    {
        private float minPoint;
        private float maxPoint;
        private float maxTicks = 10f;
        private float tickSpacing;
        private float range;
        private float niceMin;
        private float niceMax;
        private bool atLeastOne;

        /// <summary>
        /// Creates a new NiceScale for generating axis values.
        /// </summary>
        public NiceScale(float min, float max, bool atLeastOne)
        {
            minPoint = min;
            maxPoint = max;
            this.atLeastOne = atLeastOne;
            Calculate();
        }

        /// <summary>
        /// Calculate tick spacing, nice minimum, and nice maximum.
        /// </summary>
        private void Calculate()
        {
            range = NiceNum(Mathf.Abs(maxPoint - minPoint), false, false); // ensure positive range
            tickSpacing = NiceNum(range / (maxTicks - 1f), true, atLeastOne);

            if (atLeastOne)
            {
                // Clamp min at 1 if requested
                niceMin = Mathf.Max(1, Mathf.Floor(minPoint / tickSpacing) * tickSpacing);
                niceMax = Mathf.Ceil(maxPoint / tickSpacing) * tickSpacing;
                if (niceMax == niceMin) niceMax += 1; // ensure at least one tick
            }
            else
            {
                niceMin = Mathf.Floor(minPoint / tickSpacing) * tickSpacing;
                niceMax = Mathf.Ceil(maxPoint / tickSpacing) * tickSpacing;
            }
        }

        /// <summary>
        /// Produces a "nice" rounded number for the given range.
        /// </summary>
        private float NiceNum(float range, bool round, bool atLeastOne)
        {
            if (range == 0) return 0;

            float exponent = Mathf.Floor(Mathf.Log10(range));
            float fraction = range / Mathf.Pow(10f, exponent);
            float niceFraction;

            if (round)
            {
                if (fraction < 1.5f)
                    niceFraction = 1f;
                else if (fraction < 3f)
                    niceFraction = 2f;
                else if (fraction < 7f)
                    niceFraction = 5f;
                else
                    niceFraction = 10f;
            }
            else
            {
                if (fraction <= 1f)
                    niceFraction = 1f;
                else if (fraction <= 2f)
                    niceFraction = 2f;
                else if (fraction <= 5f)
                    niceFraction = 5f;
                else
                    niceFraction = 10f;
            }

            var ans = niceFraction * Mathf.Pow(10f, exponent);
            return atLeastOne ? Mathf.Max(Mathf.Floor(ans), 1) : ans;
        }

        public void SetMinMaxPoints(float min, float max)
        {
            minPoint = min;
            maxPoint = max;
            Calculate();
        }

        public void SetMaxTicks(float maxTicks)
        {
            this.maxTicks = maxTicks;
            Calculate();
        }

        public List<float> GetTicks()
        {
            List<float> result = new List<float>();

            int numberOfNumbers = Mathf.RoundToInt((niceMax - niceMin) / tickSpacing);

            for (int i = 0; i <= numberOfNumbers; i++)
            {
                result.Add(niceMin + (i * tickSpacing));
            }

            Debug.Log($"Ticks: [{string.Join(", ", result)}], Spacing: {tickSpacing}");
            return result;
        }

        // Public properties
        public float TickSpacing => tickSpacing;
        public float NiceMin => niceMin;
        public float NiceMax => niceMax;
    }
}