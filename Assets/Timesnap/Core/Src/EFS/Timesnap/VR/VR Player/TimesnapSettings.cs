using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EFS.Timesnap.VR
{
    [CreateAssetMenu(fileName = "TimesnapSettings.asset", menuName = "TimeSnap/Settings", order = 100)]
    public class TimesnapSettings : ScriptableObject
    {
        public XRDeviceType DeviceType;
    }
}