using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataProvider
{
    public List<float> Dataset;
    
    private Color m_Color;
    public Color Color => m_Color;
    
    private string m_Name;
    public string Name => m_Name;


    public int Length => Dataset.Count;
    public float MaxValue => Dataset.Max();
    public float MinValue => Dataset.Min();

    public DataProvider(Color color, string name)
    {
        Dataset = new List<float>();
        m_Color = color;
        m_Name = name;
    }

    public void AddDataPoint(float value)
    {
        Dataset.Add(value);
    }
}
