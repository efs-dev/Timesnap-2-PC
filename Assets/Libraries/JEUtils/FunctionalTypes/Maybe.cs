using System;
using JetBrains.Annotations;

namespace JME.UnionTypes
{
    public static class Maybe
    {
        public static Maybe<T> Some<T>(T t)
        {
            return new Maybe<T>(t);
        }

        public static Maybe<T> None<T>()
        {
            return new Maybe<T>();
        }
    }

    public class Maybe<T>
    {
        private readonly T _t;
        private readonly bool _isSome;

        /// <summary>
        /// A Maybe containing a value. If passed a non-null argument, the result is Some(T).
        /// </summary>
        /// <param name="t">Some value to store in the Maybe</param>
        public Maybe(T t)
        {
            _isSome = t != null;
            _t = t;
        }

        /// <summary>
        /// A Maybe with no internal value. None().
        /// </summary>
        public Maybe()
        {
            _isSome = false;
        }

        /// <summary>
        /// True if the Maybe is Some(T). False otherwise.
        /// </summary>
        public bool IsSome
        {
            get { return _isSome; }
        }

        /// <summary>
        /// True if the Maybe is None(). False otherwise.
        /// </summary>
        public bool IsNone
        {
            get { return !IsSome; }
        }

        /// <summary>
        /// Pattern matches against the value of the Maybe.
        /// </summary>
        /// <param name="some">Called with the value if the Maybe is Some(T).</param>
        /// <param name="none">Called if the Maybe is None().</param>
        public void Match([InstantHandle] Action<T> some, [InstantHandle] Action none)
        {
            if (IsSome)
                some(_t);
            else
                none();
        }

        /// <summary>
        /// Pattern matches against the value of the Maybe, returning a result.
        /// </summary>
        /// <param name="some">If the Maybe is Some(T), this function is called and the result returned.</param>
        /// <param name="none">If the Maybe is None(), this function is called and the result returned.</param>
        /// <typeparam name="TReturn">The return type of both functions and of the Match method.</typeparam>
        /// <returns>The result of the matched function.</returns>
        public TReturn Match<TReturn>([InstantHandle] Func<T, TReturn> some, [InstantHandle] Func<TReturn> none)
        {
            return IsSome ? some(_t) : none();
        }

        /// <summary>
        /// Pattern matches against the value of the Maybe, returning a result.
        /// </summary>
        /// <param name="some">Returned if the Maybe is Some(T).</param>
        /// <param name="none">If the Maybe is None(), this function is called and the result returned.</param>
        /// <typeparam name="TReturn">The return type of the Match method.</typeparam>
        /// <returns>Either the value some if the Maybe is Some(T), or the result of the function none.</returns>
        public TReturn Match<TReturn>(TReturn some, [InstantHandle] Func<TReturn> none)
        {
            return IsSome ? some : none();
        }

        /// <summary>
        /// Pattern matches against the value of the Maybe, returning a result.
        /// </summary>
        /// <param name="some">If the Maybe is Some(T), this function is called and the result returned.</param>
        /// <param name="none">Returned if the Maybe is None().</param>
        /// <typeparam name="TReturn">The return type of the Match method.</typeparam>
        /// <returns>Either the result of the function some if the Maybe is Some(T), or the value none.</returns>
        public TReturn Match<TReturn>([InstantHandle] Func<T, TReturn> some, TReturn none)
        {
            return IsSome ? some(_t) : none;
        }

        /// <summary>
        /// Pattern matches against the value of the Maybe, returning a result.
        /// </summary>
        /// <param name="some">The value to return if the Maybe is Some(T).</param>
        /// <param name="none">The value to return if the Maybe is None().</param>
        /// <typeparam name="TReturn">The type of the return value.</typeparam>
        /// <returns>Either the value some if the Maybe is Some(T), or the value none.</returns>
        [Pure]
        public TReturn Match<TReturn>(TReturn some, TReturn none)
        {
            return IsSome ? some : none;
        }

        /// <summary>
        /// Applies a function to the value of the Maybe if it exists, then returns the result as a new Maybe.
        /// If the Maybe is None(), None() is returned.
        /// </summary>
        /// <param name="fn">Called if the Maybe is Some(T). The result is returned inside of a new Maybe.</param>
        /// <typeparam name="TConverted">The type of the resulting Maybe.</typeparam>
        /// <returns>Returns None() if the Maybe is None(), or Some(T) containing the result of the function otherwise.</returns>
        public Maybe<TConverted> Map<TConverted>([InstantHandle] Func<T, TConverted> fn)
        {
            return Match(
                some: x => new Maybe<TConverted>(fn(x)),
                none: () => new Maybe<TConverted>());
        }

        /// <summary>
        /// Converts a Maybe to a Result
        /// </summary>
        /// <param name="err">The value to use for Err in the result, if the Maybe is None().</param>
        /// <typeparam name="TErr">The type of the Err.</typeparam>
        /// <returns>A result containing either the value of Some(T) or the supplied Err value.</returns>
        [Pure]
        public Result<T, TErr> OkayOr<TErr>(TErr err)
        {
            return Match(
                some: Result<T, TErr>.Ok,
                none: () => Result<T, TErr>.Err(err));
        }

        /// <summary>
        /// Extract the value from a Some(T), or return a supplied default if None().
        /// </summary>
        /// <param name="defaultValue">The value to return if this Maybe is None().</param>
        /// <returns>Either the value of this Maybe if it is Some(T), otherwise the value supplied.</returns>
        [Pure]
        public T ValueOr(T defaultValue)
        {
            return IsSome ? _t : defaultValue;
        }

        /// <summary>
        /// Extract the value from a Some(T), or call the supplied function and return the result.
        /// </summary>
        /// <param name="action">A function to call if this Maybe is None().</param>
        /// <returns>Either the value of this Maybe if it is Some(T), otherwise the result of the function.</returns>
        public T ValueOr([InstantHandle] Func<T> action)
        {
            return IsSome ? _t : action();
        }

        /// <summary>
        /// Extract the value from a Some(T), or return the default for the type T.
        /// </summary>
        /// <returns>Either the value of the Maybe if it is Some(T), or the default for T.</returns>
        [Pure]
        [CanBeNull]
        public T OrDefault()
        {
            return IsSome ? _t : default(T);
        }

        /// <summary>
        /// Creates a None() of the same type as this Maybe.
        /// </summary>
        /// <returns>None().</returns>
        [Pure]
        public Maybe<T> AsNone()
        {
            return new Maybe<T>();
        }

        /// <summary>
        /// Creates a Some(T) of the same type as this Maybe.
        /// </summary>
        /// <param name="some">The value for the new Maybe.</param>
        /// <returns>Some(T).</returns>
        [Pure]
        public Maybe<T> AsSome(T some)
        {
            return new Maybe<T>(some);
        }

        /// <summary>
        /// Returns None() if this Maybe is None(), otherwise returns the supplied Maybe.
        /// </summary>
        /// <param name="other">The Maybe to return if this Maybe is Some(T).</param>
        /// <returns>Either the supplied Maybe or None().</returns>
        public Maybe<T> And(Maybe<T> other)
        {
            return Match(some: other, none: this);
        }

        /// <summary>
        /// Returns None() if the Maybe is None(), otherwise call the function and return the result.
        /// </summary>
        /// <param name="func">A function to call if this Maybe is Some(T).</param>
        /// <returns>Either the result of the function or None().</returns>
        public Maybe<T> AndThen(Func<T, Maybe<T>> func)
        {
            return Match(some: func, none: this);
        }

        /// <summary>
        /// Returns the Maybe if it is Some(T), otherwise returns the argument.
        /// </summary>
        /// <param name="other">The Maybe to return if this Maybe is None().</param>
        /// <returns>Either this Maybe or the argument.</returns>
        public Maybe<T> Or(Maybe<T> other)
        {
            return Match(some: this, none: other);
        }

        /// <summary>
        /// Returns the Maybe if it is Some(T), otherwise returns the result of the function.
        /// </summary>
        /// <param name="func">The function to call if the Maybe is None().</param>
        /// <returns>Either this Maybe or the result of the function.</returns>
        public Maybe<T> OrElse(Func<Maybe<T>> func)
        {
            return Match(some: this, none: func);
        }

        public override string ToString()
        {
            return string.Format("Maybe<{0}> [{1}]", typeof(T), IsSome ? string.Format("Some({0})", _t) : "None()");
        }
    }
}