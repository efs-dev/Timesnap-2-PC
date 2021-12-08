using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace City.Utils
{
    public static class GameObjectExtensions
    {
        public static void SetAsChild(this GameObject self, Transform child)
        {
            self.transform.SetAsChild(child);
        }

        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var component = self.GetComponent<T>();
            return component == null ? self.AddComponent<T>() : component;
        }

        public static T GetOrAddComponent<T>(this Component self) where T : Component
        {
            return self.gameObject.GetOrAddComponent<T>();
        }

        public static void MapComponent<TIn>(this Component self, Action<TIn> fn) where TIn : Component
        {
            var component = self.GetComponent<TIn>();
            if (component != null)
            {
                fn(component);
            }
        }

        public static TOut MapComponent<TIn, TOut>(this Component self, Func<TIn, TOut> fn) where TIn : Component
        {
            var component = self.GetComponent<TIn>();
            return component == null ? default(TOut) : fn(component);
        }

        public static void DestroyIfAble(this Object self)
        {
            if (self != null)
            {
                Object.Destroy(self);
            }
        }

        public static void SetLayerSelfAndChildren(this GameObject self, int layer)
        {
            self.layer = layer;
            foreach (Transform child in self.transform)
            {
                SetLayerSelfAndChildren(child.gameObject, layer);
            }
        }
    }
}