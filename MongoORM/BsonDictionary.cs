using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class BsonDictionary<TKey, TValue> 
    {
        private Dictionary<TKey, UpdateState> States = new Dictionary<TKey, UpdateState>();
        private Dictionary<TKey, TValue> Items = new Dictionary<TKey, TValue>();

        public int Count { get; protected set; }

        public void ClearState()
        {
            States.Clear();
            Count = Items.Count;
        }

        public void Clear()
        {
            States.Clear();
            Items.Clear();
            Count = 0;
        }

        public TValue this[TKey key]
        {
            get
            {
                return Items[key];
            }
            set
            {
                if (!Items.ContainsKey(key))
                    Count++;

                Items[key] = value;
                States[key] = UpdateState.Set;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Items.TryGetValue(key, out value);
        }

        public bool TryGetValue(TKey key, out TValue value, out UpdateState state)
        {
            States.TryGetValue(key, out state);
            return Items.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value, bool dbReplicated = false)
        {
            if (!Items.ContainsKey(key))
                Count++;

            Items[key] = value;
            if (dbReplicated)
            {
                States[key] = UpdateState.Set;
            }
            else
            {
                States.Remove(key);
            }
        }

        public bool Remove(TKey key)
        {
            if (!Items.TryGetValue(key, out TValue value))
                return false;

            Count--;
            if (!States.TryGetValue(key, out UpdateState state))
                state = UpdateState.None;

            if (UpdateState.Set == state)
            {
                States.Remove(key);
                Items.Remove(key);
            }
            else if (UpdateState.None == state)
            {
                States[key] = UpdateState.Unset;
            }
            return true;
        }

        public void Foreach(UpdateState filter, Action<TKey, TValue, UpdateState> action)
        {
            foreach (var pair in Items)
            {
                if (!States.TryGetValue(pair.Key, out UpdateState state))
                    state = UpdateState.None;

                if ((filter & state) != 0)
                    action(pair.Key, pair.Value, state);
            }
        }

        public void SetState(TKey key, UpdateState state)
        {
            if (!Items.ContainsKey(key))
                return;

            if (!States.TryGetValue(key, out UpdateState oldState))
                oldState = UpdateState.None;

            if (state == oldState) return;

            if (UpdateState.Set == oldState && UpdateState.Unset == state)
            {
                States.Remove(key);
                Items.Remove(key);
                Count--;
            }
            else if (UpdateState.None == oldState && UpdateState.Unset == state)
            {
                States[key] = state;
                Count--;
            }
            else if (UpdateState.Unset == oldState)
            {
                States[key] = state;
                Count++;
            }
            else
            {
                States[key] = state;
            }
        }
    }
}
