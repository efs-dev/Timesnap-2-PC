using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using EFS.Timesnap.VR;
public class VRButton : MonoBehaviour
{
    [Serializable]
    public class ScaleConfig
    {
        public Vector3 Amount = Vector3.one * 1.5f;
        public float Duration = 1f;
        public Ease Easing = Ease.OutQuad;
    }

    public bool ShowTooltipOnRollover = true;
    [TextArea]
    public string TooltipText;

    [Space(10)]
    // --------------
    public bool ScaleOnHover = false;

    public ScaleConfig HoverScaleConfig = new ScaleConfig();
    private Vector3 _originalScale;
    private Tween _hoverTween;


    [Space(20)]
    // --------------
    public UnityEvent HoverBegin;

    public UnityEvent HoverEnd = new UnityEvent();
    public UnityEvent ClickDown = new UnityEvent();
    public UnityEvent ClickUp = new UnityEvent();

    public readonly BoolReactiveProperty Hovering = new BoolReactiveProperty(false);

    public static VRButton Overbutton;
    public static bool IsOverButton { get { return Overbutton != null; } }

    public IObservable<Unit> OnClickAsObservable() => ClickDown.AsObservable();
    public IObservable<bool> OnHoverAsObservable() => Observable.Merge(
        HoverBegin.AsObservable().Select(_ => true),
        HoverEnd.AsObservable().Select(_ => false)
    );

    public void EnableVR(float duration)
    {
        FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>().EnableVR(duration);
    }

    public void DisableVR(float duration)
    {
        FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>().DisableVR(duration);
    }

    public void Println(string s)
    {
        print(s);
    }

    void OnDisable()
    {
        if (Overbutton == this)
            Overbutton = null;
    }

    private void Start()
    {
        _originalScale = transform.localScale;
        Hovering.Subscribe(hovering =>
        {
            if (hovering)
                HoverBegin?.Invoke();
            else
                HoverEnd?.Invoke();
        });

        var canvas = GetComponent<Canvas>();

        if (canvas != null)
            canvas.worldCamera = Camera.main;

      //  if (GetComponent<BoxCollider>() != null)
       //     Debug.Log(name + " - " + transform.position + " - " + transform.localScale + " - " + GetComponent<BoxCollider>()?.size);
    }

    Vector3 GetTrueScale(Transform t)
    {
        if (t.parent == null)
            return t.localScale;
        else
        {
            var parentScale = GetTrueScale(t.parent);
            return new Vector3(t.localScale.x * parentScale.x, t.localScale.y * parentScale.y, t.localScale.z * parentScale.z);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!VRInputHelper.IsPointerEnabled)
            return;

        TimesnapVRPlayer.Instance.Pointer.SetHovering(true);
        Overbutton = this;
        Hovering.Value = true;
        if (ScaleOnHover)
        {
            KillHoverTween();
            _hoverTween = transform.DOScale(
                Vector3.Scale(_originalScale, HoverScaleConfig.Amount),
                HoverScaleConfig.Duration
            ).SetEase(HoverScaleConfig.Easing);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!VRInputHelper.IsPointerEnabled)
            return;

        TimesnapVRPlayer.Instance.Pointer.SetHovering(false);
        Overbutton = null;
        Hovering.Value = false;
        if (ScaleOnHover)
        {
            KillHoverTween();
            _hoverTween = transform.DOScale(
                _originalScale,
                HoverScaleConfig.Duration
            ).SetEase(HoverScaleConfig.Easing);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!VRInputHelper.IsPointerEnabled)
            return;

        ClickDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!VRInputHelper.IsPointerEnabled)
            return;

        ClickUp?.Invoke();
    }

    public void KillHoverTween()
    {
        _hoverTween?.Kill();
    }
    /*
    public void OnMouseDown()
    {
        OnPointerDown(null);
    }

    public void OnMouseUp()
    {
        OnPointerUp(null);
    }

    public void OnMouseEnter()
    {
        OnPointerEnter(null);
    }

    public void OnMouseExit()
    {
        OnPointerExit(null);
    }*/
    
}
