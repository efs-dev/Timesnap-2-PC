using System.Collections;
using Gvr.Internal;
using UnityEngine;

namespace EFS.Timesnap.VR
{
    /// <summary>
    /// Add this to the InstantPreview prefab to fix that issue where tracking gets stuck
    /// </summary>
    [RequireComponent(typeof(InstantPreview))]
    public class InstantPreviewWorkaround : MonoBehaviour
    {
        IEnumerator Start()
        {
            var instantPreview = GetComponent<InstantPreview>();
            instantPreview.enabled = false;
            yield return null;
            instantPreview.enabled = true;
        }
    }
}