using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MongoORM
{
    public class BsonList<T> : IList<T>
    {
        private const int _defaultCapacity = 4;
        private static readonly Element[] _emptyArray = new Element[0];

        internal int _KeySeed = 0;

        public int Count { get; private set; }

        internal Element[] internalItems;
        internal List<int> internalRemoved;

        public BsonList()
        {
            internalItems = _emptyArray;
            internalRemoved = new List<int>();
        }

        public BsonList(IEnumerable<T> collection)
        {
            if (collection is ICollection<T> c)
            {
                if (0 == c.Count)
                {
                    internalItems = _emptyArray;
                }
                else
                {
                    internalItems = new Element[c.Count];
                }
                Capacity = c.Count;
            }
            else
            {
                internalItems = _emptyArray;
                Capacity = 0;
            }

            internalRemoved = new List<int>();

            foreach (var item in collection)
            {
                internalItems[Count++] = new Element(++_KeySeed, Count, item);
            }
            ClearDirty();
        }

        public BsonList(int capacity)
        {
            this.Capacity = capacity;
            internalItems = new Element[capacity];
            internalRemoved = new List<int>();
        }

        public T this[int index]
        {
            get
            {
                return internalItems[index].Value;
            }

            set
            {
                var item = internalItems[index];
                item.Value = value;
                internalItems[index] = item;
            }
        }

        public int Capacity {
            get => internalItems.Length;
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException("Capacity");

                if (value != internalItems.Length)
                {
                    if (value > 0)
                    {
                        Element[] newList = new Element[value];
                        if (Count > 0)
                            Array.Copy(internalItems, 0, newList, 0, Count);
                        internalItems = newList;
                    }
                    else
                    {
                        internalItems = _emptyArray;
                    }
                }
            }
        }

        private void EnsureCapacity(int min)
        {
            if (internalItems.Length < min)
            {
                int newCapacity = internalItems.Length == 0 ? _defaultCapacity : internalItems.Length * 2;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }


        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (Count == internalItems.Length)
                EnsureCapacity(Count + 1);

            internalItems[Count] = new Element(++_KeySeed, Count > 0 ? internalItems[Count - 1].Key : 0, item);
            Count++;
        }

        internal void internalAdd(Element element)
        {
            if (Count == internalItems.Length)
                EnsureCapacity(Count + 1);

            if (element.Key > _KeySeed)
                _KeySeed = element.Key;

            internalItems[Count++] = element;
        }

        internal void internalPostDeserialize()
        {
            int prevKey = 0;
            for (int i = 0; i < Count; i++)
            {
                if (internalItems[i].PrevKey == prevKey)
                {
                    prevKey = internalItems[i].Key;
                    continue;
                }

                for (int j = i + 1; j < Count; j++)
                {
                    if (internalItems[j].PrevKey == prevKey)
                    {
                        var temp = internalItems[j];
                        internalItems[j] = internalItems[i];
                        internalItems[i] = temp;
                        break;
                    }
                }

                prevKey = internalItems[i].Key;
            }
        }

        public void Clear()
        {
            if (Count > 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (!internalItems[i].NewData)
                    {
                        internalRemoved.Add(internalItems[i].Key);
                    }
                }

                Array.Clear(internalItems, 0, Count);
                Count = 0;
            }
        }

        internal void ClearDirty()
        {
            for (int i = 0; i < Count; i++)
            {
                internalItems[i].ClearDirty();
            }
        }

        public bool Contains(T item)
        {
            foreach (var element in internalItems)
            {
                if (element.Value.Equals(item))
                    return true;
            }

            return false;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (internalItems[i].Value.Equals(item))
                    return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index > Count)
                throw new ArgumentOutOfRangeException("index");

            if (Count == internalItems.Length)
                EnsureCapacity(Count + 1);

            if (index < Count)
                Array.Copy(internalItems, index, internalItems, index + 1, Count - index);

            internalItems[index] = new Element(++_KeySeed, index > 0 ? internalItems[index - 1].Key : 0, item);
            Count++;

            if (index < Count - 1)
                internalItems[index + 1].PrevKey = _KeySeed;
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                var element = internalItems[i];
                if (element.Value.Equals(item))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            var element = internalItems[index];
            if (!element.NewData)
            {
                internalRemoved.Add(element.Key);
            }

            Count--;
            if (index < Count)
            { 
                Array.Copy(internalItems, index + 1, internalItems, index, Count - index);

                internalItems[index].PrevKey = index > 0 ? internalItems[index - 1].Key : 0;
            }

            internalItems[Count] = default(Element);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public class Enumerator : IEnumerator<T>
        {
            int Index = -1;
            BsonList<T> List;

            public Enumerator(BsonList<T> list)
            {
                this.List = list;
            }

            public T Current => List[Index];

            object IEnumerator.Current => List[Index];

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return ++Index < List.Count;
            }

            public void Reset()
            {
                Index = -1;
            }
        }

        public struct Element
        {
            private const int NewTag = 1;
            private const int PrevTag = 1 << 1;
            private const int ValueTag = 1 << 2;
            private int _DataDirty;

            public Element(int key, int prevKey, T value)
            {
                this.Key = key;
                this._PrevKey = prevKey;
                this._Value = value;
                this._DataDirty = NewTag;
            }

            public int Key { get; internal set; }

            internal bool PrevKeyDirty => 0 != (_DataDirty & PrevTag);
            private int _PrevKey;
            public int PrevKey
            {
                get => _PrevKey;
                set
                {
                    if (value == _PrevKey)
                        return;

                    _PrevKey = value;
                    _DataDirty |= PrevTag;
                }
            }

            internal bool ValueDirty => 0 != (_DataDirty & ValueTag);
            private T _Value;
            public T Value
            {
                get => _Value;
                set
                {
                    if (value.Equals(_Value))
                        return;

                    _Value = value;
                    _DataDirty |= ValueTag;
                }
            }

            public bool DataDirty { get => 0 != _DataDirty; }

            public bool NewData
            {
                get => 0 != (_DataDirty & NewTag);
                set => _DataDirty |= NewTag;
            }

            public void ClearDirty()
            {
                _DataDirty = 0;
            }
        }
    }
}
