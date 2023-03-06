using System.Collections.Generic;

namespace ClocAnalyzerLibrary
{
    public static class ExtensionMethods
    {
        public static void AddStatsToDict(Dictionary<string, LocStats> dictionary, LocStats stats)
        {
            var exists = dictionary.TryGetValue(stats.Type, out var dictStats);
            if (!exists)
            {
                dictStats = new LocStats { Type = stats.Type };
                dictionary.Add(stats.Type, dictStats);
            }
            dictStats.AddStats(stats);
        }
    }
}
