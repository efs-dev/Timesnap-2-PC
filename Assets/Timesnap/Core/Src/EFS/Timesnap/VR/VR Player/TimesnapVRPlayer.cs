using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;


namespace EFS.Timesnap.VR
{
    public class TimesnapVRPlayer : MonoBehaviour
    {
        public static TimesnapVRPlayer Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public GameObject CCBox;

        [HideInInspector]
        public XRDeviceType DeviceType;

        public MagicWindowStuff MagicWindow;

        public GoogleVRStuff Google;

        public string EmulatorIpAddress { get { return PlayerPrefs.HasKey("EmulatorIpAddress") ? PlayerPrefs.GetString("EmulatorIpAddress") : ""; } }

        [GvrInfo(
            "If you want to use the shift+mouse controller emulation, \nInstantPreview must be disabled",
            2)]
        public bool UseGoogleInstantPreview = false;

        //[Space(10)] public OculusVRStuff Oculus;

        public Pointer Pointer;

        [Space(10)]
        [Tooltip("Controls the reticle's distance from the camera when it is not hovering over a target")]
        public float DefaultReticleDepth = 1f;

        public bool ShowLaser = true;
        public Color LaserColor = Color.white;
        public Color LaserColorEnd = Color.white;

        public bool UseCollisionPointerDistance;
        public float PointerDistance = 1;
        public float VRDisabledPointerDistance = 1;

        public bool UseToolTipDistance;
        public float ToolTipDistance = 1;

        public UnityEvent HoverBegin;
        public UnityEvent HoverEnd;
        public UnityEvent ClickDown;
        public UnityEvent ClickUp;

        public bool AutoVREffect;
        public float AutoVREffectDuration = 0.5f;
        public bool IgnorePointerOnVRDisable;

        [HideInInspector]
        public List<NoVREffect> DisableVREffectObjects = new List<NoVREffect>();
        public bool IsVREnabled { get; private set; }


        [Inject]
        [UsedImplicitly]
        private void Construct(DeviceDetector detector)
        {
            DeviceType = detector.DeviceType.ValueOr(DeviceType);
            SetImplementation();
        }

        private void Start()
        {
            SetImplementation();
            IsVREnabled = true;
            SetHandlers(Google);
            //SetHandlers(Oculus);
            SetHandlers(MagicWindow);


            //Cursor.lockState = CursorLockMode.Locked;
            //   Cursor.visible = false;
        }

        private void SetHandlers(IVRImplementation it)
        {
            /*
            it.HoverBegin.Subscribe(go =>
            {
                var cursorOverride = go.GetComponent<CursorOverride>();
                if (cursorOverride != null)
                    SetCursorOverride(cursorOverride);
                else
                    SetHovering(true);

                HoverBegin.Invoke();
            }).AddTo(this);

            it.HoverEnd.Subscribe(go =>
            {
                SetHovering(false);
                HoverEnd.Invoke();
            }).AddTo(this);

            it.ClickDown.Subscribe(go => ClickDown.Invoke()).AddTo(this);
            it.ClickUp.Subscribe(go => ClickUp.Invoke()).AddTo(this);

            it.HoverBegin.Subscribe(target =>
            {
                var vrButton = target.GetComponent<VRButton>();
                if (vrButton != null && vrButton.ShowTooltipOnRollover)
                {
                    Pointer.ShowTooltipText(vrButton.TooltipText);
                }
            }).AddTo(this);

            it.HoverEnd.Subscribe(target =>
            {
                var vrButton = target.GetComponent<VRButton>();
                if (vrButton != null)
                {
                    Pointer.HideTooltipText();
                }
            }).AddTo(this);
            */
        }

        private void SetHovering(bool hovering)
        {
            if (!VRInputHelper.IsPointerEnabled)
            {
                Pointer.HoverCursor.gameObject.SetActive(false);
                Pointer.DefaultCursor.gameObject.SetActive(true);
            }
            else
            {
                Pointer.HoverCursor.gameObject.SetActive(hovering);
                Pointer.DefaultCursor.gameObject.SetActive(!hovering);
            }
        }

        private void SetCursorOverride(CursorOverride cursorOverride)
        {
            switch (cursorOverride.HoverBehaviour)
            {
                case CursorOverride.HoverOverrideMode.ShowNothing:
                    Pointer.HoverCursor.gameObject.SetActive(false);
                    Pointer.DefaultCursor.gameObject.SetActive(false);
                    break;
                case CursorOverride.HoverOverrideMode.ShowDefaultCursor:
                    Pointer.HoverCursor.gameObject.SetActive(false);
                    Pointer.DefaultCursor.gameObject.SetActive(true);
                    break;
                case CursorOverride.HoverOverrideMode.ShowHoverCursor:
                    Pointer.HoverCursor.gameObject.SetActive(true);
                    Pointer.DefaultCursor.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // note: called every frame by VrPlayerEditMode 
        public void SetImplementation()
        {
            /*
            Oculus.gameObject.SetActive(DeviceType == XRDeviceType.OculusGo);
            MagicWindow.gameObject.SetActive(DeviceType == XRDeviceType.MagicWindow);
            Google.gameObject.SetActive(DeviceType == XRDeviceType.GoogleCardboard ||
                                        DeviceType == XRDeviceType.GoogleDaydream);
                                        */
        }

        private IVRImplementation _previousActiveImplementation;

        private void Update()
        {
            // if (Cursor.lockState != CursorLockMode.Locked)
            //    Cursor.lockState = CursorLockMode.Locked;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            //if (Cursor.visible)
            //    Cursor.visible = false;

            Pointer.gameObject.SetActive(!Input.GetMouseButton(1));
            Pointer.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, DefaultReticleDepth));


           // Cursor.lockState = Input.GetMouseButton(1) ? CursorLockMode.Locked : CursorLockMode.Confined;

            // Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, DefaultReticleDepth)) + ", " + Input.mousePosition);

            /*
            var activeVrImplementation = GetActiveVrImplementation();
            if (activeVrImplementation != _previousActiveImplementation)
            {
                print("VR Implementation changed: " + activeVrImplementation);
                SetImplementation();
                _previousActiveImplementation = activeVrImplementation;
            }

            //Pointer.SetFromController(activeVrImplementation.RayEndpoint);
            //SetLaserProps(activeVrImplementation);

            if (AutoVREffect)
            {
                // turn on vr
                if (DisableVREffectObjects.Count == 0 && !IsVREnabled)
                {
                    Debug.Log("turn on vr");
                    EnableVR(AutoVREffectDuration);
                }
                // turn off vr
                else if (DisableVREffectObjects.Count > 0 && IsVREnabled)
                {
                    Debug.Log("turn off vr");
                    DisableVR(AutoVREffectDuration);
                }
            }

            if (!IsVREnabled)
            {
                UseCollisionPointerDistance = VRPanorama3DObject.IsOver;
                PointerDistance = VRDisabledPointerDistance;
            }

            UpdateVREffect();

            ApplyToolTipDistance();
            
            UpdateEditorAndPlayer();
            */

            //Cursor.lockState = CursorLockMode.Locked;
        }

        public void ApplyToolTipDistance()
        {
            if (UseToolTipDistance)
            {
                var basePointer = FindObjectOfType<GvrBasePointer>();
                if (basePointer != null)
                {
                    Pointer.TooltipContainer.transform.position = basePointer.GetSpecificPointAlongPointer(ToolTipDistance);
                    Pointer.TooltipContainer.transform.LookAt(Camera.main.transform, Camera.main.transform.up);
                    Pointer.TooltipContainer.transform.Rotate(Vector3.up * 180);
                }
            }

        }

        public IVRImplementation GetActiveVrImplementation()
        {
            switch (DeviceType)
            {
                //case XRDeviceType.OculusGo:
                //    return Oculus;
                case XRDeviceType.GoogleCardboard:
                case XRDeviceType.GoogleDaydream:
                //   return Google;
                case XRDeviceType.MagicWindow:
                //     return MagicWindow;
                default:
                    return null;
            }
        }

        public void UpdateEditorAndPlayer()
        {
            Google.DefaultReticleDepth = DefaultReticleDepth;
            Google.InstantPreviewEnabled = UseGoogleInstantPreview;
            //   Oculus.DefaultReticleDepth = DefaultReticleDepth;
            MagicWindow.DefaultReticleDepth = DefaultReticleDepth;
        }

        private void SetLaserProps(IVRImplementation it)
        {
            // note; these aren't safe to call from the editor because LaserVisual gets set in GvrLaserVisual's awake
            it.ShowLaser = ShowLaser;
            it.LaserColor = LaserColor;
            it.LaserColorEnd = LaserColorEnd;
        }

        public IEnumerable<TimesnapLaserPointer> GetLasers()
        {
            return GetActiveVrImplementation().Lasers;
        }

        public void DisableVR(float duration)
        {
            IsVREnabled = false;
            UseCollisionPointerDistance = true;
            var environments = GetEnvironments();
            foreach (var environment in environments)
            {
                environment.DisableVR(duration);
            }

            TargetLeftToRightEye = 1;
            VREffectDuration = duration;

            /*
            var vrPanoramas = FindObjectsOfType<VRPanorama>();
            foreach (var vrPanorama in vrPanoramas)
            {
                vrPanorama.DisableVR(duration);
            }
            */
        }

        public void EnableVR(float duration)
        {
            Debug.Log("enable vr");
            IsVREnabled = true;
            UseCollisionPointerDistance = false;
            var environments = GetEnvironments();
            foreach (var environment in environments)
            {
                environment.EnableVR(duration);
            }

            TargetLeftToRightEye = 0;
            VREffectDuration = duration;

            /*
            var vrPanoramas = FindObjectsOfType<VRPanorama>();
            foreach (var vrPanorama in vrPanoramas)
            {
                vrPanorama.EnableVR(duration);
            }
            */
        }

        public void ToggleVR(float duration)
        {
            Debug.Log("disable vr");
            var environments = GetEnvironments();
            foreach (var environment in environments)
            {
                environment.Toggle(duration);
            }

            TargetLeftToRightEye = TargetLeftToRightEye == 0 ? 1 : 0;
            VREffectDuration = duration;

            /*
            var vrPanoramas = FindObjectsOfType<VRPanorama>();
            foreach (var vrPanorama in vrPanoramas)
            {
                vrPanorama.Toggle(duration);
            }
            */
        }

        public static float LeftToRightEye { get; private set; }
        public static float TargetLeftToRightEye { get; private set; }

        public static float VREffectDuration;

        void UpdateVREffect()
        {
            if (TargetLeftToRightEye > LeftToRightEye)
            {
                LeftToRightEye = Mathf.Min(LeftToRightEye + (Time.deltaTime / VREffectDuration), TargetLeftToRightEye);
            }
            else if (LeftToRightEye > TargetLeftToRightEye)
            {
                LeftToRightEye = Mathf.Max(LeftToRightEye - (Time.deltaTime / VREffectDuration), 0);
            }
        }

        private static IEnumerable<TimesnapEnvironment> GetEnvironments()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(it => it.GetComponentsInChildren<TimesnapEnvironment>(true));
        }
    }
}