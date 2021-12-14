using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

using System;

public class VRKeyboard : MonoBehaviour {

    public string EnteredText;

    public UnityEvent OnSubmit;
    public Action<string> OnSubmitCallback = (value) => { };

    public enum TextTypes { Text, Email, Name };
    public TextTypes InputType;

    public void Submit()
    {
        OnSubmitCallback(EnteredText);
        if (OnSubmit != null)
            OnSubmit.Invoke();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
