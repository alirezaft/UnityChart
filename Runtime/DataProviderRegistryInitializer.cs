using UnityEditor;
using UnityEngine;

namespace UnityChart.Runtime
{
    [InitializeOnLoad]
    internal static class DataProviderRegistryInitializer
    {
        public static PlayModeStateChange stateChange;
        
        static DataProviderRegistryInitializer()
        {
            EditorApplication.playModeStateChanged += ResetBeforePlayMode;
        }

        private static void ResetBeforePlayMode(PlayModeStateChange state)
        {
            stateChange = state;
            
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                if(state == PlayModeStateChange.ExitingPlayMode)
                    DataProviderRegistry.instance.MarkAllDataProvidersAsPreserve();
                
                return; 
            }

            DataProviderRegistry.instance.ClearRegistry();
        }
    }
}