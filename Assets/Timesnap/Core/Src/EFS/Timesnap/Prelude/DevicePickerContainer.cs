using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EFS.Timesnap
{
    public class DevicePickerContainer : MonoBehaviour
    {
        public Button ButtonPrefab;

        public IObservable<Tuple<string, Color>> Picks
        {
            get { return _picks.AsObservable(); }
        }

        private readonly ISubject<Tuple<string, Color>> _picks = new Subject<Tuple<string, Color>>();

        public void Add(string id, Color color)
        {
            var button = Instantiate(ButtonPrefab, transform, false);
            button.gameObject.name = id;
            button.GetComponent<Image>().color = color;
//            button.transform.position = Vector3.zero;
            button.GetComponentInChildren<TMP_Text>().text = button.transform.GetSiblingIndex().ToString();
            Canvas.ForceUpdateCanvases();
            button.OnClickAsObservable().AsUnitObservable().Merge(Observable.ReturnUnit())
                .Select(_ => new Tuple<string, Color>(id, color)).Subscribe(_picks);
        }

        public void Clear()
        {
            foreach (Transform o in transform)
            {
                Destroy(o.gameObject);
            }
        }
    }
}