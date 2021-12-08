using System;
using System.Collections.Generic;
using System.Linq;
using EFS.Timesnap.Data;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EFS.Timesnap
{
    public class PickClassroomPanel : MonoBehaviour
    {
        public ClassroomPickList ClassroomPickList;
        public Button CancelButton;
        public TMP_Text MessageLabel;
        public GameObject MessageBox;

        public IObservable<Result> Open(IObservable<List<Classroom>> classroomObservable, string message = "")
        {
            return Observable.Create<Result>(observer =>
            {
                MessageBox.SetActive(message.Length != 0);
                MessageLabel.text = message;
                gameObject.SetActive(true);
                
                var classroomObservableSub = classroomObservable.Subscribe(classrooms =>
                {
                    ClassroomPickList.Data.Clear();
                    foreach (var classroom in classrooms)
                    {
                        ClassroomPickList.Data.Add(classroom);
                    }
                });

                var onPickSub = ClassroomPickList.OnPickAsObservable
                    .Take(1)
                    .Do(Close)
                    .Select(it => new Result {PickedClassroom = it})
                    .Subscribe(observer);

                var onCancelSub = CancelButton.OnClickAsObservable().Subscribe(_ =>
                {
                    gameObject.SetActive(false);
                    observer.OnNext(new Result {Cancelled = true});
                    observer.OnCompleted();
                });

                return new CompositeDisposable(onPickSub, classroomObservableSub, onCancelSub);
            });
        }

        public class Result
        {
            public Classroom PickedClassroom; // null if cancelled
            public bool Cancelled;
        }

        private void Close(Classroom classroom)
        {
            print($"picked classroom {classroom}");
            gameObject.SetActive(false);
        }
    }
}