using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
public class Test : MonoBehaviour
{
    public UnityEvent onMouseOver;
    public UnityEvent onMouseExit;

    private void OnMouseEnter()
    {
        onMouseOver.Invoke();
    }

    private void OnMouseExit()
    {
        onMouseExit.Invoke();
    }
}
