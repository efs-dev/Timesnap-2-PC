using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EFS.Timesnap
{
    public class ClassCodeInputPanel : MonoBehaviour
    {
        public Button SubmitButton;
        public Button CancelButton;
        public TMP_Text MessageLabel;
        public TMP_InputField CodeInput;
        public GameObject MessageBox;

        public IObservable<ClassCodeData> Open(string message = "")
        {
            return Observable.Create<ClassCodeData>(observer =>
            {
                MessageBox.SetActive(message.Length != 0);
                MessageLabel.text = message;
                gameObject.SetActive(true);

                var disposable1 = CodeInput.ObserveEveryValueChanged(it => it.text)
                    .Select(ValidateCode)
                    .Subscribe(valid => SubmitButton.interactable = valid);

                var disposable2 = SubmitButton.OnClickAsObservable()
                    .Select(_ => new ClassCodeData {ClassCode = CodeInput.text})
                    .TakeUntil(CancelButton.OnClickAsObservable())
                    .Take(1)
                    .DoOnCompleted(() => gameObject.SetActive(false))
                    .Subscribe(observer);

                return new CompositeDisposable(disposable1, disposable2);
            });
        }

        private bool ValidateCode(string it)
        {
            // todo
            return it.Length > 5;
        }

        public class ClassCodeData
        {
            public string ClassCode;
        }
    }
}