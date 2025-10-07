using UnityEditor;
using UnityEngine;

namespace UnityChart.Runtime
{
    [InitializeOnLoad]
    public static class DataProviderRegistryInitializer
    {
        static DataProviderRegistryInitializer()
        {
            EditorApplication.playModeStateChanged += ResetBeforePlayMode;
        }
        
        public static void ResetBeforePlayMode(PlayModeStateChange state)
        {
            if(state != PlayModeStateChange.ExitingEditMode)
                return;
            
            Debug.Log("Restart before play mode");
            DataProviderRegistry.instance.ClearRegistry();
        }
    }
}