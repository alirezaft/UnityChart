using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityChart.Editor
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

    public NiceScale(float min, float max, bool atLeastOne)
    {
        if (min > max)
            throw new ArgumentException("min should not be greater than max");
        
        minPoint = min;
        maxPoint = max;
        this.atLeastOne = atLeastOne;
        Calculate();
    }

    private void Calculate()
    {
        if (Mathf.Approximately(minPoint, maxPoint))
        {
            range = Mathf.Abs(minPoint) > 1e-6f ? Mathf.Abs(minPoint) * 0.1f : 1f;
            minPoint -= range;
            maxPoint += range;
        }

        range = NiceNum(Mathf.Abs(maxPoint - minPoint), false);
        tickSpacing = NiceNum(range / (maxTicks - 1f), true);

        if (atLeastOne)
        {
            niceMin = Mathf.Max(1, Mathf.Floor(minPoint / tickSpacing) * tickSpacing);
            niceMax = Mathf.Ceil(maxPoint / tickSpacing) * tickSpacing;
            if (niceMax == niceMin) niceMax += tickSpacing;
        }
        else
        {
            niceMin = Mathf.Floor(minPoint / tickSpacing) * tickSpacing;
            niceMax = Mathf.Ceil(maxPoint / tickSpacing) * tickSpacing;
        }
    }

    private float NiceNum(float range, bool round)
    {
        if (range <= 0) return 0;

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

        return niceFraction * Mathf.Pow(10f, exponent);
    }

    public void SetMinMaxPoints(float min, float max)
    {
        if (min > max)
            throw new ArgumentException("min should not be greater than max");
        
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
            float tick = niceMin + (i * tickSpacing);

            tick = (float)Math.Round(tick, 6, MidpointRounding.AwayFromZero);
            result.Add(tick);
        }

        return result;
    }

    public float TickSpacing => tickSpacing;
    public float NiceMin => niceMin;
    public float NiceMax => niceMax;
}

}