using JME.UnionTypes;
using UnityEngine;
using UnityEngine.XR;

namespace EFS.Timesnap.VR
{
    public class DeviceDetector
    {
        public Maybe<XRDeviceType> DeviceType => GetDeviceType();



        private Maybe<XRDeviceType> GetDeviceType()
        {
            var device = XRSettings.loadedDeviceName;
            // in the editor this isn't set... what do we do?
            if (device == "daydream")
            {
                return Maybe.Some(XRDeviceType.GoogleDaydream);
            }

            if (device == "cardboard")
            {
                return Maybe.Some(XRDeviceType.GoogleCardboard);
            }

            if (device == "Oculus")
            {
                return Maybe.Some(XRDeviceType.OculusGo);
            }

            return Maybe.None<XRDeviceType>();
        }
    }
}