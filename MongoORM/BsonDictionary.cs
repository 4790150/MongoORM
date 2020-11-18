using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MongoORM
{
    public class BsonDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        internal Dictionary<TKey, TValue> internalItems = new Dictionary<TKey, TValue>();
        internal HashSet<TKey> internalAdded = new HashSet<TKey>();
        internal HashSet<TKey> internalRemoved = new HashSet<TKey>();

        public int Count { get => internalItems.Count; }

        public ICollection<TKey> Keys => internalItems.Keys;

        public ICollection<TValue> Values => internalItems.Values;

        public bool IsReadOnly => false;

        internal void ClearDirty()
        {
            internalAdded.Clear();
            internalRemoved.Clear();
        }

        public void Clear()
        {
            foreach (var pair in internalItems)
            {
                if (!internalAdded.Contains(pair.Key))
                    internalRemoved.Add(pair.Key);
            }

            internalAdded.Clear();
            internalItems.Clear();
        }

        public TValue this[TKey key]
        {
            get
            {
                return internalItems[key];
            }
            set
            {
                internalItems[key] = value;
                internalAdded.Add(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return internalItems.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            internalItems[key] = value;
            internalAdded.Add(key);

        }

        public bool Remove(TKey key)
        {
            if (!internalItems.TryGetValue(key, out TValue value))
                return false;

            bool newData = internalAdded.Contains(key);

            internalItems.Remove(key);     
            if (newData)
            {
                internalAdded.Remove(key);
            }
            else
            {
                internalRemoved.Add(key);
            }

            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return internalItems.ContainsKey(key);
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
                _Enumerator = list.internalItems.GetEnumerator();
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
