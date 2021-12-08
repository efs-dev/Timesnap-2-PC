using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
[ExecuteInEditMode]
public class DisableScrollWhenContentFits : MonoBehaviour
{
    public ScrollRect Target;

    private void OnEnable()
    {
        Target = GetComponent<ScrollRect>();
    }

    // just use the active status of the scrollbars for now
    //   : this can break stuff if the scrollrect is scrolled down and the content shrinks
    //      normally the scrollrect would snap the content back up, but vertical scrolling gets
    //      disabled before that can happen
    //      note: attempted to mitigate by setting the normalizedPosition to 0 
    //            this fixes the above, but may cause other issues (content now seems to snap down when expanded)
    private void Update()
    {
        if (Target.verticalScrollbar != null)
        {
            Target.vertical = Target.verticalScrollbar.gameObject.activeSelf;
            if (!Target.vertical)
                Target.verticalNormalizedPosition = 0;
        }

        if (Target.horizontalScrollbar != null)
        {
            Target.horizontal = Target.horizontalScrollbar.gameObject.activeSelf;
            if (!Target.horizontal)
                Target.horizontalNormalizedPosition = 0;
        }
    }
}