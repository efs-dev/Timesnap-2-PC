using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ToggleVR : MonoBehaviour
{
    public Renderer LeftDefault;
    public Renderer LeftFallback;

    private bool _stateDisabled;
    private Tween _tween;

    public void DisableVR(float duration)
    {
        _tween?.Kill();
        _tween = LeftDefault.material.DOColor(new Color(1, 1, 1, 0), duration);
    }

    public void EnableVR(float duration)
    {
        _tween?.Kill();
        _tween = LeftDefault.material.DOColor(Color.white, duration);
    }

    public void Toggle(float duration)
    {
        if (!_stateDisabled)
        {
            EnableVR(duration);
        }
        else
        {
            DisableVR(duration);
        }

        _stateDisabled = !_stateDisabled;
    }
}