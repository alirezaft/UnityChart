using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityChart.Runtime
{
    public class DataProviderRegistry : ScriptableSingleton<DataProviderRegistry>
    {
        private List<DataProvider> m_DataProviderRegistry = new List<DataProvider>();

        public void AddDataProvider(DataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("Data provider is null");
            if (m_DataProviderRegistry.Count > 0 &&
                m_DataProviderRegistry.FirstOrDefault(item => item.ID.Equals(provider.ID)) != null)
                throw new ArgumentException("A data provider with the same ID exists.");
            
            m_DataProviderRegistry.Add(provider);
        }

        public DataProvider GetDataProvider(string id)
        {
            return m_DataProviderRegistry.Find(item => item.ID.Equals(id));
        }
    }
}