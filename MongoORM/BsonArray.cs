using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class BsonArray<T> : IEnumerable<T>, IEnumerable
    {
        private T[] Items;
        private bool[] States;

        public BsonArray(uint size)
        {
            Items = new T[size];
            States = new bool[size];
            Array.Fill(Items, default(T));
        }

        public int Count { get { return Items.Length; } }

        public void ClearState()
        {
            Array.Fill(States, false);
        }

        public bool IsModifyed(int index)
        {
            return States[index];
        }

        public void Clear()
        {
            Array.Fill(Items, default(T));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public T this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                if (Items[index].Equals(value))
                    return;

                Items[index] = value;
                States[index] = true;
            }
        }

        public class Enumerator : IEnumerator<T>
        {
            int Index = -1;
            BsonArray<T> List;

            public Enumerator(BsonArray<T> list)
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
