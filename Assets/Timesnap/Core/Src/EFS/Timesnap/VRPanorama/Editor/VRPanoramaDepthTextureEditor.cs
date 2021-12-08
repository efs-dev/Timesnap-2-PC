using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class VRPanoramaDepthTextureEditor : EditorWindow {

    [MenuItem("VRPanorama/Depth Texture Merger")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<VRPanoramaDepthTextureEditor>();
        window.titleContent = new GUIContent("DT Merger");
    }

    private Texture2D Left;
    private Texture2D Right;

    private float BridgeX = 1;
    public float ExpandY = 0;

    public string OutputName;
    public int OutputWidth = 2048;
    public int OutputHeight = 2048;

    private CustomRenderTexture _cmt;

    private Vector2 _scrollPos;
    private bool _fullPreview;


    private Texture2D PreviewLeft;
    private Texture2D PreviewRight;
    private enum EyeTypes { Left, Right };
    private EyeTypes EyeType;

    private float PreviewDepthAlpha = 1;

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUI.enabled = Left != null && Right != null;
        if (GUILayout.Button(_fullPreview ? "Edit Mode" : "Preview Mode", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            _fullPreview = !_fullPreview;
        }
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        if (!_fullPreview)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
            Left = (Texture2D)EditorGUILayout.ObjectField(Left != null ? "Left Eye (" + Left.width + "x" + Left.height + ")" : "Left Eye", Left, typeof(Texture2D), false);
            Right = (Texture2D)EditorGUILayout.ObjectField(Right != null ? "Right Eye (" + Right.width + "x" + Right.height + ")" : "Right Eye", Right, typeof(Texture2D), false);

            if (Left == null || Right == null)
            {
                EditorGUILayout.HelpBox("Requires a Left and Right eye texture.", MessageType.Info);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }

            if (Left.width != Right.width || Left.height != Right.height)
            {
                EditorGUILayout.HelpBox("Texture sizes must match!", MessageType.Error);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }

           EditorGUILayout.EndVertical();

            GUILayout.Space(5);
        }


        bool refresh = false;

        if (Left != null && Right != null)
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            var size = Screen.width * 0.95f;
            var rect = EditorGUILayout.GetControlRect(GUILayout.Width(size), GUILayout.Height(size * (Left.width / Left.height)));

            GUILayout.Space(5);

            
            BridgeX = EditorGUILayout.Slider("Bridge X", BridgeX, 0, 2);
            EditorGUILayout.HelpBox("Small objects may appear doubled because they dont overlap in the left and right eye textures. Bridge X will usually remove this gap by spreading the right texture more left, and the left texture more right.", MessageType.Info);

            ExpandY = EditorGUILayout.Slider("Expand Y", ExpandY, 0, 5);
            EditorGUILayout.HelpBox("The difference between the left and right eye is mainly horizontal, thus the merge will only increase the padding on the left and right sides of an object, not the top and bottom. Use this to spread the left and right textures vertically to create padding for the cursor.", MessageType.Info);

            if (PreviewLeft != null && PreviewRight != null)
            {
                PreviewDepthAlpha = EditorGUILayout.Slider("Preview Alpha", PreviewDepthAlpha, 0, 1);
                EyeType = (EyeTypes)EditorGUILayout.EnumPopup("Preview Eye", EyeType);
                GUI.DrawTexture(rect, EyeType == EyeTypes.Left ? PreviewLeft : PreviewRight);
            }
            

            PreviewLeft = (Texture2D)EditorGUILayout.ObjectField("VRPanorama Left Eye", PreviewLeft, typeof(Texture2D), false);
            PreviewRight = (Texture2D)EditorGUILayout.ObjectField("VRPanorama Right Eye", PreviewRight, typeof(Texture2D), false);
            EditorGUILayout.HelpBox("Drop the actual vrpanorama images that correspond to the depth texture here to place them behind the Preview Depth texture. You can change the preview texture's alpha to see how it matches against the vrpanorama visuals.", MessageType.Info);
            if (PreviewLeft == null || PreviewRight == null)
            {
                PreviewDepthAlpha = 1;
                EyeType = EyeTypes.Left;
            }

            GUI.color = new Color(1, 1, 1, PreviewDepthAlpha);
            GUI.DrawTexture(rect, _cmt);
            GUI.color = Color.white;

            refresh = GUILayout.Button("Refresh");

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        if (Left != null && (refresh || _cmt == null || _cmt.material == null || _cmt.width != OutputWidth || _cmt.height != OutputHeight))
        {
            _cmt = new CustomRenderTexture(OutputWidth, OutputHeight, RenderTextureFormat.ARGB32);
            _cmt.material = new Material(Shader.Find("VRPanoramaDepthMerger"));
        }

        if (_cmt == null || _cmt.material == null)
            return;

        _cmt.material.SetTexture("_Left", Left);
        _cmt.material.SetTexture("_Right", Right);

        _cmt.material.SetFloat("_BridgeX", BridgeX);
        _cmt.material.SetFloat("_ExpandY", ExpandY);

        GUILayout.Space(5);
        GUILayout.Space(5);
        if (!_fullPreview)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            OutputWidth = EditorGUILayout.IntField("Output Width", OutputWidth);
            OutputHeight = EditorGUILayout.IntField("Output Height", OutputHeight);
            EditorGUILayout.HelpBox("The resolution of the png to generate.", MessageType.Info);
            OutputName = EditorGUILayout.TextField("Output Name", OutputName);
            EditorGUILayout.HelpBox("The file will be created in the root of the Assets folder. Move it where you want after it generates.", MessageType.Info);

            GUILayout.Space(5);
            

            if (GUILayout.Button("Save"))
            {
                RenderTexture.active = _cmt;
                var texture = new Texture2D(_cmt.width, _cmt.height);
                texture.ReadPixels(new Rect(0, 0, _cmt.width, _cmt.height), 0, 0);
                texture.Apply();
                SaveTexture(texture, Application.dataPath + "/" + OutputName + ".png");
                RenderTexture.active = null;
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        /*
        if (GUILayout.Button("Merge"))
        {
            var texture = new Texture2D(Left.width, Left.height);

            Color testColor;
            Func<Color, Texture2D, int, int, Color> pickColor = (currentColor, testTexture, tx, ty) =>
            {
                testColor = testTexture.GetPixel(tx, ty);
                return testColor.linear.grayscale > currentColor.linear.grayscale ? testColor : currentColor; // return the brighter color
            };


            int x;
            int y;
            int mx;
            int my;
            Color color;
            int leftX;
            int leftY;
            int rightX;
            int rightY;

            var absMoveX = Mathf.Abs(BridgeX);
            var absMoveY = Mathf.Abs(ExpandY);

            var width = Left.width;
            var height = Left.height;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    color = Left.GetPixel(x, y);

                    for (mx = 0; mx <= absMoveX; mx++)
                    {
                        for (my = 0; my <= absMoveY; my++)
                        {
                            leftX = x + mx;

                            if (leftX >= 0 && leftX <= width)
                            {
                                leftY = y + my;
                                if (leftY >= 0 && leftY <= height)
                                    color = pickColor(color, Left, leftX, leftY);

                                leftY = y - my;

                                if (leftY >= 0 && leftY <= height)
                                    color = pickColor(color, Left, leftX, leftY);
                            }


                            rightX = x - mx;

                            if (rightX >= 0 && rightX <= width)
                            {
                                rightY = y + my;

                                if (rightY >= 0 && rightY <= height)
                                    color = pickColor(color, Right, rightX, rightY);

                                rightY = y - my;

                                if (rightY >= 0 && rightY <= height)
                                    color = pickColor(color, Right, rightX, rightY);
                            }                            
                        }
                    }

                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();

            SaveTexture(texture, Application.dataPath + "/" + OutputName + ".png");
        }*/

    }

    Texture2D SaveTexture(Texture2D texture, string filePath)
    {
        Debug.Log("write to: " + filePath);
        byte[] bytes = texture.EncodeToPNG();
        FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        BinaryWriter writer = new BinaryWriter(stream);
        for (int i = 0; i < bytes.Length; i++)
        {
            writer.Write(bytes[i]);
        }
        writer.Close();
        stream.Close();
        DestroyImmediate(texture);
        //I can't figure out how to import the newly created .png file as a texture
        AssetDatabase.Refresh();
        Texture2D newTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D));
        if (newTexture == null)
        {
            Debug.Log("Couldn't Import");
        }
        else
        {
            Debug.Log("Import Successful");
        }
        return newTexture;
    }
}
