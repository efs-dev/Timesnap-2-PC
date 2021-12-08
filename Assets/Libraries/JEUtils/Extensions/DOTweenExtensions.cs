using System;
using DG.Tweening;
using UniRx;

namespace Assets.Src.Utils
{
    public static class DOTweenExtensions
    {
        public static IObservable<Unit> ToObservable(this Tween self)
        {
            return Observable.Create<Unit>(observer =>
            {
                var oldOnComplete = self.onComplete;
                self.OnComplete(() =>
                {
                    oldOnComplete?.Invoke();
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

                return Disposable.Create(() => { self.Kill(); });
            });
        }
    }
}