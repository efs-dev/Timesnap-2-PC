using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoVREffect : MonoBehaviour {

	void OnEnable()
    {
        Debug.Log("onenable");
        FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>().DisableVREffectObjects.Add(this);
    }

    void OnDisable()
    {
        Debug.Log("ondisable");
        FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>().DisableVREffectObjects.Remove(this);
    }
}
