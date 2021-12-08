using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Src.Utils
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns either the value of the dictionary at `key` or if the key is not
        /// in the dictionary, return the value provided as `@default`
        /// </summary>
        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> self, TKey key,
            TVal @default = default(TVal))
        {
            try
            {
                return self[key];
            }
            catch (KeyNotFoundException _)
            {
                return @default;
            }
        }

        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> self, TKey key, Func<TVal> @default)
        {
            return self.GetOrDefault(key, @default());
        }

        /// <summary>
        /// Passes [either the value at `key` or the default for that type] through the function `fn` and stores the result
        /// at `key`
        /// </summary>
        public static TVal UpdateOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> self, TKey key, Func<TVal, TVal> fn)
        {
            var originalValue = self.GetOrDefault(key);
            var updatedValue = fn(originalValue);
            self[key] = updatedValue;
            return updatedValue;
        }

        public static TVal UpdateOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> self, TKey key, Func<TVal> @default,
            Func<TVal, TVal> fn)
        {
            var originalValue = self.GetOrDefault(key, @default());
            var updatedValue = fn(originalValue);
            self[key] = updatedValue;
            return updatedValue;
        }

        /// <summary>
        /// Select over a Dictionary using separate key and value functions
        /// </summary>
        public static IDictionary<TKeyOut, TValOut> Select<TKeyIn, TValIn, TKeyOut, TValOut>(
            this IDictionary<TKeyIn, TValIn> self,
            Func<TKeyIn, TKeyOut> keyFn,
            Func<TValIn, TValOut> valFn)
        {
            var output = new Dictionary<TKeyOut, TValOut>();
            foreach (var pair in self)
            {
                output[keyFn(pair.Key)] = valFn(pair.Value);
            }

            return output;
        }

        /// <summary>
        /// Shorthand for someEnumerable.Select(...).Where(it => it != null)
        /// Also filters out destroyed UnityEngine.Object instances (which compare truely with null)
        /// </summary>
        public static IEnumerable<TOut> SelectNotNull<TIn, TOut>(this IEnumerable<TIn> self, Func<TIn, TOut> fn)
            where TOut : class
        {
            return self.Select(fn).WhereNotNull();
        }

        /// <summary>
        /// Shorthand for someEnumerable.Where(it => it != null) 
        /// Also filters out destroyed UnityEngine.Object instances (which compare truely with null)
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> self) where T : class
        {
            // Here C# doesn't use the overloaded version of !=, but does for .Equals
            return self.Where(it => it != null && !it.Equals(null));
        }

        /// <summary>
        /// Convenience method to convert a list to a string
        /// </summary>
        public static string JoinToString<T>(this IEnumerable<T> self, string separator = ",")
        {
            return string.Join(separator, self);
        }

        /// <summary>
        /// Pick a random element from self or if self is empty, return default(T)
        /// </summary>
        /// <param name="self">the list to pick from</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>an element from self, or default(T) if no elements exist</returns>
        public static T PickRandomOrDefault<T>(this IList<T> self)
        {
            if (self.Count == 0)
                return default(T);
            return self[UnityEngine.Random.Range(0, self.Count)];
        }

        /// <summary>
        /// Zip two IEnumerables into a single IEnumerable of 2-tuples
        /// Convenience method for when you're too lazy to specify the resultSelector
        /// </summary>
        public static IEnumerable<Tuple<T, U>> Zip<T, U>(this IEnumerable<T> self, IEnumerable<U> other)
        {
            return self.Zip(other, (t, u) => new Tuple<T, U>(t, u));
        }

        /// <summary>
        /// Repeat all elements from self n times
        /// </summary>
        /// <param name="self"></param>
        /// <param name="times"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> self, int times)
        {
            return Enumerable.Repeat(self, times).FlattenOnce();
        }

        /// <summary>
        /// Just flatten one level
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> FlattenOnce<T>(this IEnumerable<IEnumerable<T>> self)
        {
            return self.SelectMany(it => it);
        }

        /// <summary>
        /// GroupsOf yields Lists of size groupSize or less until the source is exhausted
        /// </summary>
        /// <param name="self">the source</param>
        /// <param name="groupSize">the maximum size of each list</param>
        /// <typeparam name="T">The type of the IEnumerable</typeparam>
        /// <returns>A new IEnumerable yielding Lists where each List is `groupSize` consecutive elements
        /// from self, or less if there are less than `groupSize` items remaining in self</returns>
        public static IEnumerable<IEnumerable<T>> GroupsOf<T>(this IEnumerable<T> self, int groupSize)
        {
            var activeList = new List<T>();
            foreach (var x in self)
            {
                activeList.Add(x);
                if (activeList.Count >= groupSize)
                {
                    yield return activeList;
                    activeList = new List<T>();
                }
            }

            if (activeList.Count > 0) yield return activeList;
        }

        public static Vector3 Sum(this IEnumerable<Vector3> self)
        {
            return self.Aggregate(new Vector3(), (acc, x) => acc + x);
        }

        public static Vector3 Average(this IEnumerable<Vector3> self)
        {
            var n = 0;
            var ret = new Vector3();
            foreach (var x in self)
            {
                n++;
                ret += x;
            }

            return ret / n;
        }

        public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> self)
        {
            var previous = default(T);
            var first = true;
            foreach (var x in self)
            {
                if (first)
                {
                    first = false;
                    previous = x;
                    continue;
                }

                yield return new Tuple<T, T>(previous, x);
                previous = x;
            }
        }

        /// <summary>
        /// Shuffle the list in place, returns self for chaining
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<T> InPlaceShuffle<T>(this IList<T> self)
        {
            for (var i = 0; i < self.Count - 1; i++)
            {
                var j = Random.Range(i, self.Count);
                var tmp = self[i];
                self[i] = self[j];
                self[j] = tmp;
            }

            return self;
        }
    }
}