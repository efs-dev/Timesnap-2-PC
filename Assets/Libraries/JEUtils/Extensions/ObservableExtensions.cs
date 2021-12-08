using System;
using City.Utils;
using JME.UnionTypes;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

namespace UniRx
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Shorthand for source.SkipWhile(x => x == null);
        /// </summary>
        /// <param name="source">The source observable</param>
        /// <typeparam name="T">The type of the observable</typeparam>
        /// <returns>An observable which skips null items until a non-null item is encountered</returns>
        public static IObservable<T> SkipNull<T>(this IObservable<T> source)
        {
            return source.SkipWhile(x => x == null);
        }

        /// <summary>
        /// Creates an IObservable which contains values which have been run through string.Format
        /// using the emitted item as the first format argument "{0}", and the varargs as the remaining
        /// format arguments
        /// </summary>
        /// <param name="source">An input observable whose emitted items will be mapped to {0} in the formatted string</param>
        /// <param name="formatString">A format string to be passed to string.Format</param>
        /// <param name="args">the format args to fill out {1}..{n}</param>
        /// <typeparam name="T">The type of the input observable</typeparam>
        /// <returns>An observable of formatted strings</returns>
        public static IObservable<string> Format<T>(this IObservable<T> source, string formatString,
            params object[] args)
        {
            return source.Select(x => string.Format(formatString, x, args));
        }

        /// <summary>
        /// Creates an IObservable which contains only the Some values of a stream of Maybes
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T">The type of the Maybe</typeparam>
        /// <returns>An IObserable of values</returns>
        public static IObservable<T> WhereSome<T>(this IObservable<Maybe<T>> source)
        {
            return source.Where(maybe => maybe.IsSome).Select(maybe => maybe.OrDefault());
        }

        /// <summary>
        /// a shorthand for Throttle(TimeSpan.FromSeconds(seconds)) 
        /// </summary>
        public static IObservable<T> ThrottleSeconds<T>(this IObservable<T> source, double seconds)
        {
            return source.Throttle(TimeSpan.FromSeconds(seconds));
        }

        //  
        /// <summary>
        /// Special boolean overload for subscribe
        /// Ex: myBooleanObservable.Subscribe(() => print("true! :)"), () => print("false! :("))
        /// </summary>
        /// <param name="source">An observable of booleans</param>
        /// <param name="_true">Function to be called if true is emitted</param>
        /// <param name="_false">Function to be called if false is emitted</param>
        public static IDisposable Subscribe(this IObservable<bool> source, Action _true, Action _false)
        {
            return source.Subscribe(b =>
            {
                if (b)
                    _true();
                else
                    _false();
            });
        }

        /// <summary>
        /// Literate shortcut for filtering IObservables of IComparables 
        /// ex: myBoolObservable.Where(true).Subscribe(...)
        /// ex: myIntObservable.Where(10).Subscribe(...)
        /// </summary>
        public static IObservable<T> Where<T>(this IObservable<T> source, T desired) where T : IComparable
        {
            return source.Where(val => val.CompareTo(desired) == 0);
        }

        /// <summary>
        /// Shorthand for source.Select(b => !b);
        /// </summary>
        public static IObservable<bool> Not(this IObservable<bool> source)
        {
            return source.Select(b => !b);
        }

        public static IObservable<PointerEventData> OnPointerDownAsObservable<T>(this T component)
            where T : Component, IPointerDownHandler
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return component.GetOrAddComponent<ObservablePointerDownTrigger>().OnPointerDownAsObservable();
        }

        /// <summary>
        /// Shorthand for someObservable.Where(it => it != null) 
        /// Also filters out destroyed UnityEngine.Object instances (which compare truely with null)
        /// </summary>
        public static IObservable<T> WhereNotNull<T>(this IObservable<T> self) where T : class
        {
            // Here C# doesn't use the overloaded version of !=, but does for .Equals
            return self.Where(it => it != null && !it.Equals(null));
        }

        public static IObservable<T> RateLimit<T>(this IObservable<T> source, TimeSpan minDelay)
        {
            return source.Select(x =>
                Observable.Empty<T>()
                    .Delay(minDelay)
                    .StartWith(x)
            ).Concat();
        }
    }

    public static class JEObservable
    {
        /// <summary>
        /// Shorthand for Observable.Timer(TimeSpan.FromSeconds(seconds))
        /// </summary>
        /// <param name="seconds">The number of seconds this timer should last</param>
        /// <returns>An IObservable&amp;long&amp; which emits when the timer completes</returns>
        public static IObservable<long> TimerFromSeconds(double seconds)
        {
            return Observable.Timer(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Shortcut for Observable.Interval(TimeSpan.FromSeconds(seconds))
        /// </summary>
        /// <param name="seconds">The number of seconds this interval should wait between emissions</param>
        /// <returns>An IObservable which emits every `seconds` seconds</returns>
        public static IObservable<long> IntervalFromSeconds(double seconds)
        {
            return Observable.Interval(TimeSpan.FromSeconds(seconds));
        }
    }
}