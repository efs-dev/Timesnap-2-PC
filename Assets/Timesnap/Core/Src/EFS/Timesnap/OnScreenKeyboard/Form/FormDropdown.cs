using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FormDropdown : TMP_Dropdown, IFormElement
{
    public OptionData CurrentlySelectedOption => options[value];

    public IObservable<Unit> OnSelectAsObservable()
    {
        return ((UIBehaviour) this).OnSelectAsObservable().AsUnitObservable();
    }

    public IObservable<Unit> OnDeselectAsObservable()
    {
        return ((UIBehaviour) this).OnDeselectAsObservable().AsUnitObservable();
    }

    public IObservable<Unit> OnValueChangedAsObservable()
    {
        return Observable.Create<Unit>(observer =>
        {
            UnityAction<int> listener = _ => observer.OnNext(Unit.Default);
            onValueChanged.AddListener(listener);
            return Disposable.Create(() => onValueChanged.RemoveListener(listener));
        });
    }

    public void Deselect()
    {
        OnDeselect(new BaseEventData(EventSystem.current));
    }

    public void Refresh()
    {
        Debug.Log("dropdown refresh");
    }

    public void ProcessEvent(Event ev)
    {
        Debug.Log("dropdown processevent");
    }
}