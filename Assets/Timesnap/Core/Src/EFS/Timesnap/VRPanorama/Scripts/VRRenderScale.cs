using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRRenderScale : MonoBehaviour {

    [Range(0.5f, 4)]
    public float RenderScale = 1;

	// Use this for initialization
	void Start () {
        
        XRSettings.eyeTextureResolutionScale = RenderScale;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
