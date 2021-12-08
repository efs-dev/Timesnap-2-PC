using LitJson;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace EFS.Timesnap
{
    public class FakeGamePanel : MonoBehaviour
    {
        public TMP_InputField InputField;
        public Button SendEventButton;

        public void Open(PickStudentPanel.Result stuff, TimesnapDevice device)
        {
            gameObject.SetActive(true);
            SendEventButton.OnClickAsObservable().Subscribe(_ =>
            {
                device.SendProgressEvent(stuff.Classroom, stuff.Student, 2, InputField.text);
            });
        }
    }
}