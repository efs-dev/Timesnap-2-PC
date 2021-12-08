using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EFS.Timesnap.VR;
using TMPro;

using UnityEngine.Events;

public class VRSlider : MonoBehaviour
{
    public GameObject Thumb;
    public Transform Min;
    public Transform Max;

    public bool StopDraggingOnExit = true;

    public bool AutoText = true;
    public TextMeshProUGUI Text;
    public bool AppendTextPercentage;

    private float _value = 0;
    public float Value {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            RefreshStep();
            RefreshText();
            OnChange?.Invoke();
        }
    }


    public UnityEvent OnChange;
    public UnityEvent OnPointerUp;

    public float MinValue = 0;
    public float MaxValue = 10;
    public float Steps = 10;

    private float _range;
    private float _step;
    private float _valueStep;

    public int CurrentStep { get; private set; }

    private bool _isDragging;

    private Vector3 _originalPosition;
    private EFS.Timesnap.VR.TimesnapVRPlayer _vrPlayer;

    private Transform _thumbRT;

    void RefreshStep()
    {
        float rangeValue = _range * Value;
        float stepValue = rangeValue / _step;

        Debug.Log("Value: " + Value + ", _range: " + _range + ", _step: " + _step + ", rangeValue: " + rangeValue + ", step value: " + stepValue);
        //var svs = stepValue.ToString();
        //var realStepValue = float.Parse(svs);
        CurrentStep = Mathf.RoundToInt(stepValue);
        //Debug.Log(svs);
        //Debug.Log(realStepValue);
        //Debug.Log("_range: " + _range + ", " + Value + ", " + _step + ", " + _currentStep);// + ", rangeValue: " + rangeValue + ", stepValue: " + stepValue + ", curStep: " + Mathf.FloorToInt(stepValue) + ", " + Mathf.FloorToInt(rangeValue) + ", " + Mathf.Floor(stepValue) + ", " + (stepValue == 2) + ", " + int.Parse(stepValue.ToString()));
    }

    public void Refresh()
    {
        var pos = _thumbRT.localPosition;
        var range = Max.localPosition.x - Min.localPosition.x;
        pos.x = Min.localPosition.x + (CurrentStep * _valueStep * range);
        _thumbRT.localPosition = pos;
    }

    void ApplyStep()
    {
        Refresh();

        var pos = _thumbRT.localPosition;
        Value = (pos.x - Min.localPosition.x) / (Max.localPosition.x - Min.localPosition.x);
    }

    void RefreshText()
    {
        //Debug.Log(_currentStep + ", " + _step + ", " + (_currentStep * _step) + ", " + Value);
        if (Text != null && AutoText)
            Text.text = (CurrentStep * _step) + (AppendTextPercentage ? "%" : "");
    }



    // Use this for initialization
    void Awake() {
        _vrPlayer = GetComponent<EFS.Timesnap.VR.TimesnapVRPlayer>();
       Thumb.GetComponent<VRButton>().ClickDown.AddListener(() => _isDragging = true);
        /* 
        Thumb.GetComponent<VRButton>().ClickUp.AddListener(() =>
        {
            _isDragging = false;
            ApplyStep();
            OnPointerUp?.Invoke();
        });*/
        _thumbRT = Thumb.GetComponent<RectTransform>();
        _originalPosition = _thumbRT.localPosition;

        _range = MaxValue - MinValue;
        _step = _range / Steps;
        _valueStep = 1 / Steps;

        var pos = _thumbRT.localPosition;
        Value = (pos.x - Min.localPosition.x) / (Max.localPosition.x - Min.localPosition.x);

        GetComponent<VRButton>().ClickDown.AddListener(() =>
        {
            _isDragging = true;
        });

        /*
        GetComponent<VRButton>().ClickUp.AddListener(() => 
        {
            OnMoveThumb();
            OnPointerUp?.Invoke();
        });
        */
    }

    void ApplyPosition()
    {
        Thumb.transform.position = TargetManager.GetCollisionPoint(GetComponent<Collider>());// FindObjectOfType<Pointer>().transform.position;
        var pos = _thumbRT.localPosition;
        pos.x = Mathf.Clamp(pos.x, Min.localPosition.x, Max.localPosition.x);
        pos.y = _originalPosition.y;// Mathf.Clamp(pos.y, Min.localPosition.y, Max.localPosition.y);
        pos.z = _originalPosition.z;

        _thumbRT.localPosition = pos;
        Debug.Log("apply position: " + pos.x + ", minx: " + Min.localPosition.x + ", maxx: " + Max.localPosition.x);
        Value = (pos.x - Min.localPosition.x) / (Max.localPosition.x - Min.localPosition.x);
    }

    void OnMoveThumb()
    {
        ApplyPosition();
        ApplyStep();
    }

    // Update is called once per frame
    void Update() {
        if (!_isDragging)
            return;

        Debug.Log("is dragging");
        if ((StopDraggingOnExit && (TargetManager.Target == null || !TargetManager.Target.transform.IsChildOf(transform))) || Input.GetMouseButtonUp(0))//!VRPanorama3DObject.IsOver || (VRPanorama3DObject.Over.gameObject != gameObject && VRPanorama3DObject.Over.gameObject != Thumb.gameObject))))
        {
            Debug.Log("stop dragging");
            OnPointerUp?.Invoke();
            ApplyStep();
            _isDragging = false;
        }
        else
        {
            Debug.Log("apply position");
            ApplyPosition(); 
            ApplyStep();
        }
    }
}
