using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRPanoramaDepthControls : MonoBehaviour
{
    public TextMeshProUGUI DepthRangeIdText;
    public TextMeshProUGUI UnitCloseText;
    public TextMeshProUGUI UnitFarText;
    public GameObject UnitCloseMarker;
    public GameObject UnitFarMarker;

    private bool _isFar = true;


    void Update()
    {
        /*
        //var pos = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTrackedRemote);

        //if (OVRInput.Get(OVRInput.Button.Back, OVRInput.Controller.RTrackedRemote))
        //    _isFar = !_isFar;

        var vrPanoramaDepth = FindObjectOfType<VRPanoramaDepth>();
        var debugData = vrPanoramaDepth.DebugData.Find(x => x.Tag == "Depth");
        if (debugData == null)
        {
            DepthRangeIdText.text = "";
            UnitCloseText.text = "";
            UnitFarText.text = "";
            UnitCloseMarker.SetActive(false);
            UnitFarMarker.SetActive(false);
            return;
        }

        var depthRangeId = !VRPanorama3DObject.IsOver ? debugData.vrPanorama.DepthRangeId : VRPanorama3DObject.Over.DepthRangeId;

        var depthRanges = VRPanorama.DepthRanges;
        var depthRange = depthRanges.Ranges.Find(x => x.Id == depthRangeId);

        if (_isFar)
            depthRange.UnitFar = Mathf.Clamp(depthRange.UnitFar + pos.y / 250, 0, 10);
        else
            depthRange.UnitClose = Mathf.Clamp(depthRange.UnitClose + pos.y / 250, 0, 10);

        UnitCloseMarker.SetActive(!_isFar);
        UnitFarMarker.SetActive(_isFar);

        DepthRangeIdText.text = depthRange.Id;
        UnitCloseText.text = depthRange.UnitClose.ToString();
        UnitFarText.text = depthRange.UnitFar.ToString();
        */
    }
}
