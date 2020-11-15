using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public struct BsonListElement<T>
    {
        public BsonListElement(int key, T element)
        {
            this.Key = key;
            this._PrevKey = 0;
            this._PrevKeyDirty = false;
            this._Element = element;
            this._ElementDirty = false;
        }

        public BsonListElement(int key, int prevKey, T element)
        {
            this.Key = key;
            this._PrevKey = prevKey;
            this._PrevKeyDirty = false;
            this._Element = element;
            this._ElementDirty = false;
        }

        public int Key { get; internal set; }

        private bool _PrevKeyDirty;
        private int _PrevKey;
        public int PrevKey
        {
            get => _PrevKey;
            set
            {
                if (value == _PrevKey)
                    return;

                _PrevKey = value;
                _PrevKeyDirty = true;
            }
        }

        private bool _ElementDirty;
        private T _Element;
        public T Element
        {
            get => _Element;
            set
            {
                if (value.Equals(_Element))
                    return;

                _Element = value;
                _ElementDirty = true;
            }
        }

        public bool Dirty { get => _ElementDirty; set => _ElementDirty = value; }
    }

    public class BsonList<T> : IList<T>
    {
        internal List<BsonListElement<T>> ItemList;
        internal List<BsonListElement<T>> RemovedList;

        private int KeySeed = 1;

        public BsonList() : this(4)
        {

        }

        public BsonList(IEnumerable<T> collection)
        {
            int count = 0;
            foreach (var item in collection)
                count++;

            ItemList = new List<BsonListElement<T>>(count);
            RemovedList = new List<BsonListElement<T>>();

            foreach (var item in collection)
            {
                ItemList.Add(new BsonListElement<T>(KeySeed++, ItemList.Count > 0 ? ItemList.Count : 0, item));
            }
            ClearState();
        }

        public BsonList(int capacity)
        {
            ItemList = new List<BsonListElement<T>>(capacity);
            RemovedList = new List<BsonListElement<T>>(capacity);
        }

        public T this[int index]
        {
            get
            {
                return ItemList[index].Element;
            }

            set
            {
                var item = ItemList[index];
                item.Element = value;
                ItemList[index] = item;
            }
        }

        public int Count { get => ItemList.Count; }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            ItemList.Add(new BsonListElement<T>(KeySeed++, ItemList.Count > 0 ? ItemList[ItemList.Count - 1].Key : 0, item));
        }

        public void Clear()
        {
            foreach (var element in ItemList)
            {
                if (!element.Dirty)
                {
                    RemovedList.Add(element);
                }
            }
            ItemList.Clear();
        }

        public void ClearState()
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                var item = ItemList[i];
                ItemList[i] = new BsonListElement<T>(item.Key, item.PrevKey, item.Element);
            }
        }

        public bool IsModifyed(int index)
        {
            return ItemList[index].Dirty;
        }

        public bool Contains(T item)
        {
            foreach (var element in ItemList)
            {
                if (element.Element.Equals(item))
                    return true;
            }

            return false;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Element.Equals(item))
                    return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            ItemList.Insert(index, new BsonListElement<T>(KeySeed++, index > 0 ? ItemList[index - 1].Key : 0, item));
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                var element = ItemList[i];
                if (element.Element.Equals(item))
                {
                    if (!element.Dirty)
                    {
                        RemovedList.Add(element);
                    }
                    ItemList.RemoveAt(i);

                    if (i < ItemList.Count)
                    {
                        var next = ItemList[i];
                        next.PrevKey = i > 0 ? ItemList[i - 1].Key : 0;
                    }
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            var element = ItemList[index];
            if (!element.Dirty)
            {
                RemovedList.Add(ItemList[index]);
            }
            ItemList.RemoveAt(index);

            if (index < ItemList.Count)
            {
                var next = ItemList[index];
                next.PrevKey = index > 0 ? ItemList[index - 1].Key : 0;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void CopyTo(T[] array, int arrayIndex)
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
    }
}
