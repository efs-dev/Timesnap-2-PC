using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Src.Scripts.OnScreenKeyboard
{
    public class OnScreenKeyboard : MonoBehaviour, INonSelectable
    {
        public IObservable<string> KeyStrokes => _keyStrokes;
        public IObservable<Unit> Submissions => EnterKey.OnClickAsObservable();
        public IObservable<Unit> Backspaces => DeleteKey.OnClickAsObservable();

        public Key.KeyAnimationConfigStuff KeyAnimationConfig;
        public List<Key> Keys;
        public Button ShiftKey;
        public Button DeleteKey;
        public Button EnterKey;
        public CanvasGroup CanvasGroup;

        public float HideShowOffset = 1;
        public float HideShowAnimationSpeed = 0.6f;
        public Ease HideShowEase = Ease.OutQuad;

        public Color ShiftHighlightColor;
        private Color _shiftDefaultColor;
        private readonly ISubject<string> _keyStrokes = new Subject<string>();
        private Tween _hideShowTween;
        private IFormElement _targetElement;

        private void Start()
        {
            Hide(0);
            Keys.ForEach(it => it.KeyAnimationConfig = KeyAnimationConfig);

            _shiftDefaultColor = ShiftKey.image.color;

            ShiftKey.OnClickAsObservable()
                .Scan(false, (b, _) => !b)
                .Subscribe(t => ShiftKey.image.color = t ? ShiftHighlightColor : _shiftDefaultColor);

            ShiftKey.OnClickAsObservable()
                .Subscribe(_ => Keys.ForEach(key => key.ToggleUppercase()));

            Keys.Select(key => key.Strokes).Merge().Subscribe(_keyStrokes);

            KeyStrokes.Select(Event.KeyboardEvent)
                .Merge(
                    Backspaces.Select(_ => Event.KeyboardEvent("backspace"))
                ).Subscribe(ev =>
                {
                    print($"got event: [{ev}]. targetElement: {_targetElement}");

                    _targetElement?.ProcessEvent(ev);
                    _targetElement?.Refresh();
                });
        }

        public void Show(IFormElement formElement)
        {
            _targetElement = formElement;
            Show();
        }

        public void Show()
        {
            Show(HideShowAnimationSpeed);
        }

        public void Show(float duration)
        {
            _hideShowTween?.Complete();
            CanvasGroup.interactable = true;
            _hideShowTween = DOTween.Sequence()
                .Join(transform.DOLocalMoveZ(0, duration).SetEase(HideShowEase))
                .Join(CanvasGroup.DOFade(1, duration).SetEase(HideShowEase));
        }

        public void Hide()
        {
            Hide(HideShowAnimationSpeed);
        }

        public void Hide(float duration)
        {
            _targetElement = null;
            CanvasGroup.interactable = false;
            _hideShowTween?.Complete();
            _hideShowTween = DOTween.Sequence()
                .Join(transform.DOLocalMoveZ(HideShowOffset, duration).SetEase(HideShowEase))
                .Join(CanvasGroup.DOFade(0, duration).SetEase(HideShowEase));
        }

        private void OnValidate()
        {
            Keys.ForEach(it => it.KeyAnimationConfig = KeyAnimationConfig);
        }
    }
}