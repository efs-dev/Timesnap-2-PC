using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public class CameraControls : EditorWindow
{
    private const string ShowCameraControls = "VRPanorama/Debug/Show Camera Controls";
    static bool _showCameraControls;

    static CameraControls()
    {
        EditorApplication.delayCall -= OnUpdate;
        EditorApplication.delayCall += OnUpdate;
    }

    static void OnUpdate()
    {
        _showCameraControls = EditorPrefs.HasKey("VRPanoramaCameraControls") && EditorPrefs.GetInt("VRPanoramaCameraControls") == 1;
        Menu.SetChecked(ShowCameraControls, _showCameraControls);
        SceneView.RepaintAll();
    }

    [MenuItem(ShowCameraControls, priority = 200)]
    public static void OnShowCameraControls()
    {
        if (!_showCameraControls)
        {
            EditorPrefs.SetInt("VRPanoramaCameraControls", 1);
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;
            OnUpdate();
        }
        else
        {
            EditorPrefs.SetInt("VRPanoramaCameraControls", 0);
            SceneView.onSceneGUIDelegate -= OnScene;
            OnUpdate();
        }
    }

    [DidReloadScripts]
    static void OnReloadScripts()
    {
        EditorApplication.delayCall -= OnUpdate;
        EditorApplication.delayCall += OnUpdate;

        if (EditorPrefs.HasKey("VRPanoramaCameraControls") && EditorPrefs.GetInt("VRPanoramaCameraControls") == 1)
        {
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;
        }
    }

    private static void OnScene(SceneView sceneview)
    {
        Handles.BeginGUI();

        if (GUI.Button(new Rect(10, 10, 110, 20), "Center Camera"))
        {
            var sceneCamera = SceneView.lastActiveSceneView.camera;
            sceneCamera.transform.position = Vector3.zero;
            SceneView.lastActiveSceneView.AlignViewToObject(sceneCamera.transform);
        }

        Handles.EndGUI();
    }
}