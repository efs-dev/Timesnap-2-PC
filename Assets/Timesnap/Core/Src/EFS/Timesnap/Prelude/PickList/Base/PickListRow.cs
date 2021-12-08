using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickListRow : MonoBehaviour
{
    public TMP_Text Label;
    public Button Button;

    public void SetText(string text)
    {
        Label.text = text;
    }
}