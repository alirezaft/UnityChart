using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityChart{
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
            range = NiceNum(maxPoint - minPoint, false, false);
            tickSpacing = NiceNum(range / (maxTicks - 1f), true, atLeastOne);
            niceMin = Mathf.Floor(minPoint / tickSpacing) * tickSpacing;
            niceMax = Mathf.Ceil(maxPoint / tickSpacing) * tickSpacing;
        }

        /// <summary>
        /// Produces a "nice" rounded number for the given range.
        /// </summary>
        private float NiceNum(float range, bool round, bool atLeastOne)
        {
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

            StringBuilder sb = new StringBuilder();
            sb.Append("Ticks: [");
            for (float val = NiceMin; val <= NiceMax - TickSpacing; val += TickSpacing)
            {
                result.Add(val);
                sb.Append($"{val}, ");
            }

            sb.Append("]");
            Debug.Log(sb.ToString());
            return result;
        }

        // Public properties to use outside
        public float TickSpacing => tickSpacing;
        public float NiceMin => niceMin;
        public float NiceMax => niceMax;
    }
}
