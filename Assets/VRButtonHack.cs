using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButtonHack : MonoBehaviour
{
    public VRButton Owner;

    public void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        Owner.OnPointerDown(null);
    }

    public void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
        Owner.OnPointerUp(null);
    }

    public void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        Owner.OnPointerEnter(null);
    }

    public void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
        Owner.OnPointerExit(null);
    }
}