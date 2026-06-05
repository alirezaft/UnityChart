using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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


        internal List<Vector2> DataPointPositions;
        internal event Action OnDataChanged;

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
        private DataProviderOwner m_Owner;


        public int Length => m_Dataset.Count;

        public float MaxValue()
        {
            if (m_Dataset.Count == 0)
                return 0;

            if (!m_IsLastMaxValid)
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

            if (!m_IsLastMinValid)
            {
                m_IsLastMinValid = true;
                m_LastMin = m_Dataset.Min();
            }

            return m_LastMin;
        }

        /// <summary>
        /// Creates a new data provider
        /// </summary>
        /// <param name="color">The color for the graph of this provider shown on chart</param>
        /// <param name="name">Name of it shown on the legend of the chart</param>
        /// <param name="id">ID of the provider</param>
        /// <param name="owner">Use this parameter if you are using a provider for a chart in a custom inspector</param>
        public DataProvider(Color color, string name, string id, MonoBehaviour owner = null)
        {
            m_Dataset = new List<float>();
            DataPointPositions = new List<Vector2>();
            m_Color = color;
            m_Name = name;
            m_ID = id;

            m_IsLastMaxValid = false;
            m_IsLastMinValid = false;

            m_Owner = new DataProviderOwner(owner);

            DataProviderRegistry.instance.AddDataProvider(this);
        }

        public void AddDataPoint(float value)
        {
            m_Dataset.Add(value);
            if (value < m_LastMin)
                m_IsLastMinValid = false;

            if (value > m_LastMax)
                m_IsLastMaxValid = false;
            DataPointPositions.Add(new Vector2());
            OnDataChanged?.Invoke();
        }

        internal void ClearEventSubscriptions()
        {
            OnDataChanged = null;
        }

        public void Dispose()
        {
            DataProviderRegistry.instance.RemoveDataProvider(this);
            ClearEventSubscriptions();
            m_Dataset.Clear();
            DataPointPositions.Clear();
        }

        public DataProviderLegend GetLegend()
        {
            return new DataProviderLegend() { Color = m_Color, Name = m_Name };
        }

        public DataProviderOwner GetOwner()
        {
            return m_Owner;
        }
    }

    public struct DataProviderLegend
    {
        public Color Color;
        public string Name;
    }

    public struct DataProviderOwner
    {
        public MonoBehaviour owner;
        public OwnershipScope scope;
        public string ID;

        public DataProviderOwner(MonoBehaviour providerOwner)
        {
            if (providerOwner is not null)
            {
                owner = providerOwner;
                scope = OwnershipScope.ComponentScope;
                ID = owner.GetInstanceID().ToString();
            }
            else
            {
                owner = null;
                scope = OwnershipScope.GlobalScope;
                ID = "";
            }
        }
    }

    public enum OwnershipScope
    {
        GlobalScope,
        ComponentScope
    }
}