using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityChart.Runtime
{
    public class DataProviderRegistry : ScriptableSingleton<DataProviderRegistry>
    {
        private List<DataProvider> m_DataProviderRegistry = new List<DataProvider>();
        internal event Action<DataProvider> OnDataProviderRemoved;
        internal event Action<DataProvider> OnDataProviderAdded;

        private void Awake()
        {
            EditorSceneManager.sceneOpened += RemoveDeadDataProvidersOnSceneChange;
        }

        private void RemoveDeadDataProvidersOnSceneChange(Scene scene, OpenSceneMode mode)
        {
            RemoveDeadDataProviders();
        }

        internal void AddDataProvider(DataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (ProviderExists(provider))
                throw new InvalidOperationException("A data provider with the same ID with the same owner exists.");
            if (IsProviderIDNull(provider))
                throw new ArgumentNullException(nameof(provider.ID));

            RemoveDeadDataProviders();
            
            m_DataProviderRegistry.Add(provider);
            OnDataProviderAdded?.Invoke(provider);
        }

        private bool IsProviderIDNull(DataProvider provider)
        {
            return provider.ID is null || provider.ID.Trim() == "";
        }

        public DataProvider GetDataProvider(string id, DataProviderOwner? owner = null)
        {
            if(!DataProviderRegistryInitializer.OutsidePlayMode)
            {
                RemoveDeadDataProviders();
            }
            
            if (owner is null)
                return m_DataProviderRegistry.Find(item =>
                    item.ID.Equals(id) && item.GetOwner().scope == OwnershipScope.GlobalScope);

            var ans =  m_DataProviderRegistry.Find(item =>
                item.ID.Equals(id) && item.GetOwner().ID.Equals(owner.Value.ID));

            return ans;
        }

        public bool ProviderExists(DataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            
            return m_DataProviderRegistry.Count > 0 &&
                   m_DataProviderRegistry.FirstOrDefault(item =>
                       item.ID.Equals(provider.ID) &&
                       item.GetOwner().ID.Equals(provider.GetOwner().owner != null ? provider.GetOwner().ID : "")) != null;
        }

        internal void RemoveDataProvider(DataProvider provider)
        {
            if (!ProviderExists(provider))
                throw new InvalidOperationException("This provider is not in the registry.");

            OnDataProviderRemoved?.Invoke(provider);
            m_DataProviderRegistry.Remove(provider);
            provider.ClearEventSubscriptions();
        }

        private void RemoveDeadDataProviders()
        {
            var deadProviders = m_DataProviderRegistry.FindAll((item) =>
            {
                var ownerData = item.GetOwner();

                return ownerData.scope == OwnershipScope.ComponentScope && ownerData.owner == null;
            });

            foreach (var provider in deadProviders)
            {
                RemoveDataProvider(provider);
                provider.Dispose();
            }
        }

        public void ClearRegistry()
        {
            Debug.Log("REmoving");
            foreach (var provider in m_DataProviderRegistry)
            {
                OnDataProviderRemoved?.Invoke(provider);
                provider.ClearEventSubscriptions();
            }

            m_DataProviderRegistry.Clear();
        }
    }
}