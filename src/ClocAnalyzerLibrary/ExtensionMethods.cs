using System.Collections.Generic;

namespace ClocAnalyzerLibrary
{
    public static class ExtensionMethods
    {
        public static void IncrementBy<TKey>(this Dictionary<TKey, long> dictionary, TKey key, long valueToAdd)
        {
            dictionary.TryGetValue(key, out var count);
            dictionary[key] = count + valueToAdd;
        }
    }
}
