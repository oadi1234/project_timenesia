using System.Collections.Generic;

namespace _2_Scripts.ExtensionMethods
{
    public static class DictionaryExtension
    {
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (!source.TryAdd(key, value))
                source[key] = value;
        }
    }
}