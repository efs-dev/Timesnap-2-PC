using System;
using UniRx;
using UniRx.Operators;

namespace Src.Scripts.Utils
{
    internal class CompliantThrottleObservable<T> : OperatorObservableBase<T>
    {
        private readonly IObservable<T> source;
        private readonly TimeSpan dueTime;
        private readonly IScheduler scheduler;

        public CompliantThrottleObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTime = dueTime;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new CompliantThrottle(this, observer, cancel).Run();
        }

        internal class CompliantThrottle : OperatorObserverBase<T, T>
        {
            readonly CompliantThrottleObservable<T> parent;
            SerialDisposable cancelable;

            public CompliantThrottle(CompliantThrottleObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancelable = new SerialDisposable();
                var subscription = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            public override void OnNext(T value)
            {
                throw new NotImplementedException();
            }

            public override void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public override void OnCompleted()
            {
                throw new NotImplementedException();
            }
        }
    }
}