using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EFS.Timesnap.VR
{
    public class TimesnapGvrReticlePointer : GvrReticlePointer
    {
        public IObservable<GameObject> OnPointerEnterEvent => _onPointerEnterSubject.AsObservable();
        private readonly ISubject<GameObject> _onPointerEnterSubject = new Subject<GameObject>();

        public IObservable<GameObject> OnPointerExitEvent => _onPointerExitSubject.AsObservable();
        private readonly ISubject<GameObject> _onPointerExitSubject = new Subject<GameObject>();

        public IObservable<Unit> OnPointerClickUpEvent => _onPointerClickUpSubject.AsObservable();
        private readonly ISubject<Unit> _onPointerClickUpSubject = new Subject<Unit>();

        public IObservable<Unit> OnPointerClickDownEvent => _onPointerClickDownSubject.AsObservable();
        private readonly ISubject<Unit> _onPointerClickDownSubject = new Subject<Unit>();

        public override void OnPointerEnter(RaycastResult raycastResult, bool isInteractive)
        {
            base.OnPointerEnter(raycastResult, isInteractive);
            _onPointerEnterSubject.OnNext(raycastResult.gameObject);
        }

        public override void OnPointerExit(GameObject previousObject)
        {
            base.OnPointerExit(previousObject);
            _onPointerExitSubject.OnNext(previousObject);
        }

        public override void OnPointerClickDown()
        {
            base.OnPointerClickDown();
            _onPointerClickDownSubject.OnNext(Unit.Default);
        }

        public override void OnPointerClickUp()
        {
            base.OnPointerClickUp();
            _onPointerClickUpSubject.OnNext(Unit.Default);
        }

        public override bool Triggering => base.Triggering || Input.touchCount > 0;

        public override bool TriggerDown => base.TriggerDown ||
                                            Input.touchCount > 0 &&
                                            Input.touches.Any(it =>
                                                it.phase == TouchPhase.Began);

        public override bool TriggerUp => base.TriggerUp ||
                                          Input.touchCount > 0
                                          && Input.touches.Any(it =>
                                              it.phase == TouchPhase.Ended || it.phase == TouchPhase.Canceled);
    }
}