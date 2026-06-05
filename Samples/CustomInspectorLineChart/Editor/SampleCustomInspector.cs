using UnityChart.Editor;
using UnityChart.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SampleDataGenerator)), CanEditMultipleObjects]
public class SampleCustomInspector : Editor
{
    private string uxmlFileGUID = "76a836259d1452f4e92e8bc11e30111e";

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        var assetPath = AssetDatabase.GUIDToAssetPath(uxmlFileGUID);
        var uxmlFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath); 
            
        uxmlFile.CloneTree(root);
        root.Q<LineChart>().Owner = new DataProviderOwner((SampleDataGenerator)target);
        
        return root;
    }
}
