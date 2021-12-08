using System;
using System.Collections.Generic;
using Gvr.Internal;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace EFS.Timesnap.VR
{
    public class GoogleVRStuff : MonoBehaviour, IVRImplementation
    {
        public GvrHeadset Headset;
        public GvrPointerInputModule InputModule;
        public GvrControllerInput ControllerInput;
        public GvrEditorEmulator EditorEmulator;
        public InstantPreview InstantPreviewMain;

        public bool ShowLaser
        {
            set
            {
                if (_controllerPointer.LaserVisual != null)
                {
                    _controllerPointer.LaserVisual.Laser.enabled = value;
                }
            }
        }

        public Color LaserColor
        {
            set
            {
                if (_controllerPointer.LaserVisual != null)
                {
                    _controllerPointer.LaserVisual.laserColor = value;
                }
            }
        }

        public Color LaserColorEnd
        {
            set
            {
                if (_controllerPointer.LaserVisual != null)
                {
                    _controllerPointer.LaserVisual.laserColorEnd = value;
                }
            }
        }

        public List<TimesnapLaserPointer> Lasers => new List<TimesnapLaserPointer> {_controllerPointer};

        public bool InstantPreviewEnabled
        {
            set { InstantPreviewMain.gameObject.SetActive(value); }
        }

        public float DefaultReticleDepth
        {
            set { _controllerPointer.defaultReticleDistance = value; }
            private get { return _controllerPointer.defaultReticleDistance; }
        }

        public IObservable<GameObject> HoverBegin => Observable.Merge(
            _controllerPointer.OnPointerEnterEvent,
            _gazePointer.OnPointerEnterEvent);

        public IObservable<GameObject> HoverEnd => Observable.Merge(
            _controllerPointer.OnPointerExitEvent,
            _gazePointer.OnPointerExitEvent);

        public IObservable<Unit> ClickUp => Observable.Merge(
            _controllerPointer.OnPointerClickUpEvent,
            _gazePointer.OnPointerClickUpEvent);

        public IObservable<Unit> ClickDown => Observable.Merge(
            _controllerPointer.OnPointerClickDownEvent,
            _gazePointer.OnPointerClickDownEvent);


        [SerializeField] private TimesnapGvrReticlePointer _gazePointer;
        [SerializeField] private TimesnapLaserPointer _controllerPointer;


        public Vector3 RayEndpoint
        {
            get
            {
                if (_gazePointer.gameObject.activeInHierarchy)
                {
                    if (_gazePointer.CurrentRaycastResult.isValid)
                        return _gazePointer.CurrentRaycastResult.worldPosition;
                    return _gazePointer.GetRayForDistance(DefaultReticleDepth).ray.GetPoint(DefaultReticleDepth);
                }

                if (_controllerPointer.LaserVisual == null)
                {
                    return Vector3.zero;
                }

                return _controllerPointer.LaserVisual.reticle.transform.position;
            }
        }
    }
}