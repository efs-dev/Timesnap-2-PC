using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SizeMatcher : UIBehaviour
{
    public RectTransform Target;
    public bool MatchWidth;
    public bool MatchHeight;
    private RectTransform _thisRectTransform;

    protected override void Awake()
    {
        _thisRectTransform = transform as RectTransform;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (_thisRectTransform == null) return;
        var newWidth = MatchWidth ? Target.rect.width : _thisRectTransform.sizeDelta.x;
        var newHeight = MatchHeight ? Target.rect.height : _thisRectTransform.sizeDelta.y;

        if (MatchWidth)
        {
            var minWidth = LayoutUtility.GetMinWidth(_thisRectTransform);
            newWidth = Mathf.Max(newWidth, minWidth);
        }

        if (MatchHeight)
        {
            var minHeight = LayoutUtility.GetMinWidth(_thisRectTransform);
            newHeight = Mathf.Max(newHeight, minHeight);
        }

        _thisRectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}