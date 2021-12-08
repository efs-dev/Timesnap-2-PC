using UnityEngine;

namespace EFS.Timesnap
{
    [CreateAssetMenu]
    public class TimesnapPluginData : ScriptableObject
    {
        public string APIBase = "localhost:8080/api/v1";
        public string WebsocketProtocol = "ws"; // ws or wss
        public object HttpProtocol = "http"; // http or https
    }
}