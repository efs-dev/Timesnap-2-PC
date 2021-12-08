using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EFS.Timesnap.VR
{
    public class Pointer : MonoBehaviour
    {
        public static Pointer Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public TMP_Text TooltipLabel;
        public CanvasGroup TooltipContainer;

        public GameObject RegularState;
        public GameObject HoldState;

        public Image DefaultCursor;
        public Image HoverCursor;
        public Image HoldCursor;

        public Ease CursorAnimationEasing;
        public float CursorAnimationDuration;

        private Vector3 _defaultCursorOriginalScale;
        private Vector3 _hoverCursorOriginalScale;
        private Vector3 _holdCursorOriginalScale;

        private float _defaultCursorOriginalAlpha;
        private float _hoverCursorOriginalAlpha;
        private float _holdCursorOriginalAlpha;

        private Tween _hoverTweenSequence;

        private void Start()
        {
            SaveCursorState();
            ResetCursorVisibility();
            HideTooltipText();
        }

        private void ResetCursorVisibility()
        {
            DefaultCursor.gameObject.SetActive(true);
            HoverCursor.gameObject.SetActive(false);
        }

        private void SaveCursorState()
        {
            _defaultCursorOriginalAlpha = DefaultCursor.color.a;
            _hoverCursorOriginalAlpha = HoverCursor.color.a;
            _holdCursorOriginalAlpha = HoldCursor.color.a;

            _defaultCursorOriginalScale = DefaultCursor.transform.localScale;
            _hoverCursorOriginalScale = HoverCursor.transform.localScale;
            _holdCursorOriginalScale = HoldCursor.transform.localScale;
        }

        public void SetFromController(Vector3 fwd)
        {
            transform.position = fwd;
            transform.LookAt(Camera.main.transform, Camera.main.transform.up);
            transform.Rotate(Vector3.up * 180);
        }

        public void HideTooltipText()
        {
            TooltipContainer.alpha = 0;
            TooltipLabel.text = "";
            TooltipContainer.gameObject.SetActive(false);
        }

        public void ShowTooltipText(string text)
        {
            if (!VRInputHelper.IsPointerEnabled)
            {
                HideTooltipText();
                return;
            }
            // todo: fade in
            TooltipContainer.gameObject.SetActive(true);
            TooltipContainer.alpha = 1;
            TooltipLabel.text = text;
        }

        private void Update()
        {
            RegularState.SetActive(VRInputHelper.IsPointerEnabled);
            HoldState.SetActive(!VRInputHelper.IsPointerEnabled);
        }
        
        // [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public void SetHovering(bool hovering)
        {
            if (!VRInputHelper.IsPointerEnabled)
                hovering = false;

            _hoverTweenSequence?.Kill();
            if (hovering)
            {
                HoverCursor.gameObject.SetActive(true);
                _hoverTweenSequence = DOTween.Sequence()
                    .Join(DefaultCursor.DOFade(0, CursorAnimationDuration).SetEase(CursorAnimationEasing))
                    .Join(HoverCursor.DOFade(_hoverCursorOriginalAlpha, CursorAnimationDuration)
                        .SetEase(CursorAnimationEasing))
                    .Join(DefaultCursor.transform.DOScale(Vector3.zero, CursorAnimationDuration)
                        .SetEase(CursorAnimationEasing))
                    .Join(HoverCursor.transform.DOScale(_hoverCursorOriginalScale, CursorAnimationDuration)
                        .SetEase(CursorAnimationEasing));
            }
            else
            {
                _hoverTweenSequence = DOTween.Sequence()
                    .Join(DefaultCursor.DOFade(_defaultCursorOriginalAlpha, CursorAnimationDuration)
                        .SetEase(CursorAnimationEasing))
                    .Join(HoverCursor.DOFade(0, CursorAnimationDuration).SetEase(CursorAnimationEasing))
                    .Join(DefaultCursor.transform.DOScale(_defaultCursorOriginalScale, CursorAnimationDuration)
                        .SetEase(CursorAnimationEasing))
                    .Join(HoverCursor.transform.DOScale(Vector3.zero, CursorAnimationDuration)
                        .SetEase(CursorAnimationEasing));
            }
        }
    }
}