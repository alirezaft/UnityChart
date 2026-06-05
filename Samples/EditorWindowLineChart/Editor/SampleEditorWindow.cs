using UnityChart.Editor;
using UnityChart.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SampleEditorWindow : EditorWindow
{
    private string uxmlFileGUID = "75f3c0ea5afa8bc418f857e8e3c5d002";

    public void OnEnable()
    {
        var uxmlFilePath = AssetDatabase.GUIDToAssetPath(uxmlFileGUID);
        var uxmlFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlFilePath);

        uxmlFile.CloneTree(rootVisualElement);
    }

    [MenuItem("Unity Chart Samples/Editor Window Sample")]
    public static void ShowSampleEditorWindow()
    {
        var window = EditorWindow.GetWindow<SampleEditorWindow>(false, "Editor Window Chart Sample", true);
        window.Show();
    }
}
