using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace City.Utils
{
    public static class TransformExtensions
    {
        public static void SetAsChild(this Transform self, Transform child)
        {
            child.transform.SetParent(self);
            child.transform.localScale = Vector3.one;
            child.transform.localPosition = Vector3.zero;
            var rectTransform = child as RectTransform;
            if (rectTransform != null)
                rectTransform.sizeDelta = Vector3.one;
        }

        public static IEnumerable<Transform> ToEnumerable(this Transform self)
        {
            foreach (Transform child in self)
            {
                yield return child;
            }
        }
    }
}