using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EFS.Timesnap.VR
{
    [ExecuteInEditMode]
    public class VRPlayerDevices : MonoBehaviour
    {
        private static TimesnapSettings _settings;
        public static TimesnapSettings Settings {
            get
            {
                if (_settings == null)
                    _settings = Resources.Load<TimesnapSettings>("TimesnapSettings");
                return _settings;
            }
        }

        void Start()
        {
            if (Settings != null)
                GetComponent<TimesnapVRPlayer>().DeviceType = Settings.DeviceType;
        }

#if UNITY_EDITOR
        void Update()
        {
            if (!Application.isPlaying && Settings != null && GetComponent<TimesnapVRPlayer>().DeviceType != Settings.DeviceType)
                GetComponent<TimesnapVRPlayer>().DeviceType = Settings.DeviceType;
        }
#endif
    }
}