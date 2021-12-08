using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VRPanoramaDepthRanges.asset", menuName = "VRPanoramaDepthRanges", order = 1000)]
public class VRPanoramaDepthRanges : ScriptableObject
{
    public List<RangeSettings> Ranges;    

}

[System.Serializable]
public class RangeSettings
{
    public string Id;

    public float DistanceCM = 0;

    public float UnitClose = 0.63f;
    public float UnitFar = 3.44f;

    public float CMClose = 0;
    public float CMFar = 718;

    public float GetUnitDistance()
    {
        float unitRange = UnitFar - UnitClose;
        return unitRange;
        //float cmRange = CMFar - CMClose;
        //return DistanceCM * (unitRange / cmRange);
    }
}