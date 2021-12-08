using UnityEngine;

namespace EFS.Timesnap.Note
{
    public class Note : MonoBehaviour
    {
        public string Author;
        public string Title;
        [TextArea] public string Content;
    }
}