using Src.Scripts.OnScreenKeyboard;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(IFormElement))]
public class VRFormElement : MonoBehaviour
{
    public void Start()
    {
        var formElement = GetComponent<IFormElement>();
        // todo?
        formElement.OnSelectAsObservable().Subscribe(_ =>
        {
            FindObjectOfType<OnScreenKeyboard>().Show(formElement);
        });

        formElement.OnDeselectAsObservable().Subscribe(_ =>
        {
            FindObjectOfType<OnScreenKeyboard>().Hide();
        });
    }
}