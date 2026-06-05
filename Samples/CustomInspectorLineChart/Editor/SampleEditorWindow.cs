using UnityChart.Editor;
using UnityChart.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SampleDataGenerator)), CanEditMultipleObjects]
public class SampleEditorWindow : Editor
{
    [SerializeField] private VisualTreeAsset uxmlFile;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        uxmlFile.CloneTree(root);
        root.Q<LineChart>().Owner = new DataProviderOwner((SampleDataGenerator)target);

        return root;
    }
}
