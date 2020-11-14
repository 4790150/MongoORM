using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class BsonList<T> : IList<T>
    {
        struct Element
        {
            public int Key;
            public bool Pushed;
            public int Next;
        }

        public bool NewDocument { get; set; }
        List<T> ItemList;
        Link<Element> Keys;

        public BsonList() : this(4)
        {

        }

        public BsonList(IEnumerable<T> collection)
        {
            int count = 0;
            foreach (var item in collection)
                count++;

            ItemList = new List<T>(count);
            Keys = new List<Element>(count);
            NewDocument = true;

            foreach (var item in collection)
            {
                ItemList.Add(item);
                Keys.Add();
            }
        }

        public BsonList(int capacity)
        {
            ItemList = new List<T>(capacity);
            Keys = new List<int>(capacity);
            States = new List<bool>(capacity);
            NewDocument = true;
        }

        public T this[int index]
        {
            get
            {
                return ItemList[index];
            }

            set
            {
                ItemList[index] = value;
                States[index] = true;
            }
        }

        public int Count { get => ItemList.Count; }
        public int Capacity { get => ItemList.Capacity; }

        public bool IsReadOnly => false;

        public static sbyte[] GenerateKey(sbyte[] prev, sbyte[] next)
        {
            List<sbyte> result = new List<sbyte>();
            int length = Math.Max(prev.Length, next.Length);
            for (int i = 0; i < length; i++)
            {
                int prevValue = i < prev.Length ? prev[i] : sbyte.MinValue - 1;
                int nextValue = i < next.Length ? next[i] : sbyte.MaxValue + 1;

                int newValue = (prevValue + nextValue) / 2;
                if (newValue + 1 >= nextValue)
                {
                    result.Add((sbyte)Math.Max(prevValue, sbyte.MinValue));
                }
                else
                {
                    result.Add((sbyte)((prevValue + nextValue) / 2));
                    return result.ToArray();
                }

            }
            result.Add(0);
            return result.ToArray();
        }

        public void Add(T item)
        {
            ItemList.Add(item);

            int index = ItemList.Count - 1;
            Keys.Add(  );
            States.Add(true);
        }

        public void Clear()
        {
            NewDocument = true;
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
