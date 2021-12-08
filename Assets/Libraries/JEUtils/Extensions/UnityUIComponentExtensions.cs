using System;
using System.Collections.Generic;
using TMPro;

namespace UniRx
{
    public static partial class UnityUIComponentExtensions
    {
        /// <summary>
        /// Sets the value of the text field whenever a string is emitted from the observable
        /// </summary>
        /// <param name="source"></param>
        /// <param name="text">The TMP_Text instance to update whenever the source emits a string</param>
        /// <returns></returns>
        public static IDisposable SubscribeToText(this IObservable<string> source, TMP_Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }
        
        public static IDisposable SubscribeToText<T>(this IObservable<T> self, TMP_Text text, Func<T, string> selector)
        {
            return self.Select(selector).SubscribeToText(text);
        }
    }
}

