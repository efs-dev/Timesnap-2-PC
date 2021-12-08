using System.Collections.Generic;
using System.Linq;
using EFS.Timesnap.VR;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TimesnapVRPlayer))]
public class VRPlayerEditor : Editor
{
    private SerializedProperty _laserEnabledProp;
    private SerializedProperty _laserColorProp;
    private SerializedProperty _deviceTypeProp;
    private SerializedProperty _useGoogleInstantPreviewProp;

    private IList<SerializedProperty> _props;
    private bool _showUnityEvents;

    private const string ShowUnityEventsPrefsKey = "VRPlayerEditorShowUnityEvents";

    private void OnEnable()
    {
        _showUnityEvents = EditorPrefs.GetBool(ShowUnityEventsPrefsKey, false);
        _laserEnabledProp = serializedObject.FindProperty("ShowLaser");
        _laserColorProp = serializedObject.FindProperty("LaserColor");
        _deviceTypeProp = serializedObject.FindProperty("DeviceType");
        _useGoogleInstantPreviewProp = serializedObject.FindProperty("UseGoogleInstantPreview");

        var prop = serializedObject.GetIterator();
        prop.Next(true);
        _props = new List<SerializedProperty>(prop.CountInProperty());
        while (prop.NextVisible(false))
        {
            _props.Add(prop.Copy());
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var isCardboard = _deviceTypeProp.enumValueIndex == (int) XRDeviceType.GoogleCardboard;
        var isDaydream = _deviceTypeProp.enumValueIndex == (int) XRDeviceType.GoogleDaydream;
        var isGoogle = isCardboard || isDaydream;

        var supportsLaser = !isCardboard;


        foreach (var prop in _props.Where(it => it.type != "UnityEvent"))
        {
            // oh boy this is gonna bite me one day
            if (prop.name.Contains("Laser") && !supportsLaser)
            {
                continue;
            }

            if (SerializedProperty.EqualContents(prop, _laserEnabledProp) && supportsLaser)
            {
                if (GUILayout.Button("Select Lasers"))
                {
                    Selection.objects = ((TimesnapVRPlayer) target)
                        .GetLasers()
                        .Select(it => it.gameObject)
                        .Cast<Object>()
                        .ToArray();
                }
            }

            if (SerializedProperty.EqualContents(prop, _laserColorProp)
                && _laserEnabledProp.boolValue == false)
            {
                continue;
            }

            if (SerializedProperty.EqualContents(prop, _useGoogleInstantPreviewProp) && !isGoogle)
            {
                continue;
            }

            EditorGUILayout.PropertyField(prop, true);
        }

        if (GUILayout.Button("Select Pointer"))
        {
            Selection.activeGameObject = ((TimesnapVRPlayer) target).Pointer.gameObject;
        }

        EditorGUILayout.Space();
        var oldShowUnityEvents = _showUnityEvents;
        _showUnityEvents = EditorGUILayout.Foldout(_showUnityEvents, "Event Handlers");
        if (oldShowUnityEvents != _showUnityEvents)
        {
            EditorPrefs.SetBool(ShowUnityEventsPrefsKey, _showUnityEvents);
        }

        if (_showUnityEvents)
        {
            foreach (var unityEventProp in _props.Where(it => it.type == "UnityEvent"))
            {
                EditorGUILayout.PropertyField(unityEventProp, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}