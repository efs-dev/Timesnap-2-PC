using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimesnapLaserVisual : GvrLaserVisual
{
    public bool DynamicLaserDistance = false;
    public float FixedLaserDistance = 0.9f;

    protected override void UpdateCurrentPosition()
    {
        if (DynamicLaserDistance)
        {
            base.UpdateCurrentPosition();
        }
        else
        {
            currentDistance = FixedLaserDistance;
            if (GetPointForDistanceFunction != null)
            {
                currentPosition = GetPointForDistanceFunction(currentDistance);
            }
            else
            {
                var origin = transform.position;
                currentPosition = origin + (transform.forward * currentDistance);
            }

            currentLocalPosition = transform.InverseTransformPoint(currentPosition);
            currentLocalRotation = Quaternion.FromToRotation(Vector3.forward, currentLocalPosition);
        }
    }
    
}