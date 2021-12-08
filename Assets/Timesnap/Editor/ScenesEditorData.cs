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

public class ScenesEditorData : ScriptableObject
{
    //public bool AutoAddScenes;
    public List<ScenesEditorEntry> Entries = new List<ScenesEditorEntry>();
    public List<ScenesEditorEntry> AllEntries = new List<ScenesEditorEntry>();

    public bool ShowHidden;

    public string PreviousScene
    {
        get { return PlayerPrefs.HasKey("ScenesEditor_Previous_Scene") ? PlayerPrefs.GetString("ScenesEditor_Previous_Scene") : ""; }
        set { PlayerPrefs.SetString("ScenesEditor_Previous_Scene", value); }
    }


    public List<string> EditorBuildScenes;

    public Vector2 ScrollPos
    {
        get { return new Vector2( PlayerPrefs.HasKey("ScenesEditor_Scroll_Pos_X") ? PlayerPrefs.GetFloat("ScenesEditor_Scroll_Pos_X") : 0, PlayerPrefs.HasKey("ScenesEditor_Scroll_Pos_Y") ? PlayerPrefs.GetFloat("ScenesEditor_Scroll_Pos_Y") : 0); }
        set { PlayerPrefs.SetFloat("ScenesEditor_Scroll_Pos_X", value.x); PlayerPrefs.SetFloat("ScenesEditor_Scroll_Pos_Y", value.y); }
    }
}
