using UnityEditor;
using UnityEngine;

namespace UnityChart.Runtime
{
    [InitializeOnLoad]
    public static class DataProviderRegistryInitializer
    {
        public static bool OutsidePlayMode;
        
        static DataProviderRegistryInitializer()
        {
            EditorApplication.playModeStateChanged += ResetBeforePlayMode;
        }
        
        public static void ResetBeforePlayMode(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                if(state == PlayModeStateChange.ExitingPlayMode)
                    OutsidePlayMode = true;
                
                return; 
            }

            OutsidePlayMode = false;
            DataProviderRegistry.instance.ClearRegistry();
        }
    }
}