using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Linq;
using System.IO;

[CustomEditor(typeof(VRPanorama))]
public class VRPanoramaCustomInspector : Editor {

    SerializedProperty Mode;
    SerializedProperty Texture;
    SerializedProperty RightEyeTexture;


    SerializedProperty Color;
    SerializedProperty Alpha;
    SerializedProperty RenderQueue;

    SerializedProperty LeftEyePosition;
    SerializedProperty LeftEyeSize;
    SerializedProperty RightEyePosition;
    SerializedProperty RightEyeSize;


    SerializedProperty BackgroundSize;


    SerializedProperty LeftEyeTexturesFolder;
    SerializedProperty RightEyeTexturesFolder;
    SerializedProperty AnimationDuration;
    SerializedProperty AnimateInEditor;


    SerializedProperty CollisionType;
    SerializedProperty IsCollisionEnabled;
    SerializedProperty CollisionX;
    SerializedProperty CollisionY;
    SerializedProperty CollisionZ;
    SerializedProperty CollisionWidth;
    SerializedProperty CollisionHeight;
    SerializedProperty ToolTipText;


    SerializedProperty DepthTexture;
    SerializedProperty DepthRangeId;



    SerializedProperty CollisionTexture;


    SerializedProperty OnClick;

    VRPanorama _target;

    bool _leftEyeInfo;
    bool _rightEyeInfo;

    bool _animationInfo;
    bool _backgroundInfo;

    bool _distanceSettings;

    bool _collisionInfo;
    bool _renderInfo;


    bool _depthInfo;

    void OnEnable()
    {
        _target = (VRPanorama)target;
        Mode = serializedObject.FindProperty("Mode");
        Texture = serializedObject.FindProperty("Texture");
        RightEyeTexture = serializedObject.FindProperty("PartialImageSettings.RightEye.RightEyeTexture");

        LeftEyeTexturesFolder = serializedObject.FindProperty("PartialImageSettings.Animation.LeftEyeFolder");
        RightEyeTexturesFolder = serializedObject.FindProperty("PartialImageSettings.Animation.RightEyeFolder");
        AnimationDuration = serializedObject.FindProperty("PartialImageSettings.Animation.Duration");
        AnimateInEditor = serializedObject.FindProperty("AnimateInEditor");

        LeftEyePosition = serializedObject.FindProperty("PartialImageSettings.LeftEye.LeftEyePosition");
        LeftEyeSize = serializedObject.FindProperty("PartialImageSettings.LeftEye.LeftEyeSize");
        RightEyePosition = serializedObject.FindProperty("PartialImageSettings.RightEye.RightEyePosition");
        RightEyeSize = serializedObject.FindProperty("PartialImageSettings.RightEye.RightEyeSize");


        BackgroundSize = serializedObject.FindProperty("PartialImageSettings.Background.BackgroundSize");


        OnClick = serializedObject.FindProperty("OnClick");


        CollisionType = serializedObject.FindProperty("CollisionType");
        IsCollisionEnabled = serializedObject.FindProperty("IsCollisionEnabled");
        CollisionX = serializedObject.FindProperty("CollisionRotationX");
        CollisionY = serializedObject.FindProperty("CollisionRotationY");
        CollisionZ = serializedObject.FindProperty("CollisionRotationZ");
        CollisionWidth = serializedObject.FindProperty("CollisionWidth");
        CollisionHeight = serializedObject.FindProperty("CollisionHeight");

        ToolTipText = serializedObject.FindProperty("ToolTipText");


        Color = serializedObject.FindProperty("Color");
        Alpha = serializedObject.FindProperty("Alpha");
        RenderQueue = serializedObject.FindProperty("RenderQueue");

        DepthTexture = serializedObject.FindProperty("DepthTexture");

        DepthRangeId = serializedObject.FindProperty("DepthRangeId");

        CollisionTexture = serializedObject.FindProperty("CollisionTexture");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var ts = VRPanorama.TrueScale(_target.transform);
        if (ts.x != ts.y || ts.y != ts.z)
        {
            EditorGUILayout.HelpBox("VRPanorama parent's scale.x, scale.y, scale.z should all match! Do not use different scales for different axis ... this can distort the panorama with certain rotations!", MessageType.Warning);
        }

        EditorGUILayout.PropertyField(Mode, new GUIContent("Mode"));


        switch (Mode.enumValueIndex)
        {
            case (int)VRPanorama.Modes.Single:
                EditorGUILayout.PropertyField(Texture, new GUIContent("Texture"));


                if (_target.Texture == null)
                    _target.Texture = null;

                if (_target.PartialImageSettings.RightEye.RightEyeTexture != null)
                {
                    _target.PartialImageSettings.RightEye.RightEyeTexture = null;
                    EditorUtility.SetDirty(_target);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }

                if (_target.PartialImageSettings.Animation.LeftEyeTextures.Length > 0)
                {
                    _target.PartialImageSettings.Animation.LeftEyeTextures = new Texture2D[0];
                    EditorUtility.SetDirty(_target);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }
                if (_target.PartialImageSettings.Animation.RightEyeTextures.Length > 0)
                {
                    _target.PartialImageSettings.Animation.RightEyeTextures = new Texture2D[0];
                    EditorUtility.SetDirty(_target);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }
                break;
            case (int)VRPanorama.Modes.Double:

                EditorGUILayout.PropertyField(Texture, new GUIContent("Left Eye"));
                EditorGUILayout.PropertyField(RightEyeTexture, new GUIContent("Right Eye"));

                var leftClampError = _target.Texture != null && _target.Texture.wrapMode != TextureWrapMode.Clamp;


                if (_target.Texture == null)
                    _target.Texture = null;

                if (_target.PartialImageSettings.RightEye.RightEyeTexture == null)
                    _target.PartialImageSettings.RightEye.RightEyeTexture = null;

                var rightClampError = _target.PartialImageSettings.RightEye != null && _target.PartialImageSettings.RightEye.RightEyeTexture != null && _target.PartialImageSettings.RightEye.RightEyeTexture.wrapMode != TextureWrapMode.Clamp;

                if (leftClampError && rightClampError)
                {
                    EditorGUILayout.HelpBox("Left and Right eye textures must have wrapMode = Clamp", MessageType.Error);
                }
                else if (leftClampError)
                {
                    EditorGUILayout.HelpBox("Left eye texture must have wrapMode = Clamp", MessageType.Error);
                }
                else if (rightClampError)
                {
                    EditorGUILayout.HelpBox("Right eye texture must have wrapMode = Clamp", MessageType.Error);
                }

                _leftEyeInfo = EditorGUILayout.Foldout(_leftEyeInfo, "Left Eye");
                if (_leftEyeInfo)
                {
                    EditorGUILayout.PropertyField(LeftEyePosition, new GUIContent("Left Eye Offset"));
                    EditorGUILayout.PropertyField(LeftEyeSize, new GUIContent("Left Eye Size"));
                }

                _rightEyeInfo = EditorGUILayout.Foldout(_rightEyeInfo, "Right Eye");
                if (_rightEyeInfo)
                {
                    EditorGUILayout.PropertyField(RightEyePosition, new GUIContent("Right Eye Offset"));
                    EditorGUILayout.PropertyField(RightEyeSize, new GUIContent("Right Eye Size"));
                }

                _backgroundInfo = EditorGUILayout.Foldout(_backgroundInfo, "Background");
                if (_backgroundInfo)
                {
                    EditorGUILayout.PropertyField(BackgroundSize, new GUIContent("Background Size"));
                }

                if (_target.PartialImageSettings.Animation.LeftEyeTextures.Length > 0)
                {
                    _target.PartialImageSettings.Animation.LeftEyeTextures = new Texture2D[0];
                    EditorUtility.SetDirty(_target);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }
                if (_target.PartialImageSettings.Animation.RightEyeTextures.Length > 0)
                {
                    _target.PartialImageSettings.Animation.RightEyeTextures = new Texture2D[0];
                    EditorUtility.SetDirty(_target);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }

                break;
            case (int)VRPanorama.Modes.Animation:


                if (_target.Texture == null)
                    _target.Texture = null;

                if (_target.PartialImageSettings.RightEye.RightEyeTexture == null)
                    _target.PartialImageSettings.RightEye.RightEyeTexture = null;

                _animationInfo = EditorGUILayout.Foldout(_animationInfo, "Animation");

                if (_animationInfo)
                {
                    var numLeftEyeTextures = _target.PartialImageSettings.Animation.LeftEyeTextures != null ? _target.PartialImageSettings.Animation.LeftEyeTextures.Length : 0;
                    var numRightEyeTextures = _target.PartialImageSettings.Animation.RightEyeTextures != null ? _target.PartialImageSettings.Animation.RightEyeTextures.Length : 0;


                    EditorGUILayout.PropertyField(LeftEyeTexturesFolder, new GUIContent("Left Eye Folder (" + numLeftEyeTextures + ")"));
                    EditorGUILayout.PropertyField(RightEyeTexturesFolder, new GUIContent("Right Eye Folder (" + numRightEyeTextures + ")"));

                    if (numLeftEyeTextures != numRightEyeTextures)
                        EditorGUILayout.HelpBox("Left and Right eyes must have the same number of textures!", MessageType.Error);


                    leftClampError = false;
                    rightClampError = false;

                    for (var i = 0; i < _target.PartialImageSettings.Animation.LeftEyeTextures.Length; i++)
                    {
                        if (_target.PartialImageSettings.Animation.LeftEyeTextures[i] == null)
                        {
                            _target.PartialImageSettings.Animation.LeftEyeTextures = new Texture2D[0];
                            EditorUtility.SetDirty(_target);
                            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                            break;
                        }

                        if (_target.PartialImageSettings.Animation.LeftEyeTextures[i].wrapMode != TextureWrapMode.Clamp)
                            leftClampError = true;
                    }

                    for (var i = 0; i < _target.PartialImageSettings.Animation.RightEyeTextures.Length; i++)
                    {
                        if (_target.PartialImageSettings.Animation.RightEyeTextures[i] == null)
                        {
                            _target.PartialImageSettings.Animation.RightEyeTextures = new Texture2D[0];
                            EditorUtility.SetDirty(_target);
                            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                            break;
                        }
                        if (_target.PartialImageSettings.Animation.RightEyeTextures[i].wrapMode != TextureWrapMode.Clamp)
                            rightClampError = true;
                    }

                    if (leftClampError && rightClampError)
                    {
                        EditorGUILayout.HelpBox("Left and Right eye textures must have wrapMode = Clamp", MessageType.Error);
                    }
                    else if (leftClampError)
                    {
                        EditorGUILayout.HelpBox("Left eye textures must have wrapMode = Clamp", MessageType.Error);
                    }
                    else if (rightClampError)
                    {
                        EditorGUILayout.HelpBox("Right eye textures must have wrapMode = Clamp", MessageType.Error);
                    }


                    EditorGUILayout.PropertyField(AnimationDuration, new GUIContent("Duration (Seconds)"));

                    EditorGUILayout.PropertyField(AnimateInEditor, new GUIContent("Animate In Editor"));

                    if (GUILayout.Button("Load Textures"))
                    {
                        if (_target.PartialImageSettings.Animation.LeftEyeFolder != null)
                        {

                            var leftFolderPath = AssetDatabase.GetAssetPath(_target.PartialImageSettings.Animation.LeftEyeFolder);
                            var leftFolderFiles = Directory.GetFiles(leftFolderPath).ToList().FindAll(x => x.EndsWith(".png") || x.EndsWith(".jpg"));


                            _target.PartialImageSettings.Animation.LeftEyeTextures = new Texture2D[leftFolderFiles.Count];
                            for (var i = 0; i < leftFolderFiles.Count; i++)
                            {
                                _target.PartialImageSettings.Animation.LeftEyeTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(leftFolderFiles[i]);
                            }

                        }
                        else
                        {
                            _target.PartialImageSettings.Animation.LeftEyeTextures = new Texture2D[0];
                        }

                        if (_target.PartialImageSettings.Animation.RightEyeFolder != null)
                        {
                            var rightFolderPath = AssetDatabase.GetAssetPath(_target.PartialImageSettings.Animation.RightEyeFolder);
                            var rightFolderFiles = Directory.GetFiles(rightFolderPath).ToList().FindAll(x => x.EndsWith(".png") || x.EndsWith(".jpg"));

                            _target.PartialImageSettings.Animation.RightEyeTextures = new Texture2D[rightFolderFiles.Count];
                            for (var i = 0; i < rightFolderFiles.Count; i++)
                            {
                                _target.PartialImageSettings.Animation.RightEyeTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(rightFolderFiles[i]);
                            }
                        }
                        else
                        {
                            _target.PartialImageSettings.Animation.RightEyeTextures = new Texture2D[0];
                        }


                        EditorUtility.SetDirty(_target);
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                    }
                }

                _leftEyeInfo = EditorGUILayout.Foldout(_leftEyeInfo, "Left Eye");
                if (_leftEyeInfo)
                {
                    EditorGUILayout.PropertyField(LeftEyePosition, new GUIContent("Left Eye Offset"));
                    EditorGUILayout.PropertyField(LeftEyeSize, new GUIContent("Left Eye Size"));
                }

                _rightEyeInfo = EditorGUILayout.Foldout(_rightEyeInfo, "Right Eye");
                if (_rightEyeInfo)
                {
                    EditorGUILayout.PropertyField(RightEyePosition, new GUIContent("Right Eye Offset"));
                    EditorGUILayout.PropertyField(RightEyeSize, new GUIContent("Right Eye Size"));
                }


                _backgroundInfo = EditorGUILayout.Foldout(_backgroundInfo, "Background");
                if (_backgroundInfo)
                {
                    EditorGUILayout.PropertyField(BackgroundSize, new GUIContent("Background Size"));
                }









                break;
        }


        _collisionInfo = EditorGUILayout.Foldout(_collisionInfo, "Collision Info");
        if (_collisionInfo)
        {

            //EditorGUILayout.PropertyField(CollisionType, new GUIContent("Collision Type"));



            /*if (_target.CollisionType == VRPanorama.CollisionTypes.Box)
            {
                EditorGUILayout.HelpBox("Deprecated! This does not work with the depth texture!", MessageType.Warning);
                /*
                EditorGUILayout.PropertyField(IsCollisionEnabled, new GUIContent("Is Collision Enabled"));
                Slider(EditorGUILayout.GetControlRect(), CollisionZ, 0, 359, new GUIContent("Horizontal"));
                Slider(EditorGUILayout.GetControlRect(), CollisionY, 0, 359, new GUIContent("Vertical"));
                Slider(EditorGUILayout.GetControlRect(), CollisionX, 90, 270, new GUIContent("Pivot"));
                Slider(EditorGUILayout.GetControlRect(), CollisionWidth, 0, 1, new GUIContent("Width"));
                Slider(EditorGUILayout.GetControlRect(), CollisionHeight, 0, 1, new GUIContent("Height"));

                EditorGUILayout.PropertyField(ToolTipText, new GUIContent("Tool Tip"));
                
                EditorGUILayout.PropertyField(OnClick, new GUIContent("On Click"));

                if (GUILayout.Button("Look At"))
                {
                    var sceneCamera = UnityEditor.SceneView.lastActiveSceneView.camera;
                    sceneCamera.transform.position = Vector3.zero;
                    sceneCamera.transform.LookAt(_target.CollisionTester.transform, Vector3.up);
                    UnityEditor.SceneView.lastActiveSceneView.AlignViewToObject(sceneCamera.transform);
                }*/
            //}
            //else if (_target.CollisionType == VRPanorama.CollisionTypes.ColorMap)
            //{

            EditorGUILayout.PropertyField(CollisionTexture, new GUIContent("Color Map"));

            if (_target.CollisionTexture != null)
            {
                EditorGUILayout.PropertyField(IsCollisionEnabled, new GUIContent("Is Collision Enabled"));
                var textureImporter = ((TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.CollisionTexture)));
                bool save = false;
                if (!textureImporter.isReadable)
                {
                    save = true;
                }
                if (!textureImporter.alphaIsTransparency)
                {
                    save = true;
                }
                if (textureImporter.wrapMode != TextureWrapMode.Clamp)
                {
                    save = true;
                }
                if (save)
                {

                    EditorGUILayout.HelpBox("Collision Texture has invalid settings!", MessageType.Error);
                    if (GUILayout.Button("Fix"))
                    {
                        textureImporter.isReadable = true;
                        textureImporter.alphaIsTransparency = true;
                        textureImporter.wrapMode = TextureWrapMode.Clamp;
                        textureImporter.SaveAndReimport();
                    }
                }
                //}

                for (var i = 0; i < _target.ColorMapCollision.ColorData.Count; i++)
                {
                    var colorData = _target.ColorMapCollision.ColorData[i];


                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(5);


                    EditorGUI.BeginChangeCheck();


                    EditorGUILayout.BeginHorizontal();
                    var newColor = EditorGUILayout.ColorField("Color", colorData.Color);
                    GUILayout.Space(5);
                    GUI.backgroundColor = UnityEngine.Color.red;
                    var style = new GUIStyle(EditorStyles.toolbarButton);
                    style.fontStyle = FontStyle.Bold;
                    style.onNormal.textColor = UnityEngine.Color.white;
                    style.active.textColor = UnityEngine.Color.white;
                    style.padding.left = 0;
                    style.padding.right = 0;
                    style.margin.left = 3;
                    style.contentOffset = new Vector2(2, 0);
                    style.normal.textColor = UnityEngine.Color.white;
                    if (GUILayout.Button("X", style, GUILayout.Width(15)))
                    {
                        _target.ColorMapCollision.ColorData.RemoveAt(i);
                        EditorUtility.SetDirty(_target);
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                    }
                    GUI.backgroundColor = UnityEngine.Color.white;
                    GUILayout.Space(3);
                    EditorGUILayout.EndHorizontal();


                    if (newColor.a != 1 && Event.current.type != EventType.ExecuteCommand)
                    {
                        EditorGUILayout.HelpBox("The color's alpha is not 100%. Make sure that's on purpose!", MessageType.Warning);
                    }

                    var newToolTip = EditorGUILayout.TextField("Tool Tip", colorData.ToolTip);

                    var sp = serializedObject.FindProperty("ColorMapCollision.ColorData.Array.data[" + i + "].OnClick");
                    EditorGUILayout.PropertyField(sp, true);

                    sp = serializedObject.FindProperty("ColorMapCollision.ColorData.Array.data[" + i + "].OnClickUp");
                    EditorGUILayout.PropertyField(sp, true);
                    serializedObject.ApplyModifiedProperties();

                    GUILayout.Space(5);
                    EditorGUILayout.EndVertical();

                    if (EditorGUI.EndChangeCheck())
                    {
                        
                        colorData.Color = newColor;
                        colorData.ToolTip = newToolTip;
                        EditorUtility.SetDirty(_target);
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                    }

                }


                if (GUILayout.Button("New Entry"))
                {
                    _target.ColorMapCollision.ColorData.Add(new VRPanorama.ColorMapCollisionData.ColorMapCollisionColorData());
                    EditorUtility.SetDirty(_target);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }

            }

            

        }

        _depthInfo = EditorGUILayout.Foldout(_depthInfo, "Depth Info");
        if (_depthInfo)
        {
            EditorGUILayout.PropertyField(DepthTexture, new GUIContent("Depth Texture"));

            if (_target.DepthTexture != null)
            {
                var textureImporter = ((TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.DepthTexture)));
                var save = false;
                if (!textureImporter.isReadable)
                {
                    save = true;
                }
                if (textureImporter.sRGBTexture)
                {
                    save = true;
                }
                if (!textureImporter.alphaIsTransparency)
                {
                    save = true;
                }
                if (textureImporter.wrapMode != TextureWrapMode.Clamp)
                {
                    save = true;
                }
                if (save)
                {
                    EditorGUILayout.HelpBox("Depth Texture has invalid settings!", MessageType.Error);
                    if (GUILayout.Button("Fix"))
                    {
                        textureImporter.isReadable = true;
                        textureImporter.sRGBTexture = false;
                        textureImporter.alphaIsTransparency = true;
                        textureImporter.wrapMode = TextureWrapMode.Clamp;
                        textureImporter.SaveAndReimport();
                    }
                }


                EditorGUILayout.PropertyField(DepthRangeId, new GUIContent("RangeId"));

                if (VRPanorama.DepthRanges.Ranges.Find(x=> x.Id ==_target.DepthRangeId) == null)
                {
                    EditorGUILayout.HelpBox("Invalid RangeId!", MessageType.Error);
                }
            }
        }

        _renderInfo = EditorGUILayout.Foldout(_renderInfo, "Render Info");
        if (_renderInfo)
        {
            EditorGUILayout.PropertyField(Color, new GUIContent("Color"));
            EditorGUILayout.PropertyField(Alpha, new GUIContent("Alpha"));
            EditorGUILayout.PropertyField(RenderQueue, new GUIContent("RenderQueue"));

            if (GUILayout.Button("Find Mesh"))
            {
                var path = AssetDatabase.GetAssetPath(_target.GetComponent<MeshFilter>().sharedMesh);
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Debug.Log(asset);
                Selection.activeObject = asset;
               // Selection = 
            }
        }


        serializedObject.ApplyModifiedProperties();
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
