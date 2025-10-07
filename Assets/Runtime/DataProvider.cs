using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityChart.Runtime
{
    public class DataProvider
    {
        private List<float> m_Dataset;
        public List<float> Dataset
        {
            set
            {
                m_Dataset = value;
                m_IsLastMinValid = false;
                m_IsLastMaxValid = false;
                DataPointPositions.Clear();
                for (var i = 0; i < m_Dataset.Count; i++)
                {
                    DataPointPositions.Add(new Vector2());
                }
                OnDataChanged?.Invoke();
            }
            get => m_Dataset;
        }


        public List<Vector2> DataPointPositions;
        public event Action OnDataChanged;

        private string m_ID;
        public string ID => m_ID;

        private Color m_Color;
        public Color Color => m_Color;

        private string m_Name;
        public string Name => m_Name;

        private bool m_IsLastMaxValid;
        private float m_LastMax;
        private bool m_IsLastMinValid;
        private float m_LastMin;


        public int Length => m_Dataset.Count;
        public float MaxValue()
        {
            if (m_Dataset.Count == 0)
                return 0;
            
            if(!m_IsLastMaxValid)
            {
                m_IsLastMaxValid = true;
                m_LastMax = m_Dataset.Max();
            }

            return m_LastMax;
        }

        public float MinValue()
        {
            if (m_Dataset.Count == 0)
                return 0;

            if(!m_IsLastMinValid)
            {
                m_IsLastMinValid = true;
                m_LastMin = m_Dataset.Min();
            }

            return m_LastMin;
        }

        public DataProvider(Color color, string name, string id)
        {
            m_Dataset = new List<float>();
            DataPointPositions = new List<Vector2>();
            m_Color = color;
            m_Name = name;
            m_ID = id;

            m_IsLastMaxValid = false;
            m_IsLastMinValid = false;
            DataProviderRegistry.instance.AddDataProvider(this);
        }

        public void AddDataPoint(float value)
        {
            m_Dataset.Add(value);
            if(value < m_LastMin)
                m_IsLastMinValid = false;

            if (value > m_LastMax)
                m_IsLastMaxValid = false;
            DataPointPositions.Add(new Vector2());
            OnDataChanged?.Invoke();
        }

        public void ClearEventSubscriptions()
        {
            OnDataChanged = null;
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
