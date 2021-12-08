using UnityEngine;

public class iPhonexResizer : MonoBehaviour
{
    public bool IsIPhoneX = false;
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    void Start()
    {
//        IsIPhoneX = Device.generation == DeviceGeneration.iPhoneX;
        var rectTransform = GetComponent<RectTransform>();
        if (IsIPhoneX)
        {
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Left, rectTransform.rect.width - Left);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, Right, rectTransform.rect.width - Right);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, Top, rectTransform.rect.height - Top);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Bottom, rectTransform.rect.height - Bottom);
        }
    }
}