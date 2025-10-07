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
        public event Action<DataProvider> OnDataProviderRemoved;
        public event Action<DataProvider> OnDataProviderAdded;

        public void AddDataProvider(DataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("Data provider is null");
            if (ProviderExists(provider))
                throw new ArgumentException("A data provider with the same ID exists.");

            m_DataProviderRegistry.Add(provider);
        }

        public DataProvider GetDataProvider(string id)
        {
            return m_DataProviderRegistry.Find(item => item.ID.Equals(id));
        }

        public bool ProviderExists(DataProvider provider)
        {
            return m_DataProviderRegistry.Count > 0 &&
                   m_DataProviderRegistry.FirstOrDefault(item => item.ID.Equals(provider.ID)) != null;
        }

        public void RemoveDataProvider(DataProvider provider)
        {
            if (!ProviderExists(provider))
                throw new ArgumentException("This provider is not in the registery.");

            OnDataProviderRemoved?.Invoke(provider);
            m_DataProviderRegistry.Remove(provider);
            provider.ClearEventSubscriptions();
        }

        public void ClearRegistry()
        {
            foreach (var provider in m_DataProviderRegistry)
            {
                OnDataProviderRemoved?.Invoke(provider);
                provider.ClearEventSubscriptions();
            }
            m_DataProviderRegistry.Clear();
        }
    }
}