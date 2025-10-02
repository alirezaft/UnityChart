using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityChart
{
    public class DataProvider
    {
        public List<float> Dataset;

        private Color m_Color;
        public Color Color => m_Color;

        private string m_Name;
        public string Name => m_Name;

        private bool m_IsLastMaxValid;
        private float m_LastMax;
        private bool m_IsLastMinValid;
        private float m_LastMin;


        public int Length => Dataset.Count;
        public float MaxValue()
        {
            if(!m_IsLastMaxValid)
            {
                m_IsLastMaxValid = true;
                m_LastMax = Dataset.Max();
            }

            return m_LastMax;
        }

        public float MinValue()
        {
            if(!m_IsLastMinValid)
            {
                m_IsLastMinValid = true;
                m_LastMin = Dataset.Min();
            }

            return m_LastMin;
        }

        public DataProvider(Color color, string name)
        {
            Dataset = new List<float>();
            m_Color = color;
            m_Name = name;

            m_IsLastMaxValid = false;
            m_IsLastMinValid = false;
        }

        public void AddDataPoint(float value)
        {
            Dataset.Add(value);
            if(value < m_LastMin)
                m_IsLastMinValid = false;

            if (value > m_LastMax)
                m_IsLastMaxValid = false;
        }

        public DataProviderLegend GetLegend()
        {
            return new DataProviderLegend(){Color = m_Color, Name = m_Name};
        }
    }

    public struct DataProviderLegend
    {
        public Color Color;
        public string Name;
    }
}
