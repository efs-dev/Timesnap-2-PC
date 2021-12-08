using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EmulatorSettings : EditorWindow {


    [UnityEditor.MenuItem("Emulator/Settings")]
    static void SetEmulatorIpAddress()
    {
        // Get existing open window or if none, make a new one:
        var window = (EmulatorSettings)EditorWindow.GetWindow(typeof(EmulatorSettings));
        window.titleContent = new GUIContent("Emulator");
        window.Show();
    }

    void OnGUI()
    {
        var ipAddress = PlayerPrefs.HasKey("EmulatorIpAddress") ? PlayerPrefs.GetString("EmulatorIpAddress") : "";
        ipAddress = EditorGUILayout.TextField("Ip Address", ipAddress);
        PlayerPrefs.SetString("EmulatorIpAddress", ipAddress);
    }

}
