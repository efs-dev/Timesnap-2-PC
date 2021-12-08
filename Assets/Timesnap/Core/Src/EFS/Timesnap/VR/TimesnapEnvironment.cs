using DG.Tweening;
using UnityEngine;

public class TimesnapEnvironment : MonoBehaviour
{
    public Material LeftMaterial;
    public Material RightMaterial;

    public Renderer LeftEye;
    public Renderer LeftEyeFallback;
    public Renderer RightEye;

    private bool _stateDisabled;
    private Tween _tween;
    private Tween _fallbackTween;

    private int _leftEyeRenderQueue;
    private int _leftEyeFallbackRenderQueue;

    void Awake()
    {
        _leftEyeRenderQueue = LeftEye.sharedMaterial.renderQueue;
        _leftEyeFallbackRenderQueue = LeftEyeFallback.sharedMaterial.renderQueue;

        LeftEyeFallback.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        LeftEye.sharedMaterial = LeftMaterial;
        LeftEyeFallback.sharedMaterial = RightMaterial;
        LeftEyeFallback.sharedMaterial.renderQueue = LeftEye.sharedMaterial.renderQueue - 1;
        RightEye.sharedMaterial = RightMaterial;
    }

    public void DisableVR(float duration)
    {
        _stateDisabled = true;
        _tween?.Kill();
        _fallbackTween?.Kill();
        LeftEyeFallback.gameObject.SetActive(true);
        LeftEyeFallback.material.color = Color.white;
        if (gameObject.activeInHierarchy)
        {
            LeftEyeFallback.sharedMaterial.renderQueue = _leftEyeFallbackRenderQueue;
            LeftEye.sharedMaterial.renderQueue = _leftEyeRenderQueue;
            LeftEyeFallback.material.color = Color.white;
            _tween = LeftEye.material.DOColor(new Color(1, 1, 1, 0), duration);
        }
        else
        {
            // if disabled, we could have been called by TimesnapVRPlayer, so skip the animation and just turn off
            LeftEye.material.color = new Color(1, 1, 1, 0);
        }
    }

    public void EnableVR(float duration)
    {
        _stateDisabled = false;
        _tween?.Kill();
        _fallbackTween?.Kill();
        if (gameObject.activeInHierarchy)
        {
            /* _fallbackTween = LeftEyeFallback.material.DOColor(new Color(1, 1, 1, 0), duration*0.5f).SetDelay(duration*0.5f).OnComplete(() =>
             {
                 LeftEyeFallback.gameObject.SetActive(false);
                 _fallbackTween = null;
             });*/


            LeftEyeFallback.sharedMaterial.renderQueue = _leftEyeRenderQueue;
            LeftEye.sharedMaterial.renderQueue = _leftEyeFallbackRenderQueue;

            LeftEye.material.color = Color.white;
            _tween = LeftEyeFallback.material.DOColor(new Color(1, 1, 1, 0), duration).OnComplete(() =>
            {
                LeftEyeFallback.gameObject.SetActive(false);
            });
        }
        else
        {
            LeftEye.material.color = Color.white;
        }
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