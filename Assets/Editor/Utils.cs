using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChart
{
    public class Utils
    {
        public static int GetNumberOfDigits(int n)
        {
            
            return (int)Math.Floor(Math.Log10(n) + 1);;
        }

        public static float EstimateLabelLengthInPixels(int length, float fontSize)
        {
            return Mathf.Round(4.25f * length);
        }

        public static float GetNiceStep(float range, int maxTicks)
        {
            if (range <= 0f || maxTicks <= 0) 
                return 1f;

            var roughStep = range / maxTicks;

            var magnitude = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(roughStep)));

            var normalized = roughStep / magnitude;

            var niceNormalized = 0f;
            
            if (normalized < 1.5f)
                niceNormalized = 1f;
            else if (normalized < 3f)
                niceNormalized = 2f;
            else if (normalized < 7f)
                niceNormalized = 5f;
            else
                niceNormalized = 10f;

            return Mathf.Max(niceNormalized * magnitude, 1);
        }

        public static float GetNiceMin(float min, float step)
        {
            return Mathf.Floor(min / step) * step;
        }

        public static float GetNiceMax(float max, float step)
        {
            return Mathf.Ceil(max / step) * step;
        }

        public static float FindMaxAmongAllDataProviders(List<DataProvider> providers)
        {
            var ans = providers[0].MaxValue;

            for(int i = 1; i < providers.Count; i++)
            {
                var providerMax = providers[i].MaxValue;
                
                if (ans < providerMax)
                    ans = providerMax;
            }

            return ans;
        }
        
        public static float FindMinAmongAllDataProviders(List<DataProvider> providers)
        {
            var ans = providers[0].MinValue;

            for(int i = 1; i < providers.Count; i++)
            {
                var providerMax = providers[i].MinValue;
                
                if (ans > providerMax)
                    ans = providerMax;
            }

            return ans;
        }

        public static bool DoValuesHaveDifferentSigns(float n, float m)
        {
            return n * m <= 0; 
        }
        
    }
}