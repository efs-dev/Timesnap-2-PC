using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class VRKeyboardKey : MonoBehaviour {
       
    public enum VRKeyboardKeyType { DataEntry, Delete, Space, Submit };
    public VRKeyboardKeyType KeyType;
       
    private VRKeyboard _keyboard;

    private CanvasGroup _canvasGroup;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif
        _keyboard = GetComponentInParent<VRKeyboard>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;



        var boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider>();

        boxCollider.size = transform.parent.GetComponent<RectTransform>().sizeDelta;

        var button = GetComponent<VRButton>();
        if (button == null)
            button = gameObject.AddComponent<VRButton>();

        button.ClickDown.AddListener(() =>
        {
            switch (KeyType)
            {
                case VRKeyboardKeyType.DataEntry:
                    _keyboard.EnteredText += GetComponent<TextMeshProUGUI>().text.ToLower();
                    break;
                case VRKeyboardKeyType.Delete:
                    if (_keyboard.EnteredText.Length > 0)
                        _keyboard.EnteredText = _keyboard.EnteredText.Substring(0, _keyboard.EnteredText.Length - 1);
                    break;
                case VRKeyboardKeyType.Space:
                    _keyboard.EnteredText += " ";
                    break;
                case VRKeyboardKeyType.Submit:
                    if (_keyboard.InputType == VRKeyboard.TextTypes.Email)
                    {
                        if (!validateEmail(_keyboard.EnteredText))
                            return;
                    }
                    _keyboard.Submit();
                    break;
            }
        });
	}

    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";



    public static bool validateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }

    // Update is called once per frame
    void Update () {

#if UNITY_EDITOR
        gameObject.name = "Text";
        transform.parent.gameObject.name = GetComponent<TextMeshProUGUI>().text;
#endif
        if (Application.isPlaying)
        {
            switch (KeyType)
            {
                case VRKeyboardKeyType.DataEntry:
                    break;
                case VRKeyboardKeyType.Delete:
                    break;
                case VRKeyboardKeyType.Space:
                    break;
                case VRKeyboardKeyType.Submit:
                    _canvasGroup.alpha = validateEmail(_keyboard.EnteredText) ? 1 : 0.5f;
                    break;
            }
        }
    }
}
