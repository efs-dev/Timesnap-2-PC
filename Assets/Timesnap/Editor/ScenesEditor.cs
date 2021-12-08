using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class ScenesEditorLauncher
{
    static ScenesEditorLauncher()
    {
        EditorApplication.playmodeStateChanged -= ScenesEditor.OnPlayModeChanged;
	    EditorApplication.playmodeStateChanged += ScenesEditor.OnPlayModeChanged;
	    //Debug.Log("listening for playmodestatechanged");
    }
}

[Serializable]
public class ScenesEditorEntry
{
    public bool Autoplay;
    public string Path;
    public string Name;
    public bool Hidden;
}



public class ScenesEditor : EditorWindow
{
    public static ScenesEditor Instance { get; private set; }

    public static Func<List<string>> GetAdditionalScenes;

    public static void OnPlayModeChanged()
	{
		//Debug.Log("OnPlayModeChanged " + Instance + ", " + Instance.LaunchedThroughEditor);
        if (Instance == null || !Instance.LaunchedThroughEditor)
	        return;
	        

        if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
	        //Debug.Log("Starting Play");
            EditorApplication.update += ReloadLastScene;
        }
    }

    static void ReloadLastScene()
    {
        if (EditorApplication.isPlaying)
            return;

	    //Debug.Log("Ending Play");

	    //Debug.Log("active scene: " + EditorSceneManager.GetActiveScene().name + ", " + Instance.Data.PreviousScene);
        EditorApplication.update -= ReloadLastScene;
        if (EditorSceneManager.GetActiveScene().name != Instance.Data.PreviousScene)
        {
            Instance.LaunchedThroughEditor = false;
            //if (Instance.Data.AutoAddScenes)
           //     EditorBuildSettings.scenes = Instance.Data.EditorBuildScenes.ConvertAll<EditorBuildSettingsScene>(x => new EditorBuildSettingsScene(x, true)).ToArray();

            //Debug.Log(Instance.Data.PreviousScene);
            EditorSceneManager.OpenScene(Instance.Data.PreviousScene);
        }
    }

        

    public bool LaunchedThroughEditor;
    public ScenesEditorData Data;

    public Func<string, string, bool> ValidateScenePath = (name, path) => true;

    [MenuItem("Window/Scenes")]
    static void Init()
    {
        var window = EditorWindow.GetWindow<ScenesEditor>();
        window.titleContent = new GUIContent("Scenes");

        Instance = window;
        var sceneEditorDatas = AssetDatabase.FindAssets("t:ScenesEditorData");
        if (sceneEditorDatas.Length == 0)
        {
            window.Data = ScriptableObject.CreateInstance<ScenesEditorData>();
            AssetDatabase.CreateAsset(window.Data, "Assets/ScenesEditor.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            //Debug.Log("found data at: " + sceneEditorDatas[0]);
            window.Data = AssetDatabase.LoadAssetAtPath<ScenesEditorData>(AssetDatabase.GUIDToAssetPath(sceneEditorDatas[0]));
        }

        //window.Data.ScrollPos = Vector2.zero;

        window.FindScenes();
        window.Show();
    }

    [DidReloadScripts]
    static void OnReloadScripts()
    {
        var windows = Resources.FindObjectsOfTypeAll<ScenesEditor>();
        if (windows.Length == 0)
            return;
        Instance = windows[0];
        Instance.FindScenes();
    }

    private void FindScenes()
    {
        if (Data == null)
            return;

        var fileList = new List<string>();
        if (GetAdditionalScenes != null)
            fileList.AddRange(GetAdditionalScenes());

        fileList.AddRange(Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories));
        var old = Data.AllEntries.ToList();
        Data.AllEntries = fileList.ConvertAll<ScenesEditorEntry>(x => new ScenesEditorEntry() { Path = x.Replace(Application.dataPath + "\\", "").Replace("\\", "/"), Name = x.Replace("\\", "/").Substring(x.Replace("\\", "/").LastIndexOf("/") + 1, x.Length - x.Replace("\\", "/").LastIndexOf("/") - 1).Replace(".unity", "") }).FindAll(x => ValidateScenePath(x.Name, x.Path));

        old.ForEach(entry =>
        {
            var found = Data.AllEntries.Find(x => x.Name == entry.Name);
            if (found != null)
                found.Hidden = entry.Hidden;
        });
    }

    void OnGUI()
    {
        if (Data == null)
            Init();

        if (Data == null)
            return;

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUI.backgroundColor = Data.ShowHidden ? Color.yellow : Color.white;
        if (GUILayout.Button("Show Hidden", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            Data.ShowHidden = !Data.ShowHidden;
            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssets();
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Show All", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            Data.AllEntries.ForEach(entry => entry.Hidden = false);
            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Hide All", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            Data.AllEntries.ForEach(entry => entry.Hidden = true);
            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssets();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        Data.ScrollPos = EditorGUILayout.BeginScrollView(Data.ScrollPos);
        // if (!EditorApplication.isPlaying)
        //  Data.AutoAddScenes = GUILayout.Toggle(Data.AutoAddScenes, "Automatically Add All Scenes to Build");


        var assetBundleNames = new List<string>(AssetDatabase.GetAllAssetBundleNames());
        assetBundleNames.Insert(0, "None");
        for (var i = 0; i < Data.AllEntries.Count; i++)
        {
            var entry = Data.AllEntries[i];
            var path = "Assets/" + entry.Path;
            var assetIndex = path.LastIndexOf("Assets/");
            path = path.Substring(assetIndex, path.Length - assetIndex);

            if (!Data.ShowHidden && entry.Hidden)
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (Data.ShowHidden)
            {
                if (!entry.Hidden)
                {
                    if (GUILayout.Button("S", EditorStyles.toolbarButton, GUILayout.Width(25)))
                    {
                        entry.Hidden = true;
                        EditorUtility.SetDirty(Data);
                        AssetDatabase.SaveAssets();
                    }
                }
                else
                {
                    if (GUILayout.Button("H", EditorStyles.toolbarButton, GUILayout.Width(25)))
                    {
                        entry.Hidden = false;
                        EditorUtility.SetDirty(Data);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            if (GUILayout.Button("Select", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
            }
            if (!Application.isPlaying && GUILayout.Button("Open", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(path);
                }
            }

            if (!Application.isPlaying && GUILayout.Button("Play", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                // User pressed play -- autoload master scene.
                Data.PreviousScene = EditorSceneManager.GetActiveScene().path;
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(path);
                }
                LaunchedThroughEditor = true;
                //Data.EditorBuildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes).ConvertAll<string>(x => x.path);
               // EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
                EditorApplication.isPlaying = true;
            }
            EditorGUILayout.LabelField(entry.Name, GUILayout.Width(200));
            EditorGUILayout.LabelField(path);

            /*
                        Debug.Log(path);
                        var sceneAsset = AssetImporter.GetAtPath(path);
                        int index = assetBundleNames.IndexOf(sceneAsset.assetBundleName);
                        if (index == -1)
                            index = 0;
                        var newIndex = EditorGUILayout.Popup(index, assetBundleNames.ToArray(), GUILayout.Width(100));
                        sceneAsset.assetBundleName = newIndex > 0 ? assetBundleNames[newIndex] : null;
                        */
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/WebGL", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }

    public static void BuildPlayer()
    {

    }
}
