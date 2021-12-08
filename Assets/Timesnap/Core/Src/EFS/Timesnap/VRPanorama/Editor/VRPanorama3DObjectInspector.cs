using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRPanorama3DObject))]
public class VRPanorama3DObjectInspector : Editor
{

    SerializedProperty Depth;
    SerializedProperty DepthRangeId;
    SerializedProperty CollisionType;
    SerializedProperty CursorDepthOffset;
    SerializedProperty FaceCamera;
    SerializedProperty DisableWhenNoVR;

    VRPanorama3DObject _target;

    void OnEnable()
    {
        _target = (VRPanorama3DObject)target;
        Depth = serializedObject.FindProperty("Depth");
        DepthRangeId = serializedObject.FindProperty("DepthRangeId");
        CollisionType = serializedObject.FindProperty("CollisionType");
        CursorDepthOffset = serializedObject.FindProperty("CursorDepthOffset");
        FaceCamera = serializedObject.FindProperty("FaceCamera");
        DisableWhenNoVR = serializedObject.FindProperty("DisableWhenNoVR");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(CollisionType, new GUIContent("CollisionType"));

        if (_target.CollisionType == VRPanorama3DObject.CollisionTypes.Collider)
        {

        }
        else if (_target.CollisionType == VRPanorama3DObject.CollisionTypes.Depth)
        {
            EditorGUILayout.PropertyField(Depth, new GUIContent("Depth"));
            EditorGUILayout.PropertyField(DepthRangeId, new GUIContent("DepthRangeId"));
            if (VRPanorama.DepthRanges.Ranges.Find(x => x.Id == _target.DepthRangeId) == null)
            {
                EditorGUILayout.HelpBox("Invalid RangeId!", MessageType.Error);
            }
            EditorGUILayout.PropertyField(CursorDepthOffset, new GUIContent("Pointer Depth Offset"));
            EditorGUILayout.PropertyField(FaceCamera, new GUIContent("Rotate Towards Camera"));
        }

        
        EditorGUILayout.PropertyField(DisableWhenNoVR, new GUIContent("Require VR"));

        serializedObject.ApplyModifiedProperties();
    }
}
