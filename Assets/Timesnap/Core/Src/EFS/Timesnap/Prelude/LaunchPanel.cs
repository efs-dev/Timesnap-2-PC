using System;
using System.Collections.Generic;
using LitJson;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EFS.Timesnap
{
    public class LaunchPanel : MonoBehaviour
    {
        public TimesnapPluginData Data;
        public Button TeacherSignupButton;
        public Button ClassCodeButton;
        public Button StartGameDirectlyButton;
        public TeacherSignupPanel TeacherSignupPanel;
        public ClassCodeInputPanel ClassCodeInputPanel;
        public PickStudentPanel PickStudentPanel;
        public PickClassroomPanel PickClassroomPanel;
        public DevicePickerContainer DevicePickerContainer;
        public FakeGamePanel FakeGamePanel;
        const string APIbase = "http://192.168.1.24:8080/timesnap/api";
        public TMP_Text DeviceIDLabel;

        public List<Tuple<string, Color>> DeviceIDS = new List<Tuple<string, Color>>
        {
            new Tuple<string, Color>("08538002-170D-4DCA-B63A-59E1119F479F", new Color(1f, 0.33f, 0.11f)),
            new Tuple<string, Color>("55230748-3334-498F-AB18-FE6E43A41239", new Color(0.66f, 1f, 0.13f)),
            new Tuple<string, Color>("1D4B89A2-A9EF-4535-93CE-B3D9ED87685D", new Color(0.35f, 0.26f, 1f)),
            new Tuple<string, Color>("68D44341-8BF3-41F0-B2D2-D4EEBDB15609", new Color(0.4f, 1f, 0.98f)),
        };

        public StringReactiveProperty DeviceId = new StringReactiveProperty();

        private TimesnapDevice _device;

        void Start()
        {
            // todo: ???
            QualitySettings.antiAliasing = 2;

            TeacherSignupButton.onClick.AddListener(PromptForTeacherSignup);
            ClassCodeButton.onClick.AddListener(() => PromptForClassCode());
            StartGameDirectlyButton.onClick.AddListener(StartTheGame);

            DevicePickerContainer.Picks.Subscribe(it =>
            {
                DeviceId.Value = it.Item1;
                DeviceIDLabel.text = it.Item1;
                DeviceIDLabel.color = it.Item2;
            });
            SetupDevicePicker();

            // an issue: after the teacher registers on the website, our device is connected
            // but doesn't show status: online
            DeviceId.Subscribe(deviceid =>
            {
                _device?.Dispose();
                print("got change to deviceId, creating TimesnapDevice");
                _device = new TimesnapDevice(deviceid);
                _device.Classrooms.Subscribe(classrooms =>
                {
                    print("observed classrooms: " + JsonMapper.ToJson(classrooms));
//                    PickStudentPanel.StudentPickList.Data
                });
                _device.Students.Subscribe(students =>
                    print("observed students: " + JsonMapper.ToJson(students)));
                _device.Connect();
            });
        }

        private void SetupDevicePicker()
        {
            DevicePickerContainer.Clear();
            foreach (var idColorPair in DeviceIDS)
            {
                var id = idColorPair.Item1;
                var color = idColorPair.Item2;
                DevicePickerContainer.Add(id, color);
            }
        }

        private void StartTheGame()
        {
            StartTheGame("");
        }

        private void StartTheGame(string message)
        {
            gameObject.SetActive(false);

            // todo: handle _device change
            PickClassroomPanel.Open(_device.Classrooms, message)
                .SelectMany(result =>
                {
                    if (!result.Cancelled)
                    {
                        return PickStudentPanel.Open(_device.Students, result.PickedClassroom);
                    }

                    gameObject.SetActive(true);
                    return Observable.Empty<PickStudentPanel.Result>();
                })
                .Subscribe(result =>
                {
                    if (result.Cancelled)
                    {
                        gameObject.SetActive(true);
                    }
                    else
                    {
                        _device.StartPlaying(result.Classroom, result.Student, 2);
                        FakeGamePanel.Open(result, _device);
                    }
                }, onCompleted: () => print("pick-flow completed"))
                .AddTo(this);
            print("start the game...");
        }

        private void PromptForClassCode(string message = "")
        {
            gameObject.SetActive(false);
            ClassCodeInputPanel.Open(message).Subscribe(result =>
            {
                print("got class code: " + result.ClassCode);
                AuthenticateWithClassCode(result.ClassCode)
                    .Subscribe(_ => { DeviceId.SetValueAndForceNotify(DeviceId.Value); });
            }, onCompleted: () => gameObject.SetActive(true));
        }

        private void PromptForTeacherSignup()
        {
            gameObject.SetActive(false);
            TeacherSignupPanel.Open().Subscribe(signup =>
                {
                    print("got teacher email: " + signup.EmailAddress);
                    SubmitEmailToServer(signup.EmailAddress);
                },
                onCompleted: () =>
                {
//                    PromptForClassCode("you should receive an email with instructions on how to get your class code.");
//                    gameObject.SetActive(true);
                    StartTheGame("Take off the headset and check your email for further instructions");
                });
        }


        private void SubmitEmailToServer(string email)
        {
            var url = $"{APIbase}/v1/send-teacher-invite";
            var form = new WWWForm();
            form.AddField("email", email);
            form.AddField("device_id", DeviceId.Value);
            form.AddField("device_kind", "dev");
            ObservableWWW.Post(url, form).Subscribe();
        }

        private IObservable<Unit> AuthenticateWithClassCode(string classCode)
        {
            return Observable.Create<Unit>(observer =>
            {
                var url = $"{APIbase}/v1/device/register";
                var form = new WWWForm();
                form.AddField("class_code", classCode);
                form.AddField("device_id", DeviceId.Value);
                form.AddField("device_kind", "dev");
                return ObservableWWW.Post(url, form).DoOnError(err =>
                {
                    var wwwError = err as WWWErrorException;
                    if (wwwError != null)
                    {
                        Debug.LogError($"WWWError: {wwwError.StatusCode}");
                    }
                    else
                    {
                        Debug.LogError("Unknown Error: " + err.ToString());
                    }
                }).AsUnitObservable().Subscribe(observer);
            });
        }
    }
}