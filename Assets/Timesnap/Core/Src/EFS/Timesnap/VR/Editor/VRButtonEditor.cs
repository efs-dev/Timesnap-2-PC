using UnityEditor;

[CustomEditor(typeof(VRButton))]
[CanEditMultipleObjects]
public class VRButtonEditor : Editor
{
    private SerializedProperty _showTooltipOnRolloverProp;
    private SerializedProperty _tooltipTextProp;
    private SerializedProperty _scaleOnHoverProp;
    private SerializedProperty _hoverScaleConfigProp;

    private void OnEnable()
    {
        _showTooltipOnRolloverProp = serializedObject.FindProperty("ShowTooltipOnRollover");
        _tooltipTextProp = serializedObject.FindProperty("TooltipText");
        
        _scaleOnHoverProp = serializedObject.FindProperty("ScaleOnHover");
        _hoverScaleConfigProp = serializedObject.FindProperty("HoverScaleConfig");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var prop = serializedObject.GetIterator();
        prop.Next(true);

        while (prop.NextVisible(false))
        {
            if (SerializedProperty.EqualContents(prop, _tooltipTextProp)
                && _showTooltipOnRolloverProp.boolValue == false)
            {
                continue;
            }
            
            if (SerializedProperty.EqualContents(prop, _hoverScaleConfigProp)
                && _scaleOnHoverProp.boolValue == false)
            {
                continue;
            }

            EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}