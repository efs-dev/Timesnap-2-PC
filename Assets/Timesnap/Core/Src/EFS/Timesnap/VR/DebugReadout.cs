using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class DebugReadout : MonoBehaviour
{
    public Camera Cam1;
    public Camera Cam2;
    public float Speed;
    public float Mult = 1;
    public float Off = .5f;

    void Update()
    {
        Cam1.transform.localScale = Vector3.one * (Mathf.Sin(Time.frameCount / Speed) + Off) * Mult;
        Cam2.transform.localScale = Vector3.one * (Mathf.Sin(Time.frameCount / Speed) + Off) * Mult;
    }
}