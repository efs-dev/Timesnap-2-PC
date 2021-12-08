using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

public class PickList<T> : MonoBehaviour
{
    public TMP_InputField SearchField;

    public IObservable<T> OnPickAsObservable => _onPickSubject;

    public Transform ContentContainer;
    public PickListRow RowPrefab;
    public ReactiveCollection<T> Data = new ReactiveCollection<T>();
    public Func<T, string> Stringifier = it => it.ToString();
    private readonly ISubject<T> _onPickSubject = new Subject<T>();

    private void Start()
    {
        SearchField.onValueChanged.AsObservable().StartWith("")
            .Merge(Data.ObserveCountChanged().Select(_ => SearchField.text))
            .Do(_ =>
            {
                foreach (Transform o in ContentContainer)
                {
                    Destroy(o.gameObject);
                }
            })
            .SelectMany(searchQuery =>
                Data.Where(it => Stringifier(it).ToLower().Contains(searchQuery.ToLower())))
            .Subscribe(datum =>
            {
                var row = Instantiate(RowPrefab, ContentContainer, false);
                row.SetText(Stringifier(datum));
                row.Button.OnClickAsObservable().Select(_ => datum).Subscribe(_onPickSubject);
            });
    }
}