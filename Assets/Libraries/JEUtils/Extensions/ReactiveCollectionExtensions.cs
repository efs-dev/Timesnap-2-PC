using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace UniRx
{
    public static class JEReactiveCollectionExtensions
    {
        /// <summary>
        /// Runs every item in source through the factory's create method, creating
        /// a new ReactiveCollection containing the result of factory.Create
        /// Keeps items in sync during add, remove, replace, and clear events
        /// One use case for this function is in keeping a collection of views in sync with a collection of models
        /// </summary>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        public static IReactiveCollection<TOut> SelectToCollection<TIn, TOut>(this IReactiveCollection<TIn> source, IFactory<TIn, TOut> factory)
        {
            return source.SelectToCollection(factory.Create);
        }
        
        /// <summary>
        /// Transforms an IReactiveCollection through a given function
        /// returning a new IReactiveCollection which should remain in
        /// sync with the original
        /// Keeps items in sync during add, remove, replace, and clear events
        /// </summary>
        public static IReactiveCollection<TOut> SelectToCollection<TIn, TOut>(this IReactiveCollection<TIn> source,
            Func<TIn, TOut> fn)
        {
            var converted = new ReactiveCollection<TOut>();
            source.ToObservable()
                .Select((element, index) => new CollectionAddEvent<TIn>(index, element))
                .Merge(source.ObserveAdd())
                .Subscribe(addEvent => converted.Insert(addEvent.Index, fn(addEvent.Value)));
            source.ObserveMove()
                .Subscribe(moveEvent => converted.Move(moveEvent.OldIndex, moveEvent.NewIndex));
            source.ObserveRemove()
                .Subscribe(removeEvent => converted.RemoveAt(removeEvent.Index));
            source.ObserveReplace()
                .Subscribe(replaceEvent => converted[replaceEvent.Index] = fn(replaceEvent.NewValue));
            source.ObserveReset()
                .Subscribe(_ => converted.Clear());
            return converted;
        }

        public static IDisposable SubscribeToCollection<T>(this IReactiveCollection<T> source, IReactiveCollection<T> target)
        {
            var disposables = new CompositeDisposable();
            source.ToObservable()
                .Select((element, index) => new CollectionAddEvent<T>(index, element))
                .Merge(source.ObserveAdd())
                .Subscribe(addEvent => { target.Insert(addEvent.Index, addEvent.Value); })
                .AddTo(disposables);
            source.ObserveRemove()
                .Subscribe(removeEvent => target.RemoveAt(removeEvent.Index))
                .AddTo(disposables);
            source.ObserveMove()
                .Subscribe(moveEvent => target.Move(moveEvent.OldIndex, moveEvent.NewIndex))
                .AddTo(disposables);
            source.ObserveReset()
                .Subscribe(resetEvent => target.Clear())
                .AddTo(disposables);
            source.ObserveReplace()
                .Subscribe(replaceEvent => target[replaceEvent.Index] = replaceEvent.NewValue)
                .AddTo(disposables);
            return disposables;
        }

        public static IDisposable SubscribeToCollection<T>(this IReactiveCollection<T> source, IList<T> target)
        {
            var disposables = new CompositeDisposable();
            source.ToObservable()
                .Select((element, index) => new CollectionAddEvent<T>(index, element))
                .Merge(source.ObserveAdd())
                .Subscribe(addEvent => { target.Insert(addEvent.Index, addEvent.Value); })
                .AddTo(disposables);
            source.ObserveRemove()
                .Subscribe(removeEvent => target.RemoveAt(removeEvent.Index))
                .AddTo(disposables);
            source.ObserveMove()
                .Subscribe(moveEvent =>
                {
                    var item = target[moveEvent.OldIndex];
                    target.RemoveAt(moveEvent.OldIndex);
                    target.Insert(moveEvent.NewIndex, item);
                })
                .AddTo(disposables);
            source.ObserveReset()
                .Subscribe(resetEvent => target.Clear())
                .AddTo(disposables);
            source.ObserveReplace()
                .Subscribe(replaceEvent => target[replaceEvent.Index] = replaceEvent.NewValue)
                .AddTo(disposables);
            return disposables;
        }
   }
}