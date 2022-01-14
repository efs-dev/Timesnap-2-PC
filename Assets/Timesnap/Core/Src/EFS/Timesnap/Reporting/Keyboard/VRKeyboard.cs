using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using TMPro;

using System;
using System.Runtime.InteropServices;


public class VRKeyboard : MonoBehaviour {

    public TextMeshProUGUI InputText;
    public string EnteredText;

    public UnityEvent OnSubmit;
    public Action<string> OnSubmitCallback = (value) => { };

    public enum TextTypes { Text, Email, Name };
    public TextTypes InputType;

    bool IsCapsLock
    {
        get { return false; }// (((ushort)GetKeyState(0x14)) & 0xffff) != 0; }
    }

    bool IsShift
    {
        get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); }
    }

    public void Submit()
    {
        OnSubmitCallback(EnteredText);
        if (OnSubmit != null)
            OnSubmit.Invoke();
    }

	
	// Update is called once per frame
	void Update ()
    {
        if (!string.IsNullOrEmpty(InputText.text))
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                EnteredText = EnteredText.Substring(0, InputText.text.Length - 1);
        }

        string add = null;
        if (Input.GetKeyDown(KeyCode.A))
            add = "a";
        if (Input.GetKeyDown(KeyCode.B))
            add = "b";
        if (Input.GetKeyDown(KeyCode.C))
            add = "c";
        if (Input.GetKeyDown(KeyCode.D))
            add = "d";
        if (Input.GetKeyDown(KeyCode.E))
            add = "e";
        if (Input.GetKeyDown(KeyCode.F))
            add = "f";
        if (Input.GetKeyDown(KeyCode.G))
            add = "g";
        if (Input.GetKeyDown(KeyCode.H))
            add = "h";
        if (Input.GetKeyDown(KeyCode.I))
            add = "i";
        if (Input.GetKeyDown(KeyCode.J))
            add = "j";
        if (Input.GetKeyDown(KeyCode.K))
            add = "k";
        if (Input.GetKeyDown(KeyCode.L))
            add = "l";
        if (Input.GetKeyDown(KeyCode.M))
            add = "m";
        if (Input.GetKeyDown(KeyCode.N))
            add = "n";
        if (Input.GetKeyDown(KeyCode.O))
            add = "o";
        if (Input.GetKeyDown(KeyCode.P))
            add = "p";
        if (Input.GetKeyDown(KeyCode.Q))
            add = "q";
        if (Input.GetKeyDown(KeyCode.R))
            add = "r";
        if (Input.GetKeyDown(KeyCode.S))
            add = "s";
        if (Input.GetKeyDown(KeyCode.T))
            add = "t";
        if (Input.GetKeyDown(KeyCode.U))
            add = "u";
        if (Input.GetKeyDown(KeyCode.V))
            add = "v";
        if (Input.GetKeyDown(KeyCode.W))
            add = "w";
        if (Input.GetKeyDown(KeyCode.X))
            add = "x";
        if (Input.GetKeyDown(KeyCode.Y))
            add = "y";
        if (Input.GetKeyDown(KeyCode.Z))
            add = "z";
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!IsShift)
                add = "1";
            else
                add = "!";
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!IsShift)
                add = "2";
            else
                add = "@";
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!IsShift)
                add = "3";
            else
                add = "#";
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!IsShift)
                add = "4";
            else
                add = "$";
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (!IsShift)
                add = "5";
            else
                add = "%";
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (!IsShift)
                add = "6";
            else
                add = "^";
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (!IsShift)
                add = "7";
            else
                add = "&";
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (!IsShift)
                add = "8";
            else
                add = "*";
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (!IsShift)
                add = "9";
            else
                add = "(";
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (!IsShift)
                add = "0";
            else
                add = ")";
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (!IsShift)
                add = "-";
            else
                add = "_";
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (!IsShift)
                add = "=";
            else
                add = "+";
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            if (!IsShift)
                add = "[";
            else
                add = "{";
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (!IsShift)
                add = "]";
            else
                add = "}";
        }
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (!IsShift)
                add = "\\";
            else
                add = "|";
        }
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            if (!IsShift)
                add = ";";
            else
                add = ":";
        }
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            if (!IsShift)
                add = "'";
            else
                add = "\"";
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            if (!IsShift)
                add = ",";
            else
                add = "<";
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            if (!IsShift)
                add = ".";
            else
                add = ">";
        }
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            if (!IsShift)
                add = "/";
            else
                add = "?";
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            add = "1";
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            add = "2";
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            add = "3";
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            add = "4";
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            add = "5";
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            add = "6";
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            add = "7";
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            add = "8";
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            add = "9";
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            add = "0";
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            add = "+";
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            add = "-";
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            add = "*";
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            add = "/";
        }


        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (!IsShift)
                add = "=";
            else
                add = "+";
        }

        if (!string.IsNullOrEmpty(add))
        {
            if (!IsCapsLock)
            {
                if (IsShift)
                    add = add.ToUpper();
            }
            else
            {
                if (!IsShift)
                    add = add.ToUpper();
            }

            EnteredText += add;
        }
    }
}
