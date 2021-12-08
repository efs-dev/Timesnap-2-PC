using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace EFS.Timesnap.VR
{
    public class MagicWindowStuff : MonoBehaviour, IVRImplementation
    {
        public IObservable<GameObject> HoverBegin => _gazePointer.OnPointerEnterEvent;
        public IObservable<GameObject> HoverEnd => _gazePointer.OnPointerExitEvent;
        public IObservable<Unit> ClickUp => _gazePointer.OnPointerClickUpEvent;
        public IObservable<Unit> ClickDown => _gazePointer.OnPointerClickDownEvent;

        [SerializeField] private TimesnapGvrReticlePointer _gazePointer;

        public float DefaultReticleDepth;

        public Vector3 RayEndpoint => _gazePointer.CurrentRaycastResult.isValid
            ? _gazePointer.CurrentRaycastResult.worldPosition
            : _gazePointer.GetRayForDistance(DefaultReticleDepth).ray.GetPoint(DefaultReticleDepth);

        private void OnEnable()
        {
            GvrPointerInputModule.Pointer = _gazePointer;
        }

        public bool ShowLaser
        {
            set
            {
                /*noop*/
            }
        }

        public Color LaserColor
        {
            set
            {
                /*noop*/
            }
        }

        public Color LaserColorEnd
        {
            set
            {
                /*noop*/
            }
        }

        public List<TimesnapLaserPointer> Lasers =>
            new List<TimesnapLaserPointer>();
    }
}