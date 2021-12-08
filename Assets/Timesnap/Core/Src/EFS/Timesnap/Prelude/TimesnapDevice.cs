using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EFS.Timesnap.Data;
using JetBrains.Annotations;
using LitJson;
using UniRx;
using UnityEngine;
using WebSocketSharp;

namespace EFS.Timesnap
{
    public class TimesnapDevice : IDisposable
    {
        public IObservable<List<Classroom>> Classrooms => _classroomSubject.ObserveOnMainThread();
        public IObservable<List<Student>> Students => _studentSubject.ObserveOnMainThread();

        private readonly WebSocket _webSocket;
        private readonly ReplaySubject<List<Classroom>> _classroomSubject = new ReplaySubject<List<Classroom>>(1);
        private readonly ReplaySubject<List<Student>> _studentSubject = new ReplaySubject<List<Student>>(1);

        public TimesnapDevice(string deviceId)
        {
            const string url = "ws://192.168.1.24:8080/timesnap/api/realtime/v1/device?device_id=";
            _webSocket = new WebSocket(url + deviceId);
            _webSocket.OnMessage += WebSocketOnMessage;
            _webSocket.OnOpen += (sender, args) => Debug.Log($"TimesnapDevice ({deviceId}) connected to server");
            _webSocket.OnClose += (sender, args) => Debug.Log($"TimesnapDevice ({deviceId}) disconnected from server");
            Application.quitting += Dispose;
        }

        public void Connect()
        {
            _webSocket.Connect();
        }

        public void Dispose()
        {
            _webSocket.Close();
            _classroomSubject.Dispose();
            _studentSubject.Dispose();
            Application.quitting -= Dispose;
        }

        public void StartPlaying(Classroom classroom, Student student, int titleId)
        {
            var classroomId = classroom.id;
            var studentId = student.id;
            _webSocket.Send(JsonMapper.ToJson(new
            {
                type = "session_start",
                classroom_id = classroomId,
                student_id = studentId,
                title_id = titleId
            }));
        }

        public void StopPlaying(Classroom classroom, Student student, int titleId)
        {
            var classroomId = classroom.id;
            var studentId = student.id;
            _webSocket.Send(JsonMapper.ToJson(new
            {
                type = "session_stop",
                classroom_id = classroomId,
                student_id = studentId,
                title_id = titleId
            }));
        }

        public void SendProgressEvent(Classroom classroom, Student student, int titleId, string data)
        {
            var classroomId = classroom.id;
            var studentId = student.id;
            _webSocket.Send(JsonMapper.ToJson(new
            {
                type = "progress_event",
                classroom_id = classroomId,
                student_id = studentId,
                title_id = titleId,
                data
            }));
        }

        // private 
        private void WebSocketOnMessage(object sender, MessageEventArgs data)
        {
            try
            {
                var message = data.Data;
                var type = GetJsonType(message);
                Debug.Log("received websocket message of type " + type);
                Debug.Log("message was: " + message);
                RouteMessage(message, type);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Encountered error while processing websocket message: {0}", e);
                throw;
            }
        }

        private void RouteMessage(string message, string type)
        {
            try
            {
                switch (type)
                {
                    case "classroom_update":
                        ProcessClassroomUpdate(message);
                        break;
                    case "student_update":
                        ProcessStudentUpdate(message);
                        break;
                    default:
                        throw new Exception($"received message of unknown type '{type}': {message}");
                }
            }
            catch (JsonException e)
            {
                throw new Exception("JsonException while processing websocket message", e);
            }
        }

        private static string GetJsonType(string message)
        {
            var jsonObject = JsonMapper.ToObject(message);
            try
            {
                var type = jsonObject["type"].ToString();
                return type;
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("Bad websocket JSON message, missing required field 'type'");
            }
        }

        private void ProcessClassroomUpdate(string data)
        {
            var msg = JsonMapper.ToObject<ClassroomsUpdateMessage>(data);
            _classroomSubject.OnNext(msg.classrooms);
        }

        private void ProcessStudentUpdate(string data)
        {
            var msg = JsonMapper.ToObject<StudentUpdateMessage>(data);
            _studentSubject.OnNext(msg.students);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [UsedImplicitly]
        private class ClassroomsUpdateMessage
        {
            public List<Classroom> classrooms;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [UsedImplicitly]
        private class StudentUpdateMessage
        {
            public List<Student> students;
        }
    }
}