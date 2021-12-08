using System;
using System.Collections.Generic;
using System.Linq;
using EFS.Timesnap.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EFS.Timesnap
{
    public class PickStudentPanel : MonoBehaviour
    {
        public StudentPickList StudentPickList;
        public Button CancelButton;

        public IObservable<Result> Open(IObservable<List<Student>> studentObservable, Classroom classroom)
        {
            return Observable.Create<Result>(observer =>
            {
                gameObject.SetActive(true);
                var studentObservableSub = studentObservable.Subscribe(students =>
                {
                    StudentPickList.Data.Clear();
                    foreach (var student in students.Where(student => student.classroom == classroom.id))
                    {
                        StudentPickList.Data.Add(student);
                    }
                });

                var pickListSub = StudentPickList.OnPickAsObservable
                    .Take(1)
                    .Do(Close)
                    .Select(student => new Result
                    {
                        Student = student,
                        Classroom = classroom
                    })
                    .Subscribe(observer);

                var onCancelSub = CancelButton.OnClickAsObservable().Subscribe(_ =>
                {
                    gameObject.SetActive(false);
                    observer.OnNext(new Result {Cancelled = true});
                    observer.OnCompleted();
                });

                return new CompositeDisposable(pickListSub, studentObservableSub, onCancelSub);
            });
        }

        public class Result
        {
            public Student Student; // null if cancelled
            public Classroom Classroom; // ditto
            public bool Cancelled;
        }

        private void Close(Student student)
        {
            print($"picked student: {student}");
            gameObject.SetActive(false);
        }
    }
}