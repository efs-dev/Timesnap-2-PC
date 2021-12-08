using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using EFS.Timesnap.VR;

using System;

[InitializeOnLoad]
public class TimesnapSettingsEditor : EditorWindow {

    [UnityEditor.Callbacks.DidReloadScripts]
    static TimesnapSettingsEditor()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
        Menu.SetChecked("VRPanorama/Devices/Occulus Go", VRPlayerDevices.Settings != null && VRPlayerDevices.Settings.DeviceType == XRDeviceType.OculusGo);
        Menu.SetChecked("VRPanorama/Devices/Daydream", VRPlayerDevices.Settings != null && VRPlayerDevices.Settings.DeviceType == XRDeviceType.GoogleDaydream);
        Menu.SetChecked("VRPanorama/Devices/PC", VRPlayerDevices.Settings != null && VRPlayerDevices.Settings.DeviceType == XRDeviceType.PC);
    }

    [MenuItem("VRPanorama/Devices/Occulus Go", priority = 1001)]
    static void DeviceOcculus()
    {
        VRPlayerDevices.Settings.DeviceType = XRDeviceType.OculusGo;
        EditorUtility.SetDirty(VRPlayerDevices.Settings);
        AssetDatabase.SaveAssets();

       /// PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, GetSdKsForDeviceType(VRPlayerDevices.Settings.DeviceType));
    }

    [MenuItem("VRPanorama/Devices/Daydream", priority = 1000)]
    static void DeviceDaydream()
    {
        VRPlayerDevices.Settings.DeviceType = XRDeviceType.GoogleDaydream;
        EditorUtility.SetDirty(VRPlayerDevices.Settings);
        AssetDatabase.SaveAssets();

      //  PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, GetSdKsForDeviceType(VRPlayerDevices.Settings.DeviceType));
    }


    [MenuItem("VRPanorama/Devices/PC", priority = 1000)]
    static void DevicePC()
    {
        VRPlayerDevices.Settings.DeviceType = XRDeviceType.PC;
        EditorUtility.SetDirty(VRPlayerDevices.Settings);
        AssetDatabase.SaveAssets();

        //PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, GetSdKsForDeviceType(VRPlayerDevices.Settings.DeviceType));
    }


    void OnGUI()
    {
        var deviceType = (XRDeviceType) EditorGUILayout.EnumPopup("Device Type", VRPlayerDevices.Settings.DeviceType);
        if (deviceType != VRPlayerDevices.Settings.DeviceType)
        {
            VRPlayerDevices.Settings.DeviceType = deviceType;
            EditorUtility.SetDirty(VRPlayerDevices.Settings);
            AssetDatabase.SaveAssets();

          //  if (deviceType != XRDeviceType.PC)
              //  PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, GetSdKsForDeviceType(deviceType));
        }
    }


    private static string[] GetSdKsForDeviceType(XRDeviceType type)
    {
        switch (type)
        {
            case XRDeviceType.OculusGo:
                return new[] { "Oculus" };
            case XRDeviceType.GoogleCardboard:
                return new[] { "cardboard", "daydream" };
            case XRDeviceType.GoogleDaydream:
                return new[] { "daydream", "cardboard" };
            case XRDeviceType.MagicWindow:
                return new[] { "daydream", "cardboard" };
            case XRDeviceType.PC:
                return new string[0];
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
