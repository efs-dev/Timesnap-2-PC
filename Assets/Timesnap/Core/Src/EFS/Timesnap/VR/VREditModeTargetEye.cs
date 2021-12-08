using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif
[ExecuteInEditMode]
public class VREditModeTargetEye : MonoBehaviour {

#if UNITY_EDITOR
    static VREditModeTargetEye()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    static void OnReloadScripts()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
        Menu.SetChecked("VRPanorama/Visualize/Left Eye", !EditorPrefs.HasKey("VRPanoramaTargetEye") || EditorPrefs.GetInt("VRPanoramaTargetEye") == 0);
        Menu.SetChecked("VRPanorama/Visualize/Right Eye", EditorPrefs.HasKey("VRPanoramaTargetEye") && EditorPrefs.GetInt("VRPanoramaTargetEye") == 1);
    }

    public enum TargetEyes { Left, Right };
    public static TargetEyes TargetEye { get { return (TargetEyes)(UnityEditor.EditorPrefs.HasKey("VRPanoramaTargetEye") == false ? 0 : UnityEditor.EditorPrefs.GetInt("VRPanoramaTargetEye")); } }
        
    [UnityEditor.MenuItem("VRPanorama/Visualize/Left Eye", priority = 1000)]
    static void ShowLeftEye()
    {
        UnityEditor.EditorPrefs.SetInt("VRPanoramaTargetEye", 0);
    }

    [UnityEditor.MenuItem("VRPanorama/Visualize/Right Eye", priority = 1001)]
    static void ShowRightEye()
    {
        UnityEditor.EditorPrefs.SetInt("VRPanoramaTargetEye", 1);
    }

    void Update()
    {
        var cameras = FindObjectsOfType<Camera>();
        
        for (var i = 0; i < cameras.Length; i++)
        {
            var camera = cameras[i];

            camera.depth = (TargetEye == TargetEyes.Left && camera.stereoTargetEye == StereoTargetEyeMask.Left) || (TargetEye == TargetEyes.Right && camera.stereoTargetEye == StereoTargetEyeMask.Right) ? 1 : 0;
        }
    }
#endif

}
