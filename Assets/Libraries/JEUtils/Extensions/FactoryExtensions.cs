using City.Utils;
using UnityEngine;
using Zenject;

namespace UniRx
{
    public static class FactoryExtensions
    {
        /// <summary>
        /// For factories which return MonoBehaviours: create the view and add it
        /// as a child of the given gameObject
        /// </summary>
        /// <param name="f">a factory</param>
        /// <param name="gameObject">the object which will become the parent of the new MonoBehaviour</param>
        /// <typeparam name="TView">the result of calling f.Create()</typeparam>
        /// <returns></returns>
        public static TView CreateInto<TView>(this IFactory<TView> f, GameObject gameObject)
            where TView : MonoBehaviour
        {
            return f.CreateInto(gameObject.transform);
        }

        public static TView CreateInto<TView, TParam>(this IFactory<TParam, TView> f, TParam t, GameObject gameObject)
            where TView : MonoBehaviour
        {
            return f.CreateInto(t, gameObject.transform);
        }

        public static TView CreateInto<TView>(this IFactory<TView> f, Transform transform)
            where TView : MonoBehaviour
        {
            var view = f.Create();
            transform.SetAsChild(view.transform);
            return view;
        }

        public static TView CreateInto<TView, TParam>(this IFactory<TParam, TView> f, TParam t, Transform transform)
            where TView : MonoBehaviour
        {
            var view = f.Create(t);
            transform.SetAsChild(view.transform);
            return view;
        }

        // some unnecessary fancyness to allow for a more literate programming style
        // eg: models.Select(myFactory.Into(transform).Create)
        private class FactoryWrapper<TArg, TOut> : IFactory<TArg, TOut> where TOut : MonoBehaviour
        {
            private readonly IFactory<TArg, TOut> _factory;
            private Transform _transform;

            public FactoryWrapper(IFactory<TArg, TOut> factory, Transform target)
            {
                _transform = target;
                _factory = factory;
            }

            public FactoryWrapper(IFactory<TArg, TOut> factory, GameObject target)
            {
                _transform = target.transform;
                _factory = factory;
            }

            public TOut Create(TArg param)
            {
                return _factory.CreateInto(param, _transform);
            }
        }

        public static IFactory<TParam, TView> Into<TView, TParam>(this IFactory<TParam, TView> self, Transform target)
            where TView : MonoBehaviour
        {
            return new FactoryWrapper<TParam, TView>(self, target);
        }

        public static IFactory<TParam, TView> Into<TView, TParam>(this IFactory<TParam, TView> self, GameObject target)
            where TView : MonoBehaviour
        {
            return new FactoryWrapper<TParam, TView>(self, target);
        }
    }
}