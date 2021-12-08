using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// note: add VRFormElement if you want the keyboard to work
public class FormInput : TMP_InputField, IFormElement
{
    private void Start()
    {
        base.Start();
    }

    private void LateUpdate()
    {
        //James did this!!!
        /*
        if (m_Keyboard != null && m_Keyboard.active)
        {
            m_Keyboard.active = false;
        }
        */
    }

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
            UnityAction<string> listener = _ => { observer.OnNext(Unit.Default); };
            onValueChanged.AddListener(listener);
            return Disposable.Create(() => onValueChanged.RemoveListener(listener));
        });
    }

    public IObservable<Unit> OnPointerUpAsObservable()
    {
        return ((UIBehaviour) this).OnPointerUpAsObservable().AsUnitObservable();
    }

    public void Deselect()
    {
        OnDeselect(new BaseEventData(EventSystem.current));
    }

    public void Refresh()
    {
        ForceLabelUpdate();
    }

    public new void ProcessEvent(Event ev)
    {
        // this is a hack
        // previously we just passed the event up, but then we had some problems on the phone. Hmm
        if (ev.keyCode == KeyCode.Backspace && text.Length >= 1)
        {
            text = text.Substring(0, text.Length - 1);
        }
        else if (!char.IsControl(ev.character))
        {
            text += ev.character;
        }
        else
        {
            print("received control character");
        }

        print("new text: [" + text + "]");
    }
}