using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class VRPanoramaDepthRangesEditor : EditorWindow {

    [MenuItem("VRPanorama/Depth Ranges")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<VRPanoramaDepthRangesEditor>();
        window.titleContent = new GUIContent("Depth Ranges");
    }

    void OnGUI()
    {
        var depthRanges = VRPanorama.DepthRanges;

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            EditorUtility.SetDirty(depthRanges);
            AssetDatabase.SaveAssets();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        for (var i = 0; i < depthRanges.Ranges.Count; i++)
        {
            var depthRange = depthRanges.Ranges[i];

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.BeginHorizontal();
                var id = EditorGUILayout.TextField("Id: ", depthRange.Id);


                var style = new GUIStyle(EditorStyles.toolbarButton);
                style.fontStyle = FontStyle.Bold;
                style.padding.left = 0;
                style.padding.right = 0;
                style.margin.left = 3;
                style.contentOffset = new Vector2(2, 0);
                style.normal.textColor = UnityEngine.Color.white;

                GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
                if (GUILayout.Button("^", style, GUILayout.Width(15)))
                {
                    if (i <= 0)
                        return;

                    depthRanges.Ranges.RemoveAt(i);
                    depthRanges.Ranges.Insert(i - 1, depthRange);
                }
                if (GUILayout.Button("v", style, GUILayout.Width(15)))
                {
                    if (i >= depthRanges.Ranges.Count - 1)
                        return;

                    depthRanges.Ranges.RemoveAt(i);
                    depthRanges.Ranges.Insert(i + 1, depthRange);
                }

                GUI.backgroundColor = UnityEngine.Color.red;
                style = new GUIStyle(EditorStyles.toolbarButton);
                style.fontStyle = FontStyle.Bold;
                style.onNormal.textColor = UnityEngine.Color.white;
                style.active.textColor = UnityEngine.Color.white;
                style.padding.left = 0;
                style.padding.right = 0;
                style.margin.left = 3;
                style.contentOffset = new Vector2(2, 0);
                style.normal.textColor = UnityEngine.Color.white;
                if (GUILayout.Button("X", style, GUILayout.Width(15)))
                {
                    if  (EditorUtility.DisplayDialog("Delete Entry?", "Are you sure you want to delete this?", "Delete", "Cancel"))
                        depthRanges.Ranges.RemoveAt(i);
                }
                GUI.backgroundColor = UnityEngine.Color.white;
                GUILayout.Space(3);
                EditorGUILayout.EndHorizontal();


                //var distanceCM = EditorGUILayout.FloatField("Distance In CM: ", depthRange.DistanceCM);
                var unitClose = EditorGUILayout.FloatField("Units Close: ", depthRange.UnitClose);
                var unitFar = EditorGUILayout.FloatField("Units Far: ", depthRange.UnitFar);

                if (id != depthRange.Id)
                {
                    depthRange.Id = id;
                }

               // if (distanceCM != depthRange.DistanceCM)
               // {
               //     depthRange.DistanceCM = distanceCM;
               // }

                if (unitClose != depthRange.UnitClose)
                {
                    depthRange.UnitClose = unitClose;
                }

                if (unitFar != depthRange.UnitFar)
                {
                    depthRange.UnitFar = unitFar;
                }

            }
        }

        if (GUILayout.Button("Add Entry"))
        {
            depthRanges.Ranges.Add(new RangeSettings());
        }
    }
}
