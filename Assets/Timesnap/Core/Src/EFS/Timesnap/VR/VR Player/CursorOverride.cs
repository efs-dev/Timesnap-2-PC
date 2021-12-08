using UnityEngine;

namespace EFS.Timesnap.VR
{
    public class CursorOverride : MonoBehaviour
    {
        public enum HoverOverrideMode
        {
            ShowHoverCursor,
            ShowDefaultCursor,
            ShowNothing,
            // ShowCustomCursor
        }

        public HoverOverrideMode HoverBehaviour;
    }
}