using System.Collections.Generic;
using System.Linq;
using System;

namespace NodeEngine
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public event EventHandler<DictionaryChangedEventArgs<TKey, TValue>> OnDictionaryChangedEvent;

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                dictionary[key] = value;
                OnDictionaryChanged(new DictionaryChangedEventArgs<TKey, TValue>(DictionaryChangeType.Update, key, value));
            }
        }

        public ICollection<TKey> Keys => dictionary.Keys;
        public ICollection<TValue> Values => dictionary.Values;
        public int Count => dictionary.Count;
        public bool IsReadOnly => ((IDictionary<TKey, TValue>)dictionary).IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            OnDictionaryChanged(new DictionaryChangedEventArgs<TKey, TValue>(DictionaryChangeType.Add, key, value));
        }

        public bool Remove(TKey key)
        {
            if (dictionary.Remove(key))
            {
                OnDictionaryChanged(new DictionaryChangedEventArgs<TKey, TValue>(DictionaryChangeType.Remove, key, default));
                return true;
            }

            return false;
        }

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public void Clear() => dictionary.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)dictionary).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        protected virtual void OnDictionaryChanged(DictionaryChangedEventArgs<TKey, TValue> e)
        {
            OnDictionaryChangedEvent?.Invoke(this, e);
        }
    }

    public class DictionaryChangedEventArgs<TKey, TValue> : EventArgs
    {
        public DictionaryChangeType ChangeType { get; }
        public TKey Key { get; }
        public TValue Value { get; }

        public DictionaryChangedEventArgs(DictionaryChangeType changeType, TKey key, TValue value)
        {
            ChangeType = changeType;
            Key = key;
            Value = value;
        }
    }

    public enum DictionaryChangeType
    {
        Add,
        Remove,
        Update
    }
}
