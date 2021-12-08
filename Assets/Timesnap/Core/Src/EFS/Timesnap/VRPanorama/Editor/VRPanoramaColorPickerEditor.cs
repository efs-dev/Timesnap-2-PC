using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Linq;
using System.IO;

public class VRPanoramaColorPickerEditor : EditorWindow {

    private VRPanoramaColorPicker _target;

    private Texture2D _cursor;

    private Vector2 _scrollPos;

    //James Did it!
    //[UnityEditor.Callbacks.DidReloadScripts]
    void OnEnable()
    {

        EditorApplication.update -= Repaint;
        EditorApplication.update += Repaint;
    }

    void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    [MenuItem("VRPanorama/Debug/VRPanoramaDepth", priority = 105)]
    static void Init()
    {
        var window = EditorWindow.GetWindow<VRPanoramaColorPickerEditor>();
        window.titleContent = new GUIContent("View Depth");
    }
    

    public void OnGUI()
    {
        _target = FindObjectOfType<VRPanoramaDepth>();
        if (_target == null)
            return;

        if (_cursor == null)
        {
            _cursor = new Texture2D(5, 5);

            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    _cursor.SetPixel(x, y, Color.clear);
                }
            }

            // black cursor
            _cursor.SetPixel(3, 2, Color.black);
            _cursor.SetPixel(3, 3, Color.black);
            _cursor.SetPixel(3, 4, Color.black);
            _cursor.SetPixel(2, 3, Color.black);
            _cursor.SetPixel(4, 3, Color.black);
            /*
            // white border
            _cursor.SetPixel(3, 1, Color.white);
            _cursor.SetPixel(3, 5, Color.white);
            _cursor.SetPixel(1, 3, Color.white);
            _cursor.SetPixel(5, 3, Color.white);

            _cursor.SetPixel(2, 1, Color.white);
            _cursor.SetPixel(2, 2, Color.white);
            _cursor.SetPixel(2, 4, Color.white);
            _cursor.SetPixel(2, 5, Color.white);

            _cursor.SetPixel(4, 1, Color.white);
            _cursor.SetPixel(4, 2, Color.white);
            _cursor.SetPixel(4, 4, Color.white);
            _cursor.SetPixel(4, 5, Color.white);

            _cursor.SetPixel(1, 2, Color.white);
            _cursor.SetPixel(2, 2, Color.white);
            _cursor.SetPixel(4, 2, Color.white);
            _cursor.SetPixel(5, 2, Color.white);

            _cursor.SetPixel(1, 4, Color.white);
            _cursor.SetPixel(2, 4, Color.white);
            _cursor.SetPixel(4, 4, Color.white);
            _cursor.SetPixel(5, 4, Color.white);
            */
            _cursor.Apply();
        }


        var size = Screen.width * 0.8f;

        if (Application.isPlaying)
        {

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(Screen.width));
            Debug.Log(_target.DebugData.Count);
            for (var i = 0; i < _target.DebugData.Count; i++)
            {
                var debugData = _target.DebugData[i];

                if (debugData.Texure != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    var rect = EditorGUILayout.GetControlRect(GUILayout.Width(size), GUILayout.Height(size));




                    /*var texture = new Texture2D(debugData.Texure.width, debugData.Texure.height);
                    texture.SetPixels(0, 0, debugData.Texure.width, debugData.Texure.height, debugData.Texure.GetPixels(0, 0, debugData.Texure.width, debugData.Texure.height));
                    texture.Apply();
                    texture.SetPixel((int)debugData.Pixel.x, (int)debugData.Pixel.y, Color.blue);
                    texture.Apply();
                    GUI.DrawTexture(rect, texture);*/

                    //var origPixel = debugData.Texure.GetPixel((int)debugData.Pixel.x, (int)debugData.Pixel.y);
                    // debugData.Texure.SetPixel((int)debugData.Pixel.x, (int)debugData.Pixel.y, Color.blue);
                    // debugData.Texure.Apply();
                    GUI.DrawTexture(rect, debugData.Texure);
                   // debugData.Texure.SetPixel((int)debugData.Pixel.x, (int)debugData.Pixel.y, origPixel);
                   // debugData.Texure.Apply();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Size: (" + debugData.Texure.width+ ", " + debugData.Texure.height + ")");
                    EditorGUILayout.LabelField("Coordinates: (" + debugData.Pixel.x + ", " + debugData.Pixel.y + ")");
                    EditorGUILayout.LabelField("Global UV: (" + debugData.UV.x + ", " + debugData.UV.y + ")");
                    EditorGUILayout.LabelField("Texture UV: (" + debugData.TextureUV.x + ", " + debugData.TextureUV.y + ")");
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();


                    var ratioX = (float)size / (float)debugData.Texure.width;
                    var ratioY = (float)size / (float)debugData.Texure.height;

                    var px = debugData.Pixel.x * ratioX;
                    var py = debugData.Pixel.y * ratioY * -1;

                    var cursorRect = new Rect(rect.x + px - 2.5f, rect.y + py - 2.5f, 5, 5);
                    GUI.DrawTexture(cursorRect, _cursor);
                    

                }
                else
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(size));
                    GUILayout.Space(size);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        


    }

    void Slider(Rect position, SerializedProperty property, float leftValue, float rightValue, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();
        var newValue = EditorGUI.Slider(position, label, property.floatValue, leftValue, rightValue);
        // Only assign the value back if it was actually changed by the user.
        // Otherwise a single value will be assigned to all objects when multi-object editing,
        // even when the user didn't touch the control.
        if (EditorGUI.EndChangeCheck())
        {
            property.floatValue = newValue;
        }
        EditorGUI.EndProperty();
    }

    private void DrawPropertyArray(SerializedProperty property, ref bool fold)
    {
        fold = EditorGUILayout.Foldout(fold, property.displayName);
        if (fold)
        {
            EditorGUI.indentLevel++;
            SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(arraySizeProp);

            EditorGUI.indentLevel++;

            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
            }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
    }

    void ArrayGUI(SerializedProperty property)
    {
        SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");
        EditorGUILayout.PropertyField(arraySizeProp);

        EditorGUI.indentLevel++;

        for (int i = 0; i < arraySizeProp.intValue; i++)
        {
            EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
        }

        EditorGUI.indentLevel--;
    }


}
