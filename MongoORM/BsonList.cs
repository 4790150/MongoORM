using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class BsonList<T> : IList<T>
    {
        private const int _defaultCapacity = 4;
        private static Element[] _emptyArray = new Element[0];

        internal int KeySeed = 0;

        public int Count { get; private set; }

        internal Element[] ItemList;
        internal List<int> Removed;

        public BsonList()
        {
            ItemList = _emptyArray;
            Removed = new List<int>();
        }

        public BsonList(IEnumerable<T> collection)
        {
            if (collection is ICollection<T> c)
            {
                if (0 == c.Count)
                {
                    ItemList = _emptyArray;
                }
                else
                {
                    ItemList = new Element[c.Count];
                }
                Capacity = c.Count;
            }
            else
            {
                ItemList = _emptyArray;
                Capacity = 0;
            }

            Removed = new List<int>();

            foreach (var item in collection)
            {
                ItemList[Count++] = new Element(++KeySeed, Count, item);
            }
            ClearDirty();
        }

        public BsonList(int capacity)
        {
            this.Capacity = capacity;
            ItemList = new Element[capacity];
            Removed = new List<int>();
        }

        public T this[int index]
        {
            get
            {
                return ItemList[index].Value;
            }

            set
            {
                var item = ItemList[index];
                item.Value = value;
                ItemList[index] = item;
            }
        }

        public int Capacity {
            get => ItemList.Length;
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException("Capacity");

                if (value != ItemList.Length)
                {
                    if (value > 0)
                    {
                        Element[] newList = new Element[value];
                        if (Count > 0)
                            Array.Copy(ItemList, 0, newList, 0, Count);
                        ItemList = newList;
                    }
                    else
                    {
                        ItemList = _emptyArray;
                    }
                }
            }
        }

        private void EnsureCapacity(int min)
        {
            if (ItemList.Length < min)
            {
                int newCapacity = ItemList.Length == 0 ? _defaultCapacity : ItemList.Length * 2;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }


        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (Count == ItemList.Length)
                EnsureCapacity(Count + 1);

            ItemList[Count] = new Element(++KeySeed, Count > 0 ? ItemList[Count - 1].Key : 0, item);
            Count++;
        }

        internal void Add(Element element)
        {
            if (Count == ItemList.Length)
                EnsureCapacity(Count + 1);

            if (element.Key > KeySeed)
                KeySeed = element.Key;

            ItemList[Count++] = element;
        }

        internal void PostDeserialize()
        {
            int prevKey = 0;
            for (int i = 0; i < Count; i++)
            {
                if (ItemList[i].PrevKey == prevKey)
                {
                    prevKey = ItemList[i].Key;
                    continue;
                }

                for (int j = i + 1; j < Count; j++)
                {
                    if (ItemList[j].PrevKey == prevKey)
                    {
                        var temp = ItemList[j];
                        ItemList[j] = ItemList[i];
                        ItemList[i] = temp;
                        break;
                    }
                }

                prevKey = ItemList[i].Key;
            }
        }

        public void Clear()
        {
            if (Count > 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (!ItemList[i].NewData)
                    {
                        Removed.Add(ItemList[i].Key);
                    }
                }

                Array.Clear(ItemList, 0, Count);
                Count = 0;
            }
        }

        internal void ClearDirty()
        {
            for (int i = 0; i < Count; i++)
            {
                ItemList[i].ClearDirty();
            }
        }

        public bool Contains(T item)
        {
            foreach (var element in ItemList)
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
                if (ItemList[i].Value.Equals(item))
                    return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index > Count)
                throw new ArgumentOutOfRangeException("index");

            if (Count == ItemList.Length)
                EnsureCapacity(Count + 1);

            if (index < Count)
                Array.Copy(ItemList, index, ItemList, index + 1, Count - index);

            ItemList[index] = new Element(++KeySeed, index > 0 ? ItemList[index - 1].Key : 0, item);
            Count++;

            if (index < Count - 1)
                ItemList[index + 1].PrevKey = KeySeed;
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                var element = ItemList[i];
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
            var element = ItemList[index];
            if (!element.NewData)
            {
                Removed.Add(element.Key);
            }

            Count--;
            if (index < Count)
            { 
                Array.Copy(ItemList, index + 1, ItemList, index, Count - index);

                ItemList[index].PrevKey = index > 0 ? ItemList[index - 1].Key : 0;
            }

            ItemList[Count] = default(Element);
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
