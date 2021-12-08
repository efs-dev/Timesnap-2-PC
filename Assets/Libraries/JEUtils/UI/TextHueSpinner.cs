using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextHueSpinner : MonoBehaviour
{
    public float    HueCycleSpeed;
    public float    Saturation;
    public float    Value;
    public TMP_Text Label;

    void Update()
    {
        var hue = Mathf.Repeat(HueCycleSpeed * Time.frameCount / 10, 1f);
        Label.color = Color.HSVToRGB(hue, Saturation, Value);
    }
}