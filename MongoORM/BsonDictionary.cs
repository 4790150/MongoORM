using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class BsonDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        internal Dictionary<TKey, TValue> Items = new Dictionary<TKey, TValue>();
        internal HashSet<TKey> Added = new HashSet<TKey>();
        internal HashSet<TKey> Removed = new HashSet<TKey>();

        public int Count { get => Items.Count; }

        public ICollection<TKey> Keys => Items.Keys;

        public ICollection<TValue> Values => Items.Values;

        public bool IsReadOnly => false;

        internal void ClearDirty()
        {
            Added.Clear();
            Removed.Clear();
        }

        public void Clear()
        {
            foreach (var pair in Items)
            {
                if (!Added.Contains(pair.Key))
                    Removed.Add(pair.Key);
            }

            Added.Clear();
            Items.Clear();
        }

        public TValue this[TKey key]
        {
            get
            {
                return Items[key];
            }
            set
            {
                Items[key] = value;
                Added.Add(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Items.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            Items[key] = value;
            Added.Add(key);

        }

        public bool Remove(TKey key)
        {
            if (!Items.TryGetValue(key, out TValue value))
                return false;

            bool newData = Added.Contains(key);

            Items.Remove(key);     
            if (newData)
            {
                Added.Remove(key);
            }
            else
            {
                Removed.Add(key);
            }

            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return Items.ContainsKey(key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            IEnumerator<KeyValuePair<TKey, TValue>> _Enumerator;

            public Enumerator(BsonDictionary<TKey, TValue> list)
            {
                _Enumerator = list.Items.GetEnumerator();
            }

            public KeyValuePair<TKey, TValue> Current => _Enumerator.Current;

            public DictionaryEntry Entry => new DictionaryEntry(_Enumerator.Current.Key, _Enumerator.Current.Value);

            public object Key => _Enumerator.Current.Key;

            public object Value => _Enumerator.Current.Value;

            object IEnumerator.Current => _Enumerator.Current;

            public void Dispose()
            {
                _Enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _Enumerator.MoveNext();
            }

            public void Reset()
            {
                _Enumerator.Reset();
            }
        }
    }
}
