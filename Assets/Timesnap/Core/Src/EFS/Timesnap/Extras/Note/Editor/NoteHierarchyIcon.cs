using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EFS.Timesnap.Note.Editor
{
    [InitializeOnLoad]
    public class NoteHierarchyIcon
    {
        private static readonly Texture2D Texture;
        private static HashSet<int> _markedObjects = new HashSet<int>();

        static NoteHierarchyIcon()
        {
            Texture = Resources.Load<Texture2D>("NoteIcon");
            EditorApplication.update += EditorUpdate;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
        }

        private static void EditorUpdate()
        {
            var notes = Resources.FindObjectsOfTypeAll<Note>();
            var noteInstanceIds = notes.Select(note => note.gameObject.GetInstanceID());
            _markedObjects = new HashSet<int>(noteInstanceIds);
        }

        private static void HierarchyWindowItemOnGui(int instanceId, Rect selectionRect)
        {
            // place the icon to the right of the list:
            var r = new Rect(selectionRect);
            r.x = r.xMax-18;
            r.width = 18;

            if (_markedObjects.Contains(instanceId))
            {
                GUI.Label(r, Texture);
            }
        }
    }
}