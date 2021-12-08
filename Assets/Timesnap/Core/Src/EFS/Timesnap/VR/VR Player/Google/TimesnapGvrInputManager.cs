using System;
using EFS.Timesnap.VR;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public class TimesnapGvrInputManager : MonoBehaviour
{
    public GvrReticlePointer ReticlePointer;
    public GvrControllerInput ControllerMain;
    public GvrTrackedController ControllerPointer;
    private XRDeviceType _deviceType;

    [Inject]
    [UsedImplicitly]
    private void Construct(DeviceDetector detector)
    {
        print(detector.DeviceType);
        _deviceType = detector.DeviceType.ValueOr(() => FindObjectOfType<TimesnapVRPlayer>().DeviceType);
    }

    private void OnEnable()
    {
        SetVRInputMechanism(_deviceType);
    }

    private void SetVRInputMechanism(XRDeviceType deviceType)
    {
        SetGazeInputActive(deviceType == XRDeviceType.GoogleCardboard);
        SetControllerInputActive(deviceType == XRDeviceType.GoogleDaydream);
    }

    private void SetGazeInputActive(bool active)
    {
        if (ReticlePointer == null)
        {
            return;
        }

        ReticlePointer.gameObject.SetActive(active);

        // Update the pointer type only if this is currently activated.
        if (!active)
        {
            return;
        }

        GvrPointerInputModule.Pointer = ReticlePointer;
    }

    private void SetControllerInputActive(bool active)
    {
        if (ControllerMain != null)
        {
            // when this is set to false, instant preview stops tracking
            ControllerMain.gameObject.SetActive(active);
        }

        if (ControllerPointer == null)
        {
            return;
        }

        ControllerPointer.gameObject.SetActive(active);

        // Update the pointer type only if this is currently activated.
        if (!active)
        {
            return;
        }

        GvrPointerInputModule.Pointer = ControllerPointer.GetComponentInChildren<GvrLaserPointer>(true);
    }
}