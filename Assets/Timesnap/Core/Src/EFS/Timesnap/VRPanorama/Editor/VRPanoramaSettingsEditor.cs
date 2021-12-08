using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class VRPanoramaSettingsEditor : EditorWindow {

    [MenuItem("VRPanorama/Settings", priority = 2000)]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<VRPanoramaSettingsEditor>();
        window.titleContent = new GUIContent("VRP Settings");
    }

    void OnGUI()
    {
        var settings = VRPanorama.Settings;

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Global Rotation", GUILayout.Width(145));
        settings.GlobalRotation = EditorGUILayout.Vector3Field("", settings.GlobalRotation);
        EditorGUILayout.EndHorizontal();
        settings.GlobalScale = EditorGUILayout.FloatField("Global Scale", settings.GlobalScale);
        settings.GlobalPointerDistance = EditorGUILayout.FloatField("Global Pointer Distance", settings.GlobalPointerDistance);

        settings.FlipHorizontal = EditorGUILayout.Toggle("Flip Horizontal", settings.FlipHorizontal);
        settings.FlipVertical = EditorGUILayout.Toggle("Flip Vertical", settings.FlipVertical);
    }
}
