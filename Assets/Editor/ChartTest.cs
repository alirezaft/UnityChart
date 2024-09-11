using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChartTest : EditorWindow
{
    private static VisualElement root;
    private void OnEnable()
    {
        var UXMLFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ChartTest.uxml");
        UXMLFile.CloneTree(rootVisualElement);
        root = rootVisualElement;
    }

    [MenuItem("UChart/TestWindow")]
    public static void ShowTestWindow()
    {
        var window = EditorWindow.GetWindow<ChartTest>(false, "Chart Test", true);
        window.Show();
    }

    [MenuItem("UChart/Repopulatedataset")]
    public static void RepopulateTestData()
    {
        root.Q<Chart>().RepopulateDataset();
    }
}
