using System;
using Gvr.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace EFS.Timesnap.VR
{
    // this is a monobehaviour that sits on the same GameObject as TimesnapVRPlayer
    // and updates stuff while in the editor
    [ExecuteInEditMode]
    public class VRPlayerEditMode : MonoBehaviour
    {
        private XRDeviceType _oldDeviceType;
#if UNITY_EDITOR
        public void Update()
        {
            var vrPlayer = GetComponent<TimesnapVRPlayer>();
            var deviceType = vrPlayer.DeviceType;

            vrPlayer.SetImplementation();
            vrPlayer.UpdateEditorAndPlayer();
            /*if (deviceType != _oldDeviceType && !Application.isPlaying)
            {
                PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, GetSdKsForDeviceType(deviceType));
                _oldDeviceType = deviceType;
//                EditorPrefs.SetString("Timesnap.");
            }*/

            EmulatorConfig.WIFI_SERVER_IP = vrPlayer.EmulatorIpAddress;
        }
#endif
      /*  private string[] GetSdKsForDeviceType(XRDeviceType type)
        {
            switch (type)
            {
                case XRDeviceType.OculusGo:
                    return new[] {"Oculus"};
                case XRDeviceType.GoogleCardboard:
                    return new[] {"cardboard", "daydream"};
                case XRDeviceType.GoogleDaydream:
                    return new[] {"daydream", "cardboard"};
                case XRDeviceType.MagicWindow:
                    return new[] {"daydream", "cardboard"};
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }*/
    }
}