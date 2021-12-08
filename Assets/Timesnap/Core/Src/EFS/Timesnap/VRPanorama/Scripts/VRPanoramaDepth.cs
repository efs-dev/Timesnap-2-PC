

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Linq;

using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class VRPanoramaColorPicker : MonoBehaviour
{

    public List<VRPanoramaColorPickerDebug> DebugData = new List<VRPanoramaColorPickerDebug>();
    public class VRPanoramaColorPickerDebug
    {
        public string Tag;
        public Texture2D Texure;
        public Vector2 TextureUV = new Vector2(-1, -1);
        public Vector2 Pixel = new Vector2(-1, -1);
        public Vector2 UV = new Vector2(-1, -1);
        public VRPanorama vrPanorama;
    }


    public class VRPanoramaColorPickerData
    {
        public string Tag = "";
        public Func<VRPanorama, Texture2D> GetTexture;
        public Action<VRPanorama, Color> OnPick;
        public Action<RaycastHit> OnPickNone = (RaycastHit hit) => { };
        public Func<VRPanorama, bool> IsValidVRPanorama;
        public Func<bool> IsEnabled = () => true;
        public Action OnDisable = () => { };

        public List<VRPanorama> VRPanoramas = new List<VRPanorama>();
    }

    [HideInInspector]
    public Transform Pointer;
    
        

    public List<VRPanoramaColorPickerData> GetTextureData = new List<VRPanoramaColorPickerData>();
        
    protected virtual void SetupTextureData() {}

    void Awake()
    {
        VRPanorama.UpdateTransform(transform);
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y), Math.Abs(transform.localScale.z));
    }

    void Start()
    {
        SetupTextureData();
        GetTextureData.ForEach(data =>
        {
            data.VRPanoramas = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(it => it.GetComponentsInChildren<VRPanorama>(true)).ToList().FindAll(x => data.IsValidVRPanorama(x));
            data.VRPanoramas.Sort((x, y) => x.RenderQueue.CompareTo(y.RenderQueue));
        });
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/Create VRPanoramaDepth", false, 0)]
    static void CreateVRPanoramaDepth()
    {
        var go = new GameObject();
        go.name = "VRPanorama Depth";
        go.AddComponent<VRPanoramaDepth>();

        if (Selection.activeGameObject != null)
            go.transform.SetParent(Selection.activeGameObject.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = new Vector3(1, 1, 1);

        Selection.activeGameObject = go;
    }
#endif

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        VRPanorama.UpdateTransform(transform);
        transform.hideFlags = HideFlags.HideInInspector;
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y), Math.Abs(transform.localScale.z));
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Resources.Load<Mesh>("VRPanorama/Sphere");
        }
        meshFilter.hideFlags = VRPanorama.ShowHidden ? HideFlags.None : HideFlags.HideInInspector;

        var collider = GetComponent<MeshCollider>();
        if (collider == null)
            collider = gameObject.AddComponent<MeshCollider>();
        collider.hideFlags = VRPanorama.ShowHidden ? HideFlags.None : HideFlags.HideInInspector;


        DebugData.Clear();

        if (!Application.isPlaying)
        {
            LayerController.CreateLayer("VRColorPicker");
            gameObject.layer = LayerMask.NameToLayer("VRColorPicker");
            return;
        }
#endif

        Vector3 rayPointerStart = Pointer.position;
        Vector3 rayPointerEnd = rayPointerStart +
          (Pointer.forward * 100);
        
//#if UNITY_EDITOR
       // if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
       // {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            rayPointerStart = ray.origin;
            rayPointerEnd = rayPointerStart + (ray.direction * 100);
       // }
//#endif
        
        Vector3 cameraLocation = Camera.main.transform.position;
        Vector3 finalRayDirection = rayPointerEnd - cameraLocation;
        finalRayDirection.Normalize();

        Vector3 finalRayStart = cameraLocation + (finalRayDirection * Camera.main.nearClipPlane);


        RaycastHit hit;
        if (!Physics.Raycast(new Ray(finalRayStart, finalRayDirection), out hit, 100, LayerMask.GetMask("VRColorPicker", "VRPanorama3DObject")))
            return;

        //Debug.Log("raycast hit: " + hit.collider.gameObject);

        var uvx = VRPanorama.Settings.FlipHorizontal ? 1 - (Mathf.Clamp(hit.textureCoord.x * 2f, 1, 2) - 1) : Mathf.Clamp(hit.textureCoord.x * 2f, 0, 1);
        var uvy = hit.textureCoord.y;

        //if (VRPanorama.Settings.FlipHorizontal)
          //  uvx = 1 - uvx;
        if (VRPanorama.Settings.FlipVertical)
            uvy = 1 - uvy;


        //Debug.Log(uvx + ", " + uvy);

        var panoramas = FindObjectsOfType<VRPanorama>().ToList();
        panoramas.Sort((x, y) => x.RenderQueue.CompareTo(y.RenderQueue));

        GetTextureData.ForEach(data =>
        {
            //Debug.Log("data: " + data.Tag);
            if (data.IsEnabled())
            {
                //Debug.Log(data.Tag + " enabled");
                bool picked = false;
                for (var i = panoramas.Count- 1; i >= 0; i--)
                {
                    var vrPanorama = panoramas[i];

                    if (!vrPanorama.gameObject.activeInHierarchy || !data.IsValidVRPanorama(vrPanorama))
                        continue;

                    //Debug.Log(vrPanorama);

                    if (uvx < vrPanorama.UV.x || uvx > vrPanorama.UV.x + vrPanorama.UV.width ||
                        1 - uvy < vrPanorama.UV.y || 1 - uvy > vrPanorama.UV.y + vrPanorama.UV.height)
                        continue;

                    

                    var clippedUVX = uvx - vrPanorama.UV.x;
                    var clippedUVY = uvy - (1 - vrPanorama.UV.y);

                    var fittedUVX = clippedUVX / vrPanorama.UV.width;
                    var fittedUVY = clippedUVY / vrPanorama.UV.height;

                    var zTexture = data.GetTexture(vrPanorama);
                    var px = Mathf.FloorToInt(fittedUVX * zTexture.width);
                    var py = zTexture.height + Mathf.FloorToInt(fittedUVY * zTexture.height);

                    // Debug.Log(vrPanorama.name + "vrPanorama.UV.y: " + vrPanorama.UV.y + ", vrPanorama.UV.height: " + vrPanorama.UV.height + ", uvy: " + uvy + ", clippedUVY: " + clippedUVY + ", fittedUVY: " + fittedUVY + ", py: " + py);

                    //Debug.Log(zTexture.name + ", width: " + zTexture.width + ", height: " + zTexture.height + ", px: " + px + ", py: " + py);

                    var color = zTexture.GetPixel(px, py);
                    //if (color.a == 0)
                   //     continue;

                    DebugData.Add(new VRPanoramaColorPickerDebug() { Tag = data.Tag, Texure = zTexture, Pixel = new Vector2(px, py + zTexture.height), UV = new Vector2(uvx, uvy), TextureUV = new Vector2(vrPanorama.UV.x, vrPanorama.UV.y), vrPanorama = vrPanorama});

                    data.OnPick(vrPanorama, color);
                    picked = true;
                    break;
                }
                if (!picked)
                {

                    DebugData.Add(new VRPanoramaColorPickerDebug());

                    data.OnPickNone(hit);
                }
            }
            else
            {

                DebugData.Add(new VRPanoramaColorPickerDebug());

                data.OnDisable();
            }


        });
    }
}

public class VRPanoramaDepth : VRPanoramaColorPicker
{
    private EFS.Timesnap.VR.TimesnapVRPlayer _vrPlayer;



    private EFS.Timesnap.VR.Pointer _pointer;



    protected override void SetupTextureData()
    {
        _pointer = FindObjectOfType<EFS.Timesnap.VR.Pointer>();

        var data = new VRPanoramaColorPickerData();
        data.Tag = "Depth";
        data.IsValidVRPanorama = (vrpanorama) => vrpanorama.DepthTexture != null;
        data.GetTexture = (vrpanorama) => vrpanorama.DepthTexture;
        data.OnPick = (vrpanorama, color) =>
        {
            var colorDistance = color.linear.grayscale;

            // Debug.Log("distance: " + colorDistance);
            var rangeData = VRPanorama.DepthRanges.Ranges.Count > 0 ? VRPanorama.DepthRanges.Ranges.Find(x => x.Id == vrpanorama.DepthRangeId) : null;
            if (rangeData != null)
            {
                var dist = (1 - colorDistance) * rangeData.GetUnitDistance();
                _vrPlayer.PointerDistance = rangeData.UnitClose + dist;
            }
        };
        data.IsEnabled = () => !VRPanorama3DObject.IsOver && _vrPlayer.IsVREnabled;
        data.OnPickNone = (hit) =>
        {
            _vrPlayer.PointerDistance = VRPanorama.Settings.GlobalPointerDistance;
        };
        GetTextureData.Add(data);

        data = new VRPanoramaColorPickerData();
        data.Tag = "Collision";
        data.IsValidVRPanorama = (vrpanorama) => vrpanorama.CollisionTexture != null && vrpanorama.IsCollisionEnabled;
        data.GetTexture = (vrpanorama) => vrpanorama.CollisionTexture;
        data.OnPick = (vrpanorama, color) =>
        {
            if (!VRInputHelper.IsPointerEnabled || TargetManager.IsPopupOpen)
                return;

            var colorData = vrpanorama.ColorMapCollision.ColorData.Find(x => x.Color == color);

            //Debug.Log("Color: " + color);
            if (colorData != null && (_vrPlayer.IsVREnabled || !_vrPlayer.IgnorePointerOnVRDisable))
            {
                _pointer.SetHovering(true);
                _pointer.ShowTooltipText(colorData.ToolTip);

                if (GvrControllerInput.ClickButtonDown)// ||  OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All))
                {
                    colorData.OnClick?.Invoke();
                }
                else if (GvrControllerInput.ClickButtonUp)// || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All))
                {
                    colorData.OnClickUp?.Invoke();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    colorData.OnClick?.Invoke();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    colorData.OnClickUp?.Invoke();
                }
            }
            else
            {
                if (!VRButton.IsOverButton)
                {
                    _pointer.HideTooltipText();
                    _pointer.SetHovering(false);
                }
            }

        };
        data.OnPickNone = (hit) =>
        {
            if (!VRButton.IsOverButton)
            {
                _pointer.HideTooltipText();
                _pointer.SetHovering(false);

            }
        };
        data.IsEnabled = () => !VRPanorama3DObject.IsOver;
        GetTextureData.Add(data);

        data = new VRPanoramaColorPickerData();
        data.Tag = "3D Object";
        data.IsValidVRPanorama = (vrpanorama) => false;
        data.GetTexture = (vrpanorama) => null;
        data.OnPick = (vrpanorama, color) => { };
        data.OnPickNone = (hit) =>
        {
            if (VRPanorama3DObject.Over.CollisionType == VRPanorama3DObject.CollisionTypes.Depth)
            {
                _vrPlayer.UseCollisionPointerDistance = false;
                var rangeData = VRPanorama.DepthRanges.Ranges.Count > 0 ? VRPanorama.DepthRanges.Ranges.Find(x => x.Id == VRPanorama3DObject.Over.DepthRangeId) : null;
                if (rangeData != null)
                {

                    var dist = (1 - VRPanorama3DObject.Over.Depth.linear.grayscale) * rangeData.GetUnitDistance();
                    _vrPlayer.PointerDistance = rangeData.UnitClose + dist;
                }
            }
            //else if (VRPanorama3DObject.Over.CollisionType == VRPanorama3DObject.CollisionTypes.Collider)
          //  {
               // Pointer.position = hit.point;
             //   _vrPlayer.ApplyToolTipDistance();
           // }
        };
        data.IsEnabled = () => { return VRPanorama3DObject.IsOver && VRPanorama3DObject.Over.CollisionType == VRPanorama3DObject.CollisionTypes.Depth; };
        GetTextureData.Add(data);
    }


    void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif
        _vrPlayer = FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>();
        Pointer = FindObjectOfType<EFS.Timesnap.VR.Pointer>().transform;

    }

}

/*
public class VRPanoramaDepth : MonoBehaviour {

    public Transform Pointer;

    public RawImage Image;

    public EFS.Timesnap.VR.TimesnapVRPlayer VRPlayer;

    public Vector2 BackgroundDepthTextureSize;

    //public bool ShowLogs;
    
    public float Close = 0.75f;
    public float Far = 2;

    public float ReadOnlyDist;

    public LayerMask TestLayer;

    private List<VRPanorama> _vrPanoramas = new List<VRPanorama>();

    public Texture2D DepthTexture { get { return _vrPanoramas[0].DepthTexture; } }

    void Start()
    {
        _vrPanoramas = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(it => it.GetComponentsInChildren<VRPanorama>(true)).ToList().FindAll(x=>x.DepthTexture != null);

        _vrPanoramas.Sort((x, y) => x.RenderQueue.CompareTo(y.RenderQueue));

    }

    

        
    // Update is called once per frame
    void Update () {

        Vector3 rayPointerStart = Pointer.position;
        Vector3 rayPointerEnd = rayPointerStart +
          (Pointer.forward * 100);

        Vector3 cameraLocation = Camera.main.transform.position;
        Vector3 finalRayDirection = rayPointerEnd - cameraLocation;
        finalRayDirection.Normalize();

        Vector3 finalRayStart = cameraLocation + (finalRayDirection * Camera.main.nearClipPlane);

        var ray = new Ray(finalRayStart, finalRayDirection);

        RaycastHit hit;

        var cam = Camera.main;
        
        if (!Physics.Raycast(ray, out hit, 100, TestLayer))
            return;

        MeshCollider meshCollider = hit.collider as MeshCollider;

        var percentH = 1 - (Mathf.Clamp(hit.textureCoord.x * 2f, 1, 2) - 1);// 1 - Mathf.Clamp(angleH / 180, 0, 1);
        var percentV = hit.textureCoord.y;// 1 - Mathf.Clamp(angleV / 180, 0, 1);


        for (var i = _vrPanoramas.Count-1; i >= 0; i--)
        {
            var vrPanorama = _vrPanoramas[i];

            if (!vrPanorama.gameObject.activeSelf)
                continue;


            if (percentH < vrPanorama.UV.x || percentH > vrPanorama.UV.x + vrPanorama.UV.width ||
                1-percentV < vrPanorama.UV.y || 1-percentV > vrPanorama.UV.y + vrPanorama.UV.height)
                continue;

            var px = Mathf.FloorToInt(percentH * BackgroundDepthTextureSize.x);
            var py = Mathf.FloorToInt(percentV * BackgroundDepthTextureSize.y);

            var color = DepthTexture.GetPixel(px, py);
            if (color.a == 0)
                continue;

            var colorDistance = color.linear.grayscale;

           // Debug.Log(hit.collider.name + ", " + vrPanorama.name + ", " + percentH + ", " + percentV + " -- " + vrPanorama.DepthUV);

            var dist = (1 - colorDistance) * (Far - Close);
            VRPlayer.PointerDistance = Close + dist;
            ReadOnlyDist = VRPlayer.PointerDistance;
            break;
        }


        //GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", DepthTexture);



    }
}
*/