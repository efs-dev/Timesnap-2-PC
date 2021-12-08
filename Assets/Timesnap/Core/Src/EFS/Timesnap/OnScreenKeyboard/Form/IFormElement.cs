using System;
using UniRx;
using UnityEngine;

public interface IFormElement
{
    IObservable<Unit> OnSelectAsObservable();
    IObservable<Unit> OnDeselectAsObservable();
    IObservable<Unit> OnValueChangedAsObservable();
    void Select();
    void Deselect();
    void Refresh();
    void ProcessEvent(Event ev);
}