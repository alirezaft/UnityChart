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
        internal event Action<DataProvider> OnDataProviderRemoved;
        internal event Action<DataProvider> OnDataProviderAdded;

        internal void AddDataProvider(DataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider)); 
            if (ProviderExists(provider))
                throw new InvalidOperationException("A data provider with the same ID exists.");
            if (provider.ID is null || provider.ID.Trim() == "")
                throw new ArgumentNullException(nameof(provider.ID));

            m_DataProviderRegistry.Add(provider);
            OnDataProviderAdded?.Invoke(provider);
        }

        public DataProvider GetDataProvider(string id)
        {
            return m_DataProviderRegistry.Find(item => item.ID.Equals(id));
        }

        public bool ProviderExists(DataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            
            return m_DataProviderRegistry.Count > 0 &&
                   m_DataProviderRegistry.FirstOrDefault(item => item.ID.Equals(provider.ID)) != null;
        }

        internal void RemoveDataProvider(DataProvider provider)
        {
            if (!ProviderExists(provider))
                throw new InvalidOperationException("This provider is not in the registry.");

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