using System.Collections;
using EFS.Timesnap.VR;
using UniRx;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(VRButton))]
public class MagicWindowModeButton : MonoBehaviour
{
    private SpriteRenderer _img;
    private string _loadedDeviceName;
    private TimesnapVRPlayer _timesnapVRPlayer;

    void Start()
    {
        _img = GetComponent<SpriteRenderer>();
        _timesnapVRPlayer = FindObjectOfType<TimesnapVRPlayer>();
        _loadedDeviceName = XRSettings.loadedDeviceName;

        GetComponent<VRButton>().OnClickAsObservable()
            .Scan(false, (b, _) => !b)
            .Subscribe(toggled => StartCoroutine(Toggle(toggled)));
    }

    IEnumerator Toggle(bool toggled)
    {
        if (toggled)
        {
            _loadedDeviceName = XRSettings.loadedDeviceName;
            XRSettings.LoadDeviceByName("");
            _img.color = Color.red;
            _timesnapVRPlayer.DeviceType = XRDeviceType.MagicWindow;
            yield return null;
            ResetCameras();
        }
        else
        {
            print("TOGGLE: " + _loadedDeviceName);
            
            _img.color = Color.green;
            _timesnapVRPlayer.DeviceType = XRDeviceType.GoogleDaydream;
            
            XRSettings.LoadDeviceByName(_loadedDeviceName);
            yield return null;
            XRSettings.enabled = true;
        }
    }


    // source: https://developers.google.com/vr/develop/unity/guides/hybrid-apps
    // Resets camera transform and settings on all enabled eye cameras.
    void ResetCameras()
    {
        // Camera looping logic copied from GvrEditorEmulator.cs
        for (int i = 0; i < Camera.allCameras.Length; i++)
        {
            Camera cam = Camera.allCameras[i];
            if (cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None)
            {
                // Reset local position.
                // Only required if you change the camera's local position while in 2D mode.
                cam.transform.localPosition = Vector3.zero;

                // Reset local rotation.
                // Only required if you change the camera's local rotation while in 2D mode.
//                cam.transform.localRotation = Quaternion.identity;

                // No longer needed, see issue github.com/googlevr/gvr-unity-sdk/issues/628.
                // cam.ResetAspect();

                // No need to reset `fieldOfView`, since it's reset automatically.
            }
        }
    }
}