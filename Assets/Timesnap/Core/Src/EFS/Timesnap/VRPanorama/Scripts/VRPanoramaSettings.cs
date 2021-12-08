using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VRPanoramaSettings.asset", menuName = "VRPanoramaSettings", order = 1001)]
public class VRPanoramaSettings : ScriptableObject
{
    public Vector3 GlobalRotation = new Vector3(-90, 0, 90);
    public float GlobalScale = 10;

    public float GlobalPointerDistance = 1;

    public bool FlipHorizontal;
    public bool FlipVertical;
}