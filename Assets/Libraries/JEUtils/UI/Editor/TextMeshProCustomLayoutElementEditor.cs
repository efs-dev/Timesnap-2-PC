using UnityEditor;
using UnityEngine.UI;

namespace Assets.Src.Utils.UI
{
    [CustomEditor(typeof(TextMeshProCustomLayoutElement))]
    public class TextMeshProCustomLayoutElementEditor : Editor
    {
        private ILayoutElement _t;

        void OnEnable()
        {
            _t = (ILayoutElement) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("minWidth: " + _t.minWidth);
            EditorGUILayout.LabelField("minHeight: " + _t.minHeight);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("preferredWidth: " + _t.preferredWidth);
            EditorGUILayout.LabelField("preferredHeight: " + _t.preferredHeight);
            EditorGUILayout.EndHorizontal();
        }
    }
}