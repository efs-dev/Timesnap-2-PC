using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPanoramaGroup : MonoBehaviour {

    [SerializeField, GetSet("Alpha")]
    private float _alpha;

    public float Alpha
    {
        get { return _alpha; }
        set { _alpha = Mathf.Clamp(value, 0, 1); }
    }

    private float _cachedAlpha;
    private bool _cachedThisFrame;

    void LateUpdate()
    {
        _cachedThisFrame = false;
    }

    public float GetTrueAlpha()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            _cachedThisFrame = false;
#endif
        if (!_cachedThisFrame)
        {
            _cachedAlpha = Alpha;

            var vrPanoramaGroup = GetComponentInParent<VRPanoramaGroup>();
            if (vrPanoramaGroup != null && vrPanoramaGroup != this)
                _cachedAlpha *= vrPanoramaGroup.GetTrueAlpha();

            _cachedThisFrame = true;
            return _cachedAlpha;
        }
        else
        {
            return _cachedAlpha;
        }
        
    }

}
