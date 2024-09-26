using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataProvider
{
    private List<float> m_DataSet;
    public float[] Dataset => m_DataSet.ToArray();
    
    private Color m_Color;
    public Color Color => m_Color;
    
    private string m_Name;
    public string Name => m_Name;


    public int Length => m_DataSet.Count;
    public float MaxValue => m_DataSet.Max();
    public float MinValue => m_DataSet.Min();

    public DataProvider(Color color, string name)
    {
        m_DataSet = new List<float>();
        m_Color = color;
        m_Name = name;
    }

    public void AddDataPoint(float value)
    {
        m_DataSet.Add(value);
    }
}
