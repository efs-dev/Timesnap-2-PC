using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Src.Scripts.OnScreenKeyboard
{
    public abstract class Key : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, INonSelectable
    {
        [Serializable]
        public class KeyAnimationConfigStuff
        {
            public float HoverTweenDuration = 0.1f;
            public float ClickTweenScaleFactor = 0.8f;
            public float ClickTweenDuration = 0.4f;
            public int ClickTweenVibrato = 1;
            public int ClickTweenElasticity = 1;
        }

        public KeyAnimationConfigStuff KeyAnimationConfig;
        public Button Button;
        public IObservable<string> Strokes => Button.OnClickAsObservable().Select(_ => GetValue());

        public abstract void ToggleUppercase();
        protected abstract string GetValue();

        private Tween _hoverTween;
        private Tween _pointerDownTween;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _hoverTween?.Kill();
            _hoverTween = transform.DOLocalMoveZ(0.2f, KeyAnimationConfig.HoverTweenDuration);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _hoverTween?.Kill();
            _hoverTween = transform.DOLocalMoveZ(0, KeyAnimationConfig.HoverTweenDuration);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownTween?.Complete();
            _pointerDownTween = transform.DOPunchScale(Vector3.one * KeyAnimationConfig.ClickTweenScaleFactor,
                KeyAnimationConfig.ClickTweenDuration,
                KeyAnimationConfig.ClickTweenVibrato,
                KeyAnimationConfig.ClickTweenElasticity);
        }
    }
}