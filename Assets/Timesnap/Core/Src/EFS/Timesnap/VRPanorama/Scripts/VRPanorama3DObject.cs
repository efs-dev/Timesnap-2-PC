using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class VRPanorama3DObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static VRPanorama3DObject Over { get; private set; }
	public static bool IsOver { get { return Over != null; } }

    public Color Depth;
    public float CursorDepthOffset = 0;

    public string DepthRangeId;

    public enum CollisionTypes { Collider, Depth };
    public CollisionTypes CollisionType;

#if UNITY_EDITOR
    public bool UseDepthDistance;
    public bool FaceCamera;
#endif

    public bool DisableWhenNoVR;


    private EFS.Timesnap.VR.TimesnapVRPlayer _player;

    void Awake()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        for (var i = 0; i < cameras.Length; i++)
        {
            var cam = cameras[i];

            cam.cullingMask |= 1 << LayerMask.NameToLayer("VRPanorama3DObject");
        }

        _player = FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!VRInputHelper.IsPointerEnabled)
            return;

        Over = this;

        if (CollisionType == CollisionTypes.Collider)
        {
            _player.UseCollisionPointerDistance = true;
        }
        //Debug.Log("enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!VRInputHelper.IsPointerEnabled)
            return;

        Over = null;

        if (CollisionType == CollisionTypes.Collider)
        {
            _player.UseCollisionPointerDistance = false;
        }
        // Debug.Log("exit");
    }

    void OnDisable()
    {
        if (Over == this)
        {

            if (CollisionType == CollisionTypes.Collider)
            {
                _player.UseCollisionPointerDistance = false;
            }

            Over = null;
        }
    }

    void Update()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (gameObject.layer != LayerMask.NameToLayer("VRPanorama3DObject"))
            {
                LayerController.CreateLayer("VRPanorama3DObject");
                gameObject.layer = LayerMask.NameToLayer("VRPanorama3DObject");
            }

            
            if (CollisionType == CollisionTypes.Depth)
            {
                UseDepthDistance = true;

                var rangeData = VRPanorama.DepthRanges.Ranges.Count > 0 ? VRPanorama.DepthRanges.Ranges.Find(x => x.Id == DepthRangeId) : null;
                if (rangeData != null)
                {
                    var dist = rangeData.UnitClose + ((1 - Depth.linear.grayscale) * rangeData.GetUnitDistance());

                    transform.position = transform.position.normalized * dist;

                    if (FaceCamera)
                    {
                        transform.LookAt(Vector3.zero, Vector3.up);
                        transform.Rotate(Vector3.up * 180);
                    }
                }
            }
            else if (CollisionType == CollisionTypes.Collider)
            {
                UseDepthDistance = false;
                FaceCamera = false;

            }
            
            
            

            return;
        }
        
#endif

        

        if (DisableWhenNoVR && _player.IgnorePointerOnVRDisable)
        {
            if (!_player.IsVREnabled)
            {
                Over = null;
                GetComponent<Collider>().enabled = false;
            }
            else if (_player.IsVREnabled)
            {
                GetComponent<Collider>().enabled = true;
            }
        }

    }

#if UNITY_EDITOR

    public void OnMouseEnter()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            return;

        OnPointerEnter(null);
    }

    public void OnMouseExit()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            return;

        OnPointerExit(null);
    }
#endif
}
