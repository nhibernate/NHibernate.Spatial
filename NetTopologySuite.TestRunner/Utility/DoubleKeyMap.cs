﻿using System.Collections.Generic;

namespace Open.Topology.TestRunner.Utility
{
    public class DoubleKeyMap<TKey1, TKey2, TValue>
    {
        internal IDictionary<TKey1, IDictionary<TKey2, TValue>> Data = new Dictionary<TKey1, IDictionary<TKey2, TValue>>();

        public virtual TValue Put(TKey1 k1, TKey2 k2, TValue v)
        {
            Data.TryGetValue(k1, out var data2);
            var prev = default(TValue);
            if (data2 == null)
            {
                data2 = new Dictionary<TKey2, TValue>();
                Data[k1] = data2;
            }
            else
            {
                data2.TryGetValue(k2, out prev);
            }
            data2[k2] = v;
            return prev;
        }

        public virtual TValue Get(TKey1 k1, TKey2 k2)
        {
            Data.TryGetValue(k1, out var data2);
            if (data2 == null)
            {
                return default(TValue);
            }

            data2.TryGetValue(k2, out var value);
            return value;
        }

        public virtual IDictionary<TKey2, TValue> Get(TKey1 k1)
        {
            Data.TryGetValue(k1, out var value);
            return value;
        }

        /** Get all values associated with primary key */
        public virtual ICollection<TValue> Values(TKey1 k1)
        {
            Data.TryGetValue(k1, out var data2);
            if (data2 == null)
            {
                return null;
            }

            return data2.Values;
        }

        /** get all primary keys */
        public virtual ICollection<TKey1> KeySet()
        {
            return Data.Keys;
        }

        /** get all secondary keys associated with a primary key */
        public virtual ICollection<TKey2> KeySet(TKey1 k1)
        {
            Data.TryGetValue(k1, out var data2);
            if (data2 == null)
            {
                return null;
            }

            return data2.Keys;
        }

        public virtual ICollection<TValue> Values()
        {
            var s = new List<TValue>();
            foreach (var k2 in Data.Values)
            {
                foreach (var v in k2.Values)
                {
                    s.Add(v);
                }
            }
            return s;
        }
    }
}
