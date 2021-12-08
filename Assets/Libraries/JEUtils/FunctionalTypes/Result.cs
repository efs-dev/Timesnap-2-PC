using System;
using JetBrains.Annotations;

namespace JME.UnionTypes
{
    public abstract class Result<TOk, TErr>
    {
        public abstract bool IsOk { get; }
        public abstract bool IsErr { get; }

        public static Result<TOk, TErr> Ok(TOk value)
        {
            return new ResultOk(value);
        }

        public static Result<TOk, TErr> Err(TErr value)
        {
            return new ResultErr(value);
        }

        public sealed class ResultOk : Result<TOk, TErr>
        {
            private readonly TOk _value;

            public override bool IsOk
            {
                get { return true; }
            }

            public override bool IsErr
            {
                get { return false; }
            }

            public ResultOk(TOk value)
            {
                _value = value;
            }

            public override void Match(Action<TOk> ok, Action<TErr> err)
            {
                ok(_value);
            }

            public override TReturn Match<TReturn>(Func<TOk, TReturn> ok, Func<TErr, TReturn> err)
            {
                return ok(_value);
            }

            public override Maybe<TOk> Ok()
            {
                return new Maybe<TOk>(_value);
            }

            public override Maybe<TErr> Err()
            {
                return new Maybe<TErr>();
            }

            public override Result<TConverted, TErr> Map<TConverted>(Func<TOk, TConverted> fn)
            {
                return Result<TConverted, TErr>.Ok(fn(_value));
            }

            public override Result<TOk, TConverted> MapErr<TConverted>(Func<TErr, TConverted> fn)
            {
                return Result<TOk, TConverted>.Ok(_value);
            }
        }

        public sealed class ResultErr : Result<TOk, TErr>
        {
            private readonly TErr _value;

            public override bool IsOk
            {
                get { return false; }
            }

            public override bool IsErr
            {
                get { return true; }
            }

            public ResultErr(TErr value)
            {
                _value = value;
            }

            public override void Match(Action<TOk> ok, Action<TErr> err)
            {
                err(_value);
            }

            public override TReturn Match<TReturn>(Func<TOk, TReturn> ok, Func<TErr, TReturn> err)
            {
                return err(_value);
            }

            public override Maybe<TOk> Ok()
            {
                return new Maybe<TOk>();
            }

            public override Maybe<TErr> Err()
            {
                return new Maybe<TErr>(_value);
            }

            public override Result<TConverted, TErr> Map<TConverted>(Func<TOk, TConverted> fn)
            {
                return Result<TConverted, TErr>.Err(_value);
            }

            public override Result<TOk, TConverted> MapErr<TConverted>(Func<TErr, TConverted> fn)
            {
                return Result<TOk, TConverted>.Err(fn(_value));
            }
        }

        public abstract void Match([InstantHandle] Action<TOk> ok, [InstantHandle] Action<TErr> err);

        public abstract TReturn Match<TReturn>([InstantHandle] Func<TOk, TReturn> ok,
            [InstantHandle] Func<TErr, TReturn> err);

        public abstract Maybe<TOk> Ok();

        public abstract Maybe<TErr> Err();

        public abstract Result<TConverted, TErr> Map<TConverted>(Func<TOk, TConverted> fn);

        public abstract Result<TOk, TConverted> MapErr<TConverted>(Func<TErr, TConverted> fn);
    }
}