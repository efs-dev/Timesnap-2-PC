using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif

[ExecuteInEditMode]
public class VRPanorama : MonoBehaviour {

#if UNITY_EDITOR

    public enum Modes { Single, Double, Animation };
    public Modes Mode;

    public bool AnimateInEditor;


    public float CollisionRotationX = 180;
    public float CollisionRotationY = 180;
    public float CollisionRotationZ = 180;
    public float CollisionWidth = 0.2f;
    public float CollisionHeight = 0.2f;
    public string ToolTipText;

    public enum VisualModes { Normal, Depth, Collision };
    public static VisualModes VisualMode;

    [MenuItem("VRPanorama/Visualize/Normal", priority = 300)]
    static void VisualizeNormal()
    {
        VisualMode = VisualModes.Normal;
    }

    [MenuItem("VRPanorama/Visualize/Depth", priority = 301)]
    static void VisualizeDepth()
    {
        VisualMode = VisualModes.Depth;
    }

    [MenuItem("VRPanorama/Visualize/Collision", priority = 302)]
    static void VisualizeCollision()
    {
        VisualMode = VisualModes.Collision;
    }

    private Texture2D EmptyTexture;

#endif

    private static VRPanoramaDepthRanges _depthRanges;
    public static VRPanoramaDepthRanges DepthRanges {
        get
        {
            if (_depthRanges == null)
                _depthRanges = Resources.Load<VRPanoramaDepthRanges>("VRPanorama/VRPanoramaDepthRanges");

            return _depthRanges;
        }
    }
    public string DepthRangeId;

    //public enum CollisionTypes { None, Box, ColorMap };
    //public CollisionTypes CollisionType = CollisionTypes.ColorMap;

    public Rect UV = new Rect();
    public Texture2D DepthTexture;
    public Texture2D CollisionTexture;

    public ColorMapCollisionData ColorMapCollision = new ColorMapCollisionData();

    [System.Serializable]
    public class ColorMapCollisionData
    {
        public List<ColorMapCollisionColorData> ColorData = new List<ColorMapCollisionColorData>();

        [System.Serializable]
        public class ColorMapCollisionColorData
        {
            public Color Color = Color.white;
            public string ToolTip;
            public UnityEngine.Events.UnityEvent OnClick;
            public UnityEngine.Events.UnityEvent OnClickUp;
        }
    }


    public GameObject Collision;
    public GameObject CollisionRotatorH;
    public GameObject CollisionRotatorV;
    public GameObject CollisionRotatorPivot;
    public GameObject CollisionTester;

    public UnityEngine.Events.UnityEvent OnClick;
    public UnityEngine.Events.UnityEvent OnClickUp;

    public Texture2D Texture;
    public int RenderQueue = 2000;

    public Color Color = Color.white;
    public float Alpha = 1;
    
    public VRPanoramaPartialImage PartialImageSettings;

    [System.Serializable]
    public class VRPanoramaPartialImage
    {
        [System.Serializable]
        public class LeftEyeSettings
        {
            public Vector2 LeftEyePosition = new Vector2();
            public Vector2 LeftEyeSize = new Vector2(4096, 4096);
        }

        [System.Serializable]
        public class RightEyeSettings
        {
            public Texture2D RightEyeTexture;
            public Vector2 RightEyePosition;
            public Vector2 RightEyeSize = new Vector2(4096, 4096);
        }

        [System.Serializable]
        public class BackgroundSettings
        {
            public Vector2 BackgroundSize = new Vector2(8192, 4096);
        }

        public LeftEyeSettings LeftEye;
        public RightEyeSettings RightEye;
        public BackgroundSettings Background;
        public VRPanoramaAnimation Animation;
    }

    [System.Serializable]
    public class VRPanoramaAnimation
    {
#if UNITY_EDITOR
        public UnityEditor.DefaultAsset LeftEyeFolder;
        public UnityEditor.DefaultAsset RightEyeFolder;
#endif
        public Texture2D[] LeftEyeTextures = new Texture2D[0];
        public Texture2D[] RightEyeTextures = new Texture2D[0];

        public float Duration = 0;
        public float TotalTime = 0;

        public int AnimationIndex { get; set; }
    }

    // 0 left, 1 right, used to tween
   // private float _leftToRightEye;
   // private float _targetLeftToRightEye;

   // private float _duration;

    private Material _material;

    public bool IsCollisionEnabled = true;

#if UNITY_EDITOR

    public static bool ShowHidden { get; private set; }
    [MenuItem("VRPanorama/Debug/Show Hidden", priority = 100)]
    static void SetShowHidden()
    {
        PlayerPrefs.SetInt("VRPanoramaShowHidden", 1);
        ShowHidden = true;
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.DirtyHierarchyWindowSorting();
    }

    static int _framesSinceHideHidden;
    [MenuItem("VRPanorama/Debug/Hide Hidden", priority = 101)]
    static void SetHideHidden()
    {
        PlayerPrefs.SetInt("VRPanoramaShowHidden", 0);
        _framesSinceHideHidden = 0;
        ShowHidden = false;
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.DirtyHierarchyWindowSorting();
        EditorApplication.update -= RepaintHierarchy;
        EditorApplication.update += RepaintHierarchy;
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    static void OnReloadScripts()
    {
        if (PlayerPrefs.HasKey("VRPanoramaShowHidden") && PlayerPrefs.GetInt("VRPanoramaShowHidden") == 1)
            SetShowHidden();
    }

    static void RepaintHierarchy()
    {
        _framesSinceHideHidden++;
        if (_framesSinceHideHidden >= 100)
        {
            EditorApplication.update -= RepaintHierarchy;
        }
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.DirtyHierarchyWindowSorting();
    }

    [MenuItem("VRPanorama/Debug/Reload Sphere Mesh", priority = 102)]
    static void ReloadSphereMesh()
    {
        var sphere = Resources.Load<Mesh>("VRPanorama/Sphere");
        GetVRPanoramasInScene().ForEach((vrPanorama) =>
        {
            var meshRenderer = vrPanorama.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = vrPanorama.gameObject.AddComponent<MeshRenderer>();

            var meshFilter = vrPanorama.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = vrPanorama.gameObject.AddComponent<MeshFilter>();

            meshFilter.sharedMesh = sphere;

            EditorUtility.SetDirty(vrPanorama.gameObject);
        });

        GetVRPanoramaColorPickersInScene().ForEach((colorPicker) =>
        {
            var meshFilter = colorPicker.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = colorPicker.gameObject.AddComponent<MeshFilter>();

            meshFilter.sharedMesh = sphere;

            var collider = colorPicker.GetComponent<MeshCollider>();
            if (collider == null)
                collider = colorPicker.gameObject.AddComponent<MeshCollider>();

            EditorUtility.SetDirty(colorPicker.gameObject);
        });

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    static List<VRPanorama> GetVRPanoramasInScene()
    {
        return SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(it => it.GetComponentsInChildren<VRPanorama>(true)).ToList();
    }

    static List<VRPanoramaColorPicker> GetVRPanoramaColorPickersInScene()
    {
        return SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(it => it.GetComponentsInChildren<VRPanoramaColorPicker>(true)).ToList();
    }

    [MenuItem("GameObject/Create VRPanorama", false, 0)]
    static void CreateVRPanorama()
    {
        var go = new GameObject();
        go.name = "VRPanorama";
        go.AddComponent<VRPanorama>();

        if (Selection.activeGameObject != null)
            go.transform.SetParent(Selection.activeGameObject.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = new Vector3(-1, 1, 1);

        Selection.activeGameObject = go;
    }

    void CreateCollisionGameobjects()
    {
        if (CollisionRotatorH != null)
            return;

        Collision = new GameObject("Collision Initial Z");
        Collision.transform.SetParent(transform);
        Collision.transform.localPosition = Vector3.zero;
        Collision.transform.localEulerAngles = new Vector3(0, 0, 90);
        Collision.transform.localScale = Vector3.one;

        var co2 = new GameObject("Collision Initial Y");
        co2.transform.SetParent(Collision.transform);
        co2.transform.localPosition = Vector3.zero;
        co2.transform.localEulerAngles = new Vector3(0, 180, 0);
        co2.transform.localScale = Vector3.one;

        CollisionRotatorH = new GameObject("Collision Rotator Horizontal");
        CollisionRotatorH.transform.SetParent(co2.transform);
        CollisionRotatorH.transform.localPosition = Vector3.zero;
        CollisionRotatorH.transform.localEulerAngles = new Vector3(0, 0, CollisionRotationZ);
        CollisionRotatorH.transform.localScale = Vector3.one;


        CollisionRotatorV = new GameObject("Collision Rotator Vertical");
        CollisionRotatorV.transform.SetParent(CollisionRotatorH.transform);
        CollisionRotatorV.transform.localPosition = Vector3.zero;
        CollisionRotatorV.transform.localEulerAngles = new Vector3(0, CollisionRotationY, 0); ;
        CollisionRotatorV.transform.localScale = Vector3.one;


        CollisionRotatorPivot = new GameObject("Collision Rotator Pivot");
        CollisionRotatorPivot.transform.SetParent(CollisionRotatorV.transform);
        CollisionRotatorPivot.transform.localPosition = Vector3.zero;
        CollisionRotatorPivot.transform.localScale = Vector3.one;
        CollisionRotatorPivot.transform.localEulerAngles = new Vector3(CollisionRotationX, 0, 0);

        CollisionTester = new GameObject("Collision Tester");
        CollisionTester.transform.SetParent(CollisionRotatorPivot.transform);
        CollisionTester.transform.localPosition = new Vector3(0.5f, 0, 0);
        CollisionTester.transform.localEulerAngles = new Vector3(0, 0, 0);
        CollisionTester.transform.localScale = Vector3.one;
        var boxCollider = CollisionTester.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(-0.01f, 0f, 0f);
        boxCollider.size = new Vector3(0.01f, 0.2f, 0.2f);
        var vrButton = CollisionTester.AddComponent<VRButton>();
        vrButton.ShowTooltipOnRollover = true;
        vrButton.TooltipText = "Test";
        CollisionTester.AddComponent<VRPanorama3DObject>();
        CollisionTester.SetActive(false);
    }

#endif

    void RunClickDown()
    {
        OnClick?.Invoke();
    }

    void RunClickUp()
    {
        OnClickUp?.Invoke();
    }

    void Awake()
    {
        UpdateTransform(transform);
    }

    void Start()
    {
#if UNITY_EDITOR

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Resources.Load<Mesh>("VRPanorama/Sphere");
        }
        
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        CreateMaterial();
        CreateCollisionGameobjects();
#endif


        //Debug.Log(CollisionTester);
        /// Debug.Log(CollisionTester.GetComponent<VRButton>());
        // Debug.Log(CollisionTester.GetComponent<VRButton>().ClickDown);
        CollisionTester.GetComponent<VRButton>().ClickDown.RemoveAllListeners();
        CollisionTester.GetComponent<VRButton>().ClickDown.AddListener(RunClickDown);
        CollisionTester.GetComponent<VRButton>().ClickUp.RemoveAllListeners();
        CollisionTester.GetComponent<VRButton>().ClickUp.AddListener(RunClickUp);
        PartialImageSettings.Animation.TotalTime = 0;

        _material = GetComponent<Renderer>().sharedMaterial;
        _material.SetInt("_TargetEye", 0);
        _material.SetTexture("_LeftTexture", Texture);
        _material.SetTexture("_RightTexture", PartialImageSettings.RightEye.RightEyeTexture == null ? Texture : PartialImageSettings.RightEye.RightEyeTexture);
    }

    Material CreateMaterial()
    {
        _material = GetComponent<Renderer>().sharedMaterial;


        _material = new Material(Shader.Find("TimeSnap/VRPanorama"));
        _material.name = "Internal VRPanorama";


        GetComponent<Renderer>().sharedMaterial = _material;

        return _material;
    }

    /*
    public void DisableVR(float duration)
    {
        _duration = duration;
        _targetLeftToRightEye = 1;
    }

    public void EnableVR(float duration)
    {
        _duration = duration;
        _targetLeftToRightEye = 0;
    }

    public void Toggle(float duration)
    {
       // Debug.Log("Toggle: " + duration);
        _duration = duration;
        _targetLeftToRightEye = _targetLeftToRightEye == 0 ? 1 : 0;
    }
    */


    Vector2 GetUVFromPosition(Vector2 position, Vector2 size, Vector2 tiling, Vector2 backgroundSize)
    {

        var rangeX = Mathf.Abs(tiling.x);
        var percentX = (position.x) / backgroundSize.x; // (size.x*0.54f)
        var offsetX = -(rangeX * percentX);


        var rangeY = Mathf.Abs(tiling.y);
        var percentY = (backgroundSize.y - (position.y + size.y)) / backgroundSize.y; // - (size.y/4)
        var offsetY = -(rangeY * percentY);

        //Debug.Log("x: " + tiling.x + ", " + rangeX + ", " + percentX + ", " + offsetX);
       // Debug.Log("y: " + tiling.y + ", " + rangeY + ", " + percentY + ", " + offsetY);
        return new Vector2(offsetX, offsetY);

    }


    private static VRPanoramaSettings _settings;
    public static VRPanoramaSettings Settings
    {
        get
        {
            if (_settings == null)
                _settings = Resources.Load<VRPanoramaSettings>("VRPanorama/VRPanoramaSettings");

            return _settings;
        }
    }



    public static void UpdateTransform(Transform t)
    {
        var parent = t.parent;
        
        if (parent != null)
        {
            var index = t.GetSiblingIndex();
            t.SetParent(null, true);


            t.position = Vector3.zero;
            var eulerAngles = new Vector3(Settings.GlobalRotation.x, Settings.GlobalRotation.y, Settings.GlobalRotation.z);
            t.eulerAngles = eulerAngles;
            t.localScale = new Vector3(Settings.FlipHorizontal ? -Settings.GlobalScale : Settings.GlobalScale, Settings.GlobalScale, Settings.FlipVertical ? -Settings.GlobalScale : Settings.GlobalScale);

            t.SetParent(parent, true);
            t.SetSiblingIndex(index);
        }
        else
        {
            t.position = Vector3.zero;
            var eulerAngles = new Vector3(Settings.GlobalRotation.x, Settings.GlobalRotation.y, Settings.GlobalRotation.z);
            t.eulerAngles = eulerAngles;
            t.localScale = Vector3.one * Settings.GlobalScale;
        }



    }

#if UNITY_EDITOR


    public static Vector3 TrueScale(Transform t)
    {
        var scale = Vector3.one;
        var parent = t.parent;

        while (parent != null)
        {
            scale.Scale(parent.localScale);
            parent = parent.parent;
        }

        return scale;
    }


    private Vector3 _cachedPosition;
    private Vector3 _cachedRotation;
    private Vector3 _cachedScale;
    private Vector3 _cachedParentScale;
#endif

    // Update is called once per frame
    void Update ()
    {
#if UNITY_EDITOR

        if (transform.position != _cachedPosition || transform.eulerAngles != _cachedRotation || transform.localScale != _cachedScale || TrueScale(transform) != _cachedParentScale)
        {
            UpdateTransform(transform);
            _cachedPosition = transform.position;
            _cachedRotation = transform.eulerAngles;
            _cachedScale = transform.localScale;
            _cachedParentScale = TrueScale(transform);
        }


        transform.hideFlags = HideFlags.HideInInspector;

        if (GetComponent<MeshFilter>().sharedMesh == null)
        {
            GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("VRPanorama/Sphere");
        }
        CreateCollisionGameobjects();

        var mat = GetComponent<Renderer>().sharedMaterial;
        if (mat == null || mat.name != "Internal VRPanorama")
            mat = CreateMaterial();

        var ete = FindObjectOfType<VREditModeTargetEye>();
        var targetEye = (int)VREditModeTargetEye.TargetEye;


        if (Mode == Modes.Animation)
        {
            if (PartialImageSettings.Animation.LeftEyeTextures.Length > 0)
            {
                Texture = PartialImageSettings.Animation.LeftEyeTextures[0];
            }
            else
            {
                Texture = null;
            }

            if (PartialImageSettings.Animation.RightEyeTextures.Length > 0)
            {
                PartialImageSettings.RightEye.RightEyeTexture = PartialImageSettings.Animation.RightEyeTextures[0];
            }
            else
            {
                PartialImageSettings.RightEye.RightEyeTexture = Texture;
            }
        }
        


        if (Mode == Modes.Double)
        {
            var leftEyeTexture = Texture;
            var rightEyeTexture = PartialImageSettings.RightEye.RightEyeTexture;


            var leftEyeTiling = new Vector2(PartialImageSettings.Background.BackgroundSize.x / PartialImageSettings.LeftEye.LeftEyeSize.x, PartialImageSettings.Background.BackgroundSize.y / PartialImageSettings.LeftEye.LeftEyeSize.y);
            var rightEyeTiling = new Vector2(PartialImageSettings.Background.BackgroundSize.x / PartialImageSettings.RightEye.RightEyeSize.x, PartialImageSettings.Background.BackgroundSize.y / PartialImageSettings.RightEye.RightEyeSize.y);


            var leftEyeOffset = GetUVFromPosition(PartialImageSettings.LeftEye.LeftEyePosition, PartialImageSettings.LeftEye.LeftEyeSize, leftEyeTiling, PartialImageSettings.Background.BackgroundSize);
            var rightEyeOffset = GetUVFromPosition(PartialImageSettings.RightEye.RightEyePosition, PartialImageSettings.RightEye.RightEyeSize, rightEyeTiling, PartialImageSettings.Background.BackgroundSize);



            mat.SetTexture("_LeftTexture", leftEyeTexture);
            mat.SetVector("_LeftEyeTiling", leftEyeTiling);
            mat.SetVector("_LeftEyeOffset", leftEyeOffset);
            mat.SetTexture("_RightTexture", rightEyeTexture);
            mat.SetVector("_RightEyeTiling", rightEyeTiling);
            mat.SetVector("_RightEyeOffset", rightEyeOffset);
            


            UV.x = PartialImageSettings.LeftEye.LeftEyePosition.x / (PartialImageSettings.Background.BackgroundSize.x/2);
            UV.y = PartialImageSettings.LeftEye.LeftEyePosition.y / PartialImageSettings.Background.BackgroundSize.y;
            UV.width = PartialImageSettings.LeftEye.LeftEyeSize.x / (PartialImageSettings.Background.BackgroundSize.x/2);
            UV.height = PartialImageSettings.LeftEye.LeftEyeSize.y / PartialImageSettings.Background.BackgroundSize.y;
        }
        else
        {
            mat.SetTexture("_LeftTexture", Texture);
            mat.SetVector("_LeftEyeTiling", new Vector2(1, 0.5f));
            mat.SetVector("_LeftEyeOffset", new Vector2(0, 0.5f));
            mat.SetTexture("_RightTexture", Texture);
            mat.SetVector("_RightEyeOffset", new Vector2(0, 0f));
            mat.SetVector("_RightEyeTiling", new Vector2(1,  0.5f));

            UV.x = 0;
            UV.y = 0;
            UV.width = 1;
            UV.height = 1;
        }

        if (EmptyTexture == null)
        {
            EmptyTexture = new Texture2D(2, 2);
            EmptyTexture.SetPixel(0, 0, Color.clear);
            EmptyTexture.SetPixel(1, 0, Color.clear);
            EmptyTexture.SetPixel(0, 1, Color.clear);
            EmptyTexture.SetPixel(1, 1, Color.clear);
            EmptyTexture.Apply();
        }

        if (VisualMode == VisualModes.Depth)
        {
            mat.SetTexture("_LeftTexture", DepthTexture != null ? DepthTexture : EmptyTexture);
            mat.SetTexture("_RightTexture", DepthTexture != null ? DepthTexture : EmptyTexture);
            mat.SetVector("_RightEyeTiling", mat.GetVector("_LeftEyeTiling"));
            mat.SetVector("_RightEyeOffset", mat.GetVector("_LeftEyeOffset"));
        }
        else if (VisualMode == VisualModes.Collision)
        {
            mat.SetTexture("_LeftTexture", CollisionTexture != null ? CollisionTexture : EmptyTexture);
            mat.SetTexture("_RightTexture", CollisionTexture != null ? CollisionTexture : EmptyTexture);
            mat.SetVector("_RightEyeTiling", mat.GetVector("_LeftEyeTiling"));
            mat.SetVector("_RightEyeOffset", mat.GetVector("_LeftEyeOffset"));
        }


        mat.SetInt("_TargetEye", targetEye);
        mat.renderQueue = RenderQueue;

        mat.hideFlags = ShowHidden ? HideFlags.None : HideFlags.HideInInspector;
        GetComponent<MeshFilter>().hideFlags = ShowHidden ? HideFlags.None : HideFlags.HideInInspector;
        GetComponent<MeshRenderer>().hideFlags = ShowHidden ? HideFlags.None : HideFlags.HideInInspector;
        Collision.hideFlags = ShowHidden ? HideFlags.None : HideFlags.HideInHierarchy;

        CollisionTester.gameObject.SetActive(false);// CollisionType == CollisionTypes.Box);
        CollisionRotatorH.transform.localEulerAngles = new Vector3(0, 0, CollisionRotationZ);
        CollisionRotatorV.transform.localEulerAngles = new Vector3(0, 360 - CollisionRotationY, 0);
        CollisionRotatorPivot.transform.localEulerAngles = new Vector3(CollisionRotationX, 0, 0);

        CollisionTester.GetComponent<BoxCollider>().size = new Vector3(Mathf.Max(0, CollisionTester.GetComponent<BoxCollider>().size.x), Mathf.Max(0, CollisionWidth), Mathf.Max(0, CollisionHeight));


        CollisionTester.GetComponent<VRButton>().ShowTooltipOnRollover = !string.IsNullOrEmpty(ToolTipText);
        CollisionTester.GetComponent<VRButton>().TooltipText = ToolTipText;

#endif

        var animationTimeDelta = Time.deltaTime;
#if UNITY_EDITOR
        if (!Application.isPlaying && !AnimateInEditor)
        {
            PartialImageSettings.Animation.TotalTime = 0;
            animationTimeDelta = 0;
        }
#endif
        Animate(animationTimeDelta);

        _material.SetFloat("_LeftToRightEye", EFS.Timesnap.VR.TimesnapVRPlayer.LeftToRightEye);
        _material.SetColor("_Color", Color);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Alpha = Mathf.Clamp(Alpha, 0, 1);

            var alpha = Alpha;
            var vrPanoramaGroup = GetComponentInParent<VRPanoramaGroup>();
            if (vrPanoramaGroup != null)
                alpha *= vrPanoramaGroup.GetTrueAlpha();

            _material.SetFloat("_Alpha", alpha);


            EditorApplication.update-= EditorUpdateAnimation;
            EditorApplication.update += EditorUpdateAnimation;
        }
#endif
    }



#if UNITY_EDITOR

    private static double _time = 0;

    void EditorUpdateAnimation()
    {
        EditorApplication.QueuePlayerLoopUpdate();
    }
#endif

    void Animate(float deltaTime)
    {
        if (PartialImageSettings.Animation.LeftEyeTextures.Length > 0)
        {
            _material.SetTexture("_LeftTexture", PartialImageSettings.Animation.LeftEyeTextures[PartialImageSettings.Animation.AnimationIndex]);
            _material.SetTexture("_RightTexture", PartialImageSettings.Animation.RightEyeTextures[PartialImageSettings.Animation.AnimationIndex]);
            //PartialImageSettings.Animation.AnimationIndex = PartialImageSettings.Animation.AnimationIndex < PartialImageSettings.Animation.LeftEyeTextures.Length - 1 ? PartialImageSettings.Animation.AnimationIndex + 1 : 0;

            if (PartialImageSettings.Animation.Duration <= 0)
                PartialImageSettings.Animation.Duration = 0.01f;

            PartialImageSettings.Animation.TotalTime += deltaTime;
            var percent = (PartialImageSettings.Animation.TotalTime % PartialImageSettings.Animation.Duration) / PartialImageSettings.Animation.Duration;

            PartialImageSettings.Animation.AnimationIndex = Mathf.FloorToInt(PartialImageSettings.Animation.LeftEyeTextures.Length * percent);
        }
    }

    void LateUpdate()
    {
        var alpha = Alpha;
        var vrPanoramaGroup = GetComponentInParent<VRPanoramaGroup>();
        if (vrPanoramaGroup != null)
            alpha *= vrPanoramaGroup.GetTrueAlpha();

        _material.SetFloat("_Alpha", alpha);
    }


}