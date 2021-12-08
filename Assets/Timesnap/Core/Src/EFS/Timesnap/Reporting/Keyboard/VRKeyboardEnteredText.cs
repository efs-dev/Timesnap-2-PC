using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
public class VRKeyboardEnteredText : MonoBehaviour {

    private TextMeshProUGUI _text;
    private VRKeyboard _keyboard;
	// Use this for initialization
	void Start () {
        _text = GetComponent<TextMeshProUGUI>();
        _keyboard = GetComponentInParent<VRKeyboard>();
    }
	
	// Update is called once per frame
	void Update () {        
        switch (_keyboard.InputType)
        {
            case VRKeyboard.TextTypes.Email:
                _text.text = _keyboard.EnteredText;
                break;
            case VRKeyboard.TextTypes.Name:
                var casedName = "";
                for (var i = 0; i < _keyboard.EnteredText.Length; i++)
                {
                    if (i == 0 || _keyboard.EnteredText[i - 1] == ' ')
                        casedName += _keyboard.EnteredText.Substring(i, 1).ToUpper();
                    else
                        casedName += _keyboard.EnteredText.Substring(i, 1).ToLower();
                }
                _text.text = casedName;
                break;
        }
	}
}
