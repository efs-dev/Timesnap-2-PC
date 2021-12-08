using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EFS.Timesnap
{
    public class TeacherSignupPanel : MonoBehaviour
    {
        public Button SubmitButton;
        public Button CancelButton;
        public TMP_InputField EmailInput;

        public IObservable<TeacherSignupDetails> Open()
        {
            return Observable.Create<TeacherSignupDetails>(observer =>
            {
                gameObject.SetActive(true);
                
                var disposable1 = EmailInput.ObserveEveryValueChanged(it => it.text)
                    .Select(ValidateEmail)
                    .Subscribe(valid => SubmitButton.interactable = valid);

                var disposable2 = SubmitButton.OnClickAsObservable()
                    .Select(_ => new TeacherSignupDetails {EmailAddress = EmailInput.text})
                    .TakeUntil(CancelButton.OnClickAsObservable())
                    .Take(1)
                    .DoOnCompleted(() => gameObject.SetActive(false))
                    .Subscribe(observer);

                return new CompositeDisposable(disposable1, disposable2);
            });
        }

        private static bool ValidateEmail(string str)
        {
            // lol
            return str.Contains("@") && str.Contains(".");
        }

        public class TeacherSignupDetails
        {
            public string EmailAddress;
        }
    }
}