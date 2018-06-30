using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions
{
    public static class DIctionaryExtensions
    {
        public static bool TryRemove<K, V>(this Dictionary<K, V> dict, K key, V value)
        {
            var finded = dict.TryGetValue(key, out var fromDictValue);
            if (!finded)
                return false;

            if (!fromDictValue.Equals(value))
                return false;

            dict.Remove(key);
            return true;

        }
    }
}
