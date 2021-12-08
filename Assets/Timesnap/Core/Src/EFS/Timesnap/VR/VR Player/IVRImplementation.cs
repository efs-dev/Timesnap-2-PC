using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace EFS.Timesnap.VR
{
    public interface IVRImplementation
    {
        IObservable<GameObject> HoverBegin { get; }
        IObservable<GameObject> HoverEnd { get; }
        IObservable<Unit> ClickDown { get; }
        IObservable<Unit> ClickUp { get; }
        Vector3 RayEndpoint { get; }
        bool ShowLaser {  set; }
        Color LaserColor {  set; }
        Color LaserColorEnd { set; }
        List<TimesnapLaserPointer> Lasers { get; }
    }
}