using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ModestTree;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace City.Utils.Components
{
    // todo: does this handle removing destroyed objects?
    public abstract class ReactiveGameObjectCollection<T> : MonoBehaviour, IReactiveCollection<T>
        where T : MonoBehaviour
    {
        public bool IsReadOnly
        {
            get { return _reactiveCollection.IsReadOnly; }
        }

        int IReactiveCollection<T>.Count
        {
            get { return _reactiveCollection.Count; }
        }

        int ICollection<T>.Count
        {
            get { return _reactiveCollection.Count; }
        }

        int IReadOnlyReactiveCollection<T>.Count
        {
            get { return _reactiveCollection.Count; }
        }

        T IList<T>.this[int index]
        {
            get { return ((IList<T>) _reactiveCollection)[index]; }
            set { ((IList<T>) _reactiveCollection)[index] = value; }
        }

        T IReactiveCollection<T>.this[int index]
        {
            get { return _reactiveCollection[index]; }
            set { _reactiveCollection[index] = value; }
        }

        T IReadOnlyReactiveCollection<T>.this[int index]
        {
            get { return ((IReadOnlyReactiveCollection<T>) _reactiveCollection)[index]; }
        }

        private readonly IReactiveCollection<T> _reactiveCollection = new ReactiveCollection<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _reactiveCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _reactiveCollection).GetEnumerator();
        }

        public T PopOrThrow()
        {
            if (_reactiveCollection.Count == 0)
            {
                throw new InvalidOperationException("Can't pop from emptry collection");
            }

            return PopOrNull();
        }

        /// <summary>
        /// Removes the first element from this collection and reparent's it into the Collection's parent
        /// </summary>
        /// <returns>The first element or null if the collection is empty</returns>
        [CanBeNull]
        public T PopOrNull()
        {
            if (_reactiveCollection.Count == 0)
            {
                return null;
            }

            var el = _reactiveCollection[0];
            _reactiveCollection.RemoveAt(0);
            el.transform.SetParent(transform.parent);
            return el;
        }


        public virtual void Add(T item)
        {
//            Debug.Log("ReactiveListComponent<" + typeof(T) + ">::Add" + item);
            if (item == null) return;
            item.transform.SetParent(transform);
            item.transform.SetAsLastSibling();
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            _reactiveCollection.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            foreach (var o in _reactiveCollection.Select(x => x.gameObject))
            {
                Destroy(o);
            }

            _reactiveCollection.Clear();
        }

        public bool Contains(T item)
        {
            return _reactiveCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _reactiveCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var result = _reactiveCollection.Remove(item);

            // Really checking if it's destroyed here
            if (item)
            {
                Destroy(item.gameObject);
            }

            return result;
        }

        public void Move(int oldIndex, int newIndex)
        {
            _reactiveCollection[oldIndex].transform.SetSiblingIndex(newIndex);
            _reactiveCollection.Move(oldIndex, newIndex);
        }

        public IObservable<CollectionAddEvent<T>> ObserveAdd()
        {
            return _reactiveCollection.ObserveAdd();
        }

        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            return _reactiveCollection.ObserveCountChanged(notifyCurrentCount);
        }

        public IObservable<CollectionMoveEvent<T>> ObserveMove()
        {
            return _reactiveCollection.ObserveMove();
        }

        public IObservable<CollectionRemoveEvent<T>> ObserveRemove()
        {
            return _reactiveCollection.ObserveRemove();
        }

        public IObservable<CollectionReplaceEvent<T>> ObserveReplace()
        {
            return _reactiveCollection.ObserveReplace();
        }

        public IObservable<Unit> ObserveReset()
        {
            return _reactiveCollection.ObserveReset();
        }

        public int IndexOf(T item)
        {
            return _reactiveCollection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            item.transform.SetParent(transform);
            item.transform.SetSiblingIndex(index);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            _reactiveCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Destroy(_reactiveCollection[index].gameObject);
            _reactiveCollection.RemoveAt(index);
        }
    }
}